using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Collections;
using EnergyMonitor.Controllers.Utils;
using EnergyMonitor.Controllers.Admin.Filters;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Implement;
using EnergyMonitor.Models.Repository.Entity;
using EnergyMonitor.Models.LinqEntity;
using System.Linq;
using System.Xml;
using System.Text;

namespace EnergyMonitor.Controllers.Admin
{
    /// <summary>
    /// 能耗分析
    /// </summary>
    [AdminFilter]
    public class AnalysisController : Controller
    {
        private IPowerClassRepos _powerClassRepos = null;
        private IAnalogHistoryRepos _analogHistoryRepos = null;
        private IBuildingRepos _buildingRepos = null;

        public AnalysisController()
            : this(new PowerClassRepos(), new AnalogHistoryRepos(),new BuildingRepos())
        {
        }
        
        public AnalysisController(IPowerClassRepos powerClassRepos, IAnalogHistoryRepos analogHistoryRepos,IBuildingRepos buildingRepos)
        {
            _powerClassRepos = powerClassRepos;
            _analogHistoryRepos = analogHistoryRepos;
            _buildingRepos = buildingRepos;
        }

        /// <summary>
        /// 跳转单个时间对比
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 10, VaryByParam = "none")]
        [AuthenticationFilter]
        public ActionResult Elec()
        {
            var powerList = _powerClassRepos.GetStatisTypes();
            return View(powerList);
        }

        #region 纵向粒度对比分析

        /// <summary>
        /// Ajax获取单个时间对比分析
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="granularity"></param>
        /// <returns></returns>
        public ActionResult GetElecVGAjax(int currentPage, int totalPages, int objType, int objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType, double sum)
        {
            if (Request.IsAjaxRequest())
            {
                Pager pager = null;
                string[] powerTypes = null;
                if (!string.IsNullOrWhiteSpace(powerType))
                {
                    powerTypes = powerType.Split(new char[] { '_' });
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
                if (totalPages == -1 && objIDs > 0)
                {
                    if (granularity == "day")//按天
                    {
                        int totalRows = _analogHistoryRepos.GetEnergyStatisDayCount(objType, objIDs, startTime, endTime, powerTypes);
                        pager = new Pager(1, totalRows);
                    }
                    else if (granularity == "month")//按月
                    {
                        int totalRows = _analogHistoryRepos.GetEnergyStatisMonthCount(objType, objIDs, startTime, endTime, powerTypes);
                        pager = new Pager(1, totalRows);
                    }
                    else if (granularity == "year") //按年
                    {
                        int totalRows = _analogHistoryRepos.GetEnergyStatisYear(objType, objIDs, startTime, endTime, powerTypes, 1).Count();
                        pager = new Pager(1, totalRows);
                    }
                }
                else
                {
                    pager = new Pager(currentPage, totalPages, false);
                }
                IList<ChartStatisEntity> list = null;
                if (pager.TotalPages > 0)
                {
                    if (granularity == "day")//按天
                    {
                        if (sum < 0)
                        {
                            sum = _analogHistoryRepos.GetEnergyStatisDaySum(objType, objIDs, startTime, endTime, powerTypes);
                        }
                        list = _analogHistoryRepos.GetEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, sum).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                        foreach (var item in list)
                        {
                            item.TimeBlock = item.DTimeStr;
                        }
                    }
                    else if (granularity == "month")//按月
                    {
                        if (sum < 0)
                        {
                            sum = _analogHistoryRepos.GetEnergyStatisMonthSum(objType, objIDs, startTime, endTime, powerTypes);
                        }
                        list = _analogHistoryRepos.GetEnergyStatisMonth(objType, objIDs, startTime, endTime, powerTypes, sum).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                        foreach (var item in list)
                        {
                            item.TimeBlock = item.MTimeStr;
                        }
                    }
                    else if (granularity == "year") //按年
                    {
                        if (sum < 0)
                        {
                            sum = _analogHistoryRepos.GetEnergyStatisYear(objType, objIDs, startTime, endTime, powerTypes, 1).Sum(x => x.StatisVal);
                        }
                        list = _analogHistoryRepos.GetEnergyStatisYear(objType, objIDs, startTime, endTime, powerTypes, sum).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                        foreach (var item in list)
                        {
                            item.TimeBlock = item.YTimeStr;
                        }
                    }
                }
                var resultData = new
                {
                    yAxisTitle = yAxisTitle,
                    totalPages = pager.TotalPages,
                    totalSum = sum,
                    data = list
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// Ajax获取单个时间对比分析(不分页)
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="granularity"></param>
        /// <returns></returns>
        public ActionResult GetElecVGAjaxNoPage(int objType, int objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType, double sum)
        {
            if (Request.IsAjaxRequest())
            {
                string[] powerTypes = null;
                int totalRows = 0;
                if (!string.IsNullOrWhiteSpace(powerType))
                {
                    powerTypes = powerType.Split(new char[] { '_' });
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
                IList<ChartStatisEntity> list = null;
                if (objIDs > 0)
                {
                    if (granularity == "day")//按天
                    {
                        if (sum < 0)
                        {
                            sum = _analogHistoryRepos.GetEnergyStatisDaySum(objType, objIDs, startTime, endTime, powerTypes);
                        }
                        list = _analogHistoryRepos.GetEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, sum).OrderBy(p => p.Time).ToList();
                        totalRows = list.Count();
                        foreach (var item in list)
                        {
                            item.TimeBlock = item.DTimeStr;
                        }
                    }
                    else if (granularity == "month")//按月
                    {
                        if (sum < 0)
                        {
                            sum = _analogHistoryRepos.GetEnergyStatisMonthSum(objType, objIDs, startTime, endTime, powerTypes);
                        }
                        list = _analogHistoryRepos.GetEnergyStatisMonth(objType, objIDs, startTime, endTime, powerTypes, sum).OrderBy(p => p.Time).ToList();
                        totalRows = list.Count();
                        foreach (var item in list)
                        {
                            item.TimeBlock = item.MTimeStr;
                        }
                    }
                    else if (granularity == "year") //按年
                    {
                        if (sum < 0)
                        {
                            sum = _analogHistoryRepos.GetEnergyStatisYear(objType, objIDs, startTime, endTime, powerTypes, 1).Sum(x => x.StatisVal);
                        }
                        list = _analogHistoryRepos.GetEnergyStatisYear(objType, objIDs, startTime, endTime, powerTypes, sum).OrderBy(p => p.Time).ToList();
                        totalRows = list.Count();
                        foreach (var item in list)
                        {
                            item.TimeBlock = item.YTimeStr;
                        }
                    }
                }
                var resultData = new
                {
                    yAxisTitle = yAxisTitle,
                    count = totalRows,
                    totalSum = sum,
                    data = list
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 导出单个时间对比分析Excel
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="granularity"></param>
        /// <param name="powerType"></param>
        /// <returns></returns>
        public ActionResult GetElecVGExcel(int objType, int objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType)
        {
            if (objIDs > 0)
            {
                IList list = null;
                string[] powerTypes = null;
                if (!string.IsNullOrWhiteSpace(powerType))
                {
                    powerTypes = powerType.Split(new char[] { '_' });
                }
                if (granularity == "day")//按天
                {
                    double sum = _analogHistoryRepos.GetEnergyStatisDaySum(objType, objIDs, startTime, endTime, powerTypes);
                    list = _analogHistoryRepos.GetEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, sum).ToList();
                }
                else//按月
                {
                    double sum = _analogHistoryRepos.GetEnergyStatisMonthSum(objType, objIDs, startTime, endTime, powerTypes);
                    list = _analogHistoryRepos.GetEnergyStatisMonth(objType, objIDs, startTime, endTime, powerTypes, sum).ToList();
                }
                if (list != null)
                {
                    string[] headers = { "日期", "用能值", "所占比例" };
                    string[] properties = { "TimeBlock", "StatisSVal", "SPercentage" };
                    string title = "能耗单个时间对比分析(" + startTime.ToString("yyyy-MM-dd") + "_" + endTime.ToString("yyyy-MM-dd") + ")";
                    return this.Excel(list, "能耗单个时间对比分析.xls", title, headers, properties);
                }
            }
            return null;
        }

        /// <summary>
        /// Ajax获取单个时间对比分析数据
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="granularity"></param>
        /// <param name="powerType"></param>
        /// <returns></returns>
        public ActionResult GetElecVGAnalysisAjax(int objType, int objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType)
        {
            if (Request.IsAjaxRequest() && objIDs > 0)
            {
                int totalRows = 0;
                string[] powerTypes = null;
                if (!string.IsNullOrWhiteSpace(powerType))
                {
                    powerTypes = powerType.Split(new char[] { '_' });
                }
                IList<ChartStatisEntity> list = null;
                if (granularity == "day")//按天
                {
                    list = _analogHistoryRepos.GetEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, 1).ToList();
                }
                else//按月
                {
                    list = _analogHistoryRepos.GetEnergyStatisMonth(objType, objIDs, startTime, endTime, powerTypes, 1).ToList();
                }
                totalRows = list.Count;
                if (totalRows > 0)
                {
                    double sum = list.Sum(x => x.StatisVal);
                    var maxObj = list.OrderByDescending(x => x.StatisVal).FirstOrDefault();
                    var minObj = list.OrderBy(x => x.StatisVal).FirstOrDefault();
                    maxObj.Sum = sum;
                    minObj.Sum = sum;
                    var resultData = new
                    {
                        totalRows = totalRows,
                        totalSum = sum.ToString("f1"),
                        average = (sum / totalRows).ToString("f1"),
                        maxObj = maxObj,
                        minObj = minObj
                    };
                    return Json(resultData, JsonRequestBehavior.AllowGet);
                }
                return Json(new { totalRows = totalRows }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        #endregion

        /// <summary>
        /// 跳转单个分类对比分析
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 10, VaryByParam = "none")]
        [AuthenticationFilter]
        public ActionResult VCElec()
        {
            var powerList = _powerClassRepos.GetStatisTypes();
            return View(powerList);
        }

        public ActionResult GetSpecifiedBuildingEnergy(string buildingId, string time, string gatewayId, string operation, string type)
        {                
                XmlTextWriter writer = new XmlTextWriter(Response.OutputStream, Encoding.UTF8);
                writer.WriteStartDocument();
                writer.WriteStartElement("root");
               
                writer.WriteStartElement("common");
                writer.WriteStartElement("building_id");
                writer.WriteString(buildingId.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("gateway_id");
                writer.WriteString(gatewayId);
                writer.WriteEndElement();
                writer.WriteStartElement("type");
                writer.WriteString(type);
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("data");
                writer.WriteStartAttribute("operation", null);
                writer.WriteString(operation);
                writer.WriteEndAttribute();
                if (operation == "report" || operation == "continuous")
                {
                    string powerType = "001";
                    string[] powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
                    string timeStr = time;
                    time = time.Substring(0, 10) + "0000";
                    DateTime endTime = DateTime.ParseExact(time, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);                   
                    IList<ChartStatisEntity> list = _analogHistoryRepos.GetGuangHuaConsupmtion(218, endTime, powerTypes).ToList();
                    //IList<ChartStatisEntity> list1 = _analogHistoryRepos.GetGuangHuaConsupmtion(221, endTime, powerTypes).ToList();
                    //IList<ChartStatisEntity> list2 = _analogHistoryRepos.GetGuangHuaConsupmtion(311, endTime, powerTypes).ToList();
                    Double[] energys = new Double[15];
                    int i = 0;
                    foreach (var item in list)
                    {
                        if ("001001".Equals(item.PowerType))
                        {
                            i = 1;
                            energys[i] = item.StatisVal;
                        }
                        else if ("001002".Equals(item.PowerType))
                        {
                            i = 2;
                            energys[i] = item.StatisVal;
                        }
                        else if ("001003".Equals(item.PowerType) || "001004".Equals(item.PowerType))
                        {
                            i = 3;
                            energys[i] = energys[i] + item.StatisVal;
                        }                     
                    }
					for (int j = 1; j < 4; j++)
                    {
                        energys[0] = energys[0] + energys[j];                       
                    }
                    //foreach (var item in list1)
                    //{
                    //    if ("001001".Equals(item.PowerType))
                    //    {
                    //        i = 6;
                    //        energys[i] = item.StatisVal;
                    //    }
                    //    else if ("001002".Equals(item.PowerType))
                    //    {
                    //        i = 7;
                    //        energys[i] = item.StatisVal;
                    //    }
                    //    else if ("001003".Equals(item.PowerType) || "001004".Equals(item.PowerType))
                    //    {
                    //        i = 8;
                    //        energys[i] = energys[i] + item.StatisVal;
                    //    }                     
                    //}
                    //for (int j = 6; j < 9; j++)
                    //{
                    //    energys[5] = energys[5] + energys[j];                       
                    //}
                    //foreach (var item in list2)
                    //{
                    //    if ("001001".Equals(item.PowerType))
                    //    {
                    //        i = 11;
                    //        energys[i] = item.StatisVal;
                    //    }
                    //    else if ("001002".Equals(item.PowerType))
                    //    {
                    //        i = 12;
                    //        energys[i] = item.StatisVal;
                    //    }
                    //    else if ("001003".Equals(item.PowerType) || "001004".Equals(item.PowerType))
                    //    {
                    //        i = 13;
                    //        energys[i] = energys[i] + item.StatisVal;
                    //    }                     
                    //}
                    //for (int j = 11; j < 14; j++)
                    //{
                    //    energys[10] = energys[10] + energys[j];                       
                    //}					
                    writer.WriteStartElement("time");
                    writer.WriteString(timeStr);
                    writer.WriteEndElement();

                    writer.WriteStartElement("energy_items");
                    writer.WriteStartElement("energy_item");
                    writer.WriteStartAttribute("code", null);
                    writer.WriteString("01000");
                    writer.WriteEndAttribute();
                    writer.WriteString(energys[0].ToString());
                    writer.WriteEndElement();
                    writer.WriteStartElement("energy_item");
                    writer.WriteStartAttribute("code", null);
                    writer.WriteString("01A00");
                    writer.WriteEndAttribute();
                    writer.WriteString(energys[1].ToString());
                    writer.WriteEndElement();
                    writer.WriteStartElement("energy_item");
                    writer.WriteStartAttribute("code", null);
                    writer.WriteString("01B00");
                    writer.WriteEndAttribute();
                    writer.WriteString(energys[2].ToString());
                    writer.WriteEndElement();
                    writer.WriteStartElement("energy_item");
                    writer.WriteStartAttribute("code", null);
                    writer.WriteString("01C00");
                    writer.WriteEndAttribute();
                    writer.WriteString(energys[3].ToString());
                    writer.WriteEndElement();
                    writer.WriteStartElement("energy_item");
                    writer.WriteStartAttribute("code", null);
                    writer.WriteString("01D00");
                    writer.WriteEndAttribute();
                    writer.WriteString(energys[4].ToString());
                    writer.WriteEndElement();
					
                    //writer.WriteStartElement("energy_item");
                    //writer.WriteStartAttribute("code", null);
                    //writer.WriteString("01000");
                    //writer.WriteEndAttribute();
                    //writer.WriteString(energys[5].ToString());
                    //writer.WriteEndElement();
                    //writer.WriteStartElement("energy_item");
                    //writer.WriteStartAttribute("code", null);
                    //writer.WriteString("01A00");
                    //writer.WriteEndAttribute();
                    //writer.WriteString(energys[6].ToString());
                    //writer.WriteEndElement();
                    //writer.WriteStartElement("energy_item");
                    //writer.WriteStartAttribute("code", null);
                    //writer.WriteString("01B00");
                    //writer.WriteEndAttribute();
                    //writer.WriteString(energys[7].ToString());
                    //writer.WriteEndElement();
                    //writer.WriteStartElement("energy_item");
                    //writer.WriteStartAttribute("code", null);
                    //writer.WriteString("01C00");
                    //writer.WriteEndAttribute();
                    //writer.WriteString(energys[8].ToString());
                    //writer.WriteEndElement();
                    //writer.WriteStartElement("energy_item");
                    //writer.WriteStartAttribute("code", null);
                    //writer.WriteString("01D00");
                    //writer.WriteEndAttribute();
                    //writer.WriteString(energys[9].ToString());
                    //writer.WriteEndElement();
					
                    //writer.WriteStartElement("energy_item");
                    //writer.WriteStartAttribute("code", null);
                    //writer.WriteString("01000");
                    //writer.WriteEndAttribute();
                    //writer.WriteString(energys[10].ToString());
                    //writer.WriteEndElement();
                    //writer.WriteStartElement("energy_item");
                    //writer.WriteStartAttribute("code", null);
                    //writer.WriteString("01A00");
                    //writer.WriteEndAttribute();
                    //writer.WriteString(energys[11].ToString());
                    //writer.WriteEndElement();
                    //writer.WriteStartElement("energy_item");
                    //writer.WriteStartAttribute("code", null);
                    //writer.WriteString("01B00");
                    //writer.WriteEndAttribute();
                    //writer.WriteString(energys[12].ToString());
                    //writer.WriteEndElement();
                    //writer.WriteStartElement("energy_item");
                    //writer.WriteStartAttribute("code", null);
                    //writer.WriteString("01C00");
                    //writer.WriteEndAttribute();
                    //writer.WriteString(energys[13].ToString());
                    //writer.WriteEndElement();
                    //writer.WriteStartElement("energy_item");
                    //writer.WriteStartAttribute("code", null);
                    //writer.WriteString("01D00");
                    //writer.WriteEndAttribute();
                    //writer.WriteString(energys[14].ToString());
                    //writer.WriteEndElement();
                    writer.WriteEndElement();

                    writer.WriteStartElement("meters");
                    writer.WriteStartAttribute("total ", null);
                    writer.WriteString("0");
                    writer.WriteEndAttribute();

                    //writer.WriteStartElement("meter");
                    //writer.WriteStartAttribute("id", null);
                    //writer.WriteString("A001");                
                    //writer.WriteAttributeString("name", "1号电表");


                    //writer.WriteStartElement("function");
                    //writer.WriteStartAttribute("id", null);
                    //writer.WriteString("WPP");
                    //writer.WriteStartAttribute("error", null);
                    //writer.WriteString("");
                    //writer.WriteEndAttribute();
                    //writer.WriteString("数据1");
                    //writer.WriteEndElement();
                    //writer.WriteEndElement();

                    //writer.WriteStartElement("meter");
                    //writer.WriteAttributeString("id", "A002");
                    //writer.WriteAttributeString("name", "2号电表");


                    //writer.WriteStartElement("function");
                    //writer.WriteStartAttribute("id", null);
                    //writer.WriteString("WPP");
                    //writer.WriteStartAttribute("error", null);
                    //writer.WriteString("xx");
                    //writer.WriteEndAttribute();
                    //writer.WriteString("数据2");
                    //writer.WriteEndElement();
                    //writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                else if (operation == "finish")
                {
                    string powerType = "001";
                    string[] powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
                    string timeStr = time;
                    time = time.Substring(0, 10) + "0000";
                    DateTime endTime = DateTime.ParseExact(time, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                    IList<ChartStatisEntity> list = _analogHistoryRepos.GetGuangHuaConsupmtion(218, endTime, powerTypes).ToList();
                    Double[] energys = new Double[5];
                    int i = 0;
                    foreach (var item in list)
                    {
                        if ("001001".Equals(item.PowerType))
                        {
                            i = 1;
                            energys[i] = item.StatisVal;
                        }
                        else if ("001002".Equals(item.PowerType))
                        {
                            i = 2;
                            energys[i] = item.StatisVal;
                        }
                        else if ("001003".Equals(item.PowerType) || "001004".Equals(item.PowerType))
                        {
                            i = 3;
                            energys[i] = energys[i] + item.StatisVal;
                        }                     
                    }
                    for (int j = 1; j < 4; j++)
                    {
                        energys[0] = energys[0] + energys[j];
                    }
                    writer.WriteStartElement("time");
                    writer.WriteString(timeStr);
                    writer.WriteEndElement();

                    writer.WriteStartElement("energy_items");
                    writer.WriteStartElement("energy_item");
                    writer.WriteStartAttribute("code", null);
                    writer.WriteString("01000");
                    writer.WriteEndAttribute();
                    writer.WriteString(energys[0].ToString());
                    writer.WriteEndElement();
                    writer.WriteStartElement("energy_item");
                    writer.WriteStartAttribute("code", null);
                    writer.WriteString("01A00");
                    writer.WriteEndAttribute();
                    writer.WriteString(energys[1].ToString());
                    writer.WriteEndElement();
                    writer.WriteStartElement("energy_item");
                    writer.WriteStartAttribute("code", null);
                    writer.WriteString("01B00");
                    writer.WriteEndAttribute();
                    writer.WriteString(energys[2].ToString());
                    writer.WriteEndElement();
                    writer.WriteStartElement("energy_item");
                    writer.WriteStartAttribute("code", null);
                    writer.WriteString("01C00");
                    writer.WriteEndAttribute();
                    writer.WriteString(energys[3].ToString());
                    writer.WriteEndElement();
                    writer.WriteStartElement("energy_item");
                    writer.WriteStartAttribute("code", null);
                    writer.WriteString("01D00");
                    writer.WriteEndAttribute();
                    writer.WriteString(energys[4].ToString());
                    writer.WriteEndElement();
                    writer.WriteEndElement();

                    writer.WriteStartElement("meters");
                    writer.WriteStartAttribute("total ", null);
                    writer.WriteString("0");
                    writer.WriteEndAttribute();

                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
             
                writer.Flush();
                writer.Close();
                return null;
               
           
        }

        /// <summary>
        /// Ajax单个分类对比分析数据
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
        public ActionResult GetVCElecAjax(int currentPage, int totalPages, int objType, int objIDs, DateTime startTime, DateTime endTime, string powerType, double sum)
        {
            if (Request.IsAjaxRequest() && objIDs > 0)
            {
                Pager pager = null;
                //string[] powerTypes = null;
                //if (!string.IsNullOrWhiteSpace(powerType))
                //{
                //    powerTypes = powerType.Split(new char[] { '_' });
                //}
                string[] powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
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
                    int totalRows = _analogHistoryRepos.GetEnergyByPowerCount(objType, objIDs, startTime, endTime, powerTypes, 1);
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
                        sum = _analogHistoryRepos.GetEnergy(objType, objIDs, powerTypes, startTime, endTime);
                    }
                    list = _analogHistoryRepos.GetEnergyByPower(objType, objIDs, startTime, endTime, powerTypes, sum).Skip(pager.StartRow).Take(pager.PageSize).ToList();
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
        /// Ajax获取单个分类对比分分析数据
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerType"></param>
        /// <returns></returns>
        public ActionResult GetVCElecAnalysisAjax(int objType, int objIDs, DateTime startTime, DateTime endTime, string powerType)
        {
            if (Request.IsAjaxRequest() && objIDs > 0)
            {
                //string[] powerTypes = null;
                //if (!string.IsNullOrWhiteSpace(powerType))
                //{
                //    powerTypes = powerType.Split(new char[] { '_' });
                //}
                string[] powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
                var queryData = _analogHistoryRepos.GetEnergyByPower(objType, objIDs, startTime, endTime, powerTypes, 1).ToList();
                int totalRows = queryData.Count;
                if (totalRows > 0)
                {
                    double sum = queryData.Sum(x => x.StatisVal);
                    var maxObj = queryData.OrderByDescending(x => x.StatisVal).FirstOrDefault();
                    var minObj = queryData.OrderBy(x => x.StatisVal).FirstOrDefault();
                    maxObj.Sum = sum;
                    minObj.Sum = sum;
                    var resultData = new
                    {
                        totalRows = totalRows,
                        totalSum = sum.ToString("f1"),
                        average = (sum / totalRows).ToString("f1"),
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
        /// 导出单个分类对比分析Excel
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerType"></param>
        /// <returns></returns>
        public ActionResult GetVCElecExcel(int objType, int objIDs, DateTime startTime, DateTime endTime, string powerType)
        {
            if (objIDs > 0)
            {
                //string[] powerTypes = null;
                //if (!string.IsNullOrWhiteSpace(powerType))
                //{
                //    powerTypes = powerType.Split(new char[] { '_' });
                //}
                string[] powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
                double sum = _analogHistoryRepos.GetEnergyStatisDaySum(objType, objIDs, startTime, endTime, powerTypes);
                IList list = _analogHistoryRepos.GetEnergyByPower(objType, objIDs, startTime, endTime, powerTypes, sum).ToList();
                if (list != null)
                {
                    string[] headers = { "能耗类型", "用能值", "所占比例" };
                    string[] properties = { "PowerName", "StatisSVal", "SPercentage" };
                    string title = "能耗单个分类对比分析(" + startTime.ToString("yyyy-MM-dd") + "_" + endTime.ToString("yyyy-MM-dd") + ")";
                    return this.Excel(list, "能耗单个分类对比分析.xls", title, headers, properties);
                }
            }
            return null;
        }

        /// <summary>
        /// 跳转单个时间分类对比分析
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 10, VaryByParam = "none")]
        [AuthenticationFilter]
        public ActionResult VGCElec()
        {
            var powerList = _powerClassRepos.GetStatisTypes();
            return View(powerList);
        }

        /// <summary>
        /// Ajax获取单个时间分类对比分析一览表数据
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
        public ActionResult GetVGCElecListAjax(int currentPage, int totalPages, int objType, int objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType, double sum)
        {
            if (Request.IsAjaxRequest())
            {
                Pager pager = null;
                //string[] powerTypes = null;
                //if (!string.IsNullOrWhiteSpace(powerType))
                //{
                //    powerTypes = powerType.Split(new char[] { '_' });
                //}
                string[] powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
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
                if (totalPages == -1 && objIDs > 0)
                {
                    if (granularity == "day")//按天
                    {
                        totalRows = _analogHistoryRepos.GetDayEnergyByGranularityPower(objType, objIDs, startTime, endTime, powerTypes, 1).Count();
                    }
                    else//按月
                    {
                        totalRows = _analogHistoryRepos.GetMonthEnergyByGranularityPower(objType, objIDs, startTime, endTime, powerTypes, 1).Count();
                    }
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
                            sum = _analogHistoryRepos.GetEnergyStatisDaySum(objType, objIDs, startTime, endTime, powerTypes);
                        }
                        list = _analogHistoryRepos.GetDayEnergyByGranularityPower(objType, objIDs, startTime, endTime, powerTypes, sum).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    }
                    else//按月
                    {
                        if (sum < 0)
                        {
                            sum = _analogHistoryRepos.GetEnergyStatisMonthSum(objType, objIDs, startTime, endTime, powerTypes);
                        }
                        list = _analogHistoryRepos.GetMonthEnergyByGranularityPower(objType, objIDs, startTime, endTime, powerTypes, sum).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    }
                }
                var resultData = new
                {
                    totalPages = pager.TotalPages,
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
        /// Ajax获取单个时间分类对比分析一览表数据(不分页)
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
        public ActionResult GetVGCElecListAjaxNoPage(int objType, int objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType, double sum)
        {
            if (Request.IsAjaxRequest())
            {
                //string[] powerTypes = null;
                //if (!string.IsNullOrWhiteSpace(powerType))
                //{
                //    powerTypes = powerType.Split(new char[] { '_' });
                //}
                string[] powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
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
                if (objIDs > 0)
                {
                    if (granularity == "day")//按天
                    {
                        totalRows = _analogHistoryRepos.GetDayEnergyByGranularityPower(objType, objIDs, startTime, endTime, powerTypes, 1).Count();
                    }
                    else//按月
                    {
                        totalRows = _analogHistoryRepos.GetMonthEnergyByGranularityPower(objType, objIDs, startTime, endTime, powerTypes, 1).Count();
                    }
                }
                IList list = null;
                if (objIDs > 0)
                {
                    if (granularity == "day")//按天
                    {
                        if (sum < 0)
                        {
                            sum = _analogHistoryRepos.GetEnergyStatisDaySum(objType, objIDs, startTime, endTime, powerTypes);
                        }
                        list = _analogHistoryRepos.GetDayEnergyByGranularityPower(objType, objIDs, startTime, endTime, powerTypes, sum).OrderBy(x => x.Time).ToList();
                    }
                    else//按月
                    {
                        if (sum < 0)
                        {
                            sum = _analogHistoryRepos.GetEnergyStatisMonthSum(objType, objIDs, startTime, endTime, powerTypes);
                        }
                        list = _analogHistoryRepos.GetMonthEnergyByGranularityPower(objType, objIDs, startTime, endTime, powerTypes, sum).OrderBy(x => x.Time).ToList();
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
        /// Ajax获取单个时间分类对比分析图表数据
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
        public ActionResult GetVGCElecChartAjax(int currentPage, int totalPages, int objType, int objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType)
        {
            if (Request.IsAjaxRequest())
            {
                Pager pager = null;
                //string[] powerTypes = null;
                //if (!string.IsNullOrWhiteSpace(powerType))
                //{
                //    powerTypes = powerType.Split(new char[] { '_' });
                //}
                string[] powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
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
                if (totalPages == -1 && objIDs > 0)
                {
                    int totalRows = 0;
                    if (granularity == "day")//按天
                    {
                        totalRows = _analogHistoryRepos.GetEnergyStatisDayCount(objType, objIDs, startTime, endTime, powerTypes);
                    }
                    else//按月
                    {
                        totalRows = _analogHistoryRepos.GetEnergyStatisMonthCount(objType, objIDs, startTime, endTime, powerTypes);
                    }
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
                        var timeList = _analogHistoryRepos.GetDateByDay(objType, objIDs, startTime, endTime, powerTypes).OrderBy(x => x).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetDayEnergyByGranularityPower(objType, objIDs, startTime, endTime, powerTypes, 1).ToList();
                    }
                    else//按月
                    {
                        var timeList = _analogHistoryRepos.GetDateByMonth(objType, objIDs, startTime, endTime, powerTypes).OrderBy(x => x).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetMonthEnergyByGranularityPower(objType, objIDs, startTime, endTime, powerTypes, 1).ToList();
                    }
                    var powerNameList = dataList.Select(x => x.PowerName).Distinct();
                    foreach (var item in powerNameList)
                    {
                        var dataItem = new
                        {
                            powerName = item,
                            dataList = dataList.Where(x => x.PowerName == item).ToList()
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
        /// Ajax获取单个时间分类对比分析图表数据（不分页）
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
        public ActionResult GetVGCElecChartAjaxNoPage(int objType, int objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType)
        {
            if (Request.IsAjaxRequest())
            {
                //string[] powerTypes = null;
                //if (!string.IsNullOrWhiteSpace(powerType))
                //{
                //    powerTypes = powerType.Split(new char[] { '_' });
                //}
                string[] powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
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
                if (objIDs > 0)
                {
                    if (granularity == "day")//按天
                    {
                        totalRows = _analogHistoryRepos.GetEnergyStatisDayCount(objType, objIDs, startTime, endTime, powerTypes);
                    }
                    else//按月
                    {
                        totalRows = _analogHistoryRepos.GetEnergyStatisMonthCount(objType, objIDs, startTime, endTime, powerTypes);
                    }
                }
                IList list = new ArrayList();
                IList indexValList = null;
                if (objIDs > 0)
                {
                    IList<ChartStatisEntity> dataList = null;
                    if (granularity == "day")//按天
                    {
                        var timeList = _analogHistoryRepos.GetDateByDay(objType, objIDs, startTime, endTime, powerTypes).OrderBy(x => x).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetDayEnergyByGranularityPower(objType, objIDs, startTime, endTime, powerTypes, 1).ToList();
                    }
                    else//按月
                    {
                        var timeList = _analogHistoryRepos.GetDateByMonth(objType, objIDs, startTime, endTime, powerTypes).OrderBy(x => x).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetMonthEnergyByGranularityPower(objType, objIDs, startTime, endTime, powerTypes, 1).ToList();
                    }
                    var powerNameList = dataList.Select(x => x.PowerName).Distinct();
                    foreach (var item in powerNameList)
                    {
                        var dataItem = new
                        {
                            powerName = item,
                            dataList = dataList.Where(x => x.PowerName == item).ToList()
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
        /// 导出单个时间分类对比分析数据Excel
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="granularity"></param>
        /// <param name="powerType"></param>
        /// <returns></returns>
        public ActionResult GetVGCElecExcel(int objType, int objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType)
        {
            if (objIDs > 0)
            {
                IList list = null;
                //string[] powerTypes = null;
                //if (!string.IsNullOrWhiteSpace(powerType))
                //{
                //    powerTypes = powerType.Split(new char[] { '_' });
                //}
                string[] powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
                if (granularity == "day")//按天
                {
                    double sum = _analogHistoryRepos.GetEnergyStatisDaySum(objType, objIDs, startTime, endTime, powerTypes);
                    list = _analogHistoryRepos.GetDayEnergyByGranularityPower(objType, objIDs, startTime, endTime, powerTypes, sum).ToList();
                }
                else//按月
                {
                    double sum = _analogHistoryRepos.GetEnergyStatisMonthSum(objType, objIDs, startTime, endTime, powerTypes);
                    list = _analogHistoryRepos.GetMonthEnergyByGranularityPower(objType, objIDs, startTime, endTime, powerTypes, sum).ToList();
                }
                if (list != null)
                {
                    string title = "能耗单个时间分类对比分析(" + startTime.ToString("yyyy-MM-dd") + "_" + endTime.ToString("yyyy-MM-dd") + ")";
                    string[] headers = { "日期", "能耗类型", "用能值", "所占比例" };
                    string[] properties = { "TimeBlock", "PowerName", "StatisSVal", "SPercentage" };
                    return this.Excel(list, "能耗单个时间分类对比分析.xls", title, headers, properties);
                }
            }
            return null;
        }

        /// <summary>
        /// 获取单个时间分类对比分析数据
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="granularity"></param>
        /// <param name="powerType"></param>
        /// <returns></returns>
        public ActionResult GetVGCElecAnalysisAjax(int objType, int objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType)
        {
            if (Request.IsAjaxRequest() && objIDs > 0)
            {
                //string[] powerTypes = null;
                //if (!string.IsNullOrWhiteSpace(powerType))
                //{
                //    powerTypes = powerType.Split(new char[] { '_' });
                //}
                string[] powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
                List<ChartStatisEntity> list = null;
                if (granularity == "day")
                {
                    list = _analogHistoryRepos.GetDayEnergyByGranularityPower(objType, objIDs, startTime, endTime, powerTypes, 1).ToList();
                }
                else
                {
                    list = _analogHistoryRepos.GetMonthEnergyByGranularityPower(objType, objIDs, startTime, endTime, powerTypes, 1).ToList();
                }
                int totalRows = list.Count;
                if (totalRows > 0)
                {
                    double totalSum = list.Sum(x => x.StatisVal);
                    var powerNameList = list.Select(x => x.PowerName).Distinct();
                    IList powerList = new ArrayList();
                    foreach (var item in powerNameList)
                    {
                        var powerSum = list.Where(x => x.PowerName == item).Sum(x => x.StatisVal);
                        var powerItem = new
                        {
                            powerName = item,
                            powerSum = powerSum.ToString("f1"),
                            powerPercentage = (powerSum * 100 / totalSum).ToString("f1") + "%"
                        };
                        powerList.Add(powerItem);
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
                        powerNameList = powerNameList,
                        powerList = powerList,
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
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetHElecAjax(int currentPage, int totalPages, int objType, string objIDs, DateTime startTime, DateTime endTime, string powerType, double sum,string statisticMode)
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
                IList areaList = new ArrayList();
                if (statisticMode != "totalEnergy")
                {
                    foreach (int id in ids)
                    {
                        if (_buildingRepos.GetBuilding(id).BDI_Area == null) areaList.Add(_buildingRepos.GetBuilding(id).BDI_Name);
                    }
                }
                IList list = _analogHistoryRepos.GetEnergy(objType, ids, powerTypes, startTime, endTime, sum,statisticMode).ToList();             
                int a = 1;
                String buidingNameList = "";
                if (statisticMode != "totalEnergy")
                {
                    foreach (var item in areaList)
                    {
                        if (a == 1)
                        {
                            buidingNameList += item;
                            a++;
                        }
                        else
                        {
                            buidingNameList = buidingNameList + "和" + item;
                        }
                    }
                }
                var resultData = new
                {
                    totalPages = -1,
                    yAxisTitle = yAxisTitle,
                    totalSum = sum,
                    data = list,
                    buidingNameList = buidingNameList
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 获取多个对比分析查询数据提供给手机端使用
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerType"></param>
        /// <param name="sum"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetHElecAjaxForMobile(string objIDs, DateTime startTime, DateTime endTime)
        {
            //string[] powerTypes = null;
            //if (!string.IsNullOrWhiteSpace(powerType))
            //{
            //    powerTypes = powerType.Split(new char[] { '_' });
            //}
            //提供给手机端使用，供用户查询房间能耗所以有些参数写死
            int objType = 5;
            string powerType = "001";
            string statisticMode = "totalEnergy";
            double sum = 0;

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
            IList areaList = new ArrayList();
            if (statisticMode != "totalEnergy")
            {
                foreach (int id in ids)
                {
                    if (_buildingRepos.GetBuilding(id).BDI_Area == null) areaList.Add(_buildingRepos.GetBuilding(id).BDI_Name);
                }
            }
            IList list = _analogHistoryRepos.GetEnergy(objType, ids, powerTypes, startTime, endTime, sum, statisticMode).ToList();
            int a = 1;
            String buidingNameList = "";
            if (statisticMode != "totalEnergy")
            {
                foreach (var item in areaList)
                {
                    if (a == 1)
                    {
                        buidingNameList += item;
                        a++;
                    }
                    else
                    {
                        buidingNameList = buidingNameList + "和" + item;
                    }
                }
            }
            var resultData = new
            {
                totalPages = -1,
                yAxisTitle = yAxisTitle,
                data = list,
                buidingNameList = buidingNameList
            };
            return Json(resultData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Ajax获取多个对比分析
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerType"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetHElecAnalysisAjax(int objType, string objIDs, DateTime startTime, DateTime endTime, string powerType,string statisticMode)
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
                IList areaList = new ArrayList();
                if (statisticMode != "totalEnergy")
                {
                    foreach (int id in ids)
                    {
                        if (_buildingRepos.GetBuilding(id).BDI_Area == null) areaList.Add(_buildingRepos.GetBuilding(id).BDI_Name);
                    }
                }
                List<ChartStatisEntity> list = _analogHistoryRepos.GetEnergy(objType, ids, powerTypes, startTime, endTime, 1,statisticMode).ToList();
                int totalRows = list.Count;
                if (totalRows > 0)
                {
                    double sum = list.Sum(x => Convert.ToDouble(x.StatisSVal));
                    var maxObj = list.OrderByDescending(x => Convert.ToDouble(x.StatisSVal)).FirstOrDefault();
                    maxObj.Sum = sum;
                    maxObj = list.OrderByDescending(x => Convert.ToDouble(x.StatisSVal)).FirstOrDefault();
                    var minObj = list.OrderBy(x => Convert.ToDouble(x.StatisSVal)).FirstOrDefault();
                    minObj.Sum = sum;
                    minObj = list.OrderBy(x => Convert.ToDouble(x.StatisSVal)).FirstOrDefault();
                    var average = sum / totalRows;
                    var greaterEqualAverageCount = list.Where(x => Convert.ToDouble(x.StatisSVal) >= average).Count();
                    var smallerAverageCount = list.Where(x => Convert.ToDouble(x.StatisSVal) < average).Count();
                    int a = 1;
                    String buidingNameList = "";
                    if (statisticMode != "totalEnergy")
                    {
                        foreach (var item in areaList)
                        {
                            if (a == 1)
                            {
                                buidingNameList += item;
                                a++;
                            }
                            else
                            {
                                buidingNameList = buidingNameList + "和" + item;
                            }
                        }
                    }
                    var resultData = new
                    {
                        totalRows = totalRows,
                        totalSum = sum.ToString("f1"),
                        average = average.ToString("f1"),
                        greaterEqualAverageCount = greaterEqualAverageCount,
                        smallerAverageCount = smallerAverageCount,
                        maxObj = maxObj,
                        minObj = minObj,
                        buidingNameList = buidingNameList
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
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetHElecExcel(int objType, string objIDs, DateTime startTime, DateTime endTime, string powerType, string attachName,string statisticMode)
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
            double sum = _analogHistoryRepos.GetAllEnergyByIDs(objType, ids, startTime, endTime, powerTypes,statisticMode);
            var list = _analogHistoryRepos.GetEnergy(objType, ids, powerTypes, startTime, endTime, sum,statisticMode).ToList();
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
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetHGElecListAjax(int currentPage, int totalPages, int objType, string objIDs, DateTime startTime, DateTime endTime, string granularity, string assignMonth, string assignDay, string powerType, double sum,string statisticMode)
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
                IList areaList = new ArrayList();
                if (statisticMode != "totalEnergy")
                {
                    foreach (int id in ids)
                    {
                        if (_buildingRepos.GetBuilding(id).BDI_Area == null) areaList.Add(_buildingRepos.GetBuilding(id).BDI_Name);
                    }
                }
                if (totalPages == -1)
                {
                    int totalRows = 0;
                    if (granularity == "day")//按天
                    {
                        totalRows = _analogHistoryRepos.GetDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, 1,statisticMode).Count();
                    }
                    else if(granularity == "month")//按月
                    {
                        totalRows = _analogHistoryRepos.GetMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, 1,statisticMode).Count();
                    }
                    else if (granularity == "specificMonth")//按指定月份
                    {
                        totalRows = _analogHistoryRepos.GetAssignMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignMonth, powerTypes, 1,statisticMode).Count();
                    }
                    else//按指定日期
                        totalRows = _analogHistoryRepos.GetAssignDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignDay, powerTypes, 1,statisticMode).Count();
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
                            //sum = _analogHistoryRepos.GetEnergySum(objType, ids, startTime, endTime, powerTypes);
                            sum = _analogHistoryRepos.GetDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, 1, statisticMode).Sum(x => Convert.ToDouble(x.StatisSVal));
                        }
                        list = _analogHistoryRepos.GetDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, sum, statisticMode).Skip(pager.StartRow).Take(pager.PageSize).ToList();                        
                    }
                    else if(granularity == "month")//按月
                    {
                        if (sum < 0)
                        {
                            //sum = _analogHistoryRepos.GetEnergySum(objType, ids, startTime, endTime, powerTypes);
                            sum = _analogHistoryRepos.GetMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, 1, statisticMode).Sum(x => Convert.ToDouble(x.StatisSVal));
                        }
                        list = _analogHistoryRepos.GetMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, sum, statisticMode).Skip(pager.StartRow).Take(pager.PageSize).ToList();                      
                    }
                    else if (granularity == "specificMonth")//按指定月份
                    {
                        if (sum < 0)
                        {
                            //sum = _analogHistoryRepos.GetEnergySum(objType, ids, startTime, endTime, powerTypes);
                            sum = _analogHistoryRepos.GetAssignMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignMonth, powerTypes, 1, statisticMode).Sum(x => Convert.ToDouble(x.StatisSVal));
                        }
                        list = _analogHistoryRepos.GetAssignMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignMonth, powerTypes, sum, statisticMode).Skip(pager.StartRow).Take(pager.PageSize).ToList();                        
                    }
                    else//按指定日期
                    {
                        if (sum < 0)
                        {
                            //sum = _analogHistoryRepos.GetEnergySum(objType, ids, startTime, endTime, powerTypes);
                            sum = _analogHistoryRepos.GetAssignDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignDay, powerTypes, 1, statisticMode).Sum(x => Convert.ToDouble(x.StatisSVal));
                        }
                        list = _analogHistoryRepos.GetAssignDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignDay, powerTypes, sum, statisticMode).Skip(pager.StartRow).Take(pager.PageSize).ToList();                        
                    }
                }
                int a = 1;
                String buidingNameList = "";
                if (statisticMode != "totalEnergy")
                {
                    foreach (var item in areaList)
                    {
                        if (a == 1)
                        {
                            buidingNameList += item;
                            a++;
                        }
                        else
                        {
                            buidingNameList = buidingNameList + "和" + item;
                        }
                    }
                }
                var resultData = new
                {
                    totalPages = pager.TotalPages,
                    yAxisTitle = yAxisTitle,
                    totalSum = sum,
                    data = list,
                    buidingNameList = buidingNameList
                };

                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 获取多个时间对比分析一览表查询数据提供给手机端使用
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
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetHGElecListAjaxForMobile(int currentPage, int totalPages, string objIDs, DateTime startTime, DateTime endTime, string granularity, string assignMonth, string assignDay,double sum)
        {
            //提供给手机端查询房间能耗，所以写死一些参数
            int objType = 5;
            string powerType = "001";           
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
            IList areaList = new ArrayList();
            if (statisticMode != "totalEnergy")
            {
                foreach (int id in ids)
                {
                    if (_buildingRepos.GetBuilding(id).BDI_Area == null) areaList.Add(_buildingRepos.GetBuilding(id).BDI_Name);
                }
            }
            //if (totalPages == -1)
            //{
                //int totalRows = 0;
                //if (granularity == "day")//按天
                //{
                //    totalRows = _analogHistoryRepos.GetDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, 1, statisticMode).Count();
                //}
                //else if (granularity == "month")//按月
                //{
                //    totalRows = _analogHistoryRepos.GetMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, 1, statisticMode).Count();
                //}
                //else if (granularity == "specificMonth")//按指定月份
                //{
                //    totalRows = _analogHistoryRepos.GetAssignMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignMonth, powerTypes, 1, statisticMode).Count();
                //}
                //else//按指定日期
                //    totalRows = _analogHistoryRepos.GetAssignDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignDay, powerTypes, 1, statisticMode).Count();
                //pager = new Pager(1, totalRows);
            //}
            //else
            //{
            //    pager = new Pager(currentPage, totalPages, false);
            //}
            IList list = null;
            //if (pager.TotalPages > 0)
            //{

                if (granularity == "day")//按天
                {
                    if (sum < 0)
                    {
                        //sum = _analogHistoryRepos.GetEnergySum(objType, ids, startTime, endTime, powerTypes);
                        sum = _analogHistoryRepos.GetDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, 1, statisticMode).Sum(x => Convert.ToDouble(x.StatisSVal));
                    }
                    list = _analogHistoryRepos.GetDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, sum, statisticMode).ToList();
                }
                else if (granularity == "month")//按月
                {
                    if (sum < 0)
                    {
                        //sum = _analogHistoryRepos.GetEnergySum(objType, ids, startTime, endTime, powerTypes);
                        sum = _analogHistoryRepos.GetMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, 1, statisticMode).Sum(x => Convert.ToDouble(x.StatisSVal));
                    }
                    list = _analogHistoryRepos.GetMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, sum, statisticMode).ToList();
                }
                else if (granularity == "specificMonth")//按指定月份
                {
                    if (sum < 0)
                    {
                        //sum = _analogHistoryRepos.GetEnergySum(objType, ids, startTime, endTime, powerTypes);
                        sum = _analogHistoryRepos.GetAssignMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignMonth, powerTypes, 1, statisticMode).Sum(x => Convert.ToDouble(x.StatisSVal));
                    }
                    list = _analogHistoryRepos.GetAssignMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignMonth, powerTypes, sum, statisticMode).ToList();
                }
                else//按指定日期
                {
                    if (sum < 0)
                    {
                        //sum = _analogHistoryRepos.GetEnergySum(objType, ids, startTime, endTime, powerTypes);
                        sum = _analogHistoryRepos.GetAssignDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignDay, powerTypes, 1, statisticMode).Sum(x => Convert.ToDouble(x.StatisSVal));
                    }
                    list = _analogHistoryRepos.GetAssignDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignDay, powerTypes, sum, statisticMode).ToList();
                }
            //}
            int a = 1;
            String buidingNameList = "";
            if (statisticMode != "totalEnergy")
            {
                foreach (var item in areaList)
                {
                    if (a == 1)
                    {
                        buidingNameList += item;
                        a++;
                    }
                    else
                    {
                        buidingNameList = buidingNameList + "和" + item;
                    }
                }
            }
            List<DateTime> timeList1 = new List<DateTime>();
            List<String> nameList = new List<String>();
            List<DateTime> realTimeList = new List<DateTime>();
            List<String> dateTimeList = new List<String>();
            List<String> dateEnergyVal = new List<String>();
            List<String> realDateEnergyVal = new List<String>();
            foreach (ChartStatisEntity item in list)
            {
                if (!timeList1.Contains(item.Time))
                    timeList1.Add(item.Time);
                if (!nameList.Contains(item.Name))
                    nameList.Add(item.Name);
            }
            DateTime startDateTime = timeList1.FirstOrDefault();
            DateTime endDateTime = timeList1.LastOrDefault();
            DateTime midDateTime = startDateTime;               
            ChartStatisEntity sub = new ChartStatisEntity();
            List<String>  timeList = new List<String>();
            foreach (ChartStatisEntity item in list)
            {
                if (item.Time.CompareTo(midDateTime) > 0)
                {
                    while (midDateTime.CompareTo(item.Time) < 0)
                    {
                        realDateEnergyVal.Add(null);
                        if (granularity == "day")
                        {
                            timeList.Add(midDateTime.ToString("yyyy-MM-dd"));
                            midDateTime = midDateTime.AddDays(1);
                        }
                        else if (granularity == "month")
                        {
                            timeList.Add(midDateTime.ToString("yyyy-MM"));
                            midDateTime = midDateTime.AddMonths(1);
                        }                        
                    }  
                    realDateEnergyVal.Add(item.StatisSVal);                  
                    if (granularity == "day")
                    {
                        timeList.Add(midDateTime.ToString("yyyy-MM-dd"));
                        if (endDateTime.CompareTo(item.Time) == 0)
                        {
                            midDateTime = startDateTime;
                            continue;
                        }
                        midDateTime = midDateTime.AddDays(1);
                    }
                    else if (granularity == "month")
                    {
                        timeList.Add(midDateTime.ToString("yyyy-MM"));
                        if (endDateTime.CompareTo(item.Time) == 0)
                        {
                            midDateTime = startDateTime;
                            continue;
                        }
                        midDateTime = midDateTime.AddMonths(1);
                    }         
                }else{
                    realDateEnergyVal.Add(item.StatisSVal);
                    if (granularity == "day")
                    {
                        timeList.Add(midDateTime.ToString("yyyy-MM-dd"));
                        if (endDateTime.CompareTo(item.Time) == 0)
                        {
                            midDateTime = startDateTime;
                            continue;
                        }
                        midDateTime = midDateTime.AddDays(1);
                    }
                    else if (granularity == "month")
                    {
                        timeList.Add(midDateTime.ToString("yyyy-MM"));
                        if (endDateTime.CompareTo(item.Time) == 0)
                        {
                            midDateTime = startDateTime;
                            continue;
                        }
                        midDateTime = midDateTime.AddMonths(1);
                    }         
                }
            }
         
            while(true) {
                realDateEnergyVal.Add(null);
                if (granularity == "day")
                {
                    timeList.Add(midDateTime.ToString("yyyy-MM-dd"));
                    if (endDateTime.CompareTo(midDateTime) == 0)
                    {
                        break;
                    }
                    midDateTime = midDateTime.AddDays(1);
                }
                else if (granularity == "month")
                {
                    timeList.Add(midDateTime.ToString("yyyy-MM"));
                    if (endDateTime.CompareTo(midDateTime) == 0)
                    {
                        break;
                    }
                    midDateTime = midDateTime.AddMonths(1);
                }         
            }          
            var resultData = new
            {             
                yAxisTitle = yAxisTitle,
                totalSum = sum,
                dateTimeList = timeList,
                realDateEnergyVal = realDateEnergyVal,
                nameList = nameList
            };
            return Json(resultData, JsonRequestBehavior.AllowGet);         
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
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetHGElecListAjaxNoPage(int objType, string objIDs, DateTime startTime, DateTime endTime, string granularity, string assignMonth, string assignDay, string powerType, double sum, string statisticMode)
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
                IList areaList = new ArrayList();
                if (statisticMode != "totalEnergy")
                {
                    foreach (int id in ids)
                    {
                        if (_buildingRepos.GetBuilding(id).BDI_Area == null) areaList.Add(_buildingRepos.GetBuilding(id).BDI_Name);
                    }
                }
                int totalRows = 0;
                if (ids.Length > 0)
                {
                    if (granularity == "day")//按天
                    {
                        totalRows = _analogHistoryRepos.GetDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, 1,statisticMode).Count();
                    }
                    else if (granularity == "month")//按月
                    {
                        totalRows = _analogHistoryRepos.GetMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, 1,statisticMode).Count();
                    }
                    else if (granularity == "specificMonth")//按指定月份
                    {
                        totalRows = _analogHistoryRepos.GetAssignMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignMonth, powerTypes, 1,statisticMode).Count();
                    }
                    else//按指定日期
                        totalRows = _analogHistoryRepos.GetAssignDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignDay, powerTypes, 1,statisticMode).Count();
                }
                IList list = null;
                if (ids.Length > 0)
                {
                    if (granularity == "day")//按天
                    {
                        if (sum < 0)
                        {
                            //sum = _analogHistoryRepos.GetEnergySum(objType, ids, startTime, endTime, powerTypes);
                            sum = _analogHistoryRepos.GetDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, 1, statisticMode).Sum(x => Convert.ToDouble(x.StatisSVal));
                        }
                        list = _analogHistoryRepos.GetDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, sum,statisticMode).ToList();
                    }
                    else if (granularity == "month")//按月
                    {
                        if (sum < 0)
                        {
                            //sum = _analogHistoryRepos.GetEnergySum(objType, ids, startTime, endTime, powerTypes);
                            sum = _analogHistoryRepos.GetMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, 1, statisticMode).Sum(x => Convert.ToDouble(x.StatisSVal));
                        }
                        list = _analogHistoryRepos.GetMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, sum, statisticMode).ToList();
                    }
                    else if (granularity == "specificMonth")//按指定月份
                    {
                        if (sum < 0)
                        {
                            //sum = _analogHistoryRepos.GetEnergySum(objType, ids, startTime, endTime, powerTypes);
                            sum = _analogHistoryRepos.GetAssignMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignMonth, powerTypes, 1, statisticMode).Sum(x => Convert.ToDouble(x.StatisSVal));
                        }
                        list = _analogHistoryRepos.GetAssignMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignMonth, powerTypes, sum, statisticMode).ToList();
                    }
                    else//按指定日期
                    {
                        if (sum < 0)
                        {
                            //sum = _analogHistoryRepos.GetEnergySum(objType, ids, startTime, endTime, powerTypes);
                            sum = _analogHistoryRepos.GetAssignDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignDay, powerTypes, 1, statisticMode).Sum(x => Convert.ToDouble(x.StatisSVal));
                        }
                        list = _analogHistoryRepos.GetAssignDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignDay, powerTypes, sum, statisticMode).ToList();
                    }
                }
                int a = 1;
                String buidingNameList = "";
                if (statisticMode != "totalEnergy")
                {
                    foreach (var item in areaList)
                    {
                        if (a == 1)
                        {
                            buidingNameList += item;
                            a++;
                        }
                        else
                        {
                            buidingNameList = buidingNameList + "和" + item;
                        }
                    }
                }
                var resultData = new
                {
                    count = totalRows,
                    yAxisTitle = yAxisTitle,
                    totalSum = sum,
                    data = list,
                    buidingNameList = buidingNameList
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
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetHGElecChartAjax(int currentPage, int totalPages, int objType, string objIDs, DateTime startTime, DateTime endTime, string granularity, string assignMonth, string assignDay, string powerType,string statisticMode)
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
                IList areaList = new ArrayList();
                if (statisticMode != "totalEnergy")
                {
                    foreach (int id in ids)
                    {
                        if (_buildingRepos.GetBuilding(id).BDI_Area == null) areaList.Add(_buildingRepos.GetBuilding(id).BDI_Name);
                    }
                }
                if (totalPages == -1)
                {
                    int totalRows = 0;
                    if (granularity == "day")//按天
                    {
                        totalRows = _analogHistoryRepos.GetDateByDayForAnalyze(objType, ids, startTime, endTime, powerTypes,statisticMode).Count();
                    }
                    else if(granularity == "month")//按月
                    {
                        totalRows = _analogHistoryRepos.GetDateByMonthForAnalyze(objType, ids, startTime, endTime, powerTypes,statisticMode).Count();
                    }
                    else if (granularity == "specificMonth")//按指定月份
                    {
                        totalRows = _analogHistoryRepos.GetDateByAssignMonthForAnalyze(objType, ids, startTime, endTime, assignMonth, powerTypes,statisticMode).Count();
                    }
                    else//按指定日期
                        totalRows = _analogHistoryRepos.GetDateByAssignDayForAnalyze(objType, ids, startTime, endTime, assignDay, powerTypes,statisticMode).Count();
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
                        var timeList = _analogHistoryRepos.GetDateByDayForAnalyze(objType, ids, startTime, endTime, powerTypes,statisticMode).OrderBy(x => x).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, 1,statisticMode).ToList();                       
                    }
                    else if (granularity == "month")//按月
                    {
                        var timeList = _analogHistoryRepos.GetDateByMonthForAnalyze(objType, ids, startTime, endTime, powerTypes,statisticMode).OrderBy(x => x).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, 1,statisticMode).ToList();
                    }
                    else if (granularity == "specificMonth")//按指定月份
                    {
                        var timeList = _analogHistoryRepos.GetDateByAssignMonthForAnalyze(objType, ids, startTime, endTime, assignMonth, powerTypes,statisticMode).OrderBy(x => x).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetAssignMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignMonth, powerTypes, 1,statisticMode).ToList();
                    }
                    else//按指定日期
                    {
                        var timeList = _analogHistoryRepos.GetDateByAssignDayForAnalyze(objType, ids, startTime, endTime, assignDay, powerTypes,statisticMode).OrderBy(x => x).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetAssignDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignDay, powerTypes, 1,statisticMode).ToList();
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
                int a = 1;
                String buidingNameList = "";
                if (statisticMode != "totalEnergy")
                {
                    foreach (var item in areaList)
                    {
                        if (a == 1)
                        {
                            buidingNameList += item;
                            a++;
                        }
                        else
                        {
                            buidingNameList = buidingNameList + "和" + item;
                        }
                    }
                }
                var resultData = new
                {
                    totalPages = pager.TotalPages,
                    yAxisTitle = yAxisTitle,
                    indexValList = indexValList,
                    data = list,
                    buidingNameList = buidingNameList
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
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetHGElecChartAjaxNoPage(int objType, string objIDs, DateTime startTime, DateTime endTime, string granularity, string assignMonth, string assignDay, string powerType, string statisticMode)
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
                IList areaList = new ArrayList();
                if (statisticMode != "totalEnergy")
                {
                    foreach (int id in ids)
                    {
                        if (_buildingRepos.GetBuilding(id).BDI_Area == null) areaList.Add(_buildingRepos.GetBuilding(id).BDI_Name);
                    }  
                }
                int totalRows = 0;
                if (granularity == "day")//按天
                {
                    totalRows = _analogHistoryRepos.GetDateByDayForAnalyze(objType, ids, startTime, endTime, powerTypes,statisticMode).Count();
                }
                else if(granularity == "month")//按月
                {
                    totalRows = _analogHistoryRepos.GetDateByMonthForAnalyze(objType, ids, startTime, endTime, powerTypes, statisticMode).Count();
                }
                else if (granularity == "specificMonth")//按指定月份
                {
                    totalRows = _analogHistoryRepos.GetDateByAssignMonthForAnalyze(objType, ids, startTime, endTime, assignMonth, powerTypes, statisticMode).Count();
                }
                else//按指定日期
                    totalRows = _analogHistoryRepos.GetDateByAssignDayForAnalyze(objType, ids, startTime, endTime, assignDay, powerTypes, statisticMode).Count();
                IList list = new ArrayList();
                IList indexValList = null;
                if (ids.Length > 0)
                {
                    IList<ChartStatisEntity> dataList = null;
                    if (granularity == "day")//按天
                    {
                        var timeList = _analogHistoryRepos.GetDateByDayForAnalyze(objType, ids, startTime, endTime, powerTypes, statisticMode).OrderBy(x => x).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, 1, statisticMode).ToList();
                    }
                    else if(granularity == "month")//按月
                    {
                        var timeList = _analogHistoryRepos.GetDateByMonthForAnalyze(objType, ids, startTime, endTime, powerTypes, statisticMode).OrderBy(x => x).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, 1, statisticMode).ToList();
                    }
                    else if (granularity == "specificMonth")//按指定月份
                    {
                        var timeList = _analogHistoryRepos.GetDateByAssignMonthForAnalyze(objType, ids, startTime, endTime, assignMonth, powerTypes, statisticMode).OrderBy(x => x).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetAssignMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignMonth, powerTypes, 1, statisticMode).ToList();
                    }
                    else//按指定日期
                    {
                        var timeList = _analogHistoryRepos.GetDateByAssignDayForAnalyze(objType, ids, startTime, endTime, assignDay, powerTypes, statisticMode).OrderBy(x => x).ToList();
                        startTime = timeList.Min();
                        endTime = timeList.Max();
                        dataList = _analogHistoryRepos.GetAssignDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignDay, powerTypes, 1, statisticMode).ToList();
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
                int a =1;
                String buidingNameList = "";
				if (statisticMode != "totalEnergy")
                {
                    foreach (var item in areaList)
                    {
                        if (a == 1)
                        {
                            buidingNameList += item;
                            a++;
                        }
                        else
                        {
                            buidingNameList = buidingNameList + "和" + item;
                        }
                    } 
                }
                var resultData = new
                {
                    count = totalRows,
                    yAxisTitle = yAxisTitle,
                    indexValList = indexValList,
                    data = list,
                    buidingNameList = buidingNameList
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
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetHGElecAnalysisAjax(int objType, string objIDs, DateTime startTime, DateTime endTime, string granularity, string assignMonth, string assignDay, string powerType, string statisticMode)
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
                IList areaList = new ArrayList();
                if (statisticMode != "totalEnergy")
                {
                    foreach (int id in ids)
                    {
                        if (_buildingRepos.GetBuilding(id).BDI_Area == null) areaList.Add(_buildingRepos.GetBuilding(id).BDI_Name);
                    }  
                }
                String buidingNameList = "";
               
                if (granularity == "day")
                {
                    list = _analogHistoryRepos.GetDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, 1, statisticMode).ToList();
                }
                else if(granularity == "month")
                {
                    list = _analogHistoryRepos.GetMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, 1, statisticMode).ToList();
                }
                else if (granularity == "specificMonth")//按指定月份
                {
                    list = _analogHistoryRepos.GetAssignMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignMonth, powerTypes, 1, statisticMode).ToList();
                }
                else
                    list = _analogHistoryRepos.GetAssignDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignDay, powerTypes, 1, statisticMode).ToList();
                int totalRows = list.Count;
                if (totalRows > 0)
                {
                    double totalSum = list.Sum(x => Convert.ToDouble(x.StatisSVal));
                    var queryNameList = list.Select(x => x.Name).Distinct();
                    IList queryList = new ArrayList();
                    foreach (var item in queryNameList)
                    {
                        var querySum = list.Where(x => x.Name == item).Sum(x => Convert.ToDouble(x.StatisSVal));
                        var dataItem = new
                        {
                            queryName = item,
                            querySum = querySum.ToString("f1"),
                            queryPercentage = (querySum * 100 / totalSum).ToString("f1") + "%"
                        };
                        queryList.Add(dataItem);
                    }
                    var maxObj = list.OrderByDescending(x => Convert.ToDouble(x.StatisSVal)).FirstOrDefault();
                    maxObj.Sum = totalSum;
                    var minObj = list.OrderBy(x => Convert.ToDouble(x.StatisSVal)).FirstOrDefault();
                    minObj.Sum = totalSum;
                    var average = totalSum / totalRows;
                    var greaterEqualAverageCount = list.Where(x => Convert.ToDouble(x.StatisSVal) >= average).Count();
                    var smallerAverageCount = list.Where(x => Convert.ToDouble(x.StatisSVal) < average).Count();
                     int a = 1;
                    if (statisticMode != "totalEnergy")
                    {
                        foreach (var item in areaList)
                        {
                            if (a == 1)
                            {
                                buidingNameList += item;
                                a++;
                            }
                            else
                            {
                                buidingNameList = buidingNameList + "和" + item;
                            }
                        } 
                    }
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
                        minObj = minObj,
                        buidingNameList = buidingNameList
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
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetHGElecExcel(int objType, string objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType, string attachName,string statisticMode, string assignMonth, string assignDay)
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
                double sum = _analogHistoryRepos.GetEnergySum(objType, ids, startTime, endTime, powerTypes,statisticMode);
                list = _analogHistoryRepos.GetDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, sum,statisticMode).ToList();
            }
            else if (granularity == "month")//按月
            {
                double sum = _analogHistoryRepos.GetEnergySum(objType, ids, startTime, endTime, powerTypes,statisticMode);
                list = _analogHistoryRepos.GetMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, powerTypes, sum,statisticMode).ToList();
            }
            else if (granularity == "specificMonth")//按指定月份
            {                
                    double sum = _analogHistoryRepos.GetEnergySum(objType, ids, startTime, endTime, powerTypes,statisticMode);
                    list = _analogHistoryRepos.GetAssignMonthEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignMonth, powerTypes, sum, statisticMode).ToList();            
            }
            else//按指定日期
            {               
                    double sum = _analogHistoryRepos.GetEnergySum(objType, ids, startTime, endTime, powerTypes,statisticMode);
                    list = _analogHistoryRepos.GetAssignDayEnergyByIDsGranularityForAnalyze(objType, ids, startTime, endTime, assignDay, powerTypes, sum, statisticMode).ToList();               
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
        /// 跳转多个分类对比分析
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 10, VaryByParam = "none")]
        [AuthenticationFilter]
        public ActionResult HCElec()
        {
            var powerList = _powerClassRepos.GetStatisTypes();
            return View(powerList);
        }

        /// <summary>
        /// Ajax获取多个分类对比分析一览表查询数据
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerType"></param>
        /// <param name="sum"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetHCElecListAjax(int currentPage, int totalPages, int objType, string objIDs, DateTime startTime, DateTime endTime, string powerType, double sum, string statisticMode)
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
                IList areaList = new ArrayList();
                if (statisticMode != "totalEnergy")
                {
                    foreach (int id in ids)
                    {
                        if (_buildingRepos.GetBuilding(id).BDI_Area == null) areaList.Add(_buildingRepos.GetBuilding(id).BDI_Name);
                    }
                }
                if (totalPages == -1)
                {
                    int totalRows = _analogHistoryRepos.GetEnergyByIDsPower(objType, ids, startTime, endTime, powerTypes, 1, statisticMode).Count();
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
                        sum = _analogHistoryRepos.GetAllEnergyByIDs(objType, ids, startTime, endTime, powerTypes, statisticMode);
                    }
                    list = _analogHistoryRepos.GetEnergyByIDsPower(objType, ids, startTime, endTime, powerTypes, sum, statisticMode).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                }
                int a = 1;
                String buidingNameList = "";
                if (statisticMode != "totalEnergy")
                {
                    foreach (var item in areaList)
                    {
                        if (a == 1)
                        {
                            buidingNameList += item;
                            a++;
                        }
                        else
                        {
                            buidingNameList = buidingNameList + "和" + item;
                        }
                    }
                }
                var resultData = new
                {
                    totalPages = pager.TotalPages,
                    yAxisTitle = yAxisTitle,
                    totalSum = sum,
                    data = list,
                    buidingNameList = buidingNameList
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);                                
            }
            return null;
        }

        /// <summary>
        /// Ajax获取多个分类对比分析图表查询数据
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerType"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetHCElecChartAjax(int currentPage, int totalPages, int objType, string objIDs, DateTime startTime, DateTime endTime, string powerType,string statisticMode)
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
                IList areaList = new ArrayList();
                if (statisticMode != "totalEnergy")
                {
                    foreach (int id in ids)
                    {
                        if (_buildingRepos.GetBuilding(id).BDI_Area == null) areaList.Add(_buildingRepos.GetBuilding(id).BDI_Name);
                    } 
                }
                if (totalPages == -1)
                {
                    int totalRows = _analogHistoryRepos.GetEnergyIDs(objType, ids, startTime, endTime, powerTypes).Count();
                    pager = new Pager(1, totalRows);
                }
                else
                {
                    pager = new Pager(currentPage, totalPages, false);
                }
                IList list = null;              
                IList indexValList = null;             
                if (pager.TotalPages > 0)
                {
                    list = new ArrayList();
                    ids = _analogHistoryRepos.GetEnergyIDs(objType, ids, startTime, endTime, powerTypes).OrderBy(x => x).Skip(pager.StartRow).Take(pager.PageSize).ToArray();
                    var dataList = _analogHistoryRepos.GetEnergyByIDsPower(objType, ids, startTime, endTime, powerTypes, 1, statisticMode).ToList();
                    indexValList = dataList.Select(x => x.ID).Distinct().OrderBy(x => x).ToList();
                    var powerNameList = dataList.Select(x => x.PowerName).Distinct();                    
                    foreach (var item in powerNameList)
                    {
                        var dataItem = new
                        {
                            powerName = item,
                            dataList = dataList.Where(x => x.PowerName == item).ToList()
                        };                   
                        list.Add(dataItem);
                    }
                }
                int a = 1;
                String buidingNameList = "";
                if (statisticMode != "totalEnergy")
                {
                    foreach (var item in areaList) {
                        if (a == 1)
                        {
                            buidingNameList += item;
                            a++;
                        }
                        else
                        {
                            buidingNameList = buidingNameList + "和" + item;
                        }
                    }  
                }
                var resultData = new
                {
                    totalPages = pager.TotalPages,
                    yAxisTitle = yAxisTitle,
                    indexValList = indexValList,
                    data = list,
                    buidingNameList = buidingNameList
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// Ajax获取多个分类对比分析结果
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerType"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetHCElecAnalysisAjax(int objType, string objIDs, DateTime startTime, DateTime endTime, string powerType,string statisticMode)
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
                IList areaList = new ArrayList();
                if (statisticMode != "totalEnergy")
                {
                    foreach (int id in ids)
                    {
                        if (_buildingRepos.GetBuilding(id).BDI_Area == null) areaList.Add(_buildingRepos.GetBuilding(id).BDI_Name);
                    }  
                }
                var list = _analogHistoryRepos.GetEnergyByIDsPower(objType, ids, startTime, endTime, powerTypes, 1,statisticMode).ToList();
                int totalRows = list.Count;
                if (totalRows > 0)
                {
                    double totalSum = list.Sum(x => Convert.ToDouble(x.StatisSVal));
                    var powerNameList = list.Select(x => x.PowerName).Distinct();
                    IList powerList = new ArrayList();
                    foreach (var item in powerNameList)
                    {
                        var powerSum = list.Where(x => x.PowerName == item).Sum(x => Convert.ToDouble(x.StatisSVal));
                        var dataItem = new
                        {
                            powerName = item,
                            powerSum = powerSum.ToString("f1"),
                            powerPercentage = (powerSum * 100 / totalSum).ToString("f1") + "%"
                        };
                        powerList.Add(dataItem);
                    }
                    var maxObj = list.OrderByDescending(x => Convert.ToDouble(x.StatisSVal)).FirstOrDefault();
                    maxObj.Sum = totalSum;
                    var minObj = list.OrderBy(x => Convert.ToDouble(x.StatisSVal)).FirstOrDefault();
                    minObj.Sum = totalSum;
                    var average = totalSum / totalRows;
                    var greaterEqualAverageCount = list.Where(x => Convert.ToDouble(x.StatisSVal) >= average).Count();
                    var smallerAverageCount = list.Where(x => Convert.ToDouble(x.StatisSVal) < average).Count();
                    int a = 1;
                    String buidingNameList = "";
                    if (statisticMode != "totalEnergy")
                   {
                        foreach (var item in areaList)
                        {
                            if (a == 1)
                            {
                                buidingNameList += item;
                                a++;
                            }
                            else
                            {
                                buidingNameList = buidingNameList + "和" + item;
                            }
                        } 
                    }
                    var resultData = new
                    {
                        totalRows = totalRows,
                        totalSum = totalSum.ToString("f1"),
                        powerNameList = powerNameList,
                        powerList = powerList,
                        average = average.ToString("f1"),
                        greaterEqualAverageCount = greaterEqualAverageCount,
                        smallerAverageCount = smallerAverageCount,
                        maxObj = maxObj,
                        minObj = minObj,
                        buidingNameList = buidingNameList
                    };
                    return Json(resultData, JsonRequestBehavior.AllowGet);
                }
                return Json(new { totalRows = totalRows }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 导出多个分类对比分析Excel
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerType"></param>
        /// <param name="statisticMode"></param>
        /// <returns></returns>
        public ActionResult GetHCElecExcel(int objType, string objIDs, DateTime startTime, DateTime endTime, string powerType, string attachName,string statisticMode)
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
            double sum = _analogHistoryRepos.GetAllEnergyByIDs(objType, ids, startTime, endTime, powerTypes,statisticMode);
            var list = _analogHistoryRepos.GetEnergyByIDsPower(objType, ids, startTime, endTime, powerTypes, sum,statisticMode).ToList();
            if (list != null)
            {
                foreach (var item in list)
                {
                    item.Name = attachName + item.Name;
                }
                string title = "能耗多个分类对比分析(" + startTime.ToString("yyyy-MM-dd") + "_" + endTime.ToString("yyyy-MM-dd") + ")";
                string[] headers = { "查询对象", "能耗类型", "用能值", "所占比例" };
                string[] properties = { "Name", "PowerName", "StatisSVal", "SPercentage" };
                return this.Excel(list, "能耗多个分类对比分析.xls", title, headers, properties);
            }
            return null;
        }
    }
}
