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
    public class B_tblPeriodDormitoryController : Controller
    {
        //
        // GET: /faces/B_tblPeriodDormitory/
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 156))
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
            var q = m.sp_B_tblPeriodDormitorySelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Save(Models.sp_B_tblPeriodDormitorySelect Period)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                if (Period.fldDesc == null)
                    Period.fldDesc = "";
                if (Period.fldPeriodCode == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 157))
                    {

                        // System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldId", typeof(int));
                        m.sp_B_tblPeriodDormitoryInsert(Period.fldPeriodName, Period.fldYear, MyLib.Shamsi.Shamsi2miladiDateTime(Period.fldStartDate), MyLib.Shamsi.Shamsi2miladiDateTime(Period.fldEndDate)
                            , Period.fldGuestFees, Period.fldTLPTST, Period.fldTLPTET, Convert.ToInt32(Session["UserId"]), Period.fldDesc);

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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 158))
                    {

                        m.sp_B_tblPeriodDormitoryUpdate(Period.fldPeriodCode, Period.fldPeriodName, Period.fldYear, MyLib.Shamsi.Shamsi2miladiDateTime(Period.fldStartDate), MyLib.Shamsi.Shamsi2miladiDateTime(Period.fldEndDate)
                            , Period.fldGuestFees, Period.fldTLPTST, Period.fldTLPTET, Convert.ToInt32(Session["UserId"]), Period.fldDesc);
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
            string[] _fiald = new string[] { "fldPeriodName", "fldYear"};
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblPeriodDormitorySelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 159))
                {
                    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        m.sp_B_tblPeriodDormitoryDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
                var q = m.sp_B_tblPeriodDormitorySelect("fldPeriodCode", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldPeriodCode = q.fldPeriodCode,
                    fldTLPTSTH = q.fldTLPTST.Hours,
                    fldTLPTSTM = q.fldTLPTST.Minutes,
                    fldTLPTETH = q.fldTLPTET.Hours,
                    fldTLPTETM = q.fldTLPTET.Minutes,
                    fldPeriodName = q.fldPeriodName,
                    fldEndDate = q.fldEndDate,
                    fldGuestFees = q.fldGuestFees,
                    fldYear = q.fldYear,
                    fldStartDate = q.fldStartDate,
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
