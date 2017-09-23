namespace EnergyMonitor.Controllers.Admin.ReportsTemplate
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using EnergyMonitor.Models.Repository.Interface;
    using EnergyMonitor.Models.Repository.Implement;

    /// <summary>
    /// Summary description for ThirdPointMonth.
    /// </summary>
    public partial class ThirdPointMonth : Telerik.Reporting.Report
    {
        public ThirdPointMonth()
        {
            //
            // Required for telerik Reporting designer supportT
            //
            InitializeComponent();           
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
        public void ThirdPointMonth_NeedDataSource(object sender, EventArgs e)
        {
            var yearObj = ReportParameters["year"].Value;
            int year = 0;
            if (yearObj != null)
            {
                year = Convert.ToInt32(yearObj);
                if (year <= 0)
                {
                    year = DateTime.Now.Year;
                }
            }
            else
            {
                year = DateTime.Now.Year;
            }
            String sYear = year.ToString() + "Äê";
            textBox2.Value = sYear;
            IPowerClassRepos powerClassRepos = new PowerClassRepos();
            var elecPowers = (powerClassRepos.GetSubPowers("001").Select(x => x.PC_ID).ToArray().Union(new string[] { "001" })).ToArray();

            IAnalogHistoryRepos analogHistoryRepos = new AnalogHistoryRepos();
            DateTime startDate = DateTime.Parse(year + "-1-1");
            DateTime endDate = startDate.AddYears(1);
                      
            var query = analogHistoryRepos.GetMonthEnergyByAnalogNosGranularity(startDate, endDate, elecPowers);
            table1.DataSource = query.OrderBy(x => x.Name).ToList();
        }
    }
}
