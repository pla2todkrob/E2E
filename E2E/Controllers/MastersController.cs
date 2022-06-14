﻿using E2E.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E2E.Models.Tables;
using E2E.Models.Views;
using System.Transactions;
using OfficeOpenXml;
using System.Data.Entity.Validation;
using System.IO;
using System.Net;

namespace E2E.Controllers
{
    public class MastersController : Controller
    {
        private clsManageMaster data = new clsManageMaster();
        private clsContext db = new clsContext();

        public ActionResult Departments()
        {
            return View();
        }

        public ActionResult Departments_Table()
        {
            return View(data.Department_GetAllView());
        }

        public ActionResult Departments_Form(Guid? id)
        {
            ViewBag.PlantList = data.SelectListItems_Plant();
            ViewBag.DivisionsList = new List<SelectListItem>();

            bool isNew = true;
            Master_Departments master_Departments = new Master_Departments();
            if (id.HasValue)
            {
                master_Departments = data.Department_Get(id.Value);
                isNew = false;
                ViewBag.DivisionsList = data.SelectListItems_Division(master_Departments.Master_Divisions.Plant_Id);
            }

            ViewBag.IsNew = isNew;

            return View(master_Departments);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Departments_Form(Master_Departments model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Department_Save(model))
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

        public ActionResult Departments_Delete(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                clsSwal swal = new clsSwal();
                try
                {
                    clsSaveResult clsSaveResult = data.Department_Delete(id);
                    if (clsSaveResult.CanSave)
                    {
                        scope.Complete();
                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.title = "Successful";
                    }
                    else
                    {
                        swal.text = clsSaveResult.Message;
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

        public ActionResult Divisions()
        {
            return View();
        }

        public ActionResult Divisions_Table()
        {
            return View(data.Division_GetAllView());
        }

        public ActionResult Divisions_Form(Guid? id)
        {
            ViewBag.PlantList = data.SelectListItems_Plant();

            bool isNew = true;
            Master_Divisions master_Divisions = new Master_Divisions();
            if (id.HasValue)
            {
                master_Divisions = data.Division_Get(id.Value);
                isNew = false;
            }

            ViewBag.IsNew = isNew;

            return View(master_Divisions);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Divisions_Form(Master_Divisions model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Division_Save(model))
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

        public ActionResult Divisions_Delete(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                clsSwal swal = new clsSwal();
                try
                {
                    clsSaveResult clsSaveResult = data.Division_Delete(id);
                    if (clsSaveResult.CanSave)
                    {
                        scope.Complete();
                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.title = "Successful";
                    }
                    else
                    {
                        swal.text = clsSaveResult.Message;
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

        public ActionResult Grades()
        {
            return View();
        }

        public ActionResult Grades_Form(Guid? id)
        {
            ViewBag.LineWorkList = data.SelectListItems_LineWork();

            bool isNew = true;
            Master_Grades master_Grades = new Master_Grades();
            if (id.HasValue)
            {
                master_Grades = data.Grades_Get(id.Value);
                isNew = false;
            }

            ViewBag.IsNew = isNew;

            return View(master_Grades);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Grades_Form(Master_Grades model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Grade_Save(model))
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

        public ActionResult Grades_Table()
        {
            return View(data.Grades_GetAllView());
        }

        public ActionResult Grades_Delete(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                clsSwal swal = new clsSwal();
                try
                {
                    clsSaveResult clsSaveResult = data.Grades_Delete(id);
                    if (clsSaveResult.CanSave)
                    {
                        scope.Complete();
                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.title = "Successful";
                    }
                    else
                    {
                        swal.text = clsSaveResult.Message;
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

        public ActionResult Index()
        {
            return RedirectToAction("LineWorks");
        }

        public ActionResult LineWorks()
        {
            return View();
        }

        public ActionResult LineWorks_Form(Guid? id)
        {
            ViewBag.AuthorizeList = data.SelectListItems_Authorize();

            bool isNew = true;
            Master_LineWorks master_LineWorks = new Master_LineWorks();
            if (id.HasValue)
            {
                master_LineWorks = data.LineWorks_Get(id.Value);
                isNew = false;
            }

            ViewBag.IsNew = isNew;

            return View(master_LineWorks);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LineWorks_Form(Master_LineWorks model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.LineWork_Save(model))
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

        public ActionResult LineWorks_Table()
        {
            return View(data.LineWorks_GetAll());
        }

        public ActionResult LineWorks_Delete(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                clsSwal swal = new clsSwal();
                try
                {
                    clsSaveResult clsSaveResult = data.Lineworks_Delete(id);
                    if (clsSaveResult.CanSave)
                    {
                        scope.Complete();
                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.title = "Successful";
                    }
                    else
                    {
                        swal.text = clsSaveResult.Message;
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

        public ActionResult Plants()
        {
            return View();
        }

        public ActionResult Plants_Form(Guid? id)
        {
            bool isNew = true;
            Master_Plants master_Plants = new Master_Plants();
            if (id.HasValue)
            {
                master_Plants = data.Plant_Get(id.Value);
                isNew = false;
            }

            ViewBag.IsNew = isNew;

            return View(master_Plants);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Plants_Form(Master_Plants model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Plant_Save(model))
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

        public ActionResult Plants_Delete(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                clsSwal swal = new clsSwal();
                try
                {
                    clsSaveResult clsSaveResult = data.Plants_Delete(id);
                    if (clsSaveResult.CanSave)
                    {
                        scope.Complete();
                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.title = "Successful";
                    }
                    else
                    {
                        swal.text = clsSaveResult.Message;
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

        public ActionResult Plants_Table()
        {
            return View(data.Plant_GetAll());
        }

        public ActionResult Processes()
        {
            return View();
        }

        public ActionResult Processes_Table()
        {
            return View(data.Process_GetAllView());
        }

        public ActionResult Processes_Form(Guid? id)
        {
            ViewBag.PlantsList = data.SelectListItems_Plant();
            ViewBag.DivisionsList = new List<SelectListItem>();
            ViewBag.DepartmentsList = new List<SelectListItem>();
            ViewBag.SectionsList = new List<SelectListItem>();

            bool isNew = true;
            Master_Processes master_Processes = new Master_Processes();
            if (id.HasValue)
            {
                master_Processes = data.Process_Get(id.Value);
                isNew = false;

                ViewBag.DivisionsList = data.SelectListItems_Division(master_Processes.Master_Sections.Master_Departments.Master_Divisions.Plant_Id);
                ViewBag.DepartmentsList = data.SelectListItems_Department(master_Processes.Master_Sections.Master_Departments.Division_Id);
                ViewBag.SectionsList = data.SelectListItems_Section(master_Processes.Master_Sections.Department_Id);
            }

            ViewBag.IsNew = isNew;

            return View(master_Processes);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Processes_Form(Master_Processes model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Process_Save(model))
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

        public ActionResult Processes_Delete(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                clsSwal swal = new clsSwal();
                try
                {
                    clsSaveResult clsSaveResult = data.Process_Delete(id);
                    if (clsSaveResult.CanSave)
                    {
                        scope.Complete();
                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.title = "Successful";
                    }
                    else
                    {
                        swal.text = clsSaveResult.Message;
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

        public ActionResult Sections()
        {
            return View();
        }

        public ActionResult Sections_Table()
        {
            return View(data.Section_GetAllView());
        }

        public ActionResult Sections_Form(Guid? id)
        {
            ViewBag.PlantsList = data.SelectListItems_Plant();
            ViewBag.DepartmentsList = new List<SelectListItem>();
            ViewBag.DivisionsList = new List<SelectListItem>();

            bool isNew = true;
            Master_Sections master_Sections = new Master_Sections();
            if (id.HasValue)
            {
                master_Sections = data.Section_Get(id.Value);
                isNew = false;
                ViewBag.DepartmentsList = data.SelectListItems_Department(master_Sections.Master_Departments.Division_Id);
                ViewBag.DivisionsList = data.SelectListItems_Division(master_Sections.Master_Departments.Master_Divisions.Plant_Id);
            }

            ViewBag.IsNew = isNew;

            return View(master_Sections);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Sections_Form(Master_Sections model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Section_Save(model))
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

        public ActionResult Sections_Delete(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                clsSwal swal = new clsSwal();
                try
                {
                    clsSaveResult clsSaveResult = data.Section_Delete(id);
                    if (clsSaveResult.CanSave)
                    {
                        scope.Complete();
                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.title = "Successful";
                    }
                    else
                    {
                        swal.text = clsSaveResult.Message;
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

        public ActionResult Users()
        {
            return View();
        }

        public ActionResult Users_Form(Guid? id)
        {
            ViewBag.RoleList = data.SelectListItems_Role();
            ViewBag.LineWorkList = data.SelectListItems_LineWork();
            ViewBag.GradeList = new List<SelectListItem>();
            ViewBag.PlantList = data.SelectListItems_Plant();
            ViewBag.DivisionList = new List<SelectListItem>();
            ViewBag.DepartmentList = new List<SelectListItem>();
            ViewBag.SectionList = new List<SelectListItem>();
            ViewBag.ProcessList = new List<SelectListItem>();

            ViewBag.PrefixTHList = data.SelectListItems_PrefixTH();
            ViewBag.PrefixENList = data.SelectListItems_PrefixEN();
            ViewBag.IsNew = true;

            UserDetails userDetails = new UserDetails();
            userDetails.Users = new Users();
            if (id.HasValue)
            {
                userDetails = data.UserDetails_Get(id.Value);
                ViewBag.GradeList = data.SelectListItems_Grade(userDetails.Users.Master_Grades.LineWork_Id);
                ViewBag.DivisionList = data.SelectListItems_Division(userDetails.Users.Master_Processes.Master_Sections.Master_Departments.Master_Divisions.Plant_Id);
                ViewBag.DepartmentList = data.SelectListItems_Department(userDetails.Users.Master_Processes.Master_Sections.Master_Departments.Master_Divisions.Division_Id);
                ViewBag.SectionList = data.SelectListItems_Section(userDetails.Users.Master_Processes.Master_Sections.Master_Departments.Department_Id);
                ViewBag.ProcessList = data.SelectListItems_Process(userDetails.Users.Master_Processes.Master_Sections.Section_Id);
                ViewBag.IsNew = false;
            }

            return View(userDetails);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Users_Form(UserDetails model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Users_Save(model))
                        {
                            scope.Complete();
                            swal.dangerMode = false;
                            swal.icon = "success";
                            swal.text = "บันทึกข้อมูลเรียบร้อยแล้ว";
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

        public ActionResult Users_Delete(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                clsSwal swal = new clsSwal();
                try
                {
                    clsSaveResult clsSaveResult = data.Users_Delete(id);
                    if (clsSaveResult.CanSave)
                    {
                        scope.Complete();
                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.title = "Successful";
                    }
                    else
                    {
                        swal.text = clsSaveResult.Message;
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

        public ActionResult Users_Table()
        {
            return View(data.clsUsers_GetAllView());
        }

        public ActionResult Users_Upload()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Users_UploadExcel()
        {
            clsSwal swal = new clsSwal();

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadCommitted;
            options.Timeout = TimeSpan.MaxValue;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    var files = Request.Files;
                    bool doComplete = new bool();
                    foreach (string item in files)
                    {
                        var file = files[item];
                        
                        if (file.ContentLength > 0)
                        {
                            string dir = "Users/" + DateTime.Today.ToString("d").Replace('/', '-');
                            clsServiceFTP serviceFTP = new clsServiceFTP();
                            string filePath= serviceFTP.Ftp_UploadFileToString(dir, file);
                            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(filePath);
                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                if (response.ContentLength > 0)
                                {
                                    using (Stream stream = response.GetResponseStream())
                                    {
                                        using (ExcelPackage package = new ExcelPackage(stream))
                                        {
                                            foreach (var sheet in package.Workbook.Worksheets)
                                            {
                                                for (int row = 1; row <= sheet.Dimension.End.Row; row++)
                                                {
                                                    if (row > 3)
                                                    {
                                                        if (string.IsNullOrEmpty(sheet.Cells[row, 1].Text))
                                                        {
                                                            goto EndProcess;
                                                        }
                                                        UserDetails userDetails = new UserDetails();
                                                        userDetails.Detail_EN_FirstName = sheet.Cells[row, 4].Text;
                                                        userDetails.Detail_EN_LastName = sheet.Cells[row, 5].Text;
                                                        userDetails.Prefix_EN_Id = data.Prefix_EN_GetId(sheet.Cells[row, 3].Text, true).Value;
                                                        userDetails.Detail_TH_FirstName = sheet.Cells[row, 7].Text;
                                                        userDetails.Detail_TH_LastName = sheet.Cells[row, 8].Text;
                                                        userDetails.Prefix_TH_Id = data.Prefix_TH_GetId(sheet.Cells[row, 6].Text, true).Value;
                                                        userDetails.Users = new Users();
                                                        Guid? lineworkId = data.LineWork_GetId(sheet.Cells[row, 10].Text, true);
                                                        userDetails.Users.Grade_Id = data.Grade_GetId(lineworkId.Value, sheet.Cells[row, 11].Text, sheet.Cells[row, 12].Text, true).Value;
                                                        Guid? plantId = data.Plant_GetId(sheet.Cells[row, 13].Text, true);
                                                        Guid? divisionId = data.Division_GetId(plantId.Value, sheet.Cells[row, 14].Text, true);
                                                        Guid? departmentId = data.Department_GetId(divisionId.Value, sheet.Cells[row, 15].Text, true);
                                                        Guid? sectionId = data.Section_GetId(departmentId.Value, sheet.Cells[row, 16].Text, true);

                                                        userDetails.Users.Process_Id = data.Process_GetId(sectionId.Value, sheet.Cells[row, 17].Text, true).Value;
                                                        userDetails.Users.Role_Id = 2;
                                                        userDetails.Users.User_Code = sheet.Cells[row, 2].Text;
                                                        userDetails.Users.User_CostCenter = sheet.Cells[row, 18].Text;
                                                        if (data.Users_Save(userDetails))
                                                        {
                                                            doComplete = true;
                                                        }
                                                        else
                                                        {
                                                            goto EndProcess;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                EndProcess:
                    if (doComplete)
                    {
                        scope.Complete();
                        swal.icon = "success";
                        swal.text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                        swal.title = "Successful";
                        swal.dangerMode = false;
                    }
                    else
                    {
                        scope.Dispose();
                        swal.icon = "warning";
                        swal.text = "กรุณาตรวจสอบข้อมูลอีกครั้ง";
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

        public ActionResult Users_GetSelectGrades(Guid? id)
        {
            return Json(data.SelectListItems_Grade(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Users_GetSelectDivisions(Guid? id)
        {
            return Json(data.SelectListItems_Division(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Users_GetSelectDepartments(Guid? id)
        {
            return Json(data.SelectListItems_Department(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Users_GetSelectSections(Guid? id)
        {
            return Json(data.SelectListItems_Section(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Users_GetSelectProcesses(Guid? id)
        {
            return Json(data.SelectListItems_Process(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Users_GetSelectUsers(Guid? id)
        {
            return Json(data.SelectListItems_Users(id), JsonRequestBehavior.AllowGet);
        }
        public ActionResult InquiryTopic()
        {
            return View();
        }
        public ActionResult InquiryTopic_Table()
        {
            return View(data.InquiryTopics_GetAll());
        }
        public ActionResult InquiryTopic_Form(Guid? id)
        {
            bool isNew = new bool();
            Master_InquiryTopics master_InquiryTopics = new Master_InquiryTopics();
            master_InquiryTopics.InquiryTopic_Index = 1;

            if (db.Master_InquiryTopics.Count() > 0)
            {
                master_InquiryTopics.InquiryTopic_Index = db.Master_InquiryTopics.Max(m => m.InquiryTopic_Index + 1);
            }

            if (id.HasValue)
            {
                master_InquiryTopics = db.Master_InquiryTopics.Find(id.Value);
            }
            else
            {
                isNew = true;
            }

            ViewBag.IsNew = isNew;

            return View(master_InquiryTopics);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult InquiryTopic_Form(Master_InquiryTopics model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.InquiryTopic_Save(model))
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

        public ActionResult InquiryTopic_Delete(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                clsSwal swal = new clsSwal();
                try
                {
                    clsSaveResult clsSaveResult = data.InquiryTopic_Delete(id);
                    if (clsSaveResult.CanSave)
                    {
                        scope.Complete();
                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.title = "Successful";
                    }
                    else
                    {
                        swal.text = clsSaveResult.Message;
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
    }
}