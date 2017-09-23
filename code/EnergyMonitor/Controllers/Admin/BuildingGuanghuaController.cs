using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EnergyMonitor.Controllers.Admin.Filters;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Implement;
using EnergyMonitor.Controllers.Utils;
using EnergyMonitor.Models.Repository.Entity;
using System.Collections;

namespace EnergyMonitor.Controllers.Admin
{
    /// <summary>
    /// 光华楼能耗数据管理
    /// </summary>
    [AdminFilter]
    public class BuildingGuanghuaController : Controller
    {
        private IPowerClassRepos _powerClassRepos = null;
        private IBuildingGuanghuaRepos _buildingGuanghuaRepos = null;
        private IBuildingRepos _buildingRepos = null;
        private IAnalogHistoryRepos _analogHistoryRepos = null;
        private IAMPRepos _ampRepos = null;

        public BuildingGuanghuaController()
            : this(new BuildingGuanghuaRepos(), new BuildingRepos(), new PowerClassRepos(), new AnalogHistoryRepos(), new AMPRepos())
        {
        }

        public BuildingGuanghuaController(IBuildingGuanghuaRepos buildingGuanghuaRepos, IBuildingRepos buildingRepos, IPowerClassRepos powerClassRepos, IAnalogHistoryRepos analogHistoryRepos, IAMPRepos ampRepos)
        {
            _buildingGuanghuaRepos = buildingGuanghuaRepos;
            _buildingRepos = buildingRepos;
            _powerClassRepos = powerClassRepos;
            _analogHistoryRepos = analogHistoryRepos;
            _ampRepos = ampRepos;
        }

        /// <summary>
        /// 报表管理
        /// </summary>
        /// <param name="b">建筑ID</param>
        /// <param name="y">查看年份</param>
        /// <param name="bn">查询对象全称</param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult ReportManagement(int? b, DateTime? startTime,DateTime? endTime, int? type,string bn)
        {
            if (b.HasValue && b.Value > 0)
            {
                if (startTime.HasValue && endTime.HasValue)
                {
                    ViewBag.startTime = startTime;
                    ViewBag.startTimeDate = startTime.Value.ToString("yyyy-MM-dd");
                    ViewBag.endTime = endTime;
                    ViewBag.endTimeDate = endTime.Value.ToString("yyyy-MM-dd");
                }                
                ViewBag.queryObjType = type;
                ViewBag.objectName = bn;
                return View(b.Value);
            }
            return View();
        }


        /// <summary>
        /// 跳转用电统计
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult EnergyStatistics()
        {
            var powerList = _powerClassRepos.GetStatisTypes();
            return View(powerList);
        }

