using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IRoleRepos
    {
        /// <summary>
        /// 查询所有角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        IList GetAllRoles();

        /// <summary>
        /// 查询非学生角色
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        Role GetRole(int roleID);

        /// <summary>
        /// 查询非学生角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        IList GetRoles(string roleName);

        /// <summary>
        /// 查询角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        IList GetRoles(string roleName, int skipItems, int pageSize);

        /// <summary>
        /// 根据角色名查询非学生角色个数
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        int GetRolesCount(string roleName);

        /// <summary>
        /// 获取指定角色的用户数
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        int GetRoleUserCount(int roleID);

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns>返回添加生成的ID，失败返回-1</returns>
        int AddRole(string roleName);

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        bool DeleteRole(int roleID);

        /// <summary>
        /// 修改角色名
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        bool ModifyRoleName(int roleID, string roleName);

         /// <summary>
        /// 查询角色名称是否存在
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        bool IsRoleNameExist(string roleName);
    }
}
