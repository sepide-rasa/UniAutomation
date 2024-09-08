using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Aspose.Cells;

namespace UniAutomation.Areas.faces.Controllers
{
    public class RptGozareshBookLibraryController : Controller
    {
        //
        // GET: /faces/RptGozareshBookLibrary/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            return PartialView();
        }
        public ActionResult GetCategoryId()
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblL_BookCategorySelect("", "", 0).ToList().Select(c => new { fldId = c.fldId, fldCategoryId = c.fldName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GozareshBookLibraryReport(int CategoryId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            //Session["CategoryId"] = CategoryId;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblL_BookSelectTableAdapter sp_L_Book = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblL_BookSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblL_BookCategorySelectTableAdapter L_BookCategory = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblL_BookCategorySelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_L_Book.Fill(dt.sp_tblL_BookSelect, "fldCategoryId", CategoryId.ToString(), 0);
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            L_BookCategory.Fill(dt.sp_tblL_BookCategorySelect, "fldId", CategoryId.ToString(), 0);
            FastReport.Report Report = new FastReport.Report();

            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptGozareshBookLibrary.frx");
            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.Prepare();
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }
        public ActionResult ExcelBookLibrary(int CategoryId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var rr = m.sp_tblL_BookCategorySelect("fldId", CategoryId.ToString(), 0).FirstOrDefault().fldName;
            var Checked = "fldBookTitle;fldPersianName;fldEnglishName;fldAuthor;fldPublisher;fldInterpreter;fldIsbn;";
            var data = m.sp_tblL_BookSelect("fldCategoryId", CategoryId.ToString(), 0).ToList(); ;
                string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC" };
                int index = 0;
                var Check = "";
                var StatusCheck = Checked.Split(';');
                var fldBookTitle = ""; var fldPersianName = ""; var fldEnglishName = ""; var fldAuthor = "";
                var fldPublisher = ""; var fldInterpreter = ""; 
                Workbook wb = new Workbook();
                Worksheet sheet = wb.Worksheets[0];

                try
                {
                    foreach (var item in StatusCheck)
                    {
                        switch (item)
                        {
                            case "fldBookTitle":
                                Check = "عنوان کتاب";
                                Cell cell = sheet.Cells[alpha[index] + "1"];
                                cell.PutValue(Check);
                                int i = 0;
                                foreach (var _item in data)
                                {
                                    fldBookTitle = _item.fldBookTitle;
                                    Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                                    Cell.PutValue(fldBookTitle);
                                    i++;
                                }
                                index++;
                                break;
                            case "fldPersianName":
                                Check = "نام فارسی";
                                Cell cell1 = sheet.Cells[alpha[index] + "1"];
                                cell1.PutValue(Check);
                                int j = 0;
                                foreach (var _item in data)
                                {
                                    fldPersianName = _item.fldPersianName;
                                    Cell Cell = sheet.Cells[alpha[index] + (j + 2)];
                                    Cell.PutValue(fldPersianName);
                                    j++;
                                }
                                index++;
                                break;
                            case "fldEnglishName":
                                Check = "نام انگلیسی";
                                Cell cell3 = sheet.Cells[alpha[index] + "1"];
                                cell3.PutValue(Check);
                                int k = 0;
                                foreach (var _item in data)
                                {
                                    fldEnglishName = _item.fldEnglishName;
                                    Cell Cell = sheet.Cells[alpha[index] + (k + 2)];
                                    Cell.PutValue(fldEnglishName);
                                    k++;
                                }
                                index++;
                                break;
                            case "fldPublisher":
                                Check = "ناشر";
                                Cell cell2 = sheet.Cells[alpha[index] + "1"];
                                cell2.PutValue(Check);
                                int qq = 0;
                                foreach (var _item in data)
                                {
                                    fldPublisher = _item.fldPublisher;
                                    Cell Cell = sheet.Cells[alpha[index] + (qq + 2)];
                                    Cell.PutValue(fldPublisher);
                                    qq++;
                                }
                                index++;
                                break;
                            case "fldAuthor":
                                Check = "نویسنده";
                                Cell cell4 = sheet.Cells[alpha[index] + "1"];
                                cell4.PutValue(Check);
                                int q = 0;
                                foreach (var _item in data)
                                {
                                    fldAuthor = _item.fldAuthor;
                                    Cell Cell = sheet.Cells[alpha[index] + (q + 2)];
                                    Cell.PutValue(fldAuthor);
                                    q++;
                                }
                                index++;
                                break;
                            case "fldInterpreter":
                                Check = "مترجم";
                                Cell cell6 = sheet.Cells[alpha[index] + "1"];
                                cell6.PutValue(Check);
                                int z = 0;
                                foreach (var _item in data)
                                {
                                    fldInterpreter = _item.fldInterpreter;
                                    Cell Cell = sheet.Cells[alpha[index] + (z + 2)];
                                    Cell.PutValue(fldInterpreter);
                                    z++;
                                }
                                index++;
                                break;
                            case "fldIsbn":
                                Check = "شابک";
                                Cell cell7 = sheet.Cells[alpha[index] + "1"];
                                cell7.PutValue(Check);
                                int zz = 0;
                                foreach (var _item in data)
                                {
                                    fldInterpreter = _item.fldIsbn;
                                    Cell Cell = sheet.Cells[alpha[index] + (zz + 2)];
                                    Cell.PutValue(fldInterpreter);
                                    zz++;
                                }
                                index++;
                                break;
                            
                        }
                    }
                    MemoryStream stream = new MemoryStream();
                    wb.Save(stream, SaveFormat.Excel97To2003);
                    return File(stream.ToArray(), "xls", "کتاب های رده بندی " + rr + ".xls");
                }
                catch (Exception x)
                {
                    string InnerException = "";
                    if (x.InnerException != null)
                        InnerException = x.InnerException.Message;
                    else
                        InnerException = x.Message;
                    return null;
                }
           
        }
    }
}
