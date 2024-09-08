using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Aspose.Cells;
using System.Data;
using System.Web.UI;

namespace UniAutomation.Areas.faces.Controllers
{
    public class RptPrintStudentController : Controller
    {
        //
        // GET: /faces/RptPrintStudent/

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
        public ActionResult StudentTermReport(int TermId, string Date)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            //Session["TermId"] = TermId;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblStudentSelectTableAdapter Student = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblStudentSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            if (TermId == 0 && Date!="")
                Student.Fill(dt.sp_tblStudentSelect, "fldDate", MyLib.Shamsi.Shamsi2miladiString(Date),"", 0);

            else if (TermId != 0 && Date == "")
                Student.Fill(dt.sp_tblStudentSelect, "fldTermId", TermId.ToString(),"", 0);

            else
                Student.Fill(dt.sp_tblStudentSelect, "fldTermId_Date",TermId.ToString(), MyLib.Shamsi.Shamsi2miladiString(Date), 0);

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
        public FileResult CreateExcel(int TermId, string Date)
        {
            string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC" };
            Workbook wb = new Workbook();
            Worksheet sheet = wb.Worksheets[0];
            string TermName = "";

            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var Student = p.sp_tblStudentSelect("fldTermId", TermId.ToString(),"", 0).ToList();

            if (TermId == 0 && Date != "")
                 Student = p.sp_tblStudentSelect("fldDate", MyLib.Shamsi.Shamsi2miladiString(Date),"", 0).ToList();

            else if (TermId != 0 && Date == "")
                  Student = p.sp_tblStudentSelect("fldTermId", TermId.ToString(),"", 0).ToList();

            else
                  Student = p.sp_tblStudentSelect("fldTermId_Date", TermId.ToString(), MyLib.Shamsi.Shamsi2miladiString(Date), 0).ToList();

            sheet.Cells[alpha[0] + "1"].PutValue("نام");
            sheet.Cells[alpha[1] + "1"].PutValue("نام خانوادگی");
            sheet.Cells[alpha[2] + "1"].PutValue("کد ملی");
            sheet.Cells[alpha[3] + "1"].PutValue("شماره دانشجویی");
            sheet.Cells[alpha[4] + "1"].PutValue("مقطع تحصیلی");
            sheet.Cells[alpha[5] + "1"].PutValue("رشته تحصیلی");
            sheet.Cells[alpha[6] + "1"].PutValue("وضعیت تحصیلی");
            sheet.Cells[alpha[7] + "1"].PutValue("شماره تلفن دانشجو");
            sheet.Cells[alpha[8] + "1"].PutValue("شماره تلفن والدین");

            int i = 0;
            foreach (var _item in Student)
            {
                sheet.Cells[alpha[0] + (i + 2)].PutValue(_item.fldName);
                sheet.Cells[alpha[1] + (i + 2)].PutValue(_item.fldFamily);
                sheet.Cells[alpha[2] + (i + 2)].PutValue(_item.fldMelliCode);
                sheet.Cells[alpha[3] + (i + 2)].PutValue(_item.fldStudentNumber);
                sheet.Cells[alpha[4] + (i + 2)].PutValue(_item.fldSectionName);
                sheet.Cells[alpha[5] + (i + 2)].PutValue(_item.fldCourseName);
                sheet.Cells[alpha[6] + (i + 2)].PutValue(_item.fldStatusName);
                sheet.Cells[alpha[7] + (i + 2)].PutValue(_item.fldMobile);
                sheet.Cells[alpha[8] + (i + 2)].PutValue(_item.fldParentPhone);
                TermName = _item.fldTermName;
                i++;
            }

            MemoryStream stream = new MemoryStream();
            wb.Save(stream, SaveFormat.Excel97To2003);
            return File(stream.ToArray(), "xls", "Student" + TermName + ".xls");
        }
    }
}
