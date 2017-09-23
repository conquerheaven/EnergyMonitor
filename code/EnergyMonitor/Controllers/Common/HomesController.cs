using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Controllers.User.Filters;
using System.Collections;
using EnergyMonitor.Controllers.Utils;
using EnergyMonitor.Models.Repository.Implement;
using System.Threading;


namespace EnergyMonitor.Controllers.Common
{
    /// <summary>
    /// 首页类
    /// </summary>
    /// <author>WangWei</author>
    public class HomesController : Controller
    {
        private IUserRepos _userRepos = null;
        private IFunctionRepos _functionRepos = null;
        private IRoomRepos _roomRepos = null;

        public HomesController()
            : this(new UserRepos(), new FunctionRepos(), new RoomRepos())
        {
        }

        public HomesController(IUserRepos userRepos, IFunctionRepos functionRepos, IRoomRepos roomRepos)
        {
            _userRepos = userRepos;
            _functionRepos = functionRepos;
            _roomRepos = roomRepos;
        }

        /// <summary>
        /// 用户首页
        /// </summary>
        /// <returns></returns>
        [UserFilter]
        public ActionResult Welcome()
        {
            return View();
        }

        /// <summary>
        /// 跳转登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            DateTime timeStamp = DateTime.Now;
            Session["timeStamp"] = timeStamp;
            string timeStampStr = timeStamp.Ticks.ToString();
            return Redirect(Util.GetConfigValue("uisLoginUrl") + timeStampStr);
        }

        /// <summary>
        /// 测试登录 (临时)
        /// </summary>
        /// <returns></returns>
        public ActionResult Index2()
        {
            return View("Login");
        }

        /// <summary>
        /// 检查用户UIS登录
        /// </summary>
        /// <param name="param">登录后的密文</param>
        /// <returns></returns>
        public ActionResult CheckLogin(string param)
        {
            if (Session["loginUser"] == null)//没有登录
            {
                DateTime? timeStamp = Session["timeStamp"] as DateTime?;
                if (!timeStamp.HasValue)//该时间戳已经被使用登录过，重新生成时间戳跳转登录
                {
                    timeStamp = DateTime.Now;
                    Session["timeStamp"] = timeStamp;
                    string timeStampStr = timeStamp.Value.Ticks.ToString();
                    return Redirect(Util.GetConfigValue("uisLoginUrl") + timeStampStr);
                }
                double? loginExpired = Util.GetConfigValueObj("loginExpired") as double?;
                if (loginExpired.HasValue)
                {
                    DateTime now = DateTime.Now;
                    if (timeStamp.Value.AddMinutes(loginExpired.Value) < now)//登录时效过期
                    {
                        Session["timeStamp"] = now;

                        return Redirect(Util.GetConfigValue("uisLoginUrl") + now.Ticks.ToString());
                    }
                }
                string userID = Util.DecryptStr(param, timeStamp.Value.Ticks.ToString());
                if (userID != null)//UIS登录验证成功
                {
                    Session["timeStamp"] = null;//防止多次利用该时间戳登录
                    var user = _userRepos.GetUserByID(userID);
                    if (user != null) //已经登录过的用户
                    {
                        if (!String.IsNullOrWhiteSpace(user.USR_MailActiveCode)) //邮箱没有激活
                        {
                            TempData["tempLoginUser"] = user;
                            return View("Reactive");
                        }
                        if (!user.USR_Status)//用户被禁用
                        {
                            return View("Disabled");
                        }
                        Session["loginUser"] = user;
                        Session["loginUserName"] = user.USR_Name;
                        Session["userRoomInfo"] = null;

                        // 记录用户登录时间IP和登录时间
                        DateTime loginTime = DateTime.Now;
                        Session["loginTime"] = loginTime;
                        string loginIP;
                        if (HttpContext.Request.ServerVariables["HTTP_VIA"] != null)
                        {
                            loginIP = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                        }
                        else
                        {
                            loginIP = HttpContext.Request.ServerVariables["REMOTE_ADDR"].ToString();
                        }
                        Session["loginIP"] = loginIP;
                        if (user != null)
                        {
                            _userRepos.ModifyUserInfo(user.USR_ID, loginTime.ToString(), loginIP);
                        }

                        if (user.USR_RoleID == 1)//先写死为1认为是学生用户
                        {
                            return RedirectToAction("Welcome", "Home", new { area = "User" });
                        }
                        else//非学生用户
                        {
                            //获取权限
                            var functionList = _functionRepos.GetRoleFuns(user.USR_RoleID);
                            var functionLinkList = functionList.Where(x => x.FN_ID.Length > 6).Select(x => x.FN_LinkLocation).ToList();
                            var navFunctionList = functionList.Where(x => x.FN_ID.Length == 6).ToList();
                            var subFunctionList = functionList.Where(x => x.FN_ID.Length == 9).ToList();
                            var fullSubFunctionList = functionList.Where(x => x.FN_ID.Length > 6).ToList();
                            Session["userFunctionLinks"] = functionLinkList;
                            Session["userNavFunctions"] = navFunctionList;
                            Session["userSubFunctions"] = subFunctionList;
                            Session["fullSubFunctionList"] = fullSubFunctionList;

                            return RedirectToAction("Welcome", "Home", new { area = "Admin" });
                        }
                    }
                    else //新用户
                    {
                        Session["userID"] = userID;
                        Session["userLoginTime"] = DateTime.Now;
                        return RedirectToAction("ForwardAddInfo", "User", new { area = "User" });
                    }
                }
                else//验证错误，重新跳转UIS登录
                {
                    return RedirectToAction("NoLogin");
                }
            }
            else//已经登录
            {
                var user = Session["loginUser"] as EnergyMonitor.Models.LinqEntity.User;
                if (user.USR_RoleID == 1)//先写死为1认为是学生用户
                {
                    return RedirectToAction("Welcome", "Home", new { area = "User" });
                }
                else//非学生用户
                {
                    return RedirectToAction("Welcome", "Home", new { area = "Admin" });
                }
            }
        }

