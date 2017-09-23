namespace EnergyMonitor.Controllers.Admin.ReportsTemplate
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using EnergyMonitor.Models.LinqEntity;
    using EnergyMonitor.Models.Repository.Interface;
    using EnergyMonitor.Models.Repository.Implement;

    /// <summary>
    /// ������ϸ����
    /// </summary>
    public partial class BuildingDetailReport : Telerik.Reporting.Report
    {
        public BuildingDetailReport()
        {
            InitializeComponent();

        }

        private void BuildingDetailReport_ItemDataBinding(object sender, EventArgs e)
        {
            var buildingIDObj = ReportParameters["buildingID"].Value;
            if (buildingIDObj != null)
            {
                int buildingID = Convert.ToInt32(buildingIDObj);
                IBuildingRepos buildingRepos = new BuildingRepos();
                BuildingDetailInfo building = buildingRepos.GetBuilding(buildingID);
                if (building == null)
                {
                    return;
                }
                buildingTable.DataSource = building;

                var lightDataList = buildingRepos.GetLight(buildingID);
                var officeDataList = buildingRepos.GetOfficeEquip(buildingID);
                var elevatorDataList = buildingRepos.GetElevator(buildingID);
                var waterPumpDataList = buildingRepos.GetWaterPump(buildingID);
                var windMachDataList = buildingRepos.GetWindMach(buildingID);
                var kitchenEquipDataList = buildingRepos.GetKitchenEquip(buildingID);

                lightTable.DataSource = lightDataList;
                officeEquipTable.DataSource = officeDataList;
                elevatorTable.DataSource = elevatorDataList;
                waterPumpTable.DataSource = waterPumpDataList;
                windMachTable.DataSource = windMachDataList;
                kitchenEquipTable.DataSource = kitchenEquipDataList;

                Parameter subReportPara = new Parameter("buildingID", buildingID);
                //A1A2A3
                if (!String.IsNullOrWhiteSpace(building.BDI_AirSys))
                {
                    if (building.BDI_AirSys == "����ʽȫ����ϵͳ")
                    {
                        NavigateToReportAction a1ReportAction = new NavigateToReportAction();
                        a1ReportAction.Parameters.Add(subReportPara);
                        a1ReportAction.ReportDocumentType = "EnergyMonitor.Controllers.Admin.ReportsTemplate.BuildingAppendixA1, Controllers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                        textBox26.Action = a1ReportAction;
                        textBox26.Value = "�������A1";
                    }
                    else if (building.BDI_AirSys == "����̹ܣ��·�ϵͳ")
                    {
                        NavigateToReportAction a2ReportAction = new NavigateToReportAction();
                        a2ReportAction.Parameters.Add(subReportPara);
                        a2ReportAction.ReportDocumentType = "EnergyMonitor.Controllers.Admin.ReportsTemplate.BuildingAppendixA2, Controllers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                        textBox25.Action = a2ReportAction;
                        textBox25.Value = "�������A2";
                    }
                    else if (building.BDI_AirSys == "����ʽ�յ���VRV�ľֲ�ʽ����ϵͳ")
                    {
                        NavigateToReportAction a3ReportAction = new NavigateToReportAction();
                        a3ReportAction.Parameters.Add(subReportPara);
                        a3ReportAction.ReportDocumentType = "EnergyMonitor.Controllers.Admin.ReportsTemplate.BuildingAppendixA3, Controllers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                        textBox28.Action = a3ReportAction;
                        textBox28.Value = "�������A3";
                    }
                }
                //B1
                if (!String.IsNullOrWhiteSpace(building.BDI_DevCool))
                {
                    NavigateToReportAction b1ReportAction = new NavigateToReportAction();
                    b1ReportAction.Parameters.Add(subReportPara);
                    b1ReportAction.ReportDocumentType = "EnergyMonitor.Controllers.Admin.ReportsTemplate.BuildingAppendixB1, Controllers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    textBox34.Action = b1ReportAction;
                    textBox34.Value = "�������B1";
                }
                //B2
                if (!String.IsNullOrWhiteSpace(building.BDI_DevHot))
                {
                    NavigateToReportAction b2ReportAction = new NavigateToReportAction();
                    b2ReportAction.Parameters.Add(subReportPara);
                    b2ReportAction.ReportDocumentType = "EnergyMonitor.Controllers.Admin.ReportsTemplate.BuildingAppendixB2, Controllers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    textBox36.Action = b2ReportAction;
                    textBox36.Value = "�������B2";
                }
                //B3
                if (!String.IsNullOrWhiteSpace(building.BDI_LiBr))
                {
                    NavigateToReportAction b3ReportAction = new NavigateToReportAction();
                    b3ReportAction.Parameters.Add(subReportPara);
                    b3ReportAction.ReportDocumentType = "EnergyMonitor.Controllers.Admin.ReportsTemplate.BuildingAppendixB3, Controllers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    textBox60.Action = b3ReportAction;
                    textBox60.Value = "�������B3";
                }
                //B4
                if (!String.IsNullOrWhiteSpace(building.BDI_AirSplit))
                {
                    NavigateToReportAction b4ReportAction = new NavigateToReportAction();
                    b4ReportAction.Parameters.Add(subReportPara);
                    b4ReportAction.ReportDocumentType = "EnergyMonitor.Controllers.Admin.ReportsTemplate.BuildingAppendixB4, Controllers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    textBox37.Action = b4ReportAction;
                    textBox37.Value = "�������B4";
                }
                //B5
                if (!String.IsNullOrWhiteSpace(building.BDI_BoilerPower))
                {
                    NavigateToReportAction b5ReportAction = new NavigateToReportAction();
                    b5ReportAction.Parameters.Add(subReportPara);
                    b5ReportAction.ReportDocumentType = "EnergyMonitor.Controllers.Admin.ReportsTemplate.BuildingAppendixB5, Controllers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    textBox39.Action = b5ReportAction;
                    textBox39.Value = "�������B5";
                }
                //B6
                if (!String.IsNullOrWhiteSpace(building.BDI_Boiler))
                {
                    NavigateToReportAction b6ReportAction = new NavigateToReportAction();
                    b6ReportAction.Parameters.Add(subReportPara);
                    b6ReportAction.ReportDocumentType = "EnergyMonitor.Controllers.Admin.ReportsTemplate.BuildingAppendixB6, Controllers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    textBox41.Action = b6ReportAction;
                    textBox41.Value = "�������B6";
                }
                //B7
                if (!String.IsNullOrWhiteSpace(building.BDI_PowerHot))
                {
                    NavigateToReportAction b7ReportAction = new NavigateToReportAction();
                    b7ReportAction.Parameters.Add(subReportPara);
                    b7ReportAction.ReportDocumentType = "EnergyMonitor.Controllers.Admin.ReportsTemplate.BuildingAppendixB7, Controllers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    textBox45.Action = b7ReportAction;
                    textBox45.Value = "�������B7";
                }
                //B8
                if (!String.IsNullOrWhiteSpace(building.BDI_DevOther))
                {
                    NavigateToReportAction b8ReportAction = new NavigateToReportAction();
                    b8ReportAction.Parameters.Add(subReportPara);
                    b8ReportAction.ReportDocumentType = "EnergyMonitor.Controllers.Admin.ReportsTemplate.BuildingAppendixB8, Controllers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    textBox47.Action = b8ReportAction;
                    textBox47.Value = "�������B8";
                }
            }
        }
    }

}