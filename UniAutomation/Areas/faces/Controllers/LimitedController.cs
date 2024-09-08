using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using UniAutomation.Areas.faces.Controllers.Acc;

namespace UniAutomation.Areas.faces.Controllers
{
    public class LimitedController : Controller
    {
        //
        // GET: /faces/Limited/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]),214))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
          
        }
        
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblLimitedSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Save(Models.sp_tblLimitedSelect limit)
        {
            try
            {

                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                if (limit.fldDesc == null)
                    limit.fldDesc = "";
                if (limit.fldID == 0)
                {//ثبت رکورد جدید
                     if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 215))
                    {
                        p.sp_tblLimitedInsert(limit.fldPersonal, limit.fldStudent, limit.fldTeacher, limit.fldSefServiceID, MyLib.Shamsi.Shamsi2miladiDateTime(limit.fldRoleDate), Convert.ToInt32(Session["UserId"]), limit.fldDesc);
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
                          if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 216))
                    {

                        p.sp_tblLimitedUpdate(limit.fldID, limit.fldPersonal, limit.fldStudent, limit.fldTeacher, limit.fldSefServiceID, MyLib.Shamsi.Shamsi2miladiDateTime(limit.fldRoleDate), Convert.ToInt32(Session["UserId"]), limit.fldDesc);
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
            string[] _fiald = new string[] { "fldId"};
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblLimitedSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 217))
                {
                    Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_tblLimitedDelete(Convert.ToByte(id), Convert.ToInt32(Session["UserId"]));
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
                var q = p.sp_tblLimitedSelect("fldId", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldID = q.fldID,
                    fldPersonal = q.fldPersonal,
                    fldStudent = q.fldStudent,
                    fldTeacher = q.fldTeacher,
                    fldRoleDate = q.fldRoleDate,
                    fldDesc = q.fldDesc
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }

    }
}
