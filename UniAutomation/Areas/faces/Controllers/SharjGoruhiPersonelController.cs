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
    public class SharjGoruhiPersonelController : Controller
    {
        //
        // GET: /faces/SharjGoruhiPersonel/

        public ActionResult Index(string state)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 241))
            {
                Session["state"] = state;
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
            var q = m.sp_GroupChargeSelect(Session["state"].ToString(), null, null).ToList().ToDataSourceResult(request);
            return Json(q);
        }
        public ActionResult Reload()
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_GroupChargeSelect(Session["state"].ToString(), null, null).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(List<Models.charge> ArrayL)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 242))
                {
                    foreach (var item in ArrayL)
                    {
                        if (item.fldDesc == null)
                            item.fldDesc = "";
                        if (item.fldPrice != 0)

                            p.sp_tblChargeInsert(item.fldFoodCartsID, item.fldPrice, 1, item.fldDesc, Convert.ToInt32(Session["UserId"]));

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
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 242))
                {

                    Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_tblChargeDelete(Convert.ToByte(id), Convert.ToInt32(Session["UserId"]));
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
