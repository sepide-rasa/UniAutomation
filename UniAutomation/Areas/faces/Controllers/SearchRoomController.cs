﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace UniAutomation.Areas.faces.Controllers
{
    public class SearchRoomController : Controller
    {
        //
        // GET: /faces/SearchRoom/

        public ActionResult Index()
        {
            return PartialView();
        }
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblBuildingNewDormSelect("", "", 0).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] {  "fldBuildingName" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblBuildingNewDormSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
           
            return Json(q, JsonRequestBehavior.AllowGet);
        }

    }
}