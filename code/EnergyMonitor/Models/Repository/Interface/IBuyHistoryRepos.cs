using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;
using EnergyMonitor.Models.Repository.Entity;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IBuyHistoryRepos
    {
        /// <summary>
        /// 查询指定之间内房间的购电历史数据 
        /// </summary>
        /// <param name="roomID">房间号</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="skipItems">跳过个数</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        IList<EnergyHistory> GetHistory(int roomID, DateTime startTime, DateTime endTime, int skipItems, int pageSize);

        /// <summary>
        /// 得到指定之间内房间的购电历史数据个数
        /// </summary>
        /// <param name="roomID">房间号</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        int GetHistoryCount(int roomID, DateTime startTime, DateTime endTime);

        /// <summary>
        /// 查询指定之间内房间的购电历史数据(提供给手机端使用)
        /// </summary>
        /// <param name="roomID">房间号</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="skipItems">跳过个数</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        IList<EnergyHistory> GetHistoryForMobile(int roomID, DateTime startTime, DateTime endTime, int skipItems, int pageSize);

        /// <summary>
        /// 得到指定之间内房间的购电历史数据个数(提供给手机端使用)
        /// </summary>
        /// <param name="roomID">房间号</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        int GetHistoryCountForMobile(int roomID, DateTime startTime, DateTime endTime);
    }
}
