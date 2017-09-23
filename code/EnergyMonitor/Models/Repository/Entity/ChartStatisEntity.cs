using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class ChartStatisEntity
    {
        public DateTime DTime 
        { 
            set 
            {
                //Time = value;
                TimeBlock = value.ToString("yyyy-MM-dd");
            }
        }
        public DateTime MTime 
        {           
            set
            {
                //Time = value;
                TimeBlock = value.ToString("yyyy-MM");
            }
        }
        public int YTime
        {
            set
            {
                TimeBlock = value.ToString();
            }
        }
        public DateTime HTime
        {
            set
            {
                //Time = value;
                TimeBlock = value.ToString("yyyy-MM-dd HH:00:00");
            }
        }
        public int Month { get; set; }

        public string TimeBlock { get; set; }
        public string HTimeStr 
        {
            get
            {
                return Time.ToString("HH");
            }
        }
        public string HHTimeStr
        {
            get
            {
                return Time.ToString("MM-dd HH");
            }
        }
        public string MonthStr
        {
            get
            {
                return Time.ToString("MM");
            }
        }
        public string DTimeStr
        {
            get
            {
                return Time.ToString("MM-dd");
            }
        }
        public string MTimeStr
        {
            get
            {
                return Time.ToString("yyyy-MM");
            }
        }

        public string MMTimeStr
        {
            get
            {
                return Time.ToString("MM");
            }
        }
        public string DDTimeStr
        {
            get 
            {
                return Time.ToString("dd");
            }
        }

        public string YTimeStr
        {
            get
            {
                return Time.ToString("yyyy");
            }
        }
        public DateTime Time { get; set; }
        public int? ID { get; set; }
        public string Name { get; set; }
        public string PowerType { get; set; }
        public string PowerName { get; set; }
        public double StatisVal { set; get; }
        public String StatisticMode { set; get; }
        public string StatisSVal
        {
            get
            {
                if (Area.HasValue && Area.Value > 0 && StatisticMode == "unitEnergy")
                    return (StatisVal/Convert.ToDouble(Area)).ToString("f3");
                else
                  return StatisVal.ToString("f3");
            }
        }

        public double Percentage 
        {
            get 
            {
                if (Sum <= 0)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToDouble(StatisSVal) / Sum;
                }
            }
        }
        public string SPercentage
        {
            get
            {
                if (Sum <= 0)
                {
                    return "0%";
                }
                else
                {
                    return (Convert.ToDouble(StatisSVal) * 100 / Sum).ToString("f3") + "%";
                }
            }
        }

        public double Sum { get; set; }
        public int Index { get; set; }

        public String weekDay
        {
            get
            {
                if (Time != null)
                {
                    DayOfWeek dw = Time.DayOfWeek;
                    switch (dw)
                    {
                        case DayOfWeek.Monday:
                            return "星期一";
                        case DayOfWeek.Tuesday:
                            return "星期二";
                        case DayOfWeek.Wednesday:
                            return "星期三";
                        case DayOfWeek.Thursday:
                            return "星期四";
                        case DayOfWeek.Friday:
                            return "星期五";
                        case DayOfWeek.Saturday:
                            return "星期六";
                        case DayOfWeek.Sunday:
                            return "星期日";
                    }
                }
                return TimeBlock;
            }
        }
        public double? Area
        {
            get;
            set;
        }        
        public double valPerArea
        {
            get;
            set; 
        }
        public String valPerAreaStr {
            get {
                return valPerArea.ToString("f3");
            }
        }

        public int EntityIndex { get; set; }

        public string IndexName { 
            get{
                return "" + EntityIndex + Name;
            }
        }
        
    }
}
