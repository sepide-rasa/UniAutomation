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
    public class B_tblEnterDormController : Controller
    {
        //
        // GET: /faces/B_tblEnterDorm/

        public ActionResult Index(int Id, int termId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 188))
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                var q = m.sp_B_tblTermsSelect("fldId", termId.ToString(), 30).FirstOrDefault();
                ViewBag.SarbargId = Id;
                ViewBag.termId = termId;
                ViewBag.termName = q.fldTermName;
                Session["SarbargId"] = Id;
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
            var q = m.sp_B_tblEnterDormSelect("fldSarbargId", Session["SarbargId"].ToString(),"", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }
        public ActionResult GetDorm()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_B_tblBuildingNewDormSelect("", "", 0).ToList().Select(c => new { fldID = c.fldBuildingCode, fldName = c.fldBuildingName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(Models.sp_B_tblEnterDormSelect stud)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();

                if (stud.fldDesc == null)
                    stud.fldDesc = "";
                
                if (stud.fldId == 0)
                {//ثبت رکورد جدید

                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 189))
                    {
                        p.sp_B_tblEnterDormInsert(stud.fldSarbargId, stud.fldStudentId, stud.fldSemester, stud.fldBuildingRoomsId, stud.fldTrustCharge, stud.fldFicheNo, MyLib.Shamsi.Shamsi2miladiDateTime(stud.fldEnterDate), stud.fldBedNo, Convert.ToInt32(Session["UserId"]), stud.fldDesc);

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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 190))
                    {


                        p.sp_B_tblEnterDormUpdate(stud.fldId, stud.fldSarbargId, stud.fldStudentId, stud.fldSemester, stud.fldBuildingRoomsId, stud.fldTrustCharge, stud.fldFicheNo, MyLib.Shamsi.Shamsi2miladiDateTime(stud.fldEnterDate), stud.fldBedNo, Convert.ToInt32(Session["UserId"]), stud.fldDesc);
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

        public ActionResult Reload(string field, string value,string value2, int top, int searchtype, int RoomId)
        {//جستجو
            string[] _fiald = new string[] { "fldStudentName_SarbargId", "fldStudentNumber_SarbargId", "fldSarbargId" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblEnterDormSelect(_fiald[Convert.ToInt32(field)], searchtext, value2, top).ToList();
            if (RoomId != 0)
                q = m.sp_B_tblEnterDormSelect(_fiald[Convert.ToInt32(field)], searchtext, value2, top).Where(k => k.fldBuildingRoomsId == RoomId).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 191))
                {

                    Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_B_tblEnterDormDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
                var q = p.sp_B_tblEnterDormSelect("fldId", id.ToString(),"", 1).FirstOrDefault();
                return Json(new
                {
                    
                    fldSarbargId=q.fldSarbargId,
                    fldStudentId=q.fldStudentId,
                    fldId = q.fldId,
                    fldName = q.fldStudentName,
                    fldCity=q.fldCity,
                    fldStudentNumber=q.fldStudentNumber,
                    fldCourse=q.fldCourseName,
                    fldSection=q.fldSectionName,
                    fldMobile = q.fldMobile,
                    fldParentPhone=q.fldParentPhone,
                    fldTelephone=q.fldTelephone,
                    fldStatus=q.fldStatusName,
                    fldSemester=q.fldSemester,
                    fldBuildingRooms=q.fldBuildingRoomsId,
                    fldBedNo=q.fldBedNo,
                    fldTrustCharge=q.fldTrustCharge,
                    fldFicheNo=q.fldFicheNo,
                    fldEnterDate = q.fldEnterDate,
                    fldDesc = q.fldDesc,
                    fldBuildingCodeId = q.fldBuildingCodeId,
                    fldBuildingName=q.fldBuildingName,
                    fldNameRoom=q.fldNameRoom
                    

                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public JsonResult CheckStudent(string StudentNumber,string SarbargId)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_B_tblEnterDormSelect("fldStudentNumber_SarbargId", StudentNumber, SarbargId, 1).FirstOrDefault();
                var HaveDorm = false;
                if (q != null)
                    HaveDorm = true;
                return Json(new
                {
                    HaveDorm = HaveDorm

                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
    }
}
