
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class RoleAndUserCount
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public int UserCount { get; set; }
        public bool Status { get; set; }

    }
}
