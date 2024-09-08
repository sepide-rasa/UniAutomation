using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace UniAutomation.Areas.faces.Controllers
{
    public class GroupBarCodeBookStockController : Controller
    {
        //
        // GET: /faces/GroupBarCodeBookStock/

        public ActionResult Index()
        {
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblL_BookStockSelectTableAdapter sp_Barcode = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblL_BookStockSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_Barcode.Fill(dt.sp_tblL_BookStockSelect, "", "", 0);
            Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptGroupBarCodBookStock.frx");
            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("University", "دانشکده فنی دختران شاهرود");
            Report.SetParameterValue("date", "");
            Report.SetParameterValue("time", "");
            Report.Prepare();
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }

    }
}
