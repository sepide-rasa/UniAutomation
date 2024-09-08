using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace UniAutomation.Areas.faces.Controllers
{
    public class SearchBookStockController : Controller
    {
        //
        // GET: /faces/SearchBookStock/

        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblL_BookStockSelect("", "", 30).Where(k=>k.fldActive_Deactive==true).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldId", "fldBookTitle", "fldPublisher" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblL_BookStockSelect(_fiald[Convert.ToInt32(field)], searchtext, top).Where(k => k.fldActive_Deactive == true).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

    }
}
