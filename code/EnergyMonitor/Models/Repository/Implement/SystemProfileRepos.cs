using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Interface;
using System.Collections;
 

namespace EnergyMonitor.Models.Repository.Implement
{
    public class SystemProfileRepos : ISystemProfileRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public SystemProfileRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region ISystemProfileRepos Members

        /// <summary>
        /// 根据开始字符串获取系统数据
        /// </summary>
        /// <param name="str">开始字符串</param>
        /// <returns></returns>
        public IQueryable<SystemProfile> GetByStartStr(string str)
        {
            return _dataContext.SystemProfiles.Where(x => x.SP_ID.StartsWith(str + "_"));
        }

        /// <summary>
        /// 获取所有的价格参数
        /// </summary>
        /// <returns></returns>
        public IQueryable<SystemProfile> GetAllPrice()
        {
            return GetByStartStr("price");
        }

        /// <summary>
        /// 根据键值对修改系统参数
        /// </summary>
        /// <param name="newDic"></param>
        /// <returns></returns>
        public bool Modify(IDictionary newDic)
        {
            try
            {
                foreach (var key in newDic.Keys)
                {
                    var query = _dataContext.SystemProfiles.SingleOrDefault(x => x.SP_ID == key);
                    var newVal = newDic[key].ToString();
                    if (query.SP_Value != newVal)
                    {
                        query.SP_Value = newVal;
                    }
                }
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        #endregion
    }
}
