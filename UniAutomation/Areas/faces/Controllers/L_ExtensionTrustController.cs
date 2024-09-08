using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
namespace UniAutomation.Areas.faces.Controllers
{
    public class L_ExtensionTrustController : Controller
    {
        //
        // GET: /faces/L_ExtensionTrust/

        public ActionResult Index(int id,int Id )
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblL_BookTrustSelect("fldId", id.ToString(), 1).FirstOrDefault();
            var Tarikh = q.fldTrustDate;
            ViewBag.personid = id;
            ViewBag.EndDate = q.fldTrustDate;
            ViewBag.BookTrustId = Id;
            var TamdidDate = MyLib.Shamsi.ShamsiAddDay(Tarikh, 7);
            ViewBag.TamdidDate = TamdidDate;
            Session["fldBookTrustId"] = Id;
            return PartialView();
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblL_ExtensionTrustSelect("fldBookTrustId", Session["fldBookTrustId"].ToString(), 30).ToList().ToDataSourceResult(request);
            Session.Remove("fldBookTrustId");
            return Json(q);
        }
        public ActionResult Save(Models.sp_tblL_ExtensionTrustSelect Trust)
        {

            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                if (Trust.fldDesc == null)
                    Trust.fldDesc = "";
                if (Trust.fldId == 0)
                {//ثبت رکورد جدید

                    // System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldId", typeof(int));
                    m.sp_tblL_ExtensionTrustInsert(Trust.fldBookTrustId, MyLib.Shamsi.Shamsi2miladiDateTime(Trust.fldEndDate), Convert.ToInt32(Session["UserId"]), Trust.fldDesc);

                    return Json(new { data = "تمدید کتاب انجام شد.", state = 0 });

                }
                else
                {//ویرایش رکورد ارسالی

                    m.sp_tblL_ExtensionTrustUpdate(Trust.fldId, Trust.fldBookTrustId, MyLib.Shamsi.Shamsi2miladiDateTime(Trust.fldEndDate), Convert.ToInt32(Session["UserId"]), Trust.fldDesc);
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
            string[] _fiald = new string[] { "fldEndDate" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblL_ExtensionTrustSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
    }
}
