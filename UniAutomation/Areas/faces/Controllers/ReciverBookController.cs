using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace UniAutomation.Areas.faces.Controllers
{
    public class ReciverBookController : Controller
    {
        //
        // GET: /faces/ReciverBook/

        public ActionResult Index()
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var System = m.sp_GetDate().FirstOrDefault();
            var SystemDate = System.fldDateTime;
            ViewBag.TarikhTahvil = MyLib.Shamsi.Miladi2ShamsiString(SystemDate);

            return PartialView();
        }

        
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblL_BookTrustSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }
        public ActionResult Reciver(int BookTrusrId, string TarikhTahvil)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                m.sp_L_tblBookTrustStateUpdate(Convert.ToInt32(BookTrusrId), true, MyLib.Shamsi.Shamsi2miladiDateTime(TarikhTahvil));
                return Json(new { data = "تحویل کتاب با موفقیت انجام شد.", state = 0 });
            }
                
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }

        public ActionResult Trust(int BookTrusrId)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                var q = m.sp_tblL_ExtensionTrustSelect("fldBookTrustId", BookTrusrId.ToString(), 1).FirstOrDefault();
               string Error = "";

               if (q != null)
               {
                   Error = "هر کتاب را بیش از یکبار نمی توان تمدید کرد.";
               }

                return Json(new
                {
                    Error = Error
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }


        public JsonResult LoadCart(string rfid)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                var BookStock = m.sp_tblL_BookStockSelect("fldId", rfid, 0).FirstOrDefault();
                if (BookStock != null)
                {
                    int id = Convert.ToInt32(BookStock.fldId);
                    var p = m.sp_tblL_BookTrustSelect("fldBookId", BookStock.fldId.ToString(), 0).Where(l=>l.fldStateBook==false).FirstOrDefault();
                   var day=m.sp_GetDate().FirstOrDefault().fldDateTime;
                    if (p != null)
                    {
                        return Json(new
                        {
                            BookTrustId = p.fldId,
                            BookStockID = id,
                            BookTitle = BookStock.fldBookTitle,
                            DiffDay = MyLib.Shamsi.DiffOfShamsiDate(p.fldTrustDate,MyLib.Shamsi.Miladi2ShamsiString(day)),
                            PersonName = p.fldPrsonName,
                            Err = ""
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            BookTrustId = 0,
                            BookStockID = id,
                            BookTitle = BookStock.fldBookTitle,
                            PersonName = "",
                            DiffDay="",
                            Err="کتاب موردنظر در کتابخانه موجود می باشد."
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        Err = "کد وارد شده نادرست است.",
                            BookTrustId = 0,
                            BookStockID = 0,
                        BookTitle = "",
                        PersonName = "",
                            DiffDay=""
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldBookTitle", "fldFoodCardId", "fldStateBook" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblL_BookTrustSelect(_fiald[Convert.ToInt32(field)], searchtext, top).Where(k => k.fldStateBook == false).ToList();
            if (field!="1")
                q = m.sp_tblL_BookTrustSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

      
        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                var q = m.sp_tblL_BookTrustSelect("fldId", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldId = q.fldId,
                    fldBookId = q.fldBookId,
                    fldEndDate = q.fldEndDate,
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
