using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class EnergyEntity
    {
        public int PNO { set; get; }
        public string PName { get; set; }

        public int? SchoolID { set; get; }
        public int? AreaID { set; get; }
        public int? BuildingID { set; get; }
        public int? RoomID { set; get; }

        public string SName { get; set; }
        public string AName { get; set; }
        public string BName { get; set; }
        public string RName { get; set; }

        public string Unit { get; set; }
        public string PowerType { get; set; }
        public string PowerName { get; set; }

        public DateTime RealTime { get; set; }

        private string iName;
        public string IName
        {
            set
            {
                iName = value;
            }
            get
            {
                if (iName != null)
                {
                    return iName;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(SName))
                    {
                        string tempName = SName;
                        if (!string.IsNullOrWhiteSpace(AName))
                        {
                            tempName += (">" + AName);
                        }
                        if (!string.IsNullOrWhiteSpace(BName))
                        {
                            tempName += (">" + BName);
                        }
                        if (!string.IsNullOrWhiteSpace(RName))
                        {
                            tempName += (">" + RName);
                        }
                        return tempName;
                    }
                    else
                    {
                        return "未知对象";
                    }
                }
            }
        }
        public string Time { set; get; }
        public DateTime STime {
            set
            {
                Time = value.ToString("yyyy-MM-dd HH:mm");
            }
        }
        public DateTime SecondTime
        {
            set
            {
                Time = value.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public double? Val { get; set; }
        public string ValStr
        {
            get
            {
                return Val == null ? null : Val.Value.ToString("f1");
            }
        }
        public double? RemVal { get; set; }
        public string RemValStr
        {
            get
            {
                return RemVal.HasValue ? RemVal.Value.ToString("f1") : null;
            }
        }
        public long RealTimeLong
        {
            get
            {
                return RealTime.Ticks;
            }
        }

        public double valPerArea { get; set; }
        public string valPerAreaStr {
            get {
                if (valPerArea == -1)
                {
                    return "无";
                }
                else 
                {
                    return valPerArea.ToString("f3");
                }
            }
        }
    }
}
