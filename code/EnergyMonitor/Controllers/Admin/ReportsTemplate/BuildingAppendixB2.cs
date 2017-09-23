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
    /// Summary description for BuildingAppendixB2.
    /// </summary>
    public partial class BuildingAppendixB2 : Telerik.Reporting.Report
    {
        public BuildingAppendixB2()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        private void BuildingAppendixB2_NeedDataSource(object sender, EventArgs e)
        {
            var buildingIDObj = ReportParameters["buildingID"].Value;
            if (buildingIDObj != null)
            {
                int buildingID = Convert.ToInt32(buildingIDObj);
                if (buildingID > 0)
                {
                    IBuildingRepos buildingRepos = new BuildingRepos();
                    List<BuildingAppendix> buildingAppendixList = buildingRepos.GetBuildingAppendix(buildingID, "B2").ToList();
                    var ba0 = buildingAppendixList.Where(x => x.BA_Name == "�ȱû���").ToList();
                    var ba1 = buildingAppendixList.Where(x => x.BA_Name == "ˮ��").ToList();
                    table0.DataSource = ba0;
                    table1.DataSource = ba1;
                }
            }
        }
    }
}