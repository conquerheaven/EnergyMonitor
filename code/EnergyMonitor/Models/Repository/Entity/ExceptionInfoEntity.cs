using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class ExceptionInfoEntity
    {
        public int AnalogNo { get; set; }
        public string AnalogName { get; set; }       
        public double HistoryVal { get; set; }
        public double CurrentVal { get; set; }    
       public string ExceptionName { get; set; }
        public string TimeBlock { get; set; }
        public string NewTimeBlock { get; set; }
        public string SName{ get; set; }
        public string AName{ get; set; }
        public string BName{ get; set; }
        public string RName { get; set; }
        public DateTime ExceptionDate
        {
            set
            {
                TimeBlock = value.ToString("yyyy-MM-dd HH:MM:ss");
                NewTimeBlock = value.ToString("yyyy-MM-dd");
            }
        }
        public string HistoryValS
        {
            get{
                return  HistoryVal.ToString("f1");                
            }
        }
        public string CurrentValS
        {
            get
            {
                return CurrentVal.ToString("f1");
            }
        }        
    }
}
