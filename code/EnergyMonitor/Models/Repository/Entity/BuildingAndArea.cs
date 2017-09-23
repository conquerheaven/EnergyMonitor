using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class BuildingAndArea
    {
        public int CampusID { get; set; }
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
        public int AreaID { get; set; }
        public string AreaName { get; set; }
        public string BuildingCode { get; set; }
    }
}
