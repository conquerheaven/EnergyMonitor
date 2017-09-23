using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Collections;
using EnergyMonitor.Controllers.Utils;
using EnergyMonitor.Controllers.Admin.Filters;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Implement;
using EnergyMonitor.Models.Repository.Entity;
using System.Linq;
using System.Xml;
using EnergyMonitor.Models.LinqEntity;

namespace EnergyMonitor.Controllers.Admin
{
    /// <summary>
    /// 系统管理
    /// </summary>
    [AdminFilter]
    public class SystemController : Controller
    {
        private IDepartmentRepos _departmentRepos = null;
        private IRoleRepos _roleRepos = null;
        private IUserRepos _userRepos = null;
        private IFunctionRepos _functionRepos = null;
        private IStateRealRepos _stateRealRepos = null;
        public SystemController()
            : this(new DepartmentRepos(), new RoleRepos(), new UserRepos(), new FunctionRepos(), new StateRealRepos())
        {
        }

        public SystemController(IDepartmentRepos departmentRepos, IRoleRepos roleRepos, IUserRepos userRepos, IFunctionRepos functionRepos, IStateRealRepos stateRealRepos)
        {
            _departmentRepos = departmentRepos;
            _roleRepos = roleRepos;
            _userRepos = userRepos;
            _functionRepos = functionRepos;
            _stateRealRepos = stateRealRepos;
        }

