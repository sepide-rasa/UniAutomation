using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace UniAutomation.Controllers
{
    public class PishSabtenam_KhabgahController : Controller
    {
        //
        // GET: /PishSabtenam_Khabgah/

        public ActionResult Index()
        {
            if (Session["PersonId"] == null)
                return RedirectToAction("logIn", "Accounts");

            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var q = p.sp_GetDate().FirstOrDefault();
            ViewBag.Term = Session["Pish_Term"].ToString();

            if (Session["PishStartDate"].ToString() != "")
            {
                if (q.fldDateTime.Date >= MyLib.Shamsi.Shamsi2miladiDateTime(Session["PishStartDate"].ToString()) && q.fldDateTime.Date <= MyLib.Shamsi.Shamsi2miladiDateTime(Session["PishEndDate"].ToString()))
                {
                    return PartialView();
                }
                else
                {
                    Session["ER"] = "زمان پیش ثبت نام شما از تاریخ " + Session["PishStartDate"].ToString() + "تا تاریخ " + Session["PishEndDate"].ToString() +
                        "از ساعت " + Session["PishStartTime"].ToString() + "تا ساعت " + Session["PishEndTime"].ToString() + "می باشد.";
                    return RedirectToAction("error", "_Metro");
                }
            }
            else
            {
                Session["ER"] = "در حال حاضر پیش ثبت نام امکان پذیر نمی باشد.";
                return RedirectToAction("error", "_Metro");
            }
        }

        public ActionResult GetSemester()
        {
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var q = p.sp_B_tblTermsSelect("", "", 0).ToList().Select(c => new { fldID = c.fldId, fldName = c.fldTermName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities m = new Models.UniAutomationEntities();
            var q = m.sp_tblPishSabtenam_KhabgahSelect("fldFoodCartId", Session["PersonId"].ToString(), 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Save(Models.sp_tblPishSabtenam_KhabgahSelect Sarbarg)
        {
            try
            {
                Models.UniAutomationEntities p = new Models.UniAutomationEntities();
                if (Sarbarg.fldDesc == null)
                    Sarbarg.fldDesc = "";
                if (Sarbarg.fldId == 0)
                {//ثبت رکورد جدید
                    p.sp_tblPishSabtenam_KhabgahInsert(Convert.ToInt32(Session["Pish_Term"]), Convert.ToInt64(Session["PersonId"]), 1, Sarbarg.fldDesc);
                    return Json(new { data = "پیش ثبت نام با موفقیت انجام شد.", state = 0 });

                }
                else
                {//ویرایش رکورد ارسالی 
                    p.sp_tblPishSabtenam_KhabgahUpdate(Sarbarg.fldId, Convert.ToInt32(Session["Pish_Term"]), Convert.ToInt64(Session["PersonId"]), 1, Sarbarg.fldDesc);
                    return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
                }
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldTermId" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities m = new Models.UniAutomationEntities();
            var q = m.sp_tblPishSabtenam_KhabgahSelect(_fiald[Convert.ToInt32(field)], searchtext, top).Where(k => k.fldFoodCartId == Convert.ToInt64(Session["PersonId"])).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {

                Models.UniAutomationEntities Car = new Models.UniAutomationEntities();
                if (Convert.ToInt32(id) != 0)
                {
                    Car.sp_tblPishSabtenam_KhabgahDelete(Convert.ToByte(id), 1);
                    return Json(new { data = "انصراف با موفقیت انجام شد.", state = 0 });
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
       
        public ActionResult Details()
        {
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var q = p.sp_tblPishSabtenam_KhabgahSelect("fldFoodCartId", Session["PersonId"].ToString(), 0).Where(h => h.fldTermId == Convert.ToInt32(Session["Pish_Term"])).FirstOrDefault();
            int ID = 0;
            if (q != null)
                ID = q.fldId;
            return Json(new { ID = ID, }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult HavePay(int term)
        {
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var q = p.sp_tblDorm_PaySelect("fldFoodCartsID", Session["PersonId"].ToString(), 0).Where(h => h.fldTermsId == term).FirstOrDefault();
            bool Pay = true;
            if(q==null)
                Pay = false;
            return Json(new { Pay = Pay, }, JsonRequestBehavior.AllowGet);
        }

    }
}
