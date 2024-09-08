using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniAutomation.Controllers
{
    public class _MetroController : Controller
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

        public ActionResult Window()
        {
            return PartialView();
        }
    }
}
