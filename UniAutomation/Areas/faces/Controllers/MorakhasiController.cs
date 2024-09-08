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
    public class MorakhasiController : Controller
    {
        //
        // GET: /faces/Morakhasi/

        public ActionResult Index(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 192))
            {
            ViewBag.EnterDormSarbarg = id;
            Session["EnterDormSarbarg"] = id;
            Session["field"] = "fldEnterDormSarbargId";
            Session["Value"] = id;
            Session["top"] = "30";
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
            var q = m.sp_B_tblMorakhasiSelect(Session["field"].ToString(), Session["EnterDormSarbarg"].ToString(), Convert.ToInt32(Session["top"])).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Save(Models.sp_B_tblMorakhasiSelect Morakhasi)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                if (Morakhasi.fldDesc == null)
                    Morakhasi.fldDesc = "";
                if (Morakhasi.fldId == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 193))
                    {
                        m.sp_B_tblMorakhasiInsert(Morakhasi.fldEnterDormId, MyLib.Shamsi.Shamsi2miladiDateTime(Morakhasi.fldStartDate), MyLib.Shamsi.Shamsi2miladiDateTime(Morakhasi.fldFinishDate), Morakhasi.fldHostName, Morakhasi.fldNesbat, Morakhasi.fldAddress, MyLib.Shamsi.Shamsi2miladiDateTime(Morakhasi.fldFormDate), Convert.ToInt32(Session["UserId"]), Morakhasi.fldDesc);

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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 194))
                    {
                        m.sp_B_tblMorakhasiUpdate(Morakhasi.fldId, Morakhasi.fldEnterDormId, MyLib.Shamsi.Shamsi2miladiDateTime(Morakhasi.fldStartDate), MyLib.Shamsi.Shamsi2miladiDateTime(Morakhasi.fldFinishDate), Morakhasi.fldHostName, Morakhasi.fldNesbat, Morakhasi.fldAddress, MyLib.Shamsi.Shamsi2miladiDateTime(Morakhasi.fldFormDate), Convert.ToInt32(Session["UserId"]), Morakhasi.fldDesc);
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
            string[] _fiald = new string[] { "fldEnterDormSarbargId" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            //Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            //var q = m.sp_B_tblMorakhasiSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            //return Json(q, JsonRequestBehavior.AllowGet);
            Session["field"] = _fiald[Convert.ToInt32(field)];
            Session["Value"] = searchtext;
            Session["top"] = top;
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 195))
                {
                    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        m.sp_B_tblMorakhasiDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
                var q = m.sp_B_tblMorakhasiSelect("fldId", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldId = q.fldId,
                    fldEnterDormId = q.fldEnterDormId,
                    fldStudentName = q.fldStudentName,
                    fldStudentNumber = q.fldStudentNumber,
                    fldCourseName = q.fldCourseName,
                    fldCity = q.fldCity,
                    fldBuildingName = q.fldBuildingName,
                    fldNameRoom = q.fldNameRoom,
                    fldStartDate = q.fldStartDate,
                    fldFinishDate = q.fldFinishDate,
                    fldHostName = q.fldHostName,
                    fldNesbat = q.fldNesbat,
                    fldAddress = q.fldAddress,
                    fldFormDate = q.fldFormDate,
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
