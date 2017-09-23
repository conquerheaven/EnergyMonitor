using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Collections;
using EnergyMonitor.Controllers.Utils;
using EnergyMonitor.Controllers.User.Filters;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Implement;
using EnergyMonitor.Models.Repository.Entity;
using EnergyMonitor.Models.LinqEntity;

namespace EnergyMonitor.Controllers.User
{
    /// <summary>
    /// 能耗统计控制类
    /// </summary>
    [UserFilter]
    public class EnergyController : Controller
    {
        private IUserRepos _userRepos = null;
        private IAMPRepos _ampRepos = null;
        private IAnalogHistoryRepos _analogHistoryRepos = null;
        private IBuyHistoryRepos _buyHistoryRepos = null;

        public EnergyController()
            : this(new UserRepos(), new AMPRepos(), new AnalogHistoryRepos(), new BuyHistoryRepos())
        {
        }

        public EnergyController(IUserRepos userRepos, IAMPRepos ampRepos, IAnalogHistoryRepos analogHistoryRepos, IBuyHistoryRepos buyHistoryRepos)
        {
            _userRepos = userRepos;
            _ampRepos = ampRepos;
            _analogHistoryRepos = analogHistoryRepos;
            _buyHistoryRepos = buyHistoryRepos;
        }

        /// <summary>
        /// 用电历史页面
        /// </summary>
        /// <returns></returns>
        public ActionResult EnergyHistory()
        {
            EnergyMonitor.Models.LinqEntity.User loginUser = Session["loginUser"] as EnergyMonitor.Models.LinqEntity.User;
            if (loginUser != null)
            {
                if (Session["userRoomInfo"] != null)
                {
                    var list = Session["userRoomInfo"];
                    return View(list);
                }
                else
                {
                    var list = _userRepos.GetUserRelatedRooms(loginUser.USR_ID);
                    Session["userRoomInfo"] = list;
                    return View(list);
                }
            }
            return RedirectToAction("Error", "Shared");
        }

