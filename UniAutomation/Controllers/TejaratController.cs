using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniAutomation.Controllers
{
    public class TejaratController : Controller
    {
        //public static string merchantId = "A389";
        // GET: /Tejarat/Index
        public ActionResult Index()
        {
            //Session["Amount"] = 10000;
            //Session["Tax"] = 154658;
            return PartialView();
        }
        // GET: /Tejarat/Back
        [HttpPost]
        public ActionResult Back(string paymentId, string referenceId, string resultCode)
        {
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var q = p.sp_tblParametrHay_Dargah_PardakhtSelect("fldBankId", "2","", 0).ToList();
            var id = 0;
            foreach (var item in q)
            {
                if (item.fldEnglishName == "merchantId")
                {
                    id = item.fldId;
                }
            }
            var q1 = p.sp_tblDargah_Pardakht_InfoSelect("fldDargahPardakhtId", Session["Type"].ToString(), 0).Where(h => h.fldParametrId == id).FirstOrDefault();
            var MerchantId = q1.fldMount;

            Session["referenceId"] = referenceId;
            p.sp_tblPardakhtOnlineStateUpdate(paymentId, false, referenceId);
            if (resultCode == "100")
            {

                WsTejarat.verifyRequest Verify = new WsTejarat.verifyRequest();
                Verify.merchantId = MerchantId;
                Verify.referenceNumber = referenceId;
                WsTejarat.MerchantService Client = new WsTejarat.MerchantService();
                try
                {
                    long result = Client.verify(Verify);
                    if (result > 0)
                    {                        
                        p.sp_tblPardakhtOnlineStateUpdate(paymentId, true, referenceId);
                        var Dargah = p.sp_tblDargah_PardakhtSelect("fldId", Session["Type"].ToString(), "","", 0).FirstOrDefault();
                        if (Dargah.fldDargahType == 1)
                            p.sp_tblChargeInsert(Convert.ToInt64(Session["PersonId"]), Convert.ToInt32(Session["Amount"]), Convert.ToByte(Session["Type"]), "", 1);
                        if (Dargah.fldDargahType == 5)
                            p.sp_tblDorm_PayInsert(Convert.ToInt64(Session["PersonId"]), Convert.ToInt32(Session["Amount"]), Convert.ToInt32(Session["Semester"]), 1, "");
               
                        ViewBag.Result = "تراکنش با موفقیت انجام شد";
                    }
                    else
                        switch (result)
                        {
                            case -20:
                                ViewBag.Result = "وجود کاراکترهای غیر مجاز در درخواست";
                                break;
                            case -30:
                                ViewBag.Result = "تراکنش قبلا برگشت خورده است";
                                break;
                            case -50:
                                ViewBag.Result = "طول رشته درخواست غیر مجاز است";
                                break;
                            case -51:
                                ViewBag.Result = "خطا در درخواست";
                                break;
                            case -80:
                                ViewBag.Result = "تراکنش مورد نظر یافت نشد";
                                break;
                            case -81:
                                ViewBag.Result = "خطای داخلی بانک";
                                break;
                            case -90:
                                ViewBag.Result = "تراکنش قبلا تایید شده است";
                                break;

                        }
                }
                catch (Exception E)
                {
                    ViewBag.Result = E.Message;
                }
            }
            else
                switch (resultCode)
                {
                    case "110":
                        ViewBag.Result = "انصراف توسط دارنده کارت";
                        break;
                    case "120":
                        ViewBag.Result = "موجودی حساب کافی نیست";
                        break;
                    case "130":
                        ViewBag.Result = "اطلاعات کارت اشتباه است";
                        break;
                    case "131":
                        ViewBag.Result = "رمز کارت اشتباه است";
                        break;
                    case "132":
                        ViewBag.Result = "کارت مسدود شده است";
                        break;
                    case "133":
                        ViewBag.Result = "کارت منقضی شده است";
                        break;
                    case "140":
                        ViewBag.Result = "زمان مورد نظر به پایان رسیده است";
                        break;
                    case "150":
                        ViewBag.Result = "خطای داخلی بانک";
                        break;
                    case "160":
                        ViewBag.Result = "خطا در اطلاعات cvv2 یا ExpDate";
                        break;
                    case "166":
                        ViewBag.Result = "بانک صادرکننده کارت مجوز انجام تراکنش را صادر نکرده است";
                        break;
                    case "200":
                        ViewBag.Result = "مبلغ تراکنش بیشتر از سقف مجاز هر تراکنش می باشد";
                        break;
                    case "201":
                        ViewBag.Result = "مبلغ تراکنش بیشتر از سقف مجاز در روز می باشد";
                        break;
                    case "202":
                        ViewBag.Result = "مبلغ تراکنش بیشتر از سقف مجاز در ماه می باشد";
                        break;
                }
            return View();
        }
        
        
    }
}
