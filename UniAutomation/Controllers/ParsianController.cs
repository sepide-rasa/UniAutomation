using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniAutomation.Controllers
{
    public class ParsianController : Controller
    {
        //
        // GET: /Parsian/

        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult Pay()
        {
            Parsian.EShopService e = new Parsian.EShopService();
            long authority = 0;
            byte status = 0;
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var q = p.sp_tblParametrHay_Dargah_PardakhtSelect("fldBankId", "3","", 0).ToList();
            var id = 0;
            var id_url = 0;
            foreach (var item in q)
            {
                if (item.fldEnglishName == "PIN")
                {
                    id = item.fldId;
                }
                else if (item.fldEnglishName == "BackUrl")
                {
                    id_url = item.fldId;
                }
            }
            var q1 = p.sp_tblDargah_Pardakht_InfoSelect("fldDargahPardakhtId", Session["Type"].ToString(), 0).Where(h => h.fldParametrId == id).FirstOrDefault();
            var q2 = p.sp_tblDargah_Pardakht_InfoSelect("fldDargahPardakhtId", Session["Type"].ToString(), 0).Where(h => h.fldParametrId == id_url).FirstOrDefault();
            var url = "http://" + q2.fldMount + "/Parsian/Back";
            Session["PIN"] = q1.fldMount;
            e.PinPaymentRequest(Session["PIN"].ToString(), Convert.ToInt32(Session["Amount"]), Convert.ToInt32(Session["Tax"]), url, ref authority, ref status);
            if (status == 0)
            {
                Session["authority"] = authority;
                return Redirect("https://pec.shaparak.ir/pecpaymentgateway/default.aspx?au=" + authority);
            }
            ViewBag.status = status;
            return View();
        }
        public ActionResult Back(int rs, long au)
        {
            Parsian.EShopService e = new Parsian.EShopService();
            long InvoiceNamber = 0;
            byte status = 0;
            if (rs == 0 && Session["authority"].ToString() == au.ToString())
            {
                e.PaymentEnquiry(Session["PIN"].ToString(), Convert.ToInt64(Session["authority"]), ref status, ref InvoiceNamber);
                if (status == 0 && InvoiceNamber != -1)
                {
                    Models.UniAutomationEntities p = new Models.UniAutomationEntities();
                    string ResNum = Session["Tax"].ToString();
                    
                    p.sp_tblPardakhtOnlineStateUpdate(ResNum, true, InvoiceNamber.ToString());
                    var Dargah = p.sp_tblDargah_PardakhtSelect("fldId", Session["Type"].ToString(), "","", 0).FirstOrDefault();
                    if (Dargah.fldDargahType == 1)
                        p.sp_tblChargeInsert(Convert.ToInt64(Session["PersonId"]), Convert.ToInt32(Session["Amount"]), Convert.ToByte(Session["Type"]), "", 1);
                    if (Dargah.fldDargahType == 5)
                        p.sp_tblDorm_PayInsert(Convert.ToInt64(Session["PersonId"]), Convert.ToInt32(Session["Amount"]), Convert.ToInt32(Session["Semester"]), 1, "");

                    Session["referenceId"] = InvoiceNamber;
                    
                    ViewBag.Result = "پرداخت شما با موفقیت انجام شد.";
                }
                else
                    ViewBag.Result = "نا موفق";
            }
            else
            {
                ViewBag.Result = "نا موفق";
            }

            return View();
        }

    }
}
