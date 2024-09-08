using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace UniAutomation.Areas.faces.Controllers
{
    public class BarCodBookStockController : Controller
    {
        //
        // GET: /faces/BarCodBookStock/

        public ActionResult Index(int id)
        {
            ViewBag.BookStockId = id;
            return PartialView();
        }

        public ActionResult RptSelectedBook(string BookId)
        {
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            var ID=BookId.Split(';');
            for(var i=0;i<ID.Count()-1;i++){
                var q=m.sp_tblL_BookStockSelect("fldBookId",ID[i],0).ToList();
                foreach (var Item in q)
                    dt.sp_tblL_BookStockSelect.Addsp_tblL_BookStockSelectRow(Item.fldId, Item.fldBookId, Item.fldUserID, Item.fldDate, Item.fldDesc, Item.fldBookTitle,
                        Item.fldPublisher,Item.fldAuthor,Item.fldPersianName,Item.fldEnglishName, Item.fldNashr, Item.fldRadeBandi_Kongere, Convert.ToInt32(Item.fldTirazh), 
                        Item.fldNobateChap,Item.fldPublicationYear,Convert.ToBoolean(Item.fldActive_Deactive));
            }

            Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptBarCodBookStockGroup.frx");
            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            //Report.SetParameterValue("University", "دانشکده فنی دختران شاهرود");
            Report.SetParameterValue("date", "");
            Report.SetParameterValue("time", "");
            //Report.SetParameterValue("p", Convert.ToInt32(P));
            Report.Prepare();
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);

            Session.Remove("Books");
            return File(stream.ToArray(), "application/pdf");
        }
        public ActionResult rptBarcode(int id, int P)
        {
           // Session["id"] = id;
           // Session["P"] = P;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblL_BookStockSelectTableAdapter sp_Barcode = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblL_BookStockSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_Barcode.Fill(dt.sp_tblL_BookStockSelect, "fldId", id.ToString(), 0);
            Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptBarCodBookStock.frx");
            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            //Report.SetParameterValue("University", "دانشکده فنی دختران شاهرود");
            Report.SetParameterValue("date", "");
            Report.SetParameterValue("time", "");
            Report.SetParameterValue("p", Convert.ToInt32(P));
            Report.Prepare();
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }
        public ActionResult rptBarcodeGroup(int BookId)
        {
            // Session["id"] = id;
            // Session["P"] = P;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblL_BookStockSelectTableAdapter sp_Barcode = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblL_BookStockSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_Barcode.Fill(dt.sp_tblL_BookStockSelect, "fldBookId", BookId.ToString(), 0);
            Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\RptBarCodBookStockGroup.frx");
            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            Report.SetParameterValue("University", "دانشکده فنی دختران شاهرود");
            Report.SetParameterValue("date", "");
            Report.SetParameterValue("time", "");
            //Report.SetParameterValue("p", Convert.ToInt32(P));
            Report.Prepare();
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }

    }
}
