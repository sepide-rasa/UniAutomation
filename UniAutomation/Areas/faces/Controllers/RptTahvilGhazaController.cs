using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace UniAutomation.Areas.faces.Controllers
{
    public class RptTahvilGhazaController : Controller
    {
        //
        // GET: /faces/RptTahvilGhaza/

        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult rptTahvilGhaza(string Sdate, string Edate, string Vade, string Type, string FielName, int FoodCartId, int TypeReport)
        {
            // Session["date"] = date;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_rpt_tblDeliveryFoodSelectTableAdapter sp_DeliveryFood = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_rpt_tblDeliveryFoodSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_DeliveryFood.Fill(dt.sp_rpt_tblDeliveryFoodSelect, FielName, Sdate, Edate, Vade, Type, FoodCartId, 0);
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();

            if (FoodCartId == 0)
            {
                if (TypeReport == 1)
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptGozareshTahvilGhaza.frx");
                else if (TypeReport == 2)
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptGozareshTahvilGhazaKoli.frx");
            }
            else
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptGozareshTahvilGhazaEnferadi.frx");
            
            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            //Report.SetParameterValue("University", "دانشکده فنی دختران شاهرود");
            Report.SetParameterValue("date", "");
            Report.SetParameterValue("time", "");
            var tt = "";
            if (FielName == "Reserv")
                tt = "رزرو";
            else if (FielName == "Azad")
                tt = "آزاد";
            Report.SetParameterValue("Title", tt);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }

    }
}
