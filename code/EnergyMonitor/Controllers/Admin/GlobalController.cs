using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using EnergyMonitor.Controllers.Admin.Filters;
using System.Collections;
using EnergyMonitor.Controllers.Utils;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Implement;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Entity;
using EnergyMonitor.Models.Entity;


namespace EnergyMonitor.Controllers.Admin
{
    /// <summary>
    /// 全局能耗
    /// </summary>
    public class GlobalController : Controller
    {
        private IBuildingRepos _buildingRepos = null;
        private IAnalogHistoryRepos _analogHistoryRepos = null;
        private ISchoolRepos _schoolRepos = null;
        private ISchoolAreaRepos _schoolAreaRepos = null;
        private IAMPRepos _ampRepos = null;
        private IPowerClassRepos _powerClassRepos = null;
        private IRoomRepos _roomRepos = null;
        private IBECRepos _becRepos = null;
        private IAnalogMeasurePoint _analogMeasurePointRepos = null;
        private IAnnouncementInfoRepos _announcementInfoRepos = null;
        public GlobalController()
            : this(new BuildingRepos(), new AnalogHistoryRepos(), new SchoolRepos(), new SchoolAreaRepos(), new AMPRepos(), new PowerClassRepos(), new RoomRepos(), new BECRepos(), new AnalogMeasurePointRepos() , new AnnouncementInfoRepos())
        {
        }

        public GlobalController(IBuildingRepos buildingRepos, IAnalogHistoryRepos analogHistoryRepos, ISchoolRepos schoolRepos, ISchoolAreaRepos schoolAreaRepos, IAMPRepos ampRepos, IPowerClassRepos powerClassRepos, IRoomRepos roomRepos, IBECRepos becRepos, IAnalogMeasurePoint analogMeasurePointRepos , IAnnouncementInfoRepos announcementInfoRepos)
        {
            _buildingRepos = buildingRepos;
            _analogHistoryRepos = analogHistoryRepos;
            _schoolRepos = schoolRepos;
            _schoolAreaRepos = schoolAreaRepos;
            _ampRepos = ampRepos;
            _powerClassRepos = powerClassRepos;
            _roomRepos = roomRepos;
            _becRepos = becRepos;
            _analogMeasurePointRepos = analogMeasurePointRepos;
            _announcementInfoRepos = announcementInfoRepos;
        }

