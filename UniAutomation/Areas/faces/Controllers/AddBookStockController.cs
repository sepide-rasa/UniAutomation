using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniAutomation.Areas.faces.Controllers.Acc;

namespace UniAutomation.Areas.faces.Controllers
{

    public class AddBookStockController : Controller
    {
        //
        // GET: /faces/AddBookStock/

        public ActionResult Index(int bookid, int Count)
        {
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 207))
            {
                ViewBag.bookid = bookid;
                ViewBag.Count = Count;
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult ReloadGride(int Count,int BookId)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var RadeBandi = "";
            List<Models.BookStock> groups = new List<Models.BookStock>();
            var q = m.sp_tblL_BookStockSelect("fldBookId", BookId.ToString(), 0).FirstOrDefault();
            if (q != null)
                RadeBandi = q.fldRadeBandi_Kongere;
            for (byte i = 0; i < Count; i++)
            {
                Models.BookStock S = new Models.BookStock();
                S.fldNashr = "";
                S.fldNobateChap = 0;
                S.fldRadeBandi_Kongere = RadeBandi;
                S.fldTirazh = 0;
                S.fldBookId = BookId; 
                groups.Add(S);
            }
            return Json(groups, JsonRequestBehavior.AllowGet);
            //}
        }
          
        public ActionResult Save(List<Models.BookStock> BookStock)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                
                foreach (var item in BookStock)
                {
                    if (item.fldNashr == null)
                        item.fldNashr = "";
                    if (item.fldId == 0)
                    {//ثبت رکورد جدید

                        m.sp_tblL_BookStockInsert(item.fldBookId, item.fldNashr, item.fldRadeBandi_Kongere, item.fldTirazh, item.fldNobateChap,item.fldActive_DeActive, Convert.ToInt32(Session["UserId"]), "");

                    }
                    else
                    {//ویرایش رکورد ارسالی

                        m.sp_tblL_BookStockUpdate(item.fldId, item.fldBookId, item.fldNashr, item.fldRadeBandi_Kongere, item.fldTirazh, item.fldNobateChap, item.fldActive_DeActive, Convert.ToInt32(Session["UserId"]), "");
                        

                    }
                    
                }
                return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
    }
}
