using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
namespace EnergyMonitor.Models.Entity
{
    public class AreaAndBuilding
    {
        public string AreaName { get; set; }
        public int AreaID { get; set; }
        public IList<BuildingBriefInfo> buildingInfo{ get; set; }
    }
}
