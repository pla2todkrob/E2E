using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class EFormsController : Controller
    {
        private clsManageEForm data = new clsManageEForm();
        private clsContext db = new clsContext();
        private clsServiceFTP ftp = new clsServiceFTP();
        private clsMail mail = new clsMail();

        public ActionResult _FileCollections(Guid id)
        {
            clsEForm clsEForm = new clsEForm();

            clsEForm.EForm_Galleries = db.EForm_Galleries.Where(w => w.EForm_Id == id).ToList();

            clsEForm.EForm_Files = db.EForm_Files.Where(w => w.EForm_Id == id).ToList();

            return View(clsEForm);
        }

        public ActionResult Approve()
        {
            return View();
        }

        public ActionResult Approve_Forms(Guid id, bool? res = null)
        {
            try
            {
                clsSwal swal = new clsSwal();
                EForms eForms = new EForms();

                if (res == true)
                {
                    eForms = db.EForms.Find(id);
                    eForms.Status_Id = 3;
                    eForms.ActionUserId = Guid.Parse(HttpContext.User.Identity.Name);

                    swal.dangerMode = false;
                    swal.icon = "success";
                    swal.text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                    swal.title = "Successful";

                    db.SaveChanges();

                    string status = db.System_Statuses.Find(eForms.Status_Id).Status_Name.ToString();

                    EmailForms(id, status);

                    return Json(swal, JsonRequestBehavior.AllowGet);
                }
                else if (res == false)
                {
                    eForms = db.EForms.Find(id);
                    eForms.Status_Id = 6;
                    eForms.ActionUserId = Guid.Parse(HttpContext.User.Identity.Name);

                    swal.dangerMode = false;
                    swal.icon = "success";
                    swal.text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                    swal.title = "Successful";

                    db.SaveChanges();

                    string status = db.System_Statuses.Find(eForms.Status_Id).Status_Name.ToString();

                    EmailForms(id, status);

                    return Json(swal, JsonRequestBehavior.AllowGet);
                }

                clsEForm clsEForm = new clsEForm();
                clsEForm.EForms = db.EForms.Find(id);
                clsEForm.EForm_Files = db.EForm_Files.Where(w => w.EForm_Id == id).ToList();
                clsEForm.EForm_Galleries = db.EForm_Galleries.Where(w => w.EForm_Id == id).ToList();

                return View(clsEForm);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Approve_Table_Approved()
        {
            try
            {
                return View(data.EForm_GetRequiredApprove(3));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Approve_Table_Waiting()
        {
            try
            {
                return View(data.EForm_GetRequiredApprove(1));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Delete_EForm(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                clsSwal swal = new clsSwal();
                try
                {
                    if (data.Delete_Attached(id))
                    {
                        scope.Complete();
                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.title = "Successful";
                    }
                    else
                    {
                        swal.icon = "warning";
                        swal.text = "ลบข้อมูลไม่สำเร็จ";
                        swal.title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.title = ex.TargetSite.Name;
                    swal.text = ex.Message;
                    if (ex.InnerException != null)
                    {
                        swal.text = ex.InnerException.Message;
                        if (ex.InnerException.InnerException != null)
                        {
                            swal.text = ex.InnerException.InnerException.Message;
                        }
                    }
                }

                return Json(swal, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteFiles(Guid id)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.DeleteFile(id))
                    {
                        scope.Complete();
                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "ลบไฟล์สำเร็จ";
                        swal.title = "Successful";
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    swal.title = ex.TargetSite.Name;
                    foreach (var item in ex.EntityValidationErrors)
                    {
                        foreach (var item2 in item.ValidationErrors)
                        {
                            if (string.IsNullOrEmpty(swal.text))
                            {
                                swal.text = item2.ErrorMessage;
                            }
                            else
                            {
                                swal.text += "\n" + item2.ErrorMessage;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    swal.title = ex.TargetSite.Name;
                    swal.text = ex.Message;
                    if (ex.InnerException != null)
                    {
                        swal.text = ex.InnerException.Message;
                        if (ex.InnerException.InnerException != null)
                        {
                            swal.text = ex.InnerException.InnerException.Message;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteGallery(Guid id)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.DeleteGallery(id))
                    {
                        scope.Complete();
                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "ลบไฟล์สำเร็จ";
                        swal.title = "Successful";
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    swal.title = ex.TargetSite.Name;
                    foreach (var item in ex.EntityValidationErrors)
                    {
                        foreach (var item2 in item.ValidationErrors)
                        {
                            if (string.IsNullOrEmpty(swal.text))
                            {
                                swal.text = item2.ErrorMessage;
                            }
                            else
                            {
                                swal.text += "\n" + item2.ErrorMessage;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    swal.title = ex.TargetSite.Name;
                    swal.text = ex.Message;
                    if (ex.InnerException != null)
                    {
                        swal.text = ex.InnerException.Message;
                        if (ex.InnerException.InnerException != null)
                        {
                            swal.text = ex.InnerException.InnerException.Message;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EForms_Content(Guid? id)
        {
            Guid id_emp = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.Usercode = db.Users
                .Where(w => w.User_Id == id_emp)
                .Select(s => s.User_Code)
                .FirstOrDefault();

            clsEForm clsEForm = new clsEForm();

            if (id.HasValue)
            {
                clsEForm.EForms = db.EForms.Where(w => w.EForm_Id == id).FirstOrDefault();
                clsEForm.EForm_Galleries = db.EForm_Galleries.Where(w => w.EForm_Id == id).ToList();
                clsEForm.EForm_Files = db.EForm_Files.Where(w => w.EForm_Id == id).ToList();
            }

            return View(clsEForm);
        }

        public ActionResult EForms_Create(Guid? id)
        {
            try
            {
                EForms eForms = new EForms();
                eForms.EForm_Start = DateTime.Now;

                bool isNew = true;
                if (id.HasValue)
                {
                    eForms = db.EForms.Find(id);
                    isNew = false;
                }
                ViewBag.isNew = isNew;

                return View(eForms);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EForms_Create(EForms model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.EForm_Save(model, Request.Files))
                        {
                            scope.Complete();

                            swal.dangerMode = false;
                            swal.icon = "success";
                            swal.text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.title = "Successful";
                        }
                        else
                        {
                            swal.icon = "warning";
                            swal.text = "บันทึกข้อมูลไม่สำเร็จ";
                            swal.title = "Warning";
                        }
                    }
                    catch (DbEntityValidationException ex)
                    {
                        swal.title = ex.TargetSite.Name;
                        foreach (var item in ex.EntityValidationErrors)
                        {
                            foreach (var item2 in item.ValidationErrors)
                            {
                                if (string.IsNullOrEmpty(swal.text))
                                {
                                    swal.text = item2.ErrorMessage;
                                }
                                else
                                {
                                    swal.text += "\n" + item2.ErrorMessage;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        swal.title = ex.TargetSite.Name;
                        swal.text = ex.Message;
                        if (ex.InnerException != null)
                        {
                            swal.text = ex.InnerException.Message;
                            if (ex.InnerException.InnerException != null)
                            {
                                swal.text = ex.InnerException.InnerException.Message;
                            }
                        }
                    }
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                   .Where(y => y.Count > 0)
                                   .ToList();
                swal.icon = "warning";
                swal.title = "Warning";
                foreach (var item in errors)
                {
                    foreach (var item2 in item)
                    {
                        if (string.IsNullOrEmpty(swal.text))
                        {
                            swal.text = item2.ErrorMessage;
                        }
                        else
                        {
                            swal.text += "\n" + item2.ErrorMessage;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EForms_MyForms(int? res = 2)
        {
            try
            {
                Guid id = Guid.Parse(HttpContext.User.Identity.Name);
                ViewBag.Usercode = db.Users
                    .Where(w => w.User_Id == id)
                    .Select(s => s.User_Code)
                    .FirstOrDefault();

                ViewBag.MyForm = false;

                IQueryable<EForms> query = db.EForms.OrderByDescending(o => new { o.Update, o.Create }).ThenBy(t => t.EForm_Start);

                if (res == 2)
                {
                    query = db.EForms.Where(w => w.User_Id == id).OrderByDescending(o => o.Create);
                    ViewBag.MyForm = true;
                }
                //if (res == 3)
                //{
                //    query = db.EForms.Where(w => w.EForm_End < DateTime.Today);
                //}

                return View(query.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult EForms_Table(Guid? id)
        {
            try
            {
                Guid UserId = Guid.Parse(HttpContext.User.Identity.Name);
                ViewBag.RoleId = db.Users.Where(w => w.User_Id == UserId).Select(s => s.Role_Id).FirstOrDefault();

                IQueryable<EForms> query = db.EForms.Where(w => w.EForm_Start <= DateTime.Today && w.Status_Id == 3 && (!w.EForm_End.HasValue || w.EForm_End >= DateTime.Today)).OrderByDescending(o => new { o.Update, o.Create }).ThenBy(t => t.EForm_Start);

                if (id.HasValue)
                {
                    var DeptName = db.Master_Departments.Find(id).Department_Name;

                    var DeptUser = db.Users.Where(w => w.Master_Processes.Master_Sections.Master_Departments.Department_Name == DeptName).Select(s => s.User_Id).ToList();

                    //var FormAll = db.EForms.Where(w => DeptUser.Contains(w.User_Id)).ToList();

                    query = query.Where(w => DeptUser.Contains(w.User_Id));
                    //var categorys = db.Master_Categories.Where(w => w.Category_Id == category).FirstOrDefault();
                    //query = query.Where(w => w.Category_Id == categorys.Category_Id);
                }

                return View(query.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void EmailForms(Guid id, string status)
        {
            EForms eForms = new EForms();
            eForms = db.EForms.Find(id);

            string deptName = db.Users.Find(eForms.User_Id).Master_Processes.Master_Sections.Master_Departments.Department_Name;
            Guid sendTo = eForms.User_Id;
            var linkUrl = System.Web.HttpContext.Current.Request.Url.OriginalString;
            linkUrl += "/" + eForms.EForm_Id;
            linkUrl = linkUrl.Replace("Approve_Forms", "EForms_Content");
            linkUrl = linkUrl.Split('?').FirstOrDefault();

            string subject = string.Format("[E2E][" + status + "] {0} - {1}", "E-Forms", eForms.EForm_Title);
            string content = string.Format("<p><b>Description:</b> {0}", eForms.EForm_Description);
            content += "<br />";
            content += "<br />";
            content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
            content += "<p>Thank you for your consideration</p>";
            mail.SendMail(sendTo: sendTo, strSubject: subject, strContent: content);
        }

        // GET: EForms
        public ActionResult Index()
        {
            ViewBag.Categories = SelectListItems_Category_Name();
            return View();
        }

        public ActionResult ReloadModel(Guid id)
        {
            clsEForm clsEForm = new clsEForm();

            clsEForm.EForm_Galleries = db.EForm_Galleries.Where(w => w.EForm_Id == id).ToList();

            clsEForm.EForm_Files = db.EForm_Files.Where(w => w.EForm_Id == id).ToList();

            return View(clsEForm);
        }

        public ActionResult SaveSeq(string model)
        {
            List<EForm_Galleries> eForm_Galleries = new List<EForm_Galleries>();
            eForm_Galleries = JsonConvert.DeserializeObject<List<EForm_Galleries>>(model);

            clsSwal swal = new clsSwal();
            if (eForm_Galleries != null)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Galleries_SaveSeq(eForm_Galleries))
                        {
                            scope.Complete();

                            swal.dangerMode = false;
                            swal.icon = "success";
                            swal.text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.title = "Successful";
                        }
                        else
                        {
                            swal.icon = "warning";
                            swal.text = "บันทึกข้อมูลไม่สำเร็จ";
                            swal.title = "Warning";
                        }
                    }
                    catch (DbEntityValidationException ex)
                    {
                        swal.title = ex.TargetSite.Name;
                        foreach (var item in ex.EntityValidationErrors)
                        {
                            foreach (var item2 in item.ValidationErrors)
                            {
                                if (string.IsNullOrEmpty(swal.text))
                                {
                                    swal.text = item2.ErrorMessage;
                                }
                                else
                                {
                                    swal.text += "\n" + item2.ErrorMessage;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        swal.title = ex.TargetSite.Name;
                        swal.text = ex.Message;
                        if (ex.InnerException != null)
                        {
                            swal.text = ex.InnerException.Message;
                            if (ex.InnerException.InnerException != null)
                            {
                                swal.text = ex.InnerException.InnerException.Message;
                            }
                        }
                    }
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                   .Where(y => y.Count > 0)
                                   .ToList();
                swal.icon = "warning";
                swal.title = "Warning";
                foreach (var item in errors)
                {
                    foreach (var item2 in item)
                    {
                        if (string.IsNullOrEmpty(swal.text))
                        {
                            swal.text = item2.ErrorMessage;
                        }
                        else
                        {
                            swal.text += "\n" + item2.ErrorMessage;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public List<SelectListItem> SelectListItems_Category_Name()
        {
            Guid UserId = Guid.Parse(HttpContext.User.Identity.Name);
            var DeptDistinct = db.Master_Departments.Select(s => s.Department_Name).ToList().Distinct();
            List<SelectListItem> item = new List<SelectListItem>();
            item.Add(new SelectListItem() { Text = "Select Category", Value = "" });
            foreach (var item1 in DeptDistinct)
            {
                var sql = db.Master_Departments.Where(w => w.Department_Name == item1)
                    .Select(s => new SelectListItem()
                    {
                        Value = s.Department_Id.ToString(),
                        Text = s.Department_Name
                    })
                    .FirstOrDefault();
                item.Add(sql);
            }

            return item;
        }
    }
}
