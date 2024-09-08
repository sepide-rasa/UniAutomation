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
    public class B_tblTermsController : Controller
    {
        //
        // GET: /faces/B_tblTerms/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 201))
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
            var q = m.sp_B_tblTermsSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Save(Models.sp_B_tblTermsSelect Term)
        {
            try
            {
                System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldId", typeof(int));

                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                if (Term.fldDesc == null)
                    Term.fldDesc = "";
                if (Term.fldId == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 202))
                    {
                    p.sp_B_tblTermsInsert(_id, Term.fldTermName, MyLib.Shamsi.Shamsi2miladiDateTime(Term.fldStartDate), MyLib.Shamsi.Shamsi2miladiDateTime(Term.fldFinishDate), Term.fldActive_Deactive, Convert.ToInt32(Session["UserId"]), Term.fldDesc);
                    if (Term.fldActive_Deactive == true)
                        p.sp_B_tblTermsStatusUpdate(Convert.ToInt32(_id.Value));
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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 203))
                    {
                    p.sp_B_tblTermsUpdate(Term.fldId, Term.fldTermName, MyLib.Shamsi.Shamsi2miladiDateTime(Term.fldStartDate), MyLib.Shamsi.Shamsi2miladiDateTime(Term.fldFinishDate), Term.fldActive_Deactive, Convert.ToInt32(Session["UserId"]), Term.fldDesc);
                    if (Term.fldActive_Deactive == true)
                        p.sp_B_tblTermsStatusUpdate(Convert.ToInt32(Term.fldId));   
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
            string[] _fiald = new string[] { "fldTermName" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblTermsSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 204))
                {
                    Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_B_tblTermsDelete(Convert.ToByte(id), Convert.ToInt32(Session["UserId"]));
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
                var q = p.sp_B_tblTermsSelect("fldId", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldId = q.fldId,
                    fldTermName = q.fldTermName,
                    fldFinishDate=q.fldFinishDate,
                    fldStartDate=q.fldStartDate,
                    fldActive_Deactive=q.fldActive_Deactive,
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
