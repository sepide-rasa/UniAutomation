﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%@ Register Assembly="FastReport.Web" Namespace="FastReport.Web" TagPrefix="cc1" %>
<%{
      UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
      UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblL_BookStockSelectTableAdapter sp_Barcode = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblL_BookStockSelectTableAdapter();

      sp_Barcode.Fill(dt.sp_tblL_BookStockSelect,"", "",0);

      WebReport1.Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptGroupBarCodBookStock.frx");
      WebReport1.RegisterData(dt, "uniAutomationDataSet");
      WebReport1.Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
      var time=DateTime.Now;
      WebReport1.Report.SetParameterValue("University","دانشکده فنی دختران شاهرود" );
      WebReport1.Report.SetParameterValue("date","");
      WebReport1.Report.SetParameterValue("time","");
      WebReport1.Prepare(); 
         
  } %>
<form id="Form1" runat="server" dir="ltr" >
<cc1:WebReport ID="WebReport1" runat="server" Width="100%" Height="100%" style="direction:ltr;" Font-Names="Tornado Tahoma" ToolbarIconsStyle="Green" ToolbarStyle="Large" PrintInPdf="True" AutoWidth="True" AutoHeight="True" />
</form>