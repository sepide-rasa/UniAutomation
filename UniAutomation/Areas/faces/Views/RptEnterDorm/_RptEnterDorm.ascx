<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Register Assembly="FastReport.Web" Namespace="FastReport.Web" TagPrefix="cc1" %>

<%{
      UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
      UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_B_tblEnterDorm_SarbargSelectTableAdapter sp_Sarbarg = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_B_tblEnterDorm_SarbargSelectTableAdapter();
      UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_B_tblEnterDormSelectTableAdapter sp_detail = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_B_tblEnterDormSelectTableAdapter();
      UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();
      
      sp_Sarbarg.Fill(dt.sp_B_tblEnterDorm_SarbargSelect, "fldId", Session["SarbargId"].ToString(), 0);
      sp_detail.Fill(dt.sp_B_tblEnterDormSelect, "fldSarbargId", Session["SarbargId"].ToString(),"", 0);
      sp_Setting.Fill(dt.sp_tblSettingSelect);
      
      WebReport1.Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptEnterDorm.frx");
      WebReport1.RegisterData(dt, "uniAutomationDataSet");
      
      WebReport1.Prepare();
      Session.Remove("SarbargId");
  } %>
<form id="Form1" runat="server" dir="ltr" >
<cc1:WebReport ID="WebReport1" runat="server" Width="100%" Height="100%" style="direction:ltr;" Font-Names="Tornado Tahoma" ToolbarIconsStyle="Green" ToolbarStyle="Large" PrintInPdf="True" AutoWidth="True" AutoHeight="True" />
</form>


