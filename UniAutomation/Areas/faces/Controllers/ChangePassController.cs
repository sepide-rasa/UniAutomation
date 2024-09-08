using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniAutomation.Areas.faces.Controllers
{
    public class ChangePassController : Controller
    {
        //
        // GET: /ChPass/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("UserId", "Account");
            return PartialView();
        }

        public ActionResult Save(string cuPass, string NewPass, string ConfNewPass)
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_UserSelect("fldId", Session["UserId"].ToString(), 1, "").FirstOrDefault();
            if (Session["UserPass"].ToString() == Class1.GenerateHash(cuPass))
            {
                if (NewPass == ConfNewPass)
                {
                    p.sp_UserPassUpdate(Convert.ToInt32(Session["UserId"]), Class1.GenerateHash(NewPass));
                    return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                }
                else
                    return Json(new { data = "رمز جدید با تکرار آن یکسان نیست.", state = 1 });
            }
            return Json(new { data = "رمز فعلی وارد شده معتبر نیست.", state = 1 });
        }
    }
}
