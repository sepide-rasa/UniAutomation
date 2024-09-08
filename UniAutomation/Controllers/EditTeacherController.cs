using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniAutomation.Controllers
{
    public class EditTeacherController : Controller
    {
        //
        // GET: /EditTeacher/

        public ActionResult Index()
        {
            return PartialView();
        }
        public ActionResult Save(Areas.faces.Models.Teacher teach)
        {
            try
            {
                Areas.faces.Models.UniAutomationEntities1 p = new Areas.faces.Models.UniAutomationEntities1();

                byte[] image = null;

                if (teach.fldPic != null)
                    image = UniAutomation.Helper.ClsCommon.Base64ToImage(teach.fldPic);


                if (teach.fldDesc == null)
                    teach.fldDesc = "";
                if (teach.fldEmail == null)
                    teach.fldEmail = "";
                if (teach.fldMobile == null)
                    teach.fldMobile = "";//ویرایش رکورد ارسالی 

                var q = p.sp_tblTeachersSelect("fldId", teach.fldID.ToString(), 1).FirstOrDefault();

                var k = p.sp_tblPicturesSelect("fldTeacherID", teach.fldID.ToString(), 1).FirstOrDefault();
                p.sp_tblPicturesUpdate(k.fldID, null, teach.fldID, null, null, image, q.fldUserID, "");

                p.sp_tblTeachersUpdate(teach.fldID, q.fldName, q.fldFamily, q.fldMelliCode, q.fldCourseID,teach.fldMobile, teach.fldEmail, q.fldUserID, teach.fldDesc);
                return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });

            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public JsonResult Details()
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Areas.faces.Models.UniAutomationEntities1 p = new Areas.faces.Models.UniAutomationEntities1();
                var m = p.sp_tblFoodCartsSelect("fldID", Session["PersonId"].ToString(), 0).FirstOrDefault();
                var q = p.sp_tblTeachersSelect("fldId", m.fldTeacherID.ToString(), 1).FirstOrDefault();
                var k = p.sp_tblPicturesSelect("fldTeacherID", q.fldID.ToString(), 1).FirstOrDefault();

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
                    fldpic = k.fldID

                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }

        public FileContentResult TeacherImage(int id)
        {//برگرداندن عکس 
            Areas.faces.Models.UniAutomationEntities1 p = new Areas.faces.Models.UniAutomationEntities1();

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
    }
}
