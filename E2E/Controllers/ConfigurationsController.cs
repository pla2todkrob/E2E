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
    [AllowAnonymous]
    public class ConfigurationsController : Controller
    {
        private clsContext db = new clsContext();
        private clsServiceFTP ftp = new clsServiceFTP();
        private clsManageMaster master = new clsManageMaster();
        private clsManageService data = new clsManageService();

        public ActionResult _Copyright()
        {
            System_Configurations system_Configurations = new System_Configurations();

            system_Configurations = db.System_Configurations.OrderByDescending(o => o.CreateDateTime).FirstOrDefault();
            return PartialView("_Copyright", system_Configurations);
        }

        public ActionResult _Navbar()
        {
            clsCountNavbar res = new clsCountNavbar();

            res.Admin = null;

            res.ChangeDue = data.ServiceChangeDues_List().Count;

            if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                Guid id = Guid.Parse(HttpContext.User.Identity.Name);
                res.Admin = db.Users
                    .Where(w => w.User_Id == id)
                    .Select(s => s.Role_Id)
                    .FirstOrDefault();

                ViewBag.sql = data.ServiceChangeDues_List().Count;
            }

            return PartialView("_Navbar", res);
        }

        public ActionResult _NavbarBrand()
        {
            System_Configurations system_Configurations = new System_Configurations();
            system_Configurations = db.System_Configurations
                .OrderByDescending(o => o.CreateDateTime)
                .FirstOrDefault();

            return PartialView("_NavbarBrand", system_Configurations);
        }

        public ActionResult _NavDepartment()
        {
            try
            {
                int? res = null;
                if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
                {
                    Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                    Guid deptId = db.Users.Find(userId).Master_Processes.Master_Sections.Department_Id;

                    int authorIndex = db.Users
                        .Where(w => w.User_Id == userId)
                        .Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id)
                        .FirstOrDefault();

                    if (authorIndex != 3)
                    {
                        res = db.Services
                            .Where(w => w.Is_MustBeApproved &&
                            !w.Is_Approval &&
                            w.Status_Id == 1 &&
                            w.Users.Master_Processes.Master_Sections.Department_Id == deptId).Count();
                    }
                }

                return PartialView("_NavDepartment", res);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult _NavEForms()
        {
            try
            {
                int? res = null;

                if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
                {
                    Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                    Guid deptId = db.Users.Find(userId).Master_Processes.Master_Sections.Department_Id;

                    int authorIndex = db.Users
                        .Where(w => w.User_Id == userId)
                        .Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id)
                        .FirstOrDefault();

                    var val = db.UserDetails.Where(w => w.User_Id == userId).Select(s => s.Users.Master_Grades.Master_LineWorks.Authorize_Id).FirstOrDefault();

                    ViewBag.Author = val;
                    string deptName = db.Users.Find(userId).Master_Processes.Master_Sections.Master_Departments.Department_Name;
                    List<Guid> userIdList = db.Users
                        .Where(w => w.Master_Processes.Master_Sections.Master_Departments.Department_Name == deptName).Select(s => s.User_Id).ToList();
                    res = db.EForms.Where(w => w.Status_Id == 1 && userIdList.Contains(w.User_Id)).ToList().Count();
                }

                return PartialView("_NavEForms", res);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult _NavManagement()
        {
            bool res = new bool();
            Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
            try
            {
                int authur = db.Users
                    .Where(w => w.User_Id == userId)
                    .Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id)
                    .FirstOrDefault();
                if (authur == 2)
                {
                    res = true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return PartialView("_NavManagement", res);
        }

        public ActionResult _NavService()
        {
            try
            {
                int? res = null;
                if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
                {
                    Guid userId = Guid.Parse(HttpContext.User.Identity.Name);

                    int authorIndex = db.Users
                        .Where(w => w.User_Id == userId)
                        .Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id)
                        .FirstOrDefault();

                    if (authorIndex == 3)
                    {
                        res = new clsManageService().Services_GetWaitActionCount(Guid.Parse(HttpContext.User.Identity.Name));
                    }
                    else
                    {
                        res = new clsManageService().Services_GetWaitCommitCount();
                        res += new clsManageService().Services_GetWaitActionCount(Guid.Parse(HttpContext.User.Identity.Name));
                    }
                }
                return PartialView("_NavService", res);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult _Profile()
        {
            try
            {
                clsUsers clsUsers = new clsUsers();
                if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
                {
                    Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                    clsUsers = db.Users
                        .Where(w => w.User_Id == userId)
                        .AsEnumerable()
                        .Select(s => new clsUsers()
                        {
                            User_Code = s.User_Code,
                            User_Point = s.User_Point
                        }).FirstOrDefault();
                }

                return PartialView("_Profile", clsUsers);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Configurations_Table()
        {
            var system_Configurations = db.System_Configurations.OrderByDescending(o => o.CreateDateTime).ToList();
            system_Configurations.ForEach(f => f.Users.User_Code = master.Users_GetInfomation(f.User_Id));
            return View(system_Configurations);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Configurations_Table(System_Configurations model)
        {
            clsSwal swal = new clsSwal();
            bool res = new bool();
            HttpFileCollectionBase files = Request.Files;
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        System_Configurations system_Configurations = new System_Configurations();

                        if (files[0].ContentLength != 0)
                        {
                            HttpPostedFileBase file = files[0];
                            string dir = "Configurations/" + system_Configurations.Configuration_Id;
                            string FileName = file.FileName;
                            string filepath = ftp.Ftp_UploadFileToString(dir, file, FileName);

                            system_Configurations.Configuration_Brand = filepath;
                        }
                        else
                        {
                            system_Configurations.Configuration_Brand = model.Configuration_Brand;
                        }
                        system_Configurations.Copyright = model.Copyright.Trim('©').Trim();
                        system_Configurations.User_Id = Guid.Parse(System.Web.HttpContext.Current.User.Identity.Name);
                        system_Configurations.Configuration_Point = model.Configuration_Point;
                        system_Configurations.SystemName = model.SystemName;

                        db.System_Configurations.Add(system_Configurations);
                        if (db.SaveChanges() > 0)
                        {
                            res = true;
                        }

                        if (res)
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

        public ActionResult deletelogo()
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    System_Configurations system_Configurations = new System_Configurations();

                    system_Configurations = db.System_Configurations.OrderByDescending(o => o.CreateDateTime).FirstOrDefault();

                    system_Configurations.Configuration_Brand = string.Empty;

                    if (db.SaveChanges() > 0)
                    {
                        scope.Complete();

                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "ลบข้อมูลเรียบร้อย";
                        swal.title = "Successful";
                    }
                    else
                    {
                        swal.icon = "warning";
                        swal.text = "ลบข้อมูลไม่สำเร็จ";
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

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}