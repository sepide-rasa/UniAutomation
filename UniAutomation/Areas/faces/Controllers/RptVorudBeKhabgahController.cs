using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace UniAutomation.Areas.faces.Controllers
{
    public class RptVorudBeKhabgahController : Controller
    {
        //
        // GET: /faces/RptVorudBeKhabgah/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            return PartialView();
        }
        public ActionResult GetSemester()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_B_tblTermsSelect("", "", 0).ToList().Select(c => new { fldID = c.fldId, fldName = c.fldTermName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult VorudBeKhabgahReport(string Tarikh, int foodcart)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            //Session["Tarikh"] = Tarikh;
            //Session["Semester"] = Semester;
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();            var date = m.sp_GetDate().FirstOrDefault();
            var TermId = m.sp_B_tblTermsSelect("fldTarikh", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), 0).FirstOrDefault().fldId;

            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_B_RptVorudBeKhabgah_FoodCartTableAdapter sp_VorudBeKhabgah = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_B_RptVorudBeKhabgah_FoodCartTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_VorudBeKhabgah.Fill(dt.sp_B_RptVorudBeKhabgah_FoodCart, foodcart, Tarikh);
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptVorudBeKhabgah_FoodCart.frx");
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
