using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;
using System.Linq.Dynamic;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Entity;


namespace EnergyMonitor.Models.Repository.Implement
{
    /// <summary>
    /// AnalogMeasurePoint实体类数据操作类
    /// </summary>
    public class AMPRepos : IAMPRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public AMPRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region IAMPRepos Members

        /// <summary>
        /// 得到房间测点实时数据
        /// </summary>
        /// <param name="roomID">房间号</param>
        /// <returns></returns>
        public IList<AnalogMeasurePoint> GetRealTimeEnergy(int roomID)
        {
            var list = from amp in _dataContext.AnalogMeasurePoints
                       where amp.AMP_RoomID == roomID
                       select amp;
            return list.ToList();
        }

        /// <summary>
        /// 查询校区实时测点表值个数
        /// </summary>
        /// <param name="schoolIDs">校区ID数组</param>
        /// <param name="powerTypes">能耗类型</param>
        /// <returns></returns>
        public int GetRealEnergyBySchoolCount(int?[] schoolIDs, string[] powerTypes)
        {
            var count = (from amp in _dataContext.AnalogMeasurePoints
                         where schoolIDs.Contains(amp.AMP_SchooldID)
                         && powerTypes.Contains(amp.AMP_PowerType)
                         select amp).Count();
            return count;
        }

        /// <summary>
        /// 查询校区实时测点表值
        /// </summary>
        /// <param name="schoolIDs">校区ID数组</param>
        /// <param name="powerTypes">能耗类型</param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<EnergyEntity> GetRealEnergyBySchool(int?[] schoolIDs, string[] powerTypes, int skipItems, int pageSize)
        {
            var list = (from amp in _dataContext.AnalogMeasurePoints
                        from si in _dataContext.SchoolInfos
                        where amp.AMP_SchooldID == si.SI_ID
                        && schoolIDs.Contains(amp.AMP_SchooldID)
                        && powerTypes.Contains(amp.AMP_PowerType)
                        select new EnergyEntity
                        {
                            PNO = amp.AMP_AnalogNo,
                            IName = si.SI_Name,
                            STime = amp.AMP_Date,
                            Val = amp.AMP_Val,
                            RemVal = amp.AMP_ValRem,
                            Unit = amp.AMP_Unit,
                            PowerName = amp.AMP_PowerName
                        }
                       ).Skip(skipItems).Take(pageSize);
            return list.ToList();
        }

        /// <summary>
        /// 查询区域实时测点表值个数
        /// </summary>
        /// <param name="areaIDs">区域ID数组</param>
        /// <param name="powerTypes">能耗类型</param>
        /// <returns></returns>
        public int GetRealEnergyByAreaCount(int?[] areaIDs, string[] powerTypes)
        {
            var count = (from amp in _dataContext.AnalogMeasurePoints
                         where areaIDs.Contains(amp.AMP_SAreaID)
                         && powerTypes.Contains(amp.AMP_PowerType)
                         select amp).Count();
            return count;
        }

        /// <summary>
        /// 查询区域实时测点表值
        /// </summary>
        /// <param name="areaIDs">区域ID数组</param>
        /// <param name="powerTypes"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<EnergyEntity> GetRealEnergyByArea(int?[] areaIDs, string[] powerTypes, int skipItems, int pageSize)
        {
            var list = (from amp in _dataContext.AnalogMeasurePoints
                        from sai in _dataContext.SchoolAreaInfos
                        where amp.AMP_SAreaID == sai.SAI_ID
                        && areaIDs.Contains(amp.AMP_SAreaID)
                        && powerTypes.Contains(amp.AMP_PowerType)
                        select new EnergyEntity
                        {
                            PNO = amp.AMP_AnalogNo,
                            IName = sai.SAI_Name,
                            STime = amp.AMP_Date,
                            Val = amp.AMP_Val,
                            RemVal = amp.AMP_ValRem,
                            Unit = amp.AMP_Unit,
                            PowerName = amp.AMP_PowerName
                        }
                       ).Skip(skipItems).Take(pageSize);
            return list.ToList();
        }

        /// <summary>
        /// 查询楼宇实时测点表值个数
        /// </summary>
        /// <param name="buildingIDs"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        public int GetRealEnergyByBuildingCount(int?[] buildingIDs, string[] powerTypes)
        {
            var count = (from amp in _dataContext.AnalogMeasurePoints
                         where buildingIDs.Contains(amp.AMP_BuildingID)
                         && powerTypes.Contains(amp.AMP_PowerType)
                         select amp).Count();
            return count;
        }

        /// <summary>
        /// 查询楼宇实时测点表值(包含楼宇下的房间)
        /// </summary>
        /// <param name="buildingIDs"></param>
        /// <param name="powerTypes"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<EnergyEntity> GetRealEnergyByBuilding(int?[] buildingIDs, string[] powerTypes, int skipItems, int pageSize)
        {
            var list = (from amp in _dataContext.AnalogMeasurePoints
                        from bbi in _dataContext.BuildingBriefInfos
                        where amp.AMP_BuildingID == bbi.BDI_ID
                        && buildingIDs.Contains(amp.AMP_BuildingID)
                        && powerTypes.Contains(amp.AMP_PowerType)
                        select new EnergyEntity
                        {
                            PNO = amp.AMP_AnalogNo,
                            IName = bbi.BDI_Name,
                            STime = amp.AMP_Date,
                            Val = amp.AMP_Val,
                            RemVal = amp.AMP_ValRem,
                            Unit = amp.AMP_Unit,
                            PowerName = amp.AMP_PowerName
                        }
                       ).Skip(skipItems).Take(pageSize);
            return list.ToList();
        }

