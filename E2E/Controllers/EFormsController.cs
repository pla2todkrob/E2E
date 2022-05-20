using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class EFormsController : Controller
    {
        private clsManageEForm data = new clsManageEForm();
        private clsContext db = new clsContext();
        private clsServiceFTP ftp = new clsServiceFTP();
        // GET: EForms
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EForms_Table(int res)
        {
            try
            {
                Guid id = Guid.Parse(HttpContext.User.Identity.Name);
                ViewBag.Usercode = db.Users
                    .Where(w => w.User_Id == id)
                    .Select(s => s.User_Code)
                    .FirstOrDefault();

                ViewBag.MyForm = false;

                IQueryable<EForms> query = db.EForms.OrderByDescending(o => new { o.Update, o.Create }).ThenBy(t=>t.EForm_Start) ;

                if (res == 1)
                {
                    query = query.Where(w => (w.EForm_Start <= DateTime.Today && w.EForm_End >= DateTime.Today)||(w.EForm_Start <= DateTime.Today && !w.EForm_End.HasValue));
                }
                if (res == 2)
                {
                    query = db.EForms.Where(w => w.User_Id == id);
                    ViewBag.MyForm = true;
                }
                if (res == 3)
                {
                    query = db.EForms.Where(w => w.EForm_End < DateTime.Today);
                }

                return View(query.ToList());
            }
            catch (Exception)
            {
                throw;
            }
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
        public ActionResult EForms_Create(clsEForm model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.EForm_Save(model.EForms, Request.Files))
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

        public ActionResult _FileCollections(Guid id)
        {
            clsEForm clsEForm = new clsEForm();

            clsEForm.EForm_Galleries = db.EForm_Galleries.Where(w => w.EForm_Id == id).ToList();

            clsEForm.EForm_Files = db.EForm_Files.Where(w => w.EForm_Id == id).ToList();

            return View(clsEForm);
        }

        public ActionResult ReloadModel(Guid id)
        {
            clsEForm clsEForm = new clsEForm();

            clsEForm.EForm_Galleries = db.EForm_Galleries.Where(w => w.EForm_Id == id).ToList();

            clsEForm.EForm_Files = db.EForm_Files.Where(w => w.EForm_Id == id).ToList();

            return View(clsEForm);
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
    }
}