using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;


namespace UniAutomation.Areas.faces.Controllers
{
    public class SearchBookSelectController : Controller
    {
        //
        // GET: /faces/SearchBookSelect/

        public ActionResult Index(int id)
        {
            ViewBag.state = id;
            Session["field"] = "";
            Session["Value"] = "";
            Session["top"] = "0";
            Session["CategoryId"] = "1";
            Session.Remove("Books");
            return PartialView();
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblL_BookSelect(Session["field"].ToString(), Session["Value"].ToString(), Convert.ToInt32(Session["top"])).Where(k=>k.fldCategoryId==Convert.ToInt32(Session["CategoryId"])).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Reload(string field, string value, int top, int searchtype, int CategoryId)
        {//جستجو
            string[] _fiald = new string[] { "fldBookTitle", "", "fldPublisher", "fldAuthor", "fldInterpreter" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            //Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            //var q = m.sp_tblL_BookSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            //return Json(q, JsonRequestBehavior.AllowGet);
            Session["field"] = _fiald[Convert.ToInt32(field)];
            Session["Value"] = searchtext;
            Session["top"] = top;
            Session["CategoryId"] = CategoryId;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SelectBook(string BookId)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            List<Models.sp_tblL_BookSelect> groups = new List<Models.sp_tblL_BookSelect>();

            if (Session["Books"] != null)
                groups = (List<Models.sp_tblL_BookSelect>)Session["Books"];

            var K = BookId.Split(';');
            for (byte i = 0; i < K.Length - 1; i++)
            {
                foreach (var Item in groups)
                {
                    if (Item.fldId == Convert.ToInt32(K[i]))
                    {
                        Delete(Item.fldId);
                        groups = (List<Models.sp_tblL_BookSelect>)Session["Books"];
                    }
                }

           
                var q = m.sp_tblL_BookSelect("fldId", K[i].ToString(), 1).FirstOrDefault();
                Models.sp_tblL_BookSelect V = new Models.sp_tblL_BookSelect();
                V.fldId = q.fldId;
                V.fldBookTitle = q.fldBookTitle;
                V.fldPublisher = q.fldPublisher;
                V.fldAuthor = q.fldAuthor;
                V.fldInterpreter = q.fldInterpreter;
                groups.Add(V);
            }
            Session["Books"] = groups;
            return Json(groups, JsonRequestBehavior.AllowGet);
            //}
        }
        public ActionResult Delete(int id)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            List<Models.sp_tblL_BookSelect> groups = new List<Models.sp_tblL_BookSelect>();
            List<Models.sp_tblL_BookSelect> Copy = new List<Models.sp_tblL_BookSelect>();
            Models.sp_tblL_BookSelect V = new Models.sp_tblL_BookSelect();
            if (Session["Books"] != null)
            {
                Copy = (List<Models.sp_tblL_BookSelect>)Session["Books"];
            }

            foreach (var Item in Copy)
            {
                if (Item.fldId != id)
                    groups.Add(Item);
            }

            Session["Books"] = groups;
            return Json(groups.ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}
