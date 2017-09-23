using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class TimeStatisEntity
    {
        private string timeBlock;

        public string TimeBlock
        {
            set
            {
                if (!String.IsNullOrEmpty(this.timeBlock))
                {
                    this.timeBlock = value + this.timeBlock;
                }
                else
                {
                    this.timeBlock = value;
                }
            }
            get
            {
                return this.timeBlock;
            }
        }
        public double StatisVal { set; get; }

        public string TimeBit
        {
            set
            {
                if (!String.IsNullOrEmpty(this.timeBlock))
                {
                    this.timeBlock += value;
                }
                else
                {
                    this.timeBlock = value;
                }
            }
        }

        public string StatisSVal
        {
            get
            {
                return this.StatisVal.ToString("f1");
            }
        }
    }
}
