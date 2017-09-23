
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class UserAndRole
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserMail { get; set; }
        public int? DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public bool UserStatus { get; set; }
        private string _statusName;
        public bool InnerStatusName {
            set {
                if (value)
                {
                    _statusName = "可用";
                }
                else
                {
                    _statusName = "不可用";
                }
            }
        }
        public string StatusName
        {
            get
            {
                return _statusName;
            }
            
        }

    }
}
