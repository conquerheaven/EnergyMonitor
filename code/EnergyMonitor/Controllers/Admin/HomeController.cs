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
using EnergyMonitor.Models.Repository.Entity;


namespace EnergyMonitor.Controllers.Admin
{
    /// <summary>
    /// 管理员信息业务逻辑层控制类
    /// </summary>
    /// <author>WangWei</author>
    /// <date>2010-11-26</date>
    [AdminFilter]
    public class HomeController : Controller
    {
        private IAnalogHistoryRepos _analogHistoryRepos = null;
        private ISchoolRepos _schoolRepos = null;
        private IPowerClassRepos _powerClassRepos = null;
        private IAnalogMeasurePoint _analogMeasurePointRepos = null;

        public HomeController()
            : this(new AnalogHistoryRepos(), new SchoolRepos(),new PowerClassRepos(),new AnalogMeasurePointRepos())
        {
        }
        public HomeController(IAnalogHistoryRepos analogHistoryRepos, ISchoolRepos schoolRepos, IPowerClassRepos powerClassRepos, IAnalogMeasurePoint analogMeasurePointRepos)
        {
            _analogHistoryRepos = analogHistoryRepos;
            _schoolRepos = schoolRepos;
            _powerClassRepos = powerClassRepos;
            _analogMeasurePointRepos = analogMeasurePointRepos;
        }       

        /// <summary>
        /// 管理员用户首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Welcome()
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
    }
}
