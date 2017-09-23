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
    /// Summary description for BuildingWaterMonth.
    /// </summary>
    public partial class BuildingWaterMonth : Telerik.Reporting.Report
    {
        public BuildingWaterMonth()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        private void BuildingWaterMonth_NeedDataSource(object sender, EventArgs e)
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
                    textBox134.Value = buildingName;
                    if (buildingArea != null)
                    {
                        textBox135.Value = buildingArea.ToString();
                    }
                    if (buildingUserCount != null)
                    {
                        textBox137.Value = buildingUserCount.ToString();
                    }
                    ISystemProfileRepos systemProfileRepos = new SystemProfileRepos();
                    var priceDic = systemProfileRepos.GetAllPrice().ToDictionary(x => x.SP_ID, x => x.SP_Value);
                    var priceWater = priceDic["price_water"];
                    var pricePollution = priceDic["price_pollution"];
                    textBox11.Value = priceWater;
                    textBox13.Value = pricePollution;

                    IPowerClassRepos powerClassRepos = new PowerClassRepos();
                    var waterPowers = powerClassRepos.GetSubPowers("002").Select(x => x.PC_ID).ToArray();

                    DateTime startDate = DateTime.Parse(year + "-1-1");
                    DateTime endDate = startDate.AddYears(1);
                    IAnalogHistoryRepos analogHistoryRepos = new AnalogHistoryRepos();
                    // 使用endDate2主要是因为GetEnergyStatisMonth函数中使用AnalogHistoryMonths，部分代码ahm.AHM_MTime <= endTime，所以月如果是下年1月就多了一月，重复
                    DateTime endDate2 = endDate.AddMonths(-1);
                    var monthDataDic = analogHistoryRepos.GetEnergyStatisMonth(3, buildingId, startDate, endDate2, waterPowers, 1).ToDictionary(x => x.Time.Month, x => x.StatisVal);
                    IDictionary monthValDic = new Dictionary<int, string>();
                    IDictionary monthPriceDic = new Dictionary<int, string>();
                    double waterPrice = Convert.ToDouble(priceWater);
                    // 合计月值
                    double totalMonthVal = 0;
                    // 每月水费
                    double totalMonthPrice = 0;
                    // 合计月值个数
                    int totalValCount = 0;
                    for (int i = 1; i <= 12; i++)
                    {
                        if (monthDataDic.ContainsKey(i) && monthDataDic[i] > 0)
                        {
                            monthValDic.Add(i, monthDataDic[i].ToString("f1"));
                            double price = monthDataDic[i] * waterPrice;
                            monthPriceDic.Add(i, price.ToString("f1"));
                            totalMonthVal += monthDataDic[i];
                            totalMonthPrice += price;
                            totalValCount++;
                        }
                        else
                        {
                            monthValDic.Add(i, "/");
                            monthPriceDic.Add(i, "/");
                        }
                    }
                    // 1月
                    textBox5.Value = monthValDic[1].ToString();
                    textBox6.Value = monthPriceDic[1].ToString();
                    // 2月
                    textBox18.Value = monthValDic[2].ToString();
                    textBox19.Value = monthPriceDic[2].ToString();
                    // 3月
                    textBox26.Value = monthValDic[3].ToString();
                    textBox27.Value = monthPriceDic[3].ToString();
                    // 4月
                    textBox34.Value = monthValDic[4].ToString();
                    textBox35.Value = monthPriceDic[4].ToString();
                    // 5月
                    textBox42.Value = monthValDic[5].ToString();
                    textBox43.Value = monthPriceDic[5].ToString();
                    // 6月
                    textBox50.Value = monthValDic[6].ToString();
                    textBox51.Value = monthPriceDic[6].ToString();
                    // 7月
                    textBox58.Value = monthValDic[7].ToString();
                    textBox59.Value = monthPriceDic[7].ToString();
                    // 8月
                    textBox66.Value = monthValDic[8].ToString();
                    textBox67.Value = monthPriceDic[8].ToString();
                    // 9月
                    textBox74.Value = monthValDic[9].ToString();
                    textBox75.Value = monthPriceDic[9].ToString();
                    // 10月
                    textBox82.Value = monthValDic[10].ToString();
                    textBox83.Value = monthPriceDic[10].ToString();
                    // 11月
                    textBox90.Value = monthValDic[11].ToString();
                    textBox91.Value = monthPriceDic[11].ToString();
                    // 12月
                    textBox98.Value = monthValDic[12].ToString();
                    textBox99.Value = monthPriceDic[12].ToString();
                    // 合计平均月值
                    if (totalMonthVal > 0 && totalValCount > 0)
                    {
                        textBox106.Value = totalMonthVal.ToString("f1");
                        textBox114.Value = (totalMonthVal / totalValCount).ToString("f1");
                    }
                    // 合计平均水费
                    if (totalMonthPrice > 0 && totalValCount > 0)
                    {
                        textBox107.Value = totalMonthPrice.ToString("f1");
                        textBox115.Value = (totalMonthPrice / totalValCount).ToString("f1");
                    }
                }
            }
        }
    }
}