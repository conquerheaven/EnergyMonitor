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
using System.Xml;
using EnergyMonitor.Models.LinqEntity;

namespace EnergyMonitor.Controllers.Admin
{
    /// <summary>
    /// 管理员能耗统计控制类
    /// </summary>
    [AdminFilter]
    public class EnergyController : Controller
    {
        private IUserRepos _userRepos = null;
        private IAMPRepos _ampRepos = null;
        private IRoomRepos _roomRepos = null;
        private IAnalogHistoryRepos _analogHistoryRepos = null;
        private IPowerClassRepos _powerClassRepos = null;
        private ISchoolRepos _schoolRepos = null;
        private IAnalogMeasurePoint _analogMeasurePointRepos = null;

        public EnergyController()
            : this(new UserRepos(), new AMPRepos(), new RoomRepos(), new AnalogHistoryRepos(), new PowerClassRepos(), new SchoolRepos(), new AnalogMeasurePointRepos())
        {
        }

        public EnergyController(IUserRepos userRepos, IAMPRepos ampRepos, IRoomRepos roomRepos, IAnalogHistoryRepos analogHistoryRepos, IPowerClassRepos powerClassRepos, ISchoolRepos schoolRepos, IAnalogMeasurePoint analogMeasurePointRepos)
        {
            _userRepos = userRepos;
            _ampRepos = ampRepos;
            _roomRepos = roomRepos;
            _analogHistoryRepos = analogHistoryRepos;
            _powerClassRepos = powerClassRepos;
            _schoolRepos = schoolRepos;
            _analogMeasurePointRepos = analogMeasurePointRepos;
        }

        /// <summary>
        /// 跳转最新表值
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult RealElec()
        {
            var powerList = _powerClassRepos.GetAll();
            return View(powerList);
        }

