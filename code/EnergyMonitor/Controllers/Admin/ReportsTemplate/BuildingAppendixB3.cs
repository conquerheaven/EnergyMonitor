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
    /// Summary description for BuildingAppendixB3.
    /// </summary>
    public partial class BuildingAppendixB3 : Telerik.Reporting.Report
    {
        public BuildingAppendixB3()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        private void BuildingAppendixB3_NeedDataSource(object sender, EventArgs e)
        {
            var buildingIDObj = ReportParameters["buildingID"].Value;
            if (buildingIDObj != null)
            {
                int buildingID = Convert.ToInt32(buildingIDObj);
                if (buildingID > 0)
                {
                    IBuildingRepos buildingRepos = new BuildingRepos();
                    List<BuildingAppendix> buildingAppendixList = buildingRepos.GetBuildingAppendix(buildingID, "B3").ToList();
                    var ba0 = buildingAppendixList.Where(x => x.BA_Name == "吸收式机组").ToList();
                    var ba1 = buildingAppendixList.Where(x => x.BA_Name == "冷却塔").ToList();
                    var ba2 = buildingAppendixList.Where(x => x.BA_Name == "冷温水循环泵").ToList();
                    var ba3 = buildingAppendixList.Where(x => x.BA_Name == "冷却水泵").ToList();
                    table0.DataSource = ba0;
                    table1.DataSource = ba1;
                    table2.DataSource = ba2;
                    table3.DataSource = ba3;
                }
            }
        }
    }
}