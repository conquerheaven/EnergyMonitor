using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;

namespace EnergyMonitor.Models.Repository.Implement
{
    public class ElecTSInfoRepos : IElecTSInfoRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public ElecTSInfoRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region IElecTSInfo Members

        /// <summary>
        /// 根据配电室得到该配电室的变压器
        /// </summary>
        /// <param name="SwitchingRoomID"></param>
        /// <returns></returns>
        public IList<ElecTSInfo> GetTransformersBySwitchingRoomID(int SwitchingRoomID)
        {
            var list = from ed in _dataContext.ElecTSInfo
                          where ed.ED_ID == SwitchingRoomID
                          select ed;
            return list.ToList<ElecTSInfo>();
        }     
        #endregion
    }
}
