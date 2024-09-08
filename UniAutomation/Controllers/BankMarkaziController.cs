using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UniAutomation.Models;


namespace UniAutomation.Controllers
{
    public class BankMarkaziController : Controller
    {
        //
        // GET: /BankMarkazi/

        public ActionResult Index()
        {
            if (Session["PersonId"] == null)
                return RedirectToAction("logIn", "Accounts");
            return View();
        }
        public ActionResult Pay()
        {
            PaymentRequest request=new PaymentRequest();
           

                Models.UniAutomationEntities p = new Models.UniAutomationEntities();
                var q = p.sp_tblParametrHay_Dargah_PardakhtSelect("fldBankId", "4", "", 0).ToList();
                var id = 0;
                var id_TerminalId = 0;
                var id_TransactionKey = 0;
                var id_ReturnUrl = 0;
                var id_ShenasePardakht = 0;
                foreach (var item in q)
                {
                    if (item.fldEnglishName == "MerchantId")
                    {
                        id = item.fldId;
                    }
                    else if (item.fldEnglishName == "TerminalId")
                    {
                        id_TerminalId = item.fldId;
                    }
                    
                    else if (item.fldEnglishName == "TransactionKey")
                    {
                        id_TransactionKey = item.fldId;
                    }
                    else if (item.fldEnglishName == "ShenasePardakht")
                    {
                        id_ShenasePardakht = item.fldId;
                    }
                    else if (item.fldEnglishName == "ReturnUrl")
                    {
                        id_ReturnUrl = item.fldId;
                    }
                }

                var q1 = p.sp_tblDargah_Pardakht_InfoSelect("fldDargahPardakhtId", Session["Type"].ToString(), 0).Where(h => h.fldParametrId == id).FirstOrDefault();
                var q2 = p.sp_tblDargah_Pardakht_InfoSelect("fldDargahPardakhtId", Session["Type"].ToString(), 0).Where(h => h.fldParametrId == id_TerminalId).FirstOrDefault();
                var q3 = p.sp_tblDargah_Pardakht_InfoSelect("fldDargahPardakhtId", Session["Type"].ToString(), 0).Where(h => h.fldParametrId == id_TransactionKey).FirstOrDefault();
                var q4 = p.sp_tblDargah_Pardakht_InfoSelect("fldDargahPardakhtId", Session["Type"].ToString(), 0).Where(h => h.fldParametrId == id_ReturnUrl).FirstOrDefault();
                var q5 = p.sp_tblDargah_Pardakht_InfoSelect("fldDargahPardakhtId", Session["Type"].ToString(), 0).Where(h => h.fldParametrId == id_ShenasePardakht).FirstOrDefault();
                request.MerchantId = q1.fldMount;
                request.TerminalId = q2.fldMount;
                request.MerchantKey = q3.fldMount;//TransactionKey
                request.ReturnUrl = q4.fldMount;
                request.PaymentIdentity = q5.fldMount;

                request.Amount=Convert.ToInt64(Session["Amount"]);
                request.OrderId = (Session["Tax"]).ToString();
                request.PurchasePage = "https://sadad.shaparak.ir/VPG";
            
                var dataBytes = Encoding.UTF8.GetBytes(string.Format("{0};{1};{2}", request.TerminalId , request.OrderId, request.Amount));

                var symmetric = SymmetricAlgorithm.Create("TripleDes");
                symmetric.Mode = CipherMode.ECB;
                symmetric.Padding = PaddingMode.PKCS7;

                var encryptor = symmetric.CreateEncryptor(Convert.FromBase64String( request.MerchantKey), new byte[8]);

                request.SignData = Convert.ToBase64String(encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length));

                if (HttpContext.Request.Url != null)
                    request.ReturnUrl = string.Format("{0}://{1}{2}BankMarkazi/Verify", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));

                var ipgUri = string.Format("https://sadad.shaparak.ir/api/v0/PaymentByIdentity/PaymentRequest", request.PurchasePage);


