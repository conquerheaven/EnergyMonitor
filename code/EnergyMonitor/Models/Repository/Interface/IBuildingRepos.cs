using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;
using EnergyMonitor.Models.Repository.Entity;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IBuildingRepos
    {      
        /// <summary>
        /// 查询楼宇 
        /// </summary>
        /// <param name="buildingName"></param>
        /// <param name="areaID">非正数查询所有区域</param>
        /// <returns></returns>
        IList GetBuilding(string buildingName, int areaID);

        /// <summary>
        /// 查询楼宇
        /// </summary>
        /// <param name="buildingName"></param>
        /// <param name="areaID">非正数查询所有区域</param>
        /// <returns></returns>
        IList GetBuilding(string buildingName, int areaID, int skipItems, int pageSize);

        /// <summary>
        /// 查询区域下所有楼宇(要求BDI_Flag==1)
        /// </summary>
        /// <param name="areaID"></param>
        /// <returns></returns>
        IQueryable<BuildingBriefInfo> GetBuildingsOfArea(int areaID);

                /// <summary>
        /// 查询区域下所有楼宇
        /// </summary>
        /// <param name="areaID"></param>
        /// <returns></returns>
        IQueryable<BuildingBriefInfo> GetBuildingsOfArea2(int areaID);

        /// <summary>
        /// 获取楼宇个数
        /// </summary>
        /// <param name="buildingName"></param>
        /// <param name="areaID">非正数查询所有区域</param>
        /// <returns></returns>
        int GetBuildingCount(string buildingName, int areaID);

        /// <summary>
        /// 查询楼宇详细
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        BuildingDetailInfo GetBuilding(int buildingID);

        /// <summary>
        /// 获取楼宇和区域
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        BuildingAndArea GetBuildingAndArea(int buildingID);

        /// <summary>
        /// 删除楼宇及相关设备信息
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        bool DeleteBuilding(int buildingID);

        /// <summary>
        /// 修改楼宇
        /// </summary>
        /// <param name="buildingID"></param>
        /// <param name="areaID"></param>
        /// <param name="buildingName"></param>
        /// <param name="buildingCode"></param>
        /// <returns></returns>
        bool ModifyBuildingPart(int buildingID, int areaID, string buildingName, string buildingCode);

        /// <summary>
        /// 增加楼宇
        /// </summary>
        /// <param name="buildingName"></param>
        /// <param name="areaID"></param>
        /// <param name="buildingCode"></param>
        /// <returns></returns>
        bool AddBuildingPart(string buildingName, int areaID, string buildingCode);

        /// <summary>
        /// 获取楼宇拥有的房间数
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        int GetRoomCountByBuilding(int buildingID);

        /// <summary>
        /// 查询楼宇名是否存在
        /// </summary>
        /// <param name="buildingName"></param>
        /// <param name="areaID"></param>
        /// <returns></returns>
        bool IsBuildingNameExist(string buildingName, int? areaID);

        /// <summary>
        /// 增加楼宇
        /// </summary>
        /// <param name="b"></param>
        /// <param name="areaID"></param>
        /// <returns>成功返回新的楼宇ID，否则返回0</returns>
        int AddBuilding(BuildingDetailInfo b, int areaID);

        /// <summary>
        /// 增加建筑室内照明
        /// </summary>
        /// <param name="lightList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        bool AddLight(List<LightDetail> lightList, int buildingID);

        /// <summary>
        /// 增加建筑电梯设备
        /// </summary>
        /// <param name="elevatorList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        bool AddElevator(List<ElevatorDetail> elevatorList, int buildingID);

        /// <summary>
        /// 增加建筑水泵用能设备
        /// </summary>
        /// <param name="waterPumpList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        bool AddWaterPump(List<WaterPumpDetail> waterPumpList, int buildingID);

        /// <summary>
        /// 增加建筑风机用能设备
        /// </summary>
        /// <param name="windMachList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        bool AddWindMach(List<WindMachDetail> windMachList, int buildingID);

        /// <summary>
        /// 增加建筑厨房设备
        /// </summary>
        /// <param name="kitchenEquipList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        bool AddKitchenEquip(List<KitchenEquipDetail> kitchenEquipList, int buildingID);

        /// <summary>
        /// 增加建筑室内办公设备
        /// </summary>
        /// <param name="officeEquipList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        bool AddOfficeEquip(List<OfficeEquipDetail> officeEquipList, int buildingID);

        /// <summary>
        /// 获取建筑室内照明
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        IList GetLight(int buildingID);

        /// <summary>
        /// 获取建筑电梯设备
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        IList GetElevator(int buildingID);

        /// <summary>
        /// 获取建筑水泵用能设备
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        IList GetWaterPump(int buildingID);

        /// <summary>
        /// 获取建筑风机用能设备
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        IList GetWindMach(int buildingID);

        /// <summary>
        /// 获取建筑厨房设备
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        IList GetKitchenEquip(int buildingID);

        /// <summary>
        /// 获取建筑室内办公设备
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        IList GetOfficeEquip(int buildingID);

        /// <summary>
        /// 修改建筑
        /// </summary>
        /// <param name="b"></param>
        /// <param name="areaID"></param>
        /// <returns></returns>
        bool ModifyBuilding(BuildingDetailInfo b, int areaID);

        /// <summary>
        /// 修改建筑室内照明
        /// </summary>
        /// <param name="lightList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        bool ModifyLight(List<LightDetail> lightList, int buildingID);

        /// <summary>
        /// 修改建筑电梯设备
        /// </summary>
        /// <param name="elevatorList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        bool ModifyElevator(List<ElevatorDetail> elevatorList, int buildingID);

        /// <summary>
        /// 修改建筑水泵用能设备
        /// </summary>
        /// <param name="waterPumpList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        bool ModifyWaterPump(List<WaterPumpDetail> waterPumpList, int buildingID);

        /// <summary>
        /// 修改建筑风机用能设备
        /// </summary>
        /// <param name="windMachList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        bool ModifyWindMach(List<WindMachDetail> windMachList, int buildingID);

        /// <summary>
        /// 修改建筑厨房设备
        /// </summary>
        /// <param name="kitchenEquipList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        bool ModifyKitchenEquip(List<KitchenEquipDetail> kitchenEquipList, int buildingID);

        /// <summary>
        /// 修改建筑室内办公设备
        /// </summary>
        /// <param name="officeEquipList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        bool ModifyOfficeEquip(List<OfficeEquipDetail> officeEquipList, int buildingID);

        /// <summary>
        /// 增加建筑附属信息
        /// </summary>
        /// <param name="buildingAppendixList"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        bool AddBuildingAppendix(List<BuildingAppendix> buildingAppendixList, int buildingID);

        /// <summary>
        /// 查询建筑附属表信息
        /// </summary>
        /// <param name="buildingID"></param>
        /// <param name="appendixNo"></param>
        /// <returns></returns>
        IQueryable<BuildingAppendix> GetBuildingAppendix(int buildingID, string appendixNo);

        /// <summary>
        /// 修改建筑附属信息
        /// </summary>
        /// <param name="buildingAppendixList"></param>
        /// <param name="buildingID"></param>
        /// <param name="appendixNo"></param>
        /// <returns></returns>
        bool ModifyBuildingAppendix(List<BuildingAppendix> buildingAppendixList, int buildingID, string appendixNo);

        /// <summary>
        /// 查询光华楼信息
        /// </summary>       
        /// <returns></returns>
        IList<BuildingDetailInfo> GetGuanghuaBuilding();

        /// <summary>
        /// 查询存在公式的建筑
        /// </summary>
        /// <returns></returns>
        IList<BuildingBriefInfo> GetBuildingHasOperateRule();

        /// <summary>
        /// 修改建筑公式
        /// </summary>
        /// <param name="buildingID">建筑ID</param>
        /// <param name="operateRule">建筑公式</param>
        /// <returns>是否修改成功</returns>
        bool ModifyBuildingOperateRule(int buildingID, string operateRule);

        /// <summary>
        /// 修改建筑公式
        /// </summary>
        /// <param name="buildingID">建筑ID</param>
        /// <param name="operateRule">建筑公式</param>
        /// <returns>是否修改成功</returns>
        bool ModifyBuildingHJFlag(int buildingID, byte hJFlag);

        /// <summary>
        /// 查询与测点AnalogNo相关的建筑（公式中包含AnalogNo的建筑）
        /// </summary>
        /// <param name="AnalogNo"></param>
        /// <returns></returns>
        IList<BuildingBriefInfo> GetBuildingRelateToAnalogNo(int AnalogNo);

        /// <summary>
        /// 根据建筑ID获取建筑
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        BuildingBriefInfo GetBuildingByBuildingID(int buildingID);

        /// <summary>
        /// 根据建筑名称，模糊查询建筑
        /// </summary>
        /// <param name="buildingName">建筑名称</param>
        /// <returns></returns>
        IList<BuildingBriefInfo> GetBuildingByNameFuzzily(string buildingName);
    }
}
