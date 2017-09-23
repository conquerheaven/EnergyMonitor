using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class TextEntity
    {
        public int PNO { get; set; }
        public DateTime Time{ get; set; }
        public double Val { get; set; }
    }
}