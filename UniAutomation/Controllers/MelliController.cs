using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace UniAutomation.Controllers
{
    public class MelliController : Controller
    {
        //
        // GET: /Melli/

        public Melli.MerchantUtility Bmi = new Melli.MerchantUtility();
        public ActionResult Index()
        {
            if (Session["PersonId"] == null)
                return RedirectToAction("logIn", "Accounts");
            Session["TimeStamp"] = ViewBag.TimeStamp = TimeStamp();
            Session["FP"] = ViewBag.FP = FP(Convert.ToInt64(Session["Amount"]),
                Convert.ToInt64(Session["Tax"]), ViewBag.TimeStamp);
            Session["Request"] = ViewBag.Request = RequestId(
                Convert.ToInt64(Session["Tax"]), ViewBag.FP, ViewBag.TimeStamp) ;
            return PartialView();
        }
        public string TimeStamp() { return Bmi.CalcTimeStamp(); }
        public string FP(long Amount, long TaxId, string TimeStamp)
        {
            Models.UniAutomationEntities m = new Models.UniAutomationEntities();
            var q = m.sp_tblParametrHay_Dargah_PardakhtSelect("fldBankId", "1","", 0).ToList();
            var id = 0;
            var id_TransactionKey = 0;
            foreach (var item in q)
            {
                if (item.fldEnglishName == "MerchantId")
                {
                    id = item.fldId;
                }
                else if (item.fldEnglishName == "TransactionKey")
                {
                    id_TransactionKey = item.fldId;
                }
            }
            var q2 = m.sp_tblDargah_Pardakht_InfoSelect("fldDargahPardakhtId", Session["Type"].ToString(), 0).Where(h => h.fldParametrId == id).FirstOrDefault();
            var q3 = m.sp_tblDargah_Pardakht_InfoSelect("fldDargahPardakhtId", Session["Type"].ToString(), 0).Where(h => h.fldParametrId == id_TransactionKey).FirstOrDefault();
            var MerchantId = q2.fldMount;
            var TransactionKey = q3.fldMount;

            //var MerchantId = "116937244";
            //var TransactionKey = "26QNA3418W";
            Bmi.Url = "https://sadad.shaparak.ir/services/MerchantUtility.asmx";

            string textInput = string.Concat(MerchantId, TaxId.ToString(), Amount.ToString(), TransactionKey, TimeStamp);
            MD5 hash = new MD5CryptoServiceProvider();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] Input = encoding.GetBytes(textInput);
            byte[] result = hash.ComputeHash(Input);
            return BitConverter.ToString(result);
        }
        public string RequestId(long TaxId, string FP, string TimeStamp)
        {
            Models.UniAutomationEntities m = new Models.UniAutomationEntities();
            var q = m.sp_tblParametrHay_Dargah_PardakhtSelect("fldBankId", "1","", 0).ToList();
            var id = 0;
            var id_TransactionKey = 0;
            foreach (var item in q)
            {
                if (item.fldEnglishName == "MerchantId")
                {
                    id = item.fldId;
                }
                else if (item.fldEnglishName == "TransactionKey")
                {
                    id_TransactionKey = item.fldId;
                }
            }
            var q2 = m.sp_tblDargah_Pardakht_InfoSelect("fldDargahPardakhtId", Session["Type"].ToString(), 0).Where(h => h.fldParametrId == id).FirstOrDefault();
            var q3 = m.sp_tblDargah_Pardakht_InfoSelect("fldDargahPardakhtId", Session["Type"].ToString(), 0).Where(h => h.fldParametrId == id_TransactionKey).FirstOrDefault();
            var MerchantId = q2.fldMount;
            var TransactionKey = q3.fldMount;
            //var MerchantId = "116937244";
            //var TransactionKey = "26QNA3418W";
            Bmi.Url = "https://sadad.shaparak.ir/services/MerchantUtility.asmx";
            string textInput = string.Concat(MerchantId, TaxId.ToString(), FP, TransactionKey);
            MD5 hash = new MD5CryptoServiceProvider();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] Input = encoding.GetBytes(textInput);
            byte[] result = hash.ComputeHash(Input);
            string Rq = TimeStamp + BitConverter.ToString(result);
            return Rq.Replace("-", "").ToLower();
        }
        public ActionResult Back()
        {
            if (Session["PersonId"] == null)
                return RedirectToAction("logIn", "Accounts");
            bool R = false;
            Session.Remove("ResidId");

            Models.UniAutomationEntities m = new Models.UniAutomationEntities();
            var q = m.sp_tblParametrHay_Dargah_PardakhtSelect("fldBankId", "1","", 0).ToList();
            var id = 0;
            var id_TransactionKey = 0;
            var id_TerminalId = 0;
            foreach (var item in q)
            {
                if (item.fldEnglishName == "MerchantId")
                {
                    id = item.fldId;
                }
                else if (item.fldEnglishName == "TransactionKey")
                {
                    id_TransactionKey = item.fldId;
                }
                else if (item.fldEnglishName == "TerminalId")
                {
                    id_TerminalId = item.fldId;
                }
            }
            var q1 = m.sp_tblDargah_Pardakht_InfoSelect("fldDargahPardakhtId", Session["Type"].ToString(), 0).Where(h => h.fldParametrId == id_TerminalId).FirstOrDefault();
            var q2 = m.sp_tblDargah_Pardakht_InfoSelect("fldDargahPardakhtId", Session["Type"].ToString(), 0).Where(h => h.fldParametrId == id).FirstOrDefault();
            var q3 = m.sp_tblDargah_Pardakht_InfoSelect("fldDargahPardakhtId", Session["Type"].ToString(), 0).Where(h => h.fldParametrId == id_TransactionKey).FirstOrDefault();
            var MerchantId = q2.fldMount;
            var TransactionKey = q3.fldMount;
            var TerminalId = q1.fldMount;

            //var MerchantId = "116937244";
            //var TransactionKey = "26QNA3418W";
            Bmi.Url = "https://sadad.shaparak.ir/services/MerchantUtility.asmx";
            //var TerminalId = "17993349";
            string RefNum = "", Status = "";
            Bmi.CheckRequestStatus(
                    Convert.ToInt64(Session["Tax"]),
                    MerchantId, TerminalId, TransactionKey, Session["Request"].ToString(),
                    Convert.ToInt64(Session["Amount"]), out RefNum, out Status);
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            if (Status == "COMMIT")
            {
                Session["referenceId"] = RefNum;
                p.sp_tblPardakhtOnlineStateUpdate(Session["Tax"].ToString(), true, RefNum);
                var Dargah = p.sp_tblDargah_PardakhtSelect("fldId", Session["Type"].ToString(), "","", 0).FirstOrDefault();
                if (Dargah.fldDargahType == 1)
                    p.sp_tblChargeInsert(Convert.ToInt64(Session["PersonId"]), Convert.ToInt32(Session["Amount"]), 2, "", 1);
                if (Dargah.fldDargahType == 5)
                    p.sp_tblDorm_PayInsert(Convert.ToInt64(Session["PersonId"]), Convert.ToInt32(Session["Amount"]), Convert.ToInt32(Session["Semester"]),1, "");
               
                R = true;
            }
            else
            {
                p.sp_tblPardakhtOnlineStateUpdate(Session["Tax"].ToString(), false, "");
            }
            
            if (R)
                ViewBag.Result = "پرداخت شما با موفقیت انجام شد.";
            else
                ViewBag.Result = "ناموفق";
            Session.Remove("Amount");
            Session.Remove("Semester");
            Session.Remove("Type");
            Session.Remove("Request");
            Session.Remove("TimeStamp");
            Session.Remove("Tax");
            Session.Remove("FP");
            return View();
        }        

    }
}