        /// <summary>
        /// 重新发送激活邮件
        /// </summary>
        /// <returns></returns>
        public ActionResult Reactive()
        {
            if (TempData["tempLoginUser"] == null)
            {
                return View("Error");
            }
            EnergyMonitor.Models.LinqEntity.User user = TempData["tempLoginUser"] as EnergyMonitor.Models.LinqEntity.User;
            string activeStr = Guid.NewGuid().ToString();
            string webSiteName = "http://" + Request.Url.Authority;
            new Thread(delegate()//线程发送
            {
                Util.SendActiveMail(webSiteName, user.USR_Mail, user.USR_ID, user.USR_Name, "/Homes/ActiveMail", activeStr);
            }).Start();
            _userRepos.ModifyMail(user.USR_ID, user.USR_Mail, activeStr);
            return View("ReactiveResult");
        }

        /// <summary>
        /// 测试登录验证 (临时)
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns>验证成功返回用户首页，否则返回登录页</returns>
        [UserFilter] // 临时加权限不给使用
        public ActionResult Login(string userID)
        {
            if (Session["loginUser"] == null)//是否已经登录
            {
                if (String.IsNullOrEmpty(userID))
                {
                    ViewBag.loginError = "你没有填写账号";
                    return View("Login");
                }
                var user = _userRepos.GetUserByID(userID);
                
                if (user != null)
                {
                    if (!String.IsNullOrWhiteSpace(user.USR_MailActiveCode)) //邮箱没有激活
                    {
                        TempData["tempLoginUser"] = user;
                        return View("Reactive");
                    }
                    if (!user.USR_Status)//用户被禁用
                    {
                        return View("Disabled");
                    }
                    Session["loginUser"] = user;
                    Session["loginUserName"] = user.USR_Name;
                    Session["userRoomInfo"] = null;

                    // 记录用户登录时间IP和登录时间
                    DateTime loginTime = DateTime.Now;
                    Session["loginTime"] = loginTime;
                    string loginIP;
                    if (HttpContext.Request.ServerVariables["HTTP_VIA"] != null)
                    {
                        loginIP = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                    }
                    else
                    {
                        loginIP = HttpContext.Request.ServerVariables["REMOTE_ADDR"].ToString();
                    }
                    Session["loginIP"] = loginIP;
                    if (user != null)
                    {
                        _userRepos.ModifyUserInfo(user.USR_ID, loginTime.ToString(), loginIP);
                    }

                    if (user.USR_RoleID == 1)//先写死为1认为是学生用户
                    {
                        return RedirectToAction("Welcome", "Home", new { area = "User" });   
                        //return RedirectToRoute("~/User/Home/Welcome");
                    }
                    else//非学生用户
                    {
                        //获取权限
                        var functionList = _functionRepos.GetRoleFuns(user.USR_RoleID);
                        var functionLinkList = functionList.Where(x => x.FN_ID.Length > 6).Select(x => x.FN_LinkLocation).ToList();
                        var navFunctionList = functionList.Where(x => x.FN_ID.Length == 6).ToList();
                        var subFunctionList = functionList.Where(x => x.FN_ID.Length == 9).ToList();
                        var fullSubFunctionList = functionList.Where(x => x.FN_ID.Length > 6).ToList();
                        Session["userFunctionLinks"] = functionLinkList;
                        Session["userNavFunctions"] = navFunctionList;
                        Session["userSubFunctions"] = subFunctionList;
                        Session["fullSubFunctionList"] = fullSubFunctionList;

                        return RedirectToAction("Welcome", "Home", new { area = "Admin" });
                    }
                }
                else // 用户不存在
                {
                    Session["userID"] = userID;
                    Session["userLoginTime"] = DateTime.Now;
                    return RedirectToAction("ForwardAddInfo", "User", new { area = "User" });
                }
            }
            else
            {

                var user = Session["loginUser"] as EnergyMonitor.Models.LinqEntity.User;
                if (user.USR_RoleID == 1)//先写死为1认为是学生用户
                {
                    return RedirectToAction("Welcome", "Home", new { area = "User" });
                }
                else//非学生用户
                {
                    return RedirectToAction("Welcome", "Home", new { area = "Admin" });
                }
            }
        }

