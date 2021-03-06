﻿<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Register assembly="Telerik.ReportViewer.WebForms, Version=5.1.11.713, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" namespace="Telerik.ReportViewer.WebForms" tagprefix="telerik" %>

<link href="../../../../../Content/css/report.css" rel="stylesheet" type="text/css" />
<form runat="server">
<telerik:ReportViewer ID="BuildingEnergyMonthReportViewer" runat="server" 
    CssClass="report-viewer" Width="100%" BorderColor="#EAEAEA" 
    BorderStyle="Solid" BorderWidth="1px" EnableTheming="False" Height="650px" 
    ProgressText="生成报表..." ShowZoomSelect="True">
    <Resources ExportButtonText="导出" ExportSelectFormatText="选择导出格式" 
        ExportToolTip="导出报表" />
</telerik:ReportViewer>
</form>
<script runat="server">
    protected override void OnLoad(EventArgs e)
    {
        if (Model != null)
        {
            EnergyMonitor.Controllers.Admin.ReportsTemplate.BuildingEnergyMonth buildingReport = new EnergyMonitor.Controllers.Admin.ReportsTemplate.BuildingEnergyMonth();
            buildingReport.ReportParameters["buildingId"].Value = Model;
            if (ViewBag.year != null)
            {
                buildingReport.ReportParameters["year"].Value = ViewBag.year;
            }
            BuildingEnergyMonthReportViewer.Report = buildingReport;
        }
        base.OnLoad(e);
    }
</script>