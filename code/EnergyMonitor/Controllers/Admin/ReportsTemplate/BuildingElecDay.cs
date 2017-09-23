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
    public partial class BuildingElecDay : Telerik.Reporting.Report
    {
        public BuildingElecDay()
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
            var monthObj = ReportParameters["month"].Value;
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
                int month = 0;
                if (monthObj != null)
                {
                    month = Convert.ToInt32(monthObj);
                    if (month <= 0)
                    {
                        month = DateTime.Now.Month;
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
                    textBox300.Value = month.ToString();
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
                    DateTime startDate = DateTime.Parse(year + "-" + month + "-1");
                    DateTime endDate = startDate.AddMonths(1);

                    DateTime startDate2 = startDate.AddDays(1);
                    DateTime endDate2 = endDate.AddSeconds(-1);
                    var dayDataDic = analogHistoryRepos.GetEnergyStatisDay(3, buildingId, startDate, endDate2, elecPowers, 1).ToDictionary(x => x.Time.Day, x => x.StatisVal);

                    var timeBlocks = timeHigh.Split(new char[] { ',' });
                    double[] highVals = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    foreach (var timeStr in timeBlocks)
                    {
                        var times = timeStr.Split(new char[] { '-' });
                        if (times != null && times.Length > 1)
                        {
                            int startTime = Int32.Parse(times[0]);
                            int endTime = Int32.Parse(times[1]);
                            var query = analogHistoryRepos.GetEnergyBySpecialHours(3, buildingId, startDate, endDate, elecPowers, startTime, endTime);
                            var dayData = query.ToDictionary(x => x.Time.Day, x => x.StatisVal);
                            foreach (var item in dayData)
                            {
                                highVals[item.Key] = item.Value;
                            }
                        }
                    }
                    timeBlocks = timeNormal.Split(new char[] { ',' });
                    double[] normalVals = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    foreach (var timeStr in timeBlocks)
                    {
                        var times = timeStr.Split(new char[] { '-' });
                        if (times != null && times.Length > 1)
                        {
                            int startTime = Int32.Parse(times[0]);
                            int endTime = Int32.Parse(times[1]);
                            var query = analogHistoryRepos.GetEnergyBySpecialHours(3, buildingId, startDate, endDate, elecPowers, startTime, endTime);
                            var dayData = query.ToDictionary(x => x.Time.Day, x => x.StatisVal);
                            foreach (var item in dayData)
                            {
                                normalVals[item.Key] = item.Value;
                            }
                        }
                    }
                    timeBlocks = timeLow.Split(new char[] { ',' });
                    double[] lowVals = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    foreach (var timeStr in timeBlocks)
                    {
                        var times = timeStr.Split(new char[] { '-' });
                        if (times != null && times.Length > 1)
                        {
                            int startTime = Int32.Parse(times[0]);
                            int endTime = Int32.Parse(times[1]);
                            var query = analogHistoryRepos.GetEnergyBySpecialHours(3, buildingId, startDate, endDate, elecPowers, startTime, endTime);
                            var dayData = query.ToDictionary(x => x.Time.Day, x => x.StatisVal);
                            foreach (var item in dayData)
                            {
                                lowVals[item.Key] = item.Value;
                            }
                        }
                    }

                    foreach (var item in dayDataDic) {
                        double hnlsum = highVals[item.Key] + normalVals[item.Key] + lowVals[item.Key];
                        highVals[item.Key] = highVals[item.Key] * dayDataDic[item.Key] / hnlsum;
                        normalVals[item.Key] = normalVals[item.Key] * dayDataDic[item.Key] / hnlsum;
                        lowVals[item.Key] = lowVals[item.Key] * dayDataDic[item.Key] / hnlsum;
                    }

                    IDictionary highDayValDic = new Dictionary<int, string>();
                    IDictionary normalDayValDic = new Dictionary<int, string>();
                    IDictionary lowDayValDic = new Dictionary<int, string>();
                    IDictionary dayValDic = new Dictionary<int, string>();
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
                    for (int i = 1; i <= 31; i++)
                    {
                        if (highVals[i] > 0)
                        {
                            highDayValDic.Add(i, highVals[i].ToString("f1"));
                            totalHighVal += highVals[i];
                            totalHighValCount++;
                        }
                        else
                        {
                            highDayValDic.Add(i, "/");
                        }
                        if (normalVals[i] > 0)
                        {
                            normalDayValDic.Add(i, normalVals[i].ToString("f1"));
                            totalNormalVal += normalVals[i];
                            totalNormalValCount++;
                        }
                        else
                        {
                            normalDayValDic.Add(i, "/");
                        }
                        if (lowVals[i] > 0)
                        {
                            lowDayValDic.Add(i, lowVals[i].ToString("f1"));
                            totalLowVal += lowVals[i];
                            totalLowValCount++;
                        }
                        else
                        {
                            lowDayValDic.Add(i, "/");
                        }
                        if (dayDataDic.ContainsKey(i) && dayDataDic[i] > 0)
                        {
                            dayValDic.Add(i, dayDataDic[i].ToString("f1"));
                            totalMonthVal += dayDataDic[i];
                            totalValCount++;
                        }
                        else
                        {
                            dayValDic.Add(i, "/");
                        }
                    }
                    int days = (endDate - startDate).Days;
                    // 1日
                    textBox5.Value = dayValDic[1].ToString();
                    textBox8.Value = highDayValDic[1].ToString();
                    textBox10.Value = normalDayValDic[1].ToString();
                    textBox12.Value = lowDayValDic[1].ToString();
                    // 2日
                    textBox18.Value = dayValDic[2].ToString();
                    textBox20.Value = highDayValDic[2].ToString();
                    textBox21.Value = normalDayValDic[2].ToString();
                    textBox22.Value = lowDayValDic[2].ToString();
                    // 3日
                    textBox26.Value = dayValDic[3].ToString();
                    textBox28.Value = highDayValDic[3].ToString();
                    textBox29.Value = normalDayValDic[3].ToString();
                    textBox30.Value = lowDayValDic[3].ToString();
                    // 4日
                    textBox34.Value = dayValDic[4].ToString();
                    textBox36.Value = highDayValDic[4].ToString();
                    textBox37.Value = normalDayValDic[4].ToString();
                    textBox38.Value = lowDayValDic[4].ToString();
                    // 5日
                    textBox42.Value = dayValDic[5].ToString();
                    textBox44.Value = highDayValDic[5].ToString();
                    textBox45.Value = normalDayValDic[5].ToString();
                    textBox46.Value = lowDayValDic[5].ToString();
                    // 6日
                    textBox50.Value = dayValDic[6].ToString();
                    textBox52.Value = highDayValDic[6].ToString();
                    textBox53.Value = normalDayValDic[6].ToString();
                    textBox54.Value = lowDayValDic[6].ToString();
                    // 7日
                    textBox58.Value = dayValDic[7].ToString();
                    textBox60.Value = highDayValDic[7].ToString();
                    textBox61.Value = normalDayValDic[7].ToString();
                    textBox62.Value = lowDayValDic[7].ToString();
                    // 8日
                    textBox66.Value = dayValDic[8].ToString();
                    textBox68.Value = highDayValDic[8].ToString();
                    textBox69.Value = normalDayValDic[8].ToString();
                    textBox70.Value = lowDayValDic[8].ToString();
                    // 9日
                    textBox74.Value = dayValDic[9].ToString();
                    textBox76.Value = highDayValDic[9].ToString();
                    textBox77.Value = normalDayValDic[9].ToString();
                    textBox78.Value = lowDayValDic[9].ToString();
                    // 10日
                    textBox82.Value = dayValDic[10].ToString();
                    textBox84.Value = highDayValDic[10].ToString();
                    textBox85.Value = normalDayValDic[10].ToString();
                    textBox86.Value = lowDayValDic[10].ToString();
                    // 11日
                    textBox90.Value = dayValDic[11].ToString();
                    textBox92.Value = highDayValDic[11].ToString();
                    textBox93.Value = normalDayValDic[11].ToString();
                    textBox94.Value = lowDayValDic[11].ToString();
                    // 12日
                    textBox98.Value = dayValDic[12].ToString();
                    textBox100.Value = highDayValDic[12].ToString();
                    textBox101.Value = normalDayValDic[12].ToString();
                    textBox102.Value = lowDayValDic[12].ToString();
                    // 13日
                    textBox106.Value = dayValDic[13].ToString();
                    textBox108.Value = highDayValDic[13].ToString();
                    textBox109.Value = normalDayValDic[13].ToString();
                    textBox110.Value = lowDayValDic[13].ToString();
                    // 14日
                    textBox114.Value = dayValDic[14].ToString();
                    textBox116.Value = highDayValDic[14].ToString();
                    textBox117.Value = normalDayValDic[14].ToString();
                    textBox118.Value = lowDayValDic[14].ToString();
                    // 15日
                    textBox148.Value = dayValDic[15].ToString();
                    textBox150.Value = highDayValDic[15].ToString();
                    textBox151.Value = normalDayValDic[15].ToString();
                    textBox152.Value = lowDayValDic[15].ToString();
                    // 16日
                    textBox156.Value = dayValDic[16].ToString();
                    textBox158.Value = highDayValDic[16].ToString();
                    textBox159.Value = normalDayValDic[16].ToString();
                    textBox160.Value = lowDayValDic[16].ToString();
                    // 17日
                    textBox164.Value = dayValDic[17].ToString();
                    textBox166.Value = highDayValDic[17].ToString();
                    textBox167.Value = normalDayValDic[17].ToString();
                    textBox168.Value = lowDayValDic[17].ToString();
                    // 18日
                    textBox172.Value = dayValDic[18].ToString();
                    textBox174.Value = highDayValDic[18].ToString();
                    textBox175.Value = normalDayValDic[18].ToString();
                    textBox176.Value = lowDayValDic[18].ToString();
                    // 19日
                    textBox180.Value = dayValDic[19].ToString();
                    textBox182.Value = highDayValDic[19].ToString();
                    textBox183.Value = normalDayValDic[19].ToString();
                    textBox184.Value = lowDayValDic[19].ToString();
                    // 20日
                    textBox188.Value = dayValDic[20].ToString();
                    textBox190.Value = highDayValDic[20].ToString();
                    textBox191.Value = normalDayValDic[20].ToString();
                    textBox192.Value = lowDayValDic[20].ToString();
                    // 21日
                    textBox196.Value = dayValDic[21].ToString();
                    textBox198.Value = highDayValDic[21].ToString();
                    textBox199.Value = normalDayValDic[21].ToString();
                    textBox200.Value = lowDayValDic[21].ToString();
                    // 22日
                    textBox204.Value = dayValDic[22].ToString();
                    textBox206.Value = highDayValDic[22].ToString();
                    textBox207.Value = normalDayValDic[22].ToString();
                    textBox208.Value = lowDayValDic[22].ToString();
                    // 23日
                    textBox212.Value = dayValDic[23].ToString();
                    textBox214.Value = highDayValDic[23].ToString();
                    textBox215.Value = normalDayValDic[23].ToString();
                    textBox216.Value = lowDayValDic[23].ToString();
                    // 24日
                    textBox220.Value = dayValDic[24].ToString();
                    textBox222.Value = highDayValDic[24].ToString();
                    textBox223.Value = normalDayValDic[24].ToString();
                    textBox224.Value = lowDayValDic[24].ToString();
                    // 25日
                    textBox228.Value = dayValDic[25].ToString();
                    textBox230.Value = highDayValDic[25].ToString();
                    textBox231.Value = normalDayValDic[25].ToString();
                    textBox232.Value = lowDayValDic[25].ToString();
                    // 26日
                    textBox236.Value = dayValDic[26].ToString();
                    textBox238.Value = highDayValDic[26].ToString();
                    textBox239.Value = normalDayValDic[26].ToString();
                    textBox240.Value = lowDayValDic[26].ToString();
                    // 27日
                    textBox244.Value = dayValDic[27].ToString();
                    textBox246.Value = highDayValDic[27].ToString();
                    textBox247.Value = normalDayValDic[27].ToString();
                    textBox248.Value = lowDayValDic[27].ToString();
                    // 28日
                    textBox252.Value = dayValDic[28].ToString();
                    textBox254.Value = highDayValDic[28].ToString();
                    textBox255.Value = normalDayValDic[28].ToString();
                    textBox256.Value = lowDayValDic[28].ToString();
                    // 29日
                    if (days >= 29)
                    {
                        textBox260.Value = dayValDic[29].ToString();
                        textBox262.Value = highDayValDic[29].ToString();
                        textBox263.Value = normalDayValDic[29].ToString();
                        textBox264.Value = lowDayValDic[29].ToString();
                    }
                    // 30日
                    if (days >= 30)
                    {
                        textBox268.Value = dayValDic[30].ToString();
                        textBox270.Value = highDayValDic[30].ToString();
                        textBox271.Value = normalDayValDic[30].ToString();
                        textBox272.Value = lowDayValDic[30].ToString();
                    }
                    // 31日
                    if (days >= 31)
                    {
                        textBox276.Value = dayValDic[31].ToString();
                        textBox278.Value = highDayValDic[31].ToString();
                        textBox279.Value = normalDayValDic[31].ToString();
                        textBox290.Value = lowDayValDic[31].ToString();
                    }

                    // 合计平均月值
                    if (totalMonthVal > 0 && totalValCount > 0)
                    {
                        textBox284.Value = totalMonthVal.ToString("f1");
                        textBox292.Value = (totalMonthVal / totalValCount).ToString("f1");
                    }
                    // 合计平均峰时值
                    if (totalHighVal > 0 && totalHighValCount > 0)
                    {
                        textBox286.Value = totalHighVal.ToString("f1");
                        textBox294.Value = (totalHighVal / totalHighValCount).ToString("f1");
                    }
                    // 合计平均平时值
                    if (totalNormalVal > 0 && totalNormalValCount > 0)
                    {
                        textBox287.Value = totalNormalVal.ToString("f1");
                        textBox295.Value = (totalNormalVal / totalNormalValCount).ToString("f1");
                    }
                    // 合计平均谷时值
                    if (totalLowVal > 0 && totalLowValCount > 0)
                    {
                        textBox288.Value = totalLowVal.ToString("f1");
                        textBox296.Value = (totalLowVal / totalLowValCount).ToString("f1");
                    }
                }
                
            }
            
        }

    }
}