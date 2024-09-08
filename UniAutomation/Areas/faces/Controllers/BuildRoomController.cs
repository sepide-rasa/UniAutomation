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
    public class BuildRoomController : Controller
    {
        //
        // GET: /faces/BuildRoom/

        public ActionResult Index()
        {
             if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 104))
            {
            return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult GetSection()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblSectionSelect("", "", 0).ToList().Select(c => new { fldID = c.fldID, fldName = c.fldName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblBuildingRoomsSelect("", "", "",30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldBuildingName", "fldNameRoom" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblBuildingRoomsSelect(_fiald[Convert.ToInt32(field)], searchtext,"", top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_B_tblBuildingRoomsSelect("fldRoomCode", id.ToString(),"", 1).FirstOrDefault();
                return Json(new
                {

                    fldRoomCode = q.fldRoomCode,
                    fldBuildingCodeId = q.fldBuildingCodeId,
                    fldBuildingName = q.fldBuildingName,
                    fldNameRoom = q.fldNameRoom,
                    fldCapacity = q.fldCapacity,
                    fldSectionId = q.fldSectionId,
                    fldDesc = q.fldDesc
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 107))
                {
                    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        m.sp_B_tblBuildingRoomsDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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

        public ActionResult Save(Models.sp_B_tblBuildingRoomsSelect Room)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                if (Room.fldDesc == null)
                    Room.fldDesc = "";
                if (Room.fldRoomCode == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 105))
                    {
                        // System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldId", typeof(int));
                        m.sp_B_tblBuildingRoomsInsert(Room.fldBuildingCodeId, Room.fldNameRoom, Room.fldCapacity, Room.fldSectionId, Convert.ToInt32(Session["UserId"]), Room.fldDesc);

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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 106))
                    {
                        m.sp_B_tblBuildingRoomsUpdate(Room.fldRoomCode, Room.fldBuildingCodeId, Room.fldNameRoom, Room.fldCapacity, Room.fldSectionId, Convert.ToInt32(Session["UserId"]), Room.fldDesc);
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

    }
}
