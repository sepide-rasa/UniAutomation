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
    public class L_BookStockController : Controller
    {  
        //
        // GET: /faces/L_BookStock/

        public ActionResult Index(int BookId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 206))
            {
                ViewBag.BookId = BookId;
                Session["BookId"] = BookId;
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
            var q = m.sp_tblL_BookStockSelect("fldBookId", Session["BookId"].ToString(), 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }


        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldBookTitle",  "fldBookId" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblL_BookStockSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditWin(int id)
        {
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 208))
            {
                ViewBag.id = id;
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                var q = m.sp_tblL_BookStockSelect("fldId", id.ToString(), 0).FirstOrDefault();
                ViewBag.fldBookId = q.fldBookId;
                ViewBag.fldNashr = q.fldNashr;
                ViewBag.fldActive_Deactive=q.fldActive_Deactive.ToString();
                ViewBag.fldRadeBandi_Kongere = q.fldRadeBandi_Kongere;
                ViewBag.fldTirazh = q.fldTirazh;
                ViewBag.fldNobateChap = q.fldNobateChap;
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult Save(Models.sp_tblL_BookStockSelect Book)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                if (Book.fldDesc == null)
                    Book.fldDesc = "";
                if (Book.fldNashr == null)
                    Book.fldNashr = "";
                if (Book.fldRadeBandi_Kongere == null)
                    Book.fldRadeBandi_Kongere = "";
                if (Book.fldTirazh == null)
                    Book.fldTirazh = 0;
                if (Book.fldNobateChap == null)
                    Book.fldNobateChap = 0;

                m.sp_tblL_BookStockUpdate(Book.fldId, Book.fldBookId, Book.fldNashr, Book.fldRadeBandi_Kongere, Book.fldTirazh, Book.fldNobateChap, Book.fldActive_Deactive, Convert.ToInt32(Session["UserId"]), "");
                    return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
                
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
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 209))
                {
                    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        m.sp_tblL_BookStockDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
