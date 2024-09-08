using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace UniAutomation.Areas.faces.Controllers
{
    public class RptEtebar_DateController : Controller
    {
        //
        // GET: /faces/RptEtebar_Date/

        public ActionResult Index(string State)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            ViewBag.State = State;
            return PartialView();

        }

        public ActionResult rptEtebar_Date(string Tarikh)
        {
            //Session["CartId"] = id;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblChargeSelectTableAdapter sp_Sharj = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblChargeSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_Sharj.Fill(dt.sp_tblChargeSelect, "fldDate", Tarikh, 0);
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptSharj_Date.frx");
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
        public ActionResult RptTrustBook(string Tarikh)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            //Session["TermId"] = TermId;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblL_BookTrustSelectTableAdapter BookTrust = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblL_BookTrustSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            BookTrust.Fill(dt.sp_tblL_BookTrustSelect, "AllTrust", Tarikh, 0);
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();

            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptTrustBook.frx");
            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.Prepare();
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            Report.SetParameterValue("Tarikh", Tarikh);
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }
    }
}
