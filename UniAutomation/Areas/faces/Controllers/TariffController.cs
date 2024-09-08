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
    public class TariffController : Controller
    {
        //
        // GET: /faces/Tariff/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 14))
            {
            return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult GetStatus()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblStatusSelect("", "", 30).ToList().Select(c => new { fldID = c.fldID, fldName = c.fldName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTerms()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_B_tblTermsSelect("", "", 0).ToList().Select(c => new { fldID = c.fldTermName, fldName = c.fldTermName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSection()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblSectionSelect("", "", 30).ToList().Select(c => new { fldID = c.fldID, fldName = c.fldName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFoodInfo()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblFoodInfoSelect("", "", 0).ToList().Select(c => new { fldID = c.fldID, fldName = c.fldFoodName + "_" + c.fldNobatName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblTariffSelect("", "","","","","", 0).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Save(Models.Tariff Tariff)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                if (Tariff.fldDesc == null)
                    Tariff.fldDesc = "";
                int? fldFoodInfoID = Tariff.fldFoodInfoID;
                if (fldFoodInfoID == 0)
                    fldFoodInfoID = null;

                if (Tariff.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 56))
                    {
                        p.sp_tblTariffInsert(MyLib.Shamsi.Shamsi2miladiDateTime(Tariff.fldTariffDate), Tariff.fldUserType, Tariff.fldStartDate, Tariff.fldEndDate, Tariff.fldRezervType, Tariff.fldPrice, fldFoodInfoID, Tariff.fldSectionID, Tariff.fldStatusID, Convert.ToInt32(Session["UserId"]), Tariff.fldDesc);
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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 57))
                    {
                        p.sp_tblTariffUpdate(Tariff.fldID, MyLib.Shamsi.Shamsi2miladiDateTime(Tariff.fldTariffDate), Tariff.fldUserType, Tariff.fldStartDate, Tariff.fldEndDate, Tariff.fldRezervType, Tariff.fldPrice, fldFoodInfoID, Tariff.fldSectionID, Tariff.fldStatusID, Convert.ToInt32(Session["UserId"]), Tariff.fldDesc);
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

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldId", "fldFoodName", "fldStartDate", "fldStatusName", "fldSectionName", "fldUserTypeName" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblTariffSelect(_fiald[Convert.ToInt32(field)], searchtext,"","","","", 0).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 58))
                {
                    Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_tblTariffDelete(Convert.ToByte(id), Convert.ToInt32(Session["UserId"]));
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
        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_tblTariffSelect("fldId", id.ToString(),"","","","", 1).FirstOrDefault();
                return Json(new
                {
                    fldID = q.fldID,
                    fldTariffDate=q.fldTariffDate,
                    fldUserType=q.fldUserType,
                    fldStartDate = q.fldStartDate.ToString(),
                    fldEndDate=q.fldEndDate.ToString(),
                    fldRezervType=q.fldRezervType,
                    fldPrice=q.fldPrice,
                    fldFoodInfoID=q.fldFoodInfoID,
                    fldSectionID=q.fldSectionID,
                    fldStatusID=q.fldStatusID,
                    fldDesc = q.fldDesc
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
    }
}
