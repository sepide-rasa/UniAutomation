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
    public class FoodProgramingController : Controller
    {
        //
        // GET: /faces/FoodPrograming/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 13))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult GetFoodInfo()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblFoodInfoSelect("", "", 0).ToList().Select(c => new { fldID = c.fldID, fldName = c.fldFoodName + "_" + c.fldNobatName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSelfService()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblSelfServiceSelect("", "", 0).ToList().Select(c => new { fldID = c.fldID, fldName = c.fldName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblFoodProgramingSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Save(Models.sp_tblFoodProgramingSelect food)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                if (food.fldDesc == null)
                    food.fldDesc = "";
                if (food.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 53))
                    {
                        var inf = p.sp_tblFoodInfoSelect("fldId", food.fldFoodInfoID.ToString(), 0).FirstOrDefault();
                        var q = p.sp_tblFoodProgramingSelect("fldFoodDate", food.fldFoodDate.ToString(), 0).Where(k => k.fldNobat == inf.fldNobat).Count();
                        if (q >=2)
                        {
                            return Json(new { data = "در این تاریخ برای وعده " + inf.fldNobatName + " دو برنامه غذایی تعریف شده است ", state = 1 });
           
                        }
                        else
                        {
                            p.sp_tblFoodProgramingInsert(MyLib.Shamsi.Shamsi2miladiDateTime(food.fldFoodDate), food.fldFoodInfoID, food.fldSelfServiceID, Convert.ToInt32(Session["UserId"]), food.fldDesc);
                            return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                        }
                    }
                    else
                    {
                        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");
                    }

                }
                else
                {//ویرایش رکورد ارسالی 
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 54))
                    {
                        var inf = p.sp_tblFoodInfoSelect("fldId", food.fldFoodInfoID.ToString(), 0).FirstOrDefault();
                        var q = p.sp_tblFoodProgramingSelect("fldFoodDate", food.fldFoodDate.ToString(), 0).Where(k => k.fldNobat == inf.fldNobat && k.fldID != food.fldID).Count();
                        if (q >= 2)
                        {
                            return Json(new { data = "در این تاریخ برای وعده " + inf.fldNobatName + " دو برنامه غذایی تعریف شده است ", state = 1 });

                        }
                        else
                        {
                            p.sp_tblFoodProgramingUpdate(food.fldID, MyLib.Shamsi.Shamsi2miladiDateTime(food.fldFoodDate), food.fldFoodInfoID, food.fldSelfServiceID, Convert.ToInt32(Session["UserId"]), food.fldDesc);
                            return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
                        }
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

        public JsonResult HolidayDetails(string Tarikh)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                var q = m.sp_tblHolidaySelect("fldTarikh", Tarikh, 1).FirstOrDefault();
                var Monasebat = "";
                if (q != null && Tarikh == q.fldTarikh)
                {
                   
                    Monasebat = q.fldMonasebat;
                }
                
                return Json(new
                {
                    Monasebat = Monasebat

                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldId", "fldFoodDate" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblFoodProgramingSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 55))
                    {
                Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                if (Convert.ToInt32(id) != 0)
                {
                    Car.sp_tblFoodProgramingDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
                    return Json(new { data = "حذف با موفقیت انجام شد.", state = 0 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = "رکوردی برای حذف انتخاب نشده است.", state = 1 }, JsonRequestBehavior.AllowGet);
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
                var q = p.sp_tblFoodProgramingSelect("fldId", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldID = q.fldID,
                    fldFoodDate = q.fldFoodDate,
                    fldFoodInfoID = q.fldFoodInfoID,
                    fldSelfServiceID = q.fldSelfServiceID,
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
