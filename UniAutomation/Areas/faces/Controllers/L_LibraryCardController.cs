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
    public class L_LibraryCardController : Controller
    {
        //
        // GET: /faces/L_LibraryCard/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 11))
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
            var q = m.sp_tblL_LibraryCardSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }


        public ActionResult Save(Models.sp_tblL_LibraryCardSelect cart)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 47))
                {


                    if (cart.fldDesc == null)
                        cart.fldDesc = "";
                    if (cart.fldId == 0)
                    {//ثبت رکورد جدید


                        p.sp_tblL_LibraryCardInsert(cart.fldPersonalId, cart.fldTeacherId, cart.fldStudentId, cart.fldStatus, Convert.ToInt32(Session["UserId"]), cart.fldDesc);

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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 48))
                    {


                        p.sp_tblL_LibraryCardUpdate(cart.fldId, cart.fldPersonalId, cart.fldTeacherId, cart.fldStudentId, cart.fldStatus, Convert.ToInt32(Session["UserId"]), cart.fldDesc);
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

        //public ActionResult Reload(string field, string value, string sal, string mah, string nobat, int top, int searchtype)
        //{//جستجو
        //    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
        //    var q = m.sp_tblMorkhasiSelect("Sal_Mah_Nobat", value, Convert.ToInt32(sal), Convert.ToInt32(mah), Convert.ToInt32(nobat), top).ToList();

        //    return Json(q, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldMelliCode", "fldName", "fldPersonalId", "fldTeacherId", "fldStudentId", "fldFamily" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblL_LibraryCardSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 49))
                {

                    Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_tblL_LibraryCardDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
                var q = p.sp_tblL_LibraryCardSelect("fldID", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {

                    fldId = q.fldId,
                    fldPersonalId = q.fldPersonalId,
                    fldStudentId = q.fldStudentId,
                    fldTeacherId = q.fldTeacherId,
                    fldStatus = q.fldStatus,
                    fldName = q.fldName +" "+q.fldFamily,
                    fldMelliCode = q.fldMelliCode,
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
