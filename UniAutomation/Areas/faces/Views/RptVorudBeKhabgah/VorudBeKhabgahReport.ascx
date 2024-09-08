<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%@ Register Assembly="FastReport.Web" Namespace="FastReport.Web" TagPrefix="cc1" %>
<%{
      UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
      UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_rpt_B_tblVorudBeKhabgahTableAdapter sp_VorudBeKhabgah = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_rpt_B_tblVorudBeKhabgahTableAdapter();


      sp_VorudBeKhabgah.Fill(dt.sp_rpt_B_tblVorudBeKhabgah, Session["Tarikh"].ToString(), Convert.ToInt16(Session["Semester"]));


      WebReport1.Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptVorudBeKhabgah.frx");
      WebReport1.RegisterData(dt, "uniAutomationDataSet");
      WebReport1.Prepare();
      Session.Remove("Tarikh");
      Session.Remove("Semester");

  } %>
<form id="Form1" runat="server" dir="ltr" >
<cc1:WebReport ID="WebReport1" runat="server" Width="100%" Height="100%" style="direction:ltr;" Font-Names="Tornado Tahoma" ToolbarIconsStyle="Green" ToolbarStyle="Large" PrintInPdf="True" AutoWidth="True" AutoHeight="True" />
</form>
