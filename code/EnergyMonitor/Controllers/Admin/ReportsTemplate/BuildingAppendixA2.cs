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
    using EnergyMonitor.Models.LinqEntity;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// Summary description for BuildingAppendixA2.
    /// </summary>
    public partial class BuildingAppendixA2 : Telerik.Reporting.Report
    {
        public BuildingAppendixA2()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        private void BuildingAppendixA2_NeedDataSource(object sender, EventArgs e)
        {
            var buildingIDObj = ReportParameters["buildingID"].Value;
            if (buildingIDObj != null)
            {
                int buildingID = Convert.ToInt32(buildingIDObj);
                if (buildingID > 0)
                {
                    IBuildingRepos buildingRepos = new BuildingRepos();
                    List<BuildingAppendix> buildingAppendixList = buildingRepos.GetBuildingAppendix(buildingID, "A2").ToList();
                    var ba0 = buildingAppendixList.Where(x => x.BA_Name == "新风机组").ToList();
                    var ba1 = buildingAppendixList.Where(x => x.BA_Name == "风机盘管").ToList();
                    var ba2 = buildingAppendixList.Where(x => x.BA_Name == "新风机").ToList();
                    var ba3 = buildingAppendixList.Where(x => x.BA_Name == "回风机").ToList();
                    var ba4 = buildingAppendixList.Where(x => x.BA_Name == "排风机").ToList();
                    table0.DataSource = ba0;
                    table1.DataSource = ba1;
                    table2.DataSource = ba2;
                    table3.DataSource = ba3;
                    table4.DataSource = ba4;
                }
            }
        }
    }
}