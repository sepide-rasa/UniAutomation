using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace UniAutomation.Controllers
{
    public class AccountsController : Controller
    {
        //
        // GET: /Accounts/

        public ActionResult LogIn()
        {
            return View();
        }
        public ActionResult WinChangePass()
        {
            return PartialView();

        }
        //
        // POST: /Account/LogOn        

        [HttpPost]
        public ActionResult LogIn(Models.LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Models.UniAutomationEntities p = new Models.UniAutomationEntities();
                var q = p.sp_CheckPersonPass(model.UserName, Class1.GenerateHash(model.Password)).FirstOrDefault();

                if (q != null)
                {
                    var k = p.sp_tblFoodCartsSelect("fldID", q.fldID.ToString(), 0).FirstOrDefault();
                    if (model.UserType == 1 && k.fldStudentID == null)
                    {
                        ModelState.AddModelError("", "نام کاربری و رمز عبور صحیح نیست.");
                    }
                    else if (model.UserType == 2 && k.fldTeacherID == null)
                    {
                        ModelState.AddModelError("", "نام کاربری و رمز عبور صحیح نیست.");
                    }
                    else if (model.UserType == 3 && k.fldPersonalID == null)
                    {
                        ModelState.AddModelError("", "نام کاربری و رمز عبور صحیح نیست.");
                    }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        Session["PersonId"] = q.fldID;
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "نام کاربری و رمز عبور صحیح نیست.");
                }
            }

            // If we got this far, something failed, redisplay form
            return PartialView(model);
        }
        public ActionResult SaveChangePass(string NewPass, string ConfNewPass)
        {
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var q = p.sp_tblFoodCartsSelect("fldId", Session["PersonId"].ToString(), 1).FirstOrDefault();
                if (NewPass == ConfNewPass)
                {
                    p.sp_PersonChPass(Convert.ToInt32(Session["PersonId"]), Class1.GenerateHash(NewPass));
                    return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                }
                else
                    return Json(new { data = "رمز جدید با تکرار آن یکسان نیست.", state = 1 });
        }
    }
}
