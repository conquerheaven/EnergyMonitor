using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;
using EnergyMonitor.Models.Repository.Entity;

namespace EnergyMonitor.Models.Repository.Interface
{
    /// <summary>
    /// User实体操作接口 
    /// </summary>
    public interface IUserRepos
    {      
        /// <summary>
        /// 获得所有的User实体
        /// </summary>
        /// <author>WangWei</author>
        /// <date>2010-11-17</date>
        /// <returns>User泛型的List</returns>
        IList<User> ListAll();

        /// <summary>
        /// 根据用户ID得到用户实体
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns>得到用户实体，没有返回null</returns>
        User GetUserByID(string userID);
         /// <summary>
        /// 根据用户ID和密码得到用户实体
        /// </summary>
        /// by yifang
        /// <param name="userID">用户ID</param>
        /// /// <param name="password">用户密码</param>
        /// <returns>得到用户实体，没有返回null</returns>
        User GetUserByIDAndPassword(string userID, string password);
        /// <summary>
        /// 根据用户用户ID得到用户所有房间信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns>用户所有房间的集合</returns>
        IList<UserRoomFullName> GetUserRelatedRooms(string userID);

        /// <summary>
        /// 修改邮箱
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="newMail">新邮箱</param>
        /// <param name="activeStr">邮箱激活码</param>
        /// <returns>修改成功返回true，否则返回false</returns>
        bool ModifyMail(string userID, string newMail, string activeStr);

        /// <summary>
        /// 激活邮箱
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="activeStr">激活码</param>
        /// <returns>激活码正确且未激活返回true，否则返回false</returns>
        User ActiveMail(string userID, string activeStr);

        /// <summary>
        /// 插入用户实体
        /// </summary>
        /// <param name="user"></param>
        /// <returns>插入成功返回插入的实体,否则返回null</returns>
        User InsertUser(User user);

        /// <summary>
        /// 更新用户名称、邮箱、所在院系ID、所在院系名称
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        User ModifyUserInfo(User user);

        /// <summary>
        /// 修改用户上次登录时间和IP
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="loginTime">上次登录时间</param>
        /// <param name="loginIP">上次登录IP</param>
        /// <returns>true修改成功</returns>
        bool ModifyUserInfo(string userID, string loginTime, string loginIP);

        /// <summary>
        /// 根据条件查询用户（废弃）
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="departmentID"></param>
        /// <param name="roleID"></param>
        /// <param name="userStatus"></param>
        /// <returns></returns>
        IList QueryUser(string userID, string userName, int? departmentID, int roleID, bool? userStatus);

        /// <summary>
        /// 根据条件查询用户（废弃）
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="departmentID"></param>
        /// <param name="roleID"></param>
        /// <param name="userStatus"></param>
        /// <returns></returns>
        IList QueryUser(string userID, string userName, int? departmentID, int roleID, bool? userStatus, int skipItems, int pageSize);

        /// <summary>
        /// 查询用户个数（废弃）
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="departmentID"></param>
        /// <param name="roleID"></param>
        /// <param name="userStatus"></param>
        /// <returns></returns>
        int QueryUserCount(string userID, string userName, int? departmentID, int roleID, bool? userStatus);

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        UserInfo GetUser(string userID);

        /// <summary>
        /// 修改用户状态
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool ChangeUserStatus(string userID, bool status);

        /// <summary>
        /// 修改用户角色
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="roleID"></param>
        /// <returns></returns>
        bool ModifyUserRole(string userID, int roleID);

        /// <summary>
        /// 修改用户房间
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="oldRoomID"></param>
        /// <param name="newRoomID"></param>
        /// <returns></returns>
        bool ModifyUserRoom(string userID, int oldRoomID, int newRoomID);

        /// <summary>
        /// 获取所有用户信息
        /// </summary>
        /// <returns></returns>
        IQueryable<UserInfo> GetUserInfo();

        /// <summary>
        /// 查看邮箱是否被使用
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        bool IsMailUsed(string mail);
    }
}
