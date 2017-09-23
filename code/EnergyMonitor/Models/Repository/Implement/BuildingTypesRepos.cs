using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Interface;

namespace EnergyMonitor.Models.Repository.Implement
{
    public class BuildingTypesRepos : IBuildingTypesRepos
    {
        private EnergyMonitorDataContext _dateContext;

        public BuildingTypesRepos()
        {
            _dateContext = new EnergyMonitorDataContext();
        }

        #region IBuildingTypesRepos Members

        /// <summary>
        /// 得到所有建筑类型信息
        /// </summary>
        /// <returns></returns>
        public IList<BuildingTypes> GetAllBuildingTypes()
        {
            return _dateContext.BuildingTypes.ToList();
        }

        #endregion
    }
}
