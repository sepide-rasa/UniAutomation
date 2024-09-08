<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%@ Register Assembly="FastReport.Web" Namespace="FastReport.Web" TagPrefix="cc1" %>
<%{
      UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
      UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblL_BookSelectTableAdapter sp_L_Book = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblL_BookSelectTableAdapter();
      UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblL_BookCategorySelectTableAdapter L_BookCategory = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblL_BookCategorySelectTableAdapter();
      UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();
      
      sp_L_Book.Fill(dt.sp_tblL_BookSelect, "fldCategoryId", Session["CategoryId"].ToString(), 0);
      L_BookCategory.Fill(dt.sp_tblL_BookCategorySelect, "fldId", Session["CategoryId"].ToString(), 0);
      sp_Setting.Fill(dt.sp_tblSettingSelect);

      WebReport1.Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptGozareshBookLibrary.frx");
      WebReport1.RegisterData(dt, "uniAutomationDataSet");
      WebReport1.Prepare();
      Session.Remove("CategoryId");

  } %>
<form id="Form1" runat="server" dir="ltr" >
<cc1:WebReport ID="WebReport1" runat="server" Width="100%" Height="100%" style="direction:ltr;" Font-Names="Tornado Tahoma" ToolbarIconsStyle="Green" ToolbarStyle="Large" PrintInPdf="True" AutoWidth="True" AutoHeight="True" />
</form>
