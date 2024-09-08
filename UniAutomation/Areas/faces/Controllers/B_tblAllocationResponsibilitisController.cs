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
    public class B_tblAllocationResponsibilitisController : Controller
    {
        //
        // GET: /faces/B_tblAllocationResponsibilitis/
        public ActionResult Index()
        {
             if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 108))
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
            var q = m.sp_B_tblAllocationResponsibilitisSelect("", "", 30,"").ToList().ToDataSourceResult(request);
            return Json(q);
        }


        public ActionResult Save(Models.sp_B_tblAllocationResponsibilitisSelect Cation)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                if (Cation.fldDesc == null)
                    Cation.fldDesc = "";
                if (Cation.fldAllocationCode == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 109))
                    {
                        // System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldId", typeof(int));
                        m.sp_B_tblAllocationResponsibilitisInsert(Convert.ToInt32(Cation.fldPCodeId), Cation.fldDisciplines, Cation.fldRCodeId, Cation.fldExNumber, Cation.fldEMail, Convert.ToInt32(Session["UserId"]), Cation.fldDesc);

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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 110))
                    {
                        m.sp_B_tblAllocationResponsibilitisUpdate(Cation.fldAllocationCode, Convert.ToInt32(Cation.fldPCodeId), Cation.fldDisciplines, Cation.fldRCodeId, Cation.fldExNumber, Cation.fldEMail, Convert.ToInt32(Session["UserId"]), Cation.fldDesc);
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

        public ActionResult Reload(string field, string value, int top, int searchtype,string value1)
        {//جستجو
            if(value1=="")
                value1="%%";
            string[] _fiald = new string[] { "fldPName_RCodeId", "fldRName_PCodeId", "fldPersonel_RCodeId", "fldRCodeId", "fldPCodeId" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();

            var q = m.sp_B_tblAllocationResponsibilitisSelect(_fiald[Convert.ToInt32(field)], searchtext, top, value1).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 111))
                {
                    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        m.sp_B_tblAllocationResponsibilitisDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
                var q = m.sp_B_tblAllocationResponsibilitisSelect("fldAllocationCode", id.ToString(), 1,"").FirstOrDefault();
                return Json(new
                {
                    fldAllocationCode = q.fldAllocationCode,
                    fldPCodeId = q.fldPCodeId,
                    fldRCodeId = q.fldRCodeId,
                    fldDisciplines = q.fldDisciplines,
                    fldPersonalName = q.fldPersonalName,
                    fldRName = q.fldRName,
                    fldEMail = q.fldEMail,
                    fldExNumber = q.fldExNumber
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
    }
}
