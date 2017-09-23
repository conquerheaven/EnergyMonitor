using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class EnergyHistory
    {
        public int PNO { get; set; }
        public string RoomName { get; set; }
        public string Buyer { get; set; }
        public double BuyMoney { get; set; }
        public double BuyVal { get; set; }
        public string OperType { get; set; }
        public DateTime BuyDate { get; set; }

        public string BuyMoneyStr
        {
            get
            {
                return BuyMoney.ToString("f1");
            }
        }
        public string BuyValStr
        {
            get
            {
                return BuyVal.ToString("f1");
            }
        }
        public string BuyDateStr
        {
            get
            {
                return BuyDate.ToString("yyyy-MM-dd");
            }
        }
    }
}
