namespace EnergyMonitor.Controllers.Admin.ReportsTemplate
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Linq;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using EnergyMonitor.Models.Repository.Interface;
    using EnergyMonitor.Models.Repository.Implement;
    using System.Collections.Generic;
    using System.Collections;

    /// <summary>
    /// Summary description for BuildingEnergyMonth.
    /// </summary>
    public partial class BuildingElecMonth : Telerik.Reporting.Report
    {
        public BuildingElecMonth()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        private void BuildingEnergyMonth_NeedDataSource(object sender, EventArgs e)
        {
            var buildingIDObj = ReportParameters["buildingId"].Value;
            var yearObj = ReportParameters["year"].Value;
            if (buildingIDObj != null)
            {
                int buildingId = Convert.ToInt32(buildingIDObj);
                int year = 0;
                if (yearObj != null)
                {
                    year = Convert.ToInt32(yearObj);
                    if (year <= 0)
                    {
                        year = DateTime.Now.Year;
                    }
                }
                IBuildingRepos buildingRepos = new BuildingRepos();
                var building = buildingRepos.GetBuilding(buildingId);
                if (building != null)
                {
                    string buildingName = building.BDI_Name;
                    double? buildingArea = building.BDI_Area;
                    double? buildingAreaSpe = building.BDI_AreaSpe;
                    textBox134.Value = buildingName;
                    if (buildingArea != null)
                    {
                        textBox135.Value = buildingArea.ToString();
                    }
                    if (buildingAreaSpe != null)
                    {
                        textBox137.Value = buildingAreaSpe.ToString();
                    }
                    ISystemProfileRepos systemProfileRepos = new SystemProfileRepos();
                    var priceDic = systemProfileRepos.GetAllPrice().ToDictionary(x => x.SP_ID, x => x.SP_Value);
                    var priceHighTime = priceDic["price_highTime"];
                    var priceNormalTime = priceDic["price_normalTime"];
                    var priceLowTime = priceDic["price_lowTime"];
                    textBox145.Value = priceHighTime;
                    textBox143.Value = priceNormalTime;
                    textBox141.Value = priceLowTime;

                    var timeDic = systemProfileRepos.GetByStartStr("time").ToDictionary(x => x.SP_ID, x=>x.SP_Value);
                    var timeHigh = timeDic["time_high"];
                    var timeNormal = timeDic["time_normal"];
                    var timeLow = timeDic["time_low"];

                    IPowerClassRepos powerClassRepos = new PowerClassRepos();
                    var elecPowers = (powerClassRepos.GetSubPowers("001").Select(x => x.PC_ID).ToArray().Union(new string[] { "001" })).ToArray();

                    IAnalogHistoryRepos analogHistoryRepos = new AnalogHistoryRepos();
                    DateTime startDate = DateTime.Parse(year + "-1-1");
                    DateTime endDate = startDate.AddYears(1);

                    var timeBlocks = timeHigh.Split(new char[] { ',' });
                    double[] highVals = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    foreach (var timeStr in timeBlocks)
                    {
                        var times = timeStr.Split(new char[] { '-' });
                        if (times != null && times.Length > 1)
                        {
                            int startTime = Int32.Parse(times[0]);
                            int endTime = Int32.Parse(times[1]);
                            var query = analogHistoryRepos.GetEnergyBySpecialHours(3, buildingId, startDate, endDate, elecPowers, startTime, endTime);
                            var monthQuery = query.GroupBy(x => x.Month, x => x.StatisVal).Select(g => new { Month = g.Key, Val = g.Sum() });
                            var monthData = monthQuery.ToDictionary(x => x.Month, x => x.Val);
                            foreach (var item in monthData)
                            {
                                highVals[item.Key] = item.Value;
                            }
                        }
                    }
                    timeBlocks = timeNormal.Split(new char[] { ',' });
                    double[] normalVals = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    foreach (var timeStr in timeBlocks)
                    {
                        var times = timeStr.Split(new char[] { '-' });
                        if (times != null && times.Length > 1)
                        {
                            int startTime = Int32.Parse(times[0]);
                            int endTime = Int32.Parse(times[1]);
                            var query = analogHistoryRepos.GetEnergyBySpecialHours(3, buildingId, startDate, endDate, elecPowers, startTime, endTime);
                            var monthQuery = query.GroupBy(x => x.Month, x => x.StatisVal).Select(g => new { Month = g.Key, Val = g.Sum() });
                            var monthData = monthQuery.ToDictionary(x => x.Month, x => x.Val);
                            foreach (var item in monthData)
                            {
                                normalVals[item.Key] = item.Value;
                            }
                        }
                    }
                    timeBlocks = timeLow.Split(new char[] { ',' });
                    double[] lowVals = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    foreach (var timeStr in timeBlocks)
                    {
                        var times = timeStr.Split(new char[] { '-' });
                        if (times != null && times.Length > 1)
                        {
                            int startTime = Int32.Parse(times[0]);
                            int endTime = Int32.Parse(times[1]);
                            var query = analogHistoryRepos.GetEnergyBySpecialHours(3, buildingId, startDate, endDate, elecPowers, startTime, endTime);
                            var monthQuery = query.GroupBy(x => x.Month, x => x.StatisVal).Select(g => new { Month = g.Key, Val = g.Sum() });
                            var monthData = monthQuery.ToDictionary(x => x.Month, x => x.Val);
                            foreach (var item in monthData)
                            {
                                lowVals[item.Key] = item.Value;
                            }
                        }
                    }
                    // 使用endDate2主要是因为GetEnergyStatisMonth函数中使用endTime = endTime.AddDays(1)，所以要减一天，另外使用ahm.AHM_MTime <= endTime，可以考虑多减1秒
                    DateTime endDate2 = endDate.AddDays(-1).AddSeconds(-1);
                    var monthDataDic = analogHistoryRepos.GetEnergyStatisMonth(3, buildingId, startDate, endDate2, elecPowers, 1).ToDictionary(x => x.Time.Month, x => x.StatisVal);


                    foreach (var item in monthDataDic)
                    {
                        double hnlsum = highVals[item.Key] + normalVals[item.Key] + lowVals[item.Key];
                        highVals[item.Key] = highVals[item.Key] * monthDataDic[item.Key] / hnlsum;
                        normalVals[item.Key] = normalVals[item.Key] * monthDataDic[item.Key] / hnlsum;
                        lowVals[item.Key] = lowVals[item.Key] * monthDataDic[item.Key] / hnlsum;
                    }

                    IDictionary highMonthValDic = new Dictionary<int, string>();
                    IDictionary normalMonthValDic = new Dictionary<int, string>();
                    IDictionary lowMonthValDic = new Dictionary<int, string>();
                    IDictionary monthValDic = new Dictionary<int, string>();
                    // 合计峰时值
                    double totalHighVal = 0;
                    // 峰时值个数
                    int totalHighValCount = 0;
                    // 合计平时值
                    double totalNormalVal = 0;
                    // 平时值个数
                    int totalNormalValCount = 0;
                    // 合计谷时值
                    double totalLowVal = 0;
                    // 谷时值个数
                    int totalLowValCount = 0;
                    // 合计月值
                    double totalMonthVal = 0;
                    // 合计月值个数
                    int totalValCount = 0;
                    for (int i = 1; i <= 12; i++)
                    {
                        if (highVals[i] > 0)
                        {
                            highMonthValDic.Add(i, highVals[i].ToString("f1"));
                            totalHighVal += highVals[i];
                            totalHighValCount++;
                        }
                        else
                        {
                            highMonthValDic.Add(i, "/");
                        }
                        if (normalVals[i] > 0)
                        {
                            normalMonthValDic.Add(i, normalVals[i].ToString("f1"));
                            totalNormalVal += normalVals[i];
                            totalNormalValCount++;
                        }
                        else
                        {
                            normalMonthValDic.Add(i, "/");
                        }
                        if (lowVals[i] > 0)
                        {
                            lowMonthValDic.Add(i, lowVals[i].ToString("f1"));
                            totalLowVal += lowVals[i];
                            totalLowValCount++;
                        }
                        else
                        {
                            lowMonthValDic.Add(i, "/");
                        }
                        if (monthDataDic.ContainsKey(i) && monthDataDic[i] > 0)
                        {
                            monthValDic.Add(i, monthDataDic[i].ToString("f1"));
                            totalMonthVal += monthDataDic[i];
                            totalValCount++;
                        }
                        else
                        {
                            monthValDic.Add(i, "/");
                        }
                    }
                    // 1月
                    textBox5.Value = monthValDic[1].ToString();
                    textBox8.Value = highMonthValDic[1].ToString();
                    textBox10.Value = normalMonthValDic[1].ToString();
                    textBox12.Value = lowMonthValDic[1].ToString();
                    // 2月
                    textBox18.Value = monthValDic[2].ToString();
                    textBox20.Value = highMonthValDic[2].ToString();
                    textBox21.Value = normalMonthValDic[2].ToString();
                    textBox22.Value = lowMonthValDic[2].ToString();
                    // 3月
                    textBox26.Value = monthValDic[3].ToString();
                    textBox28.Value = highMonthValDic[3].ToString();
                    textBox29.Value = normalMonthValDic[3].ToString();
                    textBox30.Value = lowMonthValDic[3].ToString();
                    // 4月
                    textBox34.Value = monthValDic[4].ToString();
                    textBox36.Value = highMonthValDic[4].ToString();
                    textBox37.Value = normalMonthValDic[4].ToString();
                    textBox38.Value = lowMonthValDic[4].ToString();
                    // 5月
                    textBox42.Value = monthValDic[5].ToString();
                    textBox44.Value = highMonthValDic[5].ToString();
                    textBox45.Value = normalMonthValDic[5].ToString();
                    textBox46.Value = lowMonthValDic[5].ToString();
                    // 6月
                    textBox50.Value = monthValDic[6].ToString();
                    textBox52.Value = highMonthValDic[6].ToString();
                    textBox53.Value = normalMonthValDic[6].ToString();
                    textBox54.Value = lowMonthValDic[6].ToString();
                    // 7月
                    textBox58.Value = monthValDic[7].ToString();
                    textBox60.Value = highMonthValDic[7].ToString();
                    textBox61.Value = normalMonthValDic[7].ToString();
                    textBox62.Value = lowMonthValDic[7].ToString();
                    // 8月
                    textBox66.Value = monthValDic[8].ToString();
                    textBox68.Value = highMonthValDic[8].ToString();
                    textBox69.Value = normalMonthValDic[8].ToString();
                    textBox70.Value = lowMonthValDic[8].ToString();
                    // 9月
                    textBox74.Value = monthValDic[9].ToString();
                    textBox76.Value = highMonthValDic[9].ToString();
                    textBox77.Value = normalMonthValDic[9].ToString();
                    textBox78.Value = lowMonthValDic[9].ToString();
                    // 10月
                    textBox82.Value = monthValDic[10].ToString();
                    textBox84.Value = highMonthValDic[10].ToString();
                    textBox85.Value = normalMonthValDic[10].ToString();
                    textBox86.Value = lowMonthValDic[10].ToString();
                    // 11月
                    textBox90.Value = monthValDic[11].ToString();
                    textBox92.Value = highMonthValDic[11].ToString();
                    textBox93.Value = normalMonthValDic[11].ToString();
                    textBox94.Value = lowMonthValDic[11].ToString();
                    // 12月
                    textBox98.Value = monthValDic[12].ToString();
                    textBox100.Value = highMonthValDic[12].ToString();
                    textBox101.Value = normalMonthValDic[12].ToString();
                    textBox102.Value = lowMonthValDic[12].ToString();
                    // 合计平均月值
                    if (totalMonthVal > 0 && totalValCount > 0)
                    {
                        textBox106.Value = totalMonthVal.ToString("f1");
                        textBox114.Value = (totalMonthVal / totalValCount).ToString("f1");
                    }
                    // 合计平均峰时值
                    if (totalHighVal > 0 && totalHighValCount > 0)
                    {
                        textBox108.Value = totalHighVal.ToString("f1");
                        textBox116.Value = (totalHighVal / totalHighValCount).ToString("f1");
                    }
                    // 合计平均平时值
                    if (totalNormalVal > 0 && totalNormalValCount > 0)
                    {
                        textBox109.Value = totalNormalVal.ToString("f1");
                        textBox117.Value = (totalNormalVal / totalNormalValCount).ToString("f1");
                    }
                    // 合计平均谷时值
                    if (totalLowVal > 0 && totalLowValCount > 0)
                    {
                        textBox110.Value = totalLowVal.ToString("f1");
                        textBox118.Value = (totalLowVal / totalLowValCount).ToString("f1");
                    }
                }
                
            }
            
        }

    }
}