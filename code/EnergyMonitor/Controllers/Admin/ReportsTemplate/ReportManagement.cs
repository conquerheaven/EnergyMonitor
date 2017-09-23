namespace EnergyMonitor.Controllers.Admin.ReportsTemplate
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using EnergyMonitor.Models.Repository.Interface;
    using EnergyMonitor.Models.Repository.Implement;

    /// <summary>
    /// Summary description for BuildingGuanghua.
    /// </summary>
    public partial class ReportManagement : Telerik.Reporting.Report
    {
        public ReportManagement()
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
            int buildingIDObj = Convert.ToInt32(ReportParameters["buildingId"].Value);
            DateTime startTime = Convert.ToDateTime(ReportParameters["startTime"].Value);
            DateTime endTime = Convert.ToDateTime(ReportParameters["endTime"].Value);
            int queryObjType = Convert.ToInt32(ReportParameters["queryObjType"].Value);

            IBuildingGuanghuaRepos buildingGuanghuaRepos = new BuildingGuanghuaRepos();
            var query = buildingGuanghuaRepos.GetBuildingGuanghuaEnergy(queryObjType, buildingIDObj, startTime, endTime);
            table1.DataSource = query;
        }
    }
}