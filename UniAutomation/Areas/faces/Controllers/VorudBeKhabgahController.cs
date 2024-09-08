using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using UniAutomation.Areas.faces.Controllers.Acc;

namespace UniAutomation.Areas.faces.Controllers
{
    public class VorudBeKhabgahController : Controller
    {
        //
        // GET: /faces/VorudBeKhabgah/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");  
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 200))
            {
            return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public ActionResult TypeKhoruj(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            ViewBag.State = id;
            return PartialView();
        }

        public JsonResult VorudReload(string rfid, string TypeKhoruj)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                var date = m.sp_GetDate().FirstOrDefault();
                bool havekhabgah = false;
                string Otagh = "";
                string Khabgah = "";
                bool Status = true;
                if (rfid != "")
                {
                    var foodcart = m.sp_tblFoodCartsSelect("fldRFID", rfid, 0).FirstOrDefault();
                    

                    if (foodcart != null)
                    {
                        int id = Convert.ToInt32(foodcart.fldID);
                        var q1 = m.sp_B_tblTermsSelect("fldTarikh", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), 0).FirstOrDefault();
                        var q2 = m.sp_B_tblEnterDormSelect("fldStudentId_Term", foodcart.fldStudentID.ToString(), q1.fldId.ToString(), 0).FirstOrDefault();
                        if (q2 != null)
                        {
                            havekhabgah = true;
                            Otagh = q2.fldNameRoom.ToString();
                            Khabgah = q2.fldBuildingName;

                            var q3 = m.sp_B_tblVorudBeKhabgahSelect("fldFoodCartId", id.ToString(), q1.fldId, 0).LastOrDefault();
                            if (q3 != null)
                                Status = !(q3.fldStatus);
                            m.sp_B_tblVorudBeKhabgahInsert(date.fldDateTime, date.fldDateTime.TimeOfDay, foodcart.fldID, Status, Convert.ToInt32(Session["UserId"]), "", Convert.ToByte(TypeKhoruj));
                        }

                            var Vorud = m.sp_B_tblVorudBeKhabgahSelect("fldTarikh", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), q1.fldId, 0).OrderByDescending(h => h.fldId).ToList();
                        
                        var p = m.sp_PersonName(id).FirstOrDefault();

                        return Json(new
                        {
                            Vorud = Vorud,
                            PersonName = p.PersonName,
                            Otagh = Otagh,
                            Khabgah = Khabgah,
                            foodcartID = foodcart.fldID,
                            havekhabgah = havekhabgah
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        
                        return Json(new
                        {
                            PersonName = "کارتی برای شما صادر نشده است.",
                            Otagh = Otagh,
                            Khabgah = Khabgah,
                            havekhabgah =false
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        PersonName = "کارت شما فعال نشده است.",
                        Otagh = Otagh,
                        Khabgah = Khabgah,
                        havekhabgah =false
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CheckStatus(string rfid)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var foodcart = p.sp_tblFoodCartsSelect("fldRFID", rfid, 0).FirstOrDefault();
                int id = Convert.ToInt32(foodcart.fldID);
                var date = p.sp_GetDate().FirstOrDefault();
                var q1 = p.sp_B_tblTermsSelect("fldTarikh", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), 0).FirstOrDefault();
                var q =p.sp_B_tblVorudBeKhabgahSelect("fldFoodCartId", id.ToString(), q1.fldId, 0).LastOrDefault();
                return Json(new
                {
                    fldID = q.fldId,
                    fldStatus = q.fldStatus
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public FileContentResult image(int id)
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
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

    }
}
