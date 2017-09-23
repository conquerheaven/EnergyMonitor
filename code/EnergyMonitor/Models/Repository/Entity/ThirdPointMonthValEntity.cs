using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class ThirdPointMonthValEntity
    {
        public int PNO { set; get; }
        public string PName { get; set; }

        public int? SchoolID { set; get; }
        public int? AreaID { set; get; }
        public int? BuildingID { set; get; }
        public int? RoomID { set; get; }

        public DateTime month { 
            set{
                monthStr = value.ToString("yyyy-MM");
            }
        }
        public String monthStr { set; get; }

        public double? val { set; get; }
    }
}
