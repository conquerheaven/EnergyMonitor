using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Interface;
using System.Collections;
using EnergyMonitor.Models.Repository.Entity;


namespace EnergyMonitor.Models.Repository.Implement
{
    public class SchoolAreaRepos : ISchoolAreaRepos
    {
        private EnergyMonitorDataContext _dataContext; 

        public SchoolAreaRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region ISchoolAreaRepos Members

        /// <summary>
        /// 查询区域
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="schoolID"></param>
        /// <returns></returns>
        public IList GetSchoolArea(string areaName, int schoolID)
        {
            if (schoolID > 0)
            {
                var list = from sa in _dataContext.SchoolAreaInfos
                           from s in _dataContext.SchoolInfos
                           where sa.SI_ID == s.SI_ID && sa.SI_ID == schoolID && sa.SAI_Name.Contains(areaName)
                           select new AreaAndSchool
                           {
                               AreaID = sa.SAI_ID,
                               AreaName = sa.SAI_Name,
                               AreaRemark = sa.SAI_Remark,
                               SchoolID = sa.SI_ID,
                               SchoolName = s.SI_Name
                           };
                return list.ToList();
            }
            else
            {
                var list = from sa in _dataContext.SchoolAreaInfos
                           from s in _dataContext.SchoolInfos
                           where sa.SI_ID == s.SI_ID && sa.SAI_Name.Contains(areaName)
                           select new AreaAndSchool
                           {
                               AreaID = sa.SAI_ID,
                               AreaName = sa.SAI_Name,
                               AreaRemark = sa.SAI_Remark,
                               SchoolID = sa.SI_ID,
                               SchoolName = s.SI_Name
                           };
                return list.ToList();
            }
        }

        /// <summary>
        /// 查询区域
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="schoolID"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList GetSchoolArea(string areaName, int schoolID, int skipItems, int pageSize)
        {
            if (schoolID > 0)
            {
                var list = (from sa in _dataContext.SchoolAreaInfos
                            from s in _dataContext.SchoolInfos
                            where sa.SI_ID == s.SI_ID && sa.SI_ID == schoolID && sa.SAI_Name.Contains(areaName)
                            select new AreaAndSchool
                            {
                                AreaID = sa.SAI_ID,
                                AreaName = sa.SAI_Name,
                                AreaRemark = sa.SAI_Remark,
                                SchoolID = sa.SI_ID,
                                SchoolName = s.SI_Name
                            }).Skip(skipItems).Take(pageSize);
                return list.ToList();
            }
            else
            {
                var list = (from sa in _dataContext.SchoolAreaInfos
                            from s in _dataContext.SchoolInfos
                            where sa.SI_ID == s.SI_ID && sa.SAI_Name.Contains(areaName)
                            select new AreaAndSchool
                            {
                                AreaID = sa.SAI_ID,
                                AreaName = sa.SAI_Name,
                                AreaRemark = sa.SAI_Remark,
                                SchoolID = sa.SI_ID,
                                SchoolName = s.SI_Name
                            }).Skip(skipItems).Take(pageSize);
                return list.ToList();
            }
        }

        /// <summary>
        /// 查询校区下所有区域
        /// </summary>
        /// <param name="schoolID"></param>
        /// <returns></returns>
        public IList<SchoolAreaInfo> GetSchoolAllArea(int schoolID)
        {
            var list = from sai in _dataContext.SchoolAreaInfos
                       where sai.SI_ID == schoolID
                       select sai;
            return list.ToList<SchoolAreaInfo>();
        }


        /// <summary>
        /// 获取区域和校区
        /// </summary>
        /// <param name="areaID"></param>
        /// <returns></returns>
        public AreaAndSchool GetAreaAndSchool(int areaID)
        {
            var list = from sa in _dataContext.SchoolAreaInfos
                       from s in _dataContext.SchoolInfos
                       where sa.SI_ID == s.SI_ID && sa.SAI_ID == areaID
                       select new AreaAndSchool
                       {
                           AreaID = sa.SAI_ID,
                           AreaName = sa.SAI_Name,
                           AreaRemark = sa.SAI_Remark,
                           SchoolID = sa.SI_ID,
                           SchoolName = s.SI_Name
                       };
            return list.Single();
        }

        /// <summary>
        /// 获取区域和校区
        /// </summary>
        /// <param name="areaID"></param>
        /// <returns></returns>
        public AreaAndSchool GetSchool(int schoolID)
        {
            var list = from s in _dataContext.SchoolInfos
                       where s.SI_ID == schoolID
                       select new AreaAndSchool
                       {
                           SchoolID = s.SI_ID,
                           SchoolName = s.SI_Name
                       };
            return list.Single();
        }

