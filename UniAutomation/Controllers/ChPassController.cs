using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniAutomation.Controllers
{
    public class ChPassController : Controller
    {
        //
        // GET: /ChPass/

        public ActionResult Index()
        {
            if (Session["PersonId"] == null)
                return RedirectToAction("logIn", "Accounts");
            return PartialView();
        }

        public ActionResult Save(string cuPass, string NewPass, string ConfNewPass)
        {
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var q = p.sp_tblFoodCartsSelect("fldId", Session["PersonId"].ToString(), 1).FirstOrDefault();
            if (q.fldPassword == Class1.GenerateHash(cuPass))
            {
                if (NewPass == ConfNewPass)
                {
                    p.sp_PersonChPass(Convert.ToInt32(Session["PersonId"]), Class1.GenerateHash(NewPass));
                    return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                }
                else
                    return Json(new { data = "رمز جدید با تکرار آن یکسان نیست.", state = 1 });
            }
            return Json(new { data = "رمز فعلی وارد شده معتبر نیست.", state = 1 });
        }
    }
}
