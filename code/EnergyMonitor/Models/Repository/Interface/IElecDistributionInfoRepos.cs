using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IElecDistributionInfoRepos
    {
        /// <summary>
        /// 根据建筑得到该建筑的配电室
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        IList<ElecDistributionInfo> GetSwitchingRoomsByBuildingID(int buildingID);
    }
}
