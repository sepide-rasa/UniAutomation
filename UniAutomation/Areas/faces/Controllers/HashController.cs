using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniAutomation.Areas.faces.Controllers
{
    public class HashController : Controller
    {
        //
        // GET: /faces/Hash/

        public ActionResult Index()
        {

            return PartialView();
        }
        public ActionResult Set(int Start, int End)
        {//جستجو
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            if (Start == 0 && End == 0)
            {
                var q = p.sp_tblFoodCartsSelect("", "", 0).ToList();
                Start = Convert.ToInt32(q[0].fldID);
                End = Convert.ToInt32(q[q.Count - 1].fldID);
            }
            var Pass="";
            for (int i = Start; i <= End; i++)
            {
                var s = p.sp_tblFoodCartsSelect("fldID", i.ToString(), 0).FirstOrDefault();
                if (s != null)
                {
                    if (s.fldTeacherID != null)
                    {
                        var T = p.sp_tblTeachersSelect("fldId", s.fldTeacherID.ToString(), 0).FirstOrDefault();
                        Pass = Class1.GenerateHash(T.fldMelliCode);
                    }
                    else if (s.fldStudentID != null)
                    {
                        var T = p.sp_tblStudentSelect("fldId", s.fldStudentID.ToString(), "", 0).FirstOrDefault();
                        Pass = Class1.GenerateHash(T.fldMelliCode);
                    }
                    else if (s.fldPersonalID != null)
                    {
                        var T = p.sp_tblPersonalSelect("fldId", s.fldPersonalID.ToString(), 0).FirstOrDefault();
                        Pass = Class1.GenerateHash(T.fldMelliCode);
                    }
                    p.sp_tblFoodCartsPassUpdate(Convert.ToInt32(s.fldID), Pass, 4);
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SetUserPass(int Start, int End)
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            if (Start == 0 && End == 0)
            {
                var q = p.sp_tblUserSelect("", "", 0,"").ToList();
                Start = Convert.ToInt32(q[0].fldID);
                End = Convert.ToInt32(q[q.Count - 1].fldID);
            }

            for (int i = Start; i <= End; i++)
            {
                var q = p.sp_tblUserSelect("fldID", i.ToString(), 0, "").FirstOrDefault();
                if (q != null)
                    p.sp_UserPassUpdate(q.fldID, Class1.GenerateHash(q.fldUserName));
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SetFoodCart(int Start, int End)
        {//جستجو
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                         var ImgPath = Server.MapPath(@"~\Content\images\Blank.jpg");
                        MemoryStream image = new MemoryStream(System.IO.File.ReadAllBytes(ImgPath));
            if (Start == 0 && End == 0)
            {
                var ss = p.sp_tblStudentSelect("", "", "", 0).ToList();
                foreach (var item in ss)
                {
                    if (p.sp_tblPicturesSelect("fldStudentID", item.fldID.ToString(), 0).Any() == false)
                        p.sp_tblPicturesInsert(item.fldID, null, null, null, image.ToArray(), Convert.ToInt32(Session["UserId"]), "");
                    
                    p.sp_tblFoodCartsInsert(item.fldID, null, null, item.fldStudentNumber, Class1.GenerateHash(item.fldMelliCode), 1, Convert.ToInt32(Session["UserId"]), "", "");
                }
            }
            for (int i = Start; i <= End; i++)
            {
                var ss = p.sp_tblStudentSelect("fldId", i.ToString(), "", 0).FirstOrDefault();
                if (p.sp_tblPicturesSelect("fldStudentID", ss.fldID.ToString(), 0).Any() == false)
                    p.sp_tblPicturesInsert(ss.fldID, null, null, null, image.ToArray(), Convert.ToInt32(Session["UserId"]), "");

                p.sp_tblFoodCartsInsert(ss.fldID, null, null, ss.fldStudentNumber, Class1.GenerateHash(ss.fldMelliCode), 1, Convert.ToInt32(Session["UserId"]), "", "");
                
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}
