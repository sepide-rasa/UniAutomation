using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace UniAutomation.Areas.faces.Controllers
{
    public class SearchRoom2Controller : Controller
    {
        //
        // GET: /faces/SearchRoom2/

        public ActionResult Index(int BuildingId,int State)
        {
            Session["BuildingId"] = BuildingId;
            ViewBag.BuildingId = BuildingId;
            ViewBag.State = State;
            return PartialView();
        }
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblBuildingRoomsSelect("fldBuildingCodeId", Session["BuildingId"].ToString(),"", 0).ToList().ToDataSourceResult(request);
            Session.Remove("BuildingId");
            return Json(q);
        }

        public ActionResult Reload(string field, string value, int top, int searchtype, int BuildingId)
        {//جستجو
            string[] _fiald = new string[] { "fldNameRoom" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblBuildingRoomsSelect(_fiald[Convert.ToInt32(field)], searchtext,"", top).ToList();

            return Json(q, JsonRequestBehavior.AllowGet);
        }

    }
}
