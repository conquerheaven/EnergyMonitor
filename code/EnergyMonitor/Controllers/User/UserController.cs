using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnergyMonitor.Controllers.User.Filters;
using System.Collections;
using EnergyMonitor.Controllers.Utils;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Implement;
using System.Threading;
using EnergyMonitor.Models.LinqEntity;



namespace EnergyMonitor.Controllers.User
{
    /// <summary>
    /// 用户信息业务逻辑层控制类
    /// </summary>
    public class UserController : Controller
    {
        private IUserRepos _userRepos = null;
        private IRoomRepos _roomRepos = null;
        private IDepartmentRepos _departmentRepos = null;

        public UserController()
            : this(new UserRepos(), new RoomRepos(), new DepartmentRepos())
        {
        }

        public UserController(IUserRepos userRepos, IRoomRepos roomRepos, IDepartmentRepos departmentRepos)
        {
            _userRepos = userRepos;
            _roomRepos = roomRepos;
            _departmentRepos = departmentRepos;
        }

        /// <summary>
        /// 个人信息
        /// </summary>
        /// <returns></returns>
        [UserFilter]
        public ActionResult PersonalInfo()
        {
            EnergyMonitor.Models.LinqEntity.User loginUser = Session["loginUser"] as EnergyMonitor.Models.LinqEntity.User;
            if (loginUser != null)
            {
                ViewBag.userID = loginUser.USR_ID;
                ViewBag.userName = loginUser.USR_Name;
                ViewBag.userMail = loginUser.USR_Mail;
                ViewBag.userDepartName = loginUser.USR_DepartName;
                if (Session["userRoomInfo"] != null)
                {
                    var list = Session["userRoomInfo"];
                    return View(list);
                }
                else
                {
                    var list = _userRepos.GetUserRelatedRooms(loginUser.USR_ID);
                    Session["userRoomInfo"] = list;
                    return View(list);
                }
            }
            return View();
        }

