using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniAutomation.Areas.faces.Controllers
{
    public class ResetPassController : Controller
    {
        //
        // GET: /faces/ResetPass/

        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult Save(List<Models.RezervG> ArrayL)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_tblFoodCartsSelect("", "", 0).ToList();
                foreach (var item in q)
                {
                    p.sp_PersonChPass(Convert.ToInt32( item.fldID),  Class1.GenerateHash(item.fldMelliCode));
                }
                return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 }); 
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
    }
}