        /// <summary>
        /// 跳转多个对比分析
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 10, VaryByParam = "none")]
        [AuthenticationFilter]
        public ActionResult HElec()
        {
            var powerList = _powerClassRepos.GetStatisTypes();
            return View(powerList);
        }

        /// <summary>
        /// 跳转多个时间对比分析
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 10, VaryByParam = "none")]
        [AuthenticationFilter]
        public ActionResult HGElec()
        {
            var powerList = _powerClassRepos.GetStatisTypes();
            return View(powerList);
        }

        /// <summary>
        /// Ajax获取用电统计数据
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="granularity"></param>
        /// <param name="powerType"></param>
        /// <param name="weekType"></param>
        /// <param name="hourType"></param>
        /// <param name="startHour"></param>
        /// <param name="endHour"></param>
        /// <returns></returns>
        public ActionResult GetBuildingGuanghuaElecAjax(int currentPage, int totalPages, int objType, int objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType, int? weekType, int? hourType, int startHour, int endHour, int selectedDay)
        {
            if (Request.IsAjaxRequest())
            {
                //endTime = endTime.AddDays(1);
                Pager pager = null;
                String[] powerTypes = null;
                if (powerType.Length == 3)
                {
                    powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
                    powerTypes = powerTypes.Union(new string[] { powerType }).ToArray();
                }
                else
                {
                    powerTypes = new string[] { powerType };
                }
                String yAxisTitle = "用电量（kWh）";
                if (powerTypes.Length != 0)
                {
                    if (powerTypes[0].Substring(0, 3) == "001")
                    {
                        yAxisTitle = "用电量（kWh）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "002")
                    {
                        yAxisTitle = "用水量（吨）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "003")
                    {
                        yAxisTitle = "燃气用量（立方米）";
                    }
                }
                DayOfWeek? dw = null;
                if (totalPages == -1)
                {
                    if (granularity == "day")//按天统计
                    {
                        int totalRows = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, 1).Count();
                        pager = new Pager(1, totalRows);
                    }
                    else if (granularity == "month")//按月统计
                    {
                        int totalRows = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisMonth(objType, objIDs, startTime, endTime, powerTypes, 1).Count();
                        pager = new Pager(1, totalRows);
                    }
                    else if (granularity == "year")//按年统计
                    {
                        int totalRows = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisYear(objType, objIDs, startTime, endTime, powerTypes, 1).Count();
                        pager = new Pager(1, totalRows);
                    }
                    else if (granularity == "week") //星期
                    {
                        if (weekType != null && weekType != 0)
                        {
                            switch (weekType.Value)
                            {
                                case 1:
                                    dw = DayOfWeek.Monday;
                                    break;
                                case 2:
                                    dw = DayOfWeek.Tuesday;
                                    break;
                                case 3:
                                    dw = DayOfWeek.Wednesday;
                                    break;
                                case 4:
                                    dw = DayOfWeek.Thursday;
                                    break;
                                case 5:
                                    dw = DayOfWeek.Friday;
                                    break;
                                case 6:
                                    dw = DayOfWeek.Saturday;
                                    break;
                                case 7:
                                    dw = DayOfWeek.Sunday;
                                    break;
                            }
                        }
                        var tempQuery = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, 1);
                        if (dw.HasValue)
                        {
                            tempQuery = tempQuery.Where(x => x.Time.DayOfWeek == dw.Value).ToList();
                        }
                        int totalRows = tempQuery.Count();
                        pager = new Pager(1, totalRows);
                    }
                    else if (granularity == "selectedday")
                    {
                        int totalRows = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, 1).Where(x => x.Time.Day == selectedDay).Count();
                        pager = new Pager(1, totalRows);
                    }
                    else //小时
                    {
                        if (hourType != null && hourType == 1 && startHour < endHour) // 指定小时段
                        {
                            int totalRows = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisSpecialHours(objType, objIDs, startTime, endTime, powerTypes, startHour, endHour, 1).Count();
                            pager = new Pager(1, totalRows);
                        }
                        else // 每小时
                        {
                            int totalRows = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisHour(objType, objIDs, startTime, endTime, powerTypes, 1).Count();
                            pager = new Pager(1, totalRows);
                        }
                    }
                }
                else
                {
                    pager = new Pager(currentPage, totalPages, false);
                }
                double average = 0;
                double max = 0;
                double min = 0;
                IList<ChartStatisEntity> list = null;
                if (pager.TotalPages > 0)
                {
                    if (granularity == "day")//按天统计
                    {
                        list = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, 1).OrderBy(x => x.Time).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    }
                    else if (granularity == "month")//按月统计
                    {
                        list = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisMonth(objType, objIDs, startTime, endTime, powerTypes, 1).OrderBy(x => x.Time).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    }
                    else if (granularity == "year")//按年统计
                    {
                        list = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisYear(objType, objIDs, startTime, endTime, powerTypes, 1).OrderBy(x => x.Time).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    }
                    else if (granularity == "week") //星期
                    {
                        if (weekType != null && weekType != 0)
                        {
                            switch (weekType.Value)
                            {
                                case 1:
                                    dw = DayOfWeek.Monday;
                                    break;
                                case 2:
                                    dw = DayOfWeek.Tuesday;
                                    break;
                                case 3:
                                    dw = DayOfWeek.Wednesday;
                                    break;
                                case 4:
                                    dw = DayOfWeek.Thursday;
                                    break;
                                case 5:
                                    dw = DayOfWeek.Friday;
                                    break;
                                case 6:
                                    dw = DayOfWeek.Saturday;
                                    break;
                                case 7:
                                    dw = DayOfWeek.Sunday;
                                    break;
                            }
                        }
                        var query = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, 1);
                        if (dw != null)
                        {
                            query = query.Where(x => x.Time.DayOfWeek == dw.Value).ToList();
                        }
                        list = query.OrderBy(x => x.Time).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    }
                    else if (granularity == "selectedday")
                    {
                        list = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, 1).Where(x => x.Time.Day == selectedDay).OrderBy(x => x.Time).ToList();
                    }
                    else //小时
                    {
                        if (hourType != null && hourType == 1 && startHour < endHour) // 指定小时段
                        {
                            list = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisSpecialHours(objType, objIDs, startTime, endTime, powerTypes, startHour, endHour, 1).OrderBy(x => x.Time).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                        }
                        else // 每小时
                        {
                            list = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisHour(objType, objIDs, startTime, endTime, powerTypes, 1).OrderBy(x => x.Time).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                        }
                    }
                    if (list != null)
                    {
                        average = list.Sum(x => x.StatisVal) / list.Count;
                        max = list.Max(x => x.StatisVal);
                        min = list.Min(x => x.StatisVal);
                    }
                }
                //String ListTimeLable = "时间";
                //String ListEnergyLable = "用电量（kWh）";

                var resultData = new
                {
                    totalPages = pager.TotalPages,
                    yAxisTitle = yAxisTitle,
                    average = average.ToString("f1"),
                    max = max.ToString("f1"),
                    min = min.ToString("f2"),
                    data = list
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// Ajax获取光华楼用电统计数据（不分页面）
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="granularity"></param>
        /// <param name="powerType"></param>
        /// <param name="weekType"></param>
        /// <param name="hourType"></param>
        /// <param name="startHour"></param>
        /// <param name="endHour"></param>
        /// <returns></returns>
        public ActionResult GetBuildingGuanghuaElecAllAjax(int objType, int objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType, int? weekType, int? hourType, int startHour, int endHour, int selectedDay)
        {
            if (Request.IsAjaxRequest())
            {
                String[] powerTypes = null;
                if (powerType.Length == 3)
                {
                    powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
                    powerTypes = powerTypes.Union(new string[] { powerType }).ToArray();
                }
                else
                {
                    powerTypes = new string[] { powerType };
                }
                String yAxisTitle = "用电量（kWh）";
                if (powerTypes.Length != 0)
                {
                    if (powerTypes[0].Substring(0, 3) == "001")
                    {
                        yAxisTitle = "用电量（kWh）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "002")
                    {
                        yAxisTitle = "用水量（吨）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "003")
                    {
                        yAxisTitle = "燃气用量（立方米）";
                    }
                }
                DayOfWeek? dw = null;
                double average = 0;
                double max = 0;
                double min = 0;
                IList<ChartStatisEntity> list = null;
                IList<IList<ChartStatisEntity>> lists = new List<IList<ChartStatisEntity>>();
                IList<int> years = new List<int>();
                IList<string> months = new List<string>();
                int resultCount = 0;
                if (granularity == "day")//按天统计
                {
                    list = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, 1).OrderBy(x => x.Time).ToList();
                }
                else if (granularity == "month")//按月统计
                {
                    list = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisMonth(objType, objIDs, startTime, endTime, powerTypes, 1).OrderBy(x => x.Time).ToList();
                }
                else if (granularity == "year")//按年统计
                {
                    list = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisYear(objType, objIDs, startTime, endTime, powerTypes, 1).OrderBy(x => x.Time).ToList();
                }
                else if (granularity == "week") //星期
                {
                    var query = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, 1);
                    if (weekType != null && weekType != 0)
                    {
                        switch (weekType.Value)
                        {
                            case 1:
                                dw = DayOfWeek.Monday;
                                break;
                            case 2:
                                dw = DayOfWeek.Tuesday;
                                break;
                            case 3:
                                dw = DayOfWeek.Wednesday;
                                break;
                            case 4:
                                dw = DayOfWeek.Thursday;
                                break;
                            case 5:
                                dw = DayOfWeek.Friday;
                                break;
                            case 6:
                                dw = DayOfWeek.Saturday;
                                break;
                            case 7:
                                dw = DayOfWeek.Sunday;
                                break;
                        }
                    }
                    if (dw != null)
                    {
                        query = query.Where(x => x.Time.DayOfWeek == dw.Value).ToList();
                    }
                    list = query.OrderBy(x => x.Time).ToList();
                }
                else if (granularity == "selectedmonth")
                { //按月同期比较
                    var query = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisMonth(objType, objIDs, startTime, endTime, powerTypes, 1);
                    resultCount = query.ToList().Count();
                    if (resultCount > 0) { min = double.MaxValue; }
                    var queryByGroup = query.OrderBy(x => x.Time).GroupBy(x => x.Time.Year);
                    IList<int> availableMonth = new List<int>();
                    for (int i = 1; i <= 12; i++)
                    {
                        Boolean allNull = true;
                        foreach (var energyYear in queryByGroup)
                        {
                            int monthItemCount = energyYear.Where(x => x.Time.Month == i).Count();
                            if (monthItemCount > 0) allNull = false;
                        }
                        if (allNull == false)
                        {
                            availableMonth.Add(i);
                        }
                    }
                    foreach (var energyYear in queryByGroup)
                    {
                        IList<ChartStatisEntity> resultList = energyYear.ToList();
                        int thisYear = resultList.FirstOrDefault().Time.Year;
                        years.Add(thisYear);
                        foreach (var month in availableMonth)
                        {
                            int count = resultList.Where(x => x.Time.Month == month).Count();
                            if (count == 0)
                            {
                                resultList.Add(new ChartStatisEntity { Time = new DateTime(thisYear, month, 1), StatisVal = 0 });
                            }
                        }
                        double tempMax = resultList.Max(x => x.StatisVal);
                        double tempMin = resultList.Min(x => x.StatisVal);
                        if (tempMax > max)
                        {
                            max = tempMax;
                        }
                        if (tempMin < min)
                        {
                            min = tempMin;
                        }
                        lists.Add(resultList.OrderBy(x => x.Time).ToList());
                    }
                }
                else if (granularity == "daytoday")
                {  //按天同期比较
                    var query = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, 1);
                    resultCount = query.ToList().Count();
                    if (resultCount > 0) { min = double.MaxValue; }
                    var queryByGroup = query.OrderBy(x => x.Time).GroupBy(x => new { x.Time.Month, x.Time.Year });
                    int groupCount = queryByGroup.Count();
                    foreach (var energyMonth in queryByGroup)
                    {
                        IList<ChartStatisEntity> resultList = energyMonth.ToList();
                        String monthStr = resultList.FirstOrDefault().Time.ToString("yyyy-MM");
                        int thisYear = resultList.FirstOrDefault().Time.Year;
                        int thisMonth = resultList.FirstOrDefault().Time.Month;
                        months.Add(monthStr);
                        for (int i = 1; i <= 31; i++)
                        {
                            int count = resultList.Where(x => x.Time.Day == i).Count();
                            if (count == 0)
                            {
                                try
                                {
                                    resultList.Add(new ChartStatisEntity { Time = new DateTime(thisYear, thisMonth, i), StatisVal = 0 });
                                }
                                catch (Exception e)
                                {

                                }
                            }
                        }
                        double tempMax = resultList.Max(x => x.StatisVal);
                        double tempMin = resultList.Min(x => x.StatisVal);
                        if (tempMax > max)
                        {
                            max = tempMax;
                        }
                        if (tempMin < min)
                        {
                            min = tempMin;
                        }
                        lists.Add(resultList.OrderBy(x => x.Time).ToList());
                    }
                }
                else if (granularity == "selectedday")
                {
                    var query = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, 1);
                    if (selectedDay != null)
                    {
                        query = query.Where(x => x.Time.Day == selectedDay).ToList();
                    }
                    list = query.OrderBy(x => x.Time).ToList();
                }
                else //小时
                {
                    if (hourType != null && hourType == 1 && startHour < endHour) // 指定小时段
                    {
                        list = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisSpecialHours(objType, objIDs, startTime, endTime, powerTypes, startHour, endHour, 1).OrderBy(x => x.Time).ToList();
                    }
                    else // 每小时
                    {
                        list = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisHour(objType, objIDs, startTime, endTime, powerTypes, 1).OrderBy(x => x.Time).ToList();
                    }
                }

                if (list != null && list.Count > 0)
                {
                    average = list.Sum(x => x.StatisVal) / list.Count;
                    max = list.Max(x => x.StatisVal);
                    min = list.Min(x => x.StatisVal);
                    var resultData = new
                    {
                        title = "能耗统计信息",
                        xAxisTitle = "时间",
                        yAxisTitle = yAxisTitle,
                        average = average.ToString("f1"),
                        max = max.ToString("f1"),
                        min = min.ToString("f1"),
                        data = list,
                        count = list.Count()
                    };
                    return Json(resultData, JsonRequestBehavior.AllowGet);
                }
                else if (list != null && list.Count == 0)
                {
                    var resultData = new
                    {
                        title = "能耗统计信息",
                        xAxisTitle = "时间",
                        yAxisTitle = yAxisTitle,
                        average = 0,
                        max = 0,
                        min = 0,
                        data = list,
                        count = 0
                    };
                    return Json(resultData, JsonRequestBehavior.AllowGet);
                }
                if (lists != null && resultCount > 0)
                {
                    var resultData2 = new
                    {
                        title = "能耗统计信息",
                        xAxisTitle = "时间",
                        yAxisTitle = yAxisTitle,
                        years = years,
                        months = months,
                        count = resultCount,
                        data = lists,
                        max = max.ToString("f1"),
                        min = min.ToString("f1")
                    };
                    return Json(resultData2, JsonRequestBehavior.AllowGet);
                }
                else if (lists != null && resultCount == 0)
                {
                    var resultData2 = new
                    {
                        title = "能耗统计信息",
                        xAxisTitle = "时间",
                        yAxisTitle = yAxisTitle,
                        years = 0,
                        months = 0,
                        count = 0,
                        data = lists,
                        max = 0,
                        min = 0
                    };
                    return Json(resultData2, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        /// <summary>
        /// 导出能耗统计Excel
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="granularity"></param>
        /// <param name="powerType"></param>
        /// <param name="weekType"></param>
        /// <param name="hourType"></param>
        /// <param name="startHour"></param>
        /// <param name="endHour"></param>
        /// <returns></returns>
        public ActionResult GetBuildingGuanghuaElecExcel(int objType, int objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType, int? weekType, int? hourType, int startHour, int endHour, int selectedDay)
        {
            if (objIDs > 0)
            {
                IList list = null;
                String[] powerTypes = null;
                if (powerType.Length == 3)
                {
                    powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
                    powerTypes = powerTypes.Union(new string[] { powerType }).ToArray();
                }
                else
                {
                    powerTypes = new string[] { powerType };
                }
                String yAxisTitle = "用电量（kWh）";
                if (powerTypes.Length != 0)
                {
                    if (powerTypes[0].Substring(0, 3) == "001")
                    {
                        yAxisTitle = "用电量（kWh）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "002")
                    {
                        yAxisTitle = "用水量（吨）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "003")
                    {
                        yAxisTitle = "燃气用量（立方米）";
                    }
                }
                if (granularity == "day")//按天统计
                {
                    list = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, 1).OrderBy(x => x.Time).ToList();
                }
                else if (granularity == "month")//按月统计
                {
                    list = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisMonth(objType, objIDs, startTime, endTime, powerTypes, 1).OrderBy(x => x.Time).ToList();
                }
                else if (granularity == "year")//按年统计
                {
                    list = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisYear(objType, objIDs, startTime, endTime, powerTypes, 1).OrderBy(x => x.Time).ToList();
                }
                else if (granularity == "week") //星期
                {
                    DayOfWeek? dw = null;
                    if (weekType != null && weekType != 0)
                    {
                        switch (weekType.Value)
                        {
                            case 1:
                                dw = DayOfWeek.Monday;
                                break;
                            case 2:
                                dw = DayOfWeek.Tuesday;
                                break;
                            case 3:
                                dw = DayOfWeek.Wednesday;
                                break;
                            case 4:
                                dw = DayOfWeek.Thursday;
                                break;
                            case 5:
                                dw = DayOfWeek.Friday;
                                break;
                            case 6:
                                dw = DayOfWeek.Saturday;
                                break;
                            case 7:
                                dw = DayOfWeek.Sunday;
                                break;
                        }
                    }
                    var query = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, 1);
                    if (dw != null)
                    {
                        query = query.Where(x => x.Time.DayOfWeek == dw.Value).ToList();
                    }
                    list = query.OrderBy(x => x.Time).ToList();
                }
                else if (granularity == "selectedday")
                {
                    var query = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, 1);
                    if (selectedDay != null)
                    {
                        query = query.Where(x => x.Time.Day == selectedDay).ToList();
                    }
                    list = query.OrderBy(x => x.Time).ToList();
                }
                else //小时
                {
                    if (hourType != null && hourType == 1 && startHour < endHour) // 指定小时段
                    {
                        list = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisSpecialHours(objType, objIDs, startTime, endTime, powerTypes, startHour, endHour, 1).OrderBy(x => x.Time).ToList();
                    }
                    else // 每小时
                    {
                        list = _analogHistoryRepos.GetBuildingGuanghuaEnergyStatisHour(objType, objIDs, startTime, endTime, powerTypes, 1).OrderBy(x => x.Time).ToList();
                    }
                }
                if (list != null)
                {
                    string title = "能耗统计(" + startTime.ToString("yyyy-MM-dd") + "_" + endTime.ToString("yyyy-MM-dd") + ")";
                    if (granularity == "hour" && hourType != null && hourType == 1 && startHour < endHour)
                    {
                        title += "/(" + startHour + ":00-" + endHour + ":00)";
                    }
                    if (granularity != "week")
                    {
                        string[] headers = { "时间", yAxisTitle };
                        string[] properties = { "TimeBlock", "StatisSVal" };
                        return this.Excel(list, title + ".xls", title, headers, properties);
                    }
                    else
                    {
                        string[] headers = { "时间", "星期", yAxisTitle };
                        string[] properties = { "TimeBlock", "weekDay", "StatisSVal" };
                        return this.Excel(list, title + ".xls", title, headers, properties);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Ajax获取多个时间对比分析一览表查询数据
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="granularity"></param>
        /// <param name="powerType"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public ActionResult GetHGElecListAjax(int currentPage, int totalPages, int objType, string objIDs, DateTime startTime, DateTime endTime, string granularity, string assignMonth, string assignDay, string powerType, double sum)
        {
            if (Request.IsAjaxRequest())
            {
                Pager pager = null;
                String[] powerTypes = null;
                if (powerType.Length == 3)
                {
                    powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
                    powerTypes = powerTypes.Union(new string[] { powerType }).ToArray();
                }
                else
                {
                    powerTypes = new string[] { powerType };
                }
                String yAxisTitle = "用电量（kWh）";
                if (powerTypes.Length != 0)
                {
                    if (powerTypes[0].Substring(0, 3) == "001")
                    {
                        yAxisTitle = "用电量（kWh）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "002")
                    {
                        yAxisTitle = "用水量（吨）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "003")
                    {
                        yAxisTitle = "燃气用量（立方米）";
                    }
                }
                string[] idStrs = objIDs.Split(new char[] { '_' });
                int?[] ids = new int?[idStrs.Length];
                for (int i = 0; i < idStrs.Length; i++)
                {
                    int temp = -1;
                    if (Int32.TryParse(idStrs[i], out temp))
                    {
                        ids[i] = temp;
                    }
                }
                if (totalPages == -1)
                {
                    int totalRows = 0;
                    if (granularity == "day")//按天
                    {
                        totalRows = _analogHistoryRepos.GetBuildingGuanghuaDayEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, 1).Count();
                    }
                    else if (granularity == "month")//按月
                    {
                        totalRows = _analogHistoryRepos.GetBuildingGuanghuaMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, 1).Count();
                    }
                    else if (granularity == "specificMonth")//按指定月份
                    {
                        totalRows = _analogHistoryRepos.GetBuildingGuanghuaAssignMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, assignMonth, powerTypes, 1).Count();
                    }
                    else//按指定日期
                        totalRows = _analogHistoryRepos.GetBuildingGuanghuaAssignDayEnergyByIDsGranularity(objType, ids, startTime, endTime, assignDay, powerTypes, 1).Count();
                    pager = new Pager(1, totalRows);
                }
                else
                {
                    pager = new Pager(currentPage, totalPages, false);
                }
                IList list = null;
                if (pager.TotalPages > 0)
                {

                    if (granularity == "day")//按天
                    {
                        if (sum < 0)
                        {
                            //sum = _analogHistoryRepos.GetBuildingGuanghuaEnergySum(objType, ids, startTime, endTime, powerTypes);
                            sum = _analogHistoryRepos.GetBuildingGuanghuaDayEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, 1).Sum(x => x.StatisVal);
                        }
                        list = _analogHistoryRepos.GetBuildingGuanghuaDayEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, sum).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    }
                    else if (granularity == "month")//按月
                    {
                        if (sum < 0)
                        {
                            //sum = _analogHistoryRepos.GetBuildingGuanghuaEnergySum(objType, ids, startTime, endTime, powerTypes);
                            sum = _analogHistoryRepos.GetBuildingGuanghuaMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, 1).Sum(x => x.StatisVal);
                        }
                        list = _analogHistoryRepos.GetBuildingGuanghuaMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, sum).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    }
                    else if (granularity == "specificMonth")//按指定月份
                    {
                        if (sum < 0)
                        {
                            //sum = _analogHistoryRepos.GetBuildingGuanghuaEnergySum(objType, ids, startTime, endTime, powerTypes);
                            sum = _analogHistoryRepos.GetBuildingGuanghuaAssignMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, assignMonth, powerTypes, 1).Sum(x => x.StatisVal);
                        }
                        list = _analogHistoryRepos.GetBuildingGuanghuaAssignMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, assignMonth, powerTypes, sum).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    }
                    else//按指定日期
                    {
                        if (sum < 0)
                        {
                            //sum = _analogHistoryRepos.GetBuildingGuanghuaEnergySum(objType, ids, startTime, endTime, powerTypes);
                            sum = _analogHistoryRepos.GetBuildingGuanghuaAssignDayEnergyByIDsGranularity(objType, ids, startTime, endTime, assignDay, powerTypes, 1).Sum(x => x.StatisVal);
                        }
                        list = _analogHistoryRepos.GetBuildingGuanghuaAssignDayEnergyByIDsGranularity(objType, ids, startTime, endTime, assignDay, powerTypes, sum).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    }
                }
                var resultData = new
                {
                    totalPages = pager.TotalPages,
                    yAxisTitle = yAxisTitle,
                    totalSum = sum,
                    data = list
                };

                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// Ajax获取多个时间对比分析一览表查询数据(不分页)
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="granularity"></param>
        /// <param name="powerType"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public ActionResult GetHGElecListAjaxNoPage(int objType, string objIDs, DateTime startTime, DateTime endTime, string granularity, string assignMonth, string assignDay, string powerType, double sum)
        {
            if (Request.IsAjaxRequest())
            {
                String[] powerTypes = null;
                if (powerType.Length == 3)
                {
                    powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
                    powerTypes = powerTypes.Union(new string[] { powerType }).ToArray();
                }
                else
                {
                    powerTypes = new string[] { powerType };
                }
                String yAxisTitle = "用电量（kWh）";
                if (powerTypes.Length != 0)
                {
                    if (powerTypes[0].Substring(0, 3) == "001")
                    {
                        yAxisTitle = "用电量（kWh）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "002")
                    {
                        yAxisTitle = "用水量（吨）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "003")
                    {
                        yAxisTitle = "燃气用量（立方米）";
                    }
                }
                string[] idStrs = objIDs.Split(new char[] { '_' });
                int?[] ids = new int?[idStrs.Length];
                for (int i = 0; i < idStrs.Length; i++)
                {
                    int temp = -1;
                    if (Int32.TryParse(idStrs[i], out temp))
                    {
                        ids[i] = temp;
                    }
                }
                int totalRows = 0;
                if (ids.Length > 0)
                {
                    if (granularity == "day")//按天
                    {
                        totalRows = _analogHistoryRepos.GetBuildingGuanghuaDayEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, 1).Count();
                    }
                    else if (granularity == "month")//按月
                    {
                        totalRows = _analogHistoryRepos.GetBuildingGuanghuaMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, 1).Count();
                    }
                    else if (granularity == "specificMonth")//按指定月份
                    {
                        totalRows = _analogHistoryRepos.GetBuildingGuanghuaAssignMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, assignMonth, powerTypes, 1).Count();
                    }
                    else//按指定日期
                        totalRows = _analogHistoryRepos.GetBuildingGuanghuaAssignDayEnergyByIDsGranularity(objType, ids, startTime, endTime, assignDay, powerTypes, 1).Count();
                }
                IList list = null;
                if (ids.Length > 0)
                {
                    if (granularity == "day")//按天
                    {
                        if (sum < 0)
                        {
                            //sum = _analogHistoryRepos.GetBuildingGuanghuaEnergySum(objType, ids, startTime, endTime, powerTypes);
                            sum = _analogHistoryRepos.GetBuildingGuanghuaDayEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, 1).Sum(x => x.StatisVal);
                        }
                        list = _analogHistoryRepos.GetBuildingGuanghuaDayEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, sum).ToList();
                    }
                    else if (granularity == "month")//按月
                    {
                        if (sum < 0)
                        {
                            //sum = _analogHistoryRepos.GetBuildingGuanghuaEnergySum(objType, ids, startTime, endTime, powerTypes);
                            sum = _analogHistoryRepos.GetBuildingGuanghuaMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, 1).Sum(x => x.StatisVal);
                        }
                        list = _analogHistoryRepos.GetBuildingGuanghuaMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, sum).ToList();
                    }
                    else if (granularity == "specificMonth")//按指定月份
                    {
                        if (sum < 0)
                        {
                            //sum = _analogHistoryRepos.GetBuildingGuanghuaEnergySum(objType, ids, startTime, endTime, powerTypes);
                            sum = _analogHistoryRepos.GetBuildingGuanghuaAssignMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, assignMonth, powerTypes, 1).Sum(x => x.StatisVal);
                        }
                        list = _analogHistoryRepos.GetBuildingGuanghuaAssignMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, assignMonth, powerTypes, sum).ToList();
                    }
                    else//按指定日期
                    {
                        if (sum < 0)
                        {
                            //sum = _analogHistoryRepos.GetBuildingGuanghuaEnergySum(objType, ids, startTime, endTime, powerTypes);
                            sum = _analogHistoryRepos.GetBuildingGuanghuaAssignDayEnergyByIDsGranularity(objType, ids, startTime, endTime, assignDay, powerTypes, 1).Sum(x => x.StatisVal);
                        }
                        list = _analogHistoryRepos.GetBuildingGuanghuaAssignDayEnergyByIDsGranularity(objType, ids, startTime, endTime, assignDay, powerTypes, sum).ToList();
                    }
                }
                var resultData = new
                {
                    count = totalRows,
                    yAxisTitle = yAxisTitle,
                    totalSum = sum,
                    data = list
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// Ajax获取多个时间对比分析图表查询数据
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="queryEndTime"></param>
        /// <param name="granularity"></param>
        /// <param name="powerType"></param>
        /// <returns></returns>
        public ActionResult GetHGElecChartAjax(int currentPage, int totalPages, int objType, string objIDs, DateTime startTime, DateTime endTime, string granularity, string assignMonth, string assignDay, string powerType)
        {
            if (Request.IsAjaxRequest())
            {
                Pager pager = null;
                String[] powerTypes = null;
                if (powerType.Length == 3)
                {
                    powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
                    powerTypes = powerTypes.Union(new string[] { powerType }).ToArray();
                }
                else
                {
                    powerTypes = new string[] { powerType };
                }
                String yAxisTitle = "用电量（kWh）";
                if (powerTypes.Length != 0)
                {
                    if (powerTypes[0].Substring(0, 3) == "001")
                    {
                        yAxisTitle = "用电量（kWh）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "002")
                    {
                        yAxisTitle = "用水量（吨）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "003")
                    {
                        yAxisTitle = "燃气用量（立方米）";
                    }
                }
                string[] idStrs = objIDs.Split(new char[] { '_' });
                int?[] ids = new int?[idStrs.Length];
                for (int i = 0; i < idStrs.Length; i++)
                {
                    int temp = -1;
                    if (Int32.TryParse(idStrs[i], out temp))
                    {
                        ids[i] = temp;
                    }
                }
                if (totalPages == -1)
                {
                    int totalRows = 0;
                    if (granularity == "day")//按天
                    {
                        totalRows = _analogHistoryRepos.GetBuildingGuanghuaDateByDay(objType, ids, startTime, endTime, powerTypes).Count();
                    }
                    else if (granularity == "month")//按月
                    {
                        totalRows = _analogHistoryRepos.GetBuildingGuanghuaDateByMonth(objType, ids, startTime, endTime, powerTypes).Count();
                    }
                    else if (granularity == "specificMonth")//按指定月份
                    {
                        totalRows = _analogHistoryRepos.GetBuildingGuanghuaDateByAssignMonth(objType, ids, startTime, endTime, assignMonth, powerTypes).Count();
                    }
                    else//按指定日期
                        totalRows = _analogHistoryRepos.GetBuildingGuanghuaDateByAssignDay(objType, ids, startTime, endTime, assignDay, powerTypes).Count();
                    pager = new Pager(1, totalRows);
                }
                else
                {
                    pager = new Pager(currentPage, totalPages, false);
                }
                IList list = new ArrayList();
                IList indexValList = null;
                if (pager.TotalPages > 0)
                {
                    IList<ChartStatisEntity> dataList = null;
                    if (granularity == "day")//按天
                    {
                        var timeList = _analogHistoryRepos.GetBuildingGuanghuaDateByDay(objType, ids, startTime, endTime, powerTypes).OrderBy(x => x).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetBuildingGuanghuaDayEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, 1).ToList();
                    }
                    else if (granularity == "month")//按月
                    {
                        var timeList = _analogHistoryRepos.GetBuildingGuanghuaDateByMonth(objType, ids, startTime, endTime, powerTypes).OrderBy(x => x).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetBuildingGuanghuaMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, 1).ToList();
                    }
                    else if (granularity == "specificMonth")//按指定月份
                    {
                        var timeList = _analogHistoryRepos.GetBuildingGuanghuaDateByAssignMonth(objType, ids, startTime, endTime, assignMonth, powerTypes).OrderBy(x => x).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetBuildingGuanghuaAssignMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, assignMonth, powerTypes, 1).ToList();
                    }
                    else//按指定日期
                    {
                        var timeList = _analogHistoryRepos.GetBuildingGuanghuaDateByAssignDay(objType, ids, startTime, endTime, assignDay, powerTypes).OrderBy(x => x).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetBuildingGuanghuaAssignDayEnergyByIDsGranularity(objType, ids, startTime, endTime, assignDay, powerTypes, 1).ToList();
                    }
                    var queryNameList = dataList.Select(x => x.Name).Distinct();
                    foreach (var item in queryNameList)
                    {
                        var dataItem = new
                        {
                            queryName = item,
                            dataList = dataList.Where(x => x.Name == item).OrderBy(x => x.Time).ToList()
                        };
                        list.Add(dataItem);
                    }
                    indexValList = dataList.Select(x => x.TimeBlock).Distinct().OrderBy(x => x).ToList();
                }
                var resultData = new
                {
                    totalPages = pager.TotalPages,
                    yAxisTitle = yAxisTitle,
                    indexValList = indexValList,
                    data = list
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// Ajax获取多个时间对比分析图表查询数据(不分页)
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="queryEndTime"></param>
        /// <param name="granularity"></param>
        /// <param name="powerType"></param>
        /// <returns></returns>
        public ActionResult GetHGElecChartAjaxNoPage(int objType, string objIDs, DateTime startTime, DateTime endTime, string granularity, string assignMonth, string assignDay, string powerType)
        {
            if (Request.IsAjaxRequest())
            {
                String[] powerTypes = null;
                if (powerType.Length == 3)
                {
                    powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
                    powerTypes = powerTypes.Union(new string[] { powerType }).ToArray();
                }
                else
                {
                    powerTypes = new string[] { powerType };
                }
                String yAxisTitle = "用电量（kWh）";
                if (powerTypes.Length != 0)
                {
                    if (powerTypes[0].Substring(0, 3) == "001")
                    {
                        yAxisTitle = "用电量（kWh）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "002")
                    {
                        yAxisTitle = "用水量（吨）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "003")
                    {
                        yAxisTitle = "燃气用量（立方米）";
                    }
                }
                string[] idStrs = objIDs.Split(new char[] { '_' });
                int?[] ids = new int?[idStrs.Length];
                for (int i = 0; i < idStrs.Length; i++)
                {
                    int temp = -1;
                    if (Int32.TryParse(idStrs[i], out temp))
                    {
                        ids[i] = temp;
                    }
                }
                int totalRows = 0;
                if (granularity == "day")//按天
                {
                    totalRows = _analogHistoryRepos.GetBuildingGuanghuaDateByDay(objType, ids, startTime, endTime, powerTypes).Count();
                }
                else if (granularity == "month")//按月
                {
                    totalRows = _analogHistoryRepos.GetBuildingGuanghuaDateByMonth(objType, ids, startTime, endTime, powerTypes).Count();
                }
                else if (granularity == "specificMonth")//按指定月份
                {
                    totalRows = _analogHistoryRepos.GetBuildingGuanghuaDateByAssignMonth(objType, ids, startTime, endTime, assignMonth, powerTypes).Count();
                }
                else//按指定日期
                    totalRows = _analogHistoryRepos.GetBuildingGuanghuaDateByAssignDay(objType, ids, startTime, endTime, assignDay, powerTypes).Count();
                IList list = new ArrayList();
                IList indexValList = null;
                if (ids.Length > 0)
                {
                    IList<ChartStatisEntity> dataList = null;
                    if (granularity == "day")//按天
                    {
                        var timeList = _analogHistoryRepos.GetBuildingGuanghuaDateByDay(objType, ids, startTime, endTime, powerTypes).OrderBy(x => x).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetBuildingGuanghuaDayEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, 1).ToList();
                    }
                    else if (granularity == "month")//按月
                    {
                        var timeList = _analogHistoryRepos.GetBuildingGuanghuaDateByMonth(objType, ids, startTime, endTime, powerTypes).OrderBy(x => x).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetBuildingGuanghuaMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, 1).ToList();
                    }
                    else if (granularity == "specificMonth")//按指定月份
                    {
                        var timeList = _analogHistoryRepos.GetBuildingGuanghuaDateByAssignMonth(objType, ids, startTime, endTime, assignMonth, powerTypes).OrderBy(x => x).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetBuildingGuanghuaAssignMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, assignMonth, powerTypes, 1).ToList();
                    }
                    else//按指定日期
                    {
                        var timeList = _analogHistoryRepos.GetBuildingGuanghuaDateByAssignDay(objType, ids, startTime, endTime, assignDay, powerTypes).OrderBy(x => x).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetBuildingGuanghuaAssignDayEnergyByIDsGranularity(objType, ids, startTime, endTime, assignDay, powerTypes, 1).ToList();
                    }
                    var queryNameList = dataList.Select(x => x.Name).Distinct();
                    foreach (var item in queryNameList)
                    {
                        var dataItem = new
                        {
                            queryName = item,
                            dataList = dataList.Where(x => x.Name == item).OrderBy(x => x.Time).ToList()
                        };
                        list.Add(dataItem);
                    }
                    indexValList = dataList.Select(x => x.TimeBlock).Distinct().OrderBy(x => x).ToList();
                }
                var resultData = new
                {
                    count = totalRows,
                    yAxisTitle = yAxisTitle,
                    indexValList = indexValList,
                    data = list
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// Ajax获取多个时间对比分析结果
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="granularity"></param>
        /// <param name="powerType"></param>
        /// <returns></returns>
        public ActionResult GetHGElecAnalysisAjax(int objType, string objIDs, DateTime startTime, DateTime endTime, string granularity, string assignMonth, string assignDay, string powerType)
        {
            if (Request.IsAjaxRequest())
            {
                String[] powerTypes = null;
                if (powerType.Length == 3)
                {
                    powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
                    powerTypes = powerTypes.Union(new string[] { powerType }).ToArray();
                }
                else
                {
                    powerTypes = new string[] { powerType };
                }
                string[] idStrs = objIDs.Split(new char[] { '_' });
                int?[] ids = new int?[idStrs.Length];
                for (int i = 0; i < idStrs.Length; i++)
                {
                    int temp = -1;
                    if (Int32.TryParse(idStrs[i], out temp))
                    {
                        ids[i] = temp;
                    }
                }
                List<ChartStatisEntity> list = null;
                if (granularity == "day")
                {
                    list = _analogHistoryRepos.GetBuildingGuanghuaDayEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, 1).ToList();
                }
                else if (granularity == "month")
                {
                    list = _analogHistoryRepos.GetBuildingGuanghuaMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, 1).ToList();
                }
                else if (granularity == "specificMonth")//按指定月份
                {
                    list = _analogHistoryRepos.GetBuildingGuanghuaAssignMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, assignMonth, powerTypes, 1).ToList();
                }
                else
                    list = _analogHistoryRepos.GetBuildingGuanghuaAssignDayEnergyByIDsGranularity(objType, ids, startTime, endTime, assignDay, powerTypes, 1).ToList();
                int totalRows = list.Count;
                if (totalRows > 0)
                {
                    double totalSum = list.Sum(x => x.StatisVal);
                    var queryNameList = list.Select(x => x.Name).Distinct();
                    IList queryList = new ArrayList();
                    foreach (var item in queryNameList)
                    {
                        var querySum = list.Where(x => x.Name == item).Sum(x => x.StatisVal);
                        var dataItem = new
                        {
                            queryName = item,
                            querySum = querySum.ToString("f1"),
                            queryPercentage = (querySum * 100 / totalSum).ToString("f1") + "%"
                        };
                        queryList.Add(dataItem);
                    }
                    var maxObj = list.OrderByDescending(x => x.StatisVal).FirstOrDefault();
                    maxObj.Sum = totalSum;
                    var minObj = list.OrderBy(x => x.StatisVal).FirstOrDefault();
                    minObj.Sum = totalSum;
                    var average = totalSum / totalRows;
                    var greaterEqualAverageCount = list.Where(x => x.StatisVal >= average).Count();
                    var smallerAverageCount = list.Where(x => x.StatisVal < average).Count();

                    var resultData = new
                    {
                        totalRows = totalRows,
                        totalSum = totalSum.ToString("f1"),
                        queryNameList = queryNameList,
                        queryList = queryList,
                        average = average.ToString("f1"),
                        greaterEqualAverageCount = greaterEqualAverageCount,
                        smallerAverageCount = smallerAverageCount,
                        maxObj = maxObj,
                        minObj = minObj
                    };
                    return Json(resultData, JsonRequestBehavior.AllowGet);
                }
                return Json(new { totalRows = totalRows }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 导出多个时间对比分析Excel
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="granularity"></param>
        /// <param name="powerType"></param>
        /// <returns></returns>
        public ActionResult GetHGElecExcel(int objType, string objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType, string attachName)
        {
            List<ChartStatisEntity> list = null;
            String[] powerTypes = null;
            if (powerType.Length == 3)
            {
                powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
                powerTypes = powerTypes.Union(new string[] { powerType }).ToArray();
            }
            else
            {
                powerTypes = new string[] { powerType };
            }
            string[] idStrs = objIDs.Split(new char[] { '_' });
            int?[] ids = new int?[idStrs.Length];
            for (int i = 0; i < idStrs.Length; i++)
            {
                int temp = -1;
                if (Int32.TryParse(idStrs[i], out temp))
                {
                    ids[i] = temp;
                }
            }

            if (granularity == "day")//按天
            {
                double sum = _analogHistoryRepos.GetBuildingGuanghuaEnergySum(objType, ids, startTime, endTime, powerTypes);
                list = _analogHistoryRepos.GetBuildingGuanghuaDayEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, sum).ToList();
            }
            else//按月
            {
                double sum = _analogHistoryRepos.GetBuildingGuanghuaEnergySum(objType, ids, startTime, endTime, powerTypes);
                list = _analogHistoryRepos.GetBuildingGuanghuaMonthEnergyByIDsGranularity(objType, ids, startTime, endTime, powerTypes, sum).ToList();
            }
            if (list != null)
            {
                foreach (var item in list)
                {
                    item.Name = attachName + item.Name;
                }
                string title = "能耗多个时间对比分析(" + startTime.ToString("yyyy-MM-dd") + "_" + endTime.ToString("yyyy-MM-dd") + ")";
                string[] headers = { "日期", "查询对象", "用能值", "所占比例" };
                string[] properties = { "TimeBlock", "Name", "StatisSVal", "SPercentage" };
                return this.Excel(list, "能耗多个时间对比分析.xls", title, headers, properties);
            }
            return null;
        }

        /// <summary>
        /// Ajax获取多个对比分析查询数据
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerType"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public ActionResult GetHElecAjax(int currentPage, int totalPages, int objType, string objIDs, DateTime startTime, DateTime endTime, string powerType, double sum)
        {
            if (Request.IsAjaxRequest())
            {
                //string[] powerTypes = null;
                //if (!string.IsNullOrWhiteSpace(powerType))
                //{
                //    powerTypes = powerType.Split(new char[] { '_' });
                //}
                String[] powerTypes = null;
                if (powerType.Length == 3)
                {
                    powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
                    powerTypes = powerTypes.Union(new string[] { powerType }).ToArray();
                }
                else
                {
                    powerTypes = new string[] { powerType };
                }
                String yAxisTitle = "用电量（kWh）";
                if (powerTypes.Length != 0)
                {
                    if (powerTypes[0].Substring(0, 3) == "001")
                    {
                        yAxisTitle = "用电量（kWh）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "002")
                    {
                        yAxisTitle = "用水量（吨）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "003")
                    {
                        yAxisTitle = "燃气用量（立方米）";
                    }
                }
                string[] idStrs = objIDs.Split(new char[] { '_' });
                int?[] ids = new int?[idStrs.Length];
                for (int i = 0; i < idStrs.Length; i++)
                {
                    int temp = -1;
                    if (Int32.TryParse(idStrs[i], out temp))
                    {
                        ids[i] = temp;
                    }
                }
                IList list = _analogHistoryRepos.GetBuildingGuanghuaEnergy(objType, ids, powerTypes, startTime, endTime, sum).ToList();

                var resultData = new
                {
                    totalPages = -1,
                    yAxisTitle = yAxisTitle,
                    totalSum = sum,
                    data = list
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// Ajax获取多个对比分析
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerType"></param>
        /// <returns></returns>
        public ActionResult GetHElecAnalysisAjax(int objType, string objIDs, DateTime startTime, DateTime endTime, string powerType)
        {
            if (Request.IsAjaxRequest())
            {
                String[] powerTypes = null;
                if (powerType.Length == 3)
                {
                    powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
                    powerTypes = powerTypes.Union(new string[] { powerType }).ToArray();
                }
                else
                {
                    powerTypes = new string[] { powerType };
                }
                string[] idStrs = objIDs.Split(new char[] { '_' });
                int?[] ids = new int?[idStrs.Length];
                for (int i = 0; i < idStrs.Length; i++)
                {
                    int temp = -1;
                    if (Int32.TryParse(idStrs[i], out temp))
                    {
                        ids[i] = temp;
                    }
                }
                List<ChartStatisEntity> list = _analogHistoryRepos.GetBuildingGuanghuaEnergy(objType, ids, powerTypes, startTime, endTime, 1).ToList();
                int totalRows = list.Count;
                if (totalRows > 0)
                {
                    double sum = list.Sum(x => x.StatisVal);
                    var maxObj = list.OrderByDescending(x => x.StatisVal).FirstOrDefault();
                    maxObj.Sum = sum;
                    var minObj = list.OrderBy(x => x.StatisVal).FirstOrDefault();
                    minObj.Sum = sum;
                    var average = sum / totalRows;
                    var greaterEqualAverageCount = list.Where(x => x.StatisVal >= average).Count();
                    var smallerAverageCount = list.Where(x => x.StatisVal < average).Count();

                    var resultData = new
                    {
                        totalRows = totalRows,
                        totalSum = sum.ToString("f1"),
                        average = average.ToString("f1"),
                        greaterEqualAverageCount = greaterEqualAverageCount,
                        smallerAverageCount = smallerAverageCount,
                        maxObj = maxObj,
                        minObj = minObj
                    };
                    return Json(resultData, JsonRequestBehavior.AllowGet);
                }
                return Json(new { totalRows = totalRows }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 导出多个对比分析查询结果
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="granularity"></param>
        /// <param name="powerType"></param>
        /// <returns></returns>
        public ActionResult GetHElecExcel(int objType, string objIDs, DateTime startTime, DateTime endTime, string powerType, string attachName)
        {
            string[] powerTypes = null;
            if (!string.IsNullOrWhiteSpace(powerType))
            {
                powerTypes = powerType.Split(new char[] { '_' });
            }
            string[] idStrs = objIDs.Split(new char[] { '_' });
            int?[] ids = new int?[idStrs.Length];
            for (int i = 0; i < idStrs.Length; i++)
            {
                int temp = -1;
                if (Int32.TryParse(idStrs[i], out temp))
                {
                    ids[i] = temp;
                }
            }
            double sum = _analogHistoryRepos.GetBuildingGuanghuaAllEnergyByIDs(objType, ids, startTime, endTime, powerTypes);
            var list = _analogHistoryRepos.GetBuildingGuanghuaEnergy(objType, ids, powerTypes, startTime, endTime, sum).ToList();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    item.Name = attachName + item.Name;
                }
                string title = "能耗多个对比分析(" + startTime.ToString("yyyy-MM-dd") + "_" + endTime.ToString("yyyy-MM-dd") + ")";
                string[] headers = { "查询对象", "用能值", "所占比例" };
                string[] properties = { "Name", "StatisSVal", "SPercentage" };
                return this.Excel(list, "能耗多个对比分析.xls", title, headers, properties);
            }
            return null;
        }

        /// <summary>
        /// 获取光华楼关联测点的所有能耗类型
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="ObjType"></param>
        /// <returns></returns>
        public ActionResult BuildingGuanghuaPowerTypesOfObj(String objID, int ObjType)
        {
            if (Request.IsAjaxRequest())
            {
                var powerTypes = _ampRepos.GetBuildingGuanghuaPowerTypesOfObj(objID, ObjType).OrderBy(x => x.PowerTypeID).ToList();
                return Json(powerTypes, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
    }
}
