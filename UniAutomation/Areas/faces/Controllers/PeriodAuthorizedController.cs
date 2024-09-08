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
    public class PeriodAuthorizedController : Controller
    {
        //
        // GET: /faces/PeriodAuthorized/

        public ActionResult Index()
        {
             if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 116))
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
            var q = m.sp_B_tblPeriodAuthorizedSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldSectionName" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblPeriodAuthorizedSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_B_tblPeriodAuthorizedSelect("fldPACode", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {

                    fldPACode = q.fldPACode,
                    fldSectionCodeId = q.fldSectionCodeId,
                    fldSectionName = q.fldSectionName,
                    fldMaxSemester = q.fldMaxSemester,
                    fldDesc = q.fldDesc
                }, JsonRequestBehavior.AllowGet);
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
                 if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 119))
                {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                if (Convert.ToInt32(id) != 0)
                {
                    m.sp_B_tblPeriodAuthorizedDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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

        public ActionResult Save(Models.sp_B_tblPeriodAuthorizedSelect Period)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                if (Period.fldDesc == null)
                    Period.fldDesc = "";
                if (Period.fldPACode == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 117))
                    {
                        // System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldId", typeof(int));
                        m.sp_B_tblPeriodAuthorizedInsert(Period.fldSectionCodeId, Period.fldMaxSemester, Convert.ToInt32(Session["UserId"]), Period.fldDesc);

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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 118))
                    {
                        m.sp_B_tblPeriodAuthorizedUpdate(Period.fldPACode, Period.fldSectionCodeId, Period.fldMaxSemester, Convert.ToInt32(Session["UserId"]), Period.fldDesc);
                        return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });

                    }
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
    }
}