        /// <summary>
        /// 地图展示
        /// </summary>
        /// <returns></returns>
        public ActionResult Map()
        {
            IList<AllSchoolEnergyInfoEntity> resultList = new List<AllSchoolEnergyInfoEntity>();
            IList<ChartStatisEntity> yearList = null;
            IList<ChartStatisEntity> monthList = null;
            IList<ChartStatisEntity> dayList = null;
            DateTime currentTime = DateTime.Now;

            String[] powerTypes = null;
            double yearVal = 0;
            double perYearVal = 0;
            double monthVal = 0;
            double perMonthVal = 0;
            double dayVal = 0;
            double perDayVal = 0;
            int i = 0;
            powerTypes = null;
            var schoolList = _schoolRepos.QueryAllSchool();
            var schoolMeasurePoint = _analogMeasurePointRepos.GetAllSchoolMeasurePoint().ToArray();
            foreach (var item in schoolList)
            {
                var schoolName = item.SI_Name;
                var buildingArea = item.SI_BuildingArea;
                yearList = _analogHistoryRepos.GetEnergyStatisYear(5, schoolMeasurePoint[i], currentTime, currentTime, powerTypes, 0);
                if (yearList.Count == 1)
                {
                    yearVal = yearList[0].StatisVal;
                    if (buildingArea > 0)
                    {
                        perYearVal = Convert.ToDouble(yearVal) / Convert.ToDouble(buildingArea);
                    }
                    else
                    {
                        perYearVal = -1;
                    }
                    monthList = _analogHistoryRepos.GetEnergyStatisMonth(5, schoolMeasurePoint[i], currentTime, currentTime, powerTypes, 0);
                    if (monthList.Count == 1)
                    {
                        monthVal = monthList[0].StatisVal;
                        if (buildingArea > 0)
                        {
                            perMonthVal = Convert.ToDouble(monthVal) / Convert.ToDouble(buildingArea);
                        }
                        else
                        {
                            perMonthVal = -1;
                        }
                        dayList = _analogHistoryRepos.GetEnergyStatisDay(5, schoolMeasurePoint[i], currentTime, currentTime, powerTypes, 0);
                        if (dayList.Count == 1)
                        {
                            dayVal = dayList[0].StatisVal;
                            if (buildingArea > 0)
                            {
                                perDayVal = Convert.ToDouble(dayVal) / Convert.ToDouble(buildingArea);
                            }
                            else
                            {
                                perDayVal = -1;
                            }
                        }
                        else
                        {
                            dayVal = 0;
                            perDayVal = 0;
                        }
                    }
                    else
                    {
                        monthVal = 0;
                        perMonthVal = 0;
                        dayVal = 0;
                        perDayVal = 0;
                    }
                    i++;
                }
                else
                {
                    yearVal = 0;
                    perYearVal = 0;
                    monthVal = 0;
                    perMonthVal = 0;
                    dayVal = 0;
                    perDayVal = 0;
                }
                var list = new AllSchoolEnergyInfoEntity
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
            return View(resultList);
        }
        /// <summary>
        /// 全屏浏览地图
        /// </summary>
        /// <param name="campusID"></param>
        /// <returns></returns>
        public ActionResult FullScrMap(int? campusID)
        {
            //获取该校区内所有建筑信息
            ArrayList infos = new ArrayList();
            var areaInfos = _schoolAreaRepos.GetSchoolAllArea(campusID.Value + 1);
            foreach (var areaInfo in areaInfos)
            {
                var buildingsInfos = _buildingRepos.GetBuildingsOfArea(areaInfo.SAI_ID).ToList<BuildingBriefInfo>();
                AreaAndBuilding newareaAndBuilding = new AreaAndBuilding { AreaName = areaInfo.SAI_Name, AreaID = areaInfo.SAI_ID, buildingInfo = buildingsInfos };
                infos.Add(newareaAndBuilding);
            }
            ViewBag.allBuildingInfo = infos;

            return View(campusID);
        }

        /// <summary>
        /// 浏览详细地图
        /// </summary>
        /// <returns></returns>
        public ActionResult BrowseMap(int? campusID)
        {

            return View(campusID);
        }

        public ActionResult FullViewMap()
        {
            List<SchoolInfo> obj = _schoolRepos.QueryAllSchool();
            return View(obj);
        }
        /// <summary>
        /// 跳转建筑详细
        /// </summary>
        /// <param name="param">建筑ID</param>
        /// <returns></returns>
        public ActionResult Detail(int? param)
        {
            if (param.HasValue && param.Value > 0)
            {
                var building = _buildingRepos.GetBuilding(param.Value);
                DateTime now = DateTime.Now;
                DateTime endMonth = DateTime.Parse(now.ToString("yyyy-MM-1"));
                // 开始时间为前十个月
                DateTime startMonth = endMonth.AddMonths(-9);
                // 先默认为电
                var monthDataList = _analogHistoryRepos.GetEnergyStatisMonth(3, param.Value, startMonth, endMonth, new string[] { "001001" }, 1).ToList();
                ViewBag.monthData = monthDataList;
                DateTime endDay = now;
                DateTime startDay = endDay.AddDays(-9);
                // 先默认为电
                var dayDataList = _analogHistoryRepos.GetEnergyStatisDay(3, param.Value, startDay, endDay, new string[] { "001001" }, 1).ToList();
                ViewBag.dayData = dayDataList;


                if (monthDataList != null && monthDataList.Count > 0)
                {
                    // 获取当月用电量
                    var currentMonth = monthDataList.OrderByDescending(x => x.Time).FirstOrDefault();
                    ViewBag.currentMonth = currentMonth;

                    double sum = monthDataList.Sum(x => x.StatisVal);
                    var maxObj = monthDataList.OrderByDescending(x => x.StatisVal).FirstOrDefault();
                    var minObj = monthDataList.OrderBy(x => x.StatisVal).FirstOrDefault();
                    maxObj.Sum = sum;
                    minObj.Sum = sum;
                    Hashtable monthAnalysisData = new Hashtable();
                    monthAnalysisData.Add("totalRows", monthDataList.Count);
                    monthAnalysisData.Add("totalSum", sum.ToString("f1"));
                    monthAnalysisData.Add("average", (sum / monthDataList.Count).ToString("f1"));
                    monthAnalysisData.Add("maxObj", maxObj);
                    monthAnalysisData.Add("minObj", minObj);
                    ViewBag.monthAnalysisData = monthAnalysisData;
                }
                if (dayDataList != null && dayDataList.Count > 0)
                {
                    double sum = dayDataList.Sum(x => x.StatisVal);
                    var maxObj = dayDataList.OrderByDescending(x => x.StatisVal).FirstOrDefault();
                    var minObj = dayDataList.OrderBy(x => x.StatisVal).FirstOrDefault();
                    maxObj.Sum = sum;
                    minObj.Sum = sum;
                    Hashtable dayAnalysisData = new Hashtable();
                    dayAnalysisData.Add("totalRows", dayDataList.Count);
                    dayAnalysisData.Add("totalSum", sum.ToString("f1"));
                    dayAnalysisData.Add("average", (sum / dayDataList.Count).ToString("f1"));
                    dayAnalysisData.Add("maxObj", maxObj);
                    dayAnalysisData.Add("minObj", minObj);
                    ViewBag.dayAnalysisData = dayAnalysisData;
                }
                return View(building);
            }
            return View();
        }


        public ActionResult GetAllTypeEnergy(int? buildingID)
        {
            if (buildingID != null)
            {
                ArrayList FirstLevelTypes = new ArrayList();
                int?[] buildingList = { buildingID };
                IList<PowerClass> powertypes = _powerClassRepos.GetThreeType();
                foreach (var powertype in powertypes)
                {
                    string[] typeStr = { powertype.PC_ID };
                    IList<EnergyEntity> powerReals = _ampRepos.GetRealEnergyByBuilding(buildingList, typeStr, 0, 100);
                    if (powertype.PC_ID.Length == 3 && powerReals.Count != 0)
                    {
                        FirstLevelTypes.Add(new { powertype = powertype.PC_ID, val = powerReals.SingleOrDefault().RemValStr });

                    }
                }
            }
            return View();
        }


        /// <summary>
        /// 建筑详细信息页面
        /// </summary>
        /// <param name="campusID"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public ActionResult BuildingDetail(int? campusID, int? buildingID)
        {
            if (campusID != null && buildingID != null)
            {
                //获取建筑基本信息
                BuildingDetailInfo building = _buildingRepos.GetBuilding(buildingID.Value);
                ViewBag.buildingInfo = building;
                int?[] buildings = { buildingID };
                string[] elecType = { "001" };
                IList<EnergyEntity> powerElecReals = new List<EnergyEntity>();
                //double totalElecVal = 0;
                IList<PowerClass> elecPowerClass = _powerClassRepos.GetSubPowers("001").ToList();
                string[] allElecTypes = elecPowerClass.Select(x => x.PC_ID).Union(new string[] { "001" }).ToArray();
                string elecPowerTypeStr = "001";
                //foreach (var elecSubPowerClass in elecPowerClass)
                //{
                //    string[] elecSubType = new string[] { elecSubPowerClass.PC_ID };
                //    elecPowerTypeStr = elecPowerTypeStr + "_" + elecSubPowerClass.PC_ID;
                //    EnergyEntity subPowerVal = _ampRepos.GetRealEnergyByBuildingOnly(buildings, elecSubType).FirstOrDefault();
                //    powerElecReals.Add(subPowerVal);
                //    if (subPowerVal != null)
                //    {
                //        totalElecVal += subPowerVal.Val.Value;
                //    }
                //}
                //powerElecReals.Insert(0, new EnergyEntity { PowerName = "总用电量", Val = totalElecVal, PowerType = elecPowerTypeStr });
                ViewBag.powerElecType = elecPowerClass.ToArray();

                string[] waterType = { "002" };
                IList<EnergyEntity> powerWaterReals = new List<EnergyEntity>();
                //double totalWaterVal = 0;
                IList<PowerClass> waterPowerClass = _powerClassRepos.GetSubPowers("002").ToList();
                string[] allWaterTypes = waterPowerClass.Select(x => x.PC_ID).Union(new string[] { "002" }).ToArray();
                string waterPowerTypeStr = "002";
                //foreach (var waterSubPowerClass in waterPowerClass)
                //{
                //    string[] waterSubType = new string[] { waterSubPowerClass.PC_ID };
                //    EnergyEntity subPowerVal = _ampRepos.GetRealEnergyByBuildingOnly(buildings, waterSubType).FirstOrDefault();
                //    powerWaterReals.Add(subPowerVal);
                //    waterPowerTypeStr = waterPowerTypeStr + "_" + waterSubPowerClass.PC_ID;
                //    if (subPowerVal != null)
                //    {
                //        totalWaterVal += subPowerVal.Val.Value;
                //    }
                //}
                //powerWaterReals.Insert(0, new EnergyEntity { PowerName = "用水量", Val = totalWaterVal, PowerType = waterPowerTypeStr });
                ViewBag.powerWaterType = waterPowerClass.ToArray();

                string[] gasType = { "003" };
                //string gasPowerTypeStr = "003";
                IList<EnergyEntity> powerGasReals = new List<EnergyEntity>();
                //double totalGasVal = 0;
                IList<PowerClass> gasPowerClass = _powerClassRepos.GetSubPowers("003").ToList();
                string[] allGusTypes = gasPowerClass.Select(x => x.PC_ID).Union(new string[] { "003" }).ToArray();
                ////foreach (var gasSubPowerClass in gasPowerClass)
                //{
                //    string[] gasSubType = new string[] { gasSubPowerClass.PC_ID };
                //    EnergyEntity subPowerVal = _ampRepos.GetRealEnergyByBuildingOnly(buildings, gasSubType).FirstOrDefault();
                //    powerGasReals.Add(subPowerVal);
                //    gasPowerTypeStr = gasPowerTypeStr + "_" + gasSubPowerClass.PC_ID;
                //    if (subPowerVal != null)
                //    {
                //        totalGasVal += subPowerVal.Val.Value;
                //    }
                //}
                //powerGasReals.Insert(0, new EnergyEntity { PowerName = "燃气用量", Val = totalGasVal, PowerType = gasPowerTypeStr });
                //ViewBag.powerGasType = gasPowerClass.ToArray();
                //if (powerElecReals.Count > 0)
                //{
                //    ViewBag.powerElecReals = powerElecReals.ToArray();
                //}
                //if (powerWaterReals.Count > 0)
                //{
                //    ViewBag.powerWaterReals = powerWaterReals.ToArray();
                //}
                //if (powerGasReals.Count > 0)
                //{
                //    ViewBag.powerGasReals = powerGasReals.ToArray();
                //}
                //获取过去24小时内各建筑每小时能耗值
                IList<ChartStatisInfo> historyElecValByHours = new List<ChartStatisInfo>();
                string[] elecPowerClassAll = elecPowerClass.Select(x => x.PC_ID).ToArray();
                IList<ChartStatisEntity> valByHoursElecAllType = _analogHistoryRepos.GetEnergyStatisHour(3, buildingID.Value, DateTime.Parse(DateTime.Now.AddHours(-25).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), allElecTypes, 1).ToList();
                foreach (var elecSubType in elecPowerClass)
                {
                    IList<Models.Repository.Entity.ChartStatisEntity> valByHoursElec = _analogHistoryRepos.GetEnergyStatisHour(3, buildingID.Value, DateTime.Parse(DateTime.Now.AddHours(-25).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), new string[] { elecSubType.PC_ID }, 1).ToList();
                    //foreach (var valByHoursElecAllTypeItem in valByHoursElecAllType)
                    //{
                    //    if (valByHoursElec.Count() > 0 && valByHoursElec.Where(p => p.HTimeStr == valByHoursElecAllTypeItem.HTimeStr).Count() == 0)
                    //    {
                    //        valByHoursElec.Add(new ChartStatisEntity { Time = valByHoursElecAllTypeItem.Time, StatisVal = 0 });
                    //    }
                    //}
                    historyElecValByHours.Add(new ChartStatisInfo { ChartData = valByHoursElec.OrderBy(x => x.Time).ToList(), PowerType = elecSubType });
                }
                historyElecValByHours.Add(new ChartStatisInfo { ChartData = valByHoursElecAllType.OrderBy(x => x.Time).ToList(), PowerType = new PowerClass { PC_ID = "001", PC_Name = "总用电量" } });
                IList<ChartStatisInfo> historyWaterValByHours = new List<ChartStatisInfo>();
                foreach (var waterSubType in waterPowerClass)
                {
                    IList<Models.Repository.Entity.ChartStatisEntity> valByHoursWater = _analogHistoryRepos.GetEnergyStatisHour(3, buildingID.Value, DateTime.Parse(DateTime.Now.AddHours(-25).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), new string[] { waterSubType.PC_ID }, 1).ToList();
                    historyWaterValByHours.Add(new ChartStatisInfo { ChartData = valByHoursWater.OrderBy(x => x.Time).ToList(), PowerType = waterSubType });
                }

                //IList<ChartStatisInfo> historyGasValByHours = new List<ChartStatisInfo>();
                //foreach (var gasSubType in gasPowerClass)
                //{
                //    IList<ChartStatisEntity> valByHoursGas = _analogHistoryRepos.GetEnergyStatisHour(3, buildingID.Value, DateTime.Parse(DateTime.Now.AddHours(-25).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), new string[] { gasSubType.PC_ID }, 1).ToList();
                //    historyGasValByHours.Add(new ChartStatisInfo { ChartData = valByHoursGas.OrderBy(x => x.Time).ToList(), PowerType = gasSubType });
                //}
                if (historyElecValByHours != null && historyElecValByHours.Count > 0)
                {
                    ViewBag.historyElecValByHours = historyElecValByHours;
                }
                if (historyWaterValByHours != null && historyWaterValByHours.Count > 0)
                {
                    ViewBag.historyWaterValByHours = historyWaterValByHours;
                }
                //if (historyGasValByHours != null && historyGasValByHours.Count > 0)
                //{
                //    ViewBag.historyGasValByHours = historyGasValByHours;
                //}

                //获取过去10天各建筑每天的能耗值以及获取建筑当天的能耗值
                IList<ChartStatisInfo> historyElecValByDays = new List<ChartStatisInfo>();
                IList<EnergyEntity> currentElecValByDays = new List<EnergyEntity>();
                int i = 0;
                ChartStatisEntity currentValByDaysElecAllType = null;
                String dtStr = DateTime.Now.ToString("MM-dd");
                IList<ChartStatisEntity> valByDaysElecAllType = _analogHistoryRepos.GetEnergyStatisDay(3, buildingID.Value, DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), allElecTypes, 1).OrderBy(x => x.Time).ToList();
                if (valByDaysElecAllType.Count != 0 && valByDaysElecAllType[valByDaysElecAllType.Count - 1].DTimeStr == dtStr)
                {
                    currentValByDaysElecAllType = valByDaysElecAllType[valByDaysElecAllType.Count - 1];
                }
                foreach (var elecSubType in elecPowerClass)
                {
                    IList<Models.Repository.Entity.ChartStatisEntity> valByDaysElec = _analogHistoryRepos.GetEnergyStatisDay(3, buildingID.Value, DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), new string[] { elecSubType.PC_ID }, 1).OrderBy(x => x.Time).ToList();
                    //foreach (var valByDaysElecAllTypeItem in valByDaysElecAllType)
                    //{
                    //    if (valByDaysElec.Count()>0 && valByDaysElec.Where(p => p.DTimeStr == valByDaysElecAllTypeItem.DTimeStr).Count() == 0)
                    //    {
                    //        valByDaysElec.Add(new ChartStatisEntity { Time = valByDaysElecAllTypeItem.Time, StatisVal = 0 });
                    //    }
                    //}
                    historyElecValByDays.Add(new ChartStatisInfo { ChartData = valByDaysElec.OrderBy(x => x.Time).ToList(), PowerType = elecSubType });
                    if (valByDaysElec.Count != 0 && valByDaysElec[valByDaysElec.Count - 1].DTimeStr == dtStr)
                    {
                        ChartStatisEntity currentChartStatisEntity = historyElecValByDays[i].ChartData[valByDaysElec.Count - 1];
                        currentElecValByDays.Add(new EnergyEntity { PowerName = currentChartStatisEntity.PowerName, Val = currentChartStatisEntity.StatisVal, PowerType = currentChartStatisEntity.PowerType });
                    }
                    else
                    {
                        currentElecValByDays.Add(null);
                    }
                    i = i + 1;
                }
                if (currentValByDaysElecAllType != null && currentValByDaysElecAllType.DTimeStr == dtStr)
                {
                    currentElecValByDays.Insert(0, new EnergyEntity { PowerName = "总用电量", Val = currentValByDaysElecAllType.StatisVal, PowerType = elecPowerTypeStr });
                }
                else
                {
                    currentElecValByDays.Insert(0, new EnergyEntity { PowerName = "总用电量", Val = null, PowerType = elecPowerTypeStr });
                }
                IList<EnergyEntity> currentWaterValByDays = new List<EnergyEntity>();
                ChartStatisEntity currentValByDaysWaterAllType = _analogHistoryRepos.GetEnergyStatisDay(3, buildingID.Value, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), allWaterTypes, 1).ToList().FirstOrDefault();
                foreach (var waterSubPowerClass in waterPowerClass)
                {
                    IList<Models.Repository.Entity.ChartStatisEntity> valByDaysWater = _analogHistoryRepos.GetEnergyStatisDay(3, buildingID.Value, DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), new string[] { waterSubPowerClass.PC_ID }, 1).OrderBy(x => x.Time).ToList();
                    //foreach (var valByDaysElecAllTypeItem in valByDaysElecAllType)
                    //{
                    //    if (valByDaysElec.Count()>0 && valByDaysElec.Where(p => p.DTimeStr == valByDaysElecAllTypeItem.DTimeStr).Count() == 0)
                    //    {
                    //        valByDaysElec.Add(new ChartStatisEntity { Time = valByDaysElecAllTypeItem.Time, StatisVal = 0 });
                    //    }
                    //}                  
                    if (valByDaysWater.Count != 0 && valByDaysWater[valByDaysWater.Count - 1].DTimeStr == dtStr)
                    {
                        ChartStatisEntity currentChartStatisEntity = valByDaysWater[valByDaysWater.Count - 1];
                        currentWaterValByDays.Add(new EnergyEntity { PowerName = currentChartStatisEntity.PowerName, Val = currentChartStatisEntity.StatisVal, PowerType = currentChartStatisEntity.PowerType });
                    }
                    else
                    {
                        currentWaterValByDays.Add(null);
                    }
                }
                if (currentValByDaysWaterAllType != null && currentValByDaysWaterAllType.DTimeStr == dtStr)
                {
                    currentWaterValByDays.Insert(0, new EnergyEntity { PowerName = "用水量", Val = currentValByDaysWaterAllType.StatisVal, PowerType = waterPowerTypeStr });
                }
                else
                {
                    currentWaterValByDays.Insert(0, new EnergyEntity { PowerName = "用水量", Val = null, PowerType = waterPowerTypeStr });
                }
                // IList<EnergyEntity> currentGusValByDays = new List<EnergyEntity>();             
                //ChartStatisEntity currentValByDaysGusAllType = _analogHistoryRepos.GetEnergyStatisDay(3, buildingID.Value, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), allGusTypes, 1).ToList().FirstOrDefault();
                // foreach (var gasSubPowerClass in gasPowerClass)
                // {
                //     IList<Models.Repository.Entity.ChartStatisEntity> valByDaysGus = _analogHistoryRepos.GetEnergyStatisDay(3, buildingID.Value, DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), new string[] { gasSubPowerClass.PC_ID }, 1).OrderBy(x => x.Time).ToList();
                //     //foreach (var valByDaysElecAllTypeItem in valByDaysElecAllType)
                //     //{
                //     //    if (valByDaysElec.Count()>0 && valByDaysElec.Where(p => p.DTimeStr == valByDaysElecAllTypeItem.DTimeStr).Count() == 0)
                //     //    {
                //     //        valByDaysElec.Add(new ChartStatisEntity { Time = valByDaysElecAllTypeItem.Time, StatisVal = 0 });
                //     //    }
                //     //}
                //     if (valByDaysGus.Count != 0 && valByDaysGus[valByDaysGus.Count - 1].DTimeStr == dtStr)
                //     {                     
                //             ChartStatisEntity currentChartStatisEntity = valByDaysGus[valByDaysGus.Count - 1];
                //             currentGusValByDays.Add(new EnergyEntity { PowerName = currentChartStatisEntity.PowerName, Val = currentChartStatisEntity.StatisVal, PowerType = currentChartStatisEntity.PowerType });                     
                //     }
                //     else
                //     {
                //         currentGusValByDays.Add(null);
                //     }
                // }
                // if (currentValByDaysGusAllType != null && currentValByDaysGusAllType.DTimeStr == dtStr)
                // {
                //     currentGusValByDays.Insert(0, new EnergyEntity { PowerName = "燃气电量", Val = currentValByDaysGusAllType.StatisVal, PowerType = gasPowerTypeStr });
                // }
                // else {
                //     currentGusValByDays.Insert(0, new EnergyEntity { PowerName = "燃气电量", Val = null, PowerType = gasPowerTypeStr });
                // }
                if (currentElecValByDays.Count > 0)
                {
                    ViewBag.powerElecReals = currentElecValByDays.ToArray();
                }
                if (currentWaterValByDays.Count > 0)
                {
                    ViewBag.powerWaterReals = currentWaterValByDays.ToArray();
                }
                //if (currentGusValByDays.Count > 0)
                //{
                //    ViewBag.powerGasReals = currentGusValByDays.ToArray();
                //}
                historyElecValByDays.Add(new ChartStatisInfo { ChartData = valByDaysElecAllType.OrderBy(x => x.Time).ToList(), PowerType = new PowerClass { PC_ID = "001", PC_Name = "总用电量" } });
                IList<ChartStatisInfo> historyWaterValByDays = new List<ChartStatisInfo>();
                foreach (var waterSubType in waterPowerClass)
                {
                    IList<ChartStatisEntity> valByDaysWater = _analogHistoryRepos.GetEnergyStatisDay(3, buildingID.Value, DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), new string[] { waterSubType.PC_ID }, 1).ToList();
                    historyWaterValByDays.Add(new ChartStatisInfo { ChartData = valByDaysWater.OrderBy(x => x.Time).ToList(), PowerType = waterSubType });
                }
                IList<ChartStatisInfo> historyGasValByDays = new List<ChartStatisInfo>();
                //foreach (var gasSubType in gasPowerClass)
                //{
                //    IList<ChartStatisEntity> valByDaysGas = _analogHistoryRepos.GetEnergyStatisDay(3, buildingID.Value, DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), new string[] { gasSubType.PC_ID }, 1).ToList();
                //    historyGasValByDays.Add(new ChartStatisInfo { ChartData = valByDaysGas.OrderBy(x => x.Time).ToList(), PowerType = gasSubType });
                //}
                if (historyElecValByDays != null && historyElecValByDays.Count > 0)
                {
                    ViewBag.historyElecValByDays = historyElecValByDays;
                }
                if (historyWaterValByDays != null && historyWaterValByDays.Count > 0)
                {
                    ViewBag.historyWaterValByDays = historyWaterValByDays;
                }
                //if (historyGasValByDays != null && historyGasValByDays.Count > 0)
                //{
                //    ViewBag.historyGasValByDays = historyGasValByDays;
                //}

                //获取过去十月各建筑每天的能耗值
                //电
                IList<ChartStatisInfo> historyElecValByMonths = new List<ChartStatisInfo>();
                IList<EnergyEntity> currentElecValByMonths = new List<EnergyEntity>();
                i = 0;
                ChartStatisEntity currentValByMonthsElecAllType = null;
                String dtmStr = DateTime.Now.Month + "";
                if (DateTime.Now.Month < 10) dtmStr = "0" + DateTime.Now.Month;
                IList<ChartStatisEntity> valByMonthsElecAllType = _analogHistoryRepos.GetEnergyStatisMonth(3, buildingID.Value, DateTime.Parse(DateTime.Now.AddMonths(-10).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), allElecTypes, 1).OrderBy(x => x.Time).ToList();
                if (valByMonthsElecAllType.Count != 0 && valByMonthsElecAllType[valByMonthsElecAllType.Count - 1].MonthStr == dtmStr)
                {
                    currentValByMonthsElecAllType = valByMonthsElecAllType[valByMonthsElecAllType.Count - 1];
                }
                foreach (var elecSubType in elecPowerClass)
                {
                    IList<Models.Repository.Entity.ChartStatisEntity> valByMonthsElec = _analogHistoryRepos.GetEnergyStatisMonth(3, buildingID.Value, DateTime.Parse(DateTime.Now.AddMonths(-10).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), new string[] { elecSubType.PC_ID }, 1).ToList();
                    //foreach (var valByMonthsElecAllTypeItem in valByMonthsElecAllType)
                    //{
                    //    if (valByMonthsElec.Count() > 0 && valByMonthsElec.Where(p => p.MTimeStr == valByMonthsElecAllTypeItem.MTimeStr).Count() == 0)
                    //    {
                    //        valByMonthsElec.Add(new ChartStatisEntity { Time = valByMonthsElecAllTypeItem.Time, StatisVal = 0 });
                    //    }
                    //}
                    historyElecValByMonths.Add(new ChartStatisInfo { ChartData = valByMonthsElec.OrderBy(x => x.Time).ToList(), PowerType = elecSubType });
                    if (valByMonthsElec.Count != 0 && historyElecValByMonths[i].ChartData[valByMonthsElec.Count - 1].MonthStr == dtmStr)
                    {
                        ChartStatisEntity currentChartStatisEntity = historyElecValByMonths[i].ChartData[valByMonthsElec.Count - 1];
                        currentElecValByMonths.Add(new EnergyEntity { PowerName = currentChartStatisEntity.PowerName, Val = currentChartStatisEntity.StatisVal, PowerType = currentChartStatisEntity.PowerType });
                    }
                    else
                    {
                        currentElecValByMonths.Add(null);
                    }
                    i = i + 1;

                }
                if (currentValByMonthsElecAllType != null && currentValByMonthsElecAllType.MonthStr == dtmStr)
                {
                    currentElecValByMonths.Insert(0, new EnergyEntity { PowerName = "总用电量", Val = currentValByMonthsElecAllType.StatisVal, PowerType = elecPowerTypeStr });
                }
                else
                {
                    currentElecValByMonths.Insert(0, new EnergyEntity { PowerName = "总用电量", Val = null, PowerType = elecPowerTypeStr });
                }
                historyElecValByMonths.Add(new ChartStatisInfo { ChartData = valByMonthsElecAllType.OrderBy(x => x.Time).ToList(), PowerType = new PowerClass { PC_ID = "001", PC_Name = "总用电量" } });

                //水
                i = 0;
                IList<ChartStatisInfo> historyWaterValByMonths = new List<ChartStatisInfo>();
                IList<EnergyEntity> currentWaterValByMonths = new List<EnergyEntity>();
                ChartStatisEntity currentValByMonthsWaterAllType = _analogHistoryRepos.GetEnergyStatisMonth(3, buildingID.Value, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), allWaterTypes, 1).ToList().FirstOrDefault();
                foreach (var waterSubPowerClass in waterPowerClass)
                {
                    IList<Models.Repository.Entity.ChartStatisEntity> valByMonthsWater = _analogHistoryRepos.GetEnergyStatisMonth(3, buildingID.Value, DateTime.Parse(DateTime.Now.AddMonths(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), new string[] { waterSubPowerClass.PC_ID }, 1).ToList();
                    historyWaterValByMonths.Add(new ChartStatisInfo { ChartData = valByMonthsWater.OrderBy(x => x.Time).ToList(), PowerType = waterSubPowerClass });
                    //foreach (var valByDaysElecAllTypeItem in valByDaysElecAllType)
                    //{
                    //    if (valByDaysElec.Count()>0 && valByDaysElec.Where(p => p.DTimeStr == valByDaysElecAllTypeItem.DTimeStr).Count() == 0)
                    //    {
                    //        valByDaysElec.Add(new ChartStatisEntity { Time = valByDaysElecAllTypeItem.Time, StatisVal = 0 });
                    //    }
                    //}                  
                    if (valByMonthsWater.Count != 0 && historyWaterValByMonths[i].ChartData[valByMonthsWater.Count - 1].MonthStr == dtmStr)
                    {
                        ChartStatisEntity currentChartStatisEntity = historyWaterValByMonths[i].ChartData[valByMonthsWater.Count - 1];
                        currentWaterValByMonths.Add(new EnergyEntity { PowerName = currentChartStatisEntity.PowerName, Val = currentChartStatisEntity.StatisVal, PowerType = currentChartStatisEntity.PowerType });
                    }
                    else
                    {
                        currentWaterValByMonths.Add(null);
                    }
                    i = i + 1;
                }
                if (currentValByMonthsWaterAllType != null && currentValByMonthsWaterAllType.MonthStr == dtmStr)
                {
                    currentWaterValByMonths.Insert(0, new EnergyEntity { PowerName = "用水量", Val = currentValByMonthsWaterAllType.StatisVal, PowerType = waterPowerTypeStr });
                }
                else
                {
                    currentWaterValByMonths.Insert(0, new EnergyEntity { PowerName = "用水量", Val = null, PowerType = waterPowerTypeStr });
                }

                ////气
                //i = 0;
                //IList<ChartStatisInfo> historyGasValByMonths = new List<ChartStatisInfo>();
                //IList<EnergyEntity> currentGusValByMonths = new List<EnergyEntity>();
                //ChartStatisEntity currentValByMonthsGusAllType = _analogHistoryRepos.GetEnergyStatisMonth(3, buildingID.Value, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), allGusTypes, 1).ToList().FirstOrDefault();
                //foreach (var gasSubPowerClass in gasPowerClass)
                //{
                //    IList<ChartStatisEntity> valByMonthsGas = _analogHistoryRepos.GetEnergyStatisMonth(3, buildingID.Value, DateTime.Parse(DateTime.Now.AddMonths(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), new string[] { gasSubPowerClass.PC_ID }, 1).ToList();
                //    historyGasValByMonths.Add(new ChartStatisInfo { ChartData = valByMonthsGas.OrderBy(x => x.Time).ToList(), PowerType = gasSubPowerClass });                   
                //    //foreach (var valByDaysElecAllTypeItem in valByDaysElecAllType)
                //    //{
                //    //    if (valByDaysElec.Count()>0 && valByDaysElec.Where(p => p.DTimeStr == valByDaysElecAllTypeItem.DTimeStr).Count() == 0)
                //    //    {
                //    //        valByDaysElec.Add(new ChartStatisEntity { Time = valByDaysElecAllTypeItem.Time, StatisVal = 0 });
                //    //    }
                //    //}
                //    if (valByMonthsGas.Count != 0 && historyGasValByMonths[i].ChartData[valByMonthsGas.Count - 1].MonthStr == dtmStr)
                //    {
                //        ChartStatisEntity currentChartStatisEntity = historyGasValByMonths[i].ChartData[valByMonthsGas.Count - 1];
                //        currentGusValByMonths.Add(new EnergyEntity { PowerName = currentChartStatisEntity.PowerName, Val = currentChartStatisEntity.StatisVal, PowerType = currentChartStatisEntity.PowerType });
                //    }
                //    else
                //    {
                //        currentGusValByMonths.Add(null);
                //    }
                //    i = i + 1;
                //}
                //if (currentValByMonthsGusAllType != null && currentValByMonthsGusAllType.MonthStr == dtmStr)
                //{
                //    currentGusValByMonths.Insert(0, new EnergyEntity { PowerName = "燃气电量", Val = currentValByMonthsGusAllType.StatisVal, PowerType = gasPowerTypeStr });
                //}
                //else
                //{
                //    currentGusValByMonths.Insert(0, new EnergyEntity { PowerName = "燃气电量", Val = null, PowerType = gasPowerTypeStr });
                //}
                if (currentElecValByMonths.Count > 0)
                {
                    ViewBag.powerElecMonthReals = currentElecValByMonths.ToArray();
                }
                if (currentWaterValByMonths.Count > 0)
                {
                    ViewBag.powerWaterMonthReals = currentWaterValByMonths.ToArray();
                }
                //if (currentGusValByMonths.Count > 0)
                //{
                //    ViewBag.powerGasMonthReals = currentGusValByMonths.ToArray();
                //}
                //电               
                IList<EnergyEntity> currentElecValByYears = new List<EnergyEntity>();
                string dty = DateTime.Now.Year.ToString();
                ChartStatisEntity currentValByYearsElecAllType = _analogHistoryRepos.GetEnergyStatisYear(3, buildingID.Value, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), allElecTypes, 1).ToList().FirstOrDefault();
                foreach (var elecSubType in elecPowerClass)
                {
                    ChartStatisEntity currentChartStatisEntity = _analogHistoryRepos.GetEnergyStatisYear(3, buildingID.Value, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), new string[] { elecSubType.PC_ID }, 1).ToList().FirstOrDefault();
                    //foreach (var valByMonthsElecAllTypeItem in valByMonthsElecAllType)
                    //{
                    //    if (valByMonthsElec.Count() > 0 && valByMonthsElec.Where(p => p.MTimeStr == valByMonthsElecAllTypeItem.MTimeStr).Count() == 0)
                    //    {
                    //        valByMonthsElec.Add(new ChartStatisEntity { Time = valByMonthsElecAllTypeItem.Time, StatisVal = 0 });
                    //    }
                    //}                  
                    if (currentChartStatisEntity != null && currentChartStatisEntity.YTimeStr == dty)
                    {
                        currentElecValByYears.Add(new EnergyEntity { PowerName = currentChartStatisEntity.PowerName, Val = currentChartStatisEntity.StatisVal, PowerType = currentChartStatisEntity.PowerType });
                    }
                    else
                    {
                        currentElecValByYears.Add(null);
                    }

                }
                if (currentValByYearsElecAllType != null && currentValByYearsElecAllType.YTimeStr == dty)
                {
                    currentElecValByYears.Insert(0, new EnergyEntity { PowerName = "总用电量", Val = currentValByYearsElecAllType.StatisVal, PowerType = elecPowerTypeStr });
                }
                else
                {
                    currentElecValByYears.Insert(0, new EnergyEntity { PowerName = "总用电量", Val = null, PowerType = elecPowerTypeStr });
                }

                //水               
                IList<EnergyEntity> currentWaterValByYears = new List<EnergyEntity>();
                ChartStatisEntity currentValByYearsWaterAllType = _analogHistoryRepos.GetEnergyStatisYear(3, buildingID.Value, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), allWaterTypes, 1).ToList().FirstOrDefault();
                foreach (var waterSubPowerClass in waterPowerClass)
                {
                    ChartStatisEntity currentChartStatisEntity = _analogHistoryRepos.GetEnergyStatisYear(3, buildingID.Value, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), new string[] { waterSubPowerClass.PC_ID }, 1).ToList().FirstOrDefault();
                    //foreach (var valByDaysElecAllTypeItem in valByDaysElecAllType)
                    //{
                    //    if (valByDaysElec.Count()>0 && valByDaysElec.Where(p => p.DTimeStr == valByDaysElecAllTypeItem.DTimeStr).Count() == 0)
                    //    {
                    //        valByDaysElec.Add(new ChartStatisEntity { Time = valByDaysElecAllTypeItem.Time, StatisVal = 0 });
                    //    }
                    //}                  
                    if (currentChartStatisEntity != null && currentChartStatisEntity.YTimeStr == dty)
                    {
                        currentWaterValByYears.Add(new EnergyEntity { PowerName = currentChartStatisEntity.PowerName, Val = currentChartStatisEntity.StatisVal, PowerType = currentChartStatisEntity.PowerType });
                    }
                    else
                    {
                        currentWaterValByYears.Add(null);
                    }
                }
                if (currentValByYearsWaterAllType != null && currentValByYearsWaterAllType.YTimeStr == dty)
                {
                    currentWaterValByYears.Insert(0, new EnergyEntity { PowerName = "用水量", Val = currentValByYearsWaterAllType.StatisVal, PowerType = waterPowerTypeStr });
                }
                else
                {
                    currentWaterValByYears.Insert(0, new EnergyEntity { PowerName = "用水量", Val = null, PowerType = waterPowerTypeStr });
                }
                //IList<EnergyEntity> currentGusValByYears = new List<EnergyEntity>();
                //ChartStatisEntity currentValByYearsGusAllType = _analogHistoryRepos.GetEnergyStatisYear(3, buildingID.Value, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), allGusTypes, 1).ToList().FirstOrDefault();
                //foreach (var gasSubPowerClass in gasPowerClass)
                //{
                //    ChartStatisEntity currentChartStatisEntity = _analogHistoryRepos.GetEnergyStatisYear(3, buildingID.Value, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), new string[] { gasSubPowerClass.PC_ID }, 1).ToList().FirstOrDefault();                   
                //    //{
                //    //    if (valByDaysElec.Count()>0 && valByDaysElec.Where(p => p.DTimeStr == valByDaysElecAllTypeItem.DTimeStr).Count() == 0)
                //    //    {
                //    //        valByDaysElec.Add(new ChartStatisEntity { Time = valByDaysElecAllTypeItem.Time, StatisVal = 0 });
                //    //    }
                //    //}
                //    if (currentChartStatisEntity != null && currentChartStatisEntity.YTimeStr == dty)
                //    {                        
                //        currentGusValByYears.Add(new EnergyEntity { PowerName = currentChartStatisEntity.PowerName, Val = currentChartStatisEntity.StatisVal, PowerType = currentChartStatisEntity.PowerType });
                //    }
                //    else
                //    {
                //        currentGusValByYears.Add(null);
                //    }
                //}
                //if (currentValByYearsGusAllType != null && currentValByYearsGusAllType.YTimeStr == dty)
                //{
                //    currentGusValByYears.Insert(0, new EnergyEntity { PowerName = "燃气电量", Val = currentValByYearsGusAllType.StatisVal, PowerType = gasPowerTypeStr });
                //}
                //else
                //{
                //    currentGusValByYears.Insert(0, new EnergyEntity { PowerName = "燃气电量", Val = null, PowerType = gasPowerTypeStr });
                //}
                if (currentElecValByYears.Count > 0)
                {
                    ViewBag.powerElecYearReals = currentElecValByYears.ToArray();
                }
                if (currentWaterValByYears.Count > 0)
                {
                    ViewBag.powerWaterYearReals = currentWaterValByYears.ToArray();
                }
                //if (currentGusValByYears.Count > 0)
                //{
                //    ViewBag.powerGasYearReals = currentGusValByYears.ToArray();
                //}  

                if (historyElecValByMonths != null && historyElecValByMonths.Count > 0)
                {
                    ViewBag.historyElecValByMonths = historyElecValByMonths;
                }
                if (historyWaterValByMonths != null && historyWaterValByMonths.Count > 0)
                {
                    ViewBag.historyWaterValByMonths = historyWaterValByMonths;
                }
                //if (historyGasValByMonths != null && historyGasValByMonths.Count > 0)
                //{
                //    ViewBag.historyGasValByMonths = historyGasValByMonths;
                //}
                //获取该校区内所有建筑信息
                ArrayList infos = new ArrayList();
                var areaInfos = _schoolAreaRepos.GetSchoolAllArea(campusID.Value + 1);
                foreach (var areaInfo in areaInfos)
                {
                    var buildingsInfos = _buildingRepos.GetBuildingsOfArea(areaInfo.SAI_ID).ToList<BuildingBriefInfo>();
                    AreaAndBuilding newareaAndBuilding = new AreaAndBuilding { AreaName = areaInfo.SAI_Name, AreaID = areaInfo.SAI_ID, buildingInfo = buildingsInfos };
                    infos.Add(newareaAndBuilding);
                }
                ViewBag.allBuildingInfo = infos;

                ViewBag.Cam_ID = campusID.Value;
                ViewBag.B_ID = buildingID.Value;

                var valElecThisYear = _analogHistoryRepos.GetEnergyBuildingCYear(buildingID.Value, allElecTypes);
                var valElecPlan = _becRepos.GetBuildingConsum(buildingID.Value, DateTime.Now.Year, elecType.Single());
                var remainElec = (valElecPlan == 0) ? 0 : valElecPlan - valElecThisYear;
                ViewBag.ValElecThisYear = Convert.ToDouble(valElecThisYear.ToString("#0.0"));
                ViewBag.ValElecRemain = Convert.ToDouble(remainElec.ToString("#0.0"));


                var valWaterThisYear = _analogHistoryRepos.GetEnergyBuildingCYear(buildingID.Value, waterPowerClass.Select(x => x.PC_ID).ToArray());
                var valWaterPlan = _becRepos.GetBuildingConsum(buildingID.Value, DateTime.Now.Year, "002");
                var remainWater = (valWaterPlan == 0) ? 0 : valWaterPlan - valWaterThisYear;
                ViewBag.ValWaterThisYear = Convert.ToDouble(valWaterThisYear.ToString("#0.0"));
                ViewBag.ValWaterRemain = Convert.ToDouble(remainWater.ToString("#0.0"));

                var valGasThisYear = _analogHistoryRepos.GetEnergyBuildingCYear(buildingID.Value, gasPowerClass.Select(x => x.PC_ID).ToArray());
                var valGasPlan = _becRepos.GetBuildingConsum(buildingID.Value, DateTime.Now.Year, "003");
                var remainGas = (valGasPlan == 0) ? 0 : valGasPlan - valGasThisYear;
                ViewBag.ValGasThisYear = Convert.ToDouble(valGasThisYear.ToString("#0.0"));
                ViewBag.ValGasRemain = Convert.ToDouble(remainGas.ToString("#0.0"));
            }
            return View();
        }

        /// <summary>
        /// Ajax 查询建筑过去十个月或十天的能耗
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public ActionResult GetRecentEnergyAjax(int buildingId, string powerType, string timeRange)
        {
            IList<Models.Repository.Entity.ChartStatisEntity> list = null;
            IList resultList = new ArrayList();
            string[] powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
            powerTypes = powerTypes.Union(new string[] { powerType }).ToArray();
            if (timeRange == "01")
            {
                var endMonth = System.DateTime.Now;
                var startMonth = endMonth.AddMonths(-10);
                var ids = new int?[] { buildingId };
                list = _analogHistoryRepos.GetEnergyStatisMonth(3, buildingId, startMonth, endMonth, powerTypes, 1).OrderBy(x => x.Time).ToList();
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        var newDataItem = new
                        {
                            StatisVal = item.StatisVal,
                            TimeBlock = item.MTimeStr
                        };
                        resultList.Add(newDataItem);
                    }
                }
            }
            else if (timeRange == "02")
            {
                var endDay = System.DateTime.Now;
                var startDay = endDay.AddDays(-10);
                var ids = new int?[] { buildingId };
                list = _analogHistoryRepos.GetEnergyStatisDay(3, buildingId, startDay, endDay, powerTypes, 1).OrderBy(x => x.Time).ToList();
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        var newDataItem = new
                        {
                            StatisVal = item.StatisVal,
                            TimeBlock = item.DTimeStr
                        };
                        resultList.Add(newDataItem);
                    }
                }
            }
            else if (timeRange == "03")
            {
                var endhour = System.DateTime.Now;
                var starthour = endhour.AddHours(-24);
                var ids = new int?[] { buildingId };
                list = _analogHistoryRepos.GetEnergyStatisHour(3, buildingId, starthour, endhour, powerTypes, 1).OrderBy(x => x.Time).ToList();
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        var newDataItem = new
                        {
                            StatisVal = item.StatisVal,
                            TimeBlock = item.HTimeStr
                        };
                        resultList.Add(newDataItem);
                    }
                }
            }
            String buildingName = null;
            try
            {
                buildingName = _buildingRepos.GetBuildingAndArea(buildingId).BuildingName;
            }
            catch (Exception e)
            {
                buildingName = "";
            }
            var valThisYear = _analogHistoryRepos.GetEnergyBuildingCYear(buildingId, powerTypes);
            var valPlan = _becRepos.GetBuildingConsum(buildingId, DateTime.Now.Year, powerType);
            var remain = (valPlan == 0) ? 0 : valPlan - valThisYear;
            var resultData = new
            {
                energyUsedYear = valThisYear.ToString("#0.0"),
                energyRemain = remain.ToString("#0.0"),
                queryList = resultList,
                buildingName = buildingName
            };
            return Json(resultData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获得校区下所有区域和楼宇
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        public ActionResult GetBuildingsOfArea(int schoolId)
        {
            if (schoolId > 0)
            {
                ArrayList infos = new ArrayList();
                var areaInfos = _schoolAreaRepos.GetSchoolAllArea(schoolId);
                foreach (var areaInfo in areaInfos)
                {
                    var buildingsInfos = _buildingRepos.GetBuildingsOfArea(areaInfo.SAI_ID).ToList<BuildingBriefInfo>();
                    infos.Add(new { AreaName = areaInfo.SAI_Name, AreaID = areaInfo.SAI_ID, Buildings = buildingsInfos });
                }
                return Json(infos, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 由建筑名称获得建筑
        /// </summary>
        /// <param name="buildingName"></param>
        /// <returns></returns>
        public ActionResult GetBuilding(string buildingName)
        {
            if (buildingName != null)
            {
                var resultBuilding = _buildingRepos.GetBuilding(buildingName, -1);
                return Json(resultBuilding, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        /// <summary>
        /// 获得校区下所有楼宇
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        public ActionResult GetBuildingsOfSchool(int schoolId)
        {
            if (schoolId > 0)
            {
                ArrayList infos = new ArrayList();
                var areaInfos = _schoolAreaRepos.GetSchoolAllArea(schoolId);
                foreach (var areaInfo in areaInfos)
                {
                    var buildingsInfos = _buildingRepos.GetBuildingsOfArea2(areaInfo.SAI_ID).ToList<BuildingBriefInfo>();
                    infos.AddRange(buildingsInfos);
                }
                return Json(infos, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="AllEnergy"></param>
        private void checkForChart(IList<EnergyAllTypeEntity> AllEnergy)
        {
            List<DateTime> valByHoursElecTimeList = new List<DateTime>();
            List<DateTime> valByHoursWaterTimeList = new List<DateTime>();
            List<DateTime> valByHoursGasTimeList = new List<DateTime>();
            List<DateTime> valByDaysElecTimeList = new List<DateTime>();
            List<DateTime> valByDaysWaterTimeList = new List<DateTime>();
            List<DateTime> valByDaysGasTimeList = new List<DateTime>();
            List<DateTime> valByMonthsElecTimeList = new List<DateTime>();
            List<DateTime> valByMonthsWaterTimeList = new List<DateTime>();
            List<DateTime> valByMonthsGasTimeList = new List<DateTime>();
            DateTime hourStartTime = DateTime.Parse(DateTime.Now.AddHours(-25).ToString("yyyy-MM-dd HH:00:00"));
            DateTime dayStartTime = DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd 00:00:00"));
            DateTime monthStartTime = DateTime.Parse(DateTime.Now.AddMonths(-10).ToString("yyyy-MM-dd 00:00:00"));

            foreach (var energyItem in AllEnergy)
            {
                if (energyItem.valByHoursElec != null)
                {
                    foreach (var valByHoursElec in energyItem.valByHoursElec)
                    {
                        if (!valByHoursElecTimeList.Contains(valByHoursElec.Time))
                        {
                            valByHoursElecTimeList.Add(valByHoursElec.Time);
                        }
                    }
                }
                if (energyItem.valByHoursWater != null)
                {
                    foreach (var valByHoursWater in energyItem.valByHoursWater)
                    {
                        if (!valByHoursWaterTimeList.Contains(valByHoursWater.Time))
                        {
                            valByHoursWaterTimeList.Add(valByHoursWater.Time);
                        }
                    }
                }

                if (energyItem.valByHoursGas != null)
                {
                    foreach (var valByHoursGas in energyItem.valByHoursGas)
                    {
                        if (!valByHoursGasTimeList.Contains(valByHoursGas.Time))
                        {
                            valByHoursGasTimeList.Add(valByHoursGas.Time);
                        }
                    }
                }

                if (energyItem.valByDaysElec != null)
                {
                    foreach (var valByDaysElec in energyItem.valByDaysElec)
                    {
                        if (!valByDaysElecTimeList.Contains(valByDaysElec.Time))
                        {
                            valByDaysElecTimeList.Add(valByDaysElec.Time);
                        }
                    }
                }

                if (energyItem.valByDaysWater != null)
                {
                    foreach (var valByDaysWater in energyItem.valByDaysWater)
                    {
                        if (!valByDaysWaterTimeList.Contains(valByDaysWater.Time))
                        {
                            valByDaysWaterTimeList.Add(valByDaysWater.Time);
                        }
                    }
                }

                if (energyItem.valByDaysGas != null)
                {
                    foreach (var valByDaysGas in energyItem.valByDaysGas)
                    {
                        if (!valByDaysGasTimeList.Contains(valByDaysGas.Time))
                        {
                            valByDaysGasTimeList.Add(valByDaysGas.Time);
                        }
                    }
                }

                if (energyItem.valByMonthsElec != null)
                {
                    foreach (var valByMonthsElec in energyItem.valByMonthsElec)
                    {
                        if (!valByMonthsElecTimeList.Contains(valByMonthsElec.Time))
                        {
                            valByMonthsElecTimeList.Add(valByMonthsElec.Time);
                        }
                    }
                }

                if (energyItem.valByMonthsWater != null)
                {
                    foreach (var valByMonthsWater in energyItem.valByMonthsWater)
                    {
                        if (!valByMonthsWaterTimeList.Contains(valByMonthsWater.Time))
                        {
                            valByMonthsWaterTimeList.Add(valByMonthsWater.Time);
                        }
                    }
                }

                if (energyItem.valByMonthsGas != null)
                {
                    foreach (var valByMonthsGas in energyItem.valByMonthsGas)
                    {
                        if (!valByMonthsGasTimeList.Contains(valByMonthsGas.Time))
                        {
                            valByMonthsGasTimeList.Add(valByMonthsGas.Time);
                        }
                    }
                }
            }

            foreach (var energyItem in AllEnergy)
            {
                if (energyItem.valByHoursElec != null)
                {
                    foreach (var timeItem in valByHoursElecTimeList)
                    {
                        if (energyItem.valByHoursElec.Where(p => p.Time == timeItem).Count() == 0)
                        {
                            energyItem.valByHoursElec.Add(new ChartStatisEntity { Time = timeItem, StatisVal = 0, EntityIndex = Convert.ToInt32(timeItem.Subtract(hourStartTime).TotalHours) });
                        }
                    }
                    energyItem.valByHoursElec = energyItem.valByHoursElec.OrderBy(p => p.Time).ToList();
                }
                if (energyItem.valByHoursWater != null)
                {
                    foreach (var timeItem in valByHoursWaterTimeList)
                    {
                        if (energyItem.valByHoursWater.Where(p => p.Time == timeItem).Count() == 0)
                        {
                            energyItem.valByHoursWater.Add(new ChartStatisEntity { Time = timeItem, StatisVal = 0, EntityIndex = Convert.ToInt32(timeItem.Subtract(hourStartTime).TotalHours) });
                        }
                    }
                    energyItem.valByHoursWater = energyItem.valByHoursWater.OrderBy(p => p.Time).ToList();
                }

                if (energyItem.valByHoursGas != null)
                {
                    foreach (var timeItem in valByHoursGasTimeList)
                    {
                        if (energyItem.valByHoursGas.Where(p => p.Time == timeItem).Count() == 0)
                        {
                            energyItem.valByHoursGas.Add(new ChartStatisEntity { Time = timeItem, StatisVal = 0, EntityIndex = Convert.ToInt32(timeItem.Subtract(hourStartTime).TotalHours) });
                        }
                    }
                    energyItem.valByHoursGas = energyItem.valByHoursGas.OrderBy(p => p.Time).ToList();
                }

                if (energyItem.valByDaysElec != null)
                {
                    foreach (var timeItem in valByDaysElecTimeList)
                    {
                        if (energyItem.valByDaysElec.Where(p => p.Time == timeItem).Count() == 0)
                        {
                            energyItem.valByDaysElec.Add(new ChartStatisEntity { Time = timeItem, StatisVal = 0, EntityIndex = Convert.ToInt32(timeItem.Subtract(dayStartTime).TotalDays) });
                        }
                    }
                    energyItem.valByDaysElec = energyItem.valByDaysElec.OrderBy(p => p.Time).ToList();
                }

                if (energyItem.valByDaysWater != null)
                {
                    foreach (var timeItem in valByDaysWaterTimeList)
                    {
                        if (energyItem.valByDaysWater.Where(p => p.Time == timeItem).Count() == 0)
                        {
                            energyItem.valByDaysWater.Add(new ChartStatisEntity { Time = timeItem, StatisVal = 0, EntityIndex = Convert.ToInt32(timeItem.Subtract(dayStartTime).TotalDays) });
                        }
                    }
                    energyItem.valByDaysWater = energyItem.valByDaysWater.OrderBy(p => p.Time).ToList();
                }

                if (energyItem.valByDaysGas != null)
                {
                    foreach (var timeItem in valByDaysGasTimeList)
                    {
                        if (energyItem.valByDaysGas.Where(p => p.Time == timeItem).Count() == 0)
                        {
                            energyItem.valByDaysGas.Add(new ChartStatisEntity { Time = timeItem, StatisVal = 0, EntityIndex = Convert.ToInt32(timeItem.Subtract(dayStartTime).TotalDays) });
                        }
                    }
                    energyItem.valByDaysGas = energyItem.valByDaysGas.OrderBy(p => p.Time).ToList();
                }

                if (energyItem.valByMonthsElec != null)
                {
                    foreach (var timeItem in valByMonthsElecTimeList)
                    {
                        if (energyItem.valByMonthsElec.Where(p => p.Time == timeItem).Count() == 0)
                        {
                            energyItem.valByMonthsElec.Add(new ChartStatisEntity { Time = timeItem, StatisVal = 0, EntityIndex = (timeItem.Year - monthStartTime.Year) * 12 + timeItem.Month - monthStartTime.Month });
                        }
                    }
                    energyItem.valByMonthsElec = energyItem.valByMonthsElec.OrderBy(p => p.Time).ToList();
                }

                if (energyItem.valByMonthsWater != null)
                {
                    foreach (var timeItem in valByMonthsWaterTimeList)
                    {
                        if (energyItem.valByMonthsWater.Where(p => p.Time == timeItem).Count() == 0)
                        {
                            energyItem.valByMonthsWater.Add(new ChartStatisEntity { Time = timeItem, StatisVal = 0, EntityIndex = (timeItem.Year - monthStartTime.Year) * 12 + timeItem.Month - monthStartTime.Month });
                        }
                    }
                    energyItem.valByMonthsWater = energyItem.valByMonthsWater.OrderBy(p => p.Time).ToList();
                }

                if (energyItem.valByMonthsGas != null)
                {
                    foreach (var timeItem in valByMonthsGasTimeList)
                    {
                        if (energyItem.valByMonthsGas.Where(p => p.Time == timeItem).Count() == 0)
                        {
                            energyItem.valByMonthsGas.Add(new ChartStatisEntity { Time = timeItem, StatisVal = 0, EntityIndex = (timeItem.Year - monthStartTime.Year) * 12 + timeItem.Month - monthStartTime.Month });
                        }
                    }
                    energyItem.valByMonthsGas = energyItem.valByMonthsGas.OrderBy(p => p.Time).ToList();
                }
            }
        }

        /// <summary>
        /// 多建筑能耗对比
        /// </summary>
        /// <returns></returns>
        public ActionResult EnergyCompare(string IDs, int? queryType)
        {
            string[] idStrs = IDs.Split('_');
            if (queryType == 3)
            {
                IList<EnergyAllTypeEntity> allTypeEnergy = new List<EnergyAllTypeEntity>();
                if (IDs != null && IDs.Count() > 0)
                {
                    foreach (var idStr in idStrs)
                    {
                        var buildingID = Convert.ToInt32(idStr);
                        EnergyAllTypeEntity energyItem = new EnergyAllTypeEntity();
                        energyItem.building = _buildingRepos.GetBuilding(buildingID);
                        int?[] buildings = { buildingID };
                        ArrayList elecType = new ArrayList();
                        elecType.Add("001");
                        IList<PowerClass> elecPowerClass = _powerClassRepos.GetSubPowers("001").ToList();
                        IList<EnergyEntity> powerElecReals = new List<EnergyEntity>();
                        double totalElecVal = 0;
                        foreach (var elecSubPowerClass in elecPowerClass)
                        {
                            string[] elecSubType = new string[] { elecSubPowerClass.PC_ID };
                            IList<EnergyEntity> subPowerVals = _ampRepos.GetRealEnergyByBuildingOnly(buildings, elecSubType);
                            EnergyEntity subPowerVal = subPowerVals.FirstOrDefault();
                            powerElecReals.Add(subPowerVal);
                            if (subPowerVal != null)
                            {
                                totalElecVal += subPowerVal.Val.Value;
                            }
                        }
                        powerElecReals.Insert(0, new EnergyEntity { PowerName = "用电量", Val = totalElecVal });
                        ViewBag.powerElecType = elecPowerClass.ToArray();

                        string[] waterType = { "002" };
                        IList<EnergyEntity> powerWaterReals = new List<EnergyEntity>();
                        double totalWaterVal = 0;
                        IList<PowerClass> waterPowerClass = _powerClassRepos.GetSubPowers("002").ToList();
                        foreach (var waterSubPowerClass in waterPowerClass)
                        {
                            string[] waterSubType = new string[] { waterSubPowerClass.PC_ID };
                            EnergyEntity subPowerVal = _ampRepos.GetRealEnergyByBuildingOnly(buildings, waterSubType).FirstOrDefault();
                            powerWaterReals.Add(subPowerVal);
                            if (subPowerVal != null)
                            {
                                totalWaterVal += subPowerVal.Val.Value;
                            }
                        }
                        powerWaterReals.Insert(0, new EnergyEntity { PowerName = "用水量", Val = totalWaterVal });
                        ViewBag.powerWaterType = waterPowerClass.ToArray();

                        string[] gasType = { "003" };
                        IList<EnergyEntity> powerGasReals = new List<EnergyEntity>();
                        double totalGasVal = 0;
                        IList<PowerClass> gasPowerClass = _powerClassRepos.GetSubPowers("003").ToList();
                        foreach (var gasSubPowerClass in gasPowerClass)
                        {
                            string[] gasSubType = new string[] { gasSubPowerClass.PC_ID };
                            EnergyEntity subPowerVal = _ampRepos.GetRealEnergyByBuildingOnly(buildings, gasSubType).FirstOrDefault();
                            powerGasReals.Add(subPowerVal);
                            if (subPowerVal != null)
                            {
                                totalGasVal += subPowerVal.Val.Value;
                            }
                        }
                        powerGasReals.Insert(0, new EnergyEntity { PowerName = "燃气用量", Val = totalGasVal });
                        ViewBag.powerGasType = gasPowerClass.ToArray();

                        if (powerElecReals.Count > 0)
                        {
                            energyItem.powerElecReals = powerElecReals.ToArray();
                        }
                        if (powerWaterReals.Count > 0)
                        {
                            energyItem.powerWaterReals = powerWaterReals.ToArray();
                        }
                        if (powerGasReals.Count > 0)
                        {
                            energyItem.powerGasReals = powerGasReals.ToArray();
                        }

                        //获取过去24小时内各建筑每小时能耗值
                        ArrayList powerelec = new ArrayList { "001" };
                        IList<PowerClass> elecSubClass = _powerClassRepos.GetSubPowers("001").ToList();
                        foreach (var subclass in elecSubClass)
                        {
                            powerelec.Add(subclass.PC_ID);
                        }
                        IList<Models.Repository.Entity.ChartStatisEntity> valByHoursElec = _analogHistoryRepos.GetEnergyStatisHour(3, buildingID, DateTime.Parse(DateTime.Now.AddHours(-25).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), (string[])powerelec.ToArray(typeof(string)), 1).ToList();
                        ArrayList powerwater = new ArrayList { "002" };
                        IList<PowerClass> waterSubClass = _powerClassRepos.GetSubPowers("002").ToList();
                        foreach (var subclass in waterSubClass)
                        {
                            powerwater.Add(subclass.PC_ID);
                        }
                        IList<Models.Repository.Entity.ChartStatisEntity> valByHoursWater = _analogHistoryRepos.GetEnergyStatisHour(3, buildingID, DateTime.Parse(DateTime.Now.AddHours(-25).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), (string[])powerwater.ToArray(typeof(string)), 1).ToList();
                        ArrayList powergas = new ArrayList { "003" };
                        IList<PowerClass> gasSubClass = _powerClassRepos.GetSubPowers("003").ToList();
                        foreach (var subclass in gasSubClass)
                        {
                            powergas.Add(subclass.PC_ID);
                        }
                        IList<Models.Repository.Entity.ChartStatisEntity> valByHoursGas = _analogHistoryRepos.GetEnergyStatisHour(3, buildingID, DateTime.Parse(DateTime.Now.AddHours(-25).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), (string[])powergas.ToArray(typeof(string)), 1).ToList();
                        if (valByHoursElec != null && valByHoursElec.Count > 0)
                        {
                            energyItem.valByHoursElec = valByHoursElec.OrderBy(x => x.Time).ToList();
                        }
                        if (valByHoursWater != null && valByHoursWater.Count > 0)
                        {
                            energyItem.valByHoursWater = valByHoursWater.OrderBy(x => x.Time).ToList();
                        }
                        if (valByHoursGas != null && valByHoursGas.Count > 0)
                        {
                            energyItem.valByHoursGas = valByHoursGas.OrderBy(x => x.Time).ToList();
                        }

                        //获取过去10天各建筑每天的能耗值
                        IList<Models.Repository.Entity.ChartStatisEntity> valByDaysElec = _analogHistoryRepos.GetEnergyStatisDay(3, buildingID, DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powerelec.ToArray(typeof(string)), 1).ToList();
                        IList<Models.Repository.Entity.ChartStatisEntity> valByDaysWater = _analogHistoryRepos.GetEnergyStatisDay(3, buildingID, DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powerwater.ToArray(typeof(string)), 1).ToList();
                        IList<Models.Repository.Entity.ChartStatisEntity> valByDaysGas = _analogHistoryRepos.GetEnergyStatisDay(3, buildingID, DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powergas.ToArray(typeof(string)), 1).ToList();
                        if (valByDaysElec != null && valByDaysElec.Count > 0)
                        {
                            energyItem.valByDaysElec = valByDaysElec.OrderBy(x => x.Time).ToList();
                        }
                        if (valByDaysWater != null && valByDaysWater.Count > 0)
                        {
                            energyItem.valByDaysWater = valByDaysWater.OrderBy(x => x.Time).ToList();
                        }
                        if (valByDaysGas != null && valByDaysGas.Count > 0)
                        {
                            energyItem.valByDaysGas = valByDaysGas.OrderBy(x => x.Time).ToList();
                        }

                        //获取过去十月各建筑每月的能耗值
                        IList<Models.Repository.Entity.ChartStatisEntity> valByMonthsElec = _analogHistoryRepos.GetEnergyStatisMonth(3, buildingID, DateTime.Parse(DateTime.Now.AddMonths(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powerelec.ToArray(typeof(string)), 1).ToList();
                        IList<Models.Repository.Entity.ChartStatisEntity> valByMonthsWater = _analogHistoryRepos.GetEnergyStatisMonth(3, buildingID, DateTime.Parse(DateTime.Now.AddMonths(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powerwater.ToArray(typeof(string)), 1).ToList();
                        IList<Models.Repository.Entity.ChartStatisEntity> valByMonthsGas = _analogHistoryRepos.GetEnergyStatisMonth(3, buildingID, DateTime.Parse(DateTime.Now.AddMonths(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powergas.ToArray(typeof(string)), 1).ToList();
                        if (valByMonthsElec != null && valByMonthsElec.Count > 0)
                        {
                            energyItem.valByMonthsElec = valByMonthsElec.OrderBy(x => x.Time).ToList();
                        }
                        if (valByMonthsWater != null && valByMonthsWater.Count > 0)
                        {
                            energyItem.valByMonthsWater = valByMonthsWater.OrderBy(x => x.Time).ToList();
                        }
                        if (valByMonthsGas != null && valByMonthsGas.Count > 0)
                        {
                            energyItem.valByMonthsGas = valByMonthsGas.OrderBy(x => x.Time).ToList();
                        }

                        allTypeEnergy.Add(energyItem);
                    }
                }
                //checkForChart(allTypeEnergy);
                ViewBag.allTypeEnergy = allTypeEnergy;
            }
            //查询测点类型为房间
            else if (queryType == 5)
            {
                IList<EnergyAllTypeEntity> allTypeEnergy = new List<EnergyAllTypeEntity>();
                if (IDs != null && IDs.Count() > 0)
                {
                    foreach (var idStr in idStrs)
                    {
                        var pointID = Convert.ToInt32(idStr);
                        EnergyAllTypeEntity energyItem = new EnergyAllTypeEntity();
                        energyItem.point = _ampRepos.GetAMP(pointID);
                        int?[] points = { pointID };
                        string[] elecType = { "001" };
                        IList<EnergyEntity> powerElecReals = new List<EnergyEntity>();
                        //double totalElecVal = 0;
                        IList<PowerClass> elecPowerClass = _powerClassRepos.GetSubPowers("001").ToList();
                        //foreach (var elecSubPowerClass in elecPowerClass)
                        //{
                        //    if (elecSubPowerClass.PC_ID == energyItem.point.AMP_PowerType)
                        //    {
                        //        EnergyEntity subPowerVal = ;
                        //        powerElecReals.Add(subPowerVal);
                        //    }
                        //    string[] elecSubType = new string[] { elecSubPowerClass.PC_ID };
                        //    EnergyEntity subPowerVal = _ampRepos.GetRealEnergyByRoom(rooms, elecSubType, 0, 100).FirstOrDefault();
                        //    powerElecReals.Add(subPowerVal);
                        //    if (subPowerVal != null)
                        //    {
                        //        totalElecVal += subPowerVal.Val.Value;
                        //    }
                        //}
                        powerElecReals.Insert(0, new EnergyEntity { PowerName = "用电量", Val = energyItem.point.AMP_Val });
                        if (energyItem.point.AMP_PowerType.StartsWith("001"))
                        {
                            powerElecReals.Add(new EnergyEntity { PowerName = energyItem.point.AMP_PowerName, Val = energyItem.point.AMP_Val });
                        }
                        ViewBag.powerElecType = elecPowerClass.ToArray();

                        string[] waterType = { "002" };
                        IList<EnergyEntity> powerWaterReals = new List<EnergyEntity>();
                        //double totalWaterVal = 0;
                        IList<PowerClass> waterPowerClass = _powerClassRepos.GetSubPowers("002").ToList();
                        //foreach (var waterSubPowerClass in waterPowerClass)
                        //{
                        //    string[] waterSubType = new string[] { waterSubPowerClass.PC_ID };
                        //    EnergyEntity subPowerVal = _ampRepos.GetRealEnergyByRoom(rooms, waterSubType, 0, 100).FirstOrDefault();
                        //    powerWaterReals.Add(subPowerVal);
                        //    if (subPowerVal != null)
                        //    {
                        //        totalWaterVal += subPowerVal.Val.Value;
                        //    }
                        //}
                        if (energyItem.point.AMP_PowerType.StartsWith("002"))
                        {
                            powerWaterReals.Add(new EnergyEntity { PowerName = energyItem.point.AMP_PowerName, Val = energyItem.point.AMP_Val });
                        }
                        powerWaterReals.Insert(0, new EnergyEntity { PowerName = "用水量", Val = energyItem.point.AMP_Val });
                        ViewBag.powerWaterType = waterPowerClass.ToArray();

                        string[] gasType = { "003" };
                        IList<EnergyEntity> powerGasReals = new List<EnergyEntity>();
                        //double totalGasVal = 0;
                        IList<PowerClass> gasPowerClass = _powerClassRepos.GetSubPowers("003").ToList();
                        //foreach (var gasSubPowerClass in gasPowerClass)
                        //{
                        //    string[] gasSubType = new string[] { gasSubPowerClass.PC_ID };
                        //    EnergyEntity subPowerVal = _ampRepos.GetRealEnergyByRoom(rooms, gasSubType, 0, 100).FirstOrDefault();
                        //    powerGasReals.Add(subPowerVal);
                        //    if (subPowerVal != null)
                        //    {
                        //        totalGasVal += subPowerVal.Val.Value;
                        //    }
                        //}
                        if (energyItem.point.AMP_PowerType.StartsWith("003"))
                        {
                            powerGasReals.Add(new EnergyEntity { PowerName = energyItem.point.AMP_PowerName, Val = energyItem.point.AMP_Val });
                        }
                        powerGasReals.Insert(0, new EnergyEntity { PowerName = "燃气用量", Val = energyItem.point.AMP_Val });
                        ViewBag.powerGasType = gasPowerClass.ToArray();
                        if (powerElecReals.Count > 0)
                        {
                            energyItem.powerElecReals = powerElecReals.ToArray();
                        }
                        if (powerWaterReals.Count > 0)
                        {
                            energyItem.powerWaterReals = powerWaterReals.ToArray();
                        }
                        if (powerGasReals.Count > 0)
                        {
                            energyItem.powerGasReals = powerGasReals.ToArray();
                        }

                        //获取过去24小时内各房间每小时能耗值
                        ArrayList powerelec = new ArrayList { "001" };
                        IList<PowerClass> elecSubClass = _powerClassRepos.GetSubPowers("001").ToList();
                        foreach (var subclass in elecSubClass)
                        {
                            powerelec.Add(subclass.PC_ID);
                        }
                        IList<Models.Repository.Entity.ChartStatisEntity> valByHoursElec = _analogHistoryRepos.GetEnergyStatisHour(5, pointID, DateTime.Parse(DateTime.Now.AddHours(-25).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), (string[])powerelec.ToArray(typeof(string)), 1).ToList();
                        ArrayList powerwater = new ArrayList { "002" };
                        IList<PowerClass> waterSubClass = _powerClassRepos.GetSubPowers("002").ToList();
                        foreach (var subclass in waterSubClass)
                        {
                            powerwater.Add(subclass.PC_ID);
                        }
                        IList<Models.Repository.Entity.ChartStatisEntity> valByHoursWater = _analogHistoryRepos.GetEnergyStatisHour(5, pointID, DateTime.Parse(DateTime.Now.AddHours(-25).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), (string[])powerwater.ToArray(typeof(string)), 1).ToList();
                        ArrayList powergas = new ArrayList { "003" };
                        IList<PowerClass> gasSubClass = _powerClassRepos.GetSubPowers("003").ToList();
                        foreach (var subclass in gasSubClass)
                        {
                            powergas.Add(subclass.PC_ID);
                        }
                        IList<Models.Repository.Entity.ChartStatisEntity> valByHoursGas = _analogHistoryRepos.GetEnergyStatisHour(5, pointID, DateTime.Parse(DateTime.Now.AddHours(-25).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), (string[])powergas.ToArray(typeof(string)), 1).ToList();
                        if (valByHoursElec != null && valByHoursElec.Count > 0)
                        {
                            energyItem.valByHoursElec = valByHoursElec.OrderBy(x => x.Time).ToList();
                        }
                        if (valByHoursWater != null && valByHoursWater.Count > 0)
                        {
                            energyItem.valByHoursWater = valByHoursWater.OrderBy(x => x.Time).ToList();
                        }
                        if (valByHoursGas != null && valByHoursGas.Count > 0)
                        {
                            energyItem.valByHoursGas = valByHoursGas.OrderBy(x => x.Time).ToList();
                        }

                        //获取过去10天各房间每天的能耗值
                        IList<Models.Repository.Entity.ChartStatisEntity> valByDaysElec = _analogHistoryRepos.GetEnergyStatisDay(5, pointID, DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powerelec.ToArray(typeof(string)), 1).ToList();
                        IList<Models.Repository.Entity.ChartStatisEntity> valByDaysWater = _analogHistoryRepos.GetEnergyStatisDay(5, pointID, DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powerwater.ToArray(typeof(string)), 1).ToList();
                        IList<Models.Repository.Entity.ChartStatisEntity> valByDaysGas = _analogHistoryRepos.GetEnergyStatisDay(5, pointID, DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powergas.ToArray(typeof(string)), 1).ToList();
                        if (valByDaysElec != null && valByDaysElec.Count > 0)
                        {
                            energyItem.valByDaysElec = valByDaysElec.OrderBy(x => x.Time).ToList();
                        }
                        if (valByDaysWater != null && valByDaysWater.Count > 0)
                        {
                            energyItem.valByDaysWater = valByDaysWater.OrderBy(x => x.Time).ToList();
                        }
                        if (valByDaysGas != null && valByDaysGas.Count > 0)
                        {
                            energyItem.valByDaysGas = valByDaysGas.OrderBy(x => x.Time).ToList();
                        }

                        //获取过去十月各建筑每天的能耗值
                        IList<Models.Repository.Entity.ChartStatisEntity> valByMonthsElec = _analogHistoryRepos.GetEnergyStatisMonth(5, pointID, DateTime.Parse(DateTime.Now.AddMonths(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powerelec.ToArray(typeof(string)), 1).ToList();
                        IList<Models.Repository.Entity.ChartStatisEntity> valByMonthsWater = _analogHistoryRepos.GetEnergyStatisMonth(5, pointID, DateTime.Parse(DateTime.Now.AddMonths(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powerwater.ToArray(typeof(string)), 1).ToList();
                        IList<Models.Repository.Entity.ChartStatisEntity> valByMonthsGas = _analogHistoryRepos.GetEnergyStatisMonth(5, pointID, DateTime.Parse(DateTime.Now.AddMonths(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powergas.ToArray(typeof(string)), 1).ToList();
                        if (valByMonthsElec != null && valByMonthsElec.Count > 0)
                        {
                            energyItem.valByMonthsElec = valByMonthsElec.OrderBy(x => x.Time).ToList();
                        }
                        if (valByMonthsWater != null && valByMonthsWater.Count > 0)
                        {
                            energyItem.valByMonthsWater = valByMonthsWater.OrderBy(x => x.Time).ToList();
                        }
                        if (valByMonthsGas != null && valByMonthsGas.Count > 0)
                        {
                            energyItem.valByMonthsGas = valByMonthsGas.OrderBy(x => x.Time).ToList();
                        }
                        allTypeEnergy.Add(energyItem);
                    }
                    //checkForChart(allTypeEnergy);
                    ViewBag.allTypeEnergy = allTypeEnergy;
                }
            }
            //查询类别为区域
            else if (queryType == 2)
            {
                IList<EnergyAllTypeEntity> allTypeEnergy = new List<EnergyAllTypeEntity>();
                if (IDs != null && IDs.Count() > 0)
                {
                    foreach (var idStr in idStrs)
                    {
                        var areaID = Convert.ToInt32(idStr);
                        EnergyAllTypeEntity energyItem = new EnergyAllTypeEntity();
                        energyItem.area = _schoolAreaRepos.GetAreaAndSchool(areaID);
                        int?[] areas = { areaID };
                        string[] elecType = { "001" };
                        IList<EnergyEntity> powerElecReals = new List<EnergyEntity>();
                        double totalElecVal = 0;
                        IList<PowerClass> elecPowerClass = _powerClassRepos.GetSubPowers("001").ToList();
                        foreach (var elecSubPowerClass in elecPowerClass)
                        {
                            string[] elecSubType = new string[] { elecSubPowerClass.PC_ID };
                            EnergyEntity subPowerVal = _ampRepos.GetRealEnergyByArea(areas, elecSubType, 0, 100).FirstOrDefault();
                            powerElecReals.Add(subPowerVal);
                            if (subPowerVal != null)
                            {
                                totalElecVal += subPowerVal.Val.Value;
                            }
                        }
                        powerElecReals.Insert(0, new EnergyEntity { PowerName = "用电量", Val = totalElecVal });
                        ViewBag.powerElecType = elecPowerClass.ToArray();

                        string[] waterType = { "002" };
                        IList<EnergyEntity> powerWaterReals = new List<EnergyEntity>();
                        double totalWaterVal = 0;
                        IList<PowerClass> waterPowerClass = _powerClassRepos.GetSubPowers("002").ToList();
                        foreach (var waterSubPowerClass in waterPowerClass)
                        {
                            string[] waterSubType = new string[] { waterSubPowerClass.PC_ID };

                            EnergyEntity subPowerVal = _ampRepos.GetRealEnergyByArea(areas, waterSubType, 0, 100).FirstOrDefault();
                            powerWaterReals.Add(subPowerVal);
                            if (subPowerVal != null)
                            {
                                totalWaterVal += subPowerVal.Val.Value;
                            }
                        }
                        powerWaterReals.Insert(0, new EnergyEntity { PowerName = "用水量", Val = totalWaterVal });
                        ViewBag.powerWaterType = waterPowerClass.ToArray();

                        string[] gasType = { "003" };
                        IList<EnergyEntity> powerGasReals = new List<EnergyEntity>();
                        double totalGasVal = 0;
                        IList<PowerClass> gasPowerClass = _powerClassRepos.GetSubPowers("003").ToList();
                        foreach (var gasSubPowerClass in gasPowerClass)
                        {
                            string[] gasSubType = new string[] { gasSubPowerClass.PC_ID };
                            EnergyEntity subPowerVal = _ampRepos.GetRealEnergyByArea(areas, gasSubType, 0, 100).FirstOrDefault();
                            powerGasReals.Add(subPowerVal);
                            if (subPowerVal != null)
                            {
                                totalGasVal += subPowerVal.Val.Value;
                            }
                        }
                        powerGasReals.Insert(0, new EnergyEntity { PowerName = "燃气用量", Val = totalGasVal });
                        ViewBag.powerGasType = gasPowerClass.ToArray();
                        if (powerElecReals.Count > 0)
                        {
                            energyItem.powerElecReals = powerElecReals.ToArray();
                        }
                        if (powerWaterReals.Count > 0)
                        {
                            energyItem.powerWaterReals = powerWaterReals.ToArray();
                        }
                        if (powerGasReals.Count > 0)
                        {
                            energyItem.powerGasReals = powerGasReals.ToArray();
                        }

                        //获取过去24小时内各区域每小时能耗值
                        ArrayList powerelec = new ArrayList { "001" };
                        IList<PowerClass> elecSubClass = _powerClassRepos.GetSubPowers("001").ToList();
                        foreach (var subclass in elecSubClass)
                        {
                            powerelec.Add(subclass.PC_ID);
                        }
                        IList<Models.Repository.Entity.ChartStatisEntity> valByHoursElec = _analogHistoryRepos.GetEnergyStatisHour(2, areaID, DateTime.Parse(DateTime.Now.AddHours(-25).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), (string[])powerelec.ToArray(typeof(string)), 1).ToList();
                        ArrayList powerwater = new ArrayList { "002" };
                        IList<PowerClass> waterSubClass = _powerClassRepos.GetSubPowers("002").ToList();
                        foreach (var subclass in waterSubClass)
                        {
                            powerwater.Add(subclass.PC_ID);
                        }
                        IList<Models.Repository.Entity.ChartStatisEntity> valByHoursWater = _analogHistoryRepos.GetEnergyStatisHour(2, areaID, DateTime.Parse(DateTime.Now.AddHours(-25).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), (string[])powerwater.ToArray(typeof(string)), 1).ToList();
                        ArrayList powergas = new ArrayList { "003" };
                        IList<PowerClass> gasSubClass = _powerClassRepos.GetSubPowers("003").ToList();
                        foreach (var subclass in gasSubClass)
                        {
                            powergas.Add(subclass.PC_ID);
                        }
                        IList<Models.Repository.Entity.ChartStatisEntity> valByHoursGas = _analogHistoryRepos.GetEnergyStatisHour(2, areaID, DateTime.Parse(DateTime.Now.AddHours(-25).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), (string[])powergas.ToArray(typeof(string)), 1).ToList();
                        if (valByHoursElec != null && valByHoursElec.Count > 0)
                        {
                            energyItem.valByHoursElec = valByHoursElec.OrderBy(x => x.Time).ToList();
                        }
                        if (valByHoursWater != null && valByHoursWater.Count > 0)
                        {
                            energyItem.valByHoursWater = valByHoursWater.OrderBy(x => x.Time).ToList();
                        }
                        if (valByHoursGas != null && valByHoursGas.Count > 0)
                        {
                            energyItem.valByHoursGas = valByHoursGas.OrderBy(x => x.Time).ToList();
                        }

                        //获取过去10天各区域每天的能耗值
                        IList<Models.Repository.Entity.ChartStatisEntity> valByDaysElec = _analogHistoryRepos.GetEnergyStatisDay(2, areaID, DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powerelec.ToArray(typeof(string)), 1).ToList();
                        IList<Models.Repository.Entity.ChartStatisEntity> valByDaysWater = _analogHistoryRepos.GetEnergyStatisDay(2, areaID, DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powerwater.ToArray(typeof(string)), 1).ToList();
                        IList<Models.Repository.Entity.ChartStatisEntity> valByDaysGas = _analogHistoryRepos.GetEnergyStatisDay(2, areaID, DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powergas.ToArray(typeof(string)), 1).ToList();
                        if (valByDaysElec != null && valByDaysElec.Count > 0)
                        {
                            energyItem.valByDaysElec = valByDaysElec.OrderBy(x => x.Time).ToList();
                        }
                        if (valByDaysWater != null && valByDaysWater.Count > 0)
                        {
                            energyItem.valByDaysWater = valByDaysWater.OrderBy(x => x.Time).ToList();
                        }
                        if (valByDaysGas != null && valByDaysGas.Count > 0)
                        {
                            energyItem.valByDaysGas = valByDaysGas.OrderBy(x => x.Time).ToList();
                        }

                        //获取过去十月各区域每天的能耗值
                        IList<Models.Repository.Entity.ChartStatisEntity> valByMonthsElec = _analogHistoryRepos.GetEnergyStatisMonth(2, areaID, DateTime.Parse(DateTime.Now.AddMonths(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powerelec.ToArray(typeof(string)), 1).ToList();
                        IList<Models.Repository.Entity.ChartStatisEntity> valByMonthsWater = _analogHistoryRepos.GetEnergyStatisMonth(2, areaID, DateTime.Parse(DateTime.Now.AddMonths(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powerwater.ToArray(typeof(string)), 1).ToList();
                        IList<Models.Repository.Entity.ChartStatisEntity> valByMonthsGas = _analogHistoryRepos.GetEnergyStatisMonth(2, areaID, DateTime.Parse(DateTime.Now.AddMonths(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powergas.ToArray(typeof(string)), 1).ToList();
                        if (valByMonthsElec != null && valByMonthsElec.Count > 0)
                        {
                            energyItem.valByMonthsElec = valByMonthsElec.OrderBy(x => x.Time).ToList();
                        }
                        if (valByMonthsWater != null && valByMonthsWater.Count > 0)
                        {
                            energyItem.valByMonthsWater = valByMonthsWater.OrderBy(x => x.Time).ToList();
                        }
                        if (valByMonthsGas != null && valByMonthsGas.Count > 0)
                        {
                            energyItem.valByMonthsGas = valByMonthsGas.OrderBy(x => x.Time).ToList();
                        }
                        allTypeEnergy.Add(energyItem);
                    }
                }
                //checkForChart(allTypeEnergy);
                ViewBag.allTypeEnergy = allTypeEnergy;
            }
            //查询类型为校区
            else if (queryType == 1)
            {
                IList<EnergyAllTypeEntity> allTypeEnergy = new List<EnergyAllTypeEntity>();
                if (IDs != null && IDs.Count() > 0)
                {
                    foreach (var idStr in idStrs)
                    {
                        var schoolID = Convert.ToInt32(idStr);
                        EnergyAllTypeEntity energyItem = new EnergyAllTypeEntity();
                        energyItem.school = _schoolAreaRepos.GetSchool(schoolID);
                        int?[] schools = { schoolID };
                        string[] elecType = { "001" };
                        IList<EnergyEntity> powerElecReals = new List<EnergyEntity>();
                        double totalElecVal = 0;
                        IList<PowerClass> elecPowerClass = _powerClassRepos.GetSubPowers("001").ToList();
                        foreach (var elecSubPowerClass in elecPowerClass)
                        {
                            string[] elecSubType = new string[] { elecSubPowerClass.PC_ID };
                            EnergyEntity subPowerVal = _ampRepos.GetRealEnergyBySchool(schools, elecSubType, 0, 100).FirstOrDefault();
                            powerElecReals.Add(subPowerVal);
                            if (subPowerVal != null)
                            {
                                totalElecVal += subPowerVal.Val.Value;
                            }
                        }
                        powerElecReals.Insert(0, new EnergyEntity { PowerName = "用电量", Val = totalElecVal });
                        ViewBag.powerElecType = elecPowerClass.ToArray();

                        string[] waterType = { "002" };
                        IList<EnergyEntity> powerWaterReals = new List<EnergyEntity>();
                        double totalWaterVal = 0;
                        IList<PowerClass> waterPowerClass = _powerClassRepos.GetSubPowers("002").ToList();
                        foreach (var waterSubPowerClass in waterPowerClass)
                        {
                            string[] waterSubType = new string[] { waterSubPowerClass.PC_ID };

                            EnergyEntity subPowerVal = _ampRepos.GetRealEnergyBySchool(schools, waterSubType, 0, 100).FirstOrDefault();
                            powerWaterReals.Add(subPowerVal);
                            if (subPowerVal != null)
                            {
                                totalWaterVal += subPowerVal.Val.Value;
                            }
                        }
                        powerWaterReals.Insert(0, new EnergyEntity { PowerName = "用水量", Val = totalWaterVal });
                        ViewBag.powerWaterType = waterPowerClass.ToArray();

                        string[] gasType = { "003" };
                        IList<EnergyEntity> powerGasReals = new List<EnergyEntity>();
                        double totalGasVal = 0;
                        IList<PowerClass> gasPowerClass = _powerClassRepos.GetSubPowers("003").ToList();
                        foreach (var gasSubPowerClass in gasPowerClass)
                        {
                            string[] gasSubType = new string[] { gasSubPowerClass.PC_ID };
                            EnergyEntity subPowerVal = _ampRepos.GetRealEnergyBySchool(schools, gasSubType, 0, 100).FirstOrDefault();
                            powerGasReals.Add(subPowerVal);
                            if (subPowerVal != null)
                            {
                                totalGasVal += subPowerVal.Val.Value;
                            }
                        }
                        powerGasReals.Insert(0, new EnergyEntity { PowerName = "燃气用量", Val = totalGasVal });
                        ViewBag.powerGasType = gasPowerClass.ToArray();
                        if (powerElecReals.Count > 0)
                        {
                            energyItem.powerElecReals = powerElecReals.ToArray();
                        }
                        if (powerWaterReals.Count > 0)
                        {
                            energyItem.powerWaterReals = powerWaterReals.ToArray();
                        }
                        if (powerGasReals.Count > 0)
                        {
                            energyItem.powerGasReals = powerGasReals.ToArray();
                        }

                        //获取过去24小时内各校区每小时能耗值
                        ArrayList powerelec = new ArrayList { "001" };
                        IList<PowerClass> elecSubClass = _powerClassRepos.GetSubPowers("001").ToList();
                        foreach (var subclass in elecSubClass)
                        {
                            powerelec.Add(subclass.PC_ID);
                        }
                        IList<Models.Repository.Entity.ChartStatisEntity> valByHoursElec = _analogHistoryRepos.GetEnergyStatisHour(1, schoolID, DateTime.Parse(DateTime.Now.AddHours(-25).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), (string[])powerelec.ToArray(typeof(string)), 1).ToList();
                        ArrayList powerwater = new ArrayList { "002" };
                        IList<PowerClass> waterSubClass = _powerClassRepos.GetSubPowers("002").ToList();
                        foreach (var subclass in waterSubClass)
                        {
                            powerwater.Add(subclass.PC_ID);
                        }
                        IList<Models.Repository.Entity.ChartStatisEntity> valByHoursWater = _analogHistoryRepos.GetEnergyStatisHour(1, schoolID, DateTime.Parse(DateTime.Now.AddHours(-25).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), (string[])powerwater.ToArray(typeof(string)), 1).ToList();
                        ArrayList powergas = new ArrayList { "003" };
                        IList<PowerClass> gasSubClass = _powerClassRepos.GetSubPowers("003").ToList();
                        foreach (var subclass in gasSubClass)
                        {
                            powergas.Add(subclass.PC_ID);
                        }
                        IList<Models.Repository.Entity.ChartStatisEntity> valByHoursGas = _analogHistoryRepos.GetEnergyStatisHour(1, schoolID, DateTime.Parse(DateTime.Now.AddHours(-25).ToString("yyyy-MM-dd HH:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")), (string[])powergas.ToArray(typeof(string)), 1).ToList();
                        if (valByHoursElec != null && valByHoursElec.Count > 0)
                        {
                            energyItem.valByHoursElec = valByHoursElec.OrderBy(x => x.Time).ToList();
                        }
                        if (valByHoursWater != null && valByHoursWater.Count > 0)
                        {
                            energyItem.valByHoursWater = valByHoursWater.OrderBy(x => x.Time).ToList();
                        }
                        if (valByHoursGas != null && valByHoursGas.Count > 0)
                        {
                            energyItem.valByHoursGas = valByHoursGas.OrderBy(x => x.Time).ToList();
                        }

                        //获取过去10天各校区每天的能耗值
                        IList<Models.Repository.Entity.ChartStatisEntity> valByDaysElec = _analogHistoryRepos.GetEnergyStatisDay(1, schoolID, DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powerelec.ToArray(typeof(string)), 1).ToList();
                        IList<Models.Repository.Entity.ChartStatisEntity> valByDaysWater = _analogHistoryRepos.GetEnergyStatisDay(1, schoolID, DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powerwater.ToArray(typeof(string)), 1).ToList();
                        IList<Models.Repository.Entity.ChartStatisEntity> valByDaysGas = _analogHistoryRepos.GetEnergyStatisDay(1, schoolID, DateTime.Parse(DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powergas.ToArray(typeof(string)), 1).ToList();
                        if (valByDaysElec != null && valByDaysElec.Count > 0)
                        {
                            energyItem.valByDaysElec = valByDaysElec.OrderBy(x => x.Time).ToList();
                        }
                        if (valByDaysWater != null && valByDaysWater.Count > 0)
                        {
                            energyItem.valByDaysWater = valByDaysWater.OrderBy(x => x.Time).ToList();
                        }
                        if (valByDaysGas != null && valByDaysGas.Count > 0)
                        {
                            energyItem.valByDaysGas = valByDaysGas.OrderBy(x => x.Time).ToList();
                        }

                        //获取过去十月各校区每天的能耗值
                        IList<Models.Repository.Entity.ChartStatisEntity> valByMonthsElec = _analogHistoryRepos.GetEnergyStatisMonth(1, schoolID, DateTime.Parse(DateTime.Now.AddMonths(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powerelec.ToArray(typeof(string)), 1).ToList();
                        IList<Models.Repository.Entity.ChartStatisEntity> valByMonthsWater = _analogHistoryRepos.GetEnergyStatisMonth(1, schoolID, DateTime.Parse(DateTime.Now.AddMonths(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powerwater.ToArray(typeof(string)), 1).ToList();
                        IList<Models.Repository.Entity.ChartStatisEntity> valByMonthsGas = _analogHistoryRepos.GetEnergyStatisMonth(1, schoolID, DateTime.Parse(DateTime.Now.AddMonths(-10).ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), (string[])powergas.ToArray(typeof(string)), 1).ToList();
                        if (valByMonthsElec != null && valByMonthsElec.Count > 0)
                        {
                            energyItem.valByMonthsElec = valByMonthsElec.OrderBy(x => x.Time).ToList();
                        }
                        if (valByMonthsWater != null && valByMonthsWater.Count > 0)
                        {
                            energyItem.valByMonthsWater = valByMonthsWater.OrderBy(x => x.Time).ToList();
                        }
                        if (valByMonthsGas != null && valByMonthsGas.Count > 0)
                        {
                            energyItem.valByMonthsGas = valByMonthsGas.OrderBy(x => x.Time).ToList();
                        }
                        allTypeEnergy.Add(energyItem);
                    }
                }
                //checkForChart(allTypeEnergy);
                ViewBag.allTypeEnergy = allTypeEnergy;
            }
            ViewBag.queryType = queryType;
            return View();
        }

        /// <summary>
        /// 获得保存的能耗对比建筑条目
        /// </summary>
        public ActionResult GetSavedCmpItemsAjax()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/Content/config/savedCmp.xml"));
            XmlElement items = doc.DocumentElement;
            ArrayList savedItems = new ArrayList();
            foreach (XmlNode item in items.ChildNodes)
            {
                savedItems.Add(new { title = item.ChildNodes.Item(0).InnerText, value = item.ChildNodes.Item(1).InnerText, queryType = item.ChildNodes.Item(2).InnerText });
            }
            return Json(savedItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加保存的能耗对比建筑条目
        /// </summary>
        /// <param name="title"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult AddSavedCmpItemsAjax(string title, string value, string queryType)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/Content/config/savedCmp.xml"));
            XmlElement items = doc.DocumentElement;
            XmlElement newItem = doc.CreateElement("item");
            XmlElement newItemTitle = doc.CreateElement("title");
            newItemTitle.InnerText = title;
            XmlElement newItemValue = doc.CreateElement("value");
            newItemValue.InnerText = value;
            XmlElement newItemQueryType = doc.CreateElement("queryType");
            newItemQueryType.InnerText = queryType;
            newItem.AppendChild(newItemTitle);
            newItem.AppendChild(newItemValue);
            newItem.AppendChild(newItemQueryType);
            items.AppendChild(newItem);
            doc.Save(Server.MapPath("~/Content/config/savedCmp.xml"));
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除能耗对比建筑条目
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public ActionResult RemoveSavedCmpItemAjax(string title)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/Content/config/savedCmp.xml"));
            string path = "descendant::item[title='" + title + "']";
            XmlNode removedItem = doc.SelectSingleNode(path);
            doc.DocumentElement.RemoveChild(removedItem);
            doc.Save(Server.MapPath("~/Content/config/savedCmp.xml"));
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询测点
        /// </summary>
        /// <param name="pointID"></param>
        /// <param name="pointName"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="powerType"></param>
        /// <param name="realFlag"></param>
        /// <param name="statFlag"></param>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public ActionResult QueryPointAjax(string pointID, string pointName, int? objType, int? objIDs, string powerType, int? realFlag, int? statFlag, int currentPage, int totalPages)
        {
            if (Request.IsAjaxRequest())
            {
                var query = _ampRepos.GetAllAMP();
                if (objType.HasValue && objType != 0)
                {
                    switch (objType)
                    {
                        case 1:
                            query = query.Where(x => x.SchoolID == objIDs);
                            break;
                        case 2:
                            query = query.Where(x => x.AreaID == objIDs);
                            break;
                        case 3:
                            query = query.Where(x => x.BuildingID == objIDs);
                            break;
                        case 4:
                            query = query.Where(x => x.RoomID == objIDs);
                            break;
                        default:
                            break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(pointID))
                {
                    int pointIDInt = 0;
                    Int32.TryParse(pointID, out pointIDInt);
                    query = query.Where(x => x.PNO == pointIDInt);
                }
                if (!string.IsNullOrWhiteSpace(pointName))
                {
                    query = query.Where(x => x.PName.Contains(pointName));
                }
                if (!string.IsNullOrWhiteSpace(powerType))
                {
                    string[] powerTypes = powerType.Split(new char[] { '_' });
                    query = query.Where(x => powerTypes.Contains(x.PowerType));
                }
                if (realFlag.HasValue && realFlag != -1)
                {
                    query = query.Where(x => x.RealFlag == realFlag);
                }
                if (statFlag.HasValue && statFlag != -1)
                {
                    query = query.Where(x => x.StatFlag == statFlag);
                }
                Pager pager = null;
                if (totalPages == -1)
                {
                    int totalRows = query.Count();
                    pager = new Pager(1, totalRows);
                }
                else
                {
                    pager = new Pager(currentPage, totalPages, false);
                }
                if (pager.TotalPages > 0)
                {
                    query = query.Skip(pager.StartRow).Take(pager.PageSize);
                    var list = query.ToList();
                    var resultData = new
                    {
                        totalPages = pager.TotalPages,
                        data = list
                    };
                    return Json(resultData, JsonRequestBehavior.AllowGet);
                }
                return Json(new { totalPages = pager.TotalPages }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 使用Ajax得到校区信息
        /// </summary>
        /// <returns>Json格式所有校区信息</returns>
        public ActionResult GetAllShoolAjax()
        {
            if (Request.IsAjaxRequest())
            {
                var schools = _roomRepos.GetAllSchool();
                IList list = new ArrayList();
                foreach (var item in schools)
                {
                    list.Add(new
                    {
                        dataID = item.SI_ID,
                        dataValue = item.SI_Name
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 使用Ajax根据校区ID得到区域
        /// </summary>
        /// <param name="schoolID">校区ID</param>
        /// <returns>Json格式区域集合</returns>
        public ActionResult GetAreasBySchoolIDAjax(int schoolID)
        {
            if (Request.IsAjaxRequest() && schoolID > 0)
            {
                var areas = _roomRepos.GetAreaBySchoolID(schoolID);
                IList list = new ArrayList();
                foreach (var item in areas)
                {
                    list.Add(new
                    {
                        dataID = item.SAI_ID,
                        dataValue = item.SAI_Name
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 根据区域得到建筑
        /// </summary>
        /// <param name="areaID">区域ID</param>
        /// <returns></returns>
        public ActionResult GetBuildingByAreaAjax(int areaID)
        {
            if (Request.IsAjaxRequest() && areaID > 0)
            {
                var buildings = _roomRepos.GetBuildingByAreaID(areaID);
                IList list = new ArrayList();
                foreach (var item in buildings)
                {
                    list.Add(new
                    {
                        dataID = item.BDI_ID,
                        dataValue = item.BDI_Name
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 根据建筑ID得到所有房间
        /// </summary>
        /// <param name="buildingID">建筑ID</param>
        /// <returns></returns>
        public ActionResult GetRoomsByBIDAjax(int buildingID)
        {
            if (Request.IsAjaxRequest() && buildingID > 0)
            {
                var rooms = _roomRepos.GetRooomByBuildingID(buildingID);
                IList list = new ArrayList();
                foreach (var item in rooms)
                {
                    list.Add(new
                    {
                        dataID = item.RI_ID,
                        dataValue = item.RI_RoomCode
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult GetPointsBySchoolAjax(int schoolID)
        {
            if (Request.IsAjaxRequest() && schoolID > 0)
            {
                List<AnalogMeasurePoint> amps = _ampRepos.GetAMPbySchoolID(schoolID);
                if (amps != null && amps.Count > 0)
                {
                    IList result = new ArrayList();
                    foreach (var item in amps)
                    {
                        result.Add(new
                        {
                            dataID = item.AMP_AnalogNo,
                            powerType = item.AMP_PowerName,
                            updateTime = item.AMP_Date.ToString("yyyy-MM-dd HH:mm:ss"),
                            dataValue = item.AMP_Name
                        });
                    }
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        //和HomeController中Welcome函数相同
        public ActionResult GetHomePageData()
        {
            IList resultList = HomePageData();
            return Json(new { resultList = resultList }, JsonRequestBehavior.AllowGet);
        }

        IList HomePageData()
        {
            IList<ChartStatisEntity> yearList = null;
            IList<ChartStatisEntity> monthList = null;
            IList<ChartStatisEntity> dayList = null;
            DateTime currentTime = DateTime.Now;

            String[] powerTypes = null;
            int i = 0;
            powerTypes = null;
            IList resultList = new ArrayList();
            IList nameList = new ArrayList();
            IList yearConsumption = new ArrayList();
            IList yearConsumptionEach = new ArrayList();
            IList yearConsumptionEachPerson = new ArrayList();
            IList monthConsumption = new ArrayList();
            IList monthConsumptionEach = new ArrayList();
            IList monthConsumptionEachPerson = new ArrayList();
            IList dayConsumption = new ArrayList();
            IList dayConsumptionEach = new ArrayList();
            IList dayConsumptionEachPerson = new ArrayList();
            var schoolList = _schoolRepos.QueryAllSchool();
            nameList.Add("当年总用电量");
            nameList.Add("当年单位建筑面积用电量");
            nameList.Add("当年人均用电量");
            nameList.Add("当月用电量");
            nameList.Add("当月单位建筑面积用电量");
            nameList.Add("当月人均用电量");
            nameList.Add("当天用电量");
            nameList.Add("当天单位建筑面积用电量");
            nameList.Add("当天人均用电量");
            var schoolMeasurePoint = _analogMeasurePointRepos.GetAllSchoolMeasurePoint().ToArray();
            foreach (var item in schoolList)
            {
                yearList = _analogHistoryRepos.GetEnergyStatisYear(5, schoolMeasurePoint[i], currentTime, currentTime, powerTypes, 0);

            }
            foreach (var item in schoolList)
            {
                var schoolName = item.SI_Name;
                var buildingArea = item.SI_BuildingArea;
                var numOfPeople = item.SI_Remark;
                yearList = _analogHistoryRepos.GetEnergyStatisYear(5, schoolMeasurePoint[i], currentTime, currentTime, powerTypes, 0);
                if (yearList.Count == 1)
                {
                    yearConsumption.Add((yearList[0].StatisVal).ToString("f2"));
                    if (buildingArea > 0)
                    {
                        yearConsumptionEach.Add((Convert.ToDouble(yearConsumption[i]) / Convert.ToDouble(buildingArea)).ToString("f2"));
                    }
                    else
                    {
                        yearConsumptionEach.Add("0");
                    }
                    yearConsumptionEachPerson.Add((Convert.ToDouble(yearConsumption[i]) / Convert.ToDouble(numOfPeople)).ToString("f2"));
                    monthList = _analogHistoryRepos.GetEnergyStatisMonth(5, schoolMeasurePoint[i], currentTime, currentTime, powerTypes, 0);
                    if (monthList.Count == 1)
                    {
                        monthConsumption.Add((monthList[0].StatisVal).ToString("f2"));
                        if (buildingArea > 0)
                        {
                            monthConsumptionEach.Add((Convert.ToDouble(monthConsumption[i]) / Convert.ToDouble(buildingArea)).ToString("f2"));
                        }
                        else
                        {
                            monthConsumptionEach.Add("0");
                        }
                        monthConsumptionEachPerson.Add((Convert.ToDouble(monthConsumption[i]) / Convert.ToDouble(numOfPeople)).ToString("f2"));
                        dayList = _analogHistoryRepos.GetEnergyStatisDay(5, schoolMeasurePoint[i], currentTime, currentTime, powerTypes, 0);
                        if (dayList.Count == 1)
                        {
                            dayConsumption.Add((dayList[0].StatisVal).ToString("f2"));
                            if (buildingArea > 0)
                            {
                                dayConsumptionEach.Add((Convert.ToDouble(dayConsumption[i]) / Convert.ToDouble(buildingArea)).ToString("f2"));
                            }
                            else
                            {
                                dayConsumptionEach.Add("0");
                            }
                            dayConsumptionEachPerson.Add((Convert.ToDouble(dayConsumption[i]) / Convert.ToDouble(numOfPeople)).ToString("f2"));
                        }
                        else
                        {
                            dayConsumption.Add("0");
                            dayConsumptionEach.Add("0");
                            dayConsumptionEachPerson.Add("0");
                        }
                    }
                    else
                    {
                        monthConsumption.Add("0");
                        monthConsumptionEach.Add("0");
                        monthConsumptionEachPerson.Add("0");
                        dayConsumption.Add("0");
                        dayConsumptionEach.Add("0");
                        dayConsumptionEachPerson.Add("0");
                    }

                }
                else
                {
                    yearConsumption.Add("0");
                    yearConsumptionEach.Add("0");
                    yearConsumptionEachPerson.Add("0");
                    monthConsumption.Add("0");
                    monthConsumptionEach.Add("0");
                    monthConsumptionEachPerson.Add("0");
                    dayConsumption.Add("0");
                    dayConsumptionEach.Add("0");
                    dayConsumptionEachPerson.Add("0");
                }
                i++;
            }
            resultList.Add(new AllSchoolEnergyInfoEntity
            {
                name = nameList[0].ToString(),
                val = yearConsumption[0].ToString(),
                valOne = yearConsumption[1].ToString(),
                valTwo = yearConsumption[2].ToString(),
                valThree = yearConsumption[3].ToString()
            });
            resultList.Add(new AllSchoolEnergyInfoEntity
            {
                name = nameList[1].ToString(),
                val = yearConsumptionEach[0].ToString(),
                valOne = yearConsumptionEach[1].ToString(),
                valTwo = yearConsumptionEach[2].ToString(),
                valThree = yearConsumptionEach[3].ToString()
            }); resultList.Add(new AllSchoolEnergyInfoEntity
            {
                name = nameList[2].ToString(),
                val = yearConsumptionEachPerson[0].ToString(),
                valOne = yearConsumptionEachPerson[1].ToString(),
                valTwo = yearConsumptionEachPerson[2].ToString(),
                valThree = yearConsumptionEachPerson[3].ToString()
            });
            resultList.Add(new AllSchoolEnergyInfoEntity
            {
                name = nameList[3].ToString(),
                val = monthConsumption[0].ToString(),
                valOne = monthConsumption[1].ToString(),
                valTwo = monthConsumption[2].ToString(),
                valThree = monthConsumption[3].ToString()
            });
            resultList.Add(new AllSchoolEnergyInfoEntity
            {
                name = nameList[4].ToString(),
                val = monthConsumptionEach[0].ToString(),
                valOne = monthConsumptionEach[1].ToString(),
                valTwo = monthConsumptionEach[2].ToString(),
                valThree = monthConsumptionEach[3].ToString()
            });
            resultList.Add(new AllSchoolEnergyInfoEntity
            {
                name = nameList[5].ToString(),
                val = monthConsumptionEachPerson[0].ToString(),
                valOne = monthConsumptionEachPerson[1].ToString(),
                valTwo = monthConsumptionEachPerson[2].ToString(),
                valThree = monthConsumptionEachPerson[3].ToString()
            });
            resultList.Add(new AllSchoolEnergyInfoEntity
            {
                name = nameList[6].ToString(),
                val = dayConsumption[0].ToString(),
                valOne = dayConsumption[1].ToString(),
                valTwo = dayConsumption[2].ToString(),
                valThree = dayConsumption[3].ToString()
            });
            resultList.Add(new AllSchoolEnergyInfoEntity
            {
                name = nameList[7].ToString(),
                val = dayConsumptionEach[0].ToString(),
                valOne = dayConsumptionEach[1].ToString(),
                valTwo = dayConsumptionEach[2].ToString(),
                valThree = dayConsumptionEach[3].ToString()
            });
            resultList.Add(new AllSchoolEnergyInfoEntity
            {
                name = nameList[8].ToString(),
                val = dayConsumptionEachPerson[0].ToString(),
                valOne = dayConsumptionEachPerson[1].ToString(),
                valTwo = dayConsumptionEachPerson[2].ToString(),
                valThree = dayConsumptionEachPerson[3].ToString()
            });
            return resultList;
        }

        public ActionResult GetAvailableAnnouncementFromStarttime(DateTime starttime)
        {
            IList<AnnouncementInfo> aiList = _announcementInfoRepos.GetAvailableAnnouncementFromStarttime(starttime);
            IList resultList = new ArrayList();
            foreach (AnnouncementInfo ai in aiList)
            {
                resultList.Add(new { 
                    ID = ai.ID,
                    Title = ai.Title,
                    Content = ai.Content,
                    CreateTime = ai.CreateTime.ToShortDateString(),
                    DeadLine = ai.DeadLine.ToShortDateString(),
                    Author = ai.Author,
                    Remark = ai.Remark
                });
            }
            return Json(new { resultList = resultList }, JsonRequestBehavior.AllowGet);
        }
    }
}
