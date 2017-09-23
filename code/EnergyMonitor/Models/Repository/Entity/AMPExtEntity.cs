using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class AMPExtEntity : IEquatable<AMPExtEntity>
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

        public int? ParentNo { get; set; }

        public int? ObjType
        {
            get
            {
                if (SchoolID > 0 && AreaID == 0)
                {
                    return 1;
                }
                if (AreaID > 0 && BuildingID == 0)
                {
                    return 2;
                }
                if (BuildingID > 0 && RoomID == 0)
                {
                    return 3;
                }
                if (RoomID > 0)
                {
                    return 4;
                }
                return null;
            }
        }
        public int? ObjIDs
        {
            get
            {
                if (SchoolID > 0 && AreaID == 0)
                {
                    return SchoolID;
                }
                if (AreaID > 0 && BuildingID == 0)
                {
                    return AreaID;
                }
                if (BuildingID > 0 && RoomID == 0)
                {
                    return BuildingID;
                }
                if (RoomID > 0)
                {
                    return RoomID;
                }
                return null;
            }
        }

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

                    //string str = SName + AName + BName + RName;
                    //return String.IsNullOrWhiteSpace(str) ? "未知对象" : str;
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
        public string Unit { get; set; }
        public string PowerType { get; set; }
        public string PowerName { get; set; }
        public int RealFlag { get; set; }
        public int StatFlag { get; set; }

        public string RealFlagStr
        {
            get
            {
                return RealFlag == 1 ? "真实点" : "虚拟点";
            }
        }
        public string StatFlagStr
        {
            get
            {
                return StatFlag == 1 ? "统计点" : "非统计点";
            }
        }
        public string ParentNoStr
        {
            get
            {
                if (ParentNo.HasValue && ParentNo.Value > 1)
                {
                    return ParentNo.Value.ToString();
                }
                else
                {
                    return "无";
                }
            }
        }

        public int? RTU_NO{get; set;}
        public int? AI_Serial { get; set; }
        public double? AI_Rate { get; set; }
        public double? AI_Base { get; set; }

        public bool Equals(AMPExtEntity other)
        {
            if (PNO == other.PNO) return true;
            else return false;
        }

        public static IEqualityComparer<AMPExtEntity> CompareByID = new AMPExtEntityEqualityComparer();

        private class AMPExtEntityEqualityComparer : IEqualityComparer<AMPExtEntity>
        {
            public bool Equals(AMPExtEntity a, AMPExtEntity b)
            {
                return a.PNO == b.PNO;
            }

            public int GetHashCode(AMPExtEntity obj)
            {
                return obj.PNO;
            }
        }
        public string Encoding { get; set; }
    }
}
