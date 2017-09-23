using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class AreaAndSchool
    {
        public int AreaID { get; set; }
        public string AreaName { get; set; }
        public string AreaRemark { get; set; }
        public int SchoolID { get; set; }
        public string SchoolName { get; set; }
    }
}
