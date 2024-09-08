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
    public class SharjGoruhiController : Controller
    {
        //
        // GET: /faces/SharjGoruhi/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 16))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
          
        }

        public ActionResult GetGroups()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblEducationGroupSelect("", "", 0).ToList().Select(c => new { fldID = c.fldID, fldName = c.fldName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTerms()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_B_tblTermsSelect("", "", 0).ToList().Select(c => new { fldID = c.fldId, fldName = c.fldTermName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reload(int? id,int? Term)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_GroupChargeSelect("Student", id, Term).ToList();

            return Json(q,JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(List<Models.charge> ArrayL)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 62))
                {
                    foreach (var item in ArrayL)
                    {
                        if (item.fldDesc == null)
                            item.fldDesc = "";
                        if (item.fldPrice != 0)

                            p.sp_tblChargeInsert(item.fldFoodCartsID, item.fldPrice, 1, item.fldDesc,Convert.ToInt32(Session["UserId"]));

                    }
                    return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
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

        //public ActionResult Reload(string value, string Sal, string mah, string nobat)
        //{//جستجو

        //    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
        //    var q = m.sp_GroupChargeSelect().ToList();
        //    return Json(q, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 64))
                {

                    Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_tblChargeDelete(Convert.ToByte(id),Convert.ToInt32(Session["UserId"]));
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

    }
}