        /// <summary>
        /// 临时验证账号密码登录(给微信平台使用)
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ActionResult LoginForMobile(string userID, string password, string loginIP)
        {
            string result = null;
            //if (String.IsNullOrEmpty(userID) || String.IsNullOrEmpty(password) || String.IsNullOrEmpty(password))
            //{
            //    var parameterNull = new
            //    {
            //        result = "parameterNull"
            //    };
            //    return Json(parameterNull, JsonRequestBehavior.AllowGet);
            //}
            string encryptPassword = Util.SHA1Encrypt(userID + password);
            var user = _userRepos.GetUserByID(userID);
            if (user != null) // 用户存在
            {
                // 使用USR_Remark作为用户密码
                // 密码不正确
                if (encryptPassword != user.USR_Remark)
                {
                    var passwordErrror = new
                    {
                        result = "passwordErrror"
                    };
                    return Json(passwordErrror, JsonRequestBehavior.AllowGet);
                }

                if (!String.IsNullOrWhiteSpace(user.USR_MailActiveCode)) //邮箱没有激活
                { 
                    var emailNotActive = new
                    {  
                        result = "emailNotActive"
                    };
                    return Json(emailNotActive, JsonRequestBehavior.AllowGet);
                }
                if (!user.USR_Status)//用户被禁用
                {
                    var userDisabled = new
                    {
                        result = "userDisabled"
                    };
                    return Json(userDisabled, JsonRequestBehavior.AllowGet);
                }
                DateTime loginTime = DateTime.Now;
                _userRepos.ModifyUserInfo(userID, loginTime.ToString(), loginIP);
                var userInfo = new
                {
                    roomID = _roomRepos.GetRoomByUserID(userID),
                    useID = user.USR_ID.ToString(),
                    result = result
                };
                 return Json(userInfo, JsonRequestBehavior.AllowGet);
            }
            return null;// 用户不存在                   
        }

