using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using UniAutomation.Areas.faces.Controllers.Acc;
using System.IO;

namespace UniAutomation.Areas.faces.Controllers
{
    public class PrintKartGoruhiController : Controller
    {
        //
        // GET: /faces/PrintKartGoruhi/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 245))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult GetGroups()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblEducationGroupSelect("", "", 0).ToList().Select(c => new { fldID = c.fldID, fldName = c.fldName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTerms()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_B_tblTermsSelect("", "", 0).ToList().Select(c => new { fldID = c.fldId, fldName = c.fldTermName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reload(int? id, int? Term)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblStudentSelect("fldTermId_EducationGroup",Term.ToString(), id.ToString(),  0).ToList();

            return Json(q, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult PrintKartWin(string id)
        //{
        //    ViewBag.StudentId = id;
        //    return PartialView();
        //}
        //public ActionResult rptKart(string StudentId, string TarikhSodoor, string TarikhEtebar, string Type)
        //{
        //    // Session["date"] = date;
        //    UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
        //    UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_studentDetailSelectTableAdapter sp_Student = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_studentDetailSelectTableAdapter();

        //    sp_Student.Fill(dt.sp_studentDetailSelect, "fldIdIN", StudentId, 0);

        //    FastReport.Report Report = new FastReport.Report();

        //    if (Type == "1")
        //        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Kart.frx");
        //    else
        //        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\KartAlmosana.frx");

        //    Report.RegisterData(dt, "uniAutomationDataSet");
        //    Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
        //    var time = DateTime.Now;
        //    //Report.SetParameterValue("University", "دانشکده فنی دختران شاهرود");
        //    Report.SetParameterValue("TarikhSodoor", TarikhSodoor);
        //    Report.SetParameterValue("TarikhEtebar", TarikhEtebar);
        //    FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
        //    MemoryStream stream = new MemoryStream();
        //    Report.Prepare();
        //    Report.Export(pdf, stream);


        //    return File(stream.ToArray(), "application/pdf");
        //}
    }
}
