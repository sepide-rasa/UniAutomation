using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;


namespace UniAutomation.Areas.faces.Controllers
{
    public class BildingRoomController : Controller
    {
        //
        // GET: /faces/BildingRoom/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblBuildingRoomsSelect("", "","", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] {"fldNameRoom" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblBuildingRoomsSelect(_fiald[Convert.ToInt32(field)], searchtext,"", top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_B_tblBuildingRoomsSelect("fldID", id.ToString(),"", 1).FirstOrDefault();
                return Json(new
                {

                    fldId = q.fldRoomCode,
                    fldPersonalId = q.fldBuildingCodeId,
                    fldStudentId = q.fldNameRoom,
                    fldTeacherId = q.fldCapacity,
                    fldDesc = q.fldDesc
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                if (Convert.ToInt32(id) != 0)
                {
                    m.sp_B_tblBuildingRoomsDelete(Convert.ToInt32(id), 1);
                    return Json(new { data = "حذف با موفقیت انجام شد.", state = 0 });
                }
                else
                {
                    return Json(new { data = "رکوردی برای حذف انتخاب نشده است.", state = 1 });
                }

            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
    }
}