        /// <summary>
        /// 临时验证账号密码登录
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ActionResult Login2(string userID, string password)
        {
            if (Session["loginUser"] == null) // 是否已经登录
            {
                if (String.IsNullOrEmpty(userID) || String.IsNullOrEmpty(password))
                {
                    ViewBag.loginError = "账号或者密码不能为空";
                    return View("Login");
                }
                // 加密后的密码
                string encryptPassword = Util.SHA1Encrypt(userID + password);
                var user = _userRepos.GetUserByID(userID);

                if (user != null) // 用户存在
                {
                    // 使用USR_Remark作为用户密码
                    // 密码不正确
                    if (encryptPassword != user.USR_Remark)
                    {
                        ViewBag.loginError = "密码不正确";
                        return View("Login");
                    }

                    if (!String.IsNullOrWhiteSpace(user.USR_MailActiveCode)) //邮箱没有激活
                    {
                        TempData["tempLoginUser"] = user;
                        return View("Reactive");
                    }
                    if (!user.USR_Status)//用户被禁用
                    {
                        return View("Disabled");
                    }
                    Session["loginUser"] = user;
                    Session["loginUserName"] = user.USR_Name;
                    Session["userRoomInfo"] = null;

                    // 记录用户登录时间IP和登录时间
                    DateTime loginTime = DateTime.Now;
                    Session["loginTime"] = loginTime;
                    string loginIP;
                    if (HttpContext.Request.ServerVariables["HTTP_VIA"] != null)
                    {
                        loginIP = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                    }
                    else
                    {
                        loginIP = HttpContext.Request.ServerVariables["REMOTE_ADDR"].ToString();
                    }
                    Session["loginIP"] = loginIP;
                    if (user != null)
                    {
                        _userRepos.ModifyUserInfo(user.USR_ID, loginTime.ToString(), loginIP);
                    }

                    if (user.USR_RoleID == 1)//先写死为1认为是学生用户
                    {
                        return RedirectToAction("Welcome", "Home", new { area = "User" });
                        //return RedirectToRoute("~/User/Home/Welcome");
                    }
                    else//非学生用户
                    {
                        //获取权限
                        var functionList = _functionRepos.GetRoleFuns(user.USR_RoleID);
                        var functionLinkList = functionList.Where(x => x.FN_ID.Length > 6).Select(x => x.FN_LinkLocation).ToList();
                        var navFunctionList = functionList.Where(x => x.FN_ID.Length == 6).ToList();
                        var subFunctionList = functionList.Where(x => x.FN_ID.Length == 9).ToList();
                        var fullSubFunctionList = functionList.Where(x => x.FN_ID.Length > 6).ToList();
                        Session["userFunctionLinks"] = functionLinkList;
                        Session["userNavFunctions"] = navFunctionList;
                        Session["userSubFunctions"] = subFunctionList;
                        Session["fullSubFunctionList"] = fullSubFunctionList;

                        return RedirectToAction("Welcome", "Home", new { area = "Admin" });
                    }
                }
                else // 用户不存在
                {
                    ViewBag.loginError = "账号不存在";
                    return View("Login");
                    //Session["userID"] = userID;
                    //Session["userLoginTime"] = DateTime.Now;
                    //return RedirectToAction("ForwardAddInfo", "User", new { area = "User" });
                }
            }
            else //已经登录
            {
                var user = Session["loginUser"] as EnergyMonitor.Models.LinqEntity.User;
                if (user.USR_RoleID == 1)//先写死为1认为是学生用户
                {
                    return RedirectToAction("Welcome", "Home", new { area = "User" });
                }
                else//非学生用户
                {
                    return RedirectToAction("Welcome", "Home", new { area = "Admin" });
                }
            }
        }

