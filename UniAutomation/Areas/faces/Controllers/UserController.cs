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
    public class UserController : Controller
    {
        //
        // GET: /faces/User/

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("logon", "Account");
            if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 83))
            {
                return PartialView();
            }
            else
            {
                Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                return RedirectToAction("error", "Metro");
            }
        }
        public JsonResult GetCascadeGroup()
        {
            Models.UniAutomationEntities1 q = new Models.UniAutomationEntities1();
            var w = q.sp_tblUserGroupSelect("", "", 0).Select(c => new { fldID = c.fldID, fldName = c.fldTitle });
            return Json(w, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Fill([DataSourceRequest] DataSourceRequest request)
        {
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_UserSelect("", "", 30,"").ToList().ToDataSourceResult(request);
            return Json(q);
        }
        public ActionResult Save(Models.User User, string[] _checked)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                byte[] image = null;

                if (User.fldImage != null)
                    image = UniAutomation.Helper.ClsCommon.Base64ToImage(User.fldImage);
                if (User.fldDesc == null)
                    User.fldDesc = "";
                if (User.fldPassword == null)
                    User.fldPassword = "";
                if (User.fldID == 0)
                {//ثبت رکورد جدید
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 84))
                    {
                        System.Data.Objects.ObjectParameter _id = new System.Data.Objects.ObjectParameter("fldID", typeof(int));

                        p.sp_tblUserInsert(_id, User.fldName, User.fldFamily, User.fldUserName,  Class1.GenerateHash(User.fldPassword), Convert.ToBoolean(User.fldStatus), Convert.ToInt32(Session["UserId"]), "");
                        p.sp_tblPicturesInsert(null, null, null, Convert.ToInt32(_id.Value), image, Convert.ToInt32(Session["UserId"]), User.fldDesc);
                        if (_checked != null)
                        {
                            for (int i = 0; i < _checked.Count(); i++)
                            {
                                p.sp_tblUser_GroupInsert(Convert.ToInt32(_checked[i]), Convert.ToInt32(_id.Value), Convert.ToInt32(Session["UserId"]), "");
                            }
                        }
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
                    if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 85))
                    {
                        var k = p.sp_tblPicturesSelect("fldUser_Id", User.fldID.ToString(), 1).FirstOrDefault();
                        if (k != null)
                            p.sp_tblPicturesUpdate(k.fldID, null, null, null, User.fldID, image, Convert.ToInt32(Session["UserId"]), User.fldDesc);
                        else
                            p.sp_tblPicturesInsert(null, null, null, User.fldID, image, Convert.ToInt32(Session["UserId"]), User.fldDesc);

                        p.sp_tblUserUpdate(User.fldID, User.fldName, User.fldFamily, User.fldUserName, Class1.GenerateHash(User.fldPassword), Convert.ToBoolean(User.fldStatus), Convert.ToInt32(Session["UserId"]), "");
                        p.sp_tblUser_GroupDelete(Convert.ToInt32(User.fldID), Convert.ToInt32(Session["UserId"]));
                        if (_checked != null)
                        {
                            for (int i = 0; i < _checked.Count(); i++)
                            {
                                p.sp_tblUser_GroupInsert(Convert.ToInt32(_checked[i]), User.fldID, Convert.ToInt32(Session["UserId"]), "");
                            }
                        }
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
            string[] _fiald = new string[] { "fldName" };
            string[] searchType = new string[] { "%{0}%", "{0}%", "{0}" };
            string searchtext = string.Format(searchType[searchtype], value);
            Models.UniAutomationEntities1 m = new Models.UniAutomationEntities1();
            var q = m.sp_UserSelect(_fiald[Convert.ToInt32(field)], searchtext, top, "").ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {//حذف یک رکورد
            try
            {
                if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 86))
                {
                    Models.UniAutomationEntities1 Car = new Models.UniAutomationEntities1();
                    if (Convert.ToInt32(id) != 0)
                    {
                        Car.sp_tblUserDelete(Convert.ToInt32(id), Convert.ToInt32(Session["UserId"]));
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
                var q = p.sp_UserSelect("fldId", id.ToString(), 1, "").FirstOrDefault();
                var _checked = p.sp_tblUser_GroupSelect("fldUserSelectID", q.fldID.ToString(), 0).ToList();
                int[] checkedNodes = new int[_checked.Count];
                for (int i = 0; i < _checked.Count; i++)
                {
                    checkedNodes[i] = Convert.ToInt32(_checked[i].fldUserGroupID);
                }
                var k = p.sp_tblPicturesSelect("fldUser_Id", q.fldID.ToString(), 1).FirstOrDefault();
                var picId = "";
                if (k != null)
                    picId = k.fldID.ToString();
                return Json(new
                {
                    fldID = q.fldID,
                    fldUserName = q.fldUserName,
                    fldName = q.fldName,
                    fldPassword = "",
                    fldFamily = q.fldFamily,
                    fldActive_Deactive = q.fldStatus,
                    fldDesc = q.fldDesc,
                    checkedNodes = checkedNodes,
                    fldpicId = picId    

                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public FileContentResult Image(int id)
        {//برگرداندن عکس 
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();

            var pic = p.sp_tblPicturesSelect("fldID", id.ToString(), 1).FirstOrDefault();
            if (pic != null)
            {
                if (pic.fldImage != null)
                {
                    return File((byte[])pic.fldImage, "jpg");
                }

            }
            return null;

        }
        public JsonResult Reset(int id)
        {
            try
            {
                Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
                var q = p.sp_tblUserSelect("fldId", id.ToString(), 0, "").FirstOrDefault();
                if (q != null)
                {
                    p.sp_UserPassUpdate(id, Class1.GenerateHash(q.fldUserName));
                    return Json(new { data = "تغییر رمز با موفقیت انجام شد.", state = 0 });
                }
                return Json(new { data = "کاربری با این مشخصات وجود ندارد.", state = 1 });

            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
    }
}
