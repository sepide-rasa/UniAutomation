using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace UniAutomation.Areas.faces.Controllers
{
    public class PayOffController : Controller
    {
        //
        // GET: /faces/PayOff/

        public ActionResult Index()
        {
                return PartialView();
        }


        public JsonResult LoadCart(string rfid)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                
                string Error = "";
                if (rfid != "")
                {
                    var foodcart = m.sp_tblFoodCartsSelect("fldRFID", rfid, 0).FirstOrDefault();
                    if (foodcart != null)
                    {
                        var books = m.sp_tblL_BookTrustSelect("fldFoodCardId", foodcart.fldID.ToString(), 0).ToList();
                        
                        int id = Convert.ToInt32(foodcart.fldID);
                        var q = m.sp_GetCharge(id).FirstOrDefault();
                        var p = m.sp_PersonName(id).FirstOrDefault();

                        return Json(new
                        {
                            PersonName = p.PersonName,
                            foodcartID = id,
                            Charge=q.Charge,
                            Error = Error,
                            books = books
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

        public ActionResult Save(Models.sp_tblChargeSelect charge)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                if (charge.fldDesc == null)
                    charge.fldDesc = "";
               //ثبت رکورد جدید
                        p.sp_tblChargeInsert(charge.fldFoodCartsID, charge.fldPrice, charge.fldChargeType, charge.fldDesc,Convert.ToInt32(Session["UserId"]));
                        return Json(new { data = "تسویه حساب دانشجو انجام شد.", state = 0 });
                  
          
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public ActionResult ReloadBooks(string FoodCardId)
        {//جستجو
            
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();

            var q = m.sp_tblL_BookTrustSelect("fldFoodCardId", FoodCardId, 0).ToList();
                    
            return Json(q, JsonRequestBehavior.AllowGet);
        }
       

       

    }
}
