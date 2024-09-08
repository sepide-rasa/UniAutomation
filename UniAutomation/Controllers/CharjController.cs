using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace UniAutomation.Controllers
{
    public class CharjController : Controller
    {
        //
        // GET: /Charj/

        public ActionResult Index()
        {
            if (Session["PersonId"] == null)
                return RedirectToAction("logIn", "Accounts");
            return PartialView();
        }

        public ActionResult GoToPay(int Amount, byte Type, int? Semester)
        {
            //type=1 => تغذیه
            //type=2 => شهریه
            //type=3 => خوابگاه
            if (Session["PersonId"] == null)
                return RedirectToAction("logIn", "Accounts");
            var Ip = WebConfigurationManager.AppSettings["UniIp"];
            if (Request.ServerVariables["SERVER_NAME"].ToString() != Ip)
                return Json(" استفاده بفرمایید" + Ip +
                    " نمی باشید لطفا از طریق آدرس " +
                    "http://" + Request.ServerVariables["SERVER_NAME"].ToString() +
                    "شما مجاز به استفاده از پرداخت اینترنتی از طریق آدرس", JsonRequestBehavior.AllowGet);
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            string Tax = "";
            var Bank = p.sp_tblDargah_PardakhtSelect("fldDargahType", Type.ToString(), "","", 0).FirstOrDefault();
            System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldTarakoneshCode", typeof(string));
            p.sp_tblPardakhtOnlineInsert(_id, Amount, false, "", Convert.ToInt64(Session["PersonId"]), Type);
            Session["Type"] = Type;
            Session["Amount"] = Amount;
            Session["Semester"] = Semester;
            Session["Tax"] = _id.Value;
            if (Bank.fldBankId == 4)
                return RedirectToAction("Index", "BankMarkazi");
            if (Bank.fldBankId == 2)
                return RedirectToAction("Index", "Tejarat");
            else if (Bank.fldBankId == 1)
                return RedirectToAction("Index", "Melli");

            return RedirectToAction("Index", "Parsian");
        }

        public ActionResult GetSemester()
        {
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var q = p.sp_B_tblTermsSelect("", "", 0).ToList().Select(c => new { fldID = c.fldId, fldName = c.fldTermName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetType()
        {
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var q = p.sp_tblDargah_PardakhtSelect("", "", "","", 0).ToList().Select(c => new { fldID = c.fldId, fldName = c.fldDargahTypeName+"_"+c.fldPos_OnlineName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DargahDetail(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.UniAutomationEntities p = new Models.UniAutomationEntities();
                var q = p.sp_tblDargah_PardakhtSelect("fldId", id.ToString(),"","", 1).FirstOrDefault();
                return Json(new
                {
                    fldBankId = q.fldBankId,
                    fldDargahType = q.fldDargahType,
                    fldPos_Online=q.fldPos_Online
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
    }
}
