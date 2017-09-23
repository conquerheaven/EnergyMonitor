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
    public class BuildingRepos : IBuildingRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public BuildingRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region IBuildingRepos Members

        /// <summary>
        /// 查询楼宇
        /// </summary>
        /// <param name="buildingName"></param>
        /// <param name="areaID"></param>
        /// <returns></returns>
        public IList GetBuilding(string buildingName, int areaID)
        {
            if (areaID > 0)
            {
                var list = from bb in _dataContext.BuildingBriefInfos
                           from sa in _dataContext.SchoolAreaInfos
                           where bb.SAI_ID == sa.SAI_ID && bb.SAI_ID == areaID && bb.BDI_Name.Contains(buildingName)
                           select new BuildingAndArea
                           {
                               CampusID = sa.SI_ID - 1,
                               BuildingID = bb.BDI_ID,
                               BuildingName = bb.BDI_Name,
                               AreaID = bb.SAI_ID,
                               AreaName = sa.SAI_Name
                           };
                return list.ToList();
            }
            else
            {
                var list = from bb in _dataContext.BuildingBriefInfos
                           from sa in _dataContext.SchoolAreaInfos
                           where bb.SAI_ID == sa.SAI_ID && bb.BDI_Name.Contains(buildingName)
                           select new BuildingAndArea
                           {
                               CampusID = sa.SI_ID - 1,
                               BuildingID = bb.BDI_ID,
                               BuildingName = bb.BDI_Name,
                               AreaID = bb.SAI_ID,
                               AreaName = sa.SAI_Name
                           };
                return list.ToList();
            }
        }

        /// <summary>
        /// 查询楼宇
        /// </summary>
        /// <param name="buildingName"></param>
        /// <param name="areaID">非正数查询所有区域</param>
        /// <returns></returns>
        public IList GetBuilding(string buildingName, int areaID, int skipItems, int pageSize)
        {
            if (areaID > 0)
            {
                var list = (from bb in _dataContext.BuildingBriefInfos
                           from sa in _dataContext.SchoolAreaInfos
                            where bb.SAI_ID == sa.SAI_ID && bb.SAI_ID == areaID && bb.BDI_Name.Contains(buildingName)
                           select new BuildingAndArea
                           {
                               BuildingID = bb.BDI_ID,
                               BuildingName = bb.BDI_Name,
                               AreaID = bb.SAI_ID,
                               AreaName = sa.SAI_Name
                           }).Skip(skipItems).Take(pageSize);
                return list.ToList();
            }
            else
            {
                var list = (from bb in _dataContext.BuildingBriefInfos
                           from sa in _dataContext.SchoolAreaInfos
                            where bb.SAI_ID == sa.SAI_ID && bb.BDI_Name.Contains(buildingName)
                           select new BuildingAndArea
                           {
                               BuildingID = bb.BDI_ID,
                               BuildingName = bb.BDI_Name,
                               AreaID = bb.SAI_ID,
                               AreaName = sa.SAI_Name
                           }).Skip(skipItems).Take(pageSize);
                return list.ToList();
            }
        }

        /// <summary>
        /// 查询区域下所有楼宇(要求BDI_Flag==1)
        /// </summary>
        /// <param name="areaID"></param>
        /// <returns></returns>
        public IQueryable<BuildingBriefInfo> GetBuildingsOfArea(int areaID) 
        {
            if (areaID > 0) 
            {
                var query = _dataContext.BuildingBriefInfos.Where(x => x.SAI_ID == areaID && x.BDI_Flag == 1).Select(x => x);
                return query;
            }
            return null;
        }

        /// <summary>
        /// 查询光华楼信息
        /// </summary>   
        /// <returns></returns>
        public IList<BuildingDetailInfo> GetGuanghuaBuilding()
        {
              var list = from bg in _dataContext.BuildingDetailInfos
                            where bg.BDI_ID == 218
                            select bg;
            return list.ToList<BuildingDetailInfo>();
        }
  
        /// <summary>
        /// 查询区域下所有楼宇
        /// </summary>
        /// <param name="areaID"></param>
        /// <returns></returns>
        public IQueryable<BuildingBriefInfo> GetBuildingsOfArea2(int areaID)
        {
            if (areaID > 0)
            {
                var query = _dataContext.BuildingBriefInfos.Where(x => x.SAI_ID == areaID).Select(x => x);
                return query;
            }
            return null;
        }

        /// <summary>
        /// 获取楼宇个数
        /// </summary>
        /// <param name="buildingName"></param>
        /// <param name="areaID">非正数查询所有区域</param>
        /// <returns></returns>
        public int GetBuildingCount(string buildingName, int areaID)
        {
            if (areaID > 0)
            {
                var count = (from bb in _dataContext.BuildingBriefInfos
                             from sa in _dataContext.SchoolAreaInfos
                             where bb.SAI_ID == sa.SAI_ID && bb.SAI_ID == areaID && bb.BDI_Name.Contains(buildingName)
                             select new BuildingAndArea
                             {
                                 BuildingID = bb.BDI_ID,
                                 BuildingName = bb.BDI_Name,
                                 AreaID = bb.SAI_ID,
                                 AreaName = sa.SAI_Name
                             }).Count();
                return count;
            }
            else
            {
                var count = (from bb in _dataContext.BuildingBriefInfos
                            from sa in _dataContext.SchoolAreaInfos
                             where bb.SAI_ID == sa.SAI_ID && bb.BDI_Name.Contains(buildingName)
                            select new BuildingAndArea
                            {
                                BuildingID = bb.BDI_ID,
                                BuildingName = bb.BDI_Name,
                                AreaID = bb.SAI_ID,
                                AreaName = sa.SAI_Name
                            }).Count();
                return count;
            }
        }

        /// <summary>
        /// 查询楼宇详细
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public BuildingDetailInfo GetBuilding(int buildingID)
        {
            return _dataContext.BuildingDetailInfos.SingleOrDefault(x => x.BDI_ID == buildingID);
        }

        /// <summary>
        /// 获取楼宇和区域
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public BuildingAndArea GetBuildingAndArea(int buildingID)
        {
            var list = from bb in _dataContext.BuildingBriefInfos
                       from sa in _dataContext.SchoolAreaInfos
                       from bd in _dataContext.BuildingDetailInfos
                       where bb.BDI_ID == bd.BDI_ID && bb.SAI_ID == sa.SAI_ID && bb.BDI_ID == buildingID
                       select new BuildingAndArea
                       {
                           BuildingID = bb.BDI_ID,
                           BuildingName = bb.BDI_Name,
                           AreaID = bb.SAI_ID,
                           AreaName = sa.SAI_Name,
                           BuildingCode = bd.BDI_Code
                       };
            if (list.Count() != 0)
            {
                return list.Single();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 删除楼宇及相关设备信息
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public bool DeleteBuilding(int buildingID)
        {
            try
            {
                _dataContext.BuildingDetailInfos.DeleteOnSubmit(_dataContext.BuildingDetailInfos.SingleOrDefault(x => x.BDI_ID == buildingID));
                _dataContext.BuildingBriefInfos.DeleteOnSubmit(_dataContext.BuildingBriefInfos.SingleOrDefault(x => x.BDI_ID == buildingID));

                _dataContext.LightDetails.DeleteAllOnSubmit(_dataContext.LightDetails.Where(x => x.BDI_ID == buildingID));
                _dataContext.ElevatorDetails.DeleteAllOnSubmit(_dataContext.ElevatorDetails.Where(x => x.BDI_ID == buildingID));
                _dataContext.WaterPumpDetails.DeleteAllOnSubmit(_dataContext.WaterPumpDetails.Where(x => x.BDI_ID == buildingID));
                _dataContext.WindMachDetails.DeleteAllOnSubmit(_dataContext.WindMachDetails.Where(x => x.BDI_ID == buildingID));
                _dataContext.KitchenEquipDetails.DeleteAllOnSubmit(_dataContext.KitchenEquipDetails.Where(x => x.BDI_ID == buildingID));
                _dataContext.OfficeEquipDetails.DeleteAllOnSubmit(_dataContext.OfficeEquipDetails.Where(x => x.BDI_ID == buildingID));

                _dataContext.SubmitChanges();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改楼宇
        /// </summary>
        /// <param name="buildingID"></param>
        /// <param name="areaID"></param>
        /// <param name="buildingName"></param>
        /// <param name="buildingCode"></param>
        /// <returns></returns>
        public bool ModifyBuildingPart(int buildingID, int areaID, string buildingName, string buildingCode)
        {
            try
            {
                BuildingBriefInfo bb = _dataContext.BuildingBriefInfos.Single(x => x.BDI_ID == buildingID);
                if (bb.SAI_ID != areaID)
                {
                    // 修改建筑信息本身的区域ID
                    bb.SAI_ID = areaID;
                    // 修改AMP表中对应的建筑区域
                    if (_dataContext.AnalogMeasurePoints.Where(x => x.AMP_BuildingID == bb.BDI_ID).Count() > 0)
                    {
                        _dataContext.ExecuteCommand("update AnalogMeasurePoint set AMP_SAreaID={0} where AMP_BuildingID={1}", areaID, bb.BDI_ID);
                        // 修改父测点编号
                        var modifiedAMPs = _dataContext.AnalogMeasurePoints.Where(x => x.AMP_BuildingID == bb.BDI_ID && x.AMP_RoomID == 0 && x.AMP_ParentNo > 1).ToList();
                        foreach (var item in modifiedAMPs)
                        {
                            var parentPoint = _dataContext.AnalogMeasurePoints.Where(x => x.AMP_CptFlag == 0 && x.AMP_Statistic == 1 && x.AMP_SAreaID == areaID && x.AMP_BuildingID == 0 && x.AMP_PowerType == item.AMP_PowerType).SingleOrDefault();
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
                bb.BDI_Name = buildingName;

                BuildingDetailInfo bd = _dataContext.BuildingDetailInfos.Single(x => x.BDI_ID == buildingID);
                bd.BDI_Name = buildingName;
                bd.BDI_Code = buildingCode;

                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 增加楼宇
        /// </summary>
        /// <param name="buildingName"></param>
        /// <param name="areaID"></param>
        /// <param name="buildingCode"></param>
        /// <returns></returns>
        public bool AddBuildingPart(string buildingName, int areaID, string buildingCode)
        {
            try
            {
                BuildingDetailInfo bd = new BuildingDetailInfo();
                bd.BDI_Name = buildingName;
                bd.BDI_Code = buildingCode;
                _dataContext.BuildingDetailInfos.InsertOnSubmit(bd);
                _dataContext.SubmitChanges();

                BuildingBriefInfo bb = new BuildingBriefInfo();
                bb.BDI_ID = bd.BDI_ID;
                bb.BDI_Name = buildingName;
                bb.SAI_ID = areaID;
                _dataContext.BuildingBriefInfos.InsertOnSubmit(bb);

                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取楼宇拥有的房间数
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public int GetRoomCountByBuilding(int buildingID)
        {
            return _dataContext.RoomInfos.Where(x => x.BDI_ID == buildingID).Count();
        }

        /// <summary>
        /// 查询楼宇名是否存在
        /// </summary>
        /// <param name="buildingName"></param>
        /// <param name="areaID"></param>
        /// <returns></returns>
        public bool IsBuildingNameExist(string buildingName, int? areaID)
        {
            var query = _dataContext.BuildingBriefInfos.Where(x => x.BDI_Name == buildingName);
            if (areaID != null && areaID.Value > 0)
            {
                query = query.Where(x => x.SAI_ID == areaID.Value);
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
        /// 增加楼宇
        /// </summary>
        /// <param name="b"></param>
        /// <param name="areaID"></param>
        /// <returns>成功返回新的楼宇ID，否则返回0</returns>
        public int AddBuilding(BuildingDetailInfo b, int areaID)
        {
            try
            {
                _dataContext.BuildingDetailInfos.InsertOnSubmit(b);
                _dataContext.SubmitChanges();

                BuildingBriefInfo bb = new BuildingBriefInfo();
                bb.BDI_ID = b.BDI_ID;
                bb.BDI_Name = b.BDI_Name;
                bb.SAI_ID = areaID;
                bb.BDI_Flag = 1;
                _dataContext.BuildingBriefInfos.InsertOnSubmit(bb);
                _dataContext.SubmitChanges();
                return b.BDI_ID;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 增加建筑室内照明
        /// </summary>
        /// <param name="lightList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public bool AddLight(List<LightDetail> lightList, int buildingID)
        {
            try
            {
                foreach (var light in lightList)
                {
                    //if (light.LD_Area == null && light.LD_Type == null && light.LD_Num == null && light.LD_Power == null && light.LD_Dgree == null && light.LD_Mode == null)
                    //{
                    //    lightList.Remove(light);
                    //}
                    //else
                    //{
                    //    light.BDI_ID = buildingID;
                    //}
                    light.BDI_ID = buildingID;
                }
                if (lightList.Count > 0)
                {
                    _dataContext.LightDetails.InsertAllOnSubmit(lightList);
                    _dataContext.SubmitChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 增加建筑电梯设备
        /// </summary>
        /// <param name="elevatorList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public bool AddElevator(List<ElevatorDetail> elevatorList, int buildingID)
        {
            try
            {
                foreach (var elevator in elevatorList)
                {
                    if (elevator.ED_SerialNum == null && elevator.ED_Type == null && elevator.ED_FactoryModel == null && elevator.ED_Power == null)
                    {
                        elevatorList.Remove(elevator);
                    }
                    else
                    {
                        elevator.BDI_ID = buildingID;
                    }
                }
                if (elevatorList.Count > 0)
                {
                    _dataContext.ElevatorDetails.InsertAllOnSubmit(elevatorList);
                    _dataContext.SubmitChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 增加建筑水泵用能设备
        /// </summary>
        /// <param name="waterPumpList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public bool AddWaterPump(List<WaterPumpDetail> waterPumpList, int buildingID)
        {
            try
            {
                foreach (var waterPump in waterPumpList)
                {
                    //if (waterPump.WPD_Num == null && waterPump.WPD_Flow == null && waterPump.WPD_Lifting == null && waterPump.WPD_Power == null && waterPump.WPD_YearRuntime == null)
                    //{
                    //    waterPumpList.Remove(waterPump);
                    //}
                    //else
                    //{
                    //    waterPump.BDI_ID = buildingID;
                    //}
                    waterPump.BDI_ID = buildingID;
                }
                if (waterPumpList.Count > 0)
                {
                    _dataContext.WaterPumpDetails.InsertAllOnSubmit(waterPumpList);
                    _dataContext.SubmitChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 增加建筑风机用能设备
        /// </summary>
        /// <param name="windMachList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public bool AddWindMach(List<WindMachDetail> windMachList, int buildingID)
        {
            try
            {
                foreach (var windMach in windMachList)
                {
                    //if (windMach.WMD_Num == null && windMach.WMD_windAmount == null && windMach.WMD_Intensity == null && windMach.WMD_Power == null && windMach.WMD_YearRuntime == null)
                    //{
                    //    windMachList.Remove(windMach);
                    //}
                    //else
                    //{
                    //    windMach.BDI_ID = buildingID;
                    //}
                    windMach.BDI_ID = buildingID;
                }
                if (windMachList.Count > 0)
                {
                    _dataContext.WindMachDetails.InsertAllOnSubmit(windMachList);
                    _dataContext.SubmitChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 增加建筑厨房设备
        /// </summary>
        /// <param name="kitchenEquipList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public bool AddKitchenEquip(List<KitchenEquipDetail> kitchenEquipList, int buildingID)
        {
            try
            {
                foreach (var kitchenEquip in kitchenEquipList)
                {
                    //if (kitchenEquip.KED_Num == null && kitchenEquip.KED_FuelType == null && kitchenEquip.KED_Consumption == null && kitchenEquip.KED_Power == null && kitchenEquip.KED_YearRuntime == null)
                    //{
                    //    kitchenEquipList.Remove(kitchenEquip);
                    //}
                    //else
                    //{
                    //    kitchenEquip.BDI_ID = buildingID;
                    //}
                    kitchenEquip.BDI_ID = buildingID;
                }
                if (kitchenEquipList.Count > 0)
                {
                    _dataContext.KitchenEquipDetails.InsertAllOnSubmit(kitchenEquipList);
                    _dataContext.SubmitChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 增加建筑室内办公设备
        /// </summary>
        /// <param name="officeEquipList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public bool AddOfficeEquip(List<OfficeEquipDetail> officeEquipList, int buildingID)
        {
            try
            {
                foreach (var officeEquip in officeEquipList)
                {
                    //if (officeEquip.OED_Num == null && officeEquip.OED_FixedPower == null && officeEquip.OED_YearPower == null)
                    //{
                    //    officeEquipList.Remove(officeEquip);
                    //}
                    //else
                    //{
                    //    officeEquip.BDI_ID = buildingID;
                    //}
                    officeEquip.BDI_ID = buildingID;
                }
                if (officeEquipList.Count > 0)
                {
                    _dataContext.OfficeEquipDetails.InsertAllOnSubmit(officeEquipList);
                    _dataContext.SubmitChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取建筑室内照明
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public IList GetLight(int buildingID)
        {
            return _dataContext.LightDetails.Where(x => x.BDI_ID == buildingID).ToList();

        }

        /// <summary>
        /// 获取建筑电梯设备
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public IList GetElevator(int buildingID)
        {
            return _dataContext.ElevatorDetails.Where(x => x.BDI_ID == buildingID).ToList();
        }

        /// <summary>
        /// 获取建筑水泵用能设备
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public IList GetWaterPump(int buildingID)
        {
            return _dataContext.WaterPumpDetails.Where(x => x.BDI_ID == buildingID).ToList();
        }

        /// <summary>
        /// 获取建筑风机用能设备
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public IList GetWindMach(int buildingID)
        {
            return _dataContext.WindMachDetails.Where(x => x.BDI_ID == buildingID).ToList();
        }

        /// <summary>
        /// 获取建筑厨房设备
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public IList GetKitchenEquip(int buildingID)
        {
            return _dataContext.KitchenEquipDetails.Where(x => x.BDI_ID == buildingID).ToList();
        }

        /// <summary>
        /// 获取建筑室内办公设备
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public IList GetOfficeEquip(int buildingID)
        {
            return _dataContext.OfficeEquipDetails.Where(x => x.BDI_ID == buildingID).ToList();
        }

        /// <summary>
        /// 修改建筑
        /// </summary>
        /// <param name="b"></param>
        /// <param name="areaID"></param>
        /// <returns></returns>
        public bool ModifyBuilding(BuildingDetailInfo b, int areaID)
        {
            try
            {
                BuildingBriefInfo bb = _dataContext.BuildingBriefInfos.SingleOrDefault(x => x.BDI_ID == b.BDI_ID);
                if (bb.SAI_ID != areaID)
                {
                    // 修改建筑信息本身的区域ID
                    bb.SAI_ID = areaID;
                    // 修改AMP表中对应的建筑区域
                    if (_dataContext.AnalogMeasurePoints.Where(x => x.AMP_BuildingID == bb.BDI_ID).Count() > 0)
                    {
                        _dataContext.ExecuteCommand("update AnalogMeasurePoint set AMP_SAreaID={0} where AMP_BuildingID={1}", areaID, bb.BDI_ID);
                        // 修改父测点编号
                        var modifiedAMPs = _dataContext.AnalogMeasurePoints.Where(x => x.AMP_BuildingID == bb.BDI_ID && x.AMP_RoomID == 0 && x.AMP_ParentNo > 1).ToList();
                        foreach (var item in modifiedAMPs)
                        {
                            var parentPoint = _dataContext.AnalogMeasurePoints.Where(x => x.AMP_CptFlag == 0 && x.AMP_Statistic == 1 && x.AMP_SAreaID == areaID && x.AMP_BuildingID == 0 && x.AMP_PowerType == item.AMP_PowerType).SingleOrDefault();
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
                bb.BDI_Name = b.BDI_Name;

                BuildingDetailInfo bd = _dataContext.BuildingDetailInfos.SingleOrDefault(x => x.BDI_ID == b.BDI_ID);
                bd.BDI_Name = b.BDI_Name;
                bd.BDI_Address = b.BDI_Address;
                bd.BDI_Code = b.BDI_Code;
                bd.BDI_Introduction = b.BDI_Introduction;
                bd.BDI_Telephone = b.BDI_Telephone;
                bd.BDI_LinkMan = b.BDI_LinkMan;
                bd.BDI_LinkManPhone = b.BDI_LinkManPhone;
                bd.BDI_StatMan = b.BDI_StatMan;
                bd.BDI_StatManPhone = b.BDI_StatManPhone;
                bd.BDI_Academy = b.BDI_Academy;
                bd.BDI_Date = b.BDI_Date;
                bd.BDI_Direction = b.BDI_Direction;
                bd.BDI_Users = b.BDI_Users;
                bd.BDI_RunPerWeek = b.BDI_RunPerWeek;
                bd.BDI_WeekStart = b.BDI_WeekStart;
                bd.BDI_WeekEnd = b.BDI_WeekEnd;
                bd.BDI_RunPreMonth = b.BDI_RunPreMonth;
                bd.BDI_MonthStart = b.BDI_MonthStart;
                bd.BDI_MonthEnd = b.BDI_MonthEnd;
                bd.BDI_RunPerYear = b.BDI_RunPerYear;
                bd.BDI_YearStart = b.BDI_YearStart;
                bd.BDI_YearEnd = b.BDI_YearEnd;
                bd.BDI_Holiday = b.BDI_Holiday;
                bd.BDI_Height = b.BDI_Height;
                bd.BDI_FloorUp = b.BDI_FloorUp;
                bd.BDI_FloorDn = b.BDI_FloorDn;
                bd.BDI_FloorHeight = b.BDI_FloorHeight;
                bd.BDI_Area = b.BDI_Area;
                bd.BDI_AreaAir = b.BDI_AreaAir;
                bd.BDI_AreaHot = b.BDI_AreaHot;
                bd.BDI_AreaSpe = b.BDI_AreaSpe;
                bd.BDI_AauditFlag = b.BDI_AauditFlag;
                bd.BDI_AauditDate = b.BDI_AauditDate;
                bd.BDI_TransforFlag = b.BDI_TransforFlag;
                bd.BDI_TransforContent = b.BDI_TransforContent;
                bd.BDI_ElectricCompName = b.BDI_ElectricCompName;
                bd.BDI_GasCompName = b.BDI_GasCompName;
                bd.BDI_Type = b.BDI_Type;
                bd.BDI_TypeOther = b.BDI_TypeOther;
                bd.BDI_AreaRatio = b.BDI_AreaRatio;
                bd.BDI_KindsOut = b.BDI_KindsOut;
                bd.BDI_WarmKeep = b.BDI_WarmKeep;
                bd.BDI_Structure = b.BDI_Structure;
                bd.BDI_WinOut = b.BDI_WinOut;
                bd.BDI_Visor = b.BDI_Visor;
                bd.BDI_Glass = b.BDI_Glass;
                bd.BDI_Win = b.BDI_Win;
                bd.BDI_TemCool = b.BDI_TemCool;
                bd.BDI_TemHot = b.BDI_TemHot;
                bd.BDI_TemSepcialCool = b.BDI_TemSepcialCool;
                bd.BDI_TemSepcialHot = b.BDI_TemSepcialHot;
                bd.BDI_AirSys = b.BDI_AirSys;
                bd.BDI_DevCool = b.BDI_DevCool;
                bd.BDI_DevHot = b.BDI_DevHot;
                bd.BDI_LiBr = b.BDI_LiBr;
                bd.BDI_BoilerPower = b.BDI_BoilerPower;
                bd.BDI_AirSplit = b.BDI_AirSplit;
                bd.BDI_Boiler = b.BDI_Boiler;
                bd.BDI_PowerHot = b.BDI_PowerHot;
                bd.BDI_DevOther = b.BDI_DevOther;
                bd.BDI_LightIn = b.BDI_LightIn;
                bd.BDI_LightOut = b.BDI_LightOut;
                bd.BDI_LightOutPower = b.BDI_LightOutPower;
                bd.BDI_LightOutWorkHours = b.BDI_LightOutWorkHours;
                if (!String.IsNullOrWhiteSpace(b.BDI_ImageUrl))
                {
                    bd.BDI_ImageUrl = b.BDI_ImageUrl;
                }
                
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改建筑室内照明
        /// </summary>
        /// <param name="lightList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public bool ModifyLight(List<LightDetail> lightList, int buildingID)
        {
            try
            {
                var modifyList =  _dataContext.LightDetails.Where(x => x.BDI_ID == buildingID).ToList();
                List<LightDetail> addList = new List<LightDetail>();
                foreach (var item in lightList)
                {
                    var modify = modifyList.SingleOrDefault(x => x.LD_ID == item.LD_ID);
                    if (modify != null)
                    {
                        modify.LD_Region = item.LD_Region;
                        modify.LD_Area = item.LD_Area;
                        modify.LD_Type = item.LD_Type;
                        modify.LD_Num = item.LD_Num;
                        modify.LD_Power = item.LD_Power;
                        modify.LD_Dgree = item.LD_Dgree;
                        modify.LD_Start = item.LD_Start;
                        modify.LD_End = item.LD_End;
                        modify.LD_Mode = item.LD_Mode;
                    }
                    else
                    {
                        item.BDI_ID = buildingID;
                        addList.Add(item);
                    }
                }
                if (addList.Count > 0)
                {
                    _dataContext.LightDetails.InsertAllOnSubmit(addList);
                }
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改建筑电梯设备
        /// </summary>
        /// <param name="elevatorList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public bool ModifyElevator(List<ElevatorDetail> elevatorList, int buildingID)
        {
            try
            {
                var modifyList = _dataContext.ElevatorDetails.Where(x => x.BDI_ID == buildingID).ToList();
                List<ElevatorDetail> addList = new List<ElevatorDetail>();
                foreach (var item in elevatorList)
                {
                    var modify = modifyList.SingleOrDefault(x => x.ED_ID == item.ED_ID);
                    if (modify != null)
                    {
                        modify.ED_SerialNum = item.ED_SerialNum;
                        modify.ED_Type = item.ED_Type;
                        modify.ED_FactoryModel = item.ED_FactoryModel;
                        modify.ED_Power = item.ED_Power;
                        modify.ED_Start = item.ED_Start;
                        modify.ED_End = item.ED_End;
                        modify.ED_Mode = item.ED_Mode;
                        modifyList.Remove(modify);
                    }
                    else
                    {
                        item.BDI_ID = buildingID;
                        addList.Add(item);
                    }
                }
                if (addList.Count > 0)
                {
                    _dataContext.ElevatorDetails.InsertAllOnSubmit(addList);
                }
                if (modifyList.Count > 0)
                {
                    _dataContext.ElevatorDetails.DeleteAllOnSubmit(modifyList);
                }
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改建筑水泵用能设备
        /// </summary>
        /// <param name="waterPumpList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public bool ModifyWaterPump(List<WaterPumpDetail> waterPumpList, int buildingID)
        {
            try
            {
                var modifyList = _dataContext.WaterPumpDetails.Where(x => x.BDI_ID == buildingID).ToList();
                List<WaterPumpDetail> addList = new List<WaterPumpDetail>();
                foreach (var item in waterPumpList)
                {
                    var modify = modifyList.SingleOrDefault(x => x.WPD_ID == item.WPD_ID);
                    if (modify != null)
                    {
                        modify.WPD_Type = item.WPD_Type;
                        modify.WPD_Num = item.WPD_Num;
                        modify.WPD_Flow = item.WPD_Flow;
                        modify.WPD_Lifting = item.WPD_Lifting;
                        modify.WPD_Power = item.WPD_Power;
                        modify.WPD_YearRuntime = item.WPD_YearRuntime;
                    }
                    else
                    {
                        item.BDI_ID = buildingID;
                        addList.Add(item);
                    }
                }
                if (addList.Count > 0)
                {
                    _dataContext.WaterPumpDetails.InsertAllOnSubmit(addList);
                }
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改建筑风机用能设备
        /// </summary>
        /// <param name="windMachList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public bool ModifyWindMach(List<WindMachDetail> windMachList, int buildingID)
        {
            try
            {
                var modifyList = _dataContext.WindMachDetails.Where(x => x.BDI_ID == buildingID).ToList();
                List<WindMachDetail> addList = new List<WindMachDetail>();
                foreach (var item in windMachList)
                {
                    var modify = modifyList.SingleOrDefault(x => x.WMD_ID == item.WMD_ID);
                    if (modify != null)
                    {
                        modify.WMD_Type = item.WMD_Type;
                        modify.WMD_Num = item.WMD_Num;
                        modify.WMD_windAmount = item.WMD_windAmount;
                        modify.WMD_Intensity = item.WMD_Intensity;
                        modify.WMD_Power = item.WMD_Power;
                        modify.WMD_YearRuntime = item.WMD_YearRuntime;
                    }
                    else
                    {
                        item.BDI_ID = buildingID;
                        addList.Add(item);
                    }
                }
                if (addList.Count > 0)
                {
                    _dataContext.WindMachDetails.InsertAllOnSubmit(addList);
                }
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改建筑厨房设备
        /// </summary>
        /// <param name="kitchenEquipList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public bool ModifyKitchenEquip(List<KitchenEquipDetail> kitchenEquipList, int buildingID)
        {
            try
            {
                var modifyList = _dataContext.KitchenEquipDetails.Where(x => x.BDI_ID == buildingID).ToList();
                List<KitchenEquipDetail> addList = new List<KitchenEquipDetail>();
                foreach (var item in kitchenEquipList)
                {
                    var modify = modifyList.SingleOrDefault(x => x.KED_ID == item.KED_ID);
                    if (modify != null)
                    {
                        modify.KED_Type = item.KED_Type;
                        modify.KED_Num = item.KED_Num;
                        modify.KED_FuelType = item.KED_FuelType;
                        modify.KED_Consumption = item.KED_Consumption;
                        modify.KED_Power = item.KED_Power;
                        modify.KED_YearRuntime = item.KED_YearRuntime;
                    }
                    else
                    {
                        item.BDI_ID = buildingID;
                        addList.Add(item);
                    }
                }
                if (addList.Count > 0)
                {
                    _dataContext.KitchenEquipDetails.InsertAllOnSubmit(addList);
                }
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改建筑室内办公设备
        /// </summary>
        /// <param name="officeEquipList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public bool ModifyOfficeEquip(List<OfficeEquipDetail> officeEquipList, int buildingID)
        {
            try
            {
                var modifyList = _dataContext.OfficeEquipDetails.Where(x => x.BDI_ID == buildingID).ToList();
                List<OfficeEquipDetail> addList = new List<OfficeEquipDetail>();
                foreach (var item in officeEquipList)
                {
                    var modify = modifyList.SingleOrDefault(x => x.OED_ID == item.OED_ID);
                    if (modify != null)
                    {
                        modify.OED_Type = item.OED_Type;
                        modify.OED_Num = item.OED_Num;
                        modify.OED_FixedPower = item.OED_FixedPower;
                        modify.OED_YearPower = item.OED_YearPower;
                        modify.OED_Start = item.OED_Start;
                        modify.OED_End = item.OED_End;
                    }
                    else
                    {
                        item.BDI_ID = buildingID;
                        addList.Add(item);
                    }
                }
                if (addList.Count > 0)
                {
                    _dataContext.OfficeEquipDetails.InsertAllOnSubmit(addList);
                }
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 增加建筑附属信息
        /// </summary>
        /// <param name="buildingAppendixList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public bool AddBuildingAppendix(List<BuildingAppendix> buildingAppendixList, int buildingID)
        {
            try
            {
                if (buildingAppendixList != null && buildingAppendixList.Count > 0)
                {
                    foreach (var item in buildingAppendixList)
                    {
                        item.BDI_ID = buildingID;
                    }
                    _dataContext.BuildingAppendixes.InsertAllOnSubmit(buildingAppendixList);
                    _dataContext.SubmitChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 查询建筑附属表信息
        /// </summary>
        /// <param name="buildingID"></param>
        /// <param name="appendixNo"></param>
        /// <returns></returns>
        public IQueryable<BuildingAppendix> GetBuildingAppendix(int buildingID, string appendixNo)
        {
            var query = _dataContext.BuildingAppendixes.Where(x => x.BDI_ID == buildingID);
            if (!String.IsNullOrWhiteSpace(appendixNo))
            {
                query = query.Where(x => x.BA_AppendixNo == appendixNo);
            }
            return query;
        }

        /// <summary>
        /// 修改建筑附属信息
        /// </summary>
        /// <param name="buildingAppendixList"></param>
        /// <param name="buildingID"></param>
        /// <param name="appendixNo"></param>
        /// <returns></returns>
        public bool ModifyBuildingAppendix(List<BuildingAppendix> buildingAppendixList, int buildingID, string appendixNo)
        {
            try
            {
                var modifyList = _dataContext.BuildingAppendixes.Where(x => x.BDI_ID == buildingID && x.BA_AppendixNo == appendixNo).ToList();
                List<BuildingAppendix> addList = new List<BuildingAppendix>();
                foreach (var item in buildingAppendixList)
                {
                    var modify = modifyList.SingleOrDefault(x => x.BA_ID == item.BA_ID);
                    if (modify != null)
                    {
                        modify.BA_Name = item.BA_Name;
                        modify.BA_Type = item.BA_Type;
                        modify.BA_Num = item.BA_Num;
                        modify.BA_ManufacturerAndModel = item.BA_ManufacturerAndModel;
                        modify.BA_CoolingCapacity = item.BA_CoolingCapacity;
                        modify.BA_HeatingCapacity = item.BA_HeatingCapacity;
                        modify.BA_Power = item.BA_Power;
                        modify.BA_PowerSource = item.BA_PowerSource;
                        modify.BA_WorkPerDay = item.BA_WorkPerDay;
                        modify.BA_WorkPerYear = item.BA_WorkPerYear;
                        modify.BA_AirVolume = item.BA_AirVolume;
                        modify.BA_WindPressure = item.BA_WindPressure;
                        modify.BA_DayWorkStart = item.BA_DayWorkStart;
                        modify.BA_DayWorkEnd = item.BA_DayWorkEnd;
                        modify.BA_AverageLoadRate = item.BA_AverageLoadRate;
                        modify.BA_CoolingWater = item.BA_CoolingWater;
                        modify.BA_Flow = item.BA_Flow;
                        modify.BA_Lifting = item.BA_Lifting;
                        modify.BA_CoolingPower = item.BA_CoolingPower;
                        modify.BA_HeatingPower = item.BA_HeatingPower;
                        modify.BA_FuelConsumption = item.BA_FuelConsumption;
                        modify.BA_CoolingWaterCapacity = item.BA_CoolingWaterCapacity;
                        modify.BA_BoilerEfficiency = item.BA_BoilerEfficiency;
                        modifyList.Remove(modify);
                    }
                    else
                    {
                        item.BDI_ID = buildingID;
                        addList.Add(item);
                    }
                }
                if (addList.Count > 0)
                {
                    _dataContext.BuildingAppendixes.InsertAllOnSubmit(addList);
                }
                if (modifyList.Count > 0)
                {
                    _dataContext.BuildingAppendixes.DeleteAllOnSubmit(modifyList);
                }
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion


        public IList<BuildingBriefInfo> GetBuildingHasOperateRule()
        {
            IList<BuildingBriefInfo> bBI = new List<BuildingBriefInfo>();

            bBI = _dataContext.BuildingBriefInfos.Where(x => x.BDI_OperateRule != "NULL").ToList();

            return bBI;
        }


        public bool ModifyBuildingOperateRule(int buildingID, string operateRule)
        {
            try
            {
                BuildingBriefInfo bBriefInfo = _dataContext.BuildingBriefInfos.Where(x => x.BDI_ID == buildingID).SingleOrDefault();
                bBriefInfo.BDI_OperateRule = operateRule;
                _dataContext.SubmitChanges();
                return true;
            }catch(Exception){
                return false;
            }
        }


        public bool ModifyBuildingHJFlag(int buildingID, byte hJFlag)
        {
            try
            {
                BuildingBriefInfo bBriefInfo = _dataContext.BuildingBriefInfos.Where(x => x.BDI_ID == buildingID).SingleOrDefault();
                bBriefInfo.BDI_HJFlag = hJFlag;
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public IList<BuildingBriefInfo> GetBuildingRelateToAnalogNo(int AnalogNo)
        {
            IList<BuildingBriefInfo> bBriefInfo = _dataContext.BuildingBriefInfos.Where(x => x.BDI_OperateRule.Contains("_"+AnalogNo)).ToList();
            return bBriefInfo;
        }


        public BuildingBriefInfo GetBuildingByBuildingID(int buildingID)
        {
            BuildingBriefInfo bBriefInfo = _dataContext.BuildingBriefInfos.Where(x => x.BDI_ID == buildingID).SingleOrDefault();
            return bBriefInfo;
        }


        public IList<BuildingBriefInfo> GetBuildingByNameFuzzily(string buildingName)
        {
            IList<BuildingBriefInfo> bBriefInfo = _dataContext.BuildingBriefInfos.Where(x => x.BDI_Name.Contains(buildingName)).ToList();
            return bBriefInfo;
        }
    }
}
