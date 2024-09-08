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
    public class RezervGoruhiController : Controller
    {
        //
        // GET: /faces/RezervGoruhi/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 17))
            {
            return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }


        public ActionResult GetGroups()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblEducationGroupSelect("", "", 0).ToList().Select(c => new { fldID = c.fldID, fldName = c.fldName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSelfService()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblSelfServiceSelect("", "", 0).ToList().Select(c => new { fldID = c.fldID, fldName = c.fldName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTerm()
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_B_tblTermsSelect("", "", 0).OrderByDescending(k => k.fldId).ToList().Select(c => new { fldID = c.fldId, fldName = c.fldTermName });
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInf(string d)
        {

            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblFoodProgramingSelect("fldFoodDate", d, 1).FirstOrDefault();

            return Json(new
            {
                id = q.fldID,
                FoodName = q.fldFoodName


            }, JsonRequestBehavior.AllowGet);

        }

        //public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        //{
        //    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
        //    var q = m.sp_GroupChargeSelect().ToList().ToDataSourceResult(request);

        //    return Json(q);
        //}

        public ActionResult Save(List<Models.RezervG> RezervG)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                string ch = "";
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 65))
                {
                    foreach (var item in RezervG)
                    {
                        if (item.fldDesc == null)
                            item.fldDesc = "";

                        var Ptype = p.sp_GetCartPersonType(Convert.ToInt32(item.fldFoodCartsID)).FirstOrDefault();
                        var date = p.sp_GetDate().FirstOrDefault();
                        var q = p.sp_tblTariffSelect("fldDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime), "", "", "", "", 0).Where(h => h.fldRezervType == true && h.fldUserType == Ptype.PrsonType).FirstOrDefault();
                        var dateReserv = p.sp_tblFoodProgramingSelect("fldId", item.fldFoodProgramingID.ToString(), 0).FirstOrDefault().fldFoodDate;
                        var takhfif = p.sp_tblDarsadTakhfifSelect("ChekPersonTakhfif", item.fldFoodCartsID.ToString(), dateReserv, 0).FirstOrDefault();
                        var darsadtakhfif = 0;
                        var MablagheGhaza = q.fldPrice;
                        if (takhfif != null)
                            darsadtakhfif = takhfif.fldDarsadTakhfif;
                        MablagheGhaza = MablagheGhaza - (MablagheGhaza * darsadtakhfif/100);

                        if (item.fldReservId == 0)
                        {
                            if (item.fldCount != 0)
                            {
                                //var q = p.sp_tblTariffSelect("", "", 1).Where(h => h.fldRezervType == true).FirstOrDefault();
                                var sharj = p.sp_GetCharge(Convert.ToInt32(item.fldFoodCartsID)).FirstOrDefault();
                                if (sharj.Charge >= q.fldPrice)
                                {
                                    p.sp_tblRezervInsert(item.fldFoodCartsID, item.fldFoodProgramingID, item.fldCount, MablagheGhaza, darsadtakhfif, item.fldDesc);
                                    p.sp_tblHistoryReserveInsert(item.fldFoodCartsID, item.fldFoodProgramingID, item.fldCount, MablagheGhaza, true, "");
                                }
                                else
                                {
                                    var q1 = p.sp_tblFoodCartsSelect("fldId", item.fldFoodCartsID.ToString(), 0).FirstOrDefault();
                                    ch += q1.fldName + " " + q1.fldFamily + "(" + q1.fldUserName + ");";
                                }
                            }
                        }
                        else
                        {
                            p.sp_tblRezervUpdate(item.fldID, item.fldFoodCartsID, item.fldFoodProgramingID, item.fldCount, MablagheGhaza, darsadtakhfif, item.fldDesc);
                        }
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

        public ActionResult Reload(string self, string date, string nobat, string Group, int TermId)
        {//جستجو

            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_ReservGroupSelect(MyLib.Shamsi.Shamsi2miladiDateTime(date), Convert.ToInt32(self), Convert.ToByte(nobat), Convert.ToInt32(Group),TermId).ToList();
            int idfood = 0;

            var foodid = m.sp_tblFoodProgramingSelect("fldFoodDate", date, 0).Where(h => h.fldNobat == Convert.ToInt32(nobat)).FirstOrDefault();
            if (foodid != null)
                idfood = foodid.fldID;

            return Json(new { data = q, foodid = idfood }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 67))
                {
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_tblRezervDelete(Convert.ToByte(id));
                        return Json(new { data = "حذف با موفقیت انجام شد.", state = 0 });
                    }
                    else
                    {
                        return Json(new { data = "رکوردی برای حذف انتخاب نشده است.", state = 1 });
                    }
                }
                else
                {
                    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                    return RedirectToAction("error", "Metro");
                }

            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }

    }
}
