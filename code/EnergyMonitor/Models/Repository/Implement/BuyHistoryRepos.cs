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
    public class BuyHistoryRepos : IBuyHistoryRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public BuyHistoryRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region IBuyHistoryRepos Members

        /// <summary>
        /// 查询指定之间内房间的购电历史数据
        /// </summary>
        /// <param name="roomID">房间号</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="skipItems">跳过个数</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public IList<EnergyHistory> GetHistory(int roomID, DateTime startTime, DateTime endTime, int skipItems, int pageSize)
        {
            var list = (from amp in _dataContext.AnalogMeasurePoints
                        join b in _dataContext.BuyHistories on amp.AMP_AnalogNo equals b.BH_AnalogNo
                        join ri in _dataContext.RoomInfos on amp.AMP_RoomID equals ri.RI_ID
                        where amp.AMP_RoomID == roomID && b.BH_Buydate >= startTime && b.BH_Buydate <= endTime
                        orderby b.BH_Buydate descending
                        select new EnergyHistory
                        {
                            PNO = amp.AMP_AnalogNo,
                            RoomName = ri.RI_RoomCode,
                            Buyer = b.BH_Buyer,
                            BuyMoney = b.BH_Moneyqty,
                            BuyVal = b.BH_Buyqty,
                            OperType = b.BH_OperType,
                            BuyDate = b.BH_Buydate
                        }).Distinct().OrderBy(x => x.BuyDate).Skip(skipItems).Take(pageSize);
            return list.ToList();
        }

        /// <summary>
        /// 查询指定之间内房间的购电历史数据(提供给手机端使用)
        /// </summary>
        /// <param name="roomID">房间号</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="skipItems">跳过个数</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public IList<EnergyHistory> GetHistoryForMobile(int roomID, DateTime startTime, DateTime endTime, int skipItems, int pageSize)
        {
            var list = (from amp in _dataContext.AnalogMeasurePoints
                        join b in _dataContext.BuyHistories on amp.AMP_AnalogNo equals b.BH_AnalogNo
                        join ri in _dataContext.RoomInfos on amp.AMP_RoomID equals ri.RI_ID
                        where amp.AMP_RoomID == roomID && b.BH_Buydate >= startTime && b.BH_Buydate <= endTime && b.BH_OperType != "电量核销"
                        orderby b.BH_Buydate descending
                        select new EnergyHistory
                        {
                            PNO = amp.AMP_AnalogNo,
                            RoomName = ri.RI_RoomCode,
                            Buyer = b.BH_Buyer,
                            BuyMoney = b.BH_Moneyqty,
                            BuyVal = b.BH_Buyqty,
                            OperType = b.BH_OperType,
                            BuyDate = b.BH_Buydate                          
            }).Distinct().OrderByDescending(x => x.BuyDate).Skip(skipItems).Take(pageSize);
            return list.ToList();
       
        }

        /// <summary>
        /// 得到指定之间内房间的购电历史数据个数(提供给手机端使用)
        /// </summary>
        /// <param name="roomID">房间号</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public int GetHistoryCountForMobile(int roomID, DateTime startTime, DateTime endTime)
        {
            var count = (from amp in _dataContext.AnalogMeasurePoints
                         join b in _dataContext.BuyHistories on amp.AMP_AnalogNo equals b.BH_AnalogNo
                         join ri in _dataContext.RoomInfos on amp.AMP_RoomID equals ri.RI_ID
                         where amp.AMP_RoomID == roomID && b.BH_Buydate >= startTime && b.BH_Buydate <= endTime && b.BH_OperType != "电量核销"
                         select new EnergyHistory
                         {
                             PNO = amp.AMP_AnalogNo,
                             RoomName = ri.RI_RoomCode,
                             Buyer = b.BH_Buyer,
                             BuyMoney = b.BH_Moneyqty,
                             BuyVal = b.BH_Buyqty,
                             OperType = b.BH_OperType,
                             BuyDate = b.BH_Buydate
                         }).Distinct().Count();
            return count;
        }

        /// <summary>
        /// 得到指定之间内房间的购电历史数据个数
        /// </summary>
        /// <param name="roomID">房间号</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public int GetHistoryCount(int roomID, DateTime startTime, DateTime endTime)
        {
            var count = (from amp in _dataContext.AnalogMeasurePoints
                         join b in _dataContext.BuyHistories on amp.AMP_AnalogNo equals b.BH_AnalogNo
                         where amp.AMP_RoomID == roomID && b.BH_Buydate >= startTime && b.BH_Buydate <= endTime
                        select b).Distinct().Count();
            return count;
        }

        #endregion
    }
}
