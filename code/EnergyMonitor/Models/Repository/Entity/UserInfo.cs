
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class UserInfo
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserMail { get; set; }
        public int? DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public bool UserStatus { get; set; }
        public string StatusName
        {
            get
            {
                if (UserStatus)
                {
                    return "可用";
                }
                else
                {
                    return "不可用";
                }
            }
        }
        public string LastLoginIP { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public string LastLoginTimeStr
        {
            get
            {
                return LastLoginTime == null ? null : LastLoginTime.Value.ToString("yyyy-MM-dd");
            }
        }
    }
}
