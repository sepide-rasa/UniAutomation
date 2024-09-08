using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace UniAutomation.Areas.faces.Controllers
{
    public class SearchFoodCartsInsController : Controller
    {
        //
        // GET: /faces/SearchFoodCartsIns/

        public ActionResult Index(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            ViewBag.State = id;
            return PartialView();
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblFoodCartsSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldFamily", "fldName", "fldMelliCode" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblFoodCartsSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult IndexWithType(int id, int type)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            ViewBag.State = id;
            Session["TypePerson"] = type;
            return PartialView();
        }
        public ActionResult FillWithType([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            int TypePerson=Convert.ToInt32(Session["TypePerson"]);

            var q = m.sp_tblFoodCartsSelect("", "", 0).Where(l => l.fldType == TypePerson ).ToList().ToDataSourceResult(request);
            if(TypePerson==1)
             q = m.sp_tblFoodCartsSelect("", "", 0).Where(l=>l.fldType==1||TypePerson==4).ToList().ToDataSourceResult(request);
            return Json(q);
        }
        public ActionResult ReloadWithType(string field, string value, int top, int searchtype)
        {//جستجو
            int TypePerson = Convert.ToInt32(Session["TypePerson"]);

            string[] _fiald = new string[] { "fldFamily", "fldName", "fldMelliCode" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblFoodCartsSelect(_fiald[Convert.ToInt32(field)], searchtext, 0).Where(l => l.fldType == TypePerson).ToList();

            if (TypePerson == 1)
             q = m.sp_tblFoodCartsSelect(_fiald[Convert.ToInt32(field)], searchtext, 0).Where(l=>l.fldType==1||TypePerson==4).ToList();
            
            return Json(q, JsonRequestBehavior.AllowGet);
        }

    }
}
