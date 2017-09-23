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
    /// ����������Ϣ����A1
    /// </summary>
    public partial class BuildingAppendixA1 : Telerik.Reporting.Report
    {
        public BuildingAppendixA1()
        {
            InitializeComponent();
        }

        private void BuildingAppendixA1_NeedDataSource(object sender, EventArgs e)
        {
            var buildingIDObj = ReportParameters["buildingID"].Value;
            if (buildingIDObj != null)
            {
                int buildingID = Convert.ToInt32(buildingIDObj);
                if (buildingID > 0)
                {
                    IBuildingRepos buildingRepos = new BuildingRepos();
                    List<BuildingAppendix> buildingAppendixList = buildingRepos.GetBuildingAppendix(buildingID, "A1").ToList();
                    var ba0 = buildingAppendixList.Where(x => x.BA_Name == "�յ�����").ToList();
                    var ba1 = buildingAppendixList.Where(x => x.BA_Name == "�ͷ��").ToList();
                    var ba2 = buildingAppendixList.Where(x => x.BA_Name == "�·��").ToList();
                    var ba3 = buildingAppendixList.Where(x => x.BA_Name == "�ط��").ToList();
                    var ba4 = buildingAppendixList.Where(x => x.BA_Name == "�ŷ��").ToList();
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