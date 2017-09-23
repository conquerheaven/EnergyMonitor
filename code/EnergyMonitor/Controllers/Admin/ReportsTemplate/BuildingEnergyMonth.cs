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
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Summary description for BuildingEnergyMonth.
    /// </summary>
    public partial class BuildingEnergyMonth : Telerik.Reporting.Report
    {
        public BuildingEnergyMonth()
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
                    double? buildingUserCount = building.BDI_Users;
                    textBox277.Value = buildingName;
                    if (buildingArea != null)
                    {
                        textBox279.Value = buildingArea.ToString();
                    }
                    if (buildingUserCount != null)
                    {
                        textBox281.Value = buildingUserCount.ToString();
                    }
                    ISystemProfileRepos systemProfileRepos = new SystemProfileRepos();
                    var priceDic = systemProfileRepos.GetAllPrice().ToDictionary(x => x.SP_ID, x => x.SP_Value);
                    var priceHighTime = Convert.ToDouble(priceDic["price_highTime"]);
                    var priceNormalTime = Convert.ToDouble(priceDic["price_normalTime"]);
                    var priceLowTime = Convert.ToDouble(priceDic["price_lowTime"]);

                    var timeDic = systemProfileRepos.GetByStartStr("time").ToDictionary(x => x.SP_ID, x => x.SP_Value);
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
                    // 使用endDate2主要是因为GetEnergyStatisMonth函数中使用AnalogHistoryMonths，部分代码ahm.AHM_MTime <= endTime，所以月如果是下年1月就多了一月，重复
                    DateTime endDate2 = endDate.AddMonths(-1);
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
                    // 月电费
                    IDictionary elecChargeDic = new Dictionary<int, string>();
                    // 单位面积电耗
                    IDictionary monthValPerUnitDic = new Dictionary<int, string>();
                    // 单位面积电费
                    IDictionary elecChargeDicPerUnitDic = new Dictionary<int, string>();
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
                    // 合计电费
                    double totalElecCharge = 0;
                    // 合计电费个数
                    int totalElecChargeCount = 0;
                    // 合计单位面积电耗
                    double totalMonthValPerUnit = 0;
                    // 合计单位面积电耗个数
                    int totalMonthValPerUnitCount = 0;
                    // 合计单位面积电费
                    double totalElecChargeDicPerUnit = 0;
                    // 合计单位面积电费个数
                    int totalElecChargeDicPerUnitCount = 0;
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
                            if (buildingArea.HasValue)
                            {
                                var val = monthDataDic[i] / buildingArea.Value;
                                monthValPerUnitDic.Add(i, val.ToString("f1"));
                                totalMonthValPerUnit += val;
                                totalMonthValPerUnitCount++;
                            }
                            else
                            {
                                monthValPerUnitDic.Add(i, "/");
                            }
                            totalMonthVal += monthDataDic[i];
                            totalValCount++;
                        }
                        else
                        {
                            monthValDic.Add(i, "/");
                            monthValPerUnitDic.Add(i, "/");
                        }
                        if (highVals[i] > 0 && normalVals[i] > 0 && lowVals[i] > 0)
                        {
                            double price = highVals[i] * priceHighTime + normalVals[i] * priceNormalTime + lowVals[i] * priceLowTime;
                            elecChargeDic.Add(i, price.ToString("f1"));
                            totalElecCharge += price;
                            totalElecChargeCount++;
                            if (buildingArea.HasValue)
                            {
                                var val = price / buildingArea.Value;
                                elecChargeDicPerUnitDic.Add(i, val.ToString("f1"));
                                totalElecChargeDicPerUnit += val;
                                totalElecChargeDicPerUnitCount++;
                            }
                            else
                            {
                                elecChargeDicPerUnitDic.Add(i, "/");
                            }
                        }
                        else
                        {
                            elecChargeDic.Add(i, "/");
                            elecChargeDicPerUnitDic.Add(i, "/");
                        }
                    }
                    // 1月
                    textBox24.Value = monthValDic[1].ToString();
                    textBox25.Value = highMonthValDic[1].ToString();
                    textBox29.Value = normalMonthValDic[1].ToString();
                    textBox31.Value = lowMonthValDic[1].ToString();
                    textBox33.Value = elecChargeDic[1].ToString();
                    textBox35.Value = monthValPerUnitDic[1].ToString();
                    textBox37.Value = elecChargeDicPerUnitDic[1].ToString();
                    // 2月
                    textBox27.Value = monthValDic[2].ToString();
                    textBox28.Value = highMonthValDic[2].ToString();
                    textBox30.Value = normalMonthValDic[2].ToString();
                    textBox32.Value = lowMonthValDic[2].ToString();
                    textBox34.Value = elecChargeDic[2].ToString();
                    textBox36.Value = monthValPerUnitDic[2].ToString();
                    textBox38.Value = elecChargeDicPerUnitDic[2].ToString();
                    // 3月
                    textBox60.Value = monthValDic[3].ToString();
                    textBox61.Value = highMonthValDic[3].ToString();
                    textBox62.Value = normalMonthValDic[3].ToString();
                    textBox63.Value = lowMonthValDic[3].ToString();
                    textBox64.Value = elecChargeDic[3].ToString();
                    textBox65.Value = monthValPerUnitDic[3].ToString();
                    textBox66.Value = elecChargeDicPerUnitDic[3].ToString();
                    // 4月
                    textBox78.Value = monthValDic[4].ToString();
                    textBox79.Value = highMonthValDic[4].ToString();
                    textBox80.Value = normalMonthValDic[4].ToString();
                    textBox81.Value = lowMonthValDic[4].ToString();
                    textBox82.Value = elecChargeDic[4].ToString();
                    textBox83.Value = monthValPerUnitDic[4].ToString();
                    textBox84.Value = elecChargeDicPerUnitDic[4].ToString();

                    // 5月
                    textBox96.Value = monthValDic[5].ToString();
                    textBox97.Value = highMonthValDic[5].ToString();
                    textBox98.Value = normalMonthValDic[5].ToString();
                    textBox99.Value = lowMonthValDic[5].ToString();
                    textBox100.Value = elecChargeDic[5].ToString();
                    textBox101.Value = monthValPerUnitDic[5].ToString();
                    textBox102.Value = elecChargeDicPerUnitDic[5].ToString();
                    // 6月
                    textBox114.Value = monthValDic[6].ToString();
                    textBox115.Value = highMonthValDic[6].ToString();
                    textBox116.Value = normalMonthValDic[6].ToString();
                    textBox117.Value = lowMonthValDic[6].ToString();
                    textBox118.Value = elecChargeDic[6].ToString();
                    textBox119.Value = monthValPerUnitDic[6].ToString();
                    textBox120.Value = elecChargeDicPerUnitDic[6].ToString();
                    // 7月
                    textBox132.Value = monthValDic[7].ToString();
                    textBox133.Value = highMonthValDic[7].ToString();
                    textBox134.Value = normalMonthValDic[7].ToString();
                    textBox135.Value = lowMonthValDic[7].ToString();
                    textBox136.Value = elecChargeDic[7].ToString();
                    textBox137.Value = monthValPerUnitDic[7].ToString();
                    textBox138.Value = elecChargeDicPerUnitDic[7].ToString();
                    // 8月
                    textBox150.Value = monthValDic[8].ToString();
                    textBox151.Value = highMonthValDic[8].ToString();
                    textBox152.Value = normalMonthValDic[8].ToString();
                    textBox153.Value = lowMonthValDic[8].ToString();
                    textBox154.Value = elecChargeDic[8].ToString();
                    textBox155.Value = monthValPerUnitDic[8].ToString();
                    textBox156.Value = elecChargeDicPerUnitDic[8].ToString();
                    // 9月
                    textBox168.Value = monthValDic[9].ToString();
                    textBox169.Value = highMonthValDic[9].ToString();
                    textBox170.Value = normalMonthValDic[9].ToString();
                    textBox171.Value = lowMonthValDic[9].ToString();
                    textBox172.Value = elecChargeDic[9].ToString();
                    textBox173.Value = monthValPerUnitDic[9].ToString();
                    textBox174.Value = elecChargeDicPerUnitDic[9].ToString();
                    // 10月
                    textBox186.Value = monthValDic[10].ToString();
                    textBox187.Value = highMonthValDic[10].ToString();
                    textBox188.Value = normalMonthValDic[10].ToString();
                    textBox189.Value = lowMonthValDic[10].ToString();
                    textBox190.Value = elecChargeDic[10].ToString();
                    textBox191.Value = monthValPerUnitDic[10].ToString();
                    textBox192.Value = elecChargeDicPerUnitDic[10].ToString();
                    // 11月
                    textBox204.Value = monthValDic[11].ToString();
                    textBox205.Value = highMonthValDic[11].ToString();
                    textBox206.Value = normalMonthValDic[11].ToString();
                    textBox207.Value = lowMonthValDic[11].ToString();
                    textBox208.Value = elecChargeDic[11].ToString();
                    textBox209.Value = monthValPerUnitDic[11].ToString();
                    textBox210.Value = elecChargeDicPerUnitDic[11].ToString();
                    // 12月
                    textBox222.Value = monthValDic[12].ToString();
                    textBox223.Value = highMonthValDic[12].ToString();
                    textBox224.Value = normalMonthValDic[12].ToString();
                    textBox225.Value = lowMonthValDic[12].ToString();
                    textBox226.Value = elecChargeDic[12].ToString();
                    textBox227.Value = monthValPerUnitDic[12].ToString();
                    textBox228.Value = elecChargeDicPerUnitDic[12].ToString();
                    // 合计平均月值
                    if (totalMonthVal > 0 && totalValCount > 0)
                    {
                        textBox240.Value = totalMonthVal.ToString("f1");
                        textBox258.Value = (totalMonthVal / totalValCount).ToString("f1");
                    }
                    // 合计平均峰时值
                    if (totalHighVal > 0 && totalHighValCount > 0)
                    {
                        textBox241.Value = totalHighVal.ToString("f1");
                        textBox259.Value = (totalHighVal / totalHighValCount).ToString("f1");
                    }
                    // 合计平均平时值
                    if (totalNormalVal > 0 && totalNormalValCount > 0)
                    {
                        textBox242.Value = totalNormalVal.ToString("f1");
                        textBox260.Value = (totalNormalVal / totalNormalValCount).ToString("f1");
                    }
                    // 合计平均谷时值
                    if (totalLowVal > 0 && totalLowValCount > 0)
                    {
                        textBox243.Value = totalLowVal.ToString("f1");
                        textBox261.Value = (totalLowVal / totalLowValCount).ToString("f1");
                    }
                    // 合计平均月电费
                    if (totalElecCharge > 0 && totalElecChargeCount > 0)
                    {
                        textBox244.Value = totalElecCharge.ToString("f1");
                        textBox262.Value = (totalElecCharge / totalElecChargeCount).ToString("f1");
                    }
                    // 合计平均单位面积电耗
                    if (totalMonthValPerUnit > 0 && totalMonthValPerUnitCount > 0)
                    {
                        textBox245.Value = totalMonthValPerUnit.ToString("f1");
                        textBox263.Value = (totalMonthValPerUnit / totalMonthValPerUnitCount).ToString("f1");
                    }
                    // 合计平均单位面积电费
                    if (totalElecChargeDicPerUnit > 0 && totalElecChargeDicPerUnitCount > 0)
                    {
                        textBox246.Value = totalElecChargeDicPerUnit.ToString("f1");
                        textBox264.Value = (totalElecChargeDicPerUnit / totalElecChargeDicPerUnitCount).ToString("f1");
                    }
                }

            }
        }
    }
}