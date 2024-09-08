using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniAutomation.Areas.Teacher.Controllers
{
    public class TeacherLoginController : Controller
    {
        //
        // GET: /Teacher/TeacherLogin/

        public ActionResult Index()
        {
            if (Session["PersonId"] == null)
                return RedirectToAction("logIn", "TeacherAccount");
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var q = p.sp_GetDate().FirstOrDefault();
            var time = q.fldDateTime;
            ViewBag.time = time.Hour.ToString().PadLeft(2, '0') + ":" +
                time.Minute.ToString().PadLeft(2, '0') + ":" +
                time.Second.ToString().PadLeft(2, '0');
            ViewBag.FromDate = MyLib.Shamsi.Miladi2ShamsiString(time.AddDays(-7));
            ViewBag.ToDate = MyLib.Shamsi.Miladi2ShamsiString(time);

            var Student = 1;
            var m = p.sp_tblFoodCartsSelect("fldID", Session["PersonId"].ToString(), 0).FirstOrDefault();
            if (m.fldStudentID == null)
                Student = 0;
            ViewBag.Student = Student;

            Session["PishStartDate"] = "";
            Session["Pish_Term"] = "";

            var s = p.sp_tblStudentSelect("fldID", m.fldStudentID.ToString(), "", 0).FirstOrDefault();


            var a = p.sp_B_tblCallRegister_SectionSelect("fldActive_Deactive", "true", 0).ToList();
            if (s != null)
            {
                foreach (var Item in a)
                {
                    if (s.fldSectionID == Item.fldSectionCodeId && s.fldStatusID == Item.fldStatusDormId && s.fldTermId == Item.fldTermId)
                    {
                        ViewBag.StartDate = Item.fldStartDate;
                        Session["PishStartDate"] = Item.fldStartDate;
                        ViewBag.EndDate = Item.fldEndDate;
                        Session["PishEndDate"] = Item.fldEndDate;
                        ViewBag.StartTime = Item.fldStartTime;
                        Session["PishStartTime"] = Item.fldStartTime;
                        ViewBag.EndTime = Item.fldEndTime;
                        Session["PishEndTime"] = Item.fldEndTime;
                        Session["Pish_Term"] = Item.fldCallTermId;
                    }
                }
            }
            var MatnEtelaiie = "";
            var etelaiie = p.sp_tblEtelaeyeSelect("fldTarikh", time.ToString(), 0).FirstOrDefault();
            if (etelaiie != null)
            {
                MatnEtelaiie = etelaiie.fldMatn;
                MatnEtelaiie = MatnEtelaiie.Replace("\n", "<br/>");
            }

            ViewBag.MatnEtelaiie = MatnEtelaiie;
            return View();
        }

        public FileContentResult Image(int id)
        {//برگرداندن عکس 
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var picid = p.sp_PersonImageId(id).FirstOrDefault();
            var pic = p.sp_tblPicturesSelect("fldId", picid.ImageId.ToString(), 1).FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldImage != null)
                {
                    return File((byte[])pic.fldImage, "jpg");
                }

            }
            return null;

        }
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("index");
        }

    }
}
