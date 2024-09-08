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
    public class Dargah_Pardakht_InfoController : Controller
    {
        //
        // GET: /faces/Dargah_Pardakht_Info/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 226))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult GetType()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblDargah_PardakhtSelect("", "","","", 0).ToList().Select(c => new { fldID = c.fldId, fldName = c.fldTitle+"_"+c.fldDargahTypeName+"_"+c.fldPos_OnlineName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblDargah_Pardakht_InfoSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Save(List<Models.sp_tblDargah_Pardakht_InfoSelect> Value, int DargahPardakhtId)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();


              //ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 227))
                    {
                        p.sp_tblDargah_Pardakht_InfoDargahPardakhtIdDelete(DargahPardakhtId, Convert.ToInt32(Session["UserId"]));
                        foreach (var item in Value)
                            p.sp_tblDargah_Pardakht_InfoInsert(item.fldDargahPardakhtId, item.fldParametrId, item.fldMount, Convert.ToInt32(Session["UserId"]), "");
                          
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

        public ActionResult Reload(int value)
        {//جستجو
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblDargah_Pardakht_Info_Value(value).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 229))
                {

                    Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_tblDargah_Pardakht_InfoDelete(Convert.ToByte(id), Convert.ToInt32(Session["UserId"]));
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
                var q = p.sp_tblDargah_Pardakht_InfoSelect("fldId", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldId = q.fldId,
                    fldDargahPardakhtId = q.fldDargahPardakhtId,
                    fldParametrId = q.fldParametrId,
                    fldMount=q.fldMount,
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
