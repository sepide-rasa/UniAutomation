using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using UniAutomation.Areas.faces.Controllers.Acc;
using System.Web.Configuration;

namespace UniAutomation.Areas.faces.Controllers
{
    public class EatingController : Controller
    {
        //
        // GET: /faces/Eating/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 19))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var date = m.sp_GetDate().FirstOrDefault();
            int? Nobat = null;

            var serve = m.sp_tblFoodServeSelect("NullFoodProgramingId", "", "", 0)
                .Where(h => date.fldDateTime.TimeOfDay >= h.fldStartTime && date.fldDateTime.TimeOfDay <= h.fldEndTime).FirstOrDefault();
            if (serve != null)
                Nobat = serve.fldNobat;
            else
            {
                serve = m.sp_tblFoodServeSelect("fldFoodDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), "", 1).FirstOrDefault();
                if (serve != null)
                    if (date.fldDateTime.TimeOfDay >= serve.fldStartTime && date.fldDateTime.TimeOfDay <= serve.fldEndTime)
                        Nobat = serve.fldNobat;
            }

            var q = m.sp_AmarSelect(date.fldDateTime, Convert.ToByte(Nobat)).ToList().ToDataSourceResult(request);
            return Json(q);
        }
         
        public JsonResult ReservReload(string rfid)
        {

            try
            {
                string Ip = WebConfigurationManager.AppSettings["UniIp"];
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                var date = m.sp_GetDate().FirstOrDefault();
                int? Nobat = null;
                bool IsRezerv = false;
                bool AzadTime = false;
                string GhazaAzad = "";
                int HaveEating = 0;
                var foodcart = m.sp_tblFoodCartsSelect("fldRFID", rfid, 0).FirstOrDefault();
                if (foodcart != null)
                {
                    int id = Convert.ToInt32(foodcart.fldID);

                    int count = 0, Azadcount = 0;
                    bool havesharj = true;
                    //از جدول رزرو انتخاب کند اگر بزرکتر از صفر بود price=0 else از جدول ساعت آزاد بخون اگر مجاز بود و تعداد پر نبود مبلغ آزاد را درج کن
                    //var time = m.sp_tblSettingSelect().FirstOrDefault();
                    var serve = m.sp_tblFoodServeSelect("NullFoodProgramingId", "", "", 0).Where(h => date.fldDateTime.TimeOfDay >= h.fldStartTime && date.fldDateTime.TimeOfDay <= h.fldEndTime).FirstOrDefault();
                    if (serve != null)
                    {
                        IsRezerv = true;
                        Nobat = serve.fldNobat;
                    }
                    else
                    {
                        serve = m.sp_tblFoodServeSelect("fldFoodDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), "", 1).FirstOrDefault();
                        if (serve != null)
                            if (date.fldDateTime.TimeOfDay >= serve.fldStartTime && date.fldDateTime.TimeOfDay <= serve.fldEndTime)
                            {
                                Nobat = serve.fldNobat;
                                AzadTime = true;
                            }
                    }

                    var q = m.sp_ReservInCurrentDate(id, date.fldDateTime, Convert.ToByte(Nobat)).ToList();
                    var IsTeacher = m.sp_tblFoodCartsSelect("fldId", id.ToString(), 1).Where(k => k.fldTeacherID != null).Any();

                    //if (IsTeacher)
                    //{
                    //    var eatcount = m.sp_tblEatingSelect("fldFoodDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), 0).Where(k => k.fldPrice > 0).ToList();
                    //    var F = m.sp_tblFoodProgramingSelect("fldFoodDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), 0).FirstOrDefault();

                    //        var pType = m.sp_GetCartPersonType(id).FirstOrDefault();
                    //        var price = m.sp_tblTariffSelect("fldDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), 0).Where(h => h.fldUserType == pType.PrsonType).FirstOrDefault();
                    //        if (price != null)
                    //        {
                    //            var sharj = m.sp_GetCharge(id).FirstOrDefault();
                    //            if (sharj.Charge > price.fldPrice)
                    //            {
                    //                m.sp_tblEatingInsert(id, F.fldID, price.fldPrice, "");
                    //                Azadcount = 1;
                    //            }
                    //            else
                    //                havesharj = false;
                    //        }

                    //}
                    //else
                    //{
                    var eat = m.sp_tblEatingSelect("fldFoodDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), 0).Where(k => k.fldFoodCartID == id).FirstOrDefault();

                    if (eat == null)
                    {
                        if (q.Count > 0 && IsRezerv == true)
                        {
                            foreach (var item in q)
                            {
                                m.sp_tblEatingInsert(item.fldFoodCartID, item.fldFoodProgramingID, 0, "");
                                count += Convert.ToInt32(item.fldCount);
                            }
                        }
                        else if (AzadTime)
                        {
                            var eatcountAzad = m.sp_tblEatingSelect("fldFoodDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), 0).Where(k => k.fldPrice > 0).ToList();
                            if (m.sp_tblFoodServeSelect("fldFoodDate_CountAzad", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), "", 0).Count() == 1)
                            {
                                var azad = m.sp_tblFoodServeSelect("fldFoodDate_CountAzad", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), "", 0).FirstOrDefault();

                                GhazaAzad = azad.fldFoodName;
                                if (date.fldDateTime.TimeOfDay >= azad.fldStartTime && date.fldDateTime.TimeOfDay <= azad.fldEndTime)
                                {
                                    if (IsTeacher == false)
                                    {

                                        var pType = m.sp_GetCartPersonType(id).FirstOrDefault();
                                        // var price = m.sp_tblTariffSelect("fldDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime),"", 0).Where(h => h.fldRezervType == false && h.fldUserType == pType.PrsonType).FirstOrDefault();

                                        //   var s = m.sp_tblStudentSelect("fldId", foodcart.fldStudentID.ToString(), "", 0).FirstOrDefault();
                                        var f = m.sp_tblFoodProgramingSelect("fldId", azad.fldFoodPrograminID.ToString(), 0).FirstOrDefault();
                                        //var price = m.sp_tblTariffSelect("fldDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), pType.PrsonType.ToString(), s.fldSectionID.ToString(), s.fldStatusID.ToString(), s.fldTermId.ToString(), 0).Where(h => h.fldRezervType == false && h.fldFoodInfoID == f.fldFoodInfoID).FirstOrDefault();
                                        var price = m.sp_tblTariffSelect("fldDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), "", "", "", "", 0).Where(h => h.fldUserType == pType.PrsonType && h.fldRezervType == false).FirstOrDefault();
                                        if (price != null)
                                        {
                                            var sharj = m.sp_GetCharge(id).FirstOrDefault();
                                            if (sharj.Charge > price.fldPrice)
                                            {
                                                m.sp_tblEatingInsert(id, azad.fldFoodPrograminID, price.fldPrice, "");
                                                Azadcount = 1;
                                            }
                                            else
                                                havesharj = false;
                                        }
                                    }
                                    else
                                    {
                                        var F = m.sp_tblFoodProgramingSelect("fldFoodDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), 0).FirstOrDefault();

                                        var pType = m.sp_GetCartPersonType(id).FirstOrDefault();
                                        var price = m.sp_tblTariffSelect("fldDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), "", "", "", "", 0).Where(h => h.fldUserType == pType.PrsonType).FirstOrDefault();
                                        if (price != null)
                                        {
                                            var sharj = m.sp_GetCharge(id).FirstOrDefault();
                                            if (sharj.Charge > price.fldPrice)
                                            {
                                                m.sp_tblEatingInsert(id, F.fldID, price.fldPrice, "");
                                                Azadcount = 1;
                                            }
                                            else
                                                havesharj = false;
                                        }
                                    }
                                }

                            }
                            else
                            {
                                return Json(new
                                {
                                    moreThanOneAzad = 1,
                                    foodcartID = foodcart.fldID
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else
                    {
                        HaveEating = 1;
                    }
                    //}
                    var eating = m.sp_tblEatingSelect("fldFoodDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), 0).OrderByDescending(h => h.fldID).ToList();
                    var Amar = m.sp_AmarSelect(date.fldDateTime, Convert.ToByte(Nobat)).ToList();
                    var p = m.sp_PersonName(id).FirstOrDefault();

                    var VaghtGhazaNis = 0;
                    if (IsRezerv == false && AzadTime == false)
                        VaghtGhazaNis = 1;

                    return Json(new
                    {
                        data = q,
                        Amar = Amar,
                        Eating = eating,
                        PersonName = p.PersonName,
                        foodcartID = foodcart.fldID,
                        Count = count,
                        Azadcount = Azadcount,
                        havesharj = havesharj,
                        GhazaAzad = GhazaAzad,
                        HaveEating = HaveEating,
                        VaghtGhazaNis = VaghtGhazaNis
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var Amar = m.sp_AmarSelect(date.fldDateTime, Convert.ToByte(Nobat)).ToList();
                    return Json(new
                    {
                        PersonName = "کارتی برای شما صادر نشده است."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message }, JsonRequestBehavior.AllowGet);
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
        public ActionResult SelectAzadMenu(int foodcartId)
        {
            ViewBag.foodcartId = foodcartId;
            return PartialView();
        }
        public ActionResult FillAzadMenu([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var date = m.sp_GetDate().FirstOrDefault();
            var q = m.sp_tblFoodServeSelect("fldFoodDate_CountAzad", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), "", 0).ToList().ToDataSourceResult(request);
            return Json(q);
        }
        public ActionResult SaveSelectAzadMenu(int foodcartId, string AzadProgrammingId, string AzadServeId)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            int Azadcount = 0;
            bool havesharj = true;
            var date = m.sp_GetDate().FirstOrDefault();
            var IsTeacher = m.sp_tblFoodCartsSelect("fldId", foodcartId.ToString(), 1).Where(k => k.fldTeacherID != null).Any();
            var azad = m.sp_tblFoodServeSelect("fldId", AzadServeId, "", 0).FirstOrDefault();

            if (date.fldDateTime.TimeOfDay >= azad.fldStartTime && date.fldDateTime.TimeOfDay <= azad.fldEndTime)
            {
                if (IsTeacher == false)
                {
                    var pType = m.sp_GetCartPersonType(foodcartId).FirstOrDefault();
                    // var price = m.sp_tblTariffSelect("fldDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime),"", 0).Where(h => h.fldRezervType == false && h.fldUserType == pType.PrsonType).FirstOrDefault();

                    //   var s = m.sp_tblStudentSelect("fldId", foodcart.fldStudentID.ToString(), "", 0).FirstOrDefault();
                    var f = m.sp_tblFoodProgramingSelect("fldId", azad.fldFoodPrograminID.ToString(), 0).FirstOrDefault();
                    //var price = m.sp_tblTariffSelect("fldDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), pType.PrsonType.ToString(), s.fldSectionID.ToString(), s.fldStatusID.ToString(), s.fldTermId.ToString(), 0).Where(h => h.fldRezervType == false && h.fldFoodInfoID == f.fldFoodInfoID).FirstOrDefault();
                    var price = m.sp_tblTariffSelect("fldDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), "", "", "", "", 0).Where(h => h.fldUserType == pType.PrsonType && h.fldRezervType == false).FirstOrDefault();
                    if (price != null)
                    {
                        var sharj = m.sp_GetCharge(foodcartId).FirstOrDefault();
                        if (sharj.Charge > price.fldPrice)
                        {
                            m.sp_tblEatingInsert(foodcartId, azad.fldFoodPrograminID, price.fldPrice, "");
                            Azadcount = 1;
                        }
                        else
                            havesharj = false;
                    }
                }
                else
                {
                    var F = m.sp_tblFoodProgramingSelect("fldFoodDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), 0).FirstOrDefault();

                    var pType = m.sp_GetCartPersonType(foodcartId).FirstOrDefault();
                    var price = m.sp_tblTariffSelect("fldDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), "", "", "", "", 0).Where(h => h.fldUserType == pType.PrsonType).FirstOrDefault();
                    if (price != null)
                    {
                        var sharj = m.sp_GetCharge(foodcartId).FirstOrDefault();
                        if (sharj.Charge > price.fldPrice)
                        {
                            m.sp_tblEatingInsert(foodcartId, F.fldID, price.fldPrice, "");
                            Azadcount = 1;
                        }
                        else
                            havesharj = false;
                    }
                }
            }
            var eating = m.sp_tblEatingSelect("fldFoodDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), 0).OrderByDescending(h => h.fldID).ToList();
            var Amar = m.sp_AmarSelect(date.fldDateTime, Convert.ToByte(azad.fldNobat)).ToList();
            var p = m.sp_PersonName(foodcartId).FirstOrDefault();
            var q = m.sp_ReservInCurrentDate(foodcartId, date.fldDateTime, Convert.ToByte(azad.fldNobat)).ToList();
            return Json(new
            {
                data = q,
                Amar = Amar,
                Eating = eating,
                PersonName = p.PersonName,
                foodcartID = foodcartId,
                Count = 0,
                Azadcount = Azadcount,
                havesharj = havesharj
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
