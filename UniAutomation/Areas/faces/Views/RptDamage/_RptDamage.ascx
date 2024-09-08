<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Register Assembly="FastReport.Web" Namespace="FastReport.Web" TagPrefix="cc1" %>

<%{
      UniAutomation.Areas.faces.DataSet.DataSet1 dt2 = new UniAutomation.Areas.faces.DataSet.DataSet1();
      UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_B_tblDamagePersonSelectTableAdapter sp_Damage = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_B_tblDamagePersonSelectTableAdapter();
      UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();
      
      string Type = "fldStudentCodeId";
      if (Convert.ToInt32(Session["Type"]) == 1)
          Type = "fldDamageDate";
      sp_Damage.Fill(dt2.sp_B_tblDamagePersonSelect, Type, Session["Value"].ToString(), 0);
      sp_Setting.Fill(dt2.sp_tblSettingSelect);
      WebReport1.Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptDamage.frx");
      WebReport1.RegisterData(dt2, "uniAutomationDataSet");
      
      WebReport1.Prepare();
      Session.Remove("Value");
      Session.Remove("Type");
      
  } %>
<form id="Form1" runat="server" dir="ltr" >
<cc1:WebReport ID="WebReport1" runat="server" Width="100%" Height="100%" style="direction:ltr;" Font-Names="Tornado Tahoma" ToolbarIconsStyle="Green" ToolbarStyle="Large" PrintInPdf="True" AutoWidth="True" AutoHeight="True" />
</form>


