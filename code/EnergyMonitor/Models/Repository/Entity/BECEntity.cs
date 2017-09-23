using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class BECEntity
    {
        public int? BuildingID { get; set; } 
        public string BuildingName {get; set;}
        public string powerType { get; set; }
        public string powerTypeName { get; set; }
        public int year { get; set; }
        public double Val { get; set; }
        public string powerUnit { get; set; }
    }
}
