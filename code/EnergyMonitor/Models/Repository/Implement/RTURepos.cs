using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Entity;
using System.Collections;

 
namespace EnergyMonitor.Models.Repository.Implement
{
    public class RTURepos : IRTURepos
    {
        private EnergyMonitorDataContext _dataContext;

        public RTURepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region IRTURepos Members

        /// <summary>
        /// 获取所有的RTU
        /// </summary>
        /// <returns></returns>
        public IQueryable<RTUInfo> GetAll()
        {
            var allRTU = from rtu in _dataContext.RTUs
                         select new RTUInfo
                         {
                             RTU_No = rtu.RTU_No,
                             RTU_Name = rtu.RTU_Name,
                         };
            return allRTU;
        }

        #endregion
    }
}
