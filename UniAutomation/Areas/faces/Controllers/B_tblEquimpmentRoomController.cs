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
    public class B_tblEquimpmentRoomController : Controller
    {
        //
        // GET: /faces/B_tblEquimpmentRoom/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 152))
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
            var q = m.sp_B_tblEquimpmentRoomSelect("", "","", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult GetEqu()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_B_tblBuildingNewDormSelect("", "", 0).ToList().Select(c => new { fldID = c.fldBuildingCode,fldName = c.fldBuildingName  });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(Models.sp_B_tblEquimpmentRoomSelect Equimp, string Equipment)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                if (Equimp.fldDesc == null)
                    Equimp.fldDesc = "";
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 153))
                {
                    var e = Equipment.Split(';');
                    var r = m.sp_B_tblEquimpmentRoomSelect("fldRoomCodeId", Equimp.fldRoomCodeId.ToString(), "", 0).ToList();
                    var DelId = false;
                    foreach (var item in r)
                    {
                        DelId = false;
                        for (int i = 0; i < e.Count() - 1; i++)
                            if (item.fldEqID == Convert.ToInt32(e[i]))
                                DelId = true;
                        if (DelId)
                            m.sp_B_tblEquimpmentRoomDelete(item.fldEqCode, Convert.ToInt32(Session["UserId"]));
                    }
                    for (int i = 0; i < e.Count()-1; i++)
                    {
                        var q = m.sp_B_tblEquimpmentRoomSelect("fldEqID", e[i].ToString(), Equimp.fldRoomCodeId.ToString(), 0).FirstOrDefault();
                        if (q == null)
                            m.sp_B_tblEquimpmentRoomInsert(Equimp.fldRoomCodeId, Convert.ToInt32((e[i])), "", Convert.ToInt32(Session["UserId"]), Equimp.fldDesc);
                        else
                            m.sp_B_tblEquimpmentRoomUpdate(q.fldEqCode, Equimp.fldRoomCodeId, Convert.ToInt32((e[i])), "", Convert.ToInt32(Session["UserId"]), Equimp.fldDesc);
                    }
                    return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
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

        public ActionResult Reload(string field, string value, string value2, int top, int searchtype, int RoomId)
        {//جستجو
            if (value2 == "")
                value2 = "%%";
            string[] _fiald = new string[] { "fldEqID", "fldBuildingName", "fldNameRoom", "fldBuildingCodeId" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblEquimpmentRoomSelect(_fiald[Convert.ToInt32(field)], searchtext,value2, top).Where(k=>k.fldRoomCodeId==RoomId).ToList();
            if (RoomId == 0)
                q = m.sp_B_tblEquimpmentRoomSelect(_fiald[Convert.ToInt32(field)], searchtext,value2, top).ToList();

            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 155))
                {
                    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        m.sp_B_tblEquimpmentRoomDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
                var q = m.sp_B_tblEquimpmentRoomSelect("fldEqCode", id.ToString(),"", 1).FirstOrDefault();

                return Json(new
                {
                    fldEqCode = q.fldEqCode,
                    fldBuildingCodeId = q.fldBuildingCodeId,
                    fldRoomCodeId = q.fldRoomCodeId,
                    fldEqID = q.fldEqID,
                    fldNameEquipment=q.fldNameEquipment,
                    fldNameRoom  = q.fldNameRoom,
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
