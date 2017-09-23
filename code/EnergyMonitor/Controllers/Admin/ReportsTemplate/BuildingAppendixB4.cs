namespace EnergyMonitor.Controllers.Admin.ReportsTemplate
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using EnergyMonitor.Models.Repository.Implement;
    using EnergyMonitor.Models.Repository.Interface;
    using EnergyMonitor.Models.LinqEntity;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections;

    /// <summary>
    /// Summary description for BuildingAppendixB4.
    /// </summary>
    public partial class BuildingAppendixB4 : Telerik.Reporting.Report
    {
        public BuildingAppendixB4()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        private void BuildingAppendixB4_NeedDataSource(object sender, EventArgs e)
        {
            var buildingIDObj = ReportParameters["buildingID"].Value;
            if (buildingIDObj != null)
            {
                int buildingID = Convert.ToInt32(buildingIDObj);
                if (buildingID > 0)
                {
                    IBuildingRepos buildingRepos = new BuildingRepos();
                    List<BuildingAppendix> buildingAppendixList = buildingRepos.GetBuildingAppendix(buildingID, "B4").ToList();
                    var ba0 = buildingAppendixList.Where(x => x.BA_Name == "分体式机组").ToList();
                    table0.DataSource = ba0;
                }
            }
        }
    }
}