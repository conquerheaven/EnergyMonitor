using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IBuildingTypesRepos
    {
        /// <summary>
        /// 得到所有建筑类型信息
        /// </summary>
        /// <returns></returns>
        IList<BuildingTypes> GetAllBuildingTypes();
    }
}
