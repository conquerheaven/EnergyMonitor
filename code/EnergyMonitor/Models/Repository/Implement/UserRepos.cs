using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;
using System.Data.Linq;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Entity;
using System.Linq.Dynamic;
 
namespace EnergyMonitor.Models.Repository.Implement
{
    /// <summary>
    /// Linq实现的User实体类数据操作类
    /// </summary>
    public class UserRepos : IUserRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public UserRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region IUserRepos Members

        /// <summary>
        /// Linq实现获得所有的User
        /// </summary>
        /// <author>WangWei</author>
        /// <date>2010-11-17</date>
        /// <returns>User泛型的List</returns>
        public IList<User> ListAll()
        {
            //linq实现
            var users = from u in _dataContext.Users
                        select u;
            return users.ToList();
        }

        /// <summary>
        /// 根据用户ID得到用户实体
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns>得到用户实体，没有返回null</returns>
        public User GetUserByID(string userID)
        {
            var user = _dataContext.Users.FirstOrDefault(u => u.USR_ID == userID);
            return user;
        }

        /// <summary>
        /// 根据用户ID和密码得到用户实体
        /// </summary>
        /// by yifang
        /// <param name="userID">用户ID</param>
        /// /// <param name="password">用户密码</param>
        /// <returns>得到用户实体，没有返回null</returns>
        public User GetUserByIDAndPassword(string userID, string password)
        {
            var user = _dataContext.Users.FirstOrDefault(u => u.USR_ID == userID && u.USR_Remark == password);
            return user;
        }

        /// <summary>
        /// 根据用户用户ID得到用户所有房间信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns>用户所有房间的集合</returns>
        public IList<UserRoomFullName> GetUserRelatedRooms(string userID)
        {
            var list = from ur in _dataContext.UserRooms
                       from ri in _dataContext.RoomInfos
                       from bdi in _dataContext.BuildingBriefInfos
                       from sai in _dataContext.SchoolAreaInfos
                       from si in _dataContext.SchoolInfos
                       where ur.USR_ID == userID && ur.RI_ID == ri.RI_ID && ri.BDI_ID == bdi.BDI_ID && bdi.SAI_ID == sai.SAI_ID && sai.SI_ID == si.SI_ID
                       select new UserRoomFullName
                       {
                           SIName = si.SI_Name,
                           SAIName = sai.SAI_Name,
                           BDIName = bdi.BDI_Name,
                           RICode = ri.RI_RoomCode,
                           RIID=ri.RI_ID
                       };
            return list.ToList();
        }

