using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class ExceptionEntity
    {
        public int  AnalogNo{ get; set; }
        public string AnalogName { get; set; }
        public DateTime ExceptionDate { get; set; }
        public string ExceptionName { get; set; }
    }
}