        /// <summary>
        /// 获取区域个数
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="schoolID">若为非正数查询所有校区</param>
        /// <returns></returns>
        public int GetSchoolAreaCount(string areaName, int schoolID)
        {
            if (schoolID > 0)
            {
                var count = (from sa in _dataContext.SchoolAreaInfos
                             from s in _dataContext.SchoolInfos
                             where sa.SI_ID == s.SI_ID && sa.SI_ID == schoolID && sa.SAI_Name.Contains(areaName)
                             select new AreaAndSchool
                             {
                                 AreaID = sa.SAI_ID,
                                 AreaName = sa.SAI_Name,
                                 AreaRemark = sa.SAI_Remark,
                                 SchoolID = sa.SI_ID,
                                 SchoolName = s.SI_Name
                             }).Count();
                return count;
            }
            else
            {
                var count = (from sa in _dataContext.SchoolAreaInfos
                             from s in _dataContext.SchoolInfos
                             where sa.SI_ID == s.SI_ID && sa.SAI_Name.Contains(areaName)
                             select new AreaAndSchool
                             {
                                 AreaID = sa.SAI_ID,
                                 AreaName = sa.SAI_Name,
                                 AreaRemark = sa.SAI_Remark,
                                 SchoolID = sa.SI_ID,
                                 SchoolName = s.SI_Name
                             }).Count();
                return count;
            }
        }

        /// <summary>
        /// 删除区域
        /// </summary>
        /// <param name="areaID"></param>
        /// <returns></returns>
        public bool DeleteSchoolArea(int areaID)
        {
            try
            {
                _dataContext.SchoolAreaInfos.DeleteOnSubmit(_dataContext.SchoolAreaInfos.Single(p => p.SAI_ID == areaID));
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 增加区域
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="schoolID"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool AddSchoolAreaPart(string areaName, int schoolID, string remark)
        {
            try
            {
                SchoolAreaInfo schoolArea = new SchoolAreaInfo();
                schoolArea.SAI_Name = areaName;
                schoolArea.SI_ID = schoolID;
                schoolArea.SAI_Remark = remark;
                _dataContext.SchoolAreaInfos.InsertOnSubmit(schoolArea);
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改区域
        /// </summary>
        /// <param name="areaID"></param>
        /// <param name="schoolID"></param>
        /// <param name="areaName"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool ModifySchoolAreaPartInfo(int areaID, int schoolID, string areaName, string remark)
        {
            try
            {
                SchoolAreaInfo schoolArea = _dataContext.SchoolAreaInfos.Single(x => x.SAI_ID == areaID);
                if (schoolID > 0 && schoolID != schoolArea.SI_ID)
                {
                    schoolArea.SI_ID = schoolID;
                    // 修改AMP表中对应的区域校区
                    if (_dataContext.AnalogMeasurePoints.Where(x => x.AMP_SAreaID == areaID).Count() > 0)
                    {
                        _dataContext.ExecuteCommand("update AnalogMeasurePoint set AMP_SchooldID={0} where AMP_SAreaID={1}", schoolID, areaID);
                        // 修改父测点编号
                        var modifiedAMPs = _dataContext.AnalogMeasurePoints.Where(x => x.AMP_SAreaID == areaID && x.AMP_BuildingID == 0 && x.AMP_ParentNo > 1).ToList();
                        foreach (var item in modifiedAMPs)
                        {
                            var parentPoint = _dataContext.AnalogMeasurePoints.Where(x => x.AMP_CptFlag == 0 && x.AMP_Statistic == 1 && x.AMP_SchooldID == schoolID && x.AMP_SAreaID == 0 && x.AMP_PowerType == item.AMP_PowerType).SingleOrDefault();
                            if (parentPoint != null)
                            {
                                item.AMP_ParentNo = parentPoint.AMP_AnalogNo;
                            }
                            else
                            {
                                item.AMP_ParentNo = 0;
                            }
                        }
                    }
                }
                schoolArea.SAI_Name = areaName;
                schoolArea.SAI_Remark = remark;

                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取区域拥有的楼宇数
        /// </summary>
        /// <param name="areaID"></param>
        /// <returns></returns>
        public int GetBuildingCountByArea(int areaID)
        {
            return _dataContext.BuildingBriefInfos.Where(x => x.SAI_ID == areaID).Count();
        }

        /// <summary>
        /// 查询区域名称是否已经存在
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="schoolID"></param>
        /// <returns></returns>
        public bool IsAreaNameExist(string areaName, int? schoolID)
        {
            var query = _dataContext.SchoolAreaInfos.Where(x => x.SAI_Name == areaName);
            if (schoolID != null && schoolID.Value > 0)
            {
                query = query.Where(x => x.SI_ID == schoolID.Value);
            }
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
        /// 得到校区区域
        /// </summary>
        /// <param name="areaId">区域Id</param>
        /// <returns></returns>
        public SchoolAreaInfo GetSchoolArea(int areaId)
        {
            return _dataContext.SchoolAreaInfos.Where(x => x.SAI_ID == areaId).SingleOrDefault();
        }

        #endregion
    }
}
