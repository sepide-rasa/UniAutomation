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
    public class FoodServeController : Controller
    {
        //
        // GET: /faces/FoodServe/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 18))
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
            var q = m.sp_tblFoodServeSelect("fldRezervType", "true","", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Save(Models.serv serv)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                if (serv.fldDesc == null)
                    serv.fldDesc = "";
                if (serv.fldFoodPrograminID == 0)
                    serv.fldFoodPrograminID = null;
                if (serv.fldNumber == 0)
                    serv.fldNumber = null;
                if (serv.fldNobat == 0)
                    serv.fldNobat = null; 

                if (serv.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 68))
                    {
                        p.sp_tblFoodServeInsert(serv.fldFoodPrograminID, serv.fldRezervType, serv.fldStartTime, serv.fldEndTime, serv.fldNumber, serv.fldNobat, Convert.ToInt32(Session["UserId"]), serv.fldDesc);
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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 69))
                    {
                        p.sp_tblFoodServeUpdate(serv.fldID, serv.fldFoodPrograminID, serv.fldRezervType, serv.fldStartTime, serv.fldEndTime, serv.fldNumber, serv.fldNobat, Convert.ToInt32(Session["UserId"]), serv.fldDesc);
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
            string[] _fiald = new string[] { "", "fldFoodPrograminID", "fldRezervType" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblFoodServeSelect(_fiald[Convert.ToInt32(field)], searchtext,"", top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 70))
                {
                    Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_tblFoodServeDelete(Convert.ToByte(id), Convert.ToInt32(Session["UserId"]));
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
                var q = p.sp_tblFoodServeSelect("fldId", id.ToString(),"", 1).FirstOrDefault();
                return Json(new
                {
                    fldID = q.fldID,
                    fldStartTimeH = q.fldStartTime.Hours,
                    fldStartTimeM = q.fldStartTime.Minutes,
                    fldEndTimeH = q.fldEndTime.Hours,
                    fldEndTimeM = q.fldEndTime.Minutes,
                    fldFoodPrograminID = q.fldFoodPrograminID,
                    fldNumber = q.fldNumber,
                    fldFoodName = q.fldFoodName,
                    fldName = q.fldName,
                    fldRezervType = q.fldRezervType,
                    fldNobat = q.fldNobat,
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
