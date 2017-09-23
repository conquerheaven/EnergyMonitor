using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Interface;
using System.Collections;
 

namespace EnergyMonitor.Models.Repository.Implement
{
    public class FunctionRepos : IFunctionRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public FunctionRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region IFunctionRepos Members

        /// <summary>
        /// 获取所有权限
        /// </summary>
        /// <returns></returns>
        public IList GetAllFuns()
        {
            return _dataContext.Functions.ToList();
        }

        /// <summary>
        /// 添加角色功能
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="funIDs"></param>
        /// <returns></returns>
        public bool AddRoleFuns(int roleID, string[] funIDs)
        {
            try
            {
                var list = new List<RoleFunction>();
                foreach (string funID in funIDs)
                {
                    RoleFunction rf = new RoleFunction();
                    rf.RF_RoleID = roleID;
                    rf.RF_FunID = funID;
                    list.Add(rf);
                }
                _dataContext.RoleFunctions.InsertAllOnSubmit(list);
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取用户所拥有的功能
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public IList<Function> GetRoleFuns(int roleID)
        {
            var list = from f in _dataContext.Functions
                       join rf in _dataContext.RoleFunctions on f.FN_ID equals rf.RF_FunID
                       where rf.RF_RoleID == roleID
                       select f;
            return list.ToList();
        }

        /// <summary>
        /// 获取用户所拥有的功能ID
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public IList<string> GetRoleFunIDs(int roleID)
        {
            return _dataContext.RoleFunctions.Where(x => x.RF_RoleID == roleID).Select(x => x.RF_FunID).ToList();
        }

        /// <summary>
        /// 获取用户所拥有的功能路径
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public IList<string> GetRoleFunLinks(int roleID)
        {
            var list = from f in _dataContext.Functions
                       join rf in _dataContext.RoleFunctions on f.FN_ID equals rf.RF_FunID
                       where rf.RF_RoleID == roleID
                       select f.FN_LinkLocation;
            return list.ToList();
        }

        /// <summary>
        /// 删除角色拥有的功能
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public bool DeleteRoleFuns(int roleID)
        {
            try
            {
                if (_dataContext.ExecuteCommand(@"delete from RoleFunction where RF_RoleID=" + roleID) > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}
