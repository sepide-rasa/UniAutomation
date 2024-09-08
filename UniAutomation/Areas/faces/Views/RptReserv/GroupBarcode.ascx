<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%@ Register Assembly="FastReport.Web" Namespace="FastReport.Web" TagPrefix="cc1" %>
<%{
      UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
      UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_rptGroupBarCodeTableAdapter sp_rptBarcode = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_rptGroupBarCodeTableAdapter();
      UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();
      
      sp_rptBarcode.Fill(dt.sp_rptGroupBarCode,Convert.ToInt32(Session["PersonType"]));
      sp_Setting.Fill(dt.sp_tblSettingSelect);
      
      WebReport1.Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptGroupBarCode.frx");
      WebReport1.RegisterData(dt, "dataSet1");
      WebReport1.Prepare();
      Session.Remove("PersonType");
  } %>
<form id="Form1" runat="server" dir="ltr" >
<cc1:WebReport ID="WebReport1" runat="server" Width="100%" Height="100%" style="direction:ltr;" Font-Names="Tornado Tahoma" ToolbarIconsStyle="Green" ToolbarStyle="Large" PrintInPdf="True" AutoWidth="True" AutoHeight="True" />
</form>