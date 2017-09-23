using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Interface;
using System.Collections;

 
namespace EnergyMonitor.Models.Repository.Implement
{
    public class SchoolRepos : ISchoolRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public SchoolRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region ISchoolRepos Members


        /// <summary>
        /// 查询校区
        /// </summary>
        /// <param name="schoolCode"></param>
        /// <param name="schoolName"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList QuerySchool(string schoolCode, string schoolName, int skipItems, int pageSize)
        {
            var list = _dataContext.SchoolInfos.Where(p => p.SI_Code.Contains(schoolCode) && p.SI_Name.Contains(schoolName));
            if (skipItems >= 0 && pageSize >= 0)
            {
                list = list.Skip(skipItems).Take(pageSize);
            }
            return list.ToList();
        }

        /// <summary>
        /// 查询校区个数
        /// </summary>
        /// <param name="schoolCode"></param>
        /// <param name="schoolName"></param>
        /// <returns></returns>
        public int QuerySchoolCount(string schoolCode, string schoolName)
        {
            var count = _dataContext.SchoolInfos.Where(p => p.SI_Code.Contains(schoolCode) && p.SI_Name.Contains(schoolName)).Count();
            return count;
        }

        /// <summary>
        /// 获取指定校区
        /// </summary>
        /// <param name="schoolID"></param>
        /// <returns></returns>
        public SchoolInfo QuerySchool(int schoolID)
        {
            return _dataContext.SchoolInfos.FirstOrDefault(p => p.SI_ID == schoolID);
        }

        /// <summary>
        /// 获取所有校区信息
        /// </summary>
        /// <returns></returns>
        public List<SchoolInfo> QueryAllSchool()
        {
            var list = from s in _dataContext.SchoolInfos select s;
            return list.ToList(); 
        }

        /// <summary>
        /// 删除校区
        /// </summary>
        /// <param name="schoolID"></param>
        /// <returns></returns>
        public bool DeleteSchool(int schoolID)
        {
            try
            {
                _dataContext.SchoolInfos.DeleteOnSubmit(_dataContext.SchoolInfos.Single(p => p.SI_ID == schoolID));
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改校区
        /// </summary>
        /// <param name="schoolID"></param>
        /// <param name="schoolCode"></param>
        /// <param name="schoolName"></param>
        /// <param name="schoolAddr"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool ModifySchoolPartInfo(int schoolID, string schoolCode, string schoolName, string schoolAddr, string remark, int buildingArea, int groudArea)
        {
            try
            {
                SchoolInfo school = _dataContext.SchoolInfos.Single(p => p.SI_ID == schoolID);
                if (!String.IsNullOrWhiteSpace(schoolName))
                {
                    school.SI_Name = schoolName;
                }
                school.SI_Code = schoolCode;
                school.SI_Address = schoolAddr;
                school.SI_Remark = remark;
                school.SI_BuildingArea = buildingArea;
                school.SI_GroudArea = groudArea;
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 增加校区
        /// </summary>
        /// <param name="schoolCode"></param>
        /// <param name="schoolName"></param>
        /// <param name="schoolAddr"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool AddSchoolPart(string schoolCode, string schoolName, string schoolAddr, string remark)
        {
            try
            {
                SchoolInfo school = new SchoolInfo();
                school.SI_Code = schoolCode;
                school.SI_Name = schoolName;
                school.SI_Address = schoolAddr;
                school.SI_Remark = remark;
                _dataContext.SchoolInfos.InsertOnSubmit(school);
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 查询指定校区拥有的区域个数
        /// </summary>
        /// <param name="schoolID"></param>
        /// <returns></returns>
        public int QueryAreaCount(int schoolID)
        {
            return _dataContext.SchoolAreaInfos.Where(x => x.SI_ID == schoolID).Count();
        }

        /// <summary>
        /// 查询校区名称是否已经存在
        /// </summary>
        /// <param name="schoolName"></param>
        /// <returns></returns>
        public bool IsSchoolNameExist(string schoolName)
        {
            var query = _dataContext.SchoolInfos.Where(x => x.SI_Name == schoolName);
            if (query.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取全校信息
        /// </summary>
        /// <returns></returns>
        public UniversityInfo GetUniversityInfo()
        {
            if (_dataContext.UniversityInfos.Count() > 0)
            {
                return _dataContext.UniversityInfos.Single();
            }
            else
            {
                return null;
            }
        }

        public void ModifyUniversityInfo(int StudentCount, float Area)
        {
            UniversityInfo info = _dataContext.UniversityInfos.Single();
            if (info != null)
            {
                info.Area = Area;
                info.StudentCount = StudentCount;
                _dataContext.SubmitChanges();
            }
        }
        #endregion
    }
}
