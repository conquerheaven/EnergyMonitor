using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Collections;
using EnergyMonitor.Controllers.Utils;
using EnergyMonitor.Controllers.Admin.Filters;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Implement;
using System.Linq;
using EnergyMonitor.Models.Repository.Entity;

namespace EnergyMonitor.Controllers.Admin
{
    /// <summary>
    /// 能耗统计
    /// </summary>
    [AdminFilter]
    public class StatisticsController : Controller
    {
        private IPowerClassRepos _powerClassRepos = null;
        private IAnalogHistoryRepos _analogHistoryRepos = null;
        private ISchoolRepos _schoolRepos = null;
        private IBuildingTypesRepos _buildingTypesRepos = null;

        public StatisticsController()
            : this(new PowerClassRepos(), new AnalogHistoryRepos(), new SchoolRepos(), new BuildingTypesRepos())
        {
        }

        public StatisticsController(IPowerClassRepos powerClassRepos, IAnalogHistoryRepos analogHistoryRepos, ISchoolRepos schoolRepos, IBuildingTypesRepos buildingTypesRepos)
        {
            _powerClassRepos = powerClassRepos;
            _analogHistoryRepos = analogHistoryRepos;
            _schoolRepos = schoolRepos;
            _buildingTypesRepos = buildingTypesRepos;
        }

        /// <summary>
        /// 跳转用电统计
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult Elec()
        {
            var powerList = _powerClassRepos.GetStatisTypes();
            return View(powerList);
        }

