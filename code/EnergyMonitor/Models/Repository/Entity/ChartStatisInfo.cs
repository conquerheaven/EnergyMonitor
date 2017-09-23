using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
namespace EnergyMonitor.Models.Repository.Entity
{
    public class ChartStatisInfo
    {
        public IList<ChartStatisEntity> ChartData { get; set; }
        public PowerClass PowerType { get; set; }
        public BuildingBriefInfo BuildingInfo { get; set; }
        public int Year { get; set; }
    }
}
