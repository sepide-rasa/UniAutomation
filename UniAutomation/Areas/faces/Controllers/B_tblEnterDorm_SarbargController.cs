﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using UniAutomation.Areas.faces.Controllers.Acc;


namespace UniAutomation.Areas.faces.Controllers
{
    public class B_tblEnterDorm_SarbargController : Controller
    {
        //
        // GET: /faces/B_tblEnterDorm_Sarbarg/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 188))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }

        }

        public ActionResult GetSemester()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_B_tblTermsSelect("", "", 0).ToList().Select(c => new { fldID = c.fldId, fldName = c.fldTermName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblEnterDorm_SarbargSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Save(Models.sp_B_tblEnterDorm_SarbargSelect Sarbarg)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                if (Sarbarg.fldDesc == null)
                    Sarbarg.fldDesc = "";
                if (Sarbarg.fldId == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 189))
                    {
                        p.sp_B_tblEnterDorm_SarbargInsert(MyLib.Shamsi.Shamsi2miladiDateTime(Sarbarg.fldTarikh), Sarbarg.fldSemester, Convert.ToInt32(Session["UserId"]), Sarbarg.fldDesc);
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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 190))
                    {
                        p.sp_B_tblEnterDorm_SarbargUpdate(Sarbarg.fldId, MyLib.Shamsi.Shamsi2miladiDateTime(Sarbarg.fldTarikh), Sarbarg.fldSemester, Convert.ToInt32(Session["UserId"]), Sarbarg.fldDesc);
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
            string[] _fiald = new string[] { "fldSemester", "fldTarikh" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblEnterDorm_SarbargSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 191))
                {

                    Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_B_tblEnterDorm_SarbargDelete(Convert.ToByte(id), Convert.ToInt32(Session["UserId"]));
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
                var q = p.sp_B_tblEnterDorm_SarbargSelect("fldId", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldID = q.fldId,
                    fldTarikh=q.fldTarikh,
                    fldSemester=q.fldSemester,
                    fldUserId=q.fldUserId,
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