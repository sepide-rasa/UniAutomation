using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using UniAutomation.Areas.faces.Controllers.Acc;
using System.IO;

namespace UniAutomation.Areas.faces.Controllers
{
    public class RptEtebarController : Controller
    {
        //
        // GET: /faces/RptEtebar/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");            
                return PartialView();           

        }

        public ActionResult rptEtebar(int id)
        {
            //Session["CartId"] = id;
            var CartId = id;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            //UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblChargeSelectTableAdapter sp_Sharj = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblChargeSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_EtebarHistoryTableAdapter sp_Sharj = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_EtebarHistoryTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            //sp_Sharj.Fill(dt.sp_tblChargeSelect, "fldFoodCartsID", CartId.ToString(), 0);
            sp_Sharj.Fill(dt.sp_EtebarHistory,CartId.ToString());
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            //Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptSharj.frx");
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptEtebarHistory.frx");
            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            //Report.SetParameterValue("University", "دانشکده فنی دختران شاهرود");
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
