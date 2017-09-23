using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class AnalogInfoEntity
    {
        public int? RTU_NO { get; set; }
        public int? AI_Serial { get; set; }
        public double? AI_Rate { get; set; }
        public double? AI_Base { get; set; }
    }
}
