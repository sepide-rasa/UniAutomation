using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace UniAutomation.Areas.faces.Controllers
{
    public class searchEnterDormController : Controller
    {
        //
        // GET: /faces/searchEnterDorm/

        public ActionResult Index(int id)
        {
            ViewBag.EnterDormSarbarg = id;
            Session["EnterDormSarbarg"] = id;
            Session["field"] = "fldSarbargId";
            Session["Value"] = id;
            Session["top"] = "30";
            return PartialView();
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblEnterDormSelect(Session["field"].ToString(), Session["Value"].ToString(), Session["EnterDormSarbarg"].ToString(), Convert.ToInt32(Session["top"])).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldStudentName_SarbargId", "fldStudentNumber_SarbargId" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            //Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            //var q = m.sp_B_tblEnterDormSelect(_fiald[Convert.ToInt32(field)], searchtext,"", top).ToList();
            //return Json(q, JsonRequestBehavior.AllowGet);
            Session["field"] = _fiald[Convert.ToInt32(field)];
            Session["Value"] = searchtext;
            Session["top"] = top;
            return Json("", JsonRequestBehavior.AllowGet);
        }

    }
}