                HttpCookie merchantTerminalKeyCookie = new HttpCookie("Data", JsonConvert.SerializeObject(request));
                Response.Cookies.Add(merchantTerminalKeyCookie);

                var data = new
                {
                    request.MerchantId,
                    request.TerminalId,
                    request.Amount,
                    request.OrderId,
                    LocalDateTime = DateTime.Now,
                    request.ReturnUrl,
                    request.SignData,
                    request.PaymentIdentity
                    //MultiplexingData = request.MultiplexingData
                };

                var res = CallApi<PayResultData>(ipgUri, data);
                res.Wait();

                if (res != null && res.Result != null)
                {
                    if (res.Result.ResCode == "0")
                    {

                        Response.Redirect(/*request.ReturnUrl*/string.Format("{0}/Purchase/Index?token={1}", request.PurchasePage, res.Result.Token));
                    }
                    ViewBag.Message = res.Result.Description;
                    return View(); ;
                }
            return View();
        }

        [HttpPost]
        public ActionResult Verify(PurchaseResult result)
        {
            return View(result);
        }

        [HttpPost]
        public ActionResult VerifyRequest(PurchaseResult result)
        {
            try
            {
                var cookie = Request.Cookies["Data"].Value;
                var model = JsonConvert.DeserializeObject<PaymentRequest>(cookie);

                var dataBytes = Encoding.UTF8.GetBytes(result.Token);

                var symmetric = SymmetricAlgorithm.Create("TripleDes");
                symmetric.Mode = CipherMode.ECB;
                symmetric.Padding = PaddingMode.PKCS7;

                var encryptor = symmetric.CreateEncryptor(Convert.FromBase64String(model.MerchantKey), new byte[8]);

                var signedData = Convert.ToBase64String(encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length));

                var data = new
                {
                    token = result.Token,
                    SignData = signedData
                };

                var ipgUri = string.Format("{0}/api/v0/Advice/Verify", model.PurchasePage);

                Models.UniAutomationEntities p = new Models.UniAutomationEntities();

                var res = CallApi<VerifyResultData>(ipgUri, data);
                if (res != null && res.Result != null)
                {
                    if (res.Result.ResCode == "0")
                    {
                        result.VerifyResultData = res.Result;
                        res.Result.Succeed = true;
                        ViewBag.Success = res.Result.Description;

                        p.sp_tblPardakhtOnlineStateUpdate(Session["Tax"].ToString(), true, res.Result.RetrivalRefNo);
                        var Dargah = p.sp_tblDargah_PardakhtSelect("fldId", Session["Type"].ToString(), "", "", 0).FirstOrDefault();
                        if (Convert.ToInt32(Session["Amount"]) > 0)
                        {
                            if (Dargah.fldDargahType == 1)
                                p.sp_tblChargeInsert(Convert.ToInt64(Session["PersonId"]), Convert.ToInt32(Session["Amount"]), 2, "", 1);
                            if (Dargah.fldDargahType == 5)
                                p.sp_tblDorm_PayInsert(Convert.ToInt64(Session["PersonId"]), Convert.ToInt32(Session["Amount"]), Convert.ToInt32(Session["Semester"]), 1, "");

                            Session["Amount"] = 0;
                        }
                        return View("AfterVerify", result);
                    }
                    else
                    {
                        p.sp_tblPardakhtOnlineStateUpdate(Session["Tax"].ToString(), false, "222");
                    }
                    ViewBag.Message = res.Result.Description;
                    return View("Verify");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.ToString();
            }

            return View("AfterVerify", result);
        }
        public ActionResult AfterVerify(PurchaseResult result)
        {
            return View(result);
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public static async Task<T> CallApi<T>(string apiUrl, object value)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                var w = client.PostAsJsonAsync(apiUrl, value);
                w.Wait();
                HttpResponseMessage response = w.Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsAsync<T>();
                    result.Wait();
                    return result.Result;
                }
                return default(T);
            }
        }
    }
}
