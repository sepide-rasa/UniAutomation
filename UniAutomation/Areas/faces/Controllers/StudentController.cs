using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using UniAutomation.Areas.faces.Controllers.Acc;
using System.IO;

namespace UniAutomation.Areas.faces.Controllers
{
    public class StudentController : Controller
    {
        //
        // GET: /faces/Student/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 4))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
 
        }

        public ActionResult GetSection()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblSectionSelect("","", 0).ToList().Select(c => new { fldID = c.fldID, fldName = c.fldName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCourse()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblCourseSelect("","", 0).ToList().Select(c => new { fldID = c.fldID, fldName = c.fldName+"("+c.fldEducationGroupName+")"  });
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStatus()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblStatusSelect("", "", 0).ToList().Select(c => new { fldID = c.fldID, fldName = c.fldName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTerm()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_B_tblTermsSelect("", "", 0).OrderByDescending(k=>k.fldId).ToList().Select(c => new { fldID = c.fldId, fldName = c.fldTermName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblStudentSelect("","","", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Save(Models.Student stu)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();

                byte[] image = null;

                if (stu.fldPic != null)
                    image = UniAutomation.Helper.ClsCommon.Base64ToImage(stu.fldPic);


                if (stu.fldDesc == null)
                    stu.fldDesc = "";
                if (stu.fldEmail == null)
                    stu.fldEmail = "";
                if (stu.fldMobile == null)
                    stu.fldMobile = "";
                if (stu.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 26))
                    {

                        System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldID", typeof(int));

                        p.sp_tblStudentInsert(_id, stu.fldName, stu.fldFamily, stu.fldMeliCode, stu.fldGender, stu.fldStudentNumber, stu.fldSystemNumber, stu.fldCourseID, stu.fldTermId, stu.fldStatusID, stu.fldSectionID, stu.fldMobile, stu.fldEmail, stu.fldParentPhone, stu.fldTelephone, stu.fldCity, stu.fldFatherName, stu.fldShenasnameNo, stu.fldMahaleSodoor, stu.fldBDate, stu.fldMazhab, Convert.ToInt32(Session["UserId"]), stu.fldDesc);
                        p.sp_tblPicturesInsert(Convert.ToInt32(_id.Value), null, null, null, image, Convert.ToInt32(Session["UserId"]), stu.fldDesc);
                        p.sp_tblFoodCartsInsert(Convert.ToInt32(_id.Value), null, null, stu.fldStudentNumber,  Class1.GenerateHash(stu.fldMeliCode), 1, Convert.ToInt32(Session["UserId"]), "", "");
                        return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                    }
                    else
                    {
                        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");
                    }

                }
                else
                {//ویرایش رکورد ارسالی 
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 27))
                    {

                        var k = p.sp_tblPicturesSelect("fldStudentID", stu.fldID.ToString(), 1).FirstOrDefault();
                        p.sp_tblPicturesUpdate(k.fldID, stu.fldID, null, null, null, image, Convert.ToInt32(Session["UserId"]), stu.fldDesc);
                        p.sp_tblStudentUpdate(stu.fldID, stu.fldName, stu.fldFamily, stu.fldMeliCode, stu.fldGender, stu.fldStudentNumber, stu.fldSystemNumber, stu.fldCourseID, stu.fldTermId, stu.fldStatusID, stu.fldSectionID, stu.fldMobile, stu.fldEmail, stu.fldParentPhone, stu.fldTelephone, stu.fldCity, stu.fldFatherName, stu.fldShenasnameNo, stu.fldMahaleSodoor, stu.fldBDate,stu.fldMazhab, Convert.ToInt32(Session["UserId"]), stu.fldDesc);
                        
                        var F = p.sp_tblFoodCartsSelect("fldStudentID", stu.fldID.ToString(), 0).FirstOrDefault();
                        p.sp_tblFoodCartsUpdate(F.fldID,stu.fldID, null, null, stu.fldStudentNumber,  Class1.GenerateHash(stu.fldMeliCode), 1, Convert.ToInt32(Session["UserId"]), "", "");
                        return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
                    }
                    else
                    {
                        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");
                    }


                }
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public ActionResult InsertFoodCart(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_tblStudentSelect("fldTermId", id.ToString(),"", 0).ToList();
                foreach (var stu in q)
                {
                    if (!p.sp_tblFoodCartsSelect("fldStudentID ", stu.fldID.ToString(), 0).Any())
                        p.sp_tblFoodCartsInsert(stu.fldID, null, null, stu.fldStudentNumber,  Class1.GenerateHash(stu.fldMelliCode), 1, Convert.ToInt32(Session["UserId"]), "", "");
                }
                return Json(new { data = " با موفقیت انجام شد.", state = 0 });
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldFamily", "fldName", "fldStudentNumber", "fldMelliCode" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblStudentSelect(_fiald[Convert.ToInt32(field)], searchtext,"", top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 28))
                {

                    Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        var f=p.sp_tblFoodCartsSelect("fldStudentID ", id, 0).FirstOrDefault();
                        p.sp_tblFoodCartsDelete(f.fldID, Convert.ToInt32(Session["UserId"]));

                        var pic=p.sp_tblPicturesSelect("fldStudentID", id, 0).FirstOrDefault();
                        p.sp_tblPicturesDelete(pic.fldID, Convert.ToInt32(Session["UserId"]));

                        p.sp_tblStudentDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
                        return Json(new { data = "حذف با موفقیت انجام شد.", state = 0 });

                    }
                    else
                    {
                        return Json(new { data = "رکوردی برای حذف انتخاب نشده است.", state = 1 });
                    }
                }
                else
                {
                    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                    return RedirectToAction("error", "Metro");
                }



            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_tblStudentSelect("fldId", id.ToString(),"", 1).FirstOrDefault();
                var k = p.sp_tblPicturesSelect("fldStudentID", q.fldID.ToString(), 1).FirstOrDefault();

                return Json(new
                {
                    fldID = q.fldID,
                    fldMazhab=q.fldMazhab,
                    fldName = q.fldName,
                    fldCity = q.fldCity,
                    fldFamily = q.fldFamily,
                    fldStatusID = q.fldStatusID,
                    fldCourseID = q.fldCourseID,
                    fldSectionID = q.fldSectionID,
                    fldCourseName=q.fldCourseName,
                    fldStatusName=q.fldStatusName,
                    fldSectionName=q.fldSectionName,
                    fldSystemNumber = q.fldSystemNumber,
                    fldEmail= q.fldEmail,
                    fldMelliCode = q.fldMelliCode,
                    TermId = q.fldTermId,
                    TermName = q.fldTermName,
                    fldStudentNumber = q.fldStudentNumber,
                    fldDesc = q.fldDesc,
                    fldpic = k.fldID,
                    fldGender = q.fldGender,
                    fldMobile = q.fldMobile,
                    fldTelephone=q.fldTelephone,
                    fldParentPhone=q.fldParentPhone,
                    fldFatherName=q.fldFatherName,
                    fldMahaleSodoor=q.fldMahaleSodoor,
                    fldShenasnameNo=q.fldShenasnameNo,
                    fldBDate=q.fldBDate
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }


        public FileContentResult StudentImage(int id)
        {//برگرداندن عکس 
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();

            var pic = p.sp_tblPicturesSelect("fldId", id.ToString(), 1).FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldImage != null)
                {
                    return File((byte[])pic.fldImage , "jpg");
                }

            }
            return null;

        }
        public ActionResult PrintKartWin(string id)
        {
            ViewBag.StudentId = id;
            return PartialView();
        }
        public FileContentResult rptKart(string StudentId, string TarikhSodoor, string TarikhEtebar, string Type)
        {
            // Session["date"] = date;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_studentDetailSelectTableAdapter sp_Student = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_studentDetailSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_Student.Fill(dt.sp_studentDetailSelect, "fldIdIN", StudentId, 0);
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();

            if (Type == "1")
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Kart.frx");
            else
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\KartAlmosana.frx");

            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            //Report.SetParameterValue("University", "دانشکده فنی دختران شاهرود");
            Report.SetParameterValue("TarikhSodoor", TarikhSodoor);
            Report.SetParameterValue("TarikhEtebar", TarikhEtebar);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            //return File(stream.ToArray(), "application/pdf");
            return File(stream.ToArray(), "application/pdf", "Kart.pdf");
        }
        public FileContentResult DownloadKart(string StudentId, string TarikhSodoor, string TarikhEtebar, string Type)
        {
            // Session["date"] = date;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_studentDetailSelectTableAdapter sp_Student = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_studentDetailSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_Student.Fill(dt.sp_studentDetailSelect, "fldIdIN", StudentId, 0);
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();

            if (Type == "1")
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\Kart.frx");
            else
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\KartAlmosana.frx");

            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            //Report.SetParameterValue("University", "دانشکده فنی دختران شاهرود");
            Report.SetParameterValue("TarikhSodoor", TarikhSodoor);
            Report.SetParameterValue("TarikhEtebar", TarikhEtebar);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            //return File(stream.ToArray(), "application/pdf");
            return File(stream.ToArray(), "application/pdf", "Kart.pdf");
        }
        
    }
}
