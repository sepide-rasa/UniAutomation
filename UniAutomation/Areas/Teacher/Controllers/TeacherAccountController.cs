using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace UniAutomation.Areas.Teacher.Controllers
{
    public class TeacherAccountController : Controller
    {
        //
        // GET: /Teacher/TeacherAccount/

        public ActionResult LogIn()
        {
            return View();
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
                    if (k.fldTeacherID == null)
                    {
                        ModelState.AddModelError("", "نام کاربری و رمز عبور صحیح نیست.");
                    }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        Session["PersonId"] = q.fldID;
                        return RedirectToAction("Index", "TeacherLogin");
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

    }
}