        /// <summary>
        /// 修改信息页面
        /// </summary>
        /// <returns></returns>
        [UserFilter]
        public ActionResult ModifyInfo()
        {
            var departmentList = _departmentRepos.GetAllDepartments();
            ViewBag.departmentList = departmentList;
            return View(Session["loginUser"]);
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="userEmail">邮箱，修改后需要激活</param>
        /// <param name="userDepartment">所属院系</param>
        /// <param name="departmentName">所属院系名称</param>
        /// <returns></returns>
        [UserFilter]
        public ActionResult ModifiedInfo(string userName, string userEmail, int userDepartment, string departmentName)
        {
            bool isModified = false;
            EnergyMonitor.Models.LinqEntity.User loginUser = Session["loginUser"] as EnergyMonitor.Models.LinqEntity.User;
            if (!String.IsNullOrWhiteSpace(userName) && loginUser.USR_Name != userName)
            {
                loginUser.USR_Name = userName;
                Session["loginUserName"] = userName;
                isModified = true;
            }
            string activeStr = null;
            if (!String.IsNullOrWhiteSpace(userEmail) && loginUser.USR_Mail != userEmail)
            {
                loginUser.USR_Mail = userEmail;
                activeStr = Guid.NewGuid().ToString();
                loginUser.USR_MailActiveCode = activeStr;
                isModified = true;
            }
            if (userDepartment > 0 && loginUser.USR_DepartID != userDepartment)
            {
                loginUser.USR_DepartID = userDepartment;
                isModified = true;
            }
            if (!String.IsNullOrWhiteSpace(departmentName) && loginUser.USR_DepartName != departmentName)
            {
                loginUser.USR_DepartName = departmentName;
                isModified = true;
            }
            if (isModified)
            {
                if(_userRepos.ModifyUserInfo(loginUser) == null)
                {
                    return RedirectToAction("Error", "Shared");
                }
                if (activeStr != null)//需要激活邮箱
                {
                    string webSiteName = "http://" + Request.Url.Authority;
                    // 线程发送
                    new Thread(delegate()
                    {
                        Util.SendActiveMail(webSiteName, userEmail, loginUser.USR_ID, userName, "/Homes/ActiveMail", activeStr);
                    }).Start();
                    ViewBag.isMailSendError = false;
                    ViewBag.mailAddr = userEmail;
                }
                else
                {
                    ViewBag.isMailSendError = null;
                }
            }
            ViewBag.isModified = isModified;
            return View("RedirectPage");
        }

        /// <summary>
        /// 使用Ajax得到校区信息
        /// </summary>
        /// <returns>Json格式所有校区信息</returns>
        public ActionResult GetAllShoolAjax()
        {
            //if (Request.IsAjaxRequest())
            if (Request.IsAjaxRequest())
            {
                var schools = _roomRepos.GetAllSchool();
                IList list = new ArrayList();
                foreach (var item in schools)
                {
                    list.Add(new
                    {
                        dataID = item.SI_ID,
                        dataValue = item.SI_Name
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 得到校区信息(提供给微信平台使用)
        /// </summary>
        /// <returns>Json格式所有校区信息</returns>
        public ActionResult GetAllShoolAjaxForMobile()
        {            
            var schools = _roomRepos.GetAllSchool();
            IList list = new ArrayList();
            foreach (var item in schools)
            {
                list.Add(new
                {
                    dataID = item.SI_ID,
                    dataValue = item.SI_Name
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);            
        }

        /// <summary>
        /// 使用Ajax根据校区ID得到区域
        /// </summary>
        /// <param name="schoolID">校区ID</param>
        /// <returns>Json格式区域集合</returns>
        public ActionResult GetAreasBySchoolIDAjax(int schoolID)
        {
            if (Request.IsAjaxRequest() && schoolID > 0)            
            {
                var areas = _roomRepos.GetAreaBySchoolID(schoolID);
                IList list = new ArrayList();
                foreach (var item in areas)
                {
                    list.Add(new
                    {
                        dataID = item.SAI_ID,
                        dataValue = item.SAI_Name
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 根据校区ID得到区域(提供给微信平台使用)
        /// </summary>
        /// <param name="schoolID">校区ID</param>
        /// <returns>Json格式区域集合</returns>
        public ActionResult GetAreasBySchoolIDAjaxForMobile(int schoolID)
        {
            if (schoolID > 0)
            {
                var areas = _roomRepos.GetAreaBySchoolID(schoolID);
                IList list = new ArrayList();
                foreach (var item in areas)
                {
                    list.Add(new
                    {
                        dataID = item.SAI_ID,
                        dataValue = item.SAI_Name
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 根据区域得到建筑
        /// </summary>
        /// <param name="areaID">区域ID</param>
        /// <returns></returns>
        public ActionResult GetBuildingByAreaAjax(int areaID)
        {
            if (Request.IsAjaxRequest() && areaID > 0)           
            {
                var buildings = _roomRepos.GetBuildingByAreaID(areaID);
                IList list = new ArrayList();
                foreach (var item in buildings)
                {
                    list.Add(new
                    {
                        dataID = item.BDI_ID,
                        dataValue = item.BDI_Name
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 根据区域得到建筑(提供给微信平台使用)
        /// </summary>
        /// <param name="areaID">区域ID</param>
        /// <returns></returns>
        public ActionResult GetBuildingByAreaAjaxForMobile(int areaID)
        {           
            if (areaID > 0)
            {
                var buildings = _roomRepos.GetBuildingByAreaID(areaID);
                IList list = new ArrayList();
                foreach (var item in buildings)
                {
                    list.Add(new
                    {
                        dataID = item.BDI_ID,
                        dataValue = item.BDI_Name
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 根据建筑ID得到所有房间
        /// </summary>
        /// <param name="buildingID">建筑ID</param>
        /// <returns></returns>
        public ActionResult GetRoomsByBIDAjax(int buildingID)
        {
            if (Request.IsAjaxRequest() && buildingID > 0)            
            {
                var rooms = _roomRepos.GetRooomByBuildingID(buildingID);
                IList list = new ArrayList();
                foreach (var item in rooms)
                {
                    list.Add(new
                    {
                        dataID = item.RI_ID,
                        dataValue = item.RI_RoomCode
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 根据建筑ID得到所有房间(提供给微信平台使用)
        /// </summary>
        /// <param name="buildingID">建筑ID</param>
        /// <returns></returns>
        public ActionResult GetRoomsByBIDAjaxForMobile(int buildingID)
        {
            if (buildingID > 0)
            {
                var rooms = _roomRepos.GetRooomByBuildingID(buildingID);
                IList list = new ArrayList();
                foreach (var item in rooms)
                {
                    list.Add(new
                    {
                        dataID = item.RI_ID,
                        dataValue = item.RI_RoomCode
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 跳转添加个人信息页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ForwardAddInfo()
        {
            ViewBag.userID = Session["userID"];
            var departmentList = _departmentRepos.GetAllDepartments();
            
            return View("AddInfo", departmentList);
        }

        /// <summary>
        /// 添加个人信息
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public ActionResult AddInfo(FormCollection collection)
        {
            string userID = collection["userID"];
            int roomID = Convert.ToInt32(collection["roomID"]);
            if (userID == null || userID.Trim() == "" || Session["userID"] == null || userID.Trim() != Session["userID"].ToString() || roomID < 0)
            {
                return RedirectToAction("Error", "Shared");
            }
            EnergyMonitor.Models.LinqEntity.User user = new EnergyMonitor.Models.LinqEntity.User();
            user.USR_ID = userID;
            user.USR_Name = collection["userName"];
            user.USR_Mail = collection["userEmail"];
            user.USR_DepartID = Convert.ToInt32(collection["userDepartment"]);
            user.USR_DepartName = collection["departmentName"];

            user.USR_RoleID = 1;//默认插入角色为1,即默认为学生组
            user.USR_Status = true;
            user.USR_LastLoginTime = Session["userLoginTime"] as DateTime?;
            string clientIP;
            if (HttpContext.Request.ServerVariables["HTTP_VIA"] != null)
            {
                clientIP = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else
            {
                clientIP = HttpContext.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            user.USR_LastLoginIP = clientIP;//插入用户登录IP

            // 用线程发送邮箱激活邮件
            string activeStr = Guid.NewGuid().ToString();
            user.USR_MailActiveCode = activeStr;
            string webSiteName = "http://" + Request.Url.Authority;
            new Thread(delegate()
            {
                Util.SendActiveMail(webSiteName, user.USR_Mail, user.USR_ID, user.USR_Name, "/Homes/ActiveMail", activeStr);
            }).Start();
            
            user = _userRepos.InsertUser(user);
            bool flag = _roomRepos.AddUserRoom(user.USR_ID, roomID);
            if (user == null || !flag)//添加信息失败
            {
                ViewBag.error = true;
            }
            else
            {
                ViewBag.error = null;
                //Session["loginUser"] = user;
                //Session["loginUserName"] = user.USR_Name;
                //Session["userRoomInfo"] = null;
            }
            return View("AddInfoResult");
        }

        /// <summary>
        /// 跳转注册页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            var departmentList = _departmentRepos.GetAllDepartments();
            return View(departmentList);
        }

        /// <summary>
        /// 返回学校所有部门的信息
        /// </summary>
        /// <returns></returns>
        public IList<DepartmentInfo> getDepartmentList()
        {
            var departmentList = _departmentRepos.GetAllDepartments();
            return departmentList;
        }

        /// <summary>
        /// 注册个人信息
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public ActionResult Registered(FormCollection collection)
        {
            string userID = collection["userID"];
            string password = collection["password"];
            int roomID = Convert.ToInt32(collection["roomID"]);
            if (string.IsNullOrWhiteSpace(userID) || roomID < 0 || string.IsNullOrWhiteSpace(password))
            {
                return RedirectToAction("Error", "Shared");
            }
            // 加密后的密码
            string encryptPassword = Util.SHA1Encrypt(userID + password);
            EnergyMonitor.Models.LinqEntity.User user = new EnergyMonitor.Models.LinqEntity.User();
            user.USR_ID = userID;
            user.USR_Name = collection["userName"];
            user.USR_Mail = collection["userEmail"];
            user.USR_DepartID = Convert.ToInt32(collection["userDepartment"]);
            user.USR_DepartName = collection["departmentName"];
            // 使用USR_Remark作为用户密码
            user.USR_Remark = encryptPassword;

            user.USR_RoleID = 22;//默认插入角色为22,即默认为测试用户组
            user.USR_Status = true;
            user.USR_LastLoginTime = Session["userLoginTime"] as DateTime?;
            string clientIP;
            if (HttpContext.Request.ServerVariables["HTTP_VIA"] != null)
            {
                clientIP = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else
            {
                clientIP = HttpContext.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            user.USR_LastLoginIP = clientIP;//插入用户登录IP

            // 用线程发送邮箱激活邮件
            string activeStr = Guid.NewGuid().ToString();
            user.USR_MailActiveCode = activeStr;
            string webSiteName = "http://" + Request.Url.Authority;
            new Thread(delegate()
            {
                Util.SendActiveMail(webSiteName, user.USR_Mail, user.USR_ID, user.USR_Name, "/Homes/ActiveMail", activeStr);
            }).Start();

            user = _userRepos.InsertUser(user);
            bool flag = _roomRepos.AddUserRoom(user.USR_ID, roomID);
            if (user == null || !flag)//添加信息失败
            {
                ViewBag.error = true;
            }
            else
            {
                ViewBag.error = null;
                //Session["loginUser"] = user;
                //Session["loginUserName"] = user.USR_Name;
                //Session["userRoomInfo"] = null;
            }
            return View("AddInfoResult");
        }

        /// <summary>
        /// 注册个人信息(给微信平台使用)
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public ActionResult RegisteredForMobile(String userID, String password, String userName, String userEmail, String userDepartment, String userDepartmentName, String roomID, String registerIP)
        {
            var dec = true;
            if (string.IsNullOrWhiteSpace(userID) || Convert.ToInt32(roomID) < 0 || string.IsNullOrWhiteSpace(password))
            {
                dec = false;
                return Json(dec, JsonRequestBehavior.AllowGet);
            }
            // 加密后的密码
            string encryptPassword = Util.SHA1Encrypt(userID + password);
            EnergyMonitor.Models.LinqEntity.User user = new EnergyMonitor.Models.LinqEntity.User();
            user.USR_ID = userID;
            user.USR_Name = userName;
            user.USR_Mail = userEmail;
            user.USR_DepartID = Convert.ToInt32(userDepartment);
            user.USR_DepartName = userDepartmentName;
            // 使用USR_Remark作为用户密码
            user.USR_Remark = encryptPassword;

            user.USR_RoleID = 22;//默认插入角色为22,即默认为测试用户组
            user.USR_Status = true;
            user.USR_LastLoginTime = DateTime.Now; 
            user.USR_LastLoginIP = registerIP;//插入用户注册IP

            // 用线程发送邮箱激活邮件
            string activeStr = Guid.NewGuid().ToString();
            user.USR_MailActiveCode = activeStr;
            string webSiteName = "http://" + Request.Url.Authority;
            new Thread(delegate()
            {
                Util.SendActiveMail(webSiteName, user.USR_Mail, user.USR_ID, user.USR_Name, "/Homes/ActiveMail", activeStr);
            }).Start();

            user = _userRepos.InsertUser(user);
            bool flag = _roomRepos.AddUserRoom(user.USR_ID, Convert.ToInt32(roomID));
            if (user == null || !flag)//添加信息失败
            {
                dec = false;
            }
            return Json(dec, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获得所有部门信息，提供给微信平台注册页面使用
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllDepartment()
        {
            var departmentList = _departmentRepos.GetAllDepartments();
            return Json(departmentList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 检查账号是否没有被使用，即可以被使用
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public ActionResult UserIDCanBeUsed(string userID)
        {
            var flag = false;
            if (!string.IsNullOrWhiteSpace(userID))
            {
                var user = _userRepos.GetUserByID(userID);
                flag = user == null ? true : false;
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 跳转激活邮箱结果页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ActiveMail()
        {
            if (TempData["activeFlag"] != null)
            {
                ViewBag.activeFlag = TempData["activeFlag"];
            }
            return View();
        }

        /// <summary>
        /// 跳转激活邮箱结果页面(供微信平台使用)
        /// </summary>
        /// <returns></returns>
        public ActionResult ActiveMailForMobile()
        {
            if (TempData["activeFlag"] != null)
            {
                ViewBag.activeFlag = TempData["activeFlag"];
            }
            return View();
        }

        /// <summary>
        /// 检查邮箱是否没有被使用，即可以被使用
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public ActionResult MailCanBeUsedAjax(string userEmail)
        {
            var flag = false;            
            flag = !_userRepos.IsMailUsed(userEmail);           
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改时检查邮箱是否没有被使用，即可以被使用
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="oldUserMail"></param>
        /// <returns></returns>
        public ActionResult ModifyMailCanBeUsedAjax(string userEmail, string oldUserMail)
        {
            var flag = false;
            if (Request.IsAjaxRequest() && !string.IsNullOrWhiteSpace(userEmail))
            {
                if (userEmail == oldUserMail)
                {
                    flag = true;
                }
                else
                {
                    flag = !_userRepos.IsMailUsed(userEmail);
                }
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
    }
}
