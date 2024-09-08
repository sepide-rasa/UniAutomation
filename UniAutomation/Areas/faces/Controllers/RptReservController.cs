using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace UniAutomation.Areas.faces.Controllers
{
    public class RptReservController : Controller
    {
        //
        // GET: /faces/RptReserv/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            return PartialView();
        }
        public ActionResult History()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            return PartialView();
        }

        public ActionResult GetGroups()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblEducationGroupSelect("", "", 0).ToList().Select(c => new { fldID = c.fldID, fldName = c.fldName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult rptReserv(string Sdate, string Edate, int nobat, long Cid)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            //Session["SrartDate"] = MyLib.Shamsi.Shamsi2miladiString(Sdate);
            //Session["EndDate"] =  MyLib.Shamsi.Shamsi2miladiString(Edate);
            //Session["Nobat"] = nobat;
            //Session["Cid"] = Cid;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_RptReservsTableAdapter sp_GroupReserv = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_RptReservsTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_GroupReserv.Fill(dt.sp_RptReservs, Convert.ToDateTime(MyLib.Shamsi.Shamsi2miladiString(Sdate)), Convert.ToDateTime(MyLib.Shamsi.Shamsi2miladiString(Edate)), Convert.ToInt32(nobat), Cid);
           sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptReserv.frx");
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
        public ActionResult rptHistoryReserv(string Sdate, string Edate, int nobat, long Cid)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            //Session["SrartDate"] = MyLib.Shamsi.Shamsi2miladiString(Sdate);
            //Session["EndDate"] =  MyLib.Shamsi.Shamsi2miladiString(Edate);
            //Session["Nobat"] = nobat;
            //Session["Cid"] = Cid;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblHistoryReserveSelectTableAdapter sp_GroupReserv = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblHistoryReserveSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_GroupReserv.Fill(dt.sp_tblHistoryReserveSelect,"AzTarikh_TaTarikh",Cid.ToString(), Sdate, Edate, Convert.ToInt32(nobat),0);
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
        public ActionResult rptReservGroup(string Sdate, string Edate, int nobat)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            //Session["SrartDate"] = MyLib.Shamsi.Shamsi2miladiString(Sdate);
            //Session["EndDate"] = MyLib.Shamsi.Shamsi2miladiString(Sdate); ;
            //Session["Nobat"] = nobat;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_RptReservsGroupsTableAdapter sp_GroupReserv = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_RptReservsGroupsTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_GroupReserv.Fill(dt.sp_RptReservsGroups, Convert.ToDateTime(MyLib.Shamsi.Shamsi2miladiString(Sdate)), Convert.ToDateTime(MyLib.Shamsi.Shamsi2miladiString(Sdate)), Convert.ToInt32(nobat));
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptReservGroup.frx");
            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            //Report.Report.SetParameterValue("University", "دانشکده فنی دختران شاهرود");
            Report.Report.SetParameterValue("date", "");
            Report.Report.SetParameterValue("time", "");
            Report.Prepare();
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }

        public ActionResult rptMande()
        {
            return PartialView();
        }

        public ActionResult rptMandeHesab(int GroupId, int Type)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
           // Session["GroupId"] = id;
            //var GroupId = id;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_GroupChargeSelectTableAdapter sp_GroupSharj = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_GroupChargeSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            var FieldName = "Student";
            if (Type == 2)
                FieldName = "Teacher";
            else if (Type == 3)
                FieldName = "Personel";
            sp_GroupSharj.Fill(dt.sp_GroupChargeSelect, FieldName, Convert.ToInt32(GroupId), null);
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptMandeHesab.frx");
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

        public ActionResult NotEat()
        {
            return PartialView();
        }

        public ActionResult RptNotEat(string Sdate,string Edate)
        {
            //Session["Sdate"] = Sdate;
            //Session["Edate"] = Edate;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_RptNotEatingTableAdapter sp_rptNoEating = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_RptNotEatingTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_rptNoEating.Fill(dt.sp_RptNotEating, MyLib.Shamsi.Shamsi2miladiDateTime(Sdate.ToString()), MyLib.Shamsi.Shamsi2miladiDateTime(Edate.ToString()));
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptNotEating.frx");
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
        public ActionResult AmarKoli()
        {
            return PartialView();
        }

        public ActionResult RptAmarKoli(string Sdate, string Edate)
        {
            //Session["Sdate"] = Sdate;
            //Session["Edate"] = Edate;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_AmarKoliTableAdapter AmarKoli = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_AmarKoliTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            AmarKoli.Fill(dt.sp_AmarKoli, MyLib.Shamsi.Shamsi2miladiDateTime(Sdate.ToString()), MyLib.Shamsi.Shamsi2miladiDateTime(Edate.ToString()));
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptAmarKoli.frx");
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

        public ActionResult Barcode()
        {
            return PartialView();
        }
        public ActionResult GroupBarcode(string id)
        {
            Session["PersonType"] = id;
            return View();
        }

        public ActionResult Amar()
        {
            return PartialView();
        }

        public ActionResult RptAmar(string date, string Vade)
        {
           // Session["date"] = date;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_AmarSelectTableAdapter sp_Amar = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_AmarSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_Amar.Fill(dt.sp_AmarSelect, MyLib.Shamsi.Shamsi2miladiDateTime(date.ToString()), Convert.ToByte(Vade));
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptAmar.frx");
            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            //Report.SetParameterValue("University", "دانشکده فنی دختران شاهرود");

            //Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            //var Currentdate = p.sp_GetDate().FirstOrDefault();
            Report.SetParameterValue("date", "");
            Report.SetParameterValue("time", "");
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }
        public ActionResult AmarInDate()
        {
            return PartialView();
        }

        public ActionResult RptAmarInDate(string Sdate, string Edate)
        {
            // Session["date"] = date;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_AmarFromDateToDateSelectTableAdapter sp_Amar = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_AmarFromDateToDateSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_Amar.Fill(dt.sp_AmarFromDateToDateSelect, MyLib.Shamsi.Shamsi2miladiDateTime(Sdate.ToString()), MyLib.Shamsi.Shamsi2miladiDateTime(Edate.ToString()));
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptAmarFromDateToDate.frx");
            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            //Report.SetParameterValue("University", "دانشکده فنی دختران شاهرود");
            Report.SetParameterValue("date", "");
            Report.SetParameterValue("time", "");
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }
    }
}
