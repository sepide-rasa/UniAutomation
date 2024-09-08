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
    public class Dargah_PardakhtController : Controller
    {
        //
        // GET: /faces/Dargah_Pardakht/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 222))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult GetBankName()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblBankFixedSelect("", "", 0).ToList().Select(c => new { fldID = c.fldId, fldName = c.fldTitle });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblDargah_PardakhtSelect("", "","","", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Save(Models.sp_tblDargah_PardakhtSelect Dargah)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();


                if (Dargah.fldDesc == null)
                    Dargah.fldDesc = "";
                if (Dargah.fldId == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 223))
                    {

                        p.sp_tblDargah_PardakhtInsert(Dargah.fldDargahType,Dargah.fldPos_Online, Dargah.fldBankId, Convert.ToInt32(Session["UserId"]), Dargah.fldDesc);
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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 224))
                    {

                        p.sp_tblDargah_PardakhtUpdate(Dargah.fldId, Dargah.fldDargahType, Dargah.fldPos_Online, Dargah.fldBankId, Convert.ToInt32(Session["UserId"]), Dargah.fldDesc);
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

        public ActionResult Reload(string field, string value, string value2,string Pos_Online, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldBank_DargahType_fldPos_Online" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblDargah_PardakhtSelect(_fiald[Convert.ToInt32(field)], value, value2,Pos_Online, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 225))
                {

                    Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_tblDargah_PardakhtDelete(Convert.ToByte(id), Convert.ToInt32(Session["UserId"]));
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
                var q = p.sp_tblDargah_PardakhtSelect("fldId", id.ToString(),"","", 1).FirstOrDefault();
                var T="0";
                if(q.fldPos_Online)
                    T="1";
                return Json(new
                {
                    fldId = q.fldId,
                    fldDargahType = q.fldDargahType,
                    fldPos_Online=T,
                    fldBankId = q.fldBankId,
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
