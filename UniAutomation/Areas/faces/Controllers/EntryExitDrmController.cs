using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniAutomation.Areas.faces.Controllers
{
    public class EntryExitDrmController : Controller
    {
        //
        // GET: /faces/EntryExitDrm/

        public ActionResult Index()
        {
            return PartialView();
        }

    }
}
