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
    /// Summary description for BuildingAppendixB5.
    /// </summary>
    public partial class BuildingAppendixB5 : Telerik.Reporting.Report
    {
        public BuildingAppendixB5()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        private void BuildingAppendixB5_NeedDataSource(object sender, EventArgs e)
        {
            var buildingIDObj = ReportParameters["buildingID"].Value;
            if (buildingIDObj != null)
            {
                int buildingID = Convert.ToInt32(buildingIDObj);
                if (buildingID > 0)
                {
                    IBuildingRepos buildingRepos = new BuildingRepos();
                    List<BuildingAppendix> buildingAppendixList = buildingRepos.GetBuildingAppendix(buildingID, "B5").ToList();
                    var ba0 = buildingAppendixList.Where(x => x.BA_Name == "¹øÂ¯").ToList();
                    var ba1 = buildingAppendixList.Where(x => x.BA_Name == "Ë®±Ã").ToList();
                    table0.DataSource = ba0;
                    table1.DataSource = ba1;
                }
            }
        }
    }
}