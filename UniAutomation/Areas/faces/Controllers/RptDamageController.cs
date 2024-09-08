using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.IO;
using UniAutomation.Areas.faces.Controllers.Acc;
namespace UniAutomation.Areas.faces.Controllers
{
    public class RptDamageController : Controller
    {
        //
        // GET: /faces/RptDamage/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
                Session["field"] = "";
                Session["Value"] = "";
                Session["top"] = "30";
                return PartialView();
            

        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblDamagePersonSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }



        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldStudentCodeId", "fldDamageDate" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblDamagePersonSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_B_tblDamagePersonSelect("fldDamageCode", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldID = q.fldDamageCode,
                    fldStudentNumber = q.fldStudentNumber,
                    fldStudentCodeId=q.fldStudentCodeId,
                    fldDamageDate = q.fldDamageDate,
                    fldEqName = q.fldEqName,
                    fldUserId = q.fldUserId,
                    fldDesc = q.fldDesc
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }

        }
        public ActionResult _RptDamage(int type,string value)
        {
            //Session["Type"] = type;
            //Session["Value"] = value;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt2 = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_B_tblDamagePersonSelectTableAdapter sp_Damage = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_B_tblDamagePersonSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            string Type = "fldStudentCodeId";
            if (Convert.ToInt32(type) == 1)
                Type = "fldDamageDate";
            sp_Damage.Fill(dt2.sp_B_tblDamagePersonSelect, Type, value.ToString(), 0);
            sp_Setting.Fill(dt2.sp_tblSettingSelect);
            dt2.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt2.sp_tblSettingSelect[0].fldName);

            FastReport.Report Report = new FastReport.Report();
            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptDamage.frx");
            Report.RegisterData(dt2, "uniAutomationDataSet");

            Report.Prepare();
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }
    }
}
