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
    public class PersonelController : Controller
    {
        //
        // GET: /faces/Personel/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 5))
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
            var q = m.sp_tblPersonalSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Save(Models.Personal Personal)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();

                byte[] image = null;

                if (Personal.fldPic != null)
                    image = UniAutomation.Helper.ClsCommon.Base64ToImage(Personal.fldPic);


                if (Personal.fldDesc == null)
                    Personal.fldDesc = "";
                if (Personal.fldEmail == null)
                    Personal.fldEmail = "";
                if (Personal.fldMobile == null)
                    Personal.fldMobile = "";
                if (Personal.fldID == 0)
                {//ثبت رکورد جدید
                     if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 29))
                    {

                        System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldID", typeof(int));
                    p.sp_tblPersonalInsert(_id, Personal.fldName, Personal.fldFamily, Personal.fldMeliCode, Personal.fldPost, Personal.fldMobile, Personal.fldEmail, Convert.ToInt32(Session["UserId"]), Personal.fldDesc);
                    p.sp_tblPicturesInsert(null, null, Convert.ToInt32(_id.Value), null, image, Convert.ToInt32(Session["UserId"]), Personal.fldDesc);
                    return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                    }
                     else
                     {
                         Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                         return RedirectToAction("error", "Metro");
                     }

                }
                else
                {//ویرایش رکورد ارسالی 
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 30))
                    {

                    var k = p.sp_tblPicturesSelect("fldPersonalID", Personal.fldID.ToString(), 1).FirstOrDefault();
                    if (k != null)
                        p.sp_tblPicturesUpdate(k.fldID, null, null, Personal.fldID, null, image, Convert.ToInt32(Session["UserId"]), Personal.fldDesc);
                    else
                        p.sp_tblPicturesInsert(null, null, Personal.fldID, null, image, Convert.ToInt32(Session["UserId"]), Personal.fldDesc);
                    p.sp_tblPersonalUpdate(Personal.fldID, Personal.fldName, Personal.fldFamily, Personal.fldMeliCode, Personal.fldPost, Personal.fldMobile, Personal.fldEmail, Convert.ToInt32(Session["UserId"]), Personal.fldDesc);
                    return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
                    }
                    else
                    {
                        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");
                    }
                }
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }

        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldFamily", "fldName" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblPersonalSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 31))
                {

                Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                if (Convert.ToInt32(id) != 0)
                {
                    Car.sp_tblPersonalDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_tblPersonalSelect("fldId", id.ToString(), 1).FirstOrDefault();
                var k = p.sp_tblPicturesSelect("fldPersonalID", id.ToString(), 1).FirstOrDefault();
                var picId = "";
                if (k != null)
                    picId = k.fldID.ToString();
                return Json(new
                {
                    fldID = q.fldID,
                    fldName = q.fldName,
                    fldFamily = q.fldFamily,
                    fldPost = q.fldPost,
                    fldEmail = q.fldEmail,
                    fldMobile = q.fldMobile,
                    fldMelliCode = q.fldMelliCode,
                    fldDesc = q.fldDesc,
                    fldpic = k.fldID,
                    fldpicId = picId    
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }


        public FileContentResult PersonalImage(int id)
        {//برگرداندن عکس 
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();

            var pic = p.sp_tblPicturesSelect("fldId", id.ToString(), 1).FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldImage != null)
                {
                    return File((byte[])pic.fldImage, "jpg");
                }

            }
            return null;

        }
        public ActionResult rptKart(string PersonId)
        {
            // Session["date"] = date;
            UniAutomation.Areas.faces.DataSet.DataSet1 dt = new UniAutomation.Areas.faces.DataSet.DataSet1();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblPersonalSelectTableAdapter sp_Personel = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblPersonalSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblPicturesSelectTableAdapter sp_Pic = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblPicturesSelectTableAdapter();
            UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter sp_Setting = new UniAutomation.Areas.faces.DataSet.DataSet1TableAdapters.sp_tblSettingSelectTableAdapter();

            sp_Personel.Fill(dt.sp_tblPersonalSelect, "fldId", PersonId, 0);
            sp_Setting.Fill(dt.sp_tblSettingSelect);
            dt.sp_tblSettingSelect[0].fldName = Class1.stringDecode(dt.sp_tblSettingSelect[0].fldName);
            sp_Pic.Fill(dt.sp_tblPicturesSelect, "fldPersonalID", PersonId, 0);

            FastReport.Report Report = new FastReport.Report();

            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\Reports\KartPersonal.frx");

            Report.RegisterData(dt, "uniAutomationDataSet");
            Report.SetParameterValue("date", MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));
            var time = DateTime.Now;
            //Report.SetParameterValue("University", "دانشکده فنی دختران شاهرود");
            //Report.SetParameterValue("TarikhSodoor", TarikhSodoor);
            //Report.SetParameterValue("TarikhEtebar", TarikhEtebar);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);


            return File(stream.ToArray(), "application/pdf");
        }
    }
}
