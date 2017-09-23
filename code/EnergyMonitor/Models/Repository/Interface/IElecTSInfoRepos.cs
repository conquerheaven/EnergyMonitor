using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;

namespace EnergyMonitor.Models.Repository.Implement
{
    public interface IElecTSInfoRepos
    {
        /// <summary>
        /// 根据配电室得到该配电室的变压器
        /// </summary>
        /// <param name="SwitchingRoomID"></param>
        /// <returns></returns>
        IList<ElecTSInfo> GetTransformersBySwitchingRoomID(int SwitchingRoomID);
    }
}
