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
    public class B_tblDamagePersonController : Controller
    {
        //
        // GET: /faces/B_tblDamagePerson/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 168))
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
            var q = m.sp_B_tblDamagePersonSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }
        

        public ActionResult Save(Models.sp_B_tblDamagePersonSelect Damage)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                if (Damage.fldDesc == null)
                    Damage.fldDesc = "";
                if (Damage.fldDamageCode == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 169))
                    {
                    // System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldId", typeof(int));
                    m.sp_B_tblDamagePersonInsert(Damage.fldDamageName, Damage.fldStudentCodeId, MyLib.Shamsi.Shamsi2miladiDateTime(Damage.fldDamageDate),
                        Damage.fldDamageTime, Damage.fldTotalAmount, Damage.fldEquimpmentRoomId, Damage.fldCount, Convert.ToInt32(Session["UserId"]), Damage.fldDesc);

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
                     if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 170))
                    {
                    m.sp_B_tblDamagePersonUpdate(Damage.fldDamageCode,Damage.fldDamageName, Damage.fldStudentCodeId, MyLib.Shamsi.Shamsi2miladiDateTime(Damage.fldDamageDate),
                        Damage.fldDamageTime, Damage.fldTotalAmount, Damage.fldEquimpmentRoomId, Damage.fldCount, Convert.ToInt32(Session["UserId"]), Damage.fldDesc);
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
            string[] _fiald = new string[] { "fldDamageName", "fldStudentCodeId", "fldStudentName", "fldStudentNumber" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblDamagePersonSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 171))
                {
                    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        m.sp_B_tblDamagePersonDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
                var q = m.sp_B_tblDamagePersonSelect("fldDamageCode", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldCount=q.fldCount,
                    fldEquimpmentRoomId = q.fldEquimpmentRoomId,
                    fldEqName = q.fldEqName,
                    fldDamageCode = q.fldDamageCode,
                    fldDamageName = q.fldDamageName,
                    fldDamageDate = q.fldDamageDate,
                    fldDamageTimeH = q.fldDamageTime.Hours,
                    fldDamageTimeM = q.fldDamageTime.Minutes,
                    fldStudentCodeId = q.fldStudentCodeId,
                    fldTotalAmount = q.fldTotalAmount,
                    fldStudentNumber=q.fldStudentNumber,
                    StudentName=q.StudentName,
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
