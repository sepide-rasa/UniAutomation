using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniAutomation.Areas.faces.Controllers.Acc;

namespace UniAutomation.Areas.faces.Controllers
{
    public class RptPrintMadrakController : Controller
    {
        //
        // GET: /faces/RptPrintMadrak/

        public ActionResult Index(long StudentId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 236))
            {
                ViewBag.StudentId = StudentId;
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }

        }
        public ActionResult Save(Models.sp_tblMadrakSelect M)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                if (M.fldDesc == null)
                    M.fldDesc = "";
                if (M.fldId == 0)
                {//ثبت رکورد جدید
                    //if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 202))
                    //{
                    p.sp_tblMadrakInsert(M.fldMadrakNum, MyLib.Shamsi.Shamsi2miladiDateTime(M.fldMadrakDate), PdfFile(M.fldStudentId), M.fldStudentId, Convert.ToInt32(Session["UserId"]), "");
                        return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                    //}
                    //else
                    //{
                    //    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                    //    return RedirectToAction("error", "Metro");
                    //}
                }
                else
                {//ویرایش رکورد ارسالی 
                    //if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 203))
                    //{
                    p.sp_tblMadrakUpdate(M.fldId, M.fldMadrakNum, MyLib.Shamsi.Shamsi2miladiDateTime(M.fldMadrakDate), PdfFile(M.fldStudentId), M.fldStudentId, Convert.ToInt32(Session["UserId"]), "");
                      return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
                    //}
                    //else
                    //{
                    //    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                    //    return RedirectToAction("error", "Metro");
                    //}
                }
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public JsonResult Details(long id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_tblMadrakSelect("fldStudentId", id.ToString(), 1).Select(c => new { fldId = c.fldId, fldMadrakDate = c.fldMadrakDate, fldMadrakNum=c.fldMadrakNum }).FirstOrDefault();
                var fldId = 0;
                var fldMadrakDate = "";
                var fldMadrakNum = "";
                var fldStudentId = id;
                if (q != null)
                {
                    fldId = q.fldId;
                    fldMadrakDate = q.fldMadrakDate;
                    fldMadrakNum = q.fldMadrakNum;
                }
                return Json(new
                {
                    fldId = fldId,
                    fldMadrakDate = fldMadrakDate,
                    fldMadrakNum = fldMadrakNum,
                    fldStudentId = fldStudentId
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public byte[] PdfFile(long StudentId)
        {
            UniAutomation.Areas.faces.DataSet.DataSet1 dt2 = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblStudentSelectTableAdapter sp_Student = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblStudentSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblMadrakSelectTableAdapter sp_Madrak = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblMadrakSelectTableAdapter();

            sp_Madrak.Fill(dt2.sp_tblMadrakSelect, "fldStudentId", StudentId.ToString(), 0);
            sp_Student.Fill(dt2.sp_tblStudentSelect, "fldID", StudentId.ToString(), "", 0);
            sp_Setting.Fill(dt2.sp_tblSettingSelect);
            dt2.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt2.sp_tblSettingSelect[0].fldName);

            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var s=p.sp_tblStudentSelect("fldID",StudentId.ToString(),"",0).FirstOrDefault();
            FastReport.Report Report = new FastReport.Report();
            if(s.fldSectionID==1)//کاردانی
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptMadrak.frx");
            else if (s.fldSectionID == 2)//کارشناسی
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptMadrakKarshenasi.frx");
            Report.RegisterData(dt2, "uniAutomationDataSet");

            Report.Prepare();
            FastReport.Export.Image.ImageExport pdf = new FastReport.Export.Image.ImageExport();
            pdf.ImageFormat = FastReport.Export.Image.ImageExportFormat.Tiff;
            pdf.Resolution = 500;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);
            return stream.ToArray();
        }
        public ActionResult RptPrint(int StudentId)
        {
            var F=PdfFile(StudentId);

            return File(F, "application/tiff", "Madrak" + StudentId +".tiff");
        }
        //public ActionResult RptPrint(int StudentId)
        //{
        //    var F = PdfFile(StudentId);

        //    return File(F, "application/pdf");
        //}
    }
}
