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
    public class B_tblBuildingNewDormController : Controller
    {
        //
        // GET: /faces/B_tblBuildingNewDorm/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 96))
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
            var q = m.sp_B_tblBuildingNewDormSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult GetStatusCodeId()
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblStatusDormSelect("", "", 0).ToList().Select(c => new { fldStatusCode = c.fldStatusCode, fldStatusCodeId = c.fldStatusType });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult Save(Models.sp_B_tblBuildingNewDormSelect Build)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                if (Build.fldDesc == null)
                    Build.fldDesc = "";
                if (Build.fldBuildingCode == 0)
                {//ثبت رکورد جدید
                     if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 97))
                    {
                    // System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldId", typeof(int));
                    m.sp_B_tblBuildingNewDormInsert(Build.fldBuildingName, Build.fldStatusCodeId, Build.fldBuildYear, Build.fldBuildExploit, Build.fldStandardCapacity, Build.fldTotalArea,Build.fldAreaInFrastucture,
                        Build.fldTotalFloor, Convert.ToInt32(Session["UserId"]), Build.fldDesc);

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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 98))
                    {

                    m.sp_B_tblBuildingNewDormUpdate(Build.fldBuildingCode,Build.fldBuildingName, Build.fldStatusCodeId, Build.fldBuildYear, Build.fldBuildExploit, Build.fldStandardCapacity, Build.fldTotalArea, Build.fldAreaInFrastucture,
                        Build.fldTotalFloor, Convert.ToInt32(Session["UserId"]), Build.fldDesc);
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
            string[] _fiald = new string[] { "fldBuildYear", "fldBuildingName" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblBuildingNewDormSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 99))
                {
                    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        m.sp_B_tblBuildingNewDormDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                var q = m.sp_B_tblBuildingNewDormSelect("fldBuildingCode", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldBuildingCode = q.fldBuildingCode,
                    fldBuildingName = q.fldBuildingName,
                    fldStatusCodeId = q.fldStatusCodeId,
                    fldBuildYear = q.fldBuildYear,
                    fldBuildExploit = q.fldBuildExploit,
                    fldStandardCapacity = q.fldStandardCapacity,
                    fldTotalArea = q.fldTotalArea,
                    fldAreaInFrastucture = q.fldAreaInFrastucture,
                    fldTotalFloor = q.fldTotalFloor,
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
