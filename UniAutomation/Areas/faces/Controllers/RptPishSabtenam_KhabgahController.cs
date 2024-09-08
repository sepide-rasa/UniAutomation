using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace UniAutomation.Areas.faces.Controllers
{
    public class RptPishSabtenam_KhabgahController : Controller
    {
        //
        // GET: /faces/RptPishSabtenam_Khabgah/

        public ActionResult Index(string State)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            ViewBag.State = State;
            return PartialView();
        }
        public ActionResult GetSemester()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_B_tblTermsSelect("", "", 0).ToList().Select(c => new { fldID = c.fldId, fldName = c.fldTermName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PishSabtenam_KhabgahReport(int TermId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            //Session["TermId"] = TermId;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblPishSabtenam_KhabgahSelectTableAdapter PishSabtenam_Khabgah = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblPishSabtenam_KhabgahSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            PishSabtenam_Khabgah.Fill(dt.sp_tblPishSabtenam_KhabgahSelect, "fldTermId", TermId.ToString(), 0);
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();

            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptPishSabtanam_Khabgah.frx");
            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.Prepare();
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }
        public ActionResult StudentTermReport(int TermId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            //Session["TermId"] = TermId;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblStudentSelectTableAdapter Student = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblStudentSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            Student.Fill(dt.sp_tblStudentSelect, "fldTermId", TermId.ToString(),"", 0);
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();

            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptStudentTerm.frx");
            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.Prepare();
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }
    }
}
