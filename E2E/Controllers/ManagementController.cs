﻿using E2E.Models;
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
    public class ManagementController : Controller
    {
        private clsManageManagement data = new clsManageManagement();
        private clsContext db = new clsContext();

        public ActionResult Delete_DocumentControl_Create(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                clsSwal swal = new clsSwal();
                try
                {
                    if (data.Delete_Document(id))
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
                        swal.text = "ข้อมูลถูกใช้งานอยู่";
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

        public ActionResult DocumentControl()
        {
            return View();
        }

        public ActionResult DocumentControl_Create(Guid? id)
        {
            ViewBag.IsNew = true;
            clsDocuments clsDocuments = new clsDocuments();
            if (id.HasValue)
            {
                ViewBag.IsNew = false;
                clsDocuments.Master_Documents = db.Master_Documents.Find(id);
                clsDocuments.Master_DocumentVersions = db.Master_DocumentVersions.Where(w => w.Document_Id == id).OrderByDescending(o => o.DocumentVersion_Number).ToList();
            }
            return View(clsDocuments);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DocumentControl_Create(clsDocuments model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Document_Save(model, Request.Files))
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

        public ActionResult DocumentControl_Table()
        {
            var sql = db.Master_Documents.ToList();
            return View(sql);
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult WorkRoot()
        {
            return View();
        }

        public ActionResult WorkRoot_Form(Guid? id)
        {
            try
            {
                clsWorkRoots clsWorkRoots = new clsWorkRoots();
                bool isNew = true;
                Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                string deptName = db.Users
                    .Where(w => w.User_Id == userId)
                    .Select(s => s.Master_Processes.Master_Sections.Master_Departments.Department_Name)
                    .FirstOrDefault();

                List<Guid> deptIds = db.Master_Departments
                    .Where(w => w.Department_Name == deptName)
                    .Select(s => s.Department_Id)
                    .ToList();

                List<Guid> userIds = db.Users
                    .Where(w => deptIds.Contains(w.Master_Processes.Master_Sections.Department_Id.Value))
                    .Select(s => s.User_Id)
                    .ToList();

                List<SelectListItem> listItems = new List<SelectListItem>();
                listItems.Add(new SelectListItem()
                {
                    Text = "Select documents",
                    Value = ""
                });
                ViewBag.DocumentList = db.Master_Documents
                    .Where(w => userIds.Contains(w.User_Id) && w.Active)
                    .Select(s => new SelectListItem()
                    {
                        Text = s.Document_Name,
                        Value = s.Document_Id.ToString()
                    })
                    .OrderBy(o => o.Text)
                    .ToList();

                List<string> secNames = db.Master_Sections
                    .Where(w => deptIds.Contains(w.Department_Id.Value) && w.Active)
                    .Select(s => s.Section_Name)
                    .OrderBy(o => o)
                    .Distinct()
                    .ToList();

                listItems = new List<SelectListItem>();
                listItems.Add(new SelectListItem()
                {
                    Text = "Select section",
                    Value = ""
                });
                foreach (string item in secNames)
                {
                    listItems.Add(db.Master_Sections
                        .Where(w => w.Section_Name == item)
                        .Select(s => new SelectListItem()
                        {
                            Value = s.Section_Id.ToString(),
                            Text = s.Section_Name
                        }).FirstOrDefault());
                }

                ViewBag.SectionList = listItems;

                if (id.HasValue)
                {
                    isNew = false;
                    clsWorkRoots.WorkRoots = db.WorkRoots.Find(id);
                    clsWorkRoots.WorkRootDocuments = db.WorkRootDocuments
                        .Where(w => w.WorkRoot_Id == id.Value)
                        .ToList();
                    if (clsWorkRoots.WorkRootDocuments.Count > 0)
                    {
                        clsWorkRoots.Document_Id = clsWorkRoots.WorkRootDocuments.Select(s => s.Document_Id).ToList();
                    }
                }

                ViewBag.IsNew = isNew;

                return View(clsWorkRoots);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WorkRoot_Form(clsWorkRoots model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.WorkRoot_Save(model))
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
                    catch (Exception ex)
                    {
                        swal.title = ex.TargetSite.Name;
                        swal.text = ex.Message;
                        var inner = ex.InnerException;
                        while (inner == null)
                        {
                            swal.text += "\n" + inner.Message;
                            inner = inner.InnerException;
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

        public ActionResult WorkRoot_Table()
        {
            try
            {
                Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                string deptName = db.Users
                    .Where(w => w.User_Id == userId)
                    .Select(s => s.Master_Processes.Master_Sections.Master_Departments.Department_Name)
                    .FirstOrDefault();

                List<Guid> deptIds = db.Master_Departments
                    .Where(w => w.Department_Name == deptName)
                    .Select(s => s.Department_Id)
                    .ToList();

                List<Guid> userIds = db.Users
                    .Where(w => deptIds.Contains(w.Master_Processes.Master_Sections.Department_Id.Value))
                    .Select(s => s.User_Id)
                    .ToList();

                List<WorkRoots> workRoots = new List<WorkRoots>();
                workRoots = db.WorkRoots
                    .Where(w => userIds.Contains(w.User_Id.Value))
                    .ToList();

                return View(workRoots);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}