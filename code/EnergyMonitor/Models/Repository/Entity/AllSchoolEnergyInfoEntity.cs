using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class AllSchoolEnergyInfoEntity
    {
        public string name { get; set; }
        public string val { get; set; }
        public string valOne { get; set; }
        public string valTwo { get; set; }
        public string valThree { get; set; }
        public string SchoolName { get; set; }
        public double YearVal { get; set; }
        public double PerYearVal { get; set; }
        public double MonthVal { get; set; }
        public double PerMonthVal { get; set; }
        public double DayVal { get; set; }
        public double PerDayVal { get; set; }
        public string YearValS { get { return YearVal.ToString("f2"); } }
        public string PerYearValS { get { return PerYearVal.ToString("f2"); } }
        public string MonthValS { get { return MonthVal.ToString("f2"); } }
        public string PerMonthValS { get { return PerMonthVal.ToString("f2"); } }
        public string DayValS { get { return DayVal.ToString("f2"); } }
        public string PerDayValS { get { return PerDayVal.ToString("f2"); } }
    }
}
