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
    public class SearchRoom3Controller : Controller
    {
        //
        // GET: /faces/SearchRoom3/

        public ActionResult Index(int BuildingId,int termId)
        {
            Session["BuildingId"] = BuildingId;
            Session["termId"] = termId;
            ViewBag.BuildingId = BuildingId;
            return PartialView();
        }
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblBuildingRoomsSelect("fldBuildingCodeId_term", Session["BuildingId"].ToString(), Session["termId"].ToString(), 0).ToList().ToDataSourceResult(request);
           // Session.Remove("BuildingId");
            return Json(q);
        }


        public ActionResult Reload(string field, string value, int top, int searchtype, int BuildingId)
        {//جستجو
            string[] _fiald = new string[] { "", "fldNameRoom_term" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblBuildingRoomsSelect(_fiald[Convert.ToInt32(field)], searchtext,Session["termId"].ToString(), top).ToList();

            return Json(q, JsonRequestBehavior.AllowGet);
        }

    }
}