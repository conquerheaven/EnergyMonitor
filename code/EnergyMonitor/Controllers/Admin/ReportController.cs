using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnergyMonitor.Controllers.Admin.Filters;
using System.Collections;
using EnergyMonitor.Controllers.Utils;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Implement;


namespace EnergyMonitor.Controllers.Admin
{
    /// <summary>
    /// 报表管理
    /// </summary>
    [AdminFilter]
    public class ReportController : Controller
    {
        private ISystemProfileRepos _systemProfileRepos = null;
        private IAnalogHistoryRepos _analogHistoryRepos = null;

        public ReportController()
            : this(new SystemProfileRepos(), new AnalogHistoryRepos())
        {
        }

        public ReportController(ISystemProfileRepos systemProfileRepos, IAnalogHistoryRepos analogHistoryRepos)
        {
            _systemProfileRepos = systemProfileRepos;
            _analogHistoryRepos = analogHistoryRepos;
        }

        /// <summary>
        /// 用电报表
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult Elec(int? b, string bn)
        {
            if (b.HasValue && b.Value > 0)
            {
                ViewBag.buildingName = bn;
                return View(b.Value);
            }
            return View();
        }

        /// <summary>
        /// 跳转设置单价参数
        /// </summary>
        /// <returns></returns>
        public ActionResult SetParameters()
        {
            var priceDic = _systemProfileRepos.GetAllPrice().ToDictionary(x => x.SP_ID);
            var timeDic = _systemProfileRepos.GetByStartStr("time").ToDictionary(x => x.SP_ID);
            var timeHigh = timeDic["time_high"].SP_Value;
            var timeNormal = timeDic["time_normal"].SP_Value;
            var timeLow = timeDic["time_low"].SP_Value;
            
            IDictionary timeFlagDic = new Dictionary<int, string>();
            var timeBlocks = timeHigh.Split(new char[] { ',' });
            string timeTipStr = "";
            string highTimeTipStr = "";
            foreach (var timeStr in timeBlocks)
            {
                var times = timeStr.Split(new char[] { '-' });
                if (times != null && times.Length > 1)
                {
                    if (string.IsNullOrWhiteSpace(highTimeTipStr))
                    {
                        highTimeTipStr += times[0] + "点-" + times[1] + "点";
                    }
                    else
                    {
                        highTimeTipStr += "," + times[0] + "点-" + times[1] + "点";
                    }
                    int startTime = Int32.Parse(times[0]);
                    int endTime = Int32.Parse(times[1]);
                    for (int i = startTime; i < endTime; i++)
                    {
                        if (!timeFlagDic.Contains(i))
                        {
                            timeFlagDic.Add(i, "high");
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(highTimeTipStr))
            {
                timeTipStr += "【峰时：" + highTimeTipStr + "】";
            }
            string normalTimeTipStr = "";
            timeBlocks = timeNormal.Split(new char[] { ',' });
            foreach (var timeStr in timeBlocks)
            {
                var times = timeStr.Split(new char[] { '-' });
                if (times != null && times.Length > 1)
                {
                    if (string.IsNullOrWhiteSpace(normalTimeTipStr))
                    {
                        normalTimeTipStr += times[0] + "点-" + times[1] + "点";
                    }
                    else
                    {
                        normalTimeTipStr += "," + times[0] + "点-" + times[1] + "点";
                    }
                    int startTime = Int32.Parse(times[0]);
                    int endTime = Int32.Parse(times[1]);
                    for (int i = startTime; i < endTime; i++)
                    {
                        if (!timeFlagDic.Contains(i))
                        {
                            timeFlagDic.Add(i, "normal");
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(normalTimeTipStr))
            {
                timeTipStr += "【平时：" + normalTimeTipStr + "】";
            }
            string lowTimeTipStr = "";
            timeBlocks = timeLow.Split(new char[] { ',' });
            foreach (var timeStr in timeBlocks)
            {
                var times = timeStr.Split(new char[] { '-' });
                if (times != null && times.Length > 1)
                {
                    if (string.IsNullOrWhiteSpace(lowTimeTipStr))
                    {
                        lowTimeTipStr += times[0] + "点-" + times[1] + "点";
                    }
                    else
                    {
                        lowTimeTipStr += "," + times[0] + "点-" + times[1] + "点";
                    }
                    int startTime = Int32.Parse(times[0]);
                    int endTime = Int32.Parse(times[1]);
                    for (int i = startTime; i < endTime; i++)
                    {
                        if (!timeFlagDic.Contains(i))
                        {
                            timeFlagDic.Add(i, "low");
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(lowTimeTipStr))
            {
                timeTipStr += "【谷时：" + lowTimeTipStr + "】";
            }
            ViewBag.timeDic = timeDic;
            ViewBag.timeFlagDic = timeFlagDic;
            ViewBag.timeTipStr = timeTipStr;
           
            return View(priceDic);
        }

        /// <summary>
        /// 修改参数
        /// </summary>
        /// <returns></returns>
        public ActionResult ModifyParameters()
        {
            IDictionary dic = new Dictionary<string, string>();
            string priceHighTime = Request.Form["price_highTime"];
            string priceNormalTime = Request.Form["price_normalTime"];
            string priceLowTime = Request.Form["price_lowTime"];
            string priceWater = Request.Form["price_water"];
            string pricePollution = Request.Form["price_pollution"];
            if (!String.IsNullOrWhiteSpace(priceHighTime))
            {
                dic.Add("price_highTime", priceHighTime);
            }
            if (!String.IsNullOrWhiteSpace(priceNormalTime))
            {
                dic.Add("price_normalTime", priceNormalTime);
            }
            if (!String.IsNullOrWhiteSpace(priceLowTime))
            {
                dic.Add("price_lowTime", priceLowTime);
            }
            if (!String.IsNullOrWhiteSpace(priceWater))
            {
                dic.Add("price_water", priceWater);
            }
            if (!String.IsNullOrWhiteSpace(pricePollution))
            {
                dic.Add("price_pollution", pricePollution);
            }
            string modifyTimeBlockFlag = Request.Form["modifyTimeBlockFlag"];
            if (modifyTimeBlockFlag == "1")
            {
                string timeHigh = Request.Form["time_high"];
                string timeNormal = Request.Form["time_normal"];
                string timeLow = Request.Form["time_low"];
                dic.Add("time_high", timeHigh);
                dic.Add("time_normal", timeNormal);
                dic.Add("time_low", timeLow);
            }

            if (dic.Count > 0)
            {
                var flag = _systemProfileRepos.Modify(dic);
                return View(flag);
            }
            return View();
        }

        /// <summary>
        /// 查询建筑月用电报表
        /// </summary>
        /// <param name="b">建筑ID</param>
        /// <param name="y">查看年份</param>
        /// <param name="bn">查询建筑全称</param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult BuildingElecMonth(int? b, int? y, string bn)
        {
            if (b.HasValue && b.Value > 0)
            {
                if (y.HasValue && y.Value > 0)
                {
                    ViewBag.year = y.Value;
                }
                ViewBag.buildingName = bn;
                return View(b.Value);
            }
            return View();
        }

        /// <summary>
        /// 查询建筑月用水报表
        /// </summary>
        /// <param name="b">建筑ID</param>
        /// <param name="y">查看年份</param>
        /// <param name="bn">查询建筑全称</param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult BuildingWaterMonth(int? b, int? y, string bn)
        {
            if (b.HasValue && b.Value > 0)
            {
                if (y.HasValue && y.Value > 0)
                {
                    ViewBag.year = y.Value;
                }
                ViewBag.buildingName = bn;
                return View(b.Value);
            }
            return View();
        }

        /// <summary>
        /// 查询建筑日用电报表
        /// </summary>
        /// <param name="b">建筑ID</param>
        /// <param name="y">查看年份</param>
        /// <param name="m">查看月份</param>
        /// <param name="bn">查询建筑全称</param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult BuildingElecDay(int? b, int? y, int? m, string bn)
        {
            if (b.HasValue && b.Value > 0)
            {
                if (y.HasValue && y.Value > 0)
                {
                    ViewBag.year = y.Value;
                }
                if (m.HasValue && m.Value > 0)
                {
                    ViewBag.month = m.Value;
                }
                ViewBag.buildingName = bn;
                return View(b.Value);
            }
            return View();
        }

        /// <summary>
        /// 查询建筑能耗总表
        /// </summary>
        /// <param name="b">建筑ID</param>
        /// <param name="y">查看年份</param>
        /// <param name="bn">查询建筑全称</param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult BuildingEnergyMonth(int? b, int? y, string bn)
        {
            if (b.HasValue && b.Value > 0)
            {
                if (y.HasValue && y.Value > 0)
                {
                    ViewBag.year = y.Value;
                }
                ViewBag.buildingName = bn;
                return View(b.Value);
            }
            return View();
        }

        /// <summary>
        /// 查询校园区域月用水报表
        /// </summary>
        /// <param name="a">建筑ID</param>
        /// <param name="y">查看年份</param>
        /// <param name="bn">查询建筑全称</param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult AreaWaterMonth(int? a, int? y, string an)
        {
            if (a.HasValue && a.Value > 0)
            {
                if (y.HasValue && y.Value > 0)
                {
                    ViewBag.year = y.Value;
                }
                ViewBag.areaName = an;
                return View(a.Value);
            }
            return View();
        }

        /// <summary>
        /// 查询三级电表报表
        /// </summary>
        /// <param name="b">建筑ID</param>
        /// <param name="y">查看年份</param>    
        /// <returns></returns>
        [AuthenticationFilter]
        public  ActionResult ThirdPointMonth(int? y,int? b)
       {
           if (y.HasValue && y.Value > 0)
           {
               ViewBag.year = y.Value;               
           }
           if (b.HasValue && b.Value > 0)
           {
               return View(b.Value);
           }        
           return View();
        }
    }
}
