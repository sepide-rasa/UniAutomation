using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace UniAutomation.Controllers
{
    public class ReservController : Controller
    {
        //
        // GET: /Reserv/

        public ActionResult Index(int id)
        {
            if (Session["PersonId"] == null)
                return RedirectToAction("logIn", "Accounts");
            ViewBag.State = id;
            return PartialView();
        }

        public ActionResult GetSelfName()
        {
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var q = p.sp_tblSelfServiceSelec("", "", 0).ToList().Select(c => new { fldId = c.fldID, fldName = c.fldName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFoods(int Selef, string date, int Nobat)
        {
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var Currentdate = p.sp_GetDate().FirstOrDefault();
            string _date = date;
            var day = p.sp_tblSettingSelect().FirstOrDefault();
            if (MyLib.Shamsi.Shamsi2miladiDateTime(date) < Currentdate.fldDateTime.AddDays(Convert.ToInt32(day.fldReservDay) - 1))
                _date = "";
            var q = p.sp_tblFoodProgramingSelect("fldFoodDate", _date, 0).Where(h => h.fldSelfServiceID == Selef & h.fldNobat == Nobat).ToList().Select(c => new { fldId = c.fldID, fldName = c.fldFoodName });

            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult HaveReserv(string date,string i)
        {
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var q = p.sp_tblFoodProgramingSelect("fldFoodDate", date, 0).ToList();
            bool haveN = false;
            bool haveSH = false;
            bool haveS = false;
            foreach(var Item in q)
            {
                if (Item.fldNobat == 1)
                {
                    if (haveS == false)
                    {
                        var t = p.sp_tblRezervSelect("fldFoodCartID", Session["PersonId"].ToString(), 0).Where(h => h.fldFoodProgramingID == Item.fldID).FirstOrDefault();
                        if (t != null)
                            haveS = true;
                    }
                }
                if (Item.fldNobat == 2)
                {
                    if (haveN == false)
                    {
                        var t = p.sp_tblRezervSelect("fldFoodCartID", Session["PersonId"].ToString(), 0).Where(h => h.fldFoodProgramingID == Item.fldID).FirstOrDefault();
                        if (t != null)
                            haveN = true;
                    }
                }
                if (Item.fldNobat == 3)
                {
                    if (haveSH == false)
                    {
                        var t = p.sp_tblRezervSelect("fldFoodCartID", Session["PersonId"].ToString(), 0).Where(h => h.fldFoodProgramingID == Item.fldID).FirstOrDefault();
                        if (t != null)
                            haveSH = true;
                    }
                }
            }
            return Json(new { haveN = haveN, haveSH = haveSH,haveS=haveS, i = i }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Cancel(string date, int nobat)
        {
            if (Session["PersonId"] == null)
                return RedirectToAction("logIn", "Accounts");
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var Currentdate = p.sp_GetDate().FirstOrDefault();
            var day = p.sp_tblSettingSelect().FirstOrDefault();
            if (MyLib.Shamsi.Shamsi2miladiDateTime(date) < Currentdate.fldDateTime.AddDays(Convert.ToInt32(day.fldReservDay) - 1))
            {
                return Json(new { data = "شما مجوز انصراف در این تاریخ را ندارید .", state = 1 });
            }
            else
            {
                var foodprog = p.sp_tblFoodProgramingSelect("fldFoodDate", date, 0).Where(h => h.fldNobat == nobat).ToList();
                foreach (var item in foodprog)
                {
                    var reserv = p.sp_tblRezervSelect("fldFoodProgramingID", item.fldID.ToString(), 0).Where(h => h.fldFoodCartID == Convert.ToInt32(Session["PersonId"])).FirstOrDefault();
                    if (reserv != null)
                    {
                        p.sp_tblReservDelete(reserv.fldID);
                        p.sp_tblHistoryReserveInsert(reserv.fldFoodCartID, reserv.fldFoodProgramingID, reserv.fldCount, reserv.fldPrice, false, "");
                    }
                }
            }
            var user = p.sp_GetCharge(Convert.ToInt32(Session["PersonId"])).FirstOrDefault();
            return Json(new { data = "انصراف با موفقیت انجام شد.", state = 0, Etebar = user.Charge, nobat = nobat });

        }

        public ActionResult Save(List<Models.Reserv> ArrayL)
        {
            if (Session["PersonId"] == null)
                return RedirectToAction("logIn", "Accounts");
            try
            {
                string Ip = WebConfigurationManager.AppSettings["UniIp"];
                Models.UniAutomationEntities p = new Models.UniAutomationEntities();
                var date = p.sp_GetDate().FirstOrDefault();
                var sharj = p.sp_GetCharge(Convert.ToInt32(Session["PersonId"])).FirstOrDefault();
                var Mablagh = 0; var darsadtakhfif = 0;
                foreach (var item in ArrayL)
                {
                    var f = p.sp_tblFoodProgramingSelect("fldId", item.fldFoodProgramingID.ToString(), 0).FirstOrDefault();
                    var takhfif = p.sp_tblDarsadTakhfifSelect("ChekPersonTakhfif", Session["PersonId"].ToString(), f.fldFoodDate, 0).FirstOrDefault();
                    if (item.fldCount != 0)
                    {
                        var Ptype = p.sp_GetCartPersonType(Convert.ToInt32(Session["PersonId"])).FirstOrDefault();
                        if (Ptype.PrsonType == 1 || Ptype.PrsonType == 4)
                        {
                            var cart = p.sp_tblFoodCartsSelect("fldId", Session["PersonId"].ToString(), 0).FirstOrDefault();
                            var s = p.sp_tblStudentSelect("fldId", cart.fldStudentID.ToString(), "", 0).FirstOrDefault();

                            //var q = p.sp_tblTariffSelect("fldDate_PrsonType", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), Ptype.PrsonType.ToString(), s.fldSectionID.ToString(), s.fldStatusID.ToString(), s.fldTermId.ToString(), 0).Where(h => h.fldRezervType == true /*&& h.fldFoodInfoID == f.fldFoodInfoID*/).FirstOrDefault();
                            // if (Ip == "78.38.152.102")
                             var q = p.sp_tblTariffSelect("fldDate_Food", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), Ptype.PrsonType.ToString(), s.fldSectionID.ToString(), s.fldStatusID.ToString(), s.fldTermId.ToString(), 0).Where(h => h.fldRezervType == true && h.fldFoodInfoID == f.fldFoodInfoID).FirstOrDefault();

                             
                            //var q = p.sp_tblTariffSelect("", "", 1).Where(h => h.fldRezervType == true).FirstOrDefault();

                            //Mablagh = Mablagh + q.fldPrice;
                            if (takhfif != null)
                                darsadtakhfif = takhfif.fldDarsadTakhfif;
                            Mablagh = Mablagh - (Mablagh * darsadtakhfif / 100);
                        }
                        else
                        {
                            var q = p.sp_tblTariffSelect("fldDate_PrsonType", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), Ptype.PrsonType.ToString(), "", "", "", 0).Where(h => h.fldRezervType == true && h.fldUserType == Ptype.PrsonType && h.fldFoodInfoID == f.fldFoodInfoID).FirstOrDefault();
                            //Mablagh = Mablagh + q.fldPrice;
                            if (takhfif != null)
                                darsadtakhfif = takhfif.fldDarsadTakhfif;
                            Mablagh = Mablagh - (Mablagh * darsadtakhfif / 100);
                        }
                    }
                }

                if (sharj.Charge >= Mablagh)
                {
                    foreach (var item in ArrayL)
                    {
                        if (item.fldCount != 0)
                        {
                            var f = p.sp_tblFoodProgramingSelect("fldId", item.fldFoodProgramingID.ToString(), 0).FirstOrDefault();
                            var takhfif = p.sp_tblDarsadTakhfifSelect("ChekPersonTakhfif", Session["PersonId"].ToString(), f.fldFoodDate, 0).FirstOrDefault();

                            var Ptype = p.sp_GetCartPersonType(Convert.ToInt32(Session["PersonId"])).FirstOrDefault();
                            if (Ptype.PrsonType == 1 || Ptype.PrsonType == 4)
                            {
                                var cart = p.sp_tblFoodCartsSelect("fldId", Session["PersonId"].ToString(), 0).FirstOrDefault();
                                var s = p.sp_tblStudentSelect("fldId", cart.fldStudentID.ToString(), "", 0).FirstOrDefault();

                                //var q = p.sp_tblTariffSelect("fldDate_PrsonType", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), Ptype.PrsonType.ToString(), s.fldSectionID.ToString(), s.fldStatusID.ToString(), s.fldTermId.ToString(), 0).Where(h => h.fldRezervType == true /*&& h.fldFoodInfoID == f.fldFoodInfoID*/).FirstOrDefault();
                                //if (Ip == "78.38.152.102")
                                 var q = p.sp_tblTariffSelect("fldDate_Food", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), Ptype.PrsonType.ToString(), s.fldSectionID.ToString(), s.fldStatusID.ToString(), s.fldTermId.ToString(), 0).Where(h => h.fldRezervType == true && h.fldFoodInfoID == f.fldFoodInfoID).FirstOrDefault();


                                var MablagheGhaza = q.fldPrice;
                                if (takhfif != null)
                                    darsadtakhfif = takhfif.fldDarsadTakhfif;
                                MablagheGhaza = MablagheGhaza - (MablagheGhaza * darsadtakhfif/100);

                                p.sp_tblRezervInsert(Convert.ToInt32(Session["PersonId"]), item.fldFoodProgramingID, item.fldCount, MablagheGhaza, darsadtakhfif, "");
                                p.sp_tblHistoryReserveInsert(Convert.ToInt32(Session["PersonId"]), item.fldFoodProgramingID, item.fldCount, MablagheGhaza, true, "");
                            }
                            else
                            {
                                var q = p.sp_tblTariffSelect("fldDate_PrsonType", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), Ptype.PrsonType.ToString(), "", "", "", 0).Where(h => h.fldRezervType == true && h.fldUserType == Ptype.PrsonType && h.fldFoodInfoID == f.fldFoodInfoID).FirstOrDefault();
                                var MablagheGhaza = q.fldPrice;
                                if (takhfif != null)
                                    darsadtakhfif = takhfif.fldDarsadTakhfif;
                                MablagheGhaza = MablagheGhaza - (MablagheGhaza * darsadtakhfif/100);

                                p.sp_tblRezervInsert(Convert.ToInt32(Session["PersonId"]), item.fldFoodProgramingID, item.fldCount, MablagheGhaza,darsadtakhfif, "");
                                p.sp_tblHistoryReserveInsert(Convert.ToInt32(Session["PersonId"]), item.fldFoodProgramingID, item.fldCount, q.fldPrice, true, "");
                            }
                        }
                    }
                    
                }
                else
                    return Json(new { data = "اعتبار شما برای رزرو غذا کافی نیست.", state = 1 });
                
                //foreach (var item in ArrayL)
                //{
                    
                //    if (item.fldCount != 0)
                //    {
                //        var Ptype = p.sp_GetCartPersonType(Convert.ToInt32(Session["PersonId"])).FirstOrDefault();
                //        var date = p.sp_GetDate().FirstOrDefault();
                //        var q = p.sp_tblTariffSelect("fldDate",MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), 0).Where(h => h.fldRezervType == true && h.fldUserType == Ptype.PrsonType).FirstOrDefault();
                //        //var q = p.sp_tblTariffSelect("", "", 1).Where(h => h.fldRezervType == true).FirstOrDefault();
                //        var sharj = p.sp_GetCharge(Convert.ToInt32(Session["PersonId"])).FirstOrDefault();
                //        if (sharj.Charge >= q.fldPrice * item.fldCount)
                //        {
                //            //int count = item.fldCount;
                //            //if (count > 1)
                //            //    count = 1;
                //            p.sp_tblRezervInsert(Convert.ToInt32(Session["PersonId"]), item.fldFoodProgramingID,item.fldCount, q.fldPrice, "");
                //        }
                //        else
                //            return Json(new { data = "اعتبار شما برای رزرو غذا کافی نیست.", state = 1 });
                //    }
                //}
                return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
    }
}
