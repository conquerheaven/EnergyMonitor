using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Entity;
using System.Collections;
using System.Data;
using EnergyMonitor.Models.Entity;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IAnalogHistoryRepos
    {

        /// <summary>
        /// 根据传入的时间获取光华楼按能耗类型分类的测点在该时间前的最大历史数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>        
        /// <returns></returns>
        IList<ChartStatisEntity> GetGuangHuaConsupmtion(int id, DateTime endTime, string[] powerTypes);

       /// <summary>
        /// 根据查询对象和能耗类别分组获取能耗使用集合(包含多个建筑在多个对象分析中存在单位面积的情况)
       /// </summary>
       /// <param name="objType"></param>
       /// <param name="ids"></param>
       /// <param name="startTime"></param>
       /// <param name="endTime"></param>
       /// <param name="powerTypes"></param>
       /// <param name="sum"></param>
       /// <param name="statisticMode"></param>
       /// <returns></returns>
        IList<ChartStatisEntity> GetEnergyByIDsPowerAnalyze(int objType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes, double sum, string statisticMode);

         /// <summary>
        /// 根据查询对象和能耗类别分组获取能耗使用集合(包含单个建筑在多个对象分析中存在单位面积的情况)
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetEnergyByIDsPowerForAnalyze(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum, string statisticMode);

        /// <summary>
        /// 根据月粒度得到指定房间的能耗
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetEnergyMonthConsume(int roomID, DateTime startTime, DateTime endTime);

        /// <summary>
        /// 获取该测点规定时间内的能耗历史值
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IQueryable<EnergyEntity> GetEnergyHistory(int analogNo, DateTime startTime, DateTime endTime);

        /// <summary>
        /// 给某个测点增加历史值
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="time"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Boolean AddEnergyHistory(int analogNo, DateTime time, double value);


        /// <summary>
        /// 添加三级电表历史值
        /// </summary>
        /// <param name="valueStr"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        Boolean AddThirdPointHistory(String valueStr, DateTime month);

        /// <summary>
        /// 修改三级测点历史值
        /// </summary>
        /// <param name="valueStr"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        Boolean ModifyThirdPointHistory(String valueStr, DateTime month);

        /// <summary>
        /// 删除三级电表某月用电量数据
        /// </summary>
        /// <param name="pointID"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        Boolean DeleteThirdPointHistory(int pointID, DateTime month);
        /// <summary>
        /// 获取当月该房间所有已用的电量值
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        double GetCurrentMonthEnergy(int roomID);

        /// <summary>
        /// 给指定时间段内的所有历史值增加或减小一定数值
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Boolean ModifyByTimePeriod(int analogNo, DateTime startTime, DateTime endTime, double value);

        /// <summary>
        /// 删除指定时间段内的所有历史值
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        Boolean DeleteByTimePeriod(int analogNo, DateTime startTime, DateTime endTime);

        /// <summary>
        /// 得到指定时间段内测点的历史值个数
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        int AICountByTimePeriod(int analogNo, DateTime startTime, DateTime endTime);
        /// <summary>
        /// 获取房间剩余电量
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        double GetCurrentRemVal(int roomID);

        /// <summary>
        /// 得到按天粒度房间能耗使用量数据个数
        /// </summary>
        /// <param name="roomID">房间号</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        int GetEnergyDayConsumeCount(int? roomID, DateTime startTime, DateTime endTime);

        /// <summary>
        /// 得到按月粒度房间能耗使用量数据个数
        /// </summary>
        /// <param name="roomID">房间号</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        int GetEnergyMonthConsumeCount(int? roomID, DateTime startTime, DateTime endTime);

        /// <summary>
        /// 得到指定房间的一段时间的用电数据
        /// </summary>
        /// <param name="roomID">房间ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        double? GetEnergyConsume(int? roomID, DateTime startTime, DateTime endTime);

        /// <summary>
        /// 得到房间能耗查询的真正起始时间
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        DateTime GetStartTime(int? roomID);

        /// <summary>
        /// 按天粒度统计房间能耗使用量
        /// </summary>
        /// <param name="roomID">房间号</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="skipItems">跳过行数</param>
        /// <param name="pageSize">每页行数</param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetEnergyDayConsume(int? roomID, DateTime startTime, DateTime endTime, int skipItems, int pageSize);

        /// <summary>
        /// 按月粒度统计房间能耗使用量
        /// </summary>
        /// <param name="roomID">房间号</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="skipItems">跳过行数</param>
        /// <param name="pageSize">每页行数</param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetEnergyMonthConsume(int? roomID, DateTime startTime, DateTime endTime, int skipItems, int pageSize);

        /// <summary>
        /// 获取能耗按天统计数据个数
        /// </summary>
        /// <param name="queryType">
        /// 1：校区
        /// 2：区域
        /// 3：楼宇
        /// 4：房间
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        int GetEnergyStatisDayCount(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes);

        /// <summary>
        /// 获取能耗按天统计数据个数
        /// </summary>
        /// <param name="queryType">
        /// 1：校区
        /// 2：区域
        /// 3：楼宇
        /// 4：房间
        /// </param>
        /// <param name="queryType"></param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="dw">星期几</param>
        /// <returns></returns>
        int GetEnergyStatisDayCount(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, DayOfWeek? dw);

        /// <summary>
        /// 获取能耗按天统计数据
        /// </summary>
        /// <param name="queryType">
        /// 1：校区
        /// 2：区域
        /// 3：楼宇
        /// 4：房间
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetEnergyStatisDayForStatistic(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum, string statisticMode);

        /// <summary>
        /// 获取能耗按天统计分析数据
        /// </summary>
        /// <param name="queryType">
        /// 1：校区
        /// 2：区域
        /// 3：楼宇
        /// 4：房间
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>         
        /// <returns></returns>
        IList<ChartStatisEntity> GetEnergyStatisDay(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum);

        /// <summary>
        /// 获取能耗按天统计分析数据(包含多个建筑单位面积能耗分析)
        /// </summary>
        /// <param name="queryType">
        /// 1：校区
        /// 2：区域
        /// 3：楼宇
        /// 4：房间
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param> 
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetEnergyStatisDayAnalyze(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum, string statisticMode);

        /// <summary>
        /// 获取能耗按天统计总能耗值
        /// </summary>
        /// <param name="queryType">
        /// 1：校区
        /// 2：区域
        /// 3：楼宇
        /// 4：房间
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        double GetEnergyStatisDaySum(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes);

        /// <summary>
        /// 获取能耗按天统计最大最小的元祖
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <param name="maxMinStr"></param>
        /// <returns></returns>
        ChartStatisEntity GetEnergyStatisDayMaxMin(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum, string maxMinStr);

        /// <summary>
        /// 获取能耗按月统计数据个数
        /// </summary>
        /// <param name="queryType">
        /// 1：校区
        /// 2：区域
        /// 3：楼宇
        /// 4：房间
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        int GetEnergyStatisMonthCount(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes);

        /// <summary>
        /// 获取能耗按月统计分析数据
        /// </summary>
        /// <param name="queryType">
        /// 1：校区
        /// 2：区域
        /// 3：楼宇
        /// 4：房间
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetEnergyStatisMonth(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum);

        /// <summary>
        /// 获取能耗按月统计数据
        /// </summary>
        /// <param name="queryType">
        /// 1：校区
        /// 2：区域
        /// 3：楼宇
        /// 4：房间
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetEnergyStatisMonthForStatistic(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum,string statisticMode);

        /// <summary>
        /// 获取能耗按指定月份统计数据
        /// </summary>
        /// <param name="queryType">
        /// 1：校区
        /// 2：区域
        /// 3：楼宇
        /// 4：房间
        /// 5：测点
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="assignMonth"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetEnergyAssignStatisMonth(int queryType, int id, DateTime startTime, DateTime endTime, string assignMonth, string[] powerTypes, double sum);

        /// <summary>
        /// 获取能耗按指定月份统计数据(包含建造单位面积能耗分析)
        /// </summary>
        /// <param name="queryType">
        /// 1：校区
        /// 2：区域
        /// 3：楼宇
        /// 4：房间
        /// 5：测点
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="assignMonth"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetEnergyAssignStatisMonthAnalyze(int queryType, int id, DateTime startTime, DateTime endTime, string assignMonth, string[] powerTypes, double sum, string statisticMode);

        /// <summary>
        /// 获取能耗按年统计分析数据
        /// </summary>
        /// <param name="queryType">
        /// 1：校区
        /// 2：区域
        /// 3：楼宇
        /// 4：房间
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetEnergyStatisYear(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum);

         /// <summary>
        /// 获取所有建筑类型按年统计分析的能耗数据
        /// </summary>
        /// <param name="queryType">  
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="buildingTypeName"></param>
        /// <param name="sum"></param>    
        /// <returns></returns>
        IList<ChartStatisEntity> GetEnergyStatisYearForAllBuildingType(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, string buildingTypeName, double sum);

         /// <summary>
        /// 获取能耗按年统计数据
        /// </summary>
        /// <param name="queryType">
        /// 1：校区
        /// 2：区域
        /// 3：楼宇
        /// 4：房间
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetEnergyStatisYearForStatistic(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum, string statisticMode);

        /// <summary>
        /// 获取能耗按月统计总能耗值
        /// </summary>
        /// <param name="queryType">
        /// 1：校区
        /// 2：区域
        /// 3：楼宇
        /// 4：房间
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        double GetEnergyStatisMonthSum(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes);

        /// <summary>
        /// 获取能耗按月统计最大最小的元祖
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <param name="maxMinStr"></param>
        /// <returns></returns>
        ChartStatisEntity GetEnergyStatisMonthMaxMin(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum, string maxMinStr);

        /// <summary>
        /// 按照分类分组查找能耗个数
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        int GetEnergyByPowerCount(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum);

        /// <summary>
        /// 获取按照分类分组的能耗使用情况
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IQueryable<ChartStatisEntity> GetEnergyByPower(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum);

        /// <summary>
        /// 获取按照天粒度和分类分组的能耗使用情况
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IQueryable<ChartStatisEntity> GetDayEnergyByGranularityPower(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum);

        /// <summary>
        /// 获取按照月粒度和分类分组的能耗使用情况
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IQueryable<ChartStatisEntity> GetMonthEnergyByGranularityPower(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum);

         /// <summary>
        /// 按年粒度查询所有建筑类型的所有日期
        /// </summary>
        /// <param name="queryType"></param>       
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        IQueryable<DateTime> GetDateByYearForStatistic(int queryType, DateTime startTime, DateTime endTime, string[] powerTypes);

         /// <summary>
        /// 获取所有建筑类型指定时间范围内的能耗总值
        /// </summary>
        /// <param name="queryType"></param>       
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        double GetEnergySumForStatistic(int queryType, DateTime startTime, DateTime endTime, string[] powerTypes);

        /// <summary>
        /// 按天粒度查询能耗的所有日期
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        IList<DateTime> GetDateByDay(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes);

        /// <summary>
        /// 按月粒度查询能耗的所有日期
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        IList<DateTime> GetDateByMonth(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes);

        /// <summary>
        /// 按指定月份粒度查询能耗的所有日期
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="assignMonth"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        IList<DateTime> GetDateByAssignMonth(int queryType, int id, DateTime startTime, DateTime endTime, string assignMonth, string[] powerTypes);
        
        /// <summary>
        /// 查询多个查询对象的能耗(废弃)
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IQueryable<ChartStatisEntity> GetEnergyByIDs(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes, double sum);

        /// <summary>
        /// 获取多个对象一段时间内的总能耗
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="statisticMode"></param>
        double GetAllEnergyByIDs(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes, string statisticMode);

        /// <summary>
        /// 得到指定条件测点包含的查询对象ID集合
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        IQueryable<int?> GetEnergyIDs(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes);

        /// <summary>
        /// 根据查询对象和能耗类别分组获取能耗使用集合
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IQueryable<ChartStatisEntity> GetEnergyByIDsPower(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes, double sum, string statisticMode);

        /// <summary>
        /// 根据查询对象和按天粒度分组获取能耗使用集合
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>       
        /// <returns></returns>
        IList<ChartStatisEntity> GetDayEnergyByIDsGranularity(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes, double sum);

        /// <summary>
        /// 根据查询对象和按天粒度分组获取能耗使用集合（包含多个建筑单位面积能耗分析）
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetDayEnergyByIDsGranularityForAnalyze(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes, double sum, string statisticMode);

         /// <summary>
        /// 根据建筑类型和按年粒度获取能耗使用集合
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="buildingType"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetYearEnergyByIDsGranularityForAllBuildingTypes(int queryType, DateTime startTime, DateTime endTime, string[] powerTypes, double sum);

        /// <summary>
        /// 根据查询结果的AnalogNo集获取能耗一年中按月粒度使用集合
        /// </summary>      
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>        
        /// <returns></returns>
        IList<ThirdPoint> GetMonthEnergyByAnalogNosGranularity(DateTime startTime, DateTime endTime, string[] powerTypes);
        
        /// <summary>
        /// 根据查询对象和按月粒度分组获取能耗使用集合
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetMonthEnergyByIDsGranularity(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes, double sum);

        /// <summary>
        /// 根据查询对象和按月粒度分组获取能耗使用集合
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetMonthEnergyByIDsGranularityForAnalyze(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes, double sum,string statisticMode);

        /// <summary>
        /// 根据查询对象和按指定月份粒度分组获取能耗使用集合
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetAssignMonthEnergyByIDsGranularity(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string assignMonth, string[] powerTypes, double sum);

        /// <summary>
        /// 根据查询对象和按指定月份粒度分组获取能耗使用集合(包含建筑单位面积能耗分析)
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetAssignMonthEnergyByIDsGranularityForAnalyze(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string assignMonth, string[] powerTypes, double sum, string statisticMode);

        /// <summary>
        /// 根据查询对象和按指定日期粒度分组获取能耗使用集合
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetAssignDayEnergyByIDsGranularity(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string assignDay, string[] powerTypes, double sum);

       /// <summary>
        /// 根据查询对象和按指定日期粒度分组获取能耗使用集合(包含建筑单位面积能耗分析)
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetAssignDayEnergyByIDsGranularityForAnalyze(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string assignDay, string[] powerTypes, double sum, string statisticMode);
  
        /// <summary>
        /// 获取能耗按指定日期统计数据(包含建造单位面积能耗分析)
        /// </summary>
        /// <param name="queryType">
        /// 1：校区
        /// 2：区域
        /// 3：楼宇
        /// 4：房间
        /// 5：测点
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="assignDay"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetEnergyAssignStatisDayAnalyze(int queryType, int id, DateTime startTime, DateTime endTime, string assignDay, string[] powerTypes, double sum, string statisticMode);

         /// <summary>
        /// 获取能耗按指定日期统计数据
        /// </summary>
        /// <param name="queryType">
        /// 1：校区
        /// 2：区域
        /// 3：楼宇
        /// 4：房间
        /// 5：测点
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="assignDay"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetEnergyAssignStatisDay(int queryType, int id, DateTime startTime, DateTime endTime, string assignDay, string[] powerTypes, double sum);

        /// <summary>
        /// 按天粒度查询能耗的所有日期
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids">查询对象数组</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        IQueryable<DateTime> GetDateByDay(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes);

         /// <summary>
        /// 按天粒度查询能耗的所有日期(包含建筑单位面积能耗分析)
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids">查询对象数组</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IQueryable<DateTime> GetDateByDayForAnalyze(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes, string statisticMode);

        /// <summary>
        /// 按月粒度查询能耗的所有日期
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids">查询对象数组</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        IQueryable<DateTime> GetDateByMonth(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes);

        /// <summary>
        /// 按月粒度查询能耗的所有日期(包含建筑单位面积能耗分析)
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids">查询对象数组</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IQueryable<DateTime> GetDateByMonthForAnalyze(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes, string statisticMode);

        /// <summary>
        /// 按指定月份粒度查询能耗的所有日期
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids">查询对象数组</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        IQueryable<DateTime> GetDateByAssignMonth(int queryType, int?[] ids, DateTime startTime, DateTime endTime,string assignMonth, string[] powerTypes);

         /// <summary>
        /// 按指定月份粒度查询能耗的所有日期(包含建造单位面积能耗分析)
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids">查询对象数组</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="assignMonth"></param>
        /// <param name="powerTypes"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IQueryable<DateTime> GetDateByAssignMonthForAnalyze(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string assignMonth, string[] powerTypes, string statisticMode);

        /// <summary>
        /// 按指定月份粒度查询能耗的所有日期
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids">查询对象数组</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="assignDay"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        IQueryable<DateTime> GetDateByAssignDay(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string assignDay, string[] powerTypes);
        
        /// <summary>
        /// 按指定日期粒度查询能耗的所有日期(包含建造单位面积能耗分析)
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids">查询对象数组</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="assignDay"></param>
        /// <param name="powerTypes"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IQueryable<DateTime> GetDateByAssignDayForAnalyze(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string assignDay, string[] powerTypes, string statisticMode);

        /// <summary>
        /// 获取多对象一点时间内范围内的能耗总值
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>        
        double GetEnergySum(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes,string statisticMode);

        /// <summary>
        /// 获取该建筑当年能耗。
        /// </summary>
        /// <param name="buildingID"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        double GetEnergyBuildingCYear(int buildingID, string[] powerTypes);

        /// <summary>
        /// 获取月粒度多个对象一段时间内能耗总量(废弃)
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="statisticMode"></param>  
        /// <returns></returns>
        double GetEnergyMonthSum(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes,string statisticMode);

        /// <summary>
        /// 得到能耗查询个数
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="powerTypes"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        int GetEnergyCount(int queryType, int?[] ids, string[] powerTypes, DateTime startTime, DateTime endTime);

        /// <summary>
        /// 查询单个对象的能耗
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="id"></param>
        /// <param name="powerTypes"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        double GetEnergy(int queryType, int id, string[] powerTypes, DateTime startTime, DateTime endTime);

        /// <summary>
        /// 查询多个对象的能耗
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="powerTypes"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IList<EnergyEntity> GetEnergy(int queryType, int?[] ids, string[] powerTypes, DateTime startTime, DateTime endTime);

        /// <summary>
        /// 查询分析多个对象的能耗
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="powerTypes"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetEnergy(int queryType, int?[] ids, string[] powerTypes, DateTime startTime, DateTime endTime, double sum,string statisticMode);

        /// <summary>
        /// 获取对象每小时用电
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IQueryable<ChartStatisEntity> GetEnergyStatisHour(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum);

        /// <summary>
        /// 获取对象每小时用电进行统计
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IQueryable<ChartStatisEntity> GetEnergyStatisHourForStatistic(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum, string statisticMode);

        /// <summary>
        /// 取出对象每天指定时间段的用能值
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="startHour"></param>
        /// <param name="endHour"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IQueryable<ChartStatisEntity> GetEnergyStatisSpecialHours(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, int startHour, int endHour, double sum);

         /// <summary>
        /// 取出对象每天指定时间段的用能值进行统计
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="startHour"></param>
        /// <param name="endHour"></param>
        /// <param name="sum"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        IQueryable<ChartStatisEntity> GetEnergyStatisSpecialHoursForStatistic(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, int startHour, int endHour, double sum, string statisticMode);

        /// <summary>
        /// 获取指定测点指定时间前后的值
        /// </summary>
        /// <param name="analogNo">测点编号</param>
        /// <param name="inputDateTime">时间</param>
        /// <returns>返回包含值的Dictionary，其中键min表示小的值，max表示较大的值</returns>
        IDictionary<string, string> GetTwoEndpointVal(int analogNo, DateTime inputDateTime);

        /// <summary>
        /// 获取指定测点指定时间前后的值，不论输入时刻是否已经存在
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="inputDateTime"></param>
        /// <returns></returns>
        IDictionary<string, string> GetTwoEndpointValAlt(int analogNo, DateTime inputDateTime);

        /// <summary>
        /// 修改测点值
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="time"></param>
        /// <param name="modifyVal"></param>
        /// <returns></returns>
        bool Modify(int analogNo, DateTime time, double modifyVal);

        /// <summary>
        /// 删除测点值
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        bool Delete(int analogNo, DateTime time);

        /// <summary>
        /// 获取指定时间的指定小时之间的能耗使用值
        /// </summary>
        /// <param name="queryType">对象类型 1:校区,2:区域,3:建筑,4:房间</param>
        /// <param name="id">对象id</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="powerTypes">能耗类型</param>
        /// <param name="startHour">统计的小时段的开始小时</param>
        /// <param name="endHour">统计的小时段的结束小时</param>
        /// <returns>包含ChartStatisEntity对象的IQueryable</returns>
        IQueryable<ChartStatisEntity> GetEnergyBySpecialHours(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, int startHour, int endHour);

        /// <summary>
        /// 查询对象历史值
        /// </summary>
        /// <param name="queryType">类型，1：校区；2：区域；3：建筑；4：房间</param>
        /// <param name="id">对象id</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="powerType">能耗类型</param>
        /// <returns>得到的对象历史值集合</returns>
        IQueryable<EnergyEntity> GetHistoryVal(int queryType, int id, DateTime startTime, DateTime endTime, string powerType);

        /// <summary>
        /// 获取光华楼中多个对象一段时间内的总能耗
        /// </summary>
        /// <param name="queryType"></param>
        /// 1：建筑
        /// 2：配电室
        /// 3：变压器
        /// 4：测点
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        double GetBuildingGuanghuaAllEnergyByIDs(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes);

        /// <summary>
        /// 查询分析光华楼中多个对象的能耗
        /// </summary>
        /// <param name="queryType"></param>
        /// 1：建筑
        /// 2：配电室
        /// 3：变压器
        /// 4：测点
        /// <param name="ids"></param>
        /// <param name="powerTypes"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetBuildingGuanghuaEnergy(int queryType, int?[] ids, string[] powerTypes, DateTime startTime, DateTime endTime, double sum);

        /// <summary>
        /// 查询光华楼中多个对象的能耗
        /// </summary>
        /// <param name="queryType"></param>
        /// 1：建筑
        /// 2：配电室
        /// 3：变压器
        /// 4：测点
        /// <param name="ids"></param>
        /// <param name="powerTypes"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IList<EnergyEntity> GetBuildingGuanghuaEnergy(int queryType, int?[] ids, string[] powerTypes, DateTime startTime, DateTime endTime);

         /// <summary>
        /// 查询光华楼单个对象的能耗
        /// </summary>
        /// <param name="queryType"></param>
        /// 1：建筑
        /// 2：配电室
        /// 3：变压器
        /// 4：测点
        /// <param name="id"></param>
        /// <param name="powerTypes"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        double GetBghEnergy(int queryType, int id, string[] powerTypes, DateTime startTime, DateTime endTime);

        /// <summary>
        /// 获取光华楼中多对象一点时间内范围内的能耗总值
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        double GetBuildingGuanghuaEnergySum(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes);

        /// <summary>
        /// 根据查询对象和按指定月份粒度分组获取光华楼能耗使用集合
        /// </summary>
        /// <param name="queryType"></param>
        /// 1：建筑
        /// 2：配电室
        /// 3：变压器
        /// 4：测点
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetBuildingGuanghuaAssignMonthEnergyByIDsGranularity(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string assignMonth, string[] powerTypes, double sum);

        /// <summary>
        /// 获取光华楼能耗按指定月份统计数据
        /// </summary>
        /// <param name="queryType">
        /// 1：建筑
        /// 2：配电室
        /// 3：变压器
        /// 4：测点
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="assignMonth"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetBuildingGuanghuaEnergyAssignStatisMonth(int queryType, int id, DateTime startTime, DateTime endTime, string assignMonth, string[] powerTypes, double sum);

        /// <summary>
        /// 根据查询对象和按指定日期粒度分组获取光华楼能耗使用集合
        /// </summary>
        /// <param name="queryType"></param>
        /// 1：建筑
        /// 2：配电室
        /// 3：变压器
        /// 4：测点
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetBuildingGuanghuaAssignDayEnergyByIDsGranularity(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string assignDay, string[] powerTypes, double sum);

        /// <summary>
        /// 按天粒度查询光华楼能耗的所有日期
        /// </summary>
        /// <param name="queryType"></param>
        /// 1：建筑
        /// 2：配电室
        /// 3：变压器
        /// 4：测点
        /// <param name="ids">查询对象数组</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        IQueryable<DateTime> GetBuildingGuanghuaDateByDay(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes);

        /// <summary>
        /// 按月粒度查询光华楼能耗的所有日期
        /// </summary>
        /// <param name="queryType"></param>
        /// 1：建筑
        /// 2：配电室
        /// 3：变压器
        /// 4：测点
        /// <param name="ids">查询对象数组</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        IQueryable<DateTime> GetBuildingGuanghuaDateByMonth(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes);

        /// <summary>
        /// 按指定月份粒度查询光华楼能耗的所有日期
        /// </summary>
        /// <param name="queryType"></param>
        /// 1：建筑
        /// 2：配电室
        /// 3：变压器
        /// 4：测点
        /// <param name="ids">查询对象数组</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="assignMonth"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        IQueryable<DateTime> GetBuildingGuanghuaDateByAssignMonth(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string assignMonth, string[] powerTypes);       
 
         /// <summary>
        /// 按指定日期粒度查询光华楼能耗的所有日期
        /// </summary>
        /// <param name="queryType"></param>
        /// 1：建筑
        /// 2：配电室
        /// 3：变压器
        /// 4：测点
        /// <param name="ids">查询对象数组</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="assignDay"></param>
        /// <param name="powerTypes"></param>
        /// <returns></returns>
        IQueryable<DateTime> GetBuildingGuanghuaDateByAssignDay(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string assignDay, string[] powerTypes);

        /// <summary>
        /// 获取光华楼能耗按指定日期统计数据
        /// </summary>
        /// <param name="queryType">
        /// 1：建筑
        /// 2：配电室
        /// 3：变压器
        /// 4：测点
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="assignDay"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetBuildingGuanghuaEnergyAssignStatisDay(int queryType, int id, DateTime startTime, DateTime endTime, string assignDay, string[] powerTypes, double sum);

         /// <summary>
        /// 根据查询对象和按天粒度分组获取光华楼能耗使用集合
        /// </summary>
        /// <param name="queryType"></param>
        /// 1：建筑
        /// 2：配电室
        /// 3：变压器
        /// 4：测点
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetBuildingGuanghuaDayEnergyByIDsGranularity(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes, double sum);

        /// <summary>
        /// 根据查询对象和按月粒度分组获取光华楼能耗使用集合
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetBuildingGuanghuaMonthEnergyByIDsGranularity(int queryType, int?[] ids, DateTime startTime, DateTime endTime, string[] powerTypes, double sum);

        /// <summary>
        /// 获取光华楼能耗按天统计数据
        /// </summary>
        /// <param name="queryType">
        /// 1：建筑
        /// 2：配电室
        /// 3：变压器
        /// 4：测点
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetBuildingGuanghuaEnergyStatisDay(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum);

         /// <summary>
        /// 获取光华楼能耗按月统计数据
        /// </summary>
        /// <param name="queryType">
        /// 1：建筑
        /// 2：配电室
        /// 3：变压器        
        /// 4 : 测点
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetBuildingGuanghuaEnergyStatisMonth(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum);

         /// <summary>
        /// 获取光华楼能耗按年统计数据
        /// </summary>
        /// <param name="queryType">
        /// 1：建筑
        /// 2：配电室
        /// 3：变压器
        /// 4：测点
        /// </param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IList<ChartStatisEntity> GetBuildingGuanghuaEnergyStatisYear(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum);

         /// <summary>
        /// 取出光华楼每天指定时间段的用能值
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="startHour"></param>
        /// <param name="endHour"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IQueryable<ChartStatisEntity> GetBuildingGuanghuaEnergyStatisSpecialHours(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, int startHour, int endHour, double sum);

        /// <summary>
        /// 获取光华楼每小时用电
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerTypes"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IQueryable<ChartStatisEntity> GetBuildingGuanghuaEnergyStatisHour(int queryType, int id, DateTime startTime, DateTime endTime, string[] powerTypes, double sum);

        /// <summary>
        /// 获取完整名称的查询对象的一段时间的使用能耗值
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="ids"></param>
        /// <param name="buildingInSchoolId">建筑所属校区，当queryType为楼宇，即为3时，且ids为null时有效</param>
        /// <param name="powerTypes"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        IQueryable<ChartStatisEntity> GetEnergyFullName(int queryType, int?[] ids, String buildingType, int buildingInSchoolId, string[] powerTypes, DateTime startTime, DateTime endTime, double sum);

        /// <summary>
        /// 导入月历史测点数据，如果月份与最新的数据不连续，则取导入月份的测点值为最早时间的值，即可能会造成不连续的月份查询值为0，实际上这之间是有值的
        /// </summary>
        /// <param name="excelData">第一行为导入的测点月份，格式为yyyy-mm，格式不正确则忽略导入该月份，第一列为导入的测点编号</param>
        /// <param name="powerType">只允许添加的测点类型，为空表示可以添加任何类型测点</param>
        /// <returns>返回成功导入的测点月历史数据的测点个数，部分测点可能因为值输入值不正确导致只导入部分月份值</returns>
        int ImportMonthData(DataSet excelData, string powerType);

        /// <summary>
        /// 导入月历史测点数据，仅能导入“001006”的用电类型，即当前数据库中历史用电类型，如果月份与最新的数据不连续，则取导入月份的测点值为最早时间的值，即可能会造成不连续的月份查询值为0，实际上这之间是有值的
        /// </summary>
        /// <param name="excelData">第一行为导入的测点月份，格式为yyyy-mm，格式不正确则忽略导入该月份，第一列为导入的测点编号</param>
        /// <returns>返回成功导入的测点月历史数据的测点个数，部分测点可能因为值输入值不正确导致只导入部分月份值</returns>
        int ImportMonthData(DataSet excelData);

        /// <summary>
        /// 查询指定测点某段时间每小时能耗读数
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IQueryable<EnergyEntity> GetHourEnergyHistoryByAnalogNo(int analogNo , DateTime startTime , DateTime endTime);

        /// <summary>
        /// 查询指定测点某段时间每天能耗读数
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IQueryable<EnergyEntity> GetdayEnergyHistoryByAnalogNo(int analogNo, DateTime startTime, DateTime endTime);

        bool UpdateHistoryValOfAnalogNo(int analogNo, DateTime startTime, DateTime endTime, IList<AnalogMeasurePoint> sonPointsList);
    }
}
