using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using UniAutomation.Areas.faces.Controllers.Acc;
using System.IO;

namespace UniAutomation.Areas.faces.Controllers
{
    public class RptEnterDormController : Controller
    {
        //
        // GET: /faces/RptEnterDorm/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
                return PartialView();
            
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblEnterDorm_SarbargSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

    

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldSemester", "fldTarikh" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblEnterDorm_SarbargSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

    
        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_B_tblEnterDorm_SarbargSelect("fldId", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldID = q.fldId,
                    fldTarikh = q.fldTarikh,
                    fldSemester = q.fldSemester,
                    fldUserId = q.fldUserId,
                    fldDesc = q.fldDesc
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }

        public ActionResult _RptEnterDorm(int id)
        {
            //Session["SarbargId"] = id;
            var SarbargId = id;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_B_tblEnterDorm_SarbargSelectTableAdapter sp_Sarbarg = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_B_tblEnterDorm_SarbargSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_B_tblEnterDormSelectTableAdapter sp_detail = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_B_tblEnterDormSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_Sarbarg.Fill(dt.sp_B_tblEnterDorm_SarbargSelect, "fldId", SarbargId.ToString(), 0);
            sp_detail.Fill(dt.sp_B_tblEnterDormSelect, "fldSarbargId", SarbargId.ToString(), "", 0);
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptEnterDorm.frx");
            Report.RegisterData(dt, "uniAutomationDataSet");

            Report.Prepare();
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }

    }
}
