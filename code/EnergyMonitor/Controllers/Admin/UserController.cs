using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using EnergyMonitor.Controllers.Utils;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Implement;
using EnergyMonitor.Controllers.Admin.Filters;
using System.Threading;


namespace EnergyMonitor.Controllers.Admin
{
    /// <summary>
    /// 个人信息业务逻辑层控制类
    /// </summary>
    /// <author>WangWei</author>
    /// <date>2010-11-26</date>
    [AdminFilter]
    public class UserController : Controller
    {
        private IUserRepos _userRepos = null;
        private IRoomRepos _roomRepos = null;
        private IDepartmentRepos _departmentRepos = null;
        private IFunctionRepos _functionRepos = null;

        public UserController()
            : this(new UserRepos(), new RoomRepos(), new DepartmentRepos(), new FunctionRepos())
        {
        }

        public UserController(IUserRepos userRepos, IRoomRepos roomRepos, IDepartmentRepos departmentRepos, IFunctionRepos functionRepos)
        {
            _userRepos = userRepos;
            _roomRepos = roomRepos;
            _departmentRepos = departmentRepos;
            _functionRepos = functionRepos;
        }

        /// <summary>
        /// 跳转个人信息
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
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
        [AuthenticationFilter]
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
                if (_userRepos.ModifyUserInfo(loginUser) == null)
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

        /// <summary>
        /// 使用Ajax根据房间名得到房间ID (废弃)
        /// </summary>
        /// <param name="roomName">房间名</param>
        /// <returns>Json格式房间ID和房间名</returns>
        public ActionResult GetRoomIDByNameAjax(string roomName)
        {
            //if (Request.IsAjaxRequest())
            //{
            //    string roomID = _ampRepos.GetRoomIDByName(roomName);

            //    return Json(new { reRoomID = roomID, reRoomName = roomName }, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    return null;
            //}
            return null;
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
        /// 跳转我的权限
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult MyRight()
        {
            EnergyMonitor.Models.LinqEntity.User loginUser = Session["loginUser"] as EnergyMonitor.Models.LinqEntity.User;
            var functionList = _functionRepos.GetRoleFuns(loginUser.USR_RoleID);
            return View(functionList);
        }

    }
}
