using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace UniAutomation.Controllers
{
    public class TarikhcheReserveController : Controller
    {
        //
        // GET: /TarikhcheReserve/

        public ActionResult History()
        {
            if (Session["PersonId"] == null)
                return RedirectToAction("logIn", "Accounts");
            return PartialView();
        }
        public ActionResult rptHistoryReserv(string Sdate, string Edate, int nobat)
        {
            if (Session["PersonId"] == null)
                return RedirectToAction("logIn", "Accounts");
            //Session["SrartDate"] = MyLib.Shamsi.Shamsi2miladiString(Sdate);
            //Session["EndDate"] =  MyLib.Shamsi.Shamsi2miladiString(Edate);
            //Session["Nobat"] = nobat;
            //Session["Cid"] = Cid;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblHistoryReserveSelectTableAdapter sp_GroupReserv = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblHistoryReserveSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_GroupReserv.Fill(dt.sp_tblHistoryReserveSelect, "AzTarikh_TaTarikh", Session["PersonId"].ToString(), Sdate, Edate, Convert.ToInt32(nobat), 0);
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptHistoryReserv.frx");
            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            //Report.SetParameterValue("University", "دانشکده فنی دختران شاهرود");
            Report.SetParameterValue("date", "");
            Report.SetParameterValue("time", "");
            Report.SetParameterValue("AzTarikh", Sdate);
            Report.SetParameterValue("TaTarikh", Edate);
            Report.Prepare();
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }
    }
}