        /// <summary>
        /// Ajax得到得到房间用电量
        /// </summary>
        /// <param name="roomID">房间号</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public ActionResult GetRoomEnergyAjax(int? roomID, DateTime startTime, DateTime endTime, string granularity)
        {
            //if (Request.IsAjaxRequest() && roomID != null)
            if (roomID != null)
            {
                endTime = endTime.AddDays(1);
                int totalItem = 0;
                if (!String.IsNullOrWhiteSpace(granularity))
                {
                    if (granularity == "day")
                    {
                        totalItem = _analogHistoryRepos.GetEnergyDayConsumeCount(roomID, startTime, endTime);
                    }
                    else
                    {
                        totalItem = _analogHistoryRepos.GetEnergyMonthConsumeCount(roomID, startTime, endTime);
                    }
                }
                Pager pager = new Pager(1, totalItem);
                int totalPage = pager.TotalPages;
                double? resDouble = _analogHistoryRepos.GetEnergyConsume(roomID, startTime, endTime);
                //DateTime realStartTime = _analogHistoryRepos.GetStartTime(roomID);
                double res = 0;
                if (resDouble.HasValue)
                {
                    res = resDouble.Value;
                }
                var result = new
                {
                    totalRes = res.ToString("f1"),
                    totalPage = totalPage,
                    //realStartDate = realStartTime.ToString("yyyy-MM-dd"),
                    realEndDate = DateTime.Now.ToString("yyyy-MM-dd")
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// Ajax得到房间详细信息
        /// </summary>
        /// <param name="roomID">房间号</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="currentPage">当前页</param>
        /// <returns></returns>
        public ActionResult GetDetailRoomEnergyAjax(int? roomID, DateTime startTime, DateTime endTime, string granularity, int currentPage)
        {
            //if (Request.IsAjaxRequest() && roomID.HasValue && roomID.Value > 0)
            if (roomID.HasValue && roomID.Value > 0)
            {
                endTime = endTime.AddDays(1);
                int pageSize = Util.PageSize;
                int skipItems = (currentPage - 1) * pageSize;
                if (!String.IsNullOrWhiteSpace(granularity))
                {
                    if (granularity == "day")
                    {
                        var list = _analogHistoryRepos.GetEnergyDayConsume(roomID, startTime, endTime, skipItems, pageSize);
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var list = _analogHistoryRepos.GetEnergyMonthConsume(roomID, startTime, endTime, skipItems, pageSize);
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 当前用电
        /// </summary>
        /// <returns></returns>
        public ActionResult EnergyBrief()
        {
            IList<UserRoomFullName> userRoomList = null;
            if (Session["userRoomInfo"] != null)
            {
                userRoomList = Session["userRoomInfo"] as IList<UserRoomFullName>;
            }
            else
            {
                EnergyMonitor.Models.LinqEntity.User loginUser = Session["loginUser"] as EnergyMonitor.Models.LinqEntity.User;
                userRoomList = _userRepos.GetUserRelatedRooms(loginUser.USR_ID);
                Session["userRoomInfo"] = userRoomList;
            }
            ViewBag.userRoomList = userRoomList;
            DateTime endTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-1"));
            // 开始时间为前三个月
            DateTime startTime = endTime.AddMonths(-3);
            IDictionary<int, IList<ChartStatisEntity>> dic = new Dictionary<int, IList<ChartStatisEntity>>();
            // 当月用电
            IDictionary<int, double> currentDic = new Dictionary<int, double>();
            // 剩余用电
            IDictionary<int, double> remDic = new Dictionary<int, double>();
            // 当前表值
            IDictionary<int, IList<AnalogMeasurePoint>> ampDic = new Dictionary<int, IList<AnalogMeasurePoint>>();
            foreach (var item in userRoomList)
            {
                int roomID = item.RIID;
                var ampList = _ampRepos.GetRealTimeEnergy(roomID);
                
                ampDic.Add(roomID, ampList);
                var list = _analogHistoryRepos.GetEnergyMonthConsume(roomID, startTime, endTime);
                double val = _analogHistoryRepos.GetCurrentMonthEnergy(roomID);
                currentDic.Add(roomID, val);
                dic.Add(roomID, list);
                double remVal = _analogHistoryRepos.GetCurrentRemVal(roomID);
                remDic.Add(roomID, remVal);
            }
            ViewBag.currentDic = currentDic;
            ViewBag.remDic = remDic;
            ViewBag.ampDic = ampDic;
            return View(dic);
        }

        /// <summary>
        /// 当前用电值（只返回值不返回页面）
        /// </summary>
        /// <returns></returns>
        public ActionResult EnergyBriefVal()
        {
            IList<UserRoomFullName> userRoomList = null;
            if (Session["userRoomInfo"] != null)
            {
                userRoomList = Session["userRoomInfo"] as IList<UserRoomFullName>;
            }
            else
            {
                EnergyMonitor.Models.LinqEntity.User loginUser = Session["loginUser"] as EnergyMonitor.Models.LinqEntity.User;
                userRoomList = _userRepos.GetUserRelatedRooms(loginUser.USR_ID);
                Session["userRoomInfo"] = userRoomList;
            }
            ViewBag.userRoomList = userRoomList;
            DateTime endTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-1"));
            // 开始时间为前三个月
            DateTime startTime = endTime.AddMonths(-3);
            IDictionary<int, IList<ChartStatisEntity>> dic = new Dictionary<int, IList<ChartStatisEntity>>();
            // 当月用电
            IDictionary<int, double> currentDic = new Dictionary<int, double>();
            // 剩余用电
            IDictionary<int, double> remDic = new Dictionary<int, double>();
            // 当前表值
            IDictionary<int, IList<AnalogMeasurePoint>> ampDic = new Dictionary<int, IList<AnalogMeasurePoint>>();
            foreach (var item in userRoomList)
            {
                int roomID = item.RIID;
                var ampList = _ampRepos.GetRealTimeEnergy(roomID);

                ampDic.Add(roomID, ampList);
                var list = _analogHistoryRepos.GetEnergyMonthConsume(roomID, startTime, endTime);
                double val = _analogHistoryRepos.GetCurrentMonthEnergy(roomID);
                currentDic.Add(roomID, val);
                dic.Add(roomID, list);
                double remVal = _analogHistoryRepos.GetCurrentRemVal(roomID);
                remDic.Add(roomID, remVal);
            }
            ViewBag.currentDic = currentDic;
            ViewBag.remDic = remDic;
            ViewBag.ampDic = ampDic;
            string roomName = "";
            int roomId = 0;
            string AMP_ValRem = "";
            string AMP_Val = "";
            foreach (var room in userRoomList)
            {
                roomName = room.urFullName;
                roomId = room.RIID;
            }
            foreach (var ampItem in ampDic[roomId])
            {
                AMP_Val = ampItem.AMP_Val.ToString("f1");
                AMP_ValRem = ampItem.AMP_ValRem.ToString();
            }
            var result = new
            {
                roomName = roomName,
                AMP_Val = AMP_Val,
                AMP_ValRem = AMP_ValRem,
                currentVal = currentDic[roomId].ToString("f1")
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 当前用电值（只返回值不返回页面）
        /// </summary>
        /// <returns></returns>
        public ActionResult EnergyBriefValForMobile(string useID)
        {
            IList<UserRoomFullName> userRoomList = _userRepos.GetUserRelatedRooms(useID);
           // ViewBag.userRoomList = userRoomList;
            DateTime endTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-1"));
            // 开始时间为前三个月
            DateTime startTime = endTime.AddMonths(-3);
            IDictionary<int, IList<ChartStatisEntity>> dic = new Dictionary<int, IList<ChartStatisEntity>>();
            // 当月用电
            IDictionary<int, double> currentDic = new Dictionary<int, double>();
            // 剩余用电
            IDictionary<int, double> remDic = new Dictionary<int, double>();
            // 当前表值
            IDictionary<int, IList<AnalogMeasurePoint>> ampDic = new Dictionary<int, IList<AnalogMeasurePoint>>();
            foreach (var item in userRoomList)
            {
                int roomID = item.RIID;
                var ampList = _ampRepos.GetRealTimeEnergy(roomID);

                ampDic.Add(roomID, ampList);
                var list = _analogHistoryRepos.GetEnergyMonthConsume(roomID, startTime, endTime);
                double val = _analogHistoryRepos.GetCurrentMonthEnergy(roomID);
                currentDic.Add(roomID, val);
                dic.Add(roomID, list);
                double remVal = _analogHistoryRepos.GetCurrentRemVal(roomID);
                remDic.Add(roomID, remVal);
            }
           // ViewBag.currentDic = currentDic;
          //  ViewBag.remDic = remDic;
          //  ViewBag.ampDic = ampDic;
            string roomName = "";
            int roomId = 0;
            string AMP_ValRem = "";
            string AMP_Val = "";
            foreach (var room in userRoomList)
            {
                roomName = room.urFullName;
                roomId = room.RIID;
            }
            foreach (var ampItem in ampDic[roomId])
            {
                AMP_Val = ampItem.AMP_Val.ToString("f1");
                AMP_ValRem = ampItem.AMP_ValRem.ToString();
            }
            var result = new
            {
                roomName = roomName,
                AMP_Val = AMP_Val,
                AMP_ValRem = AMP_ValRem,
                currentVal = currentDic[roomId].ToString("f1")
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 跳转购电历史
        /// </summary>
        /// <returns></returns>
        public ActionResult BuyHistory()
        {
            EnergyMonitor.Models.LinqEntity.User loginUser = Session["loginUser"] as EnergyMonitor.Models.LinqEntity.User;
            if (loginUser != null)
            {
                if (Session["userRoomInfo"] != null)
                {
                    var list = Session["userRoomInfo"];
                    return View(list);
                }
                else
                {
                    var list = _userRepos.GetUserRelatedRooms(loginUser.USR_ID);
                    Session["userRoomInfo"] = list;
                    return View(list);
                }
            }
            return RedirectToAction("Error", "Shared");
        }

        /// <summary>
        /// Ajax查询购电历史数据
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="totalPages"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public ActionResult QueryBuyHistoryAjax(int? roomID, int? totalPages, DateTime startTime, DateTime endTime, int currentPage)
        {
            //if (Request.IsAjaxRequest() && roomID.HasValue && totalPages.HasValue)
            if (roomID.HasValue && totalPages.HasValue)
            {
                Pager pager = null;
                if (totalPages == -1)
                {
                    int totalRows = _buyHistoryRepos.GetHistoryCount(roomID.Value, startTime, endTime);
                    pager = new Pager(1, totalRows);
                }
                else
                {
                    pager = new Pager(currentPage, totalPages.Value, false);
                }

                if (pager.TotalPages > 0)
                {
                    var list = _buyHistoryRepos.GetHistory(roomID.Value, startTime, endTime, pager.StartRow, pager.PageSize);
                    var reData = new
                    {
                        totalPages = pager.TotalPages,
                        data = list
                    };              
                    return Json(reData, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        /// <summary>
        /// Ajax查询购电历史数据(提供给手机端使用，不包括电量核销)
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="totalPages"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public ActionResult QueryBuyHistoryAjaxForMobile(int? roomID, int? totalPages, DateTime startTime, DateTime endTime, int currentPage)
        {
            //if (Request.IsAjaxRequest() && roomID.HasValue && totalPages.HasValue)
            if (roomID.HasValue && totalPages.HasValue)
            {
                Pager pager = null;
                if (totalPages == -1)
                {
                    int totalRows = _buyHistoryRepos.GetHistoryCountForMobile(roomID.Value, startTime, endTime);
                    pager = new Pager(1, totalRows);
                }
                else
                {
                    pager = new Pager(currentPage, totalPages.Value, false);
                }

                if (pager.TotalPages > 0)
                {
                    var list = _buyHistoryRepos.GetHistoryForMobile(roomID.Value, startTime, endTime, pager.StartRow, pager.PageSize);
                    var reData = new
                    {
                        totalPages = pager.TotalPages,
                        data = list
                    };
                    return Json(reData, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }
    }
}
