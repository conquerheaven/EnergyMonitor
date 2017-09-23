using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.LinqEntity;

namespace EnergyMonitor.Models.Repository.Implement
{
    public class ElecDistributionInfoRepos : IElecDistributionInfoRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public ElecDistributionInfoRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region IElecDistributionInfo Members

        /// <summary>
        /// 根据建筑得到该建筑的配电室
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public IList<ElecDistributionInfo> GetSwitchingRoomsByBuildingID(int buildingID)
        {
            var list = from ed in _dataContext.ElecDistributionInfo
                          where ed.BD_ID == buildingID
                          select ed;
            return list.ToList<ElecDistributionInfo>();
        }
        #endregion
    }
}