        /// <summary>
        /// Ajax获取最新表值数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRealElecAjax(int currentPage, int totalPages, int objType, string objIDs, string powerType)
        {
            if (Request.IsAjaxRequest())
            {
                //Pager pager = null;
                int?[] ids = null;
                //string[] powerTypes = null;
                //if (!string.IsNullOrWhiteSpace(powerType))
                //{
                //    powerTypes = powerType.Split(new char[] { '_' });
                //}
                if (objType != 0)
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
                var query = _ampRepos.GetRealEnergy();
                //if (powerTypes != null)
                //{
                //    query = query.Where(x => powerTypes.Contains(x.PowerType));
                //}
                switch (objType)
                {
                    case 1://校区
                        query = query.Where(x => ids.Contains(x.SchoolID) && x.AreaID == 0);
                        break;
                    case 2://区域
                        query = query.Where(x => ids.Contains(x.AreaID) && x.BuildingID == 0);
                        break;
                    case 3://楼宇
                        query = query.Where(x => ids.Contains(x.BuildingID) && x.RoomID == 0);
                        break;
                    case 4://房间
                        query = query.Where(x => ids.Contains(x.RoomID));
                        break;
                    case 5:
                        query = query.Where(x => ids.Contains(x.PNO));
                        break;
                    default://所有
                        break;
                }
                //if (totalPages == -1)
                //{
                //    int totalRows = query.Count();
                //    pager = new Pager(1, totalRows);
                //}
                //else
                //{
                //    pager = new Pager(currentPage, totalPages, false);
                //}
                IList list = query.ToList();
                var resultData = new
                {
                    data = list
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 导出最新表值Excel
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="powerType"></param>
        /// <returns></returns>
        public ActionResult GetRealElecExcel(int objType, string objIDs, string powerType)
        {
            int?[] ids = null;
            string[] powerTypes = null;
            if (!string.IsNullOrWhiteSpace(powerType))
            {
                powerTypes = powerType.Split(new char[] { '_' });
            }
            if (objType != 0)
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
            var query = _ampRepos.GetRealEnergy();
            switch (objType)
            {
                case 1://校区
                    query = query.Where(x => ids.Contains(x.SchoolID) && x.AreaID == 0);
                    break;
                case 2://区域
                    query = query.Where(x => ids.Contains(x.AreaID) && x.BuildingID == 0);
                    break;
                case 3://楼宇
                    query = query.Where(x => ids.Contains(x.BuildingID) && x.RoomID == 0);
                    break;
                case 4://房间
                    query = query.Where(x => ids.Contains(x.RoomID));
                    break;
                default://所有
                    break;
            }
            if (powerTypes != null)
            {
                query = query.Where(x => powerTypes.Contains(x.PowerType));
            }
            var list = query.ToList();
            if (list != null)
            {
                string title = "最新表值";
                string[] headers = { "测点编号", "所属对象", "获取时间", "测点表值（度）", "测点剩余电量（度）", "能耗类型" };
                string[] properties = { "PNO", "IName", "Time", "ValStr", "RemValStr", "PowerName" };
                return this.Excel(list, "最新表值.xls", title, headers, properties);
            }
            return null;
        }

        /// <summary>
        /// 跳转用电查询
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult QueryElec()
        {
            var powerList = _powerClassRepos.GetStatisTypes();
            return View(powerList);
        }

        /// <summary>
        /// Ajax获取用电查询数据
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult GetQueryElecAjax(int currentPage, int totalPages, int objType, string objIDs, DateTime startTime, DateTime endTime, string powerType)
        {
            if (Request.IsAjaxRequest())
            {
                Pager pager = null;
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
                String yAxisTitle1 = "用电量（kWh）";
                String yAxisTitle2 = "单位建筑面积用电量（kWh/平方米）";
                if (powerTypes.Length != 0)
                {
                    if (powerTypes[0].Substring(0, 3) == "001")
                    {
                        yAxisTitle1 = "用电量（kWh）";
                        yAxisTitle2 = "单位建筑面积用电量（kWh/平方米）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "002")
                    {
                        yAxisTitle1 = "用水量（吨）";
                        yAxisTitle2 = "单位建筑面积用水量（吨/平方米）";
                    }
                    else if (powerTypes[0].Substring(0, 3) == "003")
                    {
                        yAxisTitle1 = "燃气用量（立方米）";
                        yAxisTitle2 = "单位建筑面积燃气用量（立方米/平方米）";
                    }
                }
                if (objType != 0)
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
                //if (totalPages == -1)
                //{
                //    int totalRows = _analogHistoryRepos.GetEnergyCount(objType, ids, powerTypes, startTime, endTime);
                //    pager = new Pager(1, totalRows);
                //}
                //else
                //{
                //    pager = new Pager(currentPage, totalPages, false);
                //}
                IList list = _analogHistoryRepos.GetEnergy(objType, ids, powerTypes, startTime, endTime).ToList();
                var resultData = new
                {
                    data = list,
                    valHead = yAxisTitle1,
                    valHeadPerArea = yAxisTitle2
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 获取用电查询数据提供给手机端使用
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult GetQueryElecAjaxForMobile(string objIDs, DateTime startTime, DateTime endTime)
        {
            //因为提供给手机端使用，供用户查询房间能耗所以部分参数写死
            int currentPage = 1;
            int totalPages = -1;
            int objType = 5;
            string powerType = "001";

            Pager pager = null;
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
            String yAxisTitle1 = "用电量（kWh）";
            String yAxisTitle2 = "单位建筑面积用电量（kWh/平方米）";
            if (powerTypes.Length != 0)
            {
                if (powerTypes[0].Substring(0, 3) == "001")
                {
                    yAxisTitle1 = "用电量（kWh）";
                    yAxisTitle2 = "单位建筑面积用电量（kWh/平方米）";
                }
                else if (powerTypes[0].Substring(0, 3) == "002")
                {
                    yAxisTitle1 = "用水量（吨）";
                    yAxisTitle2 = "单位建筑面积用水量（吨/平方米）";
                }
                else if (powerTypes[0].Substring(0, 3) == "003")
                {
                    yAxisTitle1 = "燃气用量（立方米）";
                    yAxisTitle2 = "单位建筑面积燃气用量（立方米/平方米）";
                }
            }
            if (objType != 0)
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
            //if (totalPages == -1)
            //{
            //    int totalRows = _analogHistoryRepos.GetEnergyCount(objType, ids, powerTypes, startTime, endTime);
            //    pager = new Pager(1, totalRows);
            //}
            //else
            //{
            //    pager = new Pager(currentPage, totalPages, false);
            //}
            IList list = _analogHistoryRepos.GetEnergy(objType, ids, powerTypes, startTime, endTime).ToList();
            var resultData = new
            {
                data = list,
                valHead = yAxisTitle1,
                valHeadPerArea = yAxisTitle2
            };
                return Json(resultData, JsonRequestBehavior.AllowGet);           
        }

        /// <summary>
        /// 跳转查询全校用能量页面
        /// </summary>
        /// <returns></returns>
        public ActionResult QueryUniversityEnergy()
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
                        else {
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
            return View(resultList);
        
        }

        /// <summary>
        /// 查询全校用能量
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult GetQueryUnivstyElecAjax(DateTime startTime, DateTime endTime)
        {
            if (Request.IsAjaxRequest())
            {
                var schoolMeasurePoint = _analogMeasurePointRepos.GetAllSchoolMeasurePoint();
                double val = -1;
                foreach (var item in schoolMeasurePoint)
                {
                    double perVal = _analogHistoryRepos.GetEnergy(5, item, null, startTime, endTime);
                    val += perVal;
                }               
                var unInfo = _schoolRepos.GetUniversityInfo();
                double valPerstudent = (unInfo.StudentCount.HasValue && unInfo.StudentCount.Value != 0 && val != -1) ? val / unInfo.StudentCount.Value : 0;
                double valPerArea = (unInfo.Area.HasValue && unInfo.Area.Value != 0 && val != -1) ? val / unInfo.Area.Value : 0;

                var resultData = new
                {
                    statisVal = val.ToString("f4"),
                    valPerStu = valPerstudent.ToString("f4"),
                    valPerArea = valPerArea.ToString("f4")
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 导出用电查询Excel
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult GetQueryElecExcel(int objType, string objIDs, DateTime startTime, DateTime endTime, string powerType, string attachName)
        {
            int?[] ids = null;
            if (objType > 0)
            {
                string[] idStrs = objIDs.Split(new char[] { '_' });
                ids = new int?[idStrs.Length];
                string[] powerTypes = null;
                if (!string.IsNullOrWhiteSpace(powerType))
                {
                    powerTypes = powerType.Split(new char[] { '_' });
                }
                for (int i = 0; i < idStrs.Length; i++)
                {
                    int temp = -1;
                    if (Int32.TryParse(idStrs[i], out temp))
                    {
                        ids[i] = temp;
                    }
                }
                var list = _analogHistoryRepos.GetEnergy(objType, ids, powerTypes, startTime, endTime).ToList<EnergyEntity>();
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        item.IName = attachName + item.IName;
                    }
                    string title = "能耗查询(" + startTime.ToString("yyyy-MM-dd") + "_" + endTime.ToString("yyyy-MM-dd") + ")";
                    string[] headers = { "所属对象", "用能值" };
                    string[] properties = { "IName", "ValStr" };
                    return this.Excel(list, "能耗查询.xls", title, headers, properties);
                }
            }
            return null;
        }



        /// <summary>
        /// 跳转电力值查询
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult QueryVal()
        {
            var powerList = _powerClassRepos.GetNonStatisTypes();
            return View(powerList);
        }

        /// <summary>
        /// 查询电力值数据
        /// </summary>
        /// <param name="currentPage">当前页</param>
        /// <param name="totalPages">总页数</param>
        /// <param name="objType">查询对象类型</param>
        /// <param name="objIDs">查询对象Id</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="powerType">能耗类型</param>
        /// <returns>电力值数据</returns>
        public ActionResult QueryValData(int currentPage, int totalPages, int objType, int objIDs, DateTime startTime, DateTime endTime, string powerType, string renderType)
        {
            if (Request.IsAjaxRequest() && objType > 0 && objIDs > 0)
            {
                Pager pager = null;
                if (totalPages == -1)
                {
                    if (renderType == "List")
                    {
                        int totalRows = _analogHistoryRepos.GetHistoryVal(objType, objIDs, startTime, endTime, powerType).Count();
                        pager = new Pager(1, totalRows);
                        totalPages = pager.TotalPages;
                    }
                    else
                    {
                        var dateArray = _analogHistoryRepos.GetHistoryVal(objType, objIDs, startTime, endTime, powerType).Select(x => x.RealTime.Date).Distinct().OrderBy(x => x).ToArray();
                        totalPages = dateArray.Length;
                        startTime = dateArray[0];
                    }
                }
                else
                {
                    if (renderType == "List")
                    {
                        pager = new Pager(currentPage, totalPages, false);
                        totalPages = pager.TotalPages;
                    }
                    else
                    {
                        var dateArray = _analogHistoryRepos.GetHistoryVal(objType, objIDs, startTime, endTime, powerType).Select(x => x.RealTime.Date).Distinct().OrderBy(x => x).ToArray();
                        startTime = dateArray[currentPage - 1];
                    }
                }
                IList list = null;
                if (totalPages > 0)
                {
                    if (renderType == "List")
                    {
                        list = _analogHistoryRepos.GetHistoryVal(objType, objIDs, startTime, endTime, powerType).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    }
                    else
                    {
                        List<EnergyEntity> dataList = null;
                        dataList = _analogHistoryRepos.GetHistoryVal(objType, objIDs, startTime, startTime, powerType).ToList();
                        if (dataList != null)
                        {
                            list = new ArrayList();
                            var pointList = dataList.Select(x => x.PNO).Distinct();
                            foreach (var item in pointList)
                            {
                                var dataItem = new
                                {
                                    pointNo = item,
                                    dataList = dataList.Where(x => x.PNO == item).ToList()
                                };
                                list.Add(dataItem);
                            }
                        }
                    }
                }
                var resultData = new
                {
                    totalPages = totalPages,
                    data = list
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 获取电力值数据Excel
        /// </summary>
        /// <param name="objType">查询对象类型</param>
        /// <param name="objIDs">查询对象Id</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="powerType">能耗类型</param>
        /// <param name="objName">查询对象名称</param>
        /// <returns>电力值数据Excel流</returns>
        public ActionResult GetQueryValExcel(int objType, int objIDs, DateTime startTime, DateTime endTime, string powerType, string objName)
        {
            if (objType > 0 && objIDs > 0)
            {
                var list = _analogHistoryRepos.GetHistoryVal(objType, objIDs, startTime, endTime, powerType).ToList();
                if (list != null)
                {
                    string title = objName + " 电力值查询(" + startTime.ToString("yyyy-MM-dd") + "_" + endTime.ToString("yyyy-MM-dd") + ")";
                    string[] headers = { "测点编号", "能耗类型", "时间", "取值" };
                    string[] properties = { "PNO", "PowerName", "Time", "ValStr" };
                    return this.Excel(list, "电力值查询.xls", title, headers, properties);
                }
            }
            return null;
        }
    }
}
