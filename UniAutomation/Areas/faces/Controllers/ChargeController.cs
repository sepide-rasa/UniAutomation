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
    public class ChargeController : Controller
    {
        //
        // GET: /faces/Charge/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 15))
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
            var q = m.sp_tblChargeSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Save(Models.sp_tblChargeSelect charge)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                if (charge.fldDesc == null)
                    charge.fldDesc = "";
                if (charge.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 59))
                    {
                    p.sp_tblChargeInsert(charge.fldFoodCartsID, charge.fldPrice, charge.fldChargeType, charge.fldDesc,Convert.ToInt32(Session["UserId"]));
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
                        if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 60))
                    {

                    p.sp_tblChargeUpdate(charge.fldID, charge.fldFoodCartsID, charge.fldPrice, charge.fldChargeType,Convert.ToInt32(Session["UserId"]), charge.fldDesc);
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
            string[] _fiald = new string[] { "fldId", "fldFoodCartsID" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblChargeSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 61))
                    {

                Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                if (Convert.ToInt32(id) != 0)
                {
                    Car.sp_tblChargeDelete(Convert.ToInt64(id),Convert.ToInt32(Session["UserId"]));
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
                var q = p.sp_tblChargeSelect("fldId", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldID = q.fldID,
                    fldChargeType = q.fldChargeType,
                    fldFoodCartsID = q.fldFoodCartsID,
                    fldPrice = q.fldPrice,
                    PersonName = q.PersonName,
                    PersonMelliCode = q.PersonMelliCode,
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