        /// <summary>
        /// 获取该对象关联测点的所有能耗类型
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="objType"></param>
        /// <returns></returns>
        public IQueryable<PowerType> GetPowerTypesOfObj(String objID, int objType)
        {
            IQueryable<PowerType> powerTypes = null;
            string[] idStrs = objID.Split(new char[] { '_' });
            int?[] ids = new int?[idStrs.Length];
            for (int i = 0; i < idStrs.Length; i++)
            {
                int temp = -1;
                if (Int32.TryParse(idStrs[i], out temp))
                {
                    ids[i] = temp;
                }
            }
            switch (objType)
            {
                case 1:
                    powerTypes = from amp in _dataContext.AnalogMeasurePoints
                                 where ids.Contains(amp.AMP_SchooldID) && amp.AMP_SAreaID == 0 && amp.AMP_BuildingID == 0 && amp.AMP_RoomID == 0
                                 select new PowerType
                                 {
                                     PowerTypeID = amp.AMP_PowerType,
                                     PowerTypeName = amp.AMP_PowerName
                                 };
                    break;
                case 2:
                    powerTypes = from amp in _dataContext.AnalogMeasurePoints
                                 where ids.Contains(amp.AMP_SAreaID) && amp.AMP_BuildingID == 0 && amp.AMP_RoomID == 0
                                 select new PowerType
                                 {
                                     PowerTypeID = amp.AMP_PowerType,
                                     PowerTypeName = amp.AMP_PowerName
                                 };
                    break;
                case 3:
                    powerTypes = from amp in _dataContext.AnalogMeasurePoints
                                 where ids.Contains(amp.AMP_BuildingID) && amp.AMP_RoomID == 0
                                 select new PowerType
                                 {
                                     PowerTypeID = amp.AMP_PowerType,
                                     PowerTypeName = amp.AMP_PowerName
                                 };
                    break;
                case 4:
                    powerTypes = from amp in _dataContext.AnalogMeasurePoints
                                 where ids.Contains(amp.AMP_RoomID)
                                 select new PowerType
                                 {
                                     PowerTypeID = amp.AMP_PowerType,
                                     PowerTypeName = amp.AMP_PowerName
                                 };
                    break;
                case 5:
                    powerTypes = from amp in _dataContext.AnalogMeasurePoints
                                 where ids.Contains(amp.AMP_AnalogNo)
                                 select new PowerType
                                 {
                                     PowerTypeID = amp.AMP_PowerType,
                                     PowerTypeName = amp.AMP_PowerName
                                 };
                    break;
            };
            return powerTypes.Distinct();
        }

        /// <summary>
        /// 获取光华楼关联测点的所有能耗类型
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="objType"></param>
        /// <returns></returns>
        public IQueryable<PowerType> GetBuildingGuanghuaPowerTypesOfObj(String objID, int objType)
        {
            IQueryable<PowerType> powerTypes = null;
            string[] idStrs = objID.Split(new char[] { '_' });
            int?[] ids = new int?[idStrs.Length];
            for (int i = 0; i < idStrs.Length; i++)
            {
                int temp = -1;
                if (Int32.TryParse(idStrs[i], out temp))
                {
                    ids[i] = temp;
                }
            }
            switch (objType)
            {             
                case 1:
                    powerTypes = from amp in _dataContext.AnalogMeasurePoints
                                 where ids.Contains(amp.AMP_BuildingID) && amp.AMP_RoomID == 0
                                 select new PowerType
                                 {
                                     PowerTypeID = amp.AMP_PowerType,
                                     PowerTypeName = amp.AMP_PowerName
                                 };
                    break;
                case 2:
                    powerTypes = from bgh in _dataContext.BuildingGuanghua
                                 join  amp in _dataContext.AnalogMeasurePoints on bgh.BG_No equals amp.AMP_AnalogNo
                                 where ids.Contains(bgh.ED_ID) 
                                 select new PowerType
                                 {
                                     PowerTypeID = amp.AMP_PowerType,
                                     PowerTypeName = amp.AMP_PowerName
                                 };
                    break;
                case 3:
                    powerTypes = from bgh in _dataContext.BuildingGuanghua
                                 join amp in _dataContext.AnalogMeasurePoints on bgh.BG_No equals amp.AMP_AnalogNo
                                 where ids.Contains(bgh.TS_ID)
                                 select new PowerType
                                 {
                                     PowerTypeID = amp.AMP_PowerType,
                                     PowerTypeName = amp.AMP_PowerName
                                 };
                    break;
                case 4:
                    powerTypes = from amp in _dataContext.AnalogMeasurePoints
                                 where ids.Contains(amp.AMP_AnalogNo)
                                 select new PowerType
                                 {
                                     PowerTypeID = amp.AMP_PowerType,
                                     PowerTypeName = amp.AMP_PowerName
                                 };
                    break;
            };
            return powerTypes.Distinct();
        }

