using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class UserRoomFullName
    {
        public string SIName;//校区
        public string SAIName;//分校区
        public string BDIName;//楼名
        public string RICode;//房间名
        public int RIID;//房间编号
        public string URFullName;
        public string urFullName
        {
            get
            {
                return SIName + ">" + SAIName + ">" + BDIName + ">" + RICode;
            }
        }
    }
}
