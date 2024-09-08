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
    public class B_tblRegulationsController : Controller
    {
        //
        // GET: /faces/B_tblRegulations/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 164))
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
            var q = m.sp_B_tblRegulationsSelect("", "", 30).ToList().ToDataSourceResult(request);
            return Json(q);
        }

        public ActionResult Upload()
        {
            var file = Request.Files["Filedata"];
            string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
            file.SaveAs(savePath);
            Session["savePath"] = savePath;
            return Content(Url.Content(@"~\Uploaded\" + file.FileName));
        }

        public ActionResult Save(Models.sp_B_tblRegulationsSelect Regulation)
        {
            try
            {
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                if (Regulation.fldDesc == null)
                    Regulation.fldDesc = "";
                if (Regulation.fldCode == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 165))
                    {

                        System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldCode", typeof(int));

                        byte[] report_file = null;
                        if (Session["savePath"] != null)
                        {
                            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                            System.IO.File.Delete(Session["savePath"].ToString());
                            Session.Remove("savePath");
                            report_file = stream.ToArray();
                            m.sp_B_tblRegulationsInsert(_id, Regulation.fldTitle, Regulation.fldRating, Convert.ToInt32(Session["UserId"]), Regulation.fldDesc);
                            m.sp_tblContentInsert(report_file, Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserId"]), Regulation.fldDesc);
                            return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });
                        }
                        else
                            return Json(new { data = "لطفا فایل آیین نامه را وارد کنید.", state = 1 });

                    }
                    else
                    {
                        Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                        return RedirectToAction("error", "Metro");
                    }

                }
                else
                {//ویرایش رکورد ارسالی
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 166))
                    {
                        byte[] report_file = null;
                        if (Session["savePath"] != null)
                        {
                            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                            System.IO.File.Delete(Session["savePath"].ToString());
                            Session.Remove("savePath");
                            report_file = stream.ToArray();
                            m.sp_B_tblRegulationsUpdate(Regulation.fldCode, Regulation.fldTitle, Regulation.fldRating, Convert.ToInt32(Session["UserId"]), Regulation.fldDesc);
                            var k = m.sp_tblContentSelect("fldRegulationsId", Regulation.fldCode.ToString(), 1).FirstOrDefault();
                            if (k == null)
                                m.sp_tblContentInsert(report_file, Regulation.fldCode, Convert.ToInt32(Session["UserId"]), Regulation.fldDesc);
                            else
                                m.sp_tblContentUpdate(k.fldId, report_file, Regulation.fldCode, Convert.ToInt32(Session["UserId"]), Regulation.fldDesc);

                            return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
                        }
                        else
                        {
                            var k = m.sp_tblContentSelect("fldRegulationsId", Regulation.fldCode.ToString(), 1).FirstOrDefault();
                            if (k == null)
                                return Json(new { data = "لطفا فایل آیین نامه را وارد کنید.", state = 1 });
                            else
                            {
                                m.sp_tblContentUpdate(k.fldId, k.fldContent, Regulation.fldCode, Convert.ToInt32(Session["UserId"]), Regulation.fldDesc);
                                return Json(new { data = "ویرایش با موفقیت انجام شد.", state = 0 });
                            }
                        }
                        
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
            string[] _fiald = new string[] { "fldTitle", "fldRating" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_B_tblRegulationsSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 167))
                {
                    Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        var q = m.sp_tblContentSelect("fldRegulationsId", id, 0).FirstOrDefault();
                        m.sp_tblContentDelete(q.fldId, Convert.ToInt32(Session["UserId"]));

                        m.sp_B_tblRegulationsDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
                Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
                var q = m.sp_B_tblRegulationsSelect("fldCode", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    fldCode = q.fldCode,
                    fldTitle = q.fldTitle,
                    fldRating = q.fldRating,
                    fldDesc = q.fldDesc
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }

        public ActionResult GeneratePDF(int? id)
        {
            try
            {
                byte[] report_file = null;

                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var Content = p.sp_tblContentSelect("fldRegulationsId", id.ToString(), 0).FirstOrDefault();

                if (Content != null)
                    return File(Content.fldContent, "application/pdf");

                else if (Session["savePath"] != null)
                {
                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                    report_file = stream.ToArray();
                    return File(report_file, "application/pdf");
                }

                else
                    return null;
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UploadContent(HttpPostedFileBase UpContent)
        {
            if (UpContent != null)
            {
                // Some browsers send file names with full path.
                // We are only interested in the file name.
                var fileName = Path.GetFileName(UpContent.FileName);
                string savePath = Server.MapPath(@"~\Uploaded\" + fileName);
                Session["savePath"] = savePath;
                // The files are not actually saved in this demo
                UpContent.SaveAs(savePath);

                ViewBag.FileName = fileName;

            }
            return Content("");
        }
        public ActionResult RemoveContent(string fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                string physicalPath = Server.MapPath(@"~\Uploaded\" + fileNames);
                if (System.IO.File.Exists(physicalPath))
                {
                    // The files are not actually removed in this demo
                    System.IO.File.Delete(physicalPath);
                }
                Session.Remove("savePath");
            }
            return Content("");
        }

        public ActionResult DeleteContent(string id)
        {
            try
            {
                Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                var q = Car.sp_tblContentSelect("fldRegulationsId", id, 0).FirstOrDefault();
                Car.sp_tblContentDelete(q.fldId, 1);
                return Json(new { data = "حذف با موفقیت انجام شد.", state = 0 });

            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
    }
}