        /// <summary>
        /// 查询楼宇测点表值（仅楼宇）
        /// </summary>
        /// <param name="buildingIDs"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        public IList<EnergyEntity> GetRealEnergyByBuildingOnly(int?[] buildingIDs, string[] powerTypes)
        {
            var list = from amp in _dataContext.AnalogMeasurePoints
                       from bbi in _dataContext.BuildingBriefInfos
                       where amp.AMP_BuildingID == bbi.BDI_ID && buildingIDs.Contains(amp.AMP_BuildingID)
                       && powerTypes.Contains(amp.AMP_PowerType) && amp.AMP_RoomID == 0
                       select new EnergyEntity
                       {
                           PNO = amp.AMP_AnalogNo,
                           IName = bbi.BDI_Name,
                           STime = amp.AMP_Date,
                           Val = amp.AMP_Val,
                           RemVal = amp.AMP_ValRem,
                           Unit = amp.AMP_Unit,
                           PowerName = amp.AMP_PowerName,
                           PowerType = amp.AMP_PowerType
                       };
            return list.ToList();
        }

        /// <summary>
        /// 查询房间实时测点表值个数
        /// </summary>
        /// <param name="roomIDs"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        public int GetRealEnergyByRoomCount(int?[] roomIDs, string[] powerTypes)
        {
            var count = (from amp in _dataContext.AnalogMeasurePoints
                         where roomIDs.Contains(amp.AMP_RoomID)
                         && powerTypes.Contains(amp.AMP_PowerType)
                         select amp).Count();
            return count;
        }

        /// <summary>
        /// 查询房间实时测点表值
        /// </summary>
        /// <param name="roomIDs"></param>
        /// <param name="powerTypes"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<EnergyEntity> GetRealEnergyByRoom(int?[] roomIDs, string[] powerTypes, int skipItems, int pageSize)
        {
            var list = (from amp in _dataContext.AnalogMeasurePoints
                        from ri in _dataContext.RoomInfos
                        where amp.AMP_RoomID == ri.RI_ID
                        && roomIDs.Contains(amp.AMP_RoomID)
                        && powerTypes.Contains(amp.AMP_PowerType)
                        select new EnergyEntity
                        {
                            PNO = amp.AMP_AnalogNo,
                            IName = ri.RI_RoomCode,
                            STime = amp.AMP_Date,
                            Val = amp.AMP_Val,
                            RemVal = amp.AMP_ValRem,
                            Unit = amp.AMP_Unit,
                            PowerName = amp.AMP_PowerName
                        }
                       ).Skip(skipItems).Take(pageSize);
            return list.ToList();
        }

        /// <summary>
        /// 查询所有个数
        /// </summary>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        public int GetRealEnergyCount(string[] powerTypes)
        {
            var count = (from amp in _dataContext.AnalogMeasurePoints
                         where powerTypes.Contains(amp.AMP_PowerType)
                         select amp).Count();
            return count;
        }

        /// <summary>
        /// 查询所有实时测点表值
        /// </summary>
        /// <param name="powerTypes"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList GetRealEnergy(string[] powerTypes, int skipItems, int pageSize)
        {
            var list = (from amp in _dataContext.AnalogMeasurePoints
                        join si in _dataContext.SchoolInfos on amp.AMP_SchooldID equals si.SI_ID into tempsi
                        from si2 in tempsi.DefaultIfEmpty()
                        join sai in _dataContext.SchoolAreaInfos on amp.AMP_SAreaID equals sai.SAI_ID into tempsai
                        from sai2 in tempsai.DefaultIfEmpty()
                        join bbi in _dataContext.BuildingBriefInfos on amp.AMP_BuildingID equals bbi.BDI_ID into tempbbi
                        from bbi2 in tempbbi.DefaultIfEmpty()
                        join ri in _dataContext.RoomInfos on amp.AMP_RoomID equals ri.RI_ID into tempri
                        from ri2 in tempri.DefaultIfEmpty()
                        where powerTypes.Contains(amp.AMP_PowerType) && amp.AMP_Statistic == 1
                        select new EnergyEntity
                        {
                            PNO = amp.AMP_AnalogNo,
                            SName = si2.SI_Name,
                            AName = sai2.SAI_Name,
                            BName = bbi2.BDI_Name,
                            RName = ri2.RI_RoomCode,
                            STime = amp.AMP_Date,
                            Val = amp.AMP_Val,
                            RemVal = amp.AMP_ValRem,
                            Unit = amp.AMP_Unit,
                            PowerName = amp.AMP_PowerName
                        }
                       ).Skip(skipItems).Take(pageSize);
            return list.ToList();
        }

