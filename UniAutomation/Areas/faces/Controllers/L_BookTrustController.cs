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
    public class L_BookTrustController : Controller
    {
        //
        // GET: /faces/L_BookTrust/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 210))
            {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var System = m.sp_GetDate().FirstOrDefault();
            var SystemDate = System.fldDateTime;
            ViewBag.Tarikh = MyLib.Shamsi.Miladi2ShamsiString(SystemDate);
            var TaTarikh = MyLib.Shamsi.ShamsiAddDay(MyLib.Shamsi.Miladi2ShamsiString(SystemDate), 7);
            var q = m.sp_tblHolidaySelect("fldTarikh", TaTarikh, 0).FirstOrDefault();
            while (q != null)
            {
                TaTarikh = MyLib.Shamsi.ShamsiAddDay(TaTarikh, 1);
                q = m.sp_tblHolidaySelect("fldTarikh", TaTarikh, 0).FirstOrDefault();
            }

            ViewBag.TaTarikh = TaTarikh;
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
            var q = m.sp_tblL_BookTrustSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public JsonResult LoadCart(string rfid, string TaTarikh)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                var Day = 0;
                string Error = "";
                if (rfid != "")
                {
                    var foodcart = m.sp_tblFoodCartsSelect("fldRFID", rfid, 0).FirstOrDefault();
                    if (foodcart != null)
                    {
                        var q = m.sp_tblL_BookTrustSelect("fldFoodCardId", foodcart.fldID.ToString(), 30).ToList();

                        foreach (var item in q)
                        {
                            if (item.fldStateBook == false)
                                Day++;
                        }
                        if (Day >= 2)
                            Error = "بیشتر از دو کتاب نمی توان امانت گرفت";

                        int id = Convert.ToInt32(foodcart.fldID);
                        var p = m.sp_PersonName(id).FirstOrDefault();
                        if (foodcart.fldStudentID == null)
                        {
                            TaTarikh = MyLib.Shamsi.ShamsiAddDay(TaTarikh, 7);
                            Error = "";
                        }
                        return Json(new
                        {
                            TaTarikh=TaTarikh,
                            PersonName = p.PersonName,
                            foodcartID = id,
                            Error = Error
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            PersonName = "کارتی برای شما صادر نشده است.",
                            foodcartID = 0
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

                else
                {
                    return Json(new
                    {
                        PersonName = "کارت شما فعال نشده است.",
                        foodcartID = 0
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult LoadBook(string bookid)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                var Book = m.sp_tblL_BookStockSelect("fldId", bookid, 0).FirstOrDefault();
                if (Book != null)
                {
                    var trust = m.sp_tblL_BookTrustSelect("fldBookId", bookid, 0).FirstOrDefault();
                    if (trust == null)
                    {
                        return Json(new
                        {
                            BookName = Book.fldBookTitle,
                            BookID = Book.fldId
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (trust.fldStateBook == true)
                            return Json(new
                            {
                                BookName = Book.fldBookTitle,
                                BookID = Book.fldId
                            }, JsonRequestBehavior.AllowGet);
                        else
                            return Json(new
                            {
                                BookName = "کتابی مورد نظر قبلا امانت گرفته شده."
                            }, JsonRequestBehavior.AllowGet);
                    }
                    
                }
                else
                {
                    return Json(new
                    {
                        BookName = "کتابی با این شماره ثبت نشده است."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Save(Models.sp_tblL_BookTrustSelect Trust)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                if (Trust.fldDesc == null)
                    Trust.fldDesc = "";
                if (Trust.fldId == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 211))
                    {
                    // System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldId", typeof(int));
                    m.sp_tblL_BookTrustInsert(Trust.fldBookId, MyLib.Shamsi.Shamsi2miladiDateTime(Trust.fldStartDate), MyLib.Shamsi.Shamsi2miladiDateTime(Trust.fldEndDate), Trust.fldFoodCardId, Trust.fldStateBook, Convert.ToInt32(Session["UserId"]), Trust.fldDesc);

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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 212))
                    {
                    m.sp_tblL_BookTrustUpdate(Trust.fldId, Trust.fldBookId, MyLib.Shamsi.Shamsi2miladiDateTime(Trust.fldStartDate), MyLib.Shamsi.Shamsi2miladiDateTime(Trust.fldEndDate), Trust.fldFoodCardId, Trust.fldStateBook, Convert.ToInt32(Session["UserId"]), Trust.fldDesc);
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
            string[] _fiald = new string[] { "fldBookTitle", "fldFoodCardId", "fldBookStateCaption" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblL_BookTrustSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 213))
                {
                    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        m.sp_tblL_BookTrustDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
                var q = m.sp_tblL_BookTrustSelect("fldId", id.ToString(), 1).FirstOrDefault();
                var p = m.sp_PersonName(Convert.ToInt32(q.fldFoodCardId)).FirstOrDefault();
                var BookStock = m.sp_tblL_BookStockSelect("fldId", q.fldBookId.ToString(), 0).FirstOrDefault();
                var Book = m.sp_tblL_BookSelect("fldId", BookStock.fldBookId.ToString(), 0).FirstOrDefault();
                var foodcart = m.sp_tblFoodCartsSelect("fldId", q.fldFoodCardId.ToString(), 0).FirstOrDefault();
                return Json(new
                {
                    fldId = q.fldId,
                    Barcode = Book.fldBarCode,
                    RFID = foodcart.fldRFID,
                    fldFoodCardId = q.fldFoodCardId,
                    PrsonName = p.PersonName,
                    fldBookId = q.fldBookId,
                    fldStartDate = q.fldStartDate,
                    fldEndDate = q.fldEndDate,
                    fldBookTitle = q.fldBookTitle,
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
