using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.IO;

namespace UniAutomation.Areas.faces.Controllers
{
    public class CovertPersonelInfoController : Controller
    {
        //
        // GET: /faces/CovertPersonelInfo/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            return PartialView();

        }

        public ActionResult Upload()
        {
            var file = Request.Files["Filedata"];
            string filePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
            file.SaveAs(filePath);
            Session["filePath"] = filePath;
            file.SaveAs(filePath);

            return Content("");
        }
        public ActionResult UploadContent(HttpPostedFileBase UpContent)
        {
            if (UpContent != null)
            {
                // Some browsers send file names with full path.
                // We are only interested in the file name.
                var fileName = Path.GetFileName(UpContent.FileName);
                string savePath = Server.MapPath(@"~\Uploaded\" + fileName);
                Session["filePath"] = savePath;
                // The files are not actually saved in this demo
                UpContent.SaveAs(savePath);

                ViewBag.FileName = fileName;

            }
            return Content("");
        }
        public ActionResult Remove(string fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                string physicalPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.IO.Path.GetFileName(fileNames));
                if (System.IO.File.Exists(physicalPath))
                {
                    // The files are not actually removed in this demo
                    System.IO.File.Delete(physicalPath);
                }
                Session.Remove("filePath");
            }
            return Content("");
        }

        public ActionResult ImageUpload()
        {
            foreach (string inputTagName in Request.Files)
            {
                var file = Request.Files[inputTagName];
                string filePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                file.SaveAs(filePath);


                //HttpPostedFileBase Infile = Request.Files[inputTagName];
                //string ImagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.IO.Path.GetFileName(Infile.FileName));
                //var k = ImagePath.LastIndexOf("\\");
                //ImagePath = ImagePath.Substring(0,k+1);
                //Session["ImagePath"] = ImagePath;

            }
            return Content("");
        }
        public ActionResult UploadImage(HttpPostedFileBase UpImage)
        {
            if (UpImage != null)
            {
                // Some browsers send file names with full path.
                // We are only interested in the file name.
                var fileName = Path.GetFileName(UpImage.FileName);
                string savePath = Server.MapPath(@"~\Uploaded\" + fileName);
                Session["filePath"] = savePath;
                // The files are not actually saved in this demo
                UpImage.SaveAs(savePath);

                ViewBag.FileName = fileName;

            }
            return Content("");
        }
        public ActionResult RemoveImage(string ImageNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (ImageNames != null)
            {
                System.IO.File.Delete(Server.MapPath(@"~\Uploaded\" + ImageNames));
            }
            return Content("");
        }

        public ActionResult SaveTeachers()
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            string physicalPath = "";

            if (Session["filePath"] != null)
            {
                //FileStream fstream = new FileStream(Session["filePath"].ToString(), FileMode.Open);
                //Workbook workbook = new Workbook(fstream);
                //Worksheet worksheet = workbook.Worksheets[0];
                //int a=worksheet.Cells.Count;

                System.Data.OleDb.OleDbConnection oconn = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Session["filePath"].ToString() + ";Extended Properties=\"Excel 8.0;HDR=YES\";");
                oconn.Open();


                if (oconn.State == System.Data.ConnectionState.Closed)
                    oconn.Open();



                try
                {
                    System.Data.OleDb.OleDbCommand ocmd = new System.Data.OleDb.OleDbCommand("select * from [Sheet1$]", oconn);
                    System.Data.OleDb.OleDbDataReader odr = ocmd.ExecuteReader();

                    byte? cid = 0;

                    if (odr.HasRows)
                    {
                        while (odr.Read())
                        {
                            System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldID", typeof(int));

                            if (odr["reshte"].ToString() == "")
                                cid = null;
                            else
                            {
                                var Course = m.sp_tblCourseSelect("fldName", odr["reshte"].ToString(), 1).FirstOrDefault();
                                if (Course != null)
                                    cid = Course.fldID;
                                else
                                {
                                    oconn.Close();
                                    physicalPath = System.IO.Path.Combine(Session["filePath"].ToString());
                                    System.IO.File.Delete(physicalPath);
                                    return Json(new { data = "رشته ای با عنوان " + odr["reshte"].ToString() + " تعریف نشده است.", state = 1 });
                                }
                            }

                            var stu = m.sp_tblTeachersSelect("fldMelliCode", odr["codemeli"].ToString(), 0).FirstOrDefault();
                            if (stu == null)
                            {
                                //string Cod = odr["shd"].ToString();
                                //Cod = Cod.Substring(8, 6);
                                m.sp_tblTeachersInsert(_id, odr["name"].ToString(), odr["family"].ToString(), odr["codemeli"].ToString(),cid, odr["mobile"].ToString(),
                                    odr["email"].ToString(), Convert.ToInt32(Session["UserId"]), "");

                                //var ImgPath = Server.MapPath(@"~\Uploaded\" + odr["codemeli"].ToString() + ".jpg");
                                //if (!System.IO.File.Exists(ImgPath))
                                var ImgPath = Server.MapPath(@"~\Content\images\Blank.jpg");

                                MemoryStream image = new MemoryStream(System.IO.File.ReadAllBytes(ImgPath));

                                m.sp_tblPicturesInsert( null,Convert.ToInt32(_id.Value), null, null, image.ToArray(), Convert.ToInt32(Session["UserId"]), "");
                                m.sp_tblFoodCartsInsert(null, Convert.ToInt32(_id.Value), null, odr["codemeli"].ToString(), Class1.GenerateHash(odr["codemeli"].ToString()), 1, Convert.ToInt32(Session["UserId"]), "", "");

                                // System.IO.File.Delete(Server.MapPath(@"~\Uploaded\" + odr["codemeli"].ToString() + ".jpg"));
                            }
                            else
                            {
                                m.sp_tblTeachersUpdate(stu.fldID, stu.fldName, stu.fldFamily, stu.fldMelliCode, stu.fldCourseID, odr["mobile"].ToString(),
                                    odr["email"].ToString(), Convert.ToInt32(Session["UserId"]), "");

                                var ImgPath = Server.MapPath(@"~\Uploaded\" + odr["codemeli"].ToString() + ".jpg");
                                if (!System.IO.File.Exists(ImgPath))
                                    ImgPath = Server.MapPath(@"~\Content\images\Blank.jpg");

                                MemoryStream image = new MemoryStream(System.IO.File.ReadAllBytes(ImgPath));

                                var Pic = m.sp_tblPicturesSelect("fldTeacherID", stu.fldID.ToString(), 0).FirstOrDefault();
                                m.sp_tblPicturesUpdate(Pic.fldID,null, stu.fldID,  null, null, image.ToArray(), Convert.ToInt32(Session["UserId"]), "");
                                //    //var food = m.sp_tblFoodCartsSelect("fldStudentID", stu.fldID.ToString(), 0).FirstOrDefault();
                                //    //m.sp_tblFoodCartsUpdate(food.fldID, stu.fldID, null, null, stu.fldStudentNumber.ToString(), food.fldPassword, 1, Convert.ToInt32(Session["UserId"]), "", food.fldRFID);

                                System.IO.File.Delete(Server.MapPath(@"~\Uploaded\" + odr["codemeli"].ToString() + ".jpg"));
                            }
                        }
                    }
                    //fstream.Close();
                    oconn.Close();
                }
                catch (Exception ex)
                {
                    Session["ER"] = ex.Message;
                    return RedirectToAction("error", "Metro");
                }
            }
            //}


            physicalPath = System.IO.Path.Combine(Session["filePath"].ToString());
            System.IO.File.Delete(physicalPath);
            return Json(new { data = "ذخیره با موفقیت انجام شد.", state = 0 });

        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblTeachersSelect("", "",30).ToList().ToDataSourceResult(request);
            return Json(q);
        }


        public ActionResult Reload(string field, string value, int top, int searchtype)
        {//جستجو
            string[] _fiald = new string[] { "fldFamily", "fldName" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_tblTeachersSelect(_fiald[Convert.ToInt32(field)], searchtext, top).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Details(int id)
        {//نمایش اطلاعات جهت رویت کاربر
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_tblTeachersSelect("fldId", id.ToString(), 1).FirstOrDefault();
                return Json(new
                {
                    //fldId = q.fldId,
                    //fldTitle = q.fldTitle,
                    //fldDesc = q.fldDesc
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }

    }
}