        /// <summary>
        /// 修改邮箱
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="newMail">新邮箱</param>
        /// <param name="activeStr">邮箱激活码</param>
        /// <returns>修改成功返回true，否则返回false</returns>
        public bool ModifyMail(string userID, string newMail, string activeStr)
        {
            User user = _dataContext.Users.FirstOrDefault(u => u.USR_ID == userID);
            if (user != null)
            {
                user.USR_Mail = newMail;
                user.USR_MailActiveCode = activeStr;
                try
                {
                    _dataContext.SubmitChanges();
                    return true;
                }
                catch (ChangeConflictException)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 激活邮箱
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="activeStr">激活码</param>
        /// <returns>激活码正确且未激活返回true，否则返回false</returns>
        public User ActiveMail(string userID, string activeStr)
        {
            User user = _dataContext.Users.FirstOrDefault(u => u.USR_ID == userID && u.USR_MailActiveCode == activeStr);

            if (user != null)
            {
                user.USR_MailActiveCode = null;
                try
                {
                    _dataContext.SubmitChanges();
                    return user;
                }
                catch (ChangeConflictException)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 插入用户实体
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User InsertUser(User user)
        {
            try
            {
                _dataContext.Users.InsertOnSubmit(user);
                _dataContext.SubmitChanges();
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return null;
            }   
        }

        /// <summary>
        /// 更新用户名称、邮箱、所在院系ID、所在院系名称
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User ModifyUserInfo(User user)
        {
            try
            {
                User oldUser = _dataContext.Users.Single(x => x.USR_ID == user.USR_ID);
                oldUser.USR_Name = user.USR_Name;
                oldUser.USR_Mail = user.USR_Mail;
                oldUser.USR_MailActiveCode = user.USR_MailActiveCode;
                oldUser.USR_DepartID = user.USR_DepartID;
                oldUser.USR_DepartName = user.USR_DepartName;
                _dataContext.SubmitChanges();
                return oldUser;
            }
            catch (Exception)
            {
                return null;
            }
        }
       

        /// <summary>
        /// 修改用户上次登录时间和IP
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="loginTime">上次登录时间</param>
        /// <param name="loginIP">上次登录IP</param>
        /// <returns>true修改成功</returns>
        public bool ModifyUserInfo(string userID, string loginTime, string loginIP)
        {
            int count = _dataContext.ExecuteCommand("update Users set USR_LastLoginTime={0},"
                                                       + "USR_LastLoginIP={1} "
                                                       + "where USR_ID={2}",
                                                       loginTime, loginIP, userID);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据条件查询用户（废弃）
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="departmentID"></param>
        /// <param name="roleID"></param>
        /// <param name="userStatus"></param>
        /// <returns></returns>
        public IList QueryUser(string userID, string userName, int? departmentID, int roleID, bool? userStatus)
        {
            


            //string linqStr = "USR_ID.Contains(@0) && USR_Name.Contains(@1)";
            //object[] objs = new object[5];
            //objs[0] = userID;
            //objs[1] = userName;
            //int i = 2;
            //if (departmentID != null && departmentID > 0)
            //{
            //    linqStr += " && USR_DepartID==@" + i;
            //    objs[i] = departmentID;
            //    i++;
            //}
            //if (roleID > 0)
            //{
            //    linqStr += " && USR_RoleID==@" + i;
            //    objs[i] = roleID;
            //    i++;
            //}
            //if (userStatus !=null)
            //{
            //    linqStr += " && USR_Status==@" + i;
            //    objs[i] = userStatus;
            //    i++;
            //}

            string linqStr = "UserID.Contains(@0) && UserName.Contains(@1)";
            object[] objs = new object[5];
            objs[0] = userID;
            objs[1] = userName;
            int i = 2;
            if (departmentID != null && departmentID > 0)
            {
                linqStr += " && departmentID==@" + i;
                objs[i] = departmentID;
                i++;
            }
            if (roleID > 0)
            {
                linqStr += " && RoleID==@" + i;
                objs[i] = roleID;
                i++;
            }
            if (userStatus != null)
            {
                linqStr += " && userStatus==@" + i;
                objs[i] = userStatus;
                i++;
            }
            var list = (from u in _dataContext.Users
                        from r in _dataContext.Roles
                        where u.USR_RoleID==r.RL_ID
                        select new UserAndRole
                        {
                            UserID = u.USR_ID,
                            UserName = u.USR_Name,
                            UserMail = u.USR_Mail,
                            DepartmentID = u.USR_DepartID,
                            DepartmentName = u.USR_DepartName,
                            RoleID = r.RL_ID,
                            RoleName = r.RL_Name,
                            UserStatus = u.USR_Status,
                            InnerStatusName = u.USR_Status
                        }).Where(linqStr, objs);
            //var list = _dataContext.Users.Where(linqStr, objs);
            return list.ToList();
        }

        /// <summary>
        /// 根据条件查询用户（废弃）
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="departmentID"></param>
        /// <param name="roleID"></param>
        /// <param name="userStatus"></param>
        /// <returns></returns>
        public IList QueryUser(string userID, string userName, int? departmentID, int roleID, bool? userStatus, int skipItems, int pageSize)
        {
            string linqStr = "UserID.Contains(@0) && UserName.Contains(@1)";
            object[] objs = new object[5];
            objs[0] = userID;
            objs[1] = userName;
            int i = 2;
            if (departmentID != null && departmentID > 0)
            {
                linqStr += " && departmentID==@" + i;
                objs[i] = departmentID;
                i++;
            }
            if (roleID > 0)
            {
                linqStr += " && RoleID==@" + i;
                objs[i] = roleID;
                i++;
            }
            if (userStatus != null)
            {
                linqStr += " && userStatus==@" + i;
                objs[i] = userStatus;
                i++;
            }
            var list = (from u in _dataContext.Users
                        from r in _dataContext.Roles
                        where u.USR_RoleID == r.RL_ID
                        select new UserAndRole
                        {
                            UserID = u.USR_ID,
                            UserName = u.USR_Name,
                            UserMail = u.USR_Mail,
                            DepartmentID = u.USR_DepartID,
                            DepartmentName = u.USR_DepartName,
                            RoleID = r.RL_ID,
                            RoleName = r.RL_Name,
                            UserStatus = u.USR_Status,
                            InnerStatusName = u.USR_Status
                        }).Where(linqStr, objs).Skip(skipItems).Take(pageSize);
            return list.ToList();
        }

        /// <summary>
        /// 查询用户个数（废弃）
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="departmentID"></param>
        /// <param name="roleID"></param>
        /// <param name="userStatus"></param>
        /// <returns></returns>
        public int QueryUserCount(string userID, string userName, int? departmentID, int roleID, bool? userStatus)
        {
            string linqStr = "UserID.Contains(@0) && UserName.Contains(@1)";
            object[] objs = new object[5];
            objs[0] = userID;
            objs[1] = userName;
            int i = 2;
            if (departmentID != null && departmentID > 0)
            {
                linqStr += " && departmentID==@" + i;
                objs[i] = departmentID;
                i++;
            }
            if (roleID > 0)
            {
                linqStr += " && RoleID==@" + i;
                objs[i] = roleID;
                i++;
            }
            if (userStatus != null)
            {
                linqStr += " && userStatus==@" + i;
                objs[i] = userStatus;
                i++;
            }
            var count = (from u in _dataContext.Users
                        from r in _dataContext.Roles
                        where u.USR_RoleID == r.RL_ID
                        select new UserAndRole
                        {
                            UserID = u.USR_ID,
                            UserName = u.USR_Name,
                            UserMail = u.USR_Mail,
                            DepartmentID = u.USR_DepartID,
                            DepartmentName = u.USR_DepartName,
                            RoleID = r.RL_ID,
                            RoleName = r.RL_Name,
                            UserStatus = u.USR_Status
                        }).Where(linqStr, objs).Count();
            return count;
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public UserInfo GetUser(string userID)
        {
            var list = from u in _dataContext.Users
                       join r in _dataContext.Roles on u.USR_RoleID equals r.RL_ID
                       where u.USR_ID == userID
                       select new UserInfo
                       {
                           UserID = u.USR_ID,
                           UserName = u.USR_Name,
                           UserMail = u.USR_Mail,
                           DepartmentID = u.USR_DepartID,
                           DepartmentName = u.USR_DepartName,
                           RoleID = r.RL_ID,
                           RoleName = r.RL_Name,
                           UserStatus = u.USR_Status,
                           LastLoginIP = u.USR_LastLoginIP,
                           LastLoginTime = u.USR_LastLoginTime
                       };
            return list.FirstOrDefault();
        }

        /// <summary>
        /// 修改用户状态
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool ChangeUserStatus(string userID, bool status)
        {
            try
            {
                User user = _dataContext.Users.Single(x => x.USR_ID == userID);
                user.USR_Status = status;
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改用户角色
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public bool ModifyUserRole(string userID, int roleID)
        {
            try
            {
                User user = _dataContext.Users.Single(x => x.USR_ID == userID);
                user.USR_RoleID = roleID;
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改用户房间
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="oldRoomID"></param>
        /// <param name="newRoomID"></param>
        /// <returns></returns>
        public bool ModifyUserRoom(string userID, int oldRoomID, int newRoomID)
        {
            try
            {
                UserRoom ur = _dataContext.UserRooms.SingleOrDefault(x => x.USR_ID == userID && x.RI_ID == oldRoomID);
                _dataContext.UserRooms.DeleteOnSubmit(ur);
                ur = new UserRoom();
                ur.USR_ID = userID;
                ur.RI_ID = newRoomID;
                _dataContext.UserRooms.InsertOnSubmit(ur);
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取所有用户信息
        /// </summary>
        /// <returns></returns>
        public IQueryable<UserInfo> GetUserInfo()
        {
            var list = from u in _dataContext.Users
                       join r in _dataContext.Roles on u.USR_RoleID equals r.RL_ID
                       join ur in _dataContext.UserRooms on u.USR_ID equals ur.USR_ID
                       join ri in _dataContext.RoomInfos on ur.RI_ID equals ri.RI_ID
                       join bbi in _dataContext.BuildingBriefInfos on ri.BDI_ID equals bbi.BDI_ID
                       join sai in _dataContext.SchoolAreaInfos on bbi.SAI_ID equals sai.SAI_ID
                       join si in _dataContext.SchoolInfos on sai.SI_ID equals si.SI_ID
                       select new UserInfo
                       {
                           UserID = u.USR_ID,
                           UserName = u.USR_Name,
                           UserMail = u.USR_Mail,
                           DepartmentID = u.USR_DepartID,
                           DepartmentName = u.USR_DepartName,
                           RoleID = r.RL_ID,
                           RoleName = r.RL_Name,
                           RoomID = ri.RI_ID,
                           RoomName = si.SI_Name + ">" + sai.SAI_Name + ">" + bbi.BDI_Name + ">" + ri.RI_RoomCode,
                           UserStatus = u.USR_Status,
                           LastLoginIP = u.USR_LastLoginIP,
                           LastLoginTime = u.USR_LastLoginTime
                       };
            return list;
        }

        /// <summary>
        /// 查看邮箱是否被使用
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public bool IsMailUsed(string mail)
        {
            if (_dataContext.Users.Where(x => x.USR_Mail == mail && x.USR_MailActiveCode == null).Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
