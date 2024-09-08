using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniAutomation.Areas.faces.Controllers
{
    public class MetroController : Controller
    {
        //
        // GET: /Metro/

        public ActionResult error()
        {
            return PartialView();
        }

        public ActionResult YesNomsg(string id,string url)
        {
            ViewBag.ID = id;
            ViewBag.URL = url;
            return PartialView();
        }

    }
}
