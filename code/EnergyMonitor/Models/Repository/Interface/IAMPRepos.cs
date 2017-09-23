using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;
using EnergyMonitor.Models.Repository.Entity;

namespace EnergyMonitor.Models.Repository.Interface
{
    /// <summary>
    /// AnalogMeasurePoint实体操作接口
    /// </summary>
    public interface IAMPRepos
    {
        /// <summary>
        /// 得到房间测点实时数据 
        /// </summary>
        /// <param name="roomID">房间号</param>
        /// <returns></returns>
        IList<AnalogMeasurePoint> GetRealTimeEnergy(int roomID);

        /// <summary>
        /// 查询校区实时测点表值个数
        /// </summary>
        /// <param name="schoolIDs">校区ID数组</param>
        /// <param name="powerTypes">能耗类型</param>
        /// <returns></returns>
        int GetRealEnergyBySchoolCount(int?[] schoolIDs, string[] powerTypes);

        /// <summary>
        /// 查询校区实时测点表值
        /// </summary>
        /// <param name="schoolIDs">校区ID数组</param>
        /// <param name="powerTypes">能耗类型</param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IList<EnergyEntity> GetRealEnergyBySchool(int?[] schoolIDs, string[] powerTypes, int skipItems, int pageSize);

        /// <summary>
        /// 查询区域实时测点表值个数
        /// </summary>
        /// <param name="areaIDs">区域ID数组</param>
        /// <param name="powerTypes">能耗类型</param>
        /// <returns></returns>
        int GetRealEnergyByAreaCount(int?[] areaIDs, string[] powerTypes);

        /// <summary>
        /// 查询区域实时测点表值
        /// </summary>
        /// <param name="areaIDs">区域ID数组</param>
        /// <param name="powerTypes"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IList<EnergyEntity> GetRealEnergyByArea(int?[] areaIDs, string[] powerTypes, int skipItems, int pageSize);

        /// <summary>
        /// 获取光华楼关联测点的所有能耗类型
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="objType"></param>
        /// <returns></returns>
        IQueryable<PowerType> GetBuildingGuanghuaPowerTypesOfObj(String objID, int objType);

        /// <summary>
        /// 查询楼宇实时测点表值个数
        /// </summary>
        /// <param name="buildingIDs"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        int GetRealEnergyByBuildingCount(int?[] buildingIDs, string[] powerTypes);

        /// <summary>
        /// 查询楼宇实时测点表值（包含楼宇下各个房间）
        /// </summary>
        /// <param name="buildingIDs"></param>
        /// <param name="powerTypes"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IList<EnergyEntity> GetRealEnergyByBuilding(int?[] buildingIDs, string[] powerTypes, int skipItems, int pageSize);

        /// <summary>
        /// 查询楼宇实时测点表值（仅楼宇）
        /// </summary>
        /// <param name="buildingIDs"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        IList<EnergyEntity> GetRealEnergyByBuildingOnly(int?[] buildingIDs, string[] powerTypes);

        /// <summary>
        /// 查询房间实时测点表值个数
        /// </summary>
        /// <param name="roomIDs"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        int GetRealEnergyByRoomCount(int?[] roomIDs, string[] powerTypes);

        /// <summary>
        /// 查询房间实时测点表值
        /// </summary>
        /// <param name="roomIDs"></param>
        /// <param name="powerTypes"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IList<EnergyEntity> GetRealEnergyByRoom(int?[] roomIDs, string[] powerTypes, int skipItems, int pageSize);

        /// <summary>
        /// 查询所有个数
        /// </summary>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        int GetRealEnergyCount(string[] powerTypes);

        /// <summary>
        /// 查询所有实时测点表值
        /// </summary>
        /// <param name="powerTypes"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IList GetRealEnergy(string[] powerTypes, int skipItems, int pageSize);

        /// <summary>
        /// 查询实时测点表值
        /// </summary>
        /// <returns></returns>
        IQueryable<EnergyEntity> GetRealEnergy();

        /// <summary>
        /// 获取所有测点
        /// </summary>
        /// <returns></returns>
        IQueryable<AMPExtEntity> GetAllAMP();

        /// <summary>
        /// 增加测点
        /// </summary>
        /// <param name="amp"></param>
        /// <returns></returns>
        bool AddAMP(AnalogMeasurePoint amp);

        /// <summary>
        /// 获取最大测点编号
        /// </summary>
        /// <returns></returns>
        int GetAMPMaxNo();


        /// <summary>
        /// 查询指定测点是否被使用
        /// </summary>
        /// <param name="pno"></param>
        /// <returns></returns>
        bool IsUsedByObj(int pno);

        /// <summary>
        /// 删除测点
        /// </summary>
        /// <param name="pno"></param>
        /// <returns></returns>
        bool DeleteAMP(int pno);

        /// <summary>
        /// 修改AMP
        /// </summary>
        /// <param name="amp"></param>
        /// <returns></returns>
        bool ModifyAMP(AnalogMeasurePoint amp);

        /// <summary>
        /// 查询测点信息
        /// </summary>
        /// <param name="analogNo">测点编号</param>
        /// <returns></returns>
        AnalogMeasurePoint QueryAMPInfo(int? analogNo);

        /// <summary>
        /// 查询三级电表信息（该月份已录入数据的电表）
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="ampName"></param>
        /// <returns></returns>
        IQueryable<ThirdPointMonthValEntity> QueryThirdAMPHasValue(int? analogNo, String ampName, DateTime month);

        /// <summary>
        /// 查询三级电表信息（该月份未录入数据的电表）
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="ampName"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        IList<AMPExtEntity> QueryThirdPointNeedValue(int? analogNo, String ampName, DateTime month);


        /// <summary>
        /// 获取该对象关联测点的所有能耗类型
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="objType"></param>
        /// <returns></returns>
        IQueryable<PowerType> GetPowerTypesOfObj(String objID, int objType);

        /// <summary>
        /// 获取某个区域某个能耗类型的虚拟测点编号
        /// </summary>
        /// <param name="areaID"></param>
        /// <param name="powerType"></param>
        /// <returns></returns>
        int GetAMPNoByArea(int areaID, string powerType);

        /// <summary>
        /// 更新父测点的历史值，将子测点的历史值相加。
        /// </summary>
        /// <param name="analogNo"></param>
        /// <returns></returns>
        bool UpdateValueOfParentPoint(int analogNo);

        /// <summary>
        /// 根据测点编号获得测点信息
        /// </summary>
        /// <param name="analogNo"></param>
        /// <returns></returns>
        AnalogMeasurePoint GetAMP(int analogNo);

        /// <summary>
        /// 清除某个房间对应测点的所属对象信息
        /// </summary>
        /// <param name="roomID"></param>
        void ClearObjIDByRoom(int roomID);

        /// <summary>
        /// 根据建筑ID得到其真实测点
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        List<AnalogMeasurePoint> GetAMPbyBuildingID(int buildingID);

        /// <summary>
        /// 根据建筑ID得到其真实测点
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        List<AnalogMeasurePoint> GetAMPbySchoolID(int schoolID);
    }
}
