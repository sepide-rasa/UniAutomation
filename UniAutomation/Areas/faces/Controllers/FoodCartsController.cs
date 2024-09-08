using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using UniAutomation.Areas.faces.Controllers.Acc;


namespace UniAutomation.Areas.faces.Controllers
{
    public class FoodCartsController : Controller
    {
        //
        // GET: /faces/FoodCarts/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 11))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }

        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblFoodCartsSelect("All", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

       
        public ActionResult Save(Models.sp_tblFoodCartsSelect cart)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                if (cart.fldDesc == null)
                    cart.fldDesc = "";
                if (cart.fldRFID == null)
                    cart.fldRFID = "";
                if (cart.fldID == 0)
                {//ثبت رکورد جدید

                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 47))
                    {
                        var k = p.sp_tblFoodCartsSelect("fldUserName", cart.fldUserName, 0).Any();
                        if(k)
                            return Json(new { data = "قبلا کارتی با این نام کاربری صادر شده است.", state = 1 });
                        else
                        p.sp_tblFoodCartsInsert(cart.fldStudentID, cart.fldTeacherID, cart.fldPersonalID, cart.fldUserName,  Class1.GenerateHash(cart.fldPassword),cart.fldType, Convert.ToInt32(Session["UserId"]), cart.fldDesc, cart.fldRFID);

                        return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                    }
                    else
                    {
                        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");
                    }

                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 48))
                    {
                        var k = p.sp_tblFoodCartsSelect("fldUserName", cart.fldUserName, 0).FirstOrDefault();
                        if (k != null && k.fldID != cart.fldID)
                            return Json(new { data = "قبلا کارتی با این نام کاربری صادر شده است.", state = 1 });
                        p.sp_tblFoodCartsUpdate(cart.fldID, cart.fldStudentID, cart.fldTeacherID, cart.fldPersonalID, cart.fldUserName,  Class1.GenerateHash(cart.fldPassword), cart.fldType, Convert.ToInt32(Session["UserId"]), cart.fldDesc, cart.fldRFID);
                        return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
                    }
                    else
                    {
                        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");
                    }

                }
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }

        //public ActionResult Reload(string field, string value, string sal, string mah, string nobat, int top, int searchtype)
        //{//جستجو
        //    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
        //    var q = m.sp_tblMorkhasiSelect("Sal_Mah_Nobat", value, Convert.ToInt32(sal), Convert.ToInt32(mah), Convert.ToInt32(nobat), top).ToList();

        //    return Json(q, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldFamily_All", "fldName_All", "fldMelliCode_All", "fldStudentNumber_All" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblFoodCartsSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                 if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 49))
                    {

                Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                if (Convert.ToInt32(id) != 0)
                {
                    Car.sp_tblFoodCartsDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
                    return Json(new { data = "حذف با موفقیت انجام شد.", state = 0 });
                }
                else
                {
                    return Json(new { data = "رکوردی برای حذف انتخاب نشده است.", state = 1 });
                }
                      }
                     else
                     {
                         Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                         return RedirectToAction("error", "Metro");
                     }

            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public ActionResult ChangeStatus(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 250))
                {

                    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        var s=m.sp_tblFoodCartsSelect("fldId_All", id, 0).FirstOrDefault();
                        m.sp_tblFoodCartsActiveUpdate(Convert.ToInt32(id),!s.fldActive);
                        return Json(new { data = "تغییر وضعیت با موفقیت انجام شد.", state = 0 });
                    }
                    else
                    {
                        return Json(new { data = "رکوردی برای تغییر وضعیت انتخاب نشده است.", state = 1 });
                    }
                }
                else
                {
                    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                    return RedirectToAction("error", "Metro");
                }

            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_tblFoodCartsSelect("fldId_All", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {

                    fldID = q.fldID,
                    fldPersonalID = q.fldPersonalID,
                    fldStudentID = q.fldStudentID,
                    fldTeacherID = q.fldTeacherID,
                    fldName = q.fldName + " " + q.fldFamily,
                    fldMelliCode = q.fldMelliCode,
                    fldUserName = q.fldUserName,
                    fldPassword = q.fldPassword,
                    fldDesc = q.fldDesc,
                    fldRFID = q.fldRFID,
                    fldType=q.fldType
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public JsonResult Reset(int id)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_tblFoodCartsSelect("fldId_All", id.ToString(), 0).FirstOrDefault();
                if (q != null)
                {

                    p.sp_tblFoodCartsPassUpdate(id,  Class1.GenerateHash(q.fldMelliCode), Convert.ToInt32(Session["UserId"]));
                    return Json(new { data = "تغییر رمز با موفقیت انجام شد.", state = 0 });
                }
                return Json(new { data = "دانشجویی با این مشخصات وجود ندارد.", state = 1 });

            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }

    }
}
