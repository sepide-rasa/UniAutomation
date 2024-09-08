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
    public class DarsadTakhfifController : Controller
    {
        //
        // GET: /faces/DarsadTakhfif/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 246))
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
            var q = m.sp_tblDarsadTakhfifSelect("", "","", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }


        public ActionResult Save(Models.sp_tblDarsadTakhfifSelect cart)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                if (cart.fldDesc == null)
                    cart.fldDesc = "";
                if (cart.fldID == 0)
                {//ثبت رکورد جدید

                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 247))
                    {
                        p.sp_tblDarsadTakhfifInsert(cart.fldFoodCartID, cart.fldDarsadTakhfif, cart.fldAzTarikh, cart.fldTaTarikh, Convert.ToInt32(Session["UserId"]), cart.fldDesc);

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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 248))
                    {
                        p.sp_tblDarsadTakhfifUpdate(cart.fldID, cart.fldFoodCartID, cart.fldDarsadTakhfif, cart.fldAzTarikh, cart.fldTaTarikh, Convert.ToInt32(Session["UserId"]), cart.fldDesc);
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
            string[] _fiald = new string[] { "PersonName", "MelliCode", "StudentNumber", "fldDarsadTakhfif" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblDarsadTakhfifSelect(_fiald[Convert.ToInt32(field)], searchtext, "",top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 249))
                {

                    Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_tblDarsadTakhfifDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
                var q = p.sp_tblDarsadTakhfifSelect("fldID", id.ToString(),"", 1).FirstOrDefault();
                return Json(new
                {

                    fldID = q.fldID,
                    fldAzTarikh = q.fldAzTarikh,
                    fldDarsadTakhfif = q.fldDarsadTakhfif,
                    fldFoodCartID = q.fldFoodCartID,
                    fldTaTarikh = q.fldTaTarikh,
                    MelliCode = q.MelliCode,
                    PersonName = q.PersonName,
                    fldDesc = q.fldDesc,
                    StudentNumber = q.StudentNumber
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }

    }
}
