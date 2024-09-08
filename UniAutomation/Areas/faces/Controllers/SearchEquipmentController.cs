using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace UniAutomation.Areas.faces.Controllers
{
    public class SearchEquipmentController : Controller
    {
        //
        // GET: /faces/SearchEquipment/

        public ActionResult Index(int id)
        {
            ViewBag.state = id;
            Session["field"] = "";
            Session["Value"] = "";
            Session["top"] = "30";
            return PartialView();
        }
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblEquipmentSelect(Session["field"].ToString(), Session["Value"].ToString(), Convert.ToInt32(Session["top"])).ToList().ToDataSourceResult(request);
            return Json(q);
        }
        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldId", "fldName"};
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            //Models.ProductionAutomationEntities m = new Models.ProductionAutomationEntities();
            //var q = m.sp_P_tblSefaresheTolidSelect(_fiald[Convert.ToInt32(field)], searchtext,"", top).ToList();
            //return Json(q, JsonRequestBehavior.AllowGet);
            Session["field"] = _fiald[Convert.ToInt32(field)];
            Session["Value"] = searchtext;
            Session["top"] = top;
            return Json("", JsonRequestBehavior.AllowGet);
        }

    }
}
