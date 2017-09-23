using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class BuildingGuanghuaEntity
    {
        public string SwitchingRoom { get; set; }
        public string Transformer { get; set; }
        public string CntrNo { get; set; }
        public string Name { get; set; }
        public string StartVal
        {
            get
            {
                if (StartTimeVal != null)
                    return Convert.ToDouble(StartTimeVal).ToString("f1");
                else return "/";
            }
        }
        public string EndVal
        {
            get
            {
                if (EndTimeVal != null)
                    return Convert.ToDouble(EndTimeVal).ToString("f1");
                else return "/";
            }
        }
        public double? StartTimeVal { get; set; }
        public double? EndTimeVal { get; set; }
        public string StatisVal
        {
            get
            {
                if (StartTimeVal != null)
                    return (Convert.ToDouble(EndTimeVal) - Convert.ToDouble(StartTimeVal)).ToString("f1");
                else if (EndTimeVal != null)
                    return Convert.ToDouble(EndTimeVal).ToString("f1");
                else
                    return "/";
            }
        }
    }
}
