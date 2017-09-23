using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class StateEntity
    {
        public string Type { get; set; }
        public string Info { get; set; }
        public DateTime Time { get; set; }
        public int Status { get; set; }
        public int StateNo { get; set; }
        public int ParentNo { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        
        public string TimeStr
        {
            get
            {
                return Time.ToString("yyyy-MM-dd HH:mm");
            }
        }
        public int PositionIntX
        {
            get
            {
                return Convert.ToInt32(PositionX);
            }
        }
        public int PositionIntY
        {
            get
            {
                return Convert.ToInt32(PositionY);
            }
        }
    }
}
