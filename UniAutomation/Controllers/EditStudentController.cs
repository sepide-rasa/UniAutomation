using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniAutomation.Controllers
{
    public class EditStudentController : Controller
    {
        //
        // GET: /EditStudent/

        public ActionResult Index()
        {
                return PartialView();

        }
        public ActionResult Save(Areas.faces.Models.Student stu)
        {
            try
            {
                Areas.faces.Models.UniAutomationEntities1 p = new Areas.faces.Models.UniAutomationEntities1();

                byte[] image = null;

                if (stu.fldPic != null)
                    image = UniAutomation.Helper.ClsCommon.Base64ToImage(stu.fldPic);


                if (stu.fldDesc == null)
                    stu.fldDesc = "";
                if (stu.fldEmail == null)
                    stu.fldEmail = "";
                if (stu.fldMobile == null)
                    stu.fldMobile = "";//ویرایش رکورد ارسالی 

                var q = p.sp_tblStudentSelect("fldId", stu.fldID.ToString(),"", 1).FirstOrDefault();

                        var k = p.sp_tblPicturesSelect("fldStudentID", stu.fldID.ToString(), 1).FirstOrDefault();
                        p.sp_tblPicturesUpdate(k.fldID, stu.fldID, null, null, null, image, q.fldUserID,"");

                        p.sp_tblStudentUpdate(stu.fldID, q.fldName, q.fldFamily, q.fldMelliCode, q.fldGender, q.fldStudentNumber, q.fldSystemNumber, q.fldCourseID, q.fldTermId, q.fldStatusID, q.fldSectionID, stu.fldMobile, stu.fldEmail, stu.fldParentPhone, stu.fldTelephone, stu.fldCity, stu.fldFatherName, stu.fldShenasnameNo, stu.fldMahaleSodoor, stu.fldBDate,stu.fldMazhab, q.fldUserID, stu.fldDesc);
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
                var q = p.sp_tblStudentSelect("fldId",m.fldStudentID.ToString(),"", 1).FirstOrDefault();
                var k = p.sp_tblPicturesSelect("fldStudentID", q.fldID.ToString(), 1).FirstOrDefault();

                return Json(new
                {
                    fldID = q.fldID,
                    fldName = q.fldName,
                    fldCity = q.fldCity,
                    fldFamily = q.fldFamily,
                    fldStatusID = q.fldStatusID,
                    fldCourseID = q.fldCourseID,
                    fldSectionID = q.fldSectionID,
                    fldCourseName = q.fldCourseName,
                    fldStatusName = q.fldStatusName,
                    fldSectionName = q.fldSectionName,
                    fldSystemNumber = q.fldSystemNumber,
                    fldEmail = q.fldEmail,
                    fldMelliCode = q.fldMelliCode,
                    TermId = q.fldTermId,
                    TermName = q.fldTermName,
                    fldStudentNumber = q.fldStudentNumber,
                    fldDesc = q.fldDesc,
                    fldpic = k.fldID,
                    fldGender = q.fldGender,
                    fldMobile = q.fldMobile,
                    fldTelephone = q.fldTelephone,
                    fldParentPhone = q.fldParentPhone,
                    fldFatherName = q.fldFatherName,
                    fldMahaleSodoor = q.fldMahaleSodoor,
                    fldShenasnameNo = q.fldShenasnameNo,
                    fldBDate = q.fldBDate

                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }

        public FileContentResult StudentImage(int id)
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
        public JsonResult IsFill()
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Areas.faces.Models.UniAutomationEntities1 p = new Areas.faces.Models.UniAutomationEntities1();
                var m = p.sp_tblFoodCartsSelect("fldID", Session["PersonId"].ToString(), 0).FirstOrDefault();
                    var Notfill = 0;
                if (m.fldStudentID != null)
                {
                    var q = p.sp_tblStudentSelect("fldId", m.fldStudentID.ToString(), "", 1).FirstOrDefault();
                    if (!(q.fldCity != "" && q.fldEmail != "" && q.fldMobile != "" && q.fldTelephone != "" && q.fldParentPhone != ""))
                        Notfill = 1;
                }
                return Json(new
                {
                    Notfill =Notfill
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
    }
}
