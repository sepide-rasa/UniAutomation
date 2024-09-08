using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniAutomation.Areas.faces.Controllers.Acc;

namespace UniAutomation.Areas.faces.Controllers
{
    public class SettingController : Controller
    {
        //
        // GET: /faces/Setting/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 89))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult Save(Models.Setting Setting)
        {
            try
            {
                byte[] image = null;
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                if (Setting.fldImage != null)
                    image = UniAutomation.Helper.ClsCommon.Base64ToImage(Setting.fldImage);
                if (Setting.fldId == 0)
                {//ثبت رکورد جدید
                    
                    return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });

                }
                else
                {//ویرایش رکورد ارسالی 
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 90))
                    {
                        p.sp_tblSettingUpdate(Setting.fldId, "", Setting.fldStartTime, Setting.fldEndTime, Setting.fldReservDay, image, Setting.fldRais);
                        return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
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

        public JsonResult Details()
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_tblSettingSelect().FirstOrDefault();
                return Json(new
                {
                    fldID = q.fldId,
                    fldReservDay = q.fldReservDay,
                    fldImage=q.fldImage,
                    fldName=Class1.stringDecode(q.fldName),
                    fldRais = q.fldRais
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public FileContentResult Image(int id)
        {//برگرداندن عکس 
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();

            var pic = p.sp_tblSettingSelect().FirstOrDefault();
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
