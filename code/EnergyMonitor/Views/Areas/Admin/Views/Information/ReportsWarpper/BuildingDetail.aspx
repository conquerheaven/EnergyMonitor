<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Register assembly="Telerik.ReportViewer.WebForms, Version=5.1.11.713, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" namespace="Telerik.ReportViewer.WebForms" tagprefix="telerik" %>

<link href="../../../../../Content/css/report.css" rel="stylesheet" type="text/css" />
<form runat="server">
<telerik:ReportViewer ID="BuildingDetailReportViewer" runat="server" 
    CssClass="report-viewer" Width="100%" BorderColor="#EAEAEA" 
    BorderStyle="Solid" BorderWidth="1px" EnableTheming="False" Height="800px" 
    ProgressText="生成报表..." ShowZoomSelect="True">
    <Resources ExportButtonText="导出" ExportSelectFormatText="选择导出格式" 
        ExportToolTip="导出报表" />
</telerik:ReportViewer>
</form>
<script runat="server">
    //public override void VerifyRenderingInServerForm(Control control)
    //{
    //    base.VerifyRenderingInServerForm(control);
    //}
    
    protected override void OnLoad(EventArgs e)
    {
        if (Model != null)
        {
            EnergyMonitor.Controllers.Admin.ReportsTemplate.BuildingDetailReport buildingReport = new EnergyMonitor.Controllers.Admin.ReportsTemplate.BuildingDetailReport();
            buildingReport.ReportParameters["buildingID"].Value = Model;
            BuildingDetailReportViewer.Report = buildingReport;
        }
        base.OnLoad(e);
    }
</script>