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
    public class L_BookController : Controller
    {
        //
        // GET: /faces/L_Book/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 184))
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
            var q = m.sp_tblL_BookSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }
          
        public ActionResult GetLibraryId()
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblL_LibrarySelect("", "", 0).ToList().Select(c => new { fldId = c.fldId, fldLibraryId = c.fldName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCategoryId()
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblL_BookCategorySelect("", "", 0).ToList().Select(c => new { fldId = c.fldId, fldCategoryId = c.fldName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(Models.sp_tblL_BookSelect Book)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                if (Book.fldDesc == null)
                    Book.fldDesc = "";
                if (Book.fldId == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 185))
                    {
                        // System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldId", typeof(int));
                        m.sp_tblL_BookInsert(Book.fldLibraryId, Book.fldIsbn, Book.fldInterpreter, Book.fldBookTitle, Book.fldPublisher, Book.fldAuthor, Book.fldPersianName
                            , Book.fldEnglishName, Book.fldPublicationYear, Book.fldCategoryId, Book.fldBarCode, Convert.ToInt32(Session["UserId"]), Book.fldDesc);

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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 186))
                    {

                        m.sp_tblL_BookUpdate(Book.fldId, Book.fldLibraryId, Book.fldIsbn, Book.fldInterpreter, Book.fldBookTitle, Book.fldPublisher, Book.fldAuthor, Book.fldPersianName
                            , Book.fldEnglishName, Book.fldPublicationYear, Book.fldCategoryId, Book.fldBarCode, Convert.ToInt32(Session["UserId"]), Book.fldDesc);
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
            string[] _fiald = new string[] { "fldLibraryId", "fldBookTitle", "fldPersianName", "fldEnglishName" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblL_BookSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 187))
                {
                    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        var S = m.sp_tblL_BookStockSelect("fldBookId", id, 0).ToList();
                        foreach (var Item in S) { 
                        var q = m.sp_tblL_BookTrustSelect("fldbookId", Item.fldId.ToString(), 0).Where(k=>k.fldStateBook==false).FirstOrDefault();
                        if (q != null)
                            return Json(new { data = "این کتاب دارای تاریخچه امانت می باشد و قابل حذف نیست.", state = 1 });
                        }
                        
                            m.sp_tblL_BookStockfldBookIdDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
                            m.sp_tblL_BookDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
                var q = m.sp_tblL_BookSelect("fldId", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldId = q.fldId,
                    fldLibraryId = q.fldLibraryId,
                    fldCategoryId = q.fldCategoryId,  
                    fldBookTitle = q.fldBookTitle,
                    fldAuthor = q.fldAuthor,
                    fldInterpreter = q.fldInterpreter,
                    fldPublisher = q.fldPublisher,
                    fldPersianName = q.fldPersianName,
                    fldEnglishName = q.fldEnglishName,
                    fldIsbn = q.fldIsbn,
                    fldPublicationYear = q.fldPublicationYear,
                    fldBarCode = q.fldBarCode,
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
