using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class EFormsController : BaseController
    {
        private readonly ClsMail clsMail = new ClsMail();
        private readonly ClsManageEForm data = new ClsManageEForm();

        public ActionResult _FileCollections(Guid id)
        {
            ClsEForm clsEForm = new ClsEForm
            {
                EForm_Galleries = db.EForm_Galleries.Where(w => w.EForm_Id == id).OrderBy(o => o.EForm_Gallery_Seq).ToList(),

                EForm_Files = db.EForm_Files.Where(w => w.EForm_Id == id).OrderBy(o => o.EForm_File_Seq).ToList()
            };

            return View(clsEForm);
        }

        public ActionResult Approve()
        {
            return View();
        }

        public async Task<ActionResult> Approve_Forms(Guid id, bool? res = null)
        {
            try
            {
                ClsSwal swal = new ClsSwal();
                EForms eForms = new EForms();

                if (res == true)
                {
                    eForms = db.EForms.Find(id);
                    eForms.Status_Id = 3;
                    eForms.ActionUserId = loginId;

                    swal.DangerMode = false;
                    swal.Icon = "success";
                    swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                    swal.Title = "Successful";

                    db.SaveChanges();

                    string status = db.System_Statuses.Find(eForms.Status_Id).Status_Name.ToString();

                    await EmailForms(id, status);

                    return Json(swal, JsonRequestBehavior.AllowGet);
                }
                else if (res == false)
                {
                    eForms = db.EForms.Find(id);
                    eForms.Status_Id = 6;
                    eForms.ActionUserId = loginId;

                    swal.DangerMode = false;
                    swal.Icon = "success";
                    swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                    swal.Title = "Successful";

                    db.SaveChanges();

                    string status = db.System_Statuses.Find(eForms.Status_Id).Status_Name.ToString();

                    await EmailForms(id, status);

                    return Json(swal, JsonRequestBehavior.AllowGet);
                }

                ClsEForm clsEForm = new ClsEForm
                {
                    EForms = db.EForms.Find(id),
                    EForm_Files = db.EForm_Files.Where(w => w.EForm_Id == id).ToList(),
                    EForm_Galleries = db.EForm_Galleries.Where(w => w.EForm_Id == id).ToList()
                };

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

        [HttpDelete]
        public async Task<ActionResult> Delete_EForm(Guid id)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                ClsSwal swal = new ClsSwal();
                try
                {
                    if (await data.Delete_Attached(id))
                    {
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "ลบข้อมูลไม่สำเร็จ";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.GetBaseException().Message;
                }

                return Json(swal, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> DeleteFiles(Guid id)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.DeleteFile(id))
                    {
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "ลบไฟล์สำเร็จ";
                        swal.Title = "Successful";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.GetBaseException().Message;
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> DeleteGallery(Guid id)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.DeleteGallery(id))
                    {
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "ลบไฟล์สำเร็จ";
                        swal.Title = "Successful";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.GetBaseException().Message;
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EForms_Content(Guid? id)
        {
            ViewBag.Usercode = db.Users
                .Where(w => w.User_Id == loginId)
                .Select(s => s.User_Code)
                .FirstOrDefault();

            ClsEForm clsEForm = new ClsEForm();

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
                EForms eForms = new EForms
                {
                    EForm_Start = DateTime.Now
                };

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
        public async Task<ActionResult> EForms_Create(EForms model)
        {
            ClsSwal swal = new ClsSwal();
            if (ModelState.IsValid)
            {
                TransactionOptions options = new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = TimeSpan.MaxValue
                };
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options, TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        if (await data.EForm_Save(model, Request.Files))
                        {
                            scope.Complete();

                            swal.DangerMode = false;
                            swal.Icon = "success";
                            swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.Title = "Successful";
                        }
                        else
                        {
                            swal.Icon = "warning";
                            swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                            swal.Title = "Warning";
                        }
                    }
                    catch (Exception ex)
                    {
                        swal.Title = ex.Source;
                        swal.Text = ex.GetBaseException().Message;
                    }
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                   .Where(y => y.Count > 0)
                                   .ToList();
                swal.Icon = "warning";
                swal.Title = "Warning";
                foreach (var item in errors)
                {
                    foreach (var item2 in item)
                    {
                        if (string.IsNullOrEmpty(swal.Text))
                        {
                            swal.Text = item2.ErrorMessage;
                        }
                        else
                        {
                            swal.Text += "\n" + item2.ErrorMessage;
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
                ViewBag.Usercode = db.Users
                    .Where(w => w.User_Id == loginId)
                    .Select(s => s.User_Code)
                    .FirstOrDefault();

                ViewBag.MyForm = false;

                IQueryable<EForms> query = db.EForms.OrderByDescending(o => new { o.Update, o.Create }).ThenBy(t => t.EForm_Start);

                if (res == 2)
                {
                    query = db.EForms.Where(w => w.User_Id == loginId).OrderByDescending(o => o.Create);
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
                ViewBag.RoleId = db.Users.Where(w => w.User_Id == loginId).Select(s => s.Role_Id).FirstOrDefault();

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

        public async Task EmailForms(Guid id, string status)
        {
            try
            {
                EForms eForms = db.EForms.Find(id);

                Guid sendTo = eForms.User_Id;
                var linkUrl = System.Web.HttpContext.Current.Request.Url.OriginalString;
                linkUrl += "/" + eForms.EForm_Id;
                linkUrl = linkUrl.Replace("Approve_Forms", "EForms_Content");
                linkUrl = linkUrl.Split('?').FirstOrDefault();

                string subject = string.Format("[" + status + "] {0} - {1}", "E-Forms", eForms.EForm_Title);
                string content = string.Format("<p><b>Description:</b> {0}", eForms.EForm_Description);
                content += "<br />";
                content += "<br />";
                content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                content += "<p>Thank you for your consideration</p>";
                clsMail.SendToId = sendTo;
                clsMail.Subject = subject;
                clsMail.Body = content;
                await clsMail.SendMail(clsMail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: EForms
        public ActionResult Index()
        {
            ViewBag.Categories = SelectListItems_Category_Name();
            return View();
        }

        public ActionResult ReloadModel(Guid id)
        {
            ClsEForm clsEForm = new ClsEForm
            {
                EForm_Galleries = db.EForm_Galleries.Where(w => w.EForm_Id == id).ToList(),

                EForm_Files = db.EForm_Files.Where(w => w.EForm_Id == id).ToList()
            };

            return View(clsEForm);
        }

        public ActionResult SaveSeq(string model)
        {
            List<EForm_Galleries> eForm_Galleries = new List<EForm_Galleries>();
            eForm_Galleries = JsonConvert.DeserializeObject<List<EForm_Galleries>>(model);

            ClsSwal swal = new ClsSwal();
            if (eForm_Galleries != null)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Galleries_SaveSeq(eForm_Galleries))
                        {
                            scope.Complete();

                            swal.DangerMode = false;
                            swal.Icon = "success";
                            swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.Title = "Successful";
                        }
                        else
                        {
                            swal.Icon = "warning";
                            swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                            swal.Title = "Warning";
                        }
                    }
                    catch (Exception ex)
                    {
                        swal.Title = ex.Source;
                        swal.Text = ex.GetBaseException().Message;
                    }
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                   .Where(y => y.Count > 0)
                                   .ToList();
                swal.Icon = "warning";
                swal.Title = "Warning";
                foreach (var item in errors)
                {
                    foreach (var item2 in item)
                    {
                        if (string.IsNullOrEmpty(swal.Text))
                        {
                            swal.Text = item2.ErrorMessage;
                        }
                        else
                        {
                            swal.Text += "\n" + item2.ErrorMessage;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public List<SelectListItem> SelectListItems_Category_Name()
        {
            var DeptDistinct = db.Master_Departments.Select(s => s.Department_Name).Distinct();
            List<SelectListItem> item = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Select Category", Value = "" }
            };

            item.AddRange(DeptDistinct.Select(s => new SelectListItem()
            {
                Text = s,
                Value = db.Master_Departments.Where(w => w.Department_Name == s).Select(s2 => s2.Department_Id).FirstOrDefault().ToString()
            }
            ));

            return item;
        }
    }
}
