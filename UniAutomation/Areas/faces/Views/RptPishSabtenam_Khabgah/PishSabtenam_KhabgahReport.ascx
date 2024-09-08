<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%@ Register Assembly="FastReport.Web" Namespace="FastReport.Web" TagPrefix="cc1" %>
<%{
      UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
      UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblPishSabtenam_KhabgahSelectTableAdapter PishSabtenam_Khabgah = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblPishSabtenam_KhabgahSelectTableAdapter();
      UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();
      
      PishSabtenam_Khabgah.Fill(dt.sp_tblPishSabtenam_KhabgahSelect, "fldTermId", Session["TermId"].ToString(),0);
      sp_Setting.Fill(dt.sp_tblSettingSelect);

      WebReport1.Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptPishSabtanam_Khabgah.frx");
      WebReport1.RegisterData(dt, "uniAutomationDataSet");
      WebReport1.Prepare();
      Session.Remove("TermId");

  } %>
<form id="Form1" runat="server" dir="ltr" >
<cc1:WebReport ID="WebReport1" runat="server" Width="100%" Height="100%" style="direction:ltr;" Font-Names="Tornado Tahoma" ToolbarIconsStyle="Green" ToolbarStyle="Large" PrintInPdf="True" AutoWidth="True" AutoHeight="True" />
</form>