        /// <summary>
        /// 查询实时测点表值
        /// </summary>
        /// <returns></returns>
        public IQueryable<EnergyEntity> GetRealEnergy()
        {
            var list = from amp in _dataContext.AnalogMeasurePoints
                       join si in _dataContext.SchoolInfos on amp.AMP_SchooldID equals si.SI_ID into tempsi
                       from si2 in tempsi.DefaultIfEmpty()
                       join sai in _dataContext.SchoolAreaInfos on amp.AMP_SAreaID equals sai.SAI_ID into tempsai
                       from sai2 in tempsai.DefaultIfEmpty()
                       join bbi in _dataContext.BuildingBriefInfos on amp.AMP_BuildingID equals bbi.BDI_ID into tempbbi
                       from bbi2 in tempbbi.DefaultIfEmpty()
                       join ri in _dataContext.RoomInfos on amp.AMP_RoomID equals ri.RI_ID into tempri
                       from ri2 in tempri.DefaultIfEmpty()
                       where amp.AMP_Statistic == 1
                       select new EnergyEntity
                       {
                           PNO = amp.AMP_AnalogNo,
                           SchoolID = amp.AMP_SchooldID,
                           AreaID = amp.AMP_SAreaID,
                           BuildingID = amp.AMP_BuildingID,
                           RoomID = amp.AMP_RoomID,
                           SName = si2.SI_Name,
                           AName = sai2.SAI_Name,
                           BName = bbi2.BDI_Name,
                           RName = ri2.RI_RoomCode,
                           STime = amp.AMP_Date,
                           Val = amp.AMP_Val,
                           RemVal = amp.AMP_ValRem,
                           Unit = amp.AMP_Unit,
                           PowerType = amp.AMP_PowerType,
                           PowerName = amp.AMP_PowerName
                       };
            return list;
        }

        /// <summary>
        /// 获取所有测点
        /// </summary>
        /// <returns></returns>
        public IQueryable<AMPExtEntity> GetAllAMP()
        {
            var list = from amp in _dataContext.AnalogMeasurePoints
                       join si in _dataContext.SchoolInfos on amp.AMP_SchooldID equals si.SI_ID into tempsi
                       from si2 in tempsi.DefaultIfEmpty()
                       join sai in _dataContext.SchoolAreaInfos on amp.AMP_SAreaID equals sai.SAI_ID into tempsai
                       from sai2 in tempsai.DefaultIfEmpty()
                       join bbi in _dataContext.BuildingBriefInfos on amp.AMP_BuildingID equals bbi.BDI_ID into tempbbi
                       from bbi2 in tempbbi.DefaultIfEmpty()
                       join ri in _dataContext.RoomInfos on amp.AMP_RoomID equals ri.RI_ID into tempri
                       from ri2 in tempri.DefaultIfEmpty()
                       join aiinfo in _dataContext.AnalogInfos on amp.AMP_AnalogNo equals aiinfo.AI_No into tempaiinfo
                       from aiinfo2 in tempaiinfo.DefaultIfEmpty()
                       select new AMPExtEntity
                       {
                           PNO = amp.AMP_AnalogNo,
                           SchoolID = amp.AMP_SchooldID,
                           AreaID = amp.AMP_SAreaID,
                           BuildingID = amp.AMP_BuildingID,
                           RoomID = amp.AMP_RoomID,
                           PName = amp.AMP_Name,
                           SName = si2.SI_Name,
                           AName = sai2.SAI_Name,
                           BName = bbi2.BDI_Name,
                           RName = ri2.RI_RoomCode,
                           STime = amp.AMP_Date,
                           Val = amp.AMP_Val,
                           RemVal = amp.AMP_ValRem,
                           Unit = amp.AMP_Unit,
                           PowerType = amp.AMP_PowerType,
                           PowerName = amp.AMP_PowerName,
                           RealFlag = amp.AMP_CptFlag,
                           StatFlag = amp.AMP_Statistic,
                           ParentNo = amp.AMP_ParentNo,
                           RTU_NO = aiinfo2.RTU_No,
                           AI_Serial = aiinfo2.AI_Serial,
                           AI_Base = aiinfo2.AI_Base,
                           AI_Rate = aiinfo2.AI_Rate,
                           Encoding = amp.AMP_Encoding
                       };
            return list;
        }

        /// <summary>
        /// 获取最大测点编号
        /// </summary>
        /// <returns></returns>
        public int GetAMPMaxNo()
        {
            return _dataContext.AnalogMeasurePoints.Select(x => x.AMP_AnalogNo).Max();
        }

