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
    public class RezervGoruhiPersonelController : Controller
    {
        //
        // GET: /faces/RezervGoruhiPersonel/

        public ActionResult Index(string state)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 243))
            {
                Session["state"] = state;
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }     
        public ActionResult GetSelfService()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblSelfServiceSelect("", "", 0).ToList().Select(c => new { fldID = c.fldID, fldName = c.fldName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(List<Models.RezervG> RezervG)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                string ch = "";
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 244))
                {
                    foreach (var item in RezervG)
                    {
                        if (item.fldDesc == null)
                            item.fldDesc = "";

                        //if (item.fldReservId == 0)
                        //{
                            if (item.fldCount != 0)
                            {
                                var Ptype = p.sp_GetCartPersonType(Convert.ToInt32(item.fldFoodCartsID)).FirstOrDefault();
                                var date = p.sp_GetDate().FirstOrDefault();
                                var q = p.sp_tblTariffSelect("fldDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), "", "", "", "", 0).Where(h => h.fldRezervType == true && h.fldUserType == Ptype.PrsonType).FirstOrDefault();
                                //var q = p.sp_tblTariffSelect("", "", 1).Where(h => h.fldRezervType == true).FirstOrDefault();
                                var sharj = p.sp_GetCharge(Convert.ToInt32(item.fldFoodCartsID)).FirstOrDefault();

                                var dateReserv = p.sp_tblFoodProgramingSelect("fldId", item.fldFoodProgramingID.ToString(), 0).FirstOrDefault().fldFoodDate;
                                var takhfif = p.sp_tblDarsadTakhfifSelect("ChekPersonTakhfif", item.fldFoodCartsID.ToString(), dateReserv, 0).FirstOrDefault();
                                var darsadtakhfif = 0;
                                var MablagheGhaza = q.fldPrice;
                                if (takhfif != null)
                                    darsadtakhfif = takhfif.fldDarsadTakhfif;
                                MablagheGhaza = MablagheGhaza - (MablagheGhaza * darsadtakhfif/100);

                                if (sharj.Charge >= q.fldPrice)
                                {
                                    for (DateTime d = MyLib.Shamsi.Shamsi2miladiDateTime(item.fldFromDate); d <= MyLib.Shamsi.Shamsi2miladiDateTime(item.fldToDate); d = MyLib.Shamsi.Shamsi2miladiDateTime(MyLib.Shamsi.ShamsiAddDay(MyLib.Shamsi.Miladi2ShamsiString(d), 1)))
                                    {
                                        var foodid = p.sp_tblFoodProgramingSelect("fldFoodDate", MyLib.Shamsi.Miladi2ShamsiString(d), 0).Where(h => h.fldNobat == Convert.ToInt32(item.fldNobat)).FirstOrDefault();
                                        if (foodid != null)
                                        {
                                            var z=p.sp_tblRezervSelect("fldFoodCartID", item.fldFoodCartsID.ToString(), 0).Where(l => l.fldFoodProgramingID == foodid.fldID).FirstOrDefault();
                                            if (z == null)
                                            {
                                                p.sp_tblRezervInsert(item.fldFoodCartsID, foodid.fldID, item.fldCount, MablagheGhaza, darsadtakhfif, "");
                                                p.sp_tblHistoryReserveInsert(item.fldFoodCartsID, item.fldFoodProgramingID, item.fldCount, MablagheGhaza, true, "");
                                            }
                                            else
                                                p.sp_tblRezervUpdate(z.fldID, item.fldFoodCartsID, foodid.fldID, item.fldCount, MablagheGhaza, darsadtakhfif, "");
                                        }
                                    }
                                }
                                else
                                {
                                    var q1 = p.sp_tblFoodCartsSelect("fldId", item.fldFoodCartsID.ToString(), 0).FirstOrDefault();
                                    ch += q1.fldName + " " + q1.fldFamily + "(" + q1.fldUserName + ");";
                                }
                            }
                        //}
                        //else
                        //{
                        //    p.sp_tblRezervUpdate(item.fldID, item.fldFoodCartsID, item.fldFoodProgramingID, item.fldCount, item.fldPrice, item.fldDesc);
                        //}
                    }
                    return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0, ch = ch });
                }
                else
                {
                    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                    return RedirectToAction("error", "Metro");
                }
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1, ch = "" });
            }
        }

        public ActionResult Reload(string self, string nobat,string FromDate,string ToDate)
        {//جستجو

            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_ReservGroupPersonalSelect(Session["state"].ToString(), Convert.ToInt32(self), Convert.ToByte(nobat)).ToList();
            string NotFood = "";
            for (DateTime d = MyLib.Shamsi.Shamsi2miladiDateTime(FromDate); d <= MyLib.Shamsi.Shamsi2miladiDateTime(ToDate); d = MyLib.Shamsi.Shamsi2miladiDateTime(MyLib.Shamsi.ShamsiAddDay(MyLib.Shamsi.Miladi2ShamsiString(d), 1)))
            {
                var foodid = m.sp_tblFoodProgramingSelect("fldFoodDate", MyLib.Shamsi.Miladi2ShamsiString(d), 0).Where(h => h.fldNobat == Convert.ToInt32(nobat)).FirstOrDefault();
                if (foodid == null)
                    NotFood = NotFood+MyLib.Shamsi.Miladi2ShamsiString(d)+"،";                
            }

            return Json(new { data = q, NotFood = NotFood }, JsonRequestBehavior.AllowGet);
        }
    }
}
