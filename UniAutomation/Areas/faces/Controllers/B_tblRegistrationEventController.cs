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
    public class B_tblRegistrationEventController : Controller
    {
        //
        // GET: /faces/B_tblRegistrationEvent/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 160))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult GetBuild()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_B_tblBuildingNewDormSelect("", "", 0).ToList().Select(c => new { fldID = c.fldBuildingCode, fldName = c.fldBuildingName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblRegistrationEventSelect("", "","", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }


        public ActionResult Save(Models.sp_B_tblRegistrationEventSelect Registr)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                if (Registr.fldDesc == null)
                    Registr.fldDesc = "";
                if (Registr.fldEventCode == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 161))
                    {

                        // System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldId", typeof(int));
                        m.sp_B_tblRegistrationEventInsert(Registr.fldEventName, Registr.fldBuildingCodeId, Registr.fldRoomCodeId,Registr.fldStudentCodeId, Registr.fldStudentId,
                            MyLib.Shamsi.Shamsi2miladiDateTime(Registr.fldEventDate), Registr.fldEventTime, Convert.ToInt32(Session["UserId"]), Registr.fldDesc);

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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 162))
                    {
                        m.sp_B_tblRegistrationEventUpdate(Registr.fldEventCode, Registr.fldEventName, Registr.fldBuildingCodeId, Registr.fldRoomCodeId, Registr.fldStudentCodeId, Registr.fldStudentId,
                             MyLib.Shamsi.Shamsi2miladiDateTime(Registr.fldEventDate), Registr.fldEventTime, Convert.ToInt32(Session["UserId"]), Registr.fldDesc);
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

        public ActionResult Reload(string field, string value, string value2, int top, int searchtype, int RoomId)
        {//جستجو
            if (value2 == "")
                value2 = "%%";
            string[] _fiald = new string[] { "fldEventName", "fldStudentNumber", "fldEventDate", "fldBuildingCodeId" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblRegistrationEventSelect(_fiald[Convert.ToInt32(field)], searchtext, value2, top).ToList().Where(k => k.fldRoomCodeId == RoomId).ToList();
            if (RoomId == 0)
                q = m.sp_B_tblRegistrationEventSelect(_fiald[Convert.ToInt32(field)], searchtext, value2, top).ToList().ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 163))
                {
                    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        m.sp_B_tblRegistrationEventDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
                var q = m.sp_B_tblRegistrationEventSelect("fldEventCode", id.ToString(),"", 1).FirstOrDefault();
                return Json(new
                {
                    fldEventCode = q.fldEventCode,
                    fldBuildingCodeId = q.fldBuildingCodeId,
                    fldRoomCodeId = q.fldRoomCodeId,
                    fldEventDate = q.fldEventDate,
                    fldStudentId = q.fldStudentId,
                    fldStudentNumber=q.fldStudentNumber,
                    fldEventName = q.fldEventName,
                    fldEventTimeH = q.fldEventTime.Hours,
                    fldEventTimeM = q.fldEventTime.Minutes,
                    fldNameRoom = q.fldNameRoom,
                    fldBuildingName = q.fldBuildingName,
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
