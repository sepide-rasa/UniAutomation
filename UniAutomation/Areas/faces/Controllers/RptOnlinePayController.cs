using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace UniAutomation.Areas.faces.Controllers
{
    public class RptOnlinePayController : Controller
    {
        //
        // GET: /faces/RptOnlinePay/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            return PartialView();
        }

        public ActionResult OnlinePayRepot(string Sdate, string Edate, int Type, string PayType)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            //Session["SrartDate"] = Sdate;
            //Session["EndDate"] = Edate;
            //Session["Type"] = Type;
            bool PType=true;
            string Title = "گزارش پرداخت اینترنتی موفق";
            if (PayType == "0")
            {
                PType = false;
                Title = "گزارش پرداخت اینترنتی ناموفق";
            }
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_OnlinePaymentSelectTableAdapter sp_OnlinePay = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_OnlinePaymentSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_OnlinePay.Fill(dt.sp_OnlinePaymentSelect, Sdate.ToString(), Edate.ToString(), Convert.ToByte(Type), PType);
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptOnlinePay.frx");
            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            //Report.SetParameterValue("University", "دانشکده فنی دختران شاهرود");
            Report.SetParameterValue("date", "");
            Report.SetParameterValue("time", "");
            Report.SetParameterValue("Title", Title);
            Report.Prepare();
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }
    }
}
