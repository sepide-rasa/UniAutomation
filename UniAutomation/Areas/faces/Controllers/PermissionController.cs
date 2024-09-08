 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniAutomation.Areas.faces.Controllers.Acc;

namespace UniAutomation.Areas.faces.Controllers.Users
{
    [Authorize]
    public class PermissionController : Controller
    {
        //
        // GET: /Permission/

        public ActionResult Index()
        {//بارگذاری صفحه اصلی 
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 79))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }

        public JsonResult checkBox(int id)
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q1 = p.sp_tblPermissionSelect("fldGroupId", id, Convert.ToInt32(Session["UserId"])).ToList();
            int[] checkedNodes = new int[q1.Count];
            for (int i = 0; i < q1.Count; i++)
            {
                checkedNodes[i] = Convert.ToInt32(q1[i].fldApplicationPartID);
            }

            return Json(checkedNodes);
        }

        public JsonResult GetCascadeGroup()
        {
            Models.UniAutomationEntities1 car = new Models.UniAutomationEntities1();
            return Json(car.sp_tblUserGroupSelect("", "", 0).Select(c => new { fldID = c.fldID, fldName = c.fldTitle }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult _Rol(int? id)
        {
            var p = new Models.UniAutomationEntities1();
            if (id != null)
            {
                var rols = (from k in p.sp_tblApplicationPartSelect("fldPID", id.ToString(),0)
                            select new
                            {
                                id = k.fldID,
                                Name = k.fldTitle,
                                hasChildren = p.sp_tblApplicationPartSelect("fldPID", id.ToString(), 0).Any()

                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var rols = (from k in p.sp_tblApplicationPartSelect("", "", 0)
                            select new
                            {
                                id = k.fldID,
                               Name = k.fldTitle,
                                hasChildren = p.sp_tblApplicationPartSelect("", "", 0).Any()

                            });
                return Json(rols, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Save(int GroupId, List<Models.Permissions> checkedNodes)
        {
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 80))
                {
                    Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                    Car.sp_tblPermissionDelete(GroupId, Convert.ToInt32(Session["UserId"]));
                    for (int i = 0; i < checkedNodes.Count(); i++)
                    {
                        Car.sp_tblPermissionInsert(checkedNodes[i].GroupId, checkedNodes[i].RolId, Convert.ToInt32(Session["UserId"]), "");
                    }
                    return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                }
                else
                {
                    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                    return RedirectToAction("error", "Metro");
                }
            }
            catch (Exception x)
            {
                return Json(new { data = x.Message, state = 1 });
            }
        }
    }
}
