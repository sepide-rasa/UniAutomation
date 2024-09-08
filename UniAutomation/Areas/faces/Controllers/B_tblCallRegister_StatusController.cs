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
    public class B_tblCallRegister_StatusController : Controller
    {
        //
        // GET: /faces/B_tblCallRegister_Status/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 144))
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
            var q = m.sp_B_tblCallRegister_StatusSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }


        public ActionResult Save(Models.sp_B_tblCallRegister_StatusSelect Call)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                if (Call.fldDesc == null)
                    Call.fldDesc = "";
                if (Call.fldCallCode == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 145))
                    {
                        // System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldId", typeof(int));
                        m.sp_B_tblCallRegister_StatusInsert(MyLib.Shamsi.Shamsi2miladiDateTime(Call.fldStartDate), MyLib.Shamsi.Shamsi2miladiDateTime(Call.fldEndDate), Call.fldStartTime,

                            Call.fldEndTime, Call.fldStatusCodeId, Convert.ToInt32(Session["UserId"]), Call.fldDesc);

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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 146))
                    {
                        m.sp_B_tblCallRegister_StatusUpdate(Call.fldCallCode, MyLib.Shamsi.Shamsi2miladiDateTime(Call.fldStartDate), MyLib.Shamsi.Shamsi2miladiDateTime(Call.fldEndDate), Call.fldStartTime,

                           Call.fldEndTime, Call.fldStatusCodeId, Convert.ToInt32(Session["UserId"]), Call.fldDesc);
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
            string[] _fiald = new string[] { "fldStatusType", "fldStartDate", "fldEndDate" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblCallRegister_StatusSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 147))
                {
                    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        m.sp_B_tblCallRegister_StatusDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                var q = m.sp_B_tblCallRegister_StatusSelect("fldCallCode", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldCallCode = q.fldCallCode,
                    fldStartDate = q.fldStartDate,
                    fldEndDate = q.fldEndDate,
                    fldStartTimeH = q.fldStartTime.Hours,
                    fldStartTimeM = q.fldStartTime.Minutes,
                    fldEndTimeH = q.fldEndTime.Hours,
                    fldEndTimeM = q.fldEndTime.Minutes,
                    fldStatusCodeId = q.fldStatusCodeId,
                    fldStatusType = q.fldStatusType,
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
