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
    public class EatFood_DateController : Controller
    {
        //
        // GET: /faces/EatFood_Date/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 234))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult Reload(string Sdate)
        {//جستجو

            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_RptNotEating(MyLib.Shamsi.Shamsi2miladiDateTime(Sdate.ToString()), MyLib.Shamsi.Shamsi2miladiDateTime(Sdate.ToString())).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(int fldFoodCartID, int fldFoodProgramingID)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                p.sp_tblEatingInsert(fldFoodCartID, fldFoodProgramingID, 0, "گروهی_ " + Session["UserId"].ToString());
                    return Json(new { data = "تحویل غذا انجام شد.", state = 0});
             
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1, ch = "" });
            }
        }
        public ActionResult SaveGroup(List<Models.sp_tblEatingSelect> Eat)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                foreach (var item in Eat)
                {
                    p.sp_tblEatingInsert(item.fldFoodCartID, item.fldFoodProgramingID, 0, "گروهی_ " + Session["UserId"].ToString()); 
                }
                return Json(new { data = "تحویل غذا انجام شد.", state = 0 });

            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1, ch = "" });
            }
        }
    }
}
