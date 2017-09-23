using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class RoomAndBuilding
    {
        public int RoomID { get; set; }
        public string RoomCode { get; set; }
        public int? Floor { get; set; }
        public string Remark { get; set; }

        public int BuildingID { get; set; }
        public string BuildingName { get; set; }

        public int AreaID { get; set; }
        public string AreaName { get; set; }

        public int SchoolID { get; set; }
        public string SchoolName { get; set; }
    }
}