        /// <summary>
        /// 激活邮箱
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="activeStr">激活码</param>
        /// <returns></returns>
        public ActionResult ActiveMail(string userID, string activeStr)
        {
            EnergyMonitor.Models.LinqEntity.User user = _userRepos.ActiveMail(userID, activeStr);
            if (user != null)
            {
                DateTime loginTime = DateTime.Now;
                string loginIP;
                if (HttpContext.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    loginIP = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                }
                else
                {
                    loginIP = HttpContext.Request.ServerVariables["REMOTE_ADDR"].ToString();
                }
                // 修改登录时间和IP
                _userRepos.ModifyUserInfo(user.USR_ID, loginTime.ToString(), loginIP);                
                if (user.USR_Status)
                {
                    Session["loginUser"] = user;
                    Session["loginUserName"] = user.USR_Name;
                    Session["userRoomInfo"] = null;
                }
                else//用户被禁用
                {
                    return View("Disabled");
                }
                TempData["activeFlag"] = true;
                if (user.USR_RoleID == 1)//先写死为1认为是学生用户
                {
                    return RedirectToAction("ActiveMail", "User", new { area = "User" });
                }
                else//非学生用户
                {
                    //获取权限
                    var functionList = _functionRepos.GetRoleFuns(user.USR_RoleID);
                    var functionLinkList = functionList.Where(x => x.FN_ID.Length > 6).Select(x => x.FN_LinkLocation).ToList();
                    var navFunctionList = functionList.Where(x => x.FN_ID.Length == 6).ToList();
                    var subFunctionList = functionList.Where(x => x.FN_ID.Length == 9).ToList();
                    var fullSubFunctionList = functionList.Where(x => x.FN_ID.Length > 6).ToList();
                    Session["userFunctionLinks"] = functionLinkList;
                    Session["userNavFunctions"] = navFunctionList;
                    Session["userSubFunctions"] = subFunctionList;
                    Session["fullSubFunctionList"] = fullSubFunctionList;

                    return RedirectToAction("ActiveMail", "User", new { area = "Admin" });
                }
            }
            return View();
        }

        ///// <summary>
        ///// 激活邮箱
        ///// </summary>
        ///// <param name="userID">用户ID</param>
        ///// <param name="activeStr">激活码</param>
        ///// <returns></returns>
        //public ActionResult ActiveMailForMobile(string userID, string activeStr)
        //{
        //    EnergyMonitor.Models.LinqEntity.User user = _userRepos.ActiveMail(userID, activeStr);
        //    if (user != null)
        //    {
        //        //DateTime loginTime = DateTime.Now;
        //        //string loginIP;
        //        //if (HttpContext.Request.ServerVariables["HTTP_VIA"] != null)
        //        //{
        //        //    loginIP = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
        //        //}
        //        //else
        //        //{
        //        //    loginIP = HttpContext.Request.ServerVariables["REMOTE_ADDR"].ToString();
        //        //}
        //        //// 修改登录时间和IP
        //        //_userRepos.ModifyUserInfo(user.USR_ID, loginTime.ToString(), loginIP);
        //        _userRepos.ModifyUserInfoStatus(user);
        //        if (user.USR_Status)
        //        {
        //            Session["loginUser"] = user;
        //            Session["loginUserName"] = user.USR_Name;
        //            Session["userRoomInfo"] = null;
        //        }
        //        else//用户被禁用
        //        {
        //            return View("Disabled");
        //        }
        //        TempData["activeFlag"] = true;
        //        return RedirectToAction("ActiveMail", "User", new { area = "User" });
        //        }
        //        else//非学生用户
        //        {
        //            //获取权限
        //            var functionList = _functionRepos.GetRoleFuns(user.USR_RoleID);
        //            var functionLinkList = functionList.Where(x => x.FN_ID.Length > 6).Select(x => x.FN_LinkLocation).ToList();
        //            var navFunctionList = functionList.Where(x => x.FN_ID.Length == 6).ToList();
        //            var subFunctionList = functionList.Where(x => x.FN_ID.Length == 9).ToList();
        //            var fullSubFunctionList = functionList.Where(x => x.FN_ID.Length > 6).ToList();
        //            Session["userFunctionLinks"] = functionLinkList;
        //            Session["userNavFunctions"] = navFunctionList;
        //            Session["userSubFunctions"] = subFunctionList;
        //            Session["fullSubFunctionList"] = fullSubFunctionList;

        //            return RedirectToAction("ActiveMail", "User", new { area = "Admin" });
        //        }
        //    }
        //    return View();
        //}

        /// <summary>
        /// 用户退出
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Session["loginUser"] = null;
            Session["loginUserName"] = null;
            Session["userRoomInfo"] = null;
            return View("Login");
            //return Redirect(Util.GetConfigValue("uisLogoutUrl"));
        }

        /// <summary>
        /// 测试用户退出 (临时)
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout2()
        {
            Session["loginUser"] = null;
            Session["loginUserName"] = null;
            Session["userRoomInfo"] = null;
            return View("Login");
        }

        /// <summary>
        /// 没有登录
        /// </summary>
        /// <returns></returns>
        public ActionResult NoLogin()
        {
            return View();
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <returns></returns>
        public ActionResult Error()
        {
            return View();
        }
    }
}
