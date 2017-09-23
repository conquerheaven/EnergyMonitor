using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Interface;
using System.Collections;
using EnergyMonitor.Models.Repository.Entity;


namespace EnergyMonitor.Models.Repository.Implement
{
    public class RoleRepos : IRoleRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public RoleRepos() 
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region IRoleRepos Members

        /// <summary>
        /// 查询所有角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public IList GetAllRoles()
        {
            var list = from r in _dataContext.Roles
                       select r;
            return list.ToList();
        }

        /// <summary>
        /// 查询角色
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public Role GetRole(int roleID)
        {
            return _dataContext.Roles.SingleOrDefault(x => x.RL_ID == roleID);
        }

        /// <summary>
        /// 查询非学生角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public IList GetRoles(string roleName)
        {
            var list = _dataContext.Roles.Where(x => x.RL_ID > 1 && x.RL_Name.Contains(roleName));
            return list.ToList();
        }

        /// <summary>
        /// 查询非学生角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public IList GetRoles(string roleName, int skipItems, int pageSize)
        {
            var list = _dataContext.Roles.Where(x => x.RL_ID > 1 && x.RL_Name.Contains(roleName)).Skip(skipItems).Take(pageSize);
            return list.ToList();
        }

        /// <summary>
        /// 根据角色名查询非学生角色个数
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public int GetRolesCount(string roleName)
        {
            var count = _dataContext.Roles.Where(x => x.RL_ID > 1 && x.RL_Name.Contains(roleName)).Count();
            return count;
        }

        /// <summary>
        /// 获取指定角色的用户数
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public int GetRoleUserCount(int roleID)
        {
            int count = _dataContext.Users.Where(x => x.USR_RoleID == roleID).Count();
            return count;
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns>返回添加生成的ID，失败返回-1</returns>
        public int AddRole(string roleName)
        {
            try
            {
                Role role = new Role();
                role.RL_Name = roleName;
                role.RL_Status = true;
                _dataContext.Roles.InsertOnSubmit(role);
                _dataContext.SubmitChanges();
                return role.RL_ID;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public bool DeleteRole(int roleID)
        {
            try
            {
                _dataContext.Roles.DeleteOnSubmit(_dataContext.Roles.Single(x => x.RL_ID == roleID));
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改角色名
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public bool ModifyRoleName(int roleID, string roleName)
        {
            try
            {
                Role role = _dataContext.Roles.Single(x => x.RL_ID == roleID);
                role.RL_Name = roleName;
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 查询角色名称是否存在
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public bool IsRoleNameExist(string roleName)
        {
            if (_dataContext.Roles.Where(x => x.RL_Name == roleName).Count() > 0)
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}
