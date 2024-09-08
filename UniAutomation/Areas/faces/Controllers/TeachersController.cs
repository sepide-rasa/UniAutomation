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
    public class TeachersController : Controller
    {
        //
        // GET: /faces/Teachers/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 8))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
 
              }
         
        public ActionResult GetCource()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblCourseSelect("", "", 30).ToList().Select(c => new { fldID = c.fldID, fldName = c.fldName + "_" + c.fldEducationGroupName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblTeachersSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Save(Models.Teacher Teacher)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();

                byte[] image = null;

                if (Teacher.fldPic != null)
                    image = UniAutomation.Helper.ClsCommon.Base64ToImage(Teacher.fldPic);


                if (Teacher.fldDesc == null)
                    Teacher.fldDesc = "";
                if (Teacher.fldEmail == null)
                    Teacher.fldEmail = "";
                if (Teacher.fldMobile == null)
                    Teacher.fldMobile = "";
                if (Teacher.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 38))
                    {
                        System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldID", typeof(int));
                        p.sp_tblTeachersInsert(_id, Teacher.fldName, Teacher.fldFamily, Teacher.fldMeliCode, Teacher.fldCourceId, Teacher.fldMobile, Teacher.fldEmail, Convert.ToInt32(Session["UserId"]), Teacher.fldDesc);
                        p.sp_tblPicturesInsert(null, Convert.ToInt32(_id.Value), null, null, image, Convert.ToInt32(Session["UserId"]), Teacher.fldDesc);
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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 39))
                    {

                        var k = p.sp_tblPicturesSelect("fldTeacherID", Teacher.fldID.ToString(), 1).FirstOrDefault();
                        if (k != null)
                            p.sp_tblPicturesUpdate(k.fldID, null, Teacher.fldID, null, null, image, Convert.ToInt32(Session["UserId"]), Teacher.fldDesc);
                        else
                            p.sp_tblPicturesInsert(null, Teacher.fldID, null, null, image, Convert.ToInt32(Session["UserId"]), Teacher.fldDesc);
                        p.sp_tblTeachersUpdate(Teacher.fldID, Teacher.fldName, Teacher.fldFamily, Teacher.fldMeliCode, Teacher.fldCourceId, Teacher.fldMobile, Teacher.fldEmail, Convert.ToInt32(Session["UserId"]), Teacher.fldDesc);
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

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldFamily", "fldName" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblTeachersSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 40))
                {
                    Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_tblTeachersDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
                var q = p.sp_tblTeachersSelect("fldId", id.ToString(), 1).FirstOrDefault();
                var k = p.sp_tblPicturesSelect("fldTeacherID", q.fldID.ToString(), 1).FirstOrDefault();
                var picId = "";
                if (k != null)
                    picId = k.fldID.ToString();
                return Json(new
                {
                    fldID = q.fldID,
                    fldName = q.fldName,
                    fldFamily = q.fldFamily,
                    fldCourseID = q.fldCourseID,
                    fldEmail = q.fldEmail,
                    fldMobile = q.fldMobile,
                    fldMelliCode = q.fldMelliCode,
                    fldDesc = q.fldDesc,
                    fldpicId = picId    
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }


        public FileContentResult TeacherImage(int id)
        {//برگرداندن عکس 
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();

            var pic = p.sp_tblPicturesSelect("fldId", id.ToString(), 1).FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldImage != null)
                {
                    return File((byte[])pic.fldImage, "jpg");
                }

            }
            return null;

        }
        public ActionResult rptKart(string TeacherId)
        {
            // Session["date"] = date;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblTeachersSelectTableAdapter sp_Teacher = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblTeachersSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblPicturesSelectTableAdapter sp_Pic = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblPicturesSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();
        
            sp_Teacher.Fill(dt.sp_tblTeachersSelect, "fldId", TeacherId, 0);
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);
            sp_Pic.Fill(dt.sp_tblPicturesSelect, "fldTeacherID", TeacherId, 0);

            FastReport.Report Report = new FastReport.Report();

            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\KartTeacher.frx");

            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            //Report.SetParameterValue("University", "دانشکده فنی دختران شاهرود");
            //Report.SetParameterValue("TarikhSodoor", TarikhSodoor);
            //Report.SetParameterValue("TarikhEtebar", TarikhEtebar);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport(); 
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }
    }
}
