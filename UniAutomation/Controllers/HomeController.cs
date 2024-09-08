using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniAutomation.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["PersonId"] == null)
                return RedirectToAction("logIn", "Accounts");
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var q = p.sp_GetDate().FirstOrDefault();
            var time = q.fldDateTime;
            ViewBag.time = time.Hour.ToString().PadLeft(2, '0') + ":" +
                time.Minute.ToString().PadLeft(2, '0') + ":" +
                time.Second.ToString().PadLeft(2, '0');
            ViewBag.FromDate = MyLib.Shamsi.Miladi2ShamsiString(time.AddDays(-7));
            ViewBag.ToDate = MyLib.Shamsi.Miladi2ShamsiString(time);

            var Student = 0;
            var Teacher = 0;
            var Personel = 0;
            var m = p.sp_tblFoodCartsSelect("fldID", Session["PersonId"].ToString(), 0).FirstOrDefault();
            if (m.fldStudentID != null)
                Student = 1;
            else if (m.fldTeacherID != null)
                Teacher = 1;
            else if (m.fldPersonalID != null)
                Personel = 1;
            ViewBag.Student = Student;
            ViewBag.Teacher = Teacher;
            ViewBag.Personel = Personel;

            Session["PishStartDate"] = "";
            Session["Pish_Term"] = "";

                var s = p.sp_tblStudentSelect("fldID", m.fldStudentID.ToString(),"", 0).FirstOrDefault();
  
             
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
            var etelaiie = p.sp_tblEtelaeyeSelect("fldTarikh",time.ToString(),0).FirstOrDefault();
            if (etelaiie != null)
            {
                MatnEtelaiie = etelaiie.fldMatn;
                MatnEtelaiie=MatnEtelaiie.Replace("\n", "<br/>");
            }
            
            ViewBag.MatnEtelaiie = MatnEtelaiie;
                return View();
        }
        public JsonResult IsPassValid()
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Areas.faces.Models.UniAutomationEntities1 p = new Areas.faces.Models.UniAutomationEntities1();
                var m = p.sp_tblFoodCartsSelect("fldID", Session["PersonId"].ToString(), 0).FirstOrDefault();
                var NotValid = 1;
                if (m.fldTarikhUpdatePass != null)
                {
                var s=MyLib.Shamsi.ShamsiAddDay(m.fldTarikhUpdatePass,180);
                    if(MyLib.Shamsi.Shamsi2miladiDateTime(s)>p.sp_GetDate().FirstOrDefault().fldDateTime)
                        NotValid = 0;
                }
                return Json(new
                {
                    NotValid = NotValid
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
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