        /// <summary>
        /// 跳转用户管理
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult QueryUsers()
        {
            var departmentList = _departmentRepos.GetAllDepartments();
            var roleList = _roleRepos.GetAllRoles();
            ViewBag.departmentList = departmentList;
            ViewBag.roleList = roleList;
            return View();
        }

        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="departmentID"></param>
        /// <param name="roleID"></param>
        /// <param name="userStatus"></param>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public ActionResult QueryUsersAjax(string userID, string userName, int? departmentID, int? roleID, int? roomID, bool? userStatus, int? currentPage, int? totalPages)
        {
            if (Request.IsAjaxRequest())
            {
                var query = _userRepos.GetUserInfo();
                if (!string.IsNullOrWhiteSpace(userID))
                {
                    query = query.Where(x => x.UserID.Contains(userID));
                }
                if (!string.IsNullOrWhiteSpace(userName))
                {
                    query = query.Where(x => x.UserName.Contains(userName));
                }
                if (departmentID.HasValue && departmentID > 0)
                {
                    query = query.Where(x => x.DepartmentID == departmentID);
                }
                if (roleID.HasValue && roleID > 0)
                {
                    query = query.Where(x => x.RoleID == roleID);
                }
                if (roomID.HasValue && roomID > 0)
                {
                    query = query.Where(x => x.RoomID == roomID);
                }
                if (userStatus.HasValue)
                {
                    query = query.Where(x => x.UserStatus == userStatus);
                }
                Pager pager = null;
                if (totalPages == -1)
                {
                    int totalRows = query.Count();
                    pager = new Pager(1, totalRows);
                }
                else
                {
                    pager = new Pager(currentPage.Value, totalPages.Value, false);
                }
                if (pager.TotalPages > 0)
                {
                    var list = query.Skip(pager.StartRow).Take(pager.PageSize);
                    var resultData = new
                    {
                        totalPages = pager.TotalPages,
                        data = list
                    };
                    return Json(resultData, JsonRequestBehavior.AllowGet);
                }
                return Json(new { totalPages = pager.TotalPages }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 查询用户详细
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ActionResult QueryUserDetailAjax(string userID)
        {
            if (Request.IsAjaxRequest() && !String.IsNullOrWhiteSpace(userID))
            {
                var obj = _userRepos.GetUser(userID);
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 禁用启用用户
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult DisableEnableUserAjax(string userID, bool? status)
        {
            if (Request.IsAjaxRequest() && !String.IsNullOrWhiteSpace(userID) && status.HasValue)
            {
                var flag = _userRepos.ChangeUserStatus(userID, status.Value);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 修改用户角色
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="roleID"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult ModifyUserRoleAjax(string userID, int? roleID)
        {
            if (Request.IsAjaxRequest() && !String.IsNullOrWhiteSpace(userID) && roleID.HasValue && roleID.Value > 0)
            {
                var flag = _userRepos.ModifyUserRole(userID, roleID.Value);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 修改用户房间
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="newRoomID"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult ModifyUserRoomAjax(string userID, int? oldRoomID, int? newRoomID)
        {
            if (Request.IsAjaxRequest() && !String.IsNullOrWhiteSpace(userID) && oldRoomID.HasValue && oldRoomID.Value > 0 && newRoomID.HasValue && newRoomID.Value > 0)
            {
                var flag = _userRepos.ModifyUserRoom(userID, oldRoomID.Value, newRoomID.Value);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 导出用户查询Excel数据
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="departmentID"></param>
        /// <param name="roleID"></param>
        /// <param name="userStatus"></param>
        /// <returns></returns>
        public ActionResult GetUsersExcel(string userID, string userName, int? departmentID, int? roleID, int? roomID, bool? userStatus)
        {
            var query = _userRepos.GetUserInfo();
            if (!string.IsNullOrWhiteSpace(userID))
            {
                query = query.Where(x => x.UserID.Contains(userID));
            }
            if (!string.IsNullOrWhiteSpace(userName))
            {
                query = query.Where(x => x.UserName.Contains(userName));
            }
            if (departmentID.HasValue && departmentID > 0)
            {
                query = query.Where(x => x.DepartmentID == departmentID);
            }
            if (roleID.HasValue && roleID > 0)
            {
                query = query.Where(x => x.RoleID == roleID);
            }
            if (roomID.HasValue && roomID > 0)
            {
                query = query.Where(x => x.RoomID == roomID);
            }
            if (userStatus.HasValue)
            {
                query = query.Where(x => x.UserStatus == userStatus);
            }
            var list = query.ToList();
            if (list != null)
            {
                string[] headers = { "工号 / 学号", "用户名", "邮箱", "所在院系", "角色", "所属房间", "上次登录IP", "上次登录时间", "状态" };
                string[] properties = { "UserID", "UserName", "UserMail", "DepartmentName", "RoleName", "RoomName", "LastLoginIP", "LastLoginTimeStr", "StatusName" };
                return this.Excel(list, "用户.xls", headers, properties);
            }
            return null;
        }

        /// <summary>
        /// 跳转角色权限管理
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult QueryRole()
        {
            return View();
        }

        /// <summary>
        /// 查询角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public ActionResult QueryRoleAjax(string roleName, int? currentPage, int? totalPages)
        {
            if (Request.IsAjaxRequest() && currentPage.HasValue && totalPages.HasValue)
            {
                Pager pager = null;
                if (totalPages == -1)
                {
                    int totalRows = _roleRepos.GetRolesCount(roleName);
                    pager = new Pager(1, totalRows);
                }
                else
                {
                    pager = new Pager(currentPage.Value, totalPages.Value, false);
                }
                if (pager.TotalPages > 0)
                {
                    var list = _roleRepos.GetRoles(roleName, pager.StartRow, pager.PageSize);
                    var reData = new
                    {
                        totalPages = pager.TotalPages,
                        data = list
                    };
                    return Json(reData, JsonRequestBehavior.AllowGet);
                }
                return Json(new { totalPages = pager.TotalPages }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 查询该角色拥有的用户数
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public ActionResult QueryRoleUserCount(int? roleID)
        {
            if (Request.IsAjaxRequest() && roleID.HasValue && roleID.Value > 0)
            {
                int count = _roleRepos.GetRoleUserCount(roleID.Value);
                return Json(count, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public ActionResult AddRoleAjax(string roleName)
        {
            if (Request.IsAjaxRequest() && !String.IsNullOrWhiteSpace(roleName))
            {
                if (_roleRepos.AddRole(roleName) > 0)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult DeleteRoleAjax(int? roleID)
        {
            if (Request.IsAjaxRequest() && roleID.HasValue && roleID.Value > 1)//不能删除ID为1的学生角色
            {
                var flag = _roleRepos.DeleteRole(roleID.Value);
                var funFlag = _functionRepos.DeleteRoleFuns(roleID.Value);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 修改角色名
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public ActionResult ModifyRoleAjax(int? roleID, string roleName)
        {
            if (Request.IsAjaxRequest() && roleID.HasValue && roleID.Value > 0 && !String.IsNullOrWhiteSpace(roleName))
            {
                var flag = _roleRepos.ModifyRoleName(roleID.Value, roleName);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 查询角色名称是否不存在
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public ActionResult QueryRoleNameNotExistAjax(string roleName)
        {
            if (Request.IsAjaxRequest())
            {
                var flag = !_roleRepos.IsRoleNameExist(roleName);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 跳转增加角色
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult AddRole()
        {
            var list = _functionRepos.GetAllFuns();
            return View(list);
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="functionID"></param>
        /// <returns></returns>
        public ActionResult AddRoleInfo(string roleName, string functionID)
        {
            if (!string.IsNullOrWhiteSpace(roleName) && !string.IsNullOrWhiteSpace(functionID))
            {
                string[] functionIDs = functionID.Split(new char[] { '_' });
                int roleID = _roleRepos.AddRole(roleName);
                if (roleID > 0)
                {
                    var flag = _functionRepos.AddRoleFuns(roleID, functionIDs);
                    ViewBag.flag = flag;
                }
            }
            return View();
        }

        /// <summary>
        /// 跳转修改角色
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult ModifyRole(int? r)
        {
            if (r.HasValue && r.Value > 0)
            {
                var role = _roleRepos.GetRole(r.Value);
                var functionIDList = _functionRepos.GetRoleFunIDs(r.Value);
                var sortedFunctionIDList = functionIDList.OrderBy(x => x).ToList();
                if (sortedFunctionIDList.Count > 0)
                {
                    string prevFunID = "";
                    string oldFunctionIDStr = "";
                    string lastFunID = sortedFunctionIDList.Last();
                    foreach (var funID in sortedFunctionIDList)
                    {
                        if (!funID.StartsWith(prevFunID))
                        {
                            oldFunctionIDStr += (prevFunID + "_");
                        }
                        prevFunID = funID;
                    }
                    oldFunctionIDStr += lastFunID;
                    ViewBag.oldFunctionIDStr = oldFunctionIDStr;
                }
                var list = _functionRepos.GetAllFuns();

                ViewBag.role = role;
                return View(list);
            }
            return View();
        }

        /// <summary>
        /// 修改查询角色名是否可用
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="oldRoleName"></param>
        /// <returns></returns>
        public ActionResult QueryModifyRoleNameNotExistAjax(string roleName, string oldRoleName)
        {
            if (Request.IsAjaxRequest())
            {
                var flag = true;
                if (roleName != oldRoleName)
                {
                    flag = !_roleRepos.IsRoleNameExist(roleName);
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 修改角色权限
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="roleName"></param>
        /// <param name="oldRoleName"></param>
        /// <param name="functionID"></param>
        /// <param name="modifyFunFlag"></param>
        /// <returns></returns>
        public ActionResult ModifyRoleInfo(int? roleID, string roleName, string oldRoleName, string functionID, bool? modifyFunFlag)
        {
            if (roleID.HasValue && roleID.Value > 0 && modifyFunFlag.HasValue)
            {
                bool modifyNameFlag = true;
                bool deleteFunFlag = true;
                bool addRoleFlag = true;
                if (roleName != oldRoleName)
                {
                    modifyNameFlag = _roleRepos.ModifyRoleName(roleID.Value, roleName);
                }
                if (!modifyFunFlag.Value)
                {
                    deleteFunFlag = _functionRepos.DeleteRoleFuns(roleID.Value);
                    string[] functionIDs = functionID.Split(new char[] { '_' });
                    addRoleFlag = _functionRepos.AddRoleFuns(roleID.Value, functionIDs);
                }
                ViewBag.flag = false;
                if (modifyNameFlag && deleteFunFlag && addRoleFlag)
                {
                    ViewBag.flag = true;
                }
            }
            return View();
        }

        /// <summary>
        /// 查询角色权限
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public ActionResult QueryRoleFunsAjax(int? roleID)
        {
            if (Request.IsAjaxRequest() && roleID.HasValue)
            {
                var functionList = _functionRepos.GetRoleFuns(roleID.Value);
                XmlDocument xmlDoc = new XmlDocument();
                if (functionList != null && functionList.Count > 0)
                {
                    var sortedFunctionList = functionList.Where(x => x.FN_ID.Length > 3).OrderBy(x => x.FN_ID).ToList();
                    XmlElement topUl = xmlDoc.CreateElement("ul");
                    topUl.SetAttribute("class", "mktree");
                    topUl.SetAttribute("id", "functionTreeUl");
                    xmlDoc.AppendChild(topUl);
                    foreach (var item in sortedFunctionList)
                    {
                        XmlElement li = xmlDoc.CreateElement("li");
                        li.SetAttribute("id", item.FN_ID);
                        li.InnerText = item.FN_Name;
                        int len = item.FN_ID.Length;
                        if (len == 6)
                        {
                            topUl.AppendChild(li);
                        }
                        else
                        {
                            string parentID = item.FN_ID.Substring(0, len - 3);
                            XmlNode parentLi = xmlDoc.SelectSingleNode("//li[@id='" + parentID + "'] ");
                            XmlNode parentUl = parentLi.ChildNodes[1];
                            if (parentUl != null)
                            {
                                parentUl.AppendChild(li);
                            }
                            else
                            {
                                parentUl = xmlDoc.CreateElement("ul");
                                parentUl.AppendChild(li);
                                parentLi.AppendChild(parentUl);
                            }
                        }
                    }
                    string funsHtml = xmlDoc.OuterXml;
                    return Json(funsHtml, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        /// <summary>
        /// 跳转系统运行状态
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult QueryState()
        {
            double wsExpired = (double)Util.GetConfigValueObj("wsExpired"); //以分钟计
            DateTime compareTime = DateTime.Now.AddMinutes(-wsExpired);
            var list = _stateRealRepos.GetAllWorkstationsState().ToList();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (item.Time > compareTime)
                    {
                        item.Status = 1;
                    }
                    else
                    {
                        item.Status = 0;
                    }
                }
            }
            return View(list);
        }

        /// <summary>
        /// 获取RTU状态
        /// </summary>
        /// <param name="workstationNo"></param>
        /// <returns></returns>
        public ActionResult GetRTUState(int? workstationNo)
        {
            if (Request.IsAjaxRequest() && workstationNo.HasValue && workstationNo.Value > 0)
            {
                var list = _stateRealRepos.GetRTUStateByWorkstation(workstationNo.Value).ToList();
                double wsExpired = (double)Util.GetConfigValueObj("wsExpired"); //以分钟计
                DateTime compareTime = DateTime.Now.AddMinutes(-wsExpired);
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (item.Time > compareTime)
                        {
                            item.Status = 1;
                        }
                        else
                        {
                            item.Status = 0;
                        }
                    }                    
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        /// <summary>
        /// 获取仪表状态
        /// </summary>
        /// <param name="rtuNo"></param>
        /// <returns></returns>
        public ActionResult GetIntrumentState(int? rtuNo)
        {
            if (Request.IsAjaxRequest() && rtuNo.HasValue && rtuNo.Value > 0)
            {
                var list = _stateRealRepos.GetInstrumentStateByRTU(rtuNo.Value).ToList();
                if (list != null && list.Count > 0)
                {
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        /// <summary>
        /// 查询所有故障设备
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult BadState()
        {
            //// 得到所有的W、R和C设备
            //var list = _stateRealRepos.GetAllState().ToList();

            //var instrumentList = list.Where(x => x.Type == "C" && x.Status == 0).ToList();

            //var instrumentParentNo = instrumentList.Select(x => x.ParentNo);
            //var rtuList = list.Where(x => instrumentParentNo.Contains(x.StateNo) && x.Type == "R").ToList();

            //var rtuParentNo = rtuList.Select(x => x.ParentNo);
            //var workstationList = list.Where(x => rtuParentNo.Contains(x.StateNo) && x.Type == "W").ToList();

            ////理论上只要上面的情况就够了，但是当前比较特殊，某些仪表表不存在，所以必须要做下面一些操作
            //var rtuTempList = list.Where(x => x.Type == "R" && x.Status == 0).ToList();
            //rtuList.AddRange(rtuTempList);

            //var rtuTempParentNo = rtuTempList.Select(x => x.ParentNo);
            //var workstationTempList = list.Where(x => rtuTempParentNo.Contains(x.StateNo) && x.Type == "W").ToList();
            //workstationList.AddRange(workstationTempList);

            ////同上（当前即可删除）
            //double wsExpired = (double)Util.GetConfigValueObj("wsExpired"); //以分钟计
            //DateTime compareTime = DateTime.Now.AddMinutes(-wsExpired);
            //var workstationTemp2List = list.Where(x => x.Type == "W" && x.Time <= compareTime).ToList();
            //workstationList.AddRange(workstationTemp2List);

            //// 更新WS状态
            //if (workstationList != null && workstationList.Count > 0)
            //{
            //    foreach (var item in list)
            //    {
            //        item.Status = 0;
            //    }
            //}

            //ViewBag.instrumentList = instrumentList.Distinct().ToList();
            //ViewBag.rtuList = rtuList.Distinct().ToList();
            //ViewBag.workstationList = workstationList.Distinct().ToList();

            var list = _stateRealRepos.GetAllState().ToList();            
            // 更新WS状态
           
            double wsExpired = (double)Util.GetConfigValueObj("wsExpired"); //以分钟计
            DateTime compareTime = DateTime.Now.AddMinutes(-wsExpired);
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (item.Time > compareTime)
                    {
                        item.Status = 1;
                    }
                    else
                    {
                        item.Status = 0;
                    }
                }
            }

            ViewBag.instrumentList = list.Where(x => x.Type == "C" && x.Status == 0).Distinct().ToList();
            var rtuTempList = list.Where(x => x.Type == "R" && x.Status == 0).Distinct().ToList();
            ViewBag.rtuList = rtuTempList;
            var workstationList = list.Where(x => x.Type == "W" && x.Status == 0).ToList();
            var rtuTempParentNo = rtuTempList.Select(x => x.ParentNo);
            var workstationTempList = list.Where(x => rtuTempParentNo.Contains(x.StateNo) && x.Type == "W").ToList();
            workstationList.AddRange(workstationTempList);
            ViewBag.workstationList = workstationList.Distinct().ToList();
            return View();
           
        }

        /// <summary>
        /// 查看所有状态
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult AllState()
        {
            var list = _stateRealRepos.GetAllState().ToList();
            var workstationList = list.Where(x => x.Type == "W").ToList();
            // 更新WS状态
            double wsExpired = (double)Util.GetConfigValueObj("wsExpired"); //以分钟计
            DateTime compareTime = DateTime.Now.AddMinutes(-wsExpired);
            if (workstationList != null && workstationList.Count > 0)
            {
                foreach (var item in list)
                {
                    if (item.Time > compareTime)
                    {
                        item.Status = 1;
                    }
                    else
                    {
                        item.Status = 0;
                    }                   
                }
            }

            ViewBag.instrumentList = list.Where(x => x.Type == "C").ToList();
            ViewBag.rtuList = list.Where(x => x.Type == "R").ToList();
            ViewBag.workstationList = workstationList;

            return View();
        }

        /// <summary>
        /// 图表查看状态
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult DiagramState()
        {
            return View();
        }

        /// <summary>
        /// Ajax查询所有状态
        /// </summary>
        /// <returns></returns>
        public ActionResult AllStateAjax()
        {
            if (Request.IsAjaxRequest())
            {
                // 临时用RDS来保存系统状态位置
                var systemObj = _stateRealRepos.GetRDSState();
                if(systemObj == null)
                {
                    systemObj = new StateEntity();
                    systemObj.PositionX = 400;
                }
                // 默认认为系统运行即为正常状态
                systemObj.Status = 1;
                var systemList = new ArrayList();
                systemList.Add(systemObj);

                var list = _stateRealRepos.GetWordAndRTUState().ToList();
                var workstationList = list.Where(x => x.Type == "W").ToList();
                // 更新WS状态
                double wsExpired = (double)Util.GetConfigValueObj("wsExpired"); //以分钟计
                DateTime compareTime = DateTime.Now.AddMinutes(-wsExpired);
                if (workstationList != null && workstationList.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (item.Time > compareTime)
                        {
                            item.Status = 1;
                        }
                        else
                        {
                            item.Status = 0;
                        }                        
                    }
                }

                var data = new
                {
                    systemList = systemList,
                    workstationList = workstationList,
                    rtuList = list.Where(x => x.Type == "R").ToList()
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 修改图表中的所有状态的位置
        /// </summary>
        /// <param name="updateList"></param>
        /// <returns></returns>
        public ActionResult UpdateState(List<StateReal> updateList)
        {
            if (Request.IsAjaxRequest() && updateList != null && updateList.Count > 0)
            {
                var flag = _stateRealRepos.UpdateStatePosition(updateList);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
    }
}
