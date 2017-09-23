using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IFunctionRepos
    {
        /// <summary>
        /// 获取所有权限
        /// </summary>
        /// <returns></returns>
        IList GetAllFuns();

        /// <summary>
        /// 添加角色功能
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="funIDs"></param>
        /// <returns></returns>
        bool AddRoleFuns(int roleID, string[] funIDs);

        /// <summary>
        /// 获取用户所拥有的功能
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        IList<Function> GetRoleFuns(int roleID);

        /// <summary>
        /// 获取用户所拥有的功能ID
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        IList<string> GetRoleFunIDs(int roleID);

        /// <summary>
        /// 获取用户所拥有的功能路径
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        IList<string> GetRoleFunLinks(int roleID);

        /// <summary>
        /// 删除角色拥有的功能
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        bool DeleteRoleFuns(int roleID);
    }
}
