using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace UniAutomation.Areas.faces.Controllers
{
    public class RptBook_TakhirController : Controller
    {
        //
        // GET: /faces/RptBook_Takhir/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            return PartialView();
        }

        public ActionResult Book_TakhirReport(string Tarikh)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            //Session["Tarikh"] = Tarikh;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_L_tblBook_TakhirTableAdapter sp_Book_Takhir = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_L_tblBook_TakhirTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_Book_Takhir.Fill(dt.sp_L_tblBook_Takhir, Tarikh.ToString());
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptBook_Takhir.frx");
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