        /// <summary>
        /// 跳转按建筑类型统计
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult BuildingTypeStatistics()
        {
            DateTime date = DateTime.Now;
            ViewBag.currentYear = date.Year;
            return View();
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
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetElecAjax(int currentPage, int totalPages, int objType, int objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType, int? weekType, int? hourType, int startHour, int endHour, int selectedDay, string statisticMode)
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
                        int totalRows = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1,statisticMode).Count();
                        pager = new Pager(1, totalRows);
                    }
                    else if (granularity == "month")//按月统计
                    {
                        int totalRows = _analogHistoryRepos.GetEnergyStatisMonthForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1,statisticMode).Count();
                        pager = new Pager(1, totalRows);
                    }
                    else if (granularity == "year")//按年统计
                    {
                        int totalRows = _analogHistoryRepos.GetEnergyStatisYearForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1,statisticMode).Count();
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
                        var tempQuery = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode);
                        if (dw.HasValue)
                        {
                            tempQuery = tempQuery.Where(x => x.Time.DayOfWeek == dw.Value).ToList();
                        }
                        int totalRows = tempQuery.Count();
                        pager = new Pager(1, totalRows);
                    }
                    else if (granularity == "selectedday")
                    {
                        int totalRows = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).Where(x => x.Time.Day == selectedDay).Count();
                        pager = new Pager(1, totalRows);
                    }
                    else //小时
                    {
                        if (hourType != null && hourType == 1 && startHour < endHour) // 指定小时段
                        {
                            int totalRows = _analogHistoryRepos.GetEnergyStatisSpecialHoursForStatistic(objType, objIDs, startTime, endTime, powerTypes, startHour, endHour, 1,statisticMode).Count();
                            pager = new Pager(1, totalRows);
                        }
                        else // 每小时
                        {
                            int totalRows = _analogHistoryRepos.GetEnergyStatisHourForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1,statisticMode).Count();
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
                        list = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).OrderBy(x => x.Time).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    }
                    else if (granularity == "month")//按月统计
                    {
                        list = _analogHistoryRepos.GetEnergyStatisMonthForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1,statisticMode).OrderBy(x => x.Time).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    }
                    else if (granularity == "year")//按年统计
                    {
                        list = _analogHistoryRepos.GetEnergyStatisYearForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1,statisticMode).OrderBy(x => x.Time).Skip(pager.StartRow).Take(pager.PageSize).ToList();
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
                        var query = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode);
                        if (dw != null)
                        {
                            query = query.Where(x => x.Time.DayOfWeek == dw.Value).ToList();
                        }
                        list = query.OrderBy(x => x.Time).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    }
                    else if (granularity == "selectedday")
                    {
                        list = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).Where(x => x.Time.Day == selectedDay).OrderBy(x => x.Time).ToList();
                    }
                    else //小时
                    {
                        if (hourType != null && hourType == 1 && startHour < endHour) // 指定小时段
                        {
                            list = _analogHistoryRepos.GetEnergyStatisSpecialHoursForStatistic(objType, objIDs, startTime, endTime, powerTypes, startHour, endHour, 1,statisticMode).OrderBy(x => x.Time).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                        }
                        else // 每小时
                        {
                            list = _analogHistoryRepos.GetEnergyStatisHourForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1,statisticMode).OrderBy(x => x.Time).Skip(pager.StartRow).Take(pager.PageSize).ToList();
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
        /// 获取用电统计数据提供给手机端使用
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
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetElecAjaxForMobile(int currentPage, int totalPages, int objIDs, DateTime startTime, DateTime endTime, string granularity)
        {
            //endTime = endTime.AddDays(1);
            // 因为手机端是做简单的房间用电量查询，只有三种方式（按年、按月和按天），所以把有些变量写死
            int objType = 5;
            string powerType = "001";
            int? weekType = 0;
            int? hourType = 0;
            int startHour = 0;
            int endHour = 1;
            int selectedDay = 1;
            string statisticMode = "totalEnergy";

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
                    int totalRows = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).Count();
                    pager = new Pager(1, totalRows);
                }
                else if (granularity == "month")//按月统计
                {
                    int totalRows = _analogHistoryRepos.GetEnergyStatisMonthForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).Count();
                    pager = new Pager(1, totalRows);
                }
                else if (granularity == "year")//按年统计
                {
                    int totalRows = _analogHistoryRepos.GetEnergyStatisYearForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).Count();
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
                    var tempQuery = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode);
                    if (dw.HasValue)
                    {
                        tempQuery = tempQuery.Where(x => x.Time.DayOfWeek == dw.Value).ToList();
                    }
                    int totalRows = tempQuery.Count();
                    pager = new Pager(1, totalRows);
                }
                else if (granularity == "selectedday")
                {
                    int totalRows = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).Where(x => x.Time.Day == selectedDay).Count();
                    pager = new Pager(1, totalRows);
                }
                else //小时
                {
                    if (hourType != null && hourType == 1 && startHour < endHour) // 指定小时段
                    {
                        int totalRows = _analogHistoryRepos.GetEnergyStatisSpecialHoursForStatistic(objType, objIDs, startTime, endTime, powerTypes, startHour, endHour, 1, statisticMode).Count();
                        pager = new Pager(1, totalRows);
                    }
                    else // 每小时
                    {
                        int totalRows = _analogHistoryRepos.GetEnergyStatisHourForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).Count();
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
                    list = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).OrderBy(x => x.Time).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                }
                else if (granularity == "month")//按月统计
                {
                    list = _analogHistoryRepos.GetEnergyStatisMonthForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).OrderBy(x => x.Time).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                }
                else if (granularity == "year")//按年统计
                {
                    list = _analogHistoryRepos.GetEnergyStatisYearForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).OrderBy(x => x.Time).Skip(pager.StartRow).Take(pager.PageSize).ToList();
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
                    var query = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode);
                    if (dw != null)
                    {
                        query = query.Where(x => x.Time.DayOfWeek == dw.Value).ToList();
                    }
                    list = query.OrderBy(x => x.Time).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                }
                else if (granularity == "selectedday")
                {
                    list = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).Where(x => x.Time.Day == selectedDay).OrderBy(x => x.Time).ToList();
                }
                else //小时
                {
                    if (hourType != null && hourType == 1 && startHour < endHour) // 指定小时段
                    {
                        list = _analogHistoryRepos.GetEnergyStatisSpecialHoursForStatistic(objType, objIDs, startTime, endTime, powerTypes, startHour, endHour, 1, statisticMode).OrderBy(x => x.Time).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    }
                    else // 每小时
                    {
                        list = _analogHistoryRepos.GetEnergyStatisHourForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).OrderBy(x => x.Time).Skip(pager.StartRow).Take(pager.PageSize).ToList();
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
            IList<String> dateTimeList = new List<String>();
            IList<String> dateEnergyVal = new List<String>();
            foreach(var item in list){
                dateTimeList.Add(item.TimeBlock);
                dateEnergyVal.Add(item.StatisSVal);
            }
            var resultData = new
            {
                totalPages = pager.TotalPages,
                yAxisTitle = yAxisTitle,
                average = average.ToString("f1"),
                max = max.ToString("f1"),
                min = min.ToString("f2"),
                dateTimeList = dateTimeList,
                dateEnergyVal = dateEnergyVal
            };
            return Json(resultData, JsonRequestBehavior.AllowGet);
          }   

        /// <summary>
        /// Ajax获取用电统计数据（不分页面）
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
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetElecAllAjax(int objType, int objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType, int? weekType, int? hourType, int startHour, int endHour, int selectedDay,string statisticMode)
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
                    list = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).OrderBy(x => x.Time).ToList();
                }
                else if (granularity == "month")//按月统计
                {
                    list = _analogHistoryRepos.GetEnergyStatisMonthForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1,statisticMode).OrderBy(x => x.Time).ToList();
                }
                else if (granularity == "year")//按年统计
                {
                    list = _analogHistoryRepos.GetEnergyStatisYearForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1,statisticMode).OrderBy(x => x.Time).ToList();
                }
                else if (granularity == "week") //星期
                {
                    var query = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode);
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
                    var query = _analogHistoryRepos.GetEnergyStatisMonthForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1,statisticMode);
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
                    var query = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode);
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
                    var query = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode);
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
                        list = _analogHistoryRepos.GetEnergyStatisSpecialHoursForStatistic(objType, objIDs, startTime, endTime, powerTypes, startHour, endHour, 1,statisticMode).OrderBy(x => x.Time).ToList();
                    }
                    else // 每小时
                    {
                        list = _analogHistoryRepos.GetEnergyStatisHourForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1,statisticMode).OrderBy(x => x.Time).ToList();
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
        /// Ajax获取用电统计数据（不分页面）(专门提供给手机端调用)
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="granularity"></param>
        /// <returns></returns>
        public ActionResult GetElecAllAjaxForMobile(int objType, int objIDs, DateTime startTime, DateTime endTime, string granularity)
        {
            // 因为手机端是做简单用电量查询，只有三种方式（按年、按月和按天），所以把有些变量写死
            string powerType = "001";
            int? weekType = 0;
            int? hourType = 0;
            int startHour = 0;
            int endHour = 1;
            int selectedDay = 1;
            string statisticMode = "totalEnergy";

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
                list = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).OrderBy(x => x.Time).ToList();
            }
            else if (granularity == "month")//按月统计
            {
                list = _analogHistoryRepos.GetEnergyStatisMonthForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).OrderBy(x => x.Time).ToList();
            }
            else if (granularity == "year")//按年统计
            {
                list = _analogHistoryRepos.GetEnergyStatisYearForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).OrderBy(x => x.Time).ToList();
            }
            else if (granularity == "week") //星期
            {
                var query = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode);
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
                var query = _analogHistoryRepos.GetEnergyStatisMonthForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode);
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
                var query = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode);
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
                var query = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode);
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
                    list = _analogHistoryRepos.GetEnergyStatisSpecialHoursForStatistic(objType, objIDs, startTime, endTime, powerTypes, startHour, endHour, 1, statisticMode).OrderBy(x => x.Time).ToList();
                }
                else // 每小时
                {
                    list = _analogHistoryRepos.GetEnergyStatisHourForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).OrderBy(x => x.Time).ToList();
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
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetElecExcel(int objType, int objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType, int? weekType, int? hourType, int startHour, int endHour, int selectedDay, string statisticMode)
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
                    list = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode).OrderBy(x => x.Time).ToList();
                }
                else if (granularity == "month")//按月统计
                {
                    list = _analogHistoryRepos.GetEnergyStatisMonthForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1,statisticMode).OrderBy(x => x.Time).ToList();
                }
                else if (granularity == "year")//按年统计
                {
                    list = _analogHistoryRepos.GetEnergyStatisYearForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1,statisticMode).OrderBy(x => x.Time).ToList();
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
                    var query = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode);
                    if (dw != null)
                    {
                        query = query.Where(x => x.Time.DayOfWeek == dw.Value).ToList();
                    }
                    list = query.OrderBy(x => x.Time).ToList();
                }
                else if (granularity == "selectedday")
                {
                    var query = _analogHistoryRepos.GetEnergyStatisDayForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1, statisticMode);
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
                        list = _analogHistoryRepos.GetEnergyStatisSpecialHoursForStatistic(objType, objIDs, startTime, endTime, powerTypes, startHour, endHour, 1,statisticMode).OrderBy(x => x.Time).ToList();
                    }
                    else // 每小时
                    {
                        list = _analogHistoryRepos.GetEnergyStatisHourForStatistic(objType, objIDs, startTime, endTime, powerTypes, 1,statisticMode).OrderBy(x => x.Time).ToList();
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
        /// 能耗对比
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult Compare(int? y)
        {                               
            var buildingTypesList = _buildingTypesRepos.GetAllBuildingTypes();              
            if(y.HasValue && y > 0)    ViewBag.selectedBuilding = y;
            var powerList = _powerClassRepos.GetStatisTypes();
            var schoolList = _schoolRepos.QueryAllSchool();
            ViewBag.schoolList = schoolList;
            ViewBag.buildingTypesList = buildingTypesList;
            return View(powerList);
        }

        /// <summary>
        /// 获取能耗对比数据
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="schoolId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerType"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public ActionResult GetCompareEnergy(int currentPage, int totalPages, int objType, string objIDs, int schoolId, DateTime startTime, DateTime endTime, string powerType, int orderType, string buildingType, int ifPage)
        {
            if (Request.IsAjaxRequest())
            {
                Pager pager = null;
                int?[] ids = null;
                IList list = null;
                int totalRows = 0;
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
                if (orderType == 5 || orderType == 6)
                {
                    if (powerTypes.Length != 0)
                    {
                        if (powerTypes[0].Substring(0, 3) == "001")
                        {
                            yAxisTitle = "单位建筑面积用电量（kWh/m2）";
                        }
                        else if (powerTypes[0].Substring(0, 3) == "002")
                        {
                            yAxisTitle = "单位建筑面积用水量（吨/平方米）";
                        }
                        else if (powerTypes[0].Substring(0, 3) == "003")
                        {
                            yAxisTitle = "单位建筑面积燃气用量（立方米/平方米）";
                        }
                    }
                }
                else
                {
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
                }

                if (objType != 0 && objType > 0 && !string.IsNullOrWhiteSpace(objIDs))
                {
                    string[] idStrs = objIDs.Split(new char[] { '_' });
                    ids = new int?[idStrs.Length];
                    for (int i = 0; i < idStrs.Length; i++)
                    {
                        int temp = -1;
                        if (Int32.TryParse(idStrs[i], out temp))
                        {
                            ids[i] = temp;
                        }
                    }
                }
                if (ifPage == 1)
                {
                    if (totalPages == -1)
                    {
                        totalRows = _analogHistoryRepos.GetEnergyFullName(objType, ids, buildingType, schoolId, powerTypes, startTime, endTime, 1).Count();
                        pager = new Pager(1, totalRows);
                    }
                    else
                    {
                        pager = new Pager(currentPage, totalPages, false);
                    }
                    if (pager.TotalPages > 0)
                    {
                        var query = _analogHistoryRepos.GetEnergyFullName(objType, ids, buildingType, schoolId, powerTypes, startTime, endTime, 1);
                        if (orderType > 0)
                        {
                            switch (orderType)
                            {
                                case 1:
                                    query = query.OrderByDescending(x => x.StatisVal);
                                    break;
                                case 2:
                                    query = query.OrderBy(x => x.StatisVal);
                                    break;
                                case 3:
                                    query = query.OrderBy(x => x.Name);
                                    break;
                                case 4:
                                    query = query.OrderByDescending(x => x.Name);
                                    break;
                                case 5:
                                    query = query.OrderByDescending(x => x.valPerArea);
                                    break;
                                case 6:
                                    query = query.OrderBy(x => x.valPerArea);
                                    break;
                            }
                        }
                        list = query.Skip(pager.StartRow).Take(pager.PageSize).ToList();
                        foreach (ChartStatisEntity item in list)
                        {
                            item.EntityIndex = pager.StartRow + list.IndexOf(item) + 1;
                        }
                    }
                    var resultData = new
                    {
                        totalPages = pager.TotalPages,
                        yAxisTitle = yAxisTitle,
                        data = list
                    };

                    return Json(resultData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var query = _analogHistoryRepos.GetEnergyFullName(objType, ids, buildingType, schoolId, powerTypes, startTime, endTime, 1);
                    if (orderType > 0)
                    {
                        switch (orderType)
                        {
                            case 1:
                                query = query.OrderByDescending(x => x.StatisVal);
                                break;
                            case 2:
                                query = query.OrderBy(x => x.StatisVal);
                                break;
                            case 3:
                                query = query.OrderBy(x => x.Name);
                                break;
                            case 4:
                                query = query.OrderByDescending(x => x.Name);
                                break;
                            case 5:
                                query = query.OrderByDescending(x => x.valPerArea);
                                break;
                            case 6:
                                query = query.OrderBy(x => x.valPerArea);
                                break;
                        }
                    }
                    list = query.ToList();
                    totalRows = list.Count;
                    foreach (ChartStatisEntity item in list)
                    {
                        item.EntityIndex = list.IndexOf(item) + 1;
                    }
                    var resultData = new
                    {
                        count = totalRows,
                        yAxisTitle = yAxisTitle,
                        data = list
                    };

                    return Json(resultData, JsonRequestBehavior.AllowGet);
                }


            }
            return null;
        }

        /// <summary>
        /// Ajax获取所有建筑类型一览表查询数据（分页）
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>      
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>      
        /// <param name="powerType"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public ActionResult GetHGElecListAjax(int currentPage, int totalPages, DateTime startTime, DateTime endTime, string powerType, double sum)
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
                if (totalPages == -1)
                {
                    int totalRows = 0;                   
                    totalRows = _analogHistoryRepos.GetYearEnergyByIDsGranularityForAllBuildingTypes(3, startTime, endTime, powerTypes, 1).Count();                                 
                    pager = new Pager(1, totalRows);
                }
                else
                {
                    pager = new Pager(currentPage, totalPages, false);
                }
                IList list = null;
                if (pager.TotalPages > 0)
                {                   
                        if (sum < 0)
                        {
                            //sum = _analogHistoryRepos.GetEnergySum(objType, ids, startTime, endTime, powerTypes);
                            sum = _analogHistoryRepos.GetYearEnergyByIDsGranularityForAllBuildingTypes(3, startTime, endTime, powerTypes, 1).Sum(x => x.StatisVal);
                        }
                        list = _analogHistoryRepos.GetYearEnergyByIDsGranularityForAllBuildingTypes(3, startTime, endTime, powerTypes, sum).Skip(pager.StartRow).Take(pager.PageSize).ToList();                
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

        /// <summary>
        /// Ajax获取所有建筑类型一览表查询数据（不分页）
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>      
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>      
        /// <param name="powerType"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public ActionResult GetHGElecListAjaxNoPage(int currentPage, int totalPages, DateTime startTime, DateTime endTime, string powerType, double sum)
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
                int totalRows = 0;
                totalRows = _analogHistoryRepos.GetYearEnergyByIDsGranularityForAllBuildingTypes(3, startTime, endTime, powerTypes, 1).Count();                 
                IList list = null;             
                if (sum < 0)
                {                           
                    sum = _analogHistoryRepos.GetYearEnergyByIDsGranularityForAllBuildingTypes(3, startTime, endTime, powerTypes, 1).Sum(x => x.StatisVal);
                }
                list = _analogHistoryRepos.GetYearEnergyByIDsGranularityForAllBuildingTypes(3, startTime, endTime, powerTypes, 1).ToList();
           
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
        /// Ajax获取所有建筑类型的图表查询数据（分页）
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>      
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>      
        /// <param name="powerType"></param>     
        /// <returns></returns>
        public ActionResult GetHGElecChartAjax(int currentPage, int totalPages, DateTime startTime, DateTime endTime, string powerType)
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
                if (totalPages == -1)
                {
                    int totalRows = 0;
                    totalRows = _analogHistoryRepos.GetDateByYearForStatistic(3, startTime, endTime, powerTypes).Count();                  
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
                    var timeList = _analogHistoryRepos.GetDateByYearForStatistic(3, startTime, endTime, powerTypes).OrderBy(x => x).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    startTime = timeList.Min();
                    endTime = timeList.Max();
                    dataList = _analogHistoryRepos.GetYearEnergyByIDsGranularityForAllBuildingTypes(3, startTime, endTime, powerTypes, 1).ToList();                   
                  
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
        /// Ajax获取所有建筑类型的图表查询数据（不分页）
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>      
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>      
        /// <param name="powerType"></param>     
        /// <returns></returns>
        public ActionResult GetHGElecChartAjaxNoPage(int currentPage, int totalPages, DateTime startTime, DateTime endTime, string powerType)
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
                int totalRows = 0;
                totalRows = _analogHistoryRepos.GetDateByYearForStatistic(3, startTime, endTime, powerTypes).Count();               
               
                IList list = new ArrayList();
                IList indexValList = null;
              
                IList<ChartStatisEntity> dataList = null;                   
                var timeList = _analogHistoryRepos.GetDateByYearForStatistic(3, startTime, endTime, powerTypes).OrderBy(x => x).ToList();
                startTime = timeList.Min();
                endTime = timeList.Max();
                dataList = _analogHistoryRepos.GetYearEnergyByIDsGranularityForAllBuildingTypes(3, startTime, endTime, powerTypes, 1).ToList();
                 
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
        /// 导出所有建筑类型统计Excel
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>      
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>      
        /// <param name="powerType"></param>
        /// <returns></returns>
        public ActionResult GetHGElecExcel(int currentPage, int totalPages, DateTime startTime, DateTime endTime, string powerType)
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
            double sum = _analogHistoryRepos.GetEnergySumForStatistic(3, startTime, endTime, powerTypes);
            list = _analogHistoryRepos.GetYearEnergyByIDsGranularityForAllBuildingTypes(3, startTime, endTime, powerTypes, 1).ToList();          
          
            if (list != null)
            {               
                string title = "所有建筑类型能耗统计(" + startTime.ToString("yyyy-MM-dd") + "_" + endTime.ToString("yyyy-MM-dd") + ")";
                string[] headers = { "日期", "查询对象", "用能值", "所占比例" };
                string[] properties = { "TimeBlock", "Name", "StatisSVal", "SPercentage" };
                return this.Excel(list, "所有建筑类型能耗统计.xls", title, headers, properties);
            }
            return null;
        }

        /// <summary>
        /// 按各种时间粒度获得各个校区的总能耗情况和生均能耗情况
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllSchoolEnergyInfo() 
       {
           if (Request.IsAjaxRequest())
           {
                IList resultList = new ArrayList();       
                IList<ChartStatisEntity> yearList = null;
                IList<ChartStatisEntity> monthList = null;
                IList<ChartStatisEntity> dayList = null;
                var schoolList = _schoolRepos.QueryAllSchool();
               DateTime currentTime = DateTime.Now;              
               
               String[] powerTypes = null;
               double yearVal = 0;
               double perYearVal = 0;
               double monthVal = 0;
               double perMonthVal = 0;
               double dayVal = 0;
               double perDayVal = 0;
               foreach (var item in schoolList)
               {
                    var schoolName = item.SI_Name;
                    var schoolID = item.SI_ID;
                    var buildingArea = item.SI_BuildingArea;                   
                    yearList = _analogHistoryRepos.GetEnergyStatisYear(1, schoolID, currentTime, currentTime, powerTypes, 0);
                    if (yearList.Count == 1)
                    {
                        yearVal = yearList[0].StatisVal;
                        if(buildingArea > 0)   perYearVal = Convert.ToDouble(yearVal) / Convert.ToDouble(buildingArea);
                        monthList = _analogHistoryRepos.GetEnergyStatisMonth(1, schoolID, currentTime, currentTime, powerTypes, 0);
                        if (monthList.Count == 1)
                        {
                            monthVal = monthList[0].StatisVal;
                            if (buildingArea > 0) perMonthVal = Convert.ToDouble(monthVal) / Convert.ToDouble(buildingArea);
                            dayList = _analogHistoryRepos.GetEnergyStatisDay(1, schoolID, currentTime, currentTime, powerTypes, 0);
                            if (dayList.Count == 1)
                            {
                                dayVal = dayList[0].StatisVal;
                                 if(buildingArea > 0)   perDayVal = Convert.ToDouble(dayVal) / Convert.ToDouble(buildingArea);
                            }                           
                        }
                    }
                    var list = new
                    {
                        SchoolName = schoolName,
                        YearVal = yearVal,
                        PerYearVal = perYearVal,
                        MonthVal = monthVal,
                        PerMonthVal = perMonthVal,
                        DayVal = dayVal,
                        PerDayVal = perDayVal
                    };
                    resultList.Add(list);                                    
              }               
               return Json(resultList, JsonRequestBehavior.AllowGet);             
           }
           return null;            
       }

        /// <summary>
        /// 导出能耗对比Excel
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="schoolId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerType"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public ActionResult GetCompareEnergyExcel(int objType, string objIDs, int schoolId, DateTime startTime, DateTime endTime, string powerType, int orderType, string buildingType)
        {
            int?[] ids = null;
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
            if (orderType == 5 || orderType == 6)
            {
                if (powerTypes.Length != 0)
                {
                    if (powerTypes[0].Substring(0, 3) == "001")
                    {
                        yAxisTitle = "单位建筑面积用电量（kWh/m2）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "002")
                    {
                        yAxisTitle = "单位建筑面积用水量（吨/平方米）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "003")
                    {
                        yAxisTitle = "单位建筑面积燃气用量（立方米/平方米）";
                    }
                }
            }
            else
            {
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
            }

            if (objType != 0 && objType > 0 && !string.IsNullOrWhiteSpace(objIDs))
            {
                string[] idStrs = objIDs.Split(new char[] { '_' });
                ids = new int?[idStrs.Length];
                for (int i = 0; i < idStrs.Length; i++)
                {
                    int temp = -1;
                    if (Int32.TryParse(idStrs[i], out temp))
                    {
                        ids[i] = temp;
                    }
                }
            }
            var query = _analogHistoryRepos.GetEnergyFullName(objType, ids, buildingType, schoolId, powerTypes, startTime, endTime, 1);
            if (orderType > 0)
            {
                switch (orderType)
                {
                    case 1:
                        query = query.OrderByDescending(x => x.StatisVal);
                        break;
                    case 2:
                        query = query.OrderBy(x => x.StatisVal);
                        break;
                    case 3:
                        query = query.OrderBy(x => x.Name);
                        break;
                    case 4:
                        query = query.OrderByDescending(x => x.Name);
                        break;
                    case 5:
                        query = query.OrderByDescending(x => x.valPerArea);
                        break;
                    case 6:
                        query = query.OrderBy(x => x.valPerArea);
                        break;
                }
            }
            var list = query.ToList();
            string title = "能耗对比(" + startTime.ToString("yyyy-MM-dd") + "_" + endTime.ToString("yyyy-MM-dd") + ")";
            string[] headers = { "所属对象", yAxisTitle };
            string[] properties = { "Name", "StatisSVal" };
            string[] properties2 = { "Name", "valPerAreaStr" };
            if (orderType == 5 || orderType == 6) {
                return this.Excel(list, title + ".xls", title, headers, properties2);
            }
            return this.Excel(list, title + ".xls", title, headers, properties);
        }
    }
}
