using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniAutomation.Models;
using System.Net.NetworkInformation;
using System.IO;
using DotNetOpenAuth.Messaging;
using System.Net;

namespace UniAutomation.Areas.faces.Controllers
{
    
    public class AdminController : Controller
    {
        //
        // GET: /faces/Admin/
        public ActionResult PreviewRptPDFBox()
        {
            return PartialView();
        }
        public ActionResult PreviewRptImgBox()
        {
            return PartialView();
        }
        public ActionResult Index()
        {
            if (Session["UserId"] == null)            
                return RedirectToAction("Logon", "Account");            
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_GetDate().FirstOrDefault();
            var time = q.fldDateTime;
            ViewBag.time = time.Hour.ToString().PadLeft(2, '0') + ":" +
                time.Minute.ToString().PadLeft(2, '0') + ":" +
                time.Second.ToString().PadLeft(2, '0');
            ViewBag.FromDate = MyLib.Shamsi.Miladi2ShamsiString(time.AddDays(-7));
            ViewBag.ToDate = MyLib.Shamsi.Miladi2ShamsiString(time);
            return View();
        }
        public FileContentResult Image(int id)
        {//برگرداندن عکس 
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();

            var pic = p.sp_tblPicturesSelect("fldUser_Id", id.ToString(), 1).FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldImage != null)
                {
                    return File((byte[])pic.fldImage, "jpg");
                }

            }
            return null;

        }
    }
}