        /// <summary>
        /// 增加测点
        /// </summary>
        /// <param name="amp"></param>
        /// <returns></returns>
        public bool AddAMP(AnalogMeasurePoint amp)
        {
            try
            {
                //if (!amp.AMP_ParentNo.HasValue || amp.AMP_ParentNo.Value <= 0)
                //{
                //    int parentNo = 0;
                //    AnalogMeasurePoint tempAMP = null;
                //    if (amp.AMP_RoomID > 0)
                //    {
                //        tempAMP = _dataContext.AnalogMeasurePoints.FirstOrDefault(x => x.AMP_CptFlag == 0 && x.AMP_Statistic == 1 && x.AMP_PowerType == amp.AMP_PowerType && x.AMP_BuildingID == amp.AMP_BuildingID && x.AMP_RoomID == 0);
                //    }
                //    else if (amp.AMP_BuildingID > 0)
                //    {
                //        tempAMP = _dataContext.AnalogMeasurePoints.FirstOrDefault(x => x.AMP_CptFlag == 0 && x.AMP_Statistic == 1 && x.AMP_PowerType == amp.AMP_PowerType && x.AMP_SAreaID == amp.AMP_SAreaID && x.AMP_BuildingID == 0);
                //    }
                //    else if (amp.AMP_SAreaID > 0)
                //    {
                //        tempAMP = _dataContext.AnalogMeasurePoints.FirstOrDefault(x => x.AMP_CptFlag == 0 && x.AMP_Statistic == 1 && x.AMP_PowerType == amp.AMP_PowerType && x.AMP_SchooldID == amp.AMP_SchooldID && x.AMP_SAreaID == 0);
                //    }
                //    if (tempAMP != null)
                //    {
                //        parentNo = tempAMP.AMP_AnalogNo;
                //    }
                //    amp.AMP_ParentNo = parentNo;
                //}
                _dataContext.AnalogMeasurePoints.InsertOnSubmit(amp);
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 查询指定测点是否被使用
        /// </summary>
        /// <param name="pno"></param>
        /// <returns></returns>
        public bool IsUsedByObj(int pno)
        {
            var amp = _dataContext.AnalogMeasurePoints.Where(x => x.AMP_AnalogNo == pno).FirstOrDefault();
            if (amp != null)
            {
                if (amp.AMP_SchooldID > 0 || amp.AMP_SAreaID > 0 || amp.AMP_BuildingID > 0 || amp.AMP_RoomID > 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 删除测点
        /// </summary>
        /// <param name="pno"></param>
        /// <returns></returns>
        public bool DeleteAMP(int pno)
        {
            try
            {
                var amp = _dataContext.AnalogMeasurePoints.SingleOrDefault(x => x.AMP_AnalogNo == pno);
                if (amp != null)
                {
                    if (_dataContext.AnalogMeasurePoints.Where(x => x.AMP_ParentNo == amp.AMP_AnalogNo).Count() > 0)
                    {
                        // 更新父测点为该测点的测点的父测点编号为0
                        _dataContext.ExecuteCommand("update AnalogMeasurePoint set AMP_ParentNo=0 where AMP_ParentNo={0}", amp.AMP_AnalogNo);
                    }
                    _dataContext.AnalogMeasurePoints.DeleteOnSubmit(amp);
                    // 删除该测点的历史值
                    _dataContext.ExecuteCommand("delete from AnalogHistory where AH_AnalogNo={0}", amp.AMP_AnalogNo);
                    _dataContext.SubmitChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        AnalogMeasurePoint CloneAmp(AnalogMeasurePoint amp)
        {
            AnalogMeasurePoint newAmp = new AnalogMeasurePoint();
            newAmp.AMP_AnalogNo = amp.AMP_AnalogNo;
            newAmp.AMP_BuildingID = amp.AMP_BuildingID;
            newAmp.AMP_CptFlag = amp.AMP_CptFlag;
            newAmp.AMP_Date = amp.AMP_Date;
            newAmp.AMP_DepartID = amp.AMP_DepartID;
            newAmp.AMP_Encoding = amp.AMP_Encoding;
            newAmp.AMP_Name = amp.AMP_Name;
            newAmp.AMP_OperationParameter = amp.AMP_OperationParameter;
            newAmp.AMP_OperationRule = amp.AMP_OperationRule;
            newAmp.AMP_ParentNo = amp.AMP_ParentNo;
            newAmp.AMP_PowerName = amp.AMP_PowerName;
            newAmp.AMP_PowerType = amp.AMP_PowerType;
            newAmp.AMP_RoomID = amp.AMP_RoomID;
            newAmp.AMP_SAreaID = amp.AMP_SAreaID;
            newAmp.AMP_SchooldID = amp.AMP_SchooldID;
            newAmp.AMP_State = amp.AMP_State;
            newAmp.AMP_Statistic = amp.AMP_Statistic;
            newAmp.AMP_Timespan = amp.AMP_Timespan;
            newAmp.AMP_Unit = amp.AMP_Unit;
            newAmp.AMP_Val = amp.AMP_Val;
            newAmp.AMP_ValRem = amp.AMP_ValRem;
            return newAmp;
        }

        /// <summary>
        /// 修改AMP
        /// </summary>
        /// <param name="amp"></param>
        /// <returns></returns>
        public bool ModifyAMP(AnalogMeasurePoint amp)
        {
            try
            {
                AnalogMeasurePoint oldAMP = _dataContext.AnalogMeasurePoints.Single(x => x.AMP_AnalogNo == amp.AMP_AnalogNo);
                AnalogMeasurePoint tombstoneAmp = CloneAmp(oldAMP), baseAmp = CloneAmp(oldAMP);
                oldAMP.AMP_Name = amp.AMP_Name;
                oldAMP.AMP_CptFlag = amp.AMP_CptFlag;
                oldAMP.AMP_Statistic = amp.AMP_Statistic;
                oldAMP.AMP_SchooldID = amp.AMP_SchooldID;
                oldAMP.AMP_SAreaID = amp.AMP_SAreaID;
                oldAMP.AMP_BuildingID = amp.AMP_BuildingID;
                oldAMP.AMP_RoomID = amp.AMP_RoomID;
                oldAMP.AMP_PowerType = amp.AMP_PowerType;
                oldAMP.AMP_PowerName = amp.AMP_PowerName;
                ////如果父测点改变，则新增两个虚拟点，以下if语句内容为开发维护工具时新增（曾彬）
                if (oldAMP.AMP_ParentNo != amp.AMP_ParentNo)
                {
                    int newID = this.GetAMPMaxNo() + 1;
                    tombstoneAmp.AMP_CptFlag = 0;
                    tombstoneAmp.AMP_AnalogNo = newID++;
                    tombstoneAmp.AMP_Name = amp.AMP_AnalogNo.ToString() + "墓碑节点";
                    _dataContext.AnalogMeasurePoints.InsertOnSubmit(tombstoneAmp);
                    if (amp.AMP_ParentNo != 0)
                    {
                        baseAmp.AMP_CptFlag = 0;
                        baseAmp.AMP_AnalogNo = newID;
                        baseAmp.AMP_ParentNo = amp.AMP_ParentNo;
                        baseAmp.AMP_Val = -1 * oldAMP.AMP_Val;
                        baseAmp.AMP_Name = amp.AMP_AnalogNo.ToString() + "基数节点";
                        _dataContext.AnalogMeasurePoints.InsertOnSubmit(baseAmp);
                    }
                }
                ////////////////////////////////////
                oldAMP.AMP_ParentNo = amp.AMP_ParentNo;
                oldAMP.AMP_Encoding = amp.AMP_Encoding;
                //if (amp.AMP_ParentNo.HasValue && amp.AMP_ParentNo.Value > 0)
                //{
                //    oldAMP.AMP_ParentNo = amp.AMP_ParentNo;
                //}
                //else
                //{
                //    int parentNo = 0;
                //    AnalogMeasurePoint tempAMP = null;
                //    // 找他的父节点，并修改父节点
                //    if (amp.AMP_RoomID > 0)
                //    {
                //        tempAMP = _dataContext.AnalogMeasurePoints.FirstOrDefault(x => x.AMP_CptFlag == 0 && x.AMP_Statistic == 1 && x.AMP_PowerType == amp.AMP_PowerType && x.AMP_BuildingID == amp.AMP_BuildingID && x.AMP_RoomID == 0);
                //    }
                //    else if (amp.AMP_BuildingID > 0)
                //    {
                //        tempAMP = _dataContext.AnalogMeasurePoints.FirstOrDefault(x => x.AMP_CptFlag == 0 && x.AMP_Statistic == 1 && x.AMP_PowerType == amp.AMP_PowerType && x.AMP_SAreaID == amp.AMP_SAreaID && x.AMP_BuildingID == 0);
                //    }
                //    else if (amp.AMP_SAreaID > 0)
                //    {
                //        tempAMP = _dataContext.AnalogMeasurePoints.FirstOrDefault(x => x.AMP_CptFlag == 0 && x.AMP_Statistic == 1 && x.AMP_PowerType == amp.AMP_PowerType && x.AMP_SchooldID == amp.AMP_SchooldID && x.AMP_SAreaID == 0);
                //    }
                //    if (tempAMP != null)
                //    {
                //        parentNo = tempAMP.AMP_AnalogNo;
                //    }
                //    oldAMP.AMP_ParentNo = parentNo;
                //}
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 查询测点信息
        /// </summary>
        /// <param name="analogNo"></param>
        /// <returns></returns>
        public AnalogMeasurePoint QueryAMPInfo(int? analogNo)
        {
            return _dataContext.AnalogMeasurePoints.SingleOrDefault(x => x.AMP_AnalogNo == analogNo);
        }        
        
        /// <summary>
        /// 查询三级电表信息（该月份已录入数据的电表）
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="ampName"></param>
        /// <returns></returns>
        public IQueryable<ThirdPointMonthValEntity> QueryThirdAMPHasValue(int? analogNo, String ampName, DateTime month)
        {
            IQueryable<AMPExtEntity> list = null;
            IQueryable<ThirdPointMonthValEntity> finalList = null;
            DateTime currentTime = month.AddMonths(1).AddDays(-1);
            DateTime lastMonthTime = month.AddDays(-1);
            if (analogNo.HasValue && (ampName == null || ampName == ""))
            {
                list = from amp in _dataContext.AnalogMeasurePoints
                       where amp.AMP_AnalogNo == analogNo && amp.AMP_PowerType == "001007"
                       select new AMPExtEntity
                       {
                           PNO = amp.AMP_AnalogNo,
                           PName = amp.AMP_Name
                       };
            }
            else if (!analogNo.HasValue && ampName != null && ampName != "")
            {
                list = from amp in _dataContext.AnalogMeasurePoints
                       where amp.AMP_Name.Contains(ampName) && amp.AMP_PowerType == "001007"
                       select new AMPExtEntity
                       {
                           PNO = amp.AMP_AnalogNo,
                           PName = amp.AMP_Name
                       };
                var testList = list.ToList();
            }
            else if (analogNo.HasValue && ampName != null && ampName != "")
            {
                list = from amp in _dataContext.AnalogMeasurePoints
                       where amp.AMP_Name.Contains(ampName) && amp.AMP_PowerType == "001007" && amp.AMP_AnalogNo == analogNo
                       select new AMPExtEntity
                       {
                           PNO = amp.AMP_AnalogNo,
                           PName = amp.AMP_Name
                       };
            }
            else
            {
                list = from amp in _dataContext.AnalogMeasurePoints
                       where amp.AMP_PowerType == "001007"
                       select new AMPExtEntity
                       {
                           PNO = amp.AMP_AnalogNo,
                           PName = amp.AMP_Name
                       };
            }
            //找出所有三级电表测点当月的表值数据
            IQueryable<ThirdPointMonthValEntity> currentValList = from li in list
                                                                  join ah in _dataContext.AnalogHistories on li.PNO equals ah.AH_AnalogNo
                                                                  where ah.AH_Time == currentTime
                                                                  select new ThirdPointMonthValEntity
                                                                  {
                                                                      PNO = li.PNO,
                                                                      PName = li.PName,
                                                                      val = ah.AH_Value,
                                                                      month = month
                                                                  };
            //找出所有三级电表当月之前时间内最大的表值
            IQueryable<ThirdPointMonthValEntity> lastMonthValList = from li in list
                                                                    join ah in _dataContext.AnalogHistories on li.PNO equals ah.AH_AnalogNo
                                                                    where ah.AH_Time < currentTime
                                                                    group ah.AH_Value by new { ah.AH_AnalogNo, li.PName } into g
                                                                    select new ThirdPointMonthValEntity
                                                                    {
                                                                        PNO = g.Key.AH_AnalogNo,
                                                                        PName = g.Key.PName,
                                                                        val = g.Max(),
                                                                        month = month
                                                                    };
            //当月表值减去之前最大表值，得到当月实际用电量
            finalList = from currentList in currentValList
                        from lastMonthList in lastMonthValList
                        where currentList.PNO == lastMonthList.PNO
                        select new ThirdPointMonthValEntity
                        {
                            PNO = currentList.PNO,
                            PName = currentList.PName,
                            val = currentList.val - lastMonthList.val,
                            month = month
                        };
            return finalList;
        }

        /// <summary>
        /// 查询该月尚未输入用电量的三级测点
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="ampName"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public IList<AMPExtEntity> QueryThirdPointNeedValue(int? analogNo, String ampName, DateTime month)
        {
            DateTime currentTime = month.AddMonths(1).AddDays(-1);
            IQueryable<AMPExtEntity> hasValueList = from amp in _dataContext.AnalogMeasurePoints
                                                    join ah in _dataContext.AnalogHistories on amp.AMP_AnalogNo equals ah.AH_AnalogNo
                                                    where amp.AMP_PowerType == "001007" && ah.AH_Time == currentTime
                                                    select new AMPExtEntity
                                                    {
                                                        PNO = amp.AMP_AnalogNo,
                                                        PName = amp.AMP_Name
                                                    };
            IQueryable<AMPExtEntity> list = null;
            if (analogNo.HasValue && (ampName == null || ampName == ""))
            {
                list = from amp in _dataContext.AnalogMeasurePoints
                       where amp.AMP_AnalogNo == analogNo && amp.AMP_PowerType == "001007"
                       select new AMPExtEntity
                       {
                           PNO = amp.AMP_AnalogNo,
                           PName = amp.AMP_Name
                       };
            }
            else if (!analogNo.HasValue && ampName != null && ampName != "")
            {
                list = from amp in _dataContext.AnalogMeasurePoints
                       where amp.AMP_Name.Contains(ampName) && amp.AMP_PowerType == "001007"
                       select new AMPExtEntity
                       {
                           PNO = amp.AMP_AnalogNo,
                           PName = amp.AMP_Name
                       };
            }
            else if (analogNo.HasValue && ampName != null && ampName != "")
            {
                list = from amp in _dataContext.AnalogMeasurePoints
                       where amp.AMP_Name.Contains(ampName) && amp.AMP_PowerType == "001007" && amp.AMP_AnalogNo == analogNo
                       select new AMPExtEntity
                       {
                           PNO = amp.AMP_AnalogNo,
                           PName = amp.AMP_Name
                       };
            }
            else
            {
                list = from amp in _dataContext.AnalogMeasurePoints
                       where amp.AMP_PowerType == "001007"
                       select new AMPExtEntity
                       {
                           PNO = amp.AMP_AnalogNo,
                           PName = amp.AMP_Name
                       };
            }
            var actualList = list.ToList();
            var actualHasValueList = hasValueList.ToList();
            var finalList = actualList.Except(actualHasValueList, AMPExtEntity.CompareByID).ToList();
            return finalList;
        }


        /// <summary>
        /// 获取某个区域某个能耗类型的虚拟测点编号
        /// </summary>
        /// <param name="areaID"></param>
        /// <param name="powerType"></param>
        /// <returns></returns>
        public int GetAMPNoByArea(int areaID, string powerType)
        {
            try
            {
                var amp = _dataContext.AnalogMeasurePoints.Single(x => x.AMP_SAreaID == areaID && x.AMP_BuildingID == 0 && x.AMP_RoomID == 0 && x.AMP_PowerType == powerType);
                if (amp != null && amp.AMP_AnalogNo != 0)
                {
                    return amp.AMP_AnalogNo;
                }
            }
            catch (Exception e)
            {
            }
            return 0;
        }

        public bool UpdateValueOfParentPoint(int analogNo)
        {
            if (analogNo == 0) return false;
            var point = _dataContext.AnalogMeasurePoints.Single(x => x.AMP_AnalogNo == analogNo);
            int childPointsCount = _dataContext.AnalogMeasurePoints.Where(x => x.AMP_ParentNo == analogNo).Count();
            if (point.AMP_CptFlag == 0 && childPointsCount > 0)
            {
                IQueryable<AnalogHistory> oldValue = _dataContext.AnalogHistories.Where(x => x.AH_AnalogNo == analogNo);
                _dataContext.AnalogHistories.DeleteAllOnSubmit(oldValue);
                _dataContext.SubmitChanges();
                var allHistoryValue = from ah in _dataContext.AnalogHistories
                                      from amp in _dataContext.AnalogMeasurePoints
                                      where amp.AMP_AnalogNo == ah.AH_AnalogNo && amp.AMP_ParentNo == analogNo
                                      select ah;
                var mintimes = (from allHV in allHistoryValue
                                group allHV by allHV.AH_AnalogNo into g
                                select new
                                {
                                    AH_AnalogNo = g.Key,
                                    AH_Time = g.Min(x => x.AH_Time),
                                    AH_Value = g.Min(x => x.AH_Value)
                                }).ToList();
                DateTime timeCritical = mintimes.Max(x => x.AH_Time);
                DateTime oldMaxTime = timeCritical;
                timeCritical = new DateTime(timeCritical.Year, timeCritical.Month, timeCritical.Day, timeCritical.Hour, 59, 59);
                while (timeCritical < DateTime.Now)
                {
                    var maxValue = from allHV in allHistoryValue
                                   where allHV.AH_Time < timeCritical
                                   group allHV by allHV.AH_AnalogNo into g
                                   select new
                                   {
                                       AH_AnalogNo = g.Key,
                                       AH_Time = g.Max(a => a.AH_Time),
                                       AH_Value = g.Max(a => a.AH_Value)
                                   };
                    var maxValueList = maxValue.ToList();
                    AnalogHistory newAHItem = new AnalogHistory
                    {
                        AH_AnalogNo = analogNo,
                        AH_Time = maxValue.Max(x => x.AH_Time),
                        AH_Value = maxValue.Sum(x => x.AH_Value)
                    };
                    if (newAHItem.AH_Time > oldMaxTime)
                    {
                        _dataContext.AnalogHistories.InsertOnSubmit(newAHItem);
                    }
                    oldMaxTime = newAHItem.AH_Time;
                    timeCritical = timeCritical.AddHours(1);
                }
                _dataContext.SubmitChanges();
                //var maxValueEachHour = from ah in _dataContext.AnalogHistories
                //                        from amp in _dataContext.AnalogMeasurePoints
                //                        where amp.AMP_AnalogNo == ah.AH_AnalogNo && amp.AMP_ParentNo == analogNo && amp.AMP_Statistic == 1
                //                        group ah by new { amp.AMP_AnalogNo, ah.AH_Time.Year, ah.AH_Time.Month, ah.AH_Time.Day, ah.AH_Time.Hour } into g
                //                        select new
                //                        {
                //                            AH_AnalogNo = g.Key.AMP_AnalogNo,
                //                            AH_Time = g.Max(ah => ah.AH_Time),
                //                            AH_Value = g.Max(ah => ah.AH_Value)
                //                        };
                ////var maxValueEachHourList = maxValueEachHour.ToList();
                //var parentPointHistory = from mv in maxValueEachHour
                //                        group mv by new { mv.AH_Time.Year, mv.AH_Time.Month, mv.AH_Time.Day, mv.AH_Time.Hour } into g
                //                        select new 
                //                        {
                //                            AH_AnalogNo = analogNo,
                //                            AH_Time = g.Max(mv => mv.AH_Time),
                //                            AH_Value = g.Sum(mv => mv.AH_Value)
                //                        };
                //List<AnalogHistory> parentPointHistoryList = parentPointHistory.ToList();
                //_dataContext.AnalogHistories.InsertAllOnSubmit(parentPointHistory);
                //_dataContext.SubmitChanges();
                return true;
            }
            return false;
        }

        public AnalogMeasurePoint GetAMP(int analogNo)
        {
            return _dataContext.AnalogMeasurePoints.Single(x => x.AMP_AnalogNo == analogNo);
        }

        /// <summary>
        /// 清除某个房间对应测点的所属对象信息
        /// </summary>
        /// <param name="roomID"></param>
        public void ClearObjIDByRoom(int roomID)
        {
            try
            {
                AnalogMeasurePoint item = _dataContext.AnalogMeasurePoints.Single(x => x.AMP_RoomID == roomID);
                item.AMP_SchooldID = 0;
                item.AMP_SAreaID = 0;
                item.AMP_BuildingID = 0;
                item.AMP_RoomID = 0;
                _dataContext.SubmitChanges();
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// 根据建筑ID得到其真实测点
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public List<AnalogMeasurePoint> GetAMPbyBuildingID(int buildingID)
        {
            try
            {
                List<AnalogMeasurePoint> list = _dataContext.AnalogMeasurePoints.Where(x => x.AMP_BuildingID == buildingID && x.AMP_CptFlag == 1).ToList();
                List<AnalogMeasurePoint> resultList = new List<AnalogMeasurePoint>();
                foreach (var item in list)
                {
                    if (powerTypeCheck(item.AMP_PowerType) == true)
                    {
                        resultList.Add(item);
                    }
                }
                return resultList;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据建筑ID得到其真实测点
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public List<AnalogMeasurePoint> GetAMPbySchoolID(int schoolID)
        {
            try
            {
                List<AnalogMeasurePoint> list = _dataContext.AnalogMeasurePoints.Where(x => x.AMP_SchooldID == schoolID && x.AMP_SAreaID == 0 && x.AMP_BuildingID == 0 && x.AMP_CptFlag == 1).ToList();
                foreach (var item in list)
                {
                    if (powerTypeCheck(item.AMP_PowerType) == false)
                    {
                        list.Remove(item);
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private bool powerTypeCheck(string powerType)
        {
            string prefix = powerType.Substring(0, 3);
            if (prefix == "001" || prefix == "002" || prefix == "003")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }

}
