using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniAutomation.Areas.faces.Controllers.Acc;

namespace UniAutomation.Areas.faces.Controllers.BasicInf
{
    public class ApplicationPartController : Controller
    {
        //
        // GET: /ApplicationPart/

        public ActionResult Index()
        {
        //    if (Session["UserId"] == null)
        //        return RedirectToAction("logon", "Account");
        //    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 3))
        //    {
                return PartialView();
        //    }
        //    else
        //    {
        //        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
        //        return RedirectToAction("error", "Metro");
        //    }

        }

        public JsonResult _RolsTree(int? id)
        {
            try
            {
                var p = new Models.UniAutomationEntities1();

                if (id != null)
                {
                    var rols = (from k in p.sp_tblApplicationPartSelect("fldPID", id.ToString(), 0)
                                select new
                                {
                                    id = k.fldID,
                                    Name = k.fldTitle,
                                    hasChildren = p.sp_tblApplicationPartSelect("fldPID", id.ToString(), 0).Any()

                                });
                    return Json(rols, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var rols = (from k in p.sp_tblApplicationPartSelect("", "", 0)
                                select new
                                {
                                    id = k.fldID,
                                    Name = k.fldTitle,
                                    hasChildren = p.sp_tblApplicationPartSelect("", "", 0).Any()

                                });
                    return Json(rols, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                return null;
            }
        }

        public ActionResult Save(Models.sp_tblApplicationPartSelect AppPart)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                if (AppPart.fldDesc == null)
                    AppPart.fldDesc = "";
                if (AppPart.fldID == 0)
                {//ثبت رکورد جدید
                    p.sp_tblApplicationPartInsert(AppPart.fldTitle, AppPart.fldPID, Convert.ToInt32(Session["UserId"]), AppPart.fldDesc);
                    return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                }
                else
                {//ویرایش رکورد ارسالی
                    p.sp_tblApplicationPartUpdate(AppPart.fldID, AppPart.fldTitle, AppPart.fldPID, Convert.ToInt32(Session["UserId"]), AppPart.fldDesc);
                    return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
                }
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                if (Convert.ToInt32(id) != 0)
                {
                    Car.sp_tblApplicationPartDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
                    return Json(new { data = "حذف با موفقیت انجام شد.", state = 0 });
                }
                else
                {
                    return Json(new { data = "رکوردی برای حذف انتخاب نشده است.", state = 1 });
                }

            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public JsonResult Details(int id)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_tblApplicationPartSelect("fldID", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldTitle = q.fldTitle,
                    fldID = q.fldID,
                    fldPId = q.fldPID
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return null;
            }
        }

    }
}
