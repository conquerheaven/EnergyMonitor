using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class UserRealTimeEnergy
    {
        public string UserID { set; get; }
        public string UserName { set; get; }
        public int RoomID { set; get; }
        public string RoomName { set; get; }
        public int AmpID { set; get; }
        public string AmpName { set; get; }
        public double AmpVal { set; get; }
        public DateTime AmpDate { set; get; }
    }
}
