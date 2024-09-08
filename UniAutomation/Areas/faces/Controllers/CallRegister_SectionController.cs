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
    public class CallRegister_SectionController : Controller
    {
        //
        // GET: /faces/CallRegister_Section/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 148))
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
            var q =m.sp_B_tblCallRegister_SectionSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult GetStatusCodeId()
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblStatusSelect("", "", 0).ToList().Select(c => new { fldStatusCode = c.fldID, fldStatusCodeId = c.fldName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldCallCode", "fldSectionName", "fldStartDate", "fldEndDate" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblCallRegister_SectionSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_B_tblCallRegister_SectionSelect("fldCallCode", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {

                    fldCallCode = q.fldCallCode,
                    fldStartDate = q.fldStartDate,
                    fldEndDate = q.fldEndDate,
                    fldSectionCodeId = q.fldSectionCodeId,
                    fldSectionName = q.fldSectionName,
                    fldStartTimeH = q.fldStartTime.Hours,
                    fldStartTimeM = q.fldStartTime.Minutes,
                    fldEndTimeH = q.fldEndTime.Hours,
                    fldEndTimeM = q.fldEndTime.Minutes,
                    TermId = q.fldTermId,
                    TermName = q.fldTermName,
                    fldCallTermName=q.fldCallTermName,
                    fldCallTermId=q.fldCallTermId,
                    fldActive_Deactive = q.fldActive_Deactive,
                    fldStatusDormId = q.fldStatusDormId,
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
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 151))
                {
                    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        m.sp_B_tblCallRegister_SectionDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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

        public ActionResult Save(Models.sp_B_tblCallRegister_SectionSelect Term)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                if (Term.fldDesc == null)
                    Term.fldDesc = "";
                if (Term.fldCallCode == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 149))
                    {
                        // System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldId", typeof(int));
                        m.sp_B_tblCallRegister_SectionInsert(MyLib.Shamsi.Shamsi2miladiDateTime(Term.fldStartDate), MyLib.Shamsi.Shamsi2miladiDateTime(Term.fldEndDate), Term.fldStartTime, Term.fldEndTime, Term.fldSectionCodeId,Term.fldStatusDormId,Term.fldTermId,Term.fldCallTermId,Term.fldActive_Deactive, Convert.ToInt32(Session["UserId"]), Term.fldDesc);

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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 150))
                    {
                        m.sp_B_tblCallRegister_SectionUpdate(Term.fldCallCode, MyLib.Shamsi.Shamsi2miladiDateTime(Term.fldStartDate), MyLib.Shamsi.Shamsi2miladiDateTime(Term.fldEndDate), Term.fldStartTime, Term.fldEndTime, Term.fldSectionCodeId, Term.fldStatusDormId, Term.fldTermId, Term.fldCallTermId, Term.fldActive_Deactive, Convert.ToInt32(Session["UserId"]), Term.fldDesc);
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


    }
}
