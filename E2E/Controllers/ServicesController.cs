﻿using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class ServicesController : BaseController
    {
        private readonly ClsManageService data = new ClsManageService();
        private readonly ClsMail mail = new ClsMail();
        private readonly ClsManageMaster master = new ClsManageMaster();
        private readonly ReportKPI_Filter reportKPI_Filter = new ReportKPI_Filter();

        public ActionResult _AddTeam(Guid id)
        {
            try
            {
                ViewBag.TeamList = data.SelectListItems_Team(id);
                ClsServiceTeams clsServiceTeams = new ClsServiceTeams
                {
                    Service_Id = id
                };
                return PartialView("_AddTeam", clsServiceTeams);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> _AddTeam(ClsServiceTeams model)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.Service_AddTeam(model, nameof(_AddTeam)))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _Comment(Guid id)
        {
            try
            {
                ServiceComments serviceComments = new ServiceComments
                {
                    Service_Id = id
                };
                return PartialView("_Comment", serviceComments);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> _Comment(ServiceComments model)
        {
            ClsSwal swal = new ClsSwal();
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.MaxValue
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.Services_Comment(model, Request.Files))
                    {
                        scope.Complete();
                        swal.Icon = "success";
                        model.Services = null;
                        swal.Option = model;
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


            Response.ContentType = "application/json";
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _CommentHistory(Guid id)
        {
            try
            {
                return PartialView("_CommentHistory", data.ClsServices_ViewComment(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete]
        public async Task<ActionResult> _DeleteTeam(Guid id)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.Service_DeleteTeam(id, nameof(_DeleteTeam)))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _DocumentList(Guid id)
        {
            try
            {
                return PartialView("_DocumentList", db.ServiceDocuments.Where(w => w.Service_Id == id).OrderBy(o => o.Master_Documents.Document_Name).ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult _File(Guid id)
        {
            try
            {
                return PartialView("_File", data.ServiceFiles_View(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult _RefService(Guid id)
        {
            try
            {
                return PartialView("_RefService", data.ClsServices_ViewRefList(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult _SatisfactionResults(Guid id)
        {
            try
            {
                return PartialView("_SatisfactionResults", data.ClsSatisfaction_View(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Action(Guid id)
        {
            try
            {
                var gradeName = db.Users
                .Where(w => w.User_Id == loginId)
                .Select(s => s.Master_Grades.Grade_Name)
                .FirstOrDefault();

                // Split the grade name into character and number parts
                string gradeChar = new string(gradeName.TakeWhile(char.IsLetter).ToArray());
                string gradeNumStr = new string(gradeName.SkipWhile(char.IsLetter).ToArray());

                // Convert the numeric part to an integer
                if (!int.TryParse(gradeNumStr, out int gradeNum))
                {
                    throw new Exception("Invalid grade number format.");
                }

                // Create an anonymous type object to hold the split values
                var userClass = new
                {
                    GradeChar = gradeChar,
                    GradeNum = gradeNum
                };

                bool canAssign = false;
                if (userClass.GradeChar != "M" && userClass.GradeNum <= 6)
                {
                    canAssign = true;
                }

                ViewBag.CanAssign = canAssign;
                ViewBag.AuthorizeIndex = authId;
                ClsServices clsServices = data.ClsServices_View(id);

                if (clsServices.Services.Status_Id != 1)
                {
                    return RedirectToAction("Index");
                }
                else if (!clsServices.Services.Is_Commit)
                {
                    return RedirectToAction("Index");
                }

                return View(clsServices);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult All()
        {
            try
            {
                var data = db.Services
                       .OrderBy(o => o.Status_Id)
                       .ThenBy(t => t.Update)
                       .ThenByDescending(t => t.Create)
                       .Select(s => new ServiceAll()
                       {
                           Create = s.Create,
                           Department_Name = s.Users.Master_Processes.Master_Sections.Master_Departments.Department_Name,
                           Plant_Name = s.Users.Master_Plants.Plant_Name,
                           Priority_Class = s.System_Priorities.Priority_Class,
                           Priority_Id = s.Priority_Id,
                           Priority_Name = s.System_Priorities.Priority_Name,
                           Service_Id = s.Service_Id,
                           Service_Key = s.Service_Key,
                           Service_Subject = s.Service_Subject,
                           Status_Id = s.Status_Id,
                           Status_Class = s.System_Statuses.Status_Class,
                           Status_Name = s.System_Statuses.Status_Name,
                           Update = s.Update,
                           Is_OverDue = s.Is_OverDue
                       }).ToList();

                return View(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult AllRequest()
        {
            return View();
        }

        public ActionResult AllRequest_Table()
        {
            try
            {
                // Step 1: Retrieve all requests
                var requests = data.Services_GetAllRequest_IQ()
                    .Select(s => new
                    {
                        s.Create,
                        s.Service_Subject,
                        s.Service_DueDate,
                        s.Service_EstimateTime,
                        s.Service_Key,
                        s.Update,
                        s.Service_Id,
                        s.System_Priorities,
                        s.System_Statuses,
                        s.Is_OverDue,
                        s.User_Id
                    })
                    .ToList();

                // Step 2: Fetch user details for the required user IDs
                var userIds = requests
                    .Select(t => t.User_Id)
                    .Distinct()
                    .ToList();

                var userDetails = db.UserDetails
                    .Where(u => userIds.Contains(u.User_Id))
                    .ToDictionary(u => u.User_Id, u => u.Detail_EN_FirstName);

                // Step 3: Join the requests and user details in memory
                var clsServiceViewTables = requests.Select(s => new ClsServiceViewTable()
                {
                    Create = s.Create,
                    Subject = s.Service_Subject,
                    Duedate = s.Service_DueDate,
                    Estimate_time = s.Service_EstimateTime,
                    Key = s.Service_Key,
                    Update = s.Update,
                    ServiceId = s.Service_Id,
                    System_Priorities = s.System_Priorities,
                    System_Statuses = s.System_Statuses,
                    Is_OverDue = s.Is_OverDue,
                    Requester = userDetails.ContainsKey(s.User_Id) ? userDetails[s.User_Id] : null,
                    Marker = loginId == s.User_Id
                }).ToList();

                // Step 4: Group requests by System_Statuses
                var groupedRequests = clsServiceViewTables
                    .GroupBy(t => t.System_Statuses.Status_Id)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Fetch System_Statuses based on the existing keys
                var systemStatusesDict = db.System_Statuses
                    .Where(ss => groupedRequests.Keys.Contains(ss.Status_Id))
                    .ToDictionary(ss => ss.Status_Id, ss => ss);

                // Create a Dictionary<System_Statuses, List<ClsServiceUserActionName>>
                var groupedTasks = groupedRequests
                    .ToDictionary(g => systemStatusesDict[g.Key], g => g.Value);

                var model = new AllServiceViewModel
                {
                    GroupedTasks = groupedTasks,
                    AllStatuses = systemStatusesDict.Values.ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // Log exception throw;
                return View("Error", new HandleErrorInfo(ex, "Services", "AllRequest_Table"));
            }
        }

        public ActionResult AllTask()
        {
            return View();
        }

        public ActionResult AllTask_Table()
        {
            try
            {
                // Step 1: Retrieve all tasks with related data included
                var tasks = data.Services_GetAllTask_IQ()
                    .Where(s => s.Action_User_Id.HasValue) // Filter tasks with Action_User_Id
                    .Select(s => new
                    {
                        s.Action_User_Id,
                        s.Create,
                        s.Service_Subject,
                        s.Service_DueDate,
                        s.Service_EstimateTime,
                        s.Service_Key,
                        s.Update,
                        s.Service_Id,
                        s.System_Priorities,
                        s.System_Statuses,
                        s.Is_OverDue
                    })
                    .AsNoTracking()
                    .ToList();

                // Step 2: Fetch user details for the required user IDs
                var userIds = tasks
                    .Select(t => t.Action_User_Id.Value)
                    .Distinct()
                    .ToList();

                var userDetails = db.UserDetails
                    .Where(u => userIds.Contains(u.User_Id))
                    .AsNoTracking()
                    .ToDictionary(u => u.User_Id, u => u.Detail_EN_FirstName);

                // Step 3: Join the tasks and user details in memory
                var clsServiceViewTables = tasks.Select(s => new ClsServiceViewTable()
                {
                    ActionUserId = s.Action_User_Id,
                    Create = s.Create,
                    Subject = s.Service_Subject,
                    Duedate = s.Service_DueDate,
                    Estimate_time = s.Service_EstimateTime,
                    Key = s.Service_Key,
                    Update = s.Update,
                    ServiceId = s.Service_Id,
                    System_Priorities = s.System_Priorities,
                    System_Statuses = s.System_Statuses,
                    Is_OverDue = s.Is_OverDue,
                    ActionBy = userDetails.ContainsKey(s.Action_User_Id.Value) ? userDetails[s.Action_User_Id.Value] : null,
                    Marker = loginId == s.Action_User_Id.Value
                }).ToList();

                // Step 4: Group tasks by System_Statuses
                var groupedTasksById = clsServiceViewTables
                    .GroupBy(t => t.System_Statuses.Status_Id)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Fetch System_Statuses based on the existing keys
                var systemStatusesDict = db.System_Statuses
                    .Where(ss => groupedTasksById.Keys.Contains(ss.Status_Id))
                    .AsNoTracking()
                    .ToDictionary(ss => ss.Status_Id, ss => ss);

                // Create a Dictionary<System_Statuses, List<ClsServiceUserActionName>>
                var groupedTasks = groupedTasksById
                    .ToDictionary(g => systemStatusesDict[g.Key], g => g.Value);

                var model = new AllServiceViewModel
                {
                    GroupedTasks = groupedTasks,
                    AllStatuses = systemStatusesDict.Values.ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // Log exception
                return View("Error", new HandleErrorInfo(ex, "Services", "AllTask_Table"));
            }
        }

        public ActionResult Approve()
        {
            return View();
        }

        public ActionResult Approve_Form(Guid id)
        {
            try
            {
                ViewBag.AuthorizeIndex = authId;
                return View(data.ClsServices_View(id));
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
                return View(data.Services_GetRequiredApprove(true));
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
                return View(data.Services_GetRequiredApprove(false));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Check_Close_Job()
        {
            ClsSwal res = new ClsSwal();

            var ID_service = data.Service_CHK_CloseJob(loginId);

            if (ID_service != null)
            {
                res.Icon = "warning";
                res.DangerMode = true;
                res.Text = "You have a job that hasn't been closed.";
                res.Title = "Please close job";
                res.Option = ID_service;
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public bool Check_ReferenceClose_Job(Guid id)
        {
            bool res = new bool();

            if (id != loginId)
            {
                var ID_service = data.Service_CHK_CloseJob(id);

                if (ID_service != null)
                {
                    res = true;
                }
            }

            return res;
        }

        public ActionResult Commit(Guid id)
        {
            try
            {
                ViewBag.PlantList = new ClsManageMaster().SelectListItems_Plant();
                ViewBag.DivisionList = new ClsManageMaster().SelectListItems_Division();
                ClsServices clsServices = data.ClsServices_View(id);

                if (clsServices.Services.Status_Id != 1)
                {
                    return RedirectToAction("Index");
                }
                else if (clsServices.Services.Is_Commit)
                {
                    return RedirectToAction("Index");
                }

                return View(clsServices);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Commit(ClsServices model)
        {
            ClsSwal swal = new ClsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        if (await data.Services_SetCommit(model.Services, nameof(Commit)))
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

        public async Task<ActionResult> Commit_ToDepartment(Guid id)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                ClsSwal swal = new ClsSwal();
                try
                {
                    if (await data.Services_SetToDepartment(id,nameof(Commit_ToDepartment)))
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
                return Json(swal, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteDupSatisfaction()
        {
            try
            {
                List<Guid> serviceIds = db.Services
                    .Where(w => w.Status_Id == 4)
                    .Select(s => s.Service_Id)
                    .ToList();
                foreach (var item in serviceIds)
                {
                    if (db.Satisfactions.Where(w => w.Service_Id == item).Count() > 1)
                    {
                        Satisfactions satisfactions = new Satisfactions();
                        satisfactions = db.Satisfactions
                            .Where(w => w.Service_Id == item)
                            .OrderByDescending(o => o.Create)
                            .FirstOrDefault();
                        db.Entry(satisfactions).State = System.Data.Entity.EntityState.Deleted;
                        if (db.SaveChanges() > 0)
                        {
                            continue;
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ActionResult> DeleteFile(Guid id)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.ServiceFiles_Delete(id))
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

        public async Task Download_Zipfiles(List<string> Urls, string key)
        {
            ClsApi clsApi = new ClsApi();
            ClsServiceFile clsServiceFile = new ClsServiceFile();
            string ZipName = string.Format("{0}_{1}.zip", key, Urls.Count());
            string zipPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ZipName);

            using (FileStream zipToCreate = new FileStream(zipPath, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create))
                {
                    foreach (var item in Urls)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            using (WebClient webClient = new WebClient())
                            {
                                byte[] data = webClient.DownloadData(item);
                                ZipArchiveEntry entry = archive.CreateEntry(Path.GetFileName(HttpUtility.UrlDecode(item, Encoding.UTF8)));

                                using (Stream entryStream = entry.Open())
                                {
                                    entryStream.Write(data, 0, data.Length);
                                }
                            }
                        }
                    }
                }
            }

            // Read the zip file into a MemoryStream
            byte[] fileBytes;
            using (FileStream zipToRead = new FileStream(zipPath, FileMode.Open))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    zipToRead.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
            }

            // Create a FileStreamPostedFile instance
            HttpPostedFileBase objFile = new FileStreamPostedFile(new MemoryStream(fileBytes), Path.GetFileName(ZipName), "application/zip");
            clsServiceFile.FolderPath = Path.Combine("Service", key, "DocumentControls");
            clsServiceFile.Filename = Path.GetFileName(ZipName);

            // Upload the file
            var res = await clsApi.UploadFile(clsServiceFile, objFile);
            if (res.IsSuccess)
            {
                System.IO.File.Delete(zipPath);
            }

            // Set response headers and send the file
            Response.ContentType = "application/zip";
            Response.AddHeader("content-disposition", "attachment; filename=" + Path.GetFileName(ZipName));
            Response.BufferOutput = true;
            Response.OutputStream.Write(fileBytes, 0, fileBytes.Length);
            Response.End();
        }

        public async Task DownloadDocumentControl(Guid id)
        {
            try
            {
                string Key = db.Services.Find(id).Service_Key;
                var Paths = db.ServiceDocuments.Where(w => w.Service_Id == id).Select(s => s.ServiceDocument_Path).ToList();

                await Download_Zipfiles(Paths, Key);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Form(Guid? id)
        {
            try
            {
                ViewBag.PriorityList = data.SelectListItems_Priority();
                ViewBag.RefServiceList = data.SelectListItems_RefService(loginId);
                ViewBag.UserList = data.SelectListItems_User();
                bool isNew = true;
                Services services = new Services
                {
                    User_Id = loginId
                };
                if (id.HasValue)
                {
                    services = data.Services_View(id.Value);
                    isNew = false;
                }

                ViewBag.IsNew = isNew;
                return View(services);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Form(Services model)
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
                        if (!Check_ReferenceClose_Job(model.User_Id))
                        {
                            if (await data.Services_Save(model, Request.Files))
                            {
                                scope.Complete();
                                swal.DangerMode = false;
                                swal.Icon = "success";
                                swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                                swal.Title = "Successful";
                                swal.Option = model.Service_Id;
                            }
                            else
                            {
                                swal.Icon = "warning";
                                swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                                swal.Title = "Warning";
                            }
                        }
                        else
                        {
                            swal.Icon = "warning";
                            swal.Text = "บันทึกข้อมูลไม่สำเร็จ เนื่องจาก User ที่ท่านเลือกยังไม่ทำการ Close Job";
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

        [HttpDelete]
        public ActionResult Form_Delete(Guid id)
        {
            ClsSwal swal = new ClsSwal();
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    Services services = db.Services.Find(id);
                    db.Entry(services).State = System.Data.Entity.EntityState.Deleted;
                    if (db.SaveChanges() > 0)
                    {
                        scope.Commit();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    swal.Title = ex.Source;
                    swal.Text = ex.GetBaseException().Message;
                }

                return Json(swal, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SetForwarded(Guid id)
        {
            try
            {
                ViewBag.PriorityList = data.SelectListItems_Priority();
                Services services = new Services
                {
                    Ref_Service_Id = id,
                    User_Id = db.Services.Find(id).User_Id
                };

                return View(services);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SetForwarded(Services model)
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
                        swal = data.CheckMissingDocument(model.Ref_Service_Id.Value);
                        if (swal.Option == false)
                        {
                            return Json(swal, JsonRequestBehavior.AllowGet);
                        }

                        if (await data.Services_Save(model, Request.Files, true))
                        {
                            scope.Complete();
                            swal.Option = null;
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

        public ActionResult GetPriorityDateRange(int id)
        {
            try
            {
                int res = new int();
                res = db.System_Priorities
                    .Find(id).Priority_DateRange;

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult GetServiceRef(Guid id)
        {
            try
            {
                return Json(data.SelectListItems_RefService(id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Index()
        {
            try
            {
                ViewBag.AuthorizeIndex = authId;
            }
            catch (Exception)
            {
                throw;
            }

            return View();
        }

        public async Task<ActionResult> Index_Table_WaitAction()
        {
            try
            {
                List<Services> services = await WaitActionList();
                List<ClsServiceViewTable> clsServiceViewTables = services
                     .Select(s => new ClsServiceViewTable()
                     {
                         ActionBy = s.Action_User_Id.HasValue ? Users_GetName(s.Action_User_Id.Value) : "",
                         Create = s.Create,
                         Subject = s.Service_Subject,
                         Duedate = s.Service_DueDate,
                         Estimate_time = s.Service_EstimateTime,
                         Key = s.Service_Key,
                         Requester = Users_GetName(s.User_Id),
                         Update = s.Update,
                         ServiceId = s.Service_Id,
                         System_Priorities = s.System_Priorities,
                         System_Statuses = s.System_Statuses
                     }).ToList();

                ViewBag.UserNames = Users_GetName(loginId);

                return View(clsServiceViewTables);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<List<Services>> WaitActionList()
        {
            Guid departmentId = await GetDepartmentIdAsync(loginId);
            List<Services> services = await db.Services
                .AsNoTracking()
                .Where(w => w.Is_Commit && w.Status_Id == 1 && (w.Is_Approval || !w.Is_MustBeApproved) && w.Department_Id == departmentId && (w.Action_User_Id.HasValue == false || w.Action_User_Id.Value == loginId))
                .OrderByDescending(service => service.Priority_Id)
                .ThenBy(service => new { service.Create, service.Service_DueDate })
                .ToListAsync();

            return services;
        }

        public async Task<ActionResult> Index_Table_Rejected()
        {
            try
            {
                Guid departmentId = await GetDepartmentIdAsync(loginId);
                List<Services> services = await db.Services
                    .AsNoTracking()
                    .Where(w => w.Department_Id == departmentId && w.Action_User_Id.HasValue == false && w.Status_Id == 5)
                    .ToListAsync();

                List<ClsServiceViewTable> clsServiceViewTables = services
                    .Select(s => new ClsServiceViewTable()
                    {
                        Create = s.Create,
                        Subject = s.Service_Subject,
                        Duedate = s.Service_DueDate,
                        Estimate_time = s.Service_EstimateTime,
                        Key = s.Service_Key,
                        Requester = Users_GetName(s.User_Id),
                        Update = s.Update,
                        ServiceId = s.Service_Id,
                        System_Priorities = s.System_Priorities,
                        System_Statuses = s.System_Statuses
                    }).ToList();

                ViewBag.UserNames = Users_GetName(loginId);

                return View(clsServiceViewTables);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult Index_Table_WaitCommit()
        {
            try
            {
                return View(data.Services_GetWaitCommit_IQ());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult MyRequest()
        {
            return View();
        }

        public ActionResult MyRequest_Table()
        {
            try
            {
                return View(data.Services_GetMyRequest());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult MyTask()
        {
            return View();
        }

        public ActionResult MyTask_Table()
        {
            try
            {
                return View(data.Services_GetMyTask());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Report_KPI()
        {
            return View();
        }

        public async Task<ActionResult> Report_KPI_Unsatisfied(int year, int? month, Guid? userId)
        {
            Guid departmentId = await GetDepartmentIdAsync(loginId);
            IQueryable<Satisfactions> jobUnsat = db.Satisfactions
                .Where(w => w.Unsatisfied && w.Create.Year == year && w.Services.Department_Id == departmentId)
                .OrderByDescending(o => o.Create);
            if (month.HasValue)
            {
                jobUnsat = jobUnsat.Where(w => w.Create.Month <= month.Value);
            }
            else if (userId.HasValue)
            {
                jobUnsat = jobUnsat.Where(w => w.Services.Action_User_Id.Value == userId);
            }

            var interimResults = jobUnsat
                .Select(s => new
                {
                    s.Service_Id,
                    s.Services.Service_Key,
                    s.Services.Service_Subject,
                    ActionUserId = s.Services.Action_User_Id.Value,
                    RequestUserId = s.Services.User_Id,
                    Updated = s.Services.Update.Value
                }).ToList();

            List<ReportKPI_Unsatisfied> unsatisfieds = interimResults
                .Select(s => new ReportKPI_Unsatisfied()
                {
                    Service_Id = s.Service_Id,
                    Service_Key = s.Service_Key,
                    Service_Subject = s.Service_Subject,
                    UserAction = master.Users_GetInfomation(s.ActionUserId),
                    UserRequest = master.Users_GetInfomation(s.RequestUserId),
                    Updated = s.Updated
                }).ToList();

            return View(unsatisfieds);
        }

        public ActionResult Report_KPI_View(Guid id, string filter)
        {
            try
            {
                return View(data.ReportKPI_User_Views(id, reportKPI_Filter.DeserializeFilter(filter)));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ActionResult> Report_KPI_Overdue(int year, int? month, Guid? userId)
        {
            Guid departmentId = await GetDepartmentIdAsync(loginId);
            IQueryable<Services> services = db.Services
                .Where(w => w.Update.Value.Year == year && w.Is_OverDue && w.Department_Id == departmentId)
                .OrderByDescending(o => o.Update);
            if (month.HasValue)
            {
                services = services.Where(w => w.Update.Value.Month <= month.Value);
            }
            else if (userId.HasValue)
            {
                services = services.Where(w => w.Action_User_Id == userId);
            }

            // First, get the data from the database
            var interimResults = services
                .Select(s => new
                {
                    s.Service_Id,
                    s.Service_Key,
                    s.Service_Subject,
                    s.System_Statuses.Status_Class,
                    s.System_Statuses.Status_Name,
                    ActionUserId = s.Action_User_Id.Value,
                    Updated = s.Update.Value
                }).ToList(); // Execute query and get results into memory

            // Then, apply the custom method in-memory
            List<ReportKPI_Overdue> overdues = interimResults
                .Select(s => new ReportKPI_Overdue()
                {
                    Service_Id = s.Service_Id,
                    Service_Key = s.Service_Key,
                    Service_Subject = s.Service_Subject,
                    Status_Class = s.Status_Class,
                    Status_Name = s.Status_Name,
                    User_Name = master.Users_GetInfomation(s.ActionUserId),
                    Updated = s.Updated
                }).ToList();

            return View(overdues);
        }

        public ActionResult RequestChangeDue()
        {
            return View();
        }

        public async Task<ActionResult> RequestChangeDue_Accept(Guid id)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.ServiceChangeDueDate_Accept(id, nameof(RequestChangeDue_Accept)))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> RequestChangeDue_Cancel(Guid id)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.ServiceChangeDueDate_Cancel(id))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RequestChangeDue_Form(Guid id)
        {
            return View(data.ServiceChangeDueDate_New(id));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> RequestChangeDue_Form(ServiceChangeDueDate model)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.ServiceChangeDueDate_Request(model, nameof(RequestChangeDue_Form)))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> RequestChangeDue_Reject(Guid id)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.ServiceChangeDueDate_Reject(id, nameof(RequestChangeDue_Reject)))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RequestChangeDue_Table()
        {
            try
            {
                var sql = data.ServiceChangeDues_List();

                return View(sql);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<JsonResult> ResendEmail(Guid id)
        {
            ClsSwal swal = new ClsSwal();
            try
            {
                Services services = await db.Services.FindAsync(id);
                if (await mail.ResendMail(services.Service_Id))
                {
                    swal.DangerMode = false;
                    swal.Icon = "success";
                    swal.Text = "ส่งอีเมลอีกครั้งเรียบร้อยแล้ว";
                    swal.Title = "Successful";
                }
                else
                {
                    swal.Icon = "warning";
                    swal.Text = "ส่งอีเมลอีกครั้งไม่สำเร็จ";
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

        public ActionResult ServiceInfomation(Guid id)
        {
            try
            {
                ViewBag.AuthorizeIndex = authId;
                return View(data.ClsServices_View(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult ServiceInfomation_Document(Guid id)
        {
            try
            {
                ServiceDocuments serviceDocuments = new ServiceDocuments();
                serviceDocuments = db.ServiceDocuments.Find(id);

                ViewBag.HasTemplate = db.Master_DocumentVersions
                    .Any(a => a.Document_Id == serviceDocuments.Document_Id);

                return View(serviceDocuments);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ServiceInfomation_Document(ServiceDocuments model)
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
                        if (await data.Services_SaveDocumentControl(model, Request.Files))
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

        public ActionResult ServiceInfomation_View(Guid id)
        {
            try
            {
                return View(db.ServiceDocuments.Find(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult SetApproved(Guid id)
        {
            return View(new ServiceComments() { Service_Id = id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SetApproved(ServiceComments model)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.Services_SetApprove(model, nameof(SetApproved)))
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

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetAssign(Guid id)
        {
            try
            {
                string deptName = db.Users
                    .Where(w => w.User_Id == loginId)
                    .Select(s => s.Master_Processes.Master_Sections.Master_Departments.Department_Name)
                    .FirstOrDefault();
                List<Guid> depIds = db.Master_Departments
                    .Where(w => w.Department_Name == deptName)
                    .Select(s => s.Department_Id)
                    .ToList();

                ViewBag.UserList = db.Users
                    .Where(w => depIds.Contains(w.Master_Processes.Master_Sections.Department_Id))
                    .AsEnumerable()
                    .Select(s => new SelectListItem()
                    {
                        Value = s.User_Id.ToString(),
                        Text = master.Users_GetInfomation(s.User_Id)
                    }).OrderBy(o => o.Text)
                    .ToList();

                ClsServices clsServices = new ClsServices
                {
                    Service_Id = id
                };

                return View(clsServices);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SetAssign(ClsServices model)
        {
            ClsSwal swal = new ClsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        if (await data.Services_SetToUser(model.Service_Id, model.User_Id, nameof(SetAssign)))
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

        public void SetAssignUserId()
        {
            try
            {
                List<Guid> servicesIds = db.Services
                    .Where(w => !w.Assign_User_Id.HasValue && w.Is_Commit)
                    .Select(s => s.Service_Id)
                    .ToList();
                using (TransactionScope scope = new TransactionScope())
                {
                    foreach (var item in servicesIds)
                    {
                        Guid userId = db.ServiceComments
                            .Where(w => w.Comment_Content.Contains("Assign") && w.Service_Id == item)
                            .OrderByDescending(o => o.Create)
                            .Select(s => s.User_Id.Value)
                            .FirstOrDefault();

                        Services services = db.Services.Find(item);
                        services.Assign_User_Id = userId;
                        db.Entry(services).State = EntityState.Modified;
                    }
                    if (db.SaveChanges() > 0)
                    {
                        scope.Complete();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult SetCancel(Guid id)
        {
            ServiceComments serviceComments = new ServiceComments
            {
                Service_Id = id
            };

            return View(serviceComments);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SetCancel(ServiceComments model)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.Services_SetCancel(model))
                    {
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                        swal.Option = model.Service_Id;
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetClose(Guid id)
        {
            ClsInquiryTopics clsInquiryTopics = new ClsInquiryTopics
            {
                Services = db.Services.Find(id),
                List_Master_InquiryTopics = db.Master_InquiryTopics.Where(w => w.Program_Id == 1).OrderBy(o => o.InquiryTopic_Index).ToList()
            };

            return View(clsInquiryTopics);
        }

        [HttpPost]
        public async Task<ActionResult> SetClose(Guid id, List<ClsEstimate> score)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.SaveEstimate(id, score))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> SetClosePrevious()
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                ClsSwal swal = new ClsSwal();
                try
                {
                    var services = await db.Services.Where(s => s.Status_Id == 3 && s.Update.Value.Month < DateTime.Now.Month).ToListAsync();

                    foreach (var service in services)
                    {
                        await data.Services_SetClose(service, true);
                    }

                    if (await db.SaveChangesAsync() > 0)
                    {
                        scope.Complete();
                        swal.Text = "ปิดงานที่ผู้ใช้ไม่ได้ปิดในเดือนก่อนๆ เรียบร้อยแล้ว";
                        swal.Icon = "success";
                        swal.DangerMode = false;
                        swal.Title = "Successful";
                    }
                }
                catch (Exception ex)
                {
                    swal.Text = ex.GetBaseException().Message;
                }

                return Json(swal, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SetComplete(Guid id)
        {
            ServiceComments serviceComments = new ServiceComments
            {
                Service_Id = id
            };

            return View(serviceComments);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SetComplete(ServiceComments model)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    swal = data.CheckMissingDocument(model.Service_Id);
                    if (swal.Option == false)
                    {
                        return Json(swal, JsonRequestBehavior.AllowGet);
                    }

                    if (await data.Services_SetComplete(model, nameof(SetComplete)))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SetFreePoint(Guid id)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.Services_SetFreePoint(id))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetInProgress(Guid id)
        {
            try
            {
                string secName = db.Users.Where(w => w.User_Id == loginId)
                    .Select(s => s.Master_Processes.Master_Sections.Section_Name)
                    .FirstOrDefault();

                List<Guid> secIds = db.Master_Sections
                    .Where(w => w.Section_Name == secName)
                    .Select(s => s.Section_Id)
                    .ToList();

                ViewBag.WorkRootList = db.WorkRoots
                    .Where(w => secIds.Contains(w.Section_Id))
                    .Select(s => new SelectListItem()
                    {
                        Value = s.WorkRoot_Id.ToString(),
                        Text = s.WorkRoot_Name
                    }).OrderBy(o => o.Text)
                    .ToList();

                return View(data.Services_View(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SetInProgress(Services model)
        {
            ClsSwal swal = new ClsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        if (await data.Services_SetAction(model))
                        {
                            scope.Complete();
                            swal.DangerMode = false;
                            swal.Icon = "success";
                            swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.Title = "Successful";
                            swal.Option = model.Service_Id;
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

        public ActionResult SetMustApprove(Guid id)
        {
            var Services = db.Services.Find(id);
            string deptName = db.Users.Find(Services.User_Id).Master_Processes.Master_Sections.Master_Departments.Department_Name;
            List<Guid> sendTo = db.Users
                .Where(w => w.Master_Processes.Master_Sections.Master_Departments.Department_Name == deptName && w.Master_Grades.Master_LineWorks.Authorize_Id == 2)
                .Select(s => s.User_Id)
                .ToList();

            ViewBag.sendTo = db.UserDetails.Where(w => sendTo.Contains(w.User_Id)).Select(s => s.Detail_EN_FirstName + " " + s.Detail_EN_LastName).ToList();

            var result = ViewBag.sendTo;

            return View(new ServiceComments() { Service_Id = id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SetMustApprove(ServiceComments model)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.Services_SetRequired(model, nameof(SetMustApprove)))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public void SetOverDue()
        {
            try
            {
                int[] statusId = { 3, 4 };
                string msg = "Complete task";
                using (TransactionScope scope = new TransactionScope())
                {
                    foreach (var item in db.Services.AsNoTracking().Where(w => statusId.Contains(w.Status_Id)))
                    {
                        DateTime completeDate = db.ServiceComments
                            .Where(w => w.Service_Id == item.Service_Id && w.Comment_Content.StartsWith(msg))
                            .Select(s => s.Create)
                            .FirstOrDefault();

                        if (completeDate.Date > item.Service_DueDate)
                        {
                            item.Is_OverDue = true;
                            db.Services.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                    }

                    if (db.SaveChanges() > 0)
                    {
                        scope.Complete();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPending(ServiceComments model)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.Services_SetPending(model, nameof(SetPending)))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetRequestReject(Guid id)
        {
            ServiceComments serviceComments = new ServiceComments
            {
                Service_Id = id
            };

            return View(serviceComments);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SetRequestReject(ServiceComments model)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.Services_SetRequestReject(model, nameof(SetRequestReject)))
                    {
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                        swal.Option = model.Service_Id;
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetReject(Guid id)
        {
            try
            {
                ServiceComments serviceComments = new ServiceComments
                {
                    Service_Id = id
                };

                return View(serviceComments);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SetReject(ServiceComments model)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.Services_SetReject(model, nameof(SetReject)))
                    {
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                        swal.Option = model.Service_Id;
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetReturnAssign(Guid id)
        {
            ServiceComments serviceComments = new ServiceComments
            {
                Service_Id = id
            };

            return View(serviceComments);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SetReturnAssign(ServiceComments model)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.Services_SetReturnAssign(model, nameof(SetReturnAssign)))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetReturnJob(Guid id)
        {
            ServiceComments serviceComments = new ServiceComments
            {
                Service_Id = id
            };

            return View(serviceComments);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SetReturnJob(ServiceComments model)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.Services_SetReturnJob(model, nameof(SetReturnJob)))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public string Users_GetName(Guid? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return "";
                }

                return db.UserDetails
                    .Where(w => w.User_Id == id)
                    .Select(s => s.Detail_EN_FirstName)
                    .FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<JsonResult> YearList()
        {
            List<int> years = await db.Services
                .Select(s => s.Create.Year) // Select the year first
                .Distinct() // Remove duplicates
                .OrderByDescending(year => year) // Then order by the year
                .ToListAsync();

            return Json(years, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> YearlyReportOverview(int year)
        {
            int[] finishedStatus = { 3, 4 };
            int[] unfinishedStatus = { 1, 2 };
            int rejectedStatus = 5;

            Guid departmentId = await GetDepartmentIdAsync(loginId);

            var createdServicesByMonth = await GetServicesByMonthAsync(year, departmentId, true);
            var updatedServicesByMonth = await GetServicesByMonthAsync(year, departmentId, false);

            int remainingServicesCount = await CalculateRemainingServicesAsync(departmentId, year, unfinishedStatus);

            var monthlyOverviews = CalculateMonthlyOverviews(year, createdServicesByMonth, updatedServicesByMonth, finishedStatus, rejectedStatus, remainingServicesCount);

            ClsServiceKPI clsServiceKPI = new ClsServiceKPI
            {
                Remaining = remainingServicesCount,
                Overviews = monthlyOverviews
            };

            // Determine the end month for green lines
            DateTime currentDate = DateTime.Now;

            int greenEndMonth;
            if (currentDate.Year > year)
            {
                if (currentDate.Month == 1) // January case
                {
                    greenEndMonth = currentDate.Day < 8 ? 11 : 12; // January 1-7: Jan-Nov, January 8 onwards: Jan-Dec
                }
                else
                {
                    greenEndMonth = 12; // Whole year should be green
                }
            }
            else if (currentDate.Year == year)
            {
                greenEndMonth = currentDate.Day < 8 ? currentDate.Month - 2 : currentDate.Month - 1;
                greenEndMonth = greenEndMonth < 1 ? 12 : greenEndMonth; // Handle wrap-around to previous year
            }
            else
            {
                greenEndMonth = 0; // No month should be green
            }

            ViewBag.GreenEndMonth = greenEndMonth;

            return View(clsServiceKPI);
        }

        private async Task<List<MonthlyServices>> GetServicesByMonthAsync(int year, Guid departmentId, bool isCreated)
        {
            if (isCreated)
            {
                return await db.Services.AsNoTracking()
                    .Where(s => s.Create.Year == year && s.Department_Id == departmentId)
                    .GroupBy(s => s.Create.Month)
                    .Select(g => new MonthlyServices
                    {
                        Month = g.Key,
                        Services = g.ToList()
                    })
                    .OrderBy(ms => ms.Month)
                    .ToListAsync();
            }
            else
            {
                return await db.Services.AsNoTracking()
                    .Where(s => s.Update.HasValue && s.Update.Value.Year == year && s.Department_Id == departmentId)
                    .GroupBy(s => s.Update.Value.Month)
                    .Select(g => new MonthlyServices
                    {
                        Month = g.Key,
                        Services = g.ToList()
                    })
                    .OrderBy(ms => ms.Month)
                    .ToListAsync();
            }
        }


        private Expression<Func<Services, bool>> CreateYearPredicate(int year, bool isCreated)
        {
            return s => isCreated ? s.Create.Year == year : s.Update.Value.Year == year;
        }

        private async Task<int> CalculateRemainingServicesAsync(Guid departmentId, int year, int[] unfinishedStatus)
        {
            return await db.Services.AsNoTracking()
                .Where(s => ((s.Create.Year < year && unfinishedStatus.Contains(s.Status_Id)) ||
                            (s.Create.Year < year && s.Update.Value.Year >= year)) &&
                            s.Department_Id == departmentId)
                .CountAsync();
        }

        private async Task<Guid> GetDepartmentIdAsync(Guid userId)
        {
            return await db.Users
                .Where(u => u.User_Id == userId)
                .Select(u => u.Master_Processes.Master_Sections.Department_Id)
                .FirstOrDefaultAsync();
        }

        private List<ClsServiceKPI.Overview> CalculateMonthlyOverviews(int year, List<MonthlyServices> createdServicesByMonth, List<MonthlyServices> updatedServicesByMonth, int[] finishedStatus, int rejectedStatus, int initialRemainingCount)
        {
            int cumulativeTotal = initialRemainingCount;
            int cumulativeCompleted = 0, cumulativeManualClose = 0, cumulativeClosed = 0, cumulativeOverdue = 0, cumulativeUnsatisfied = 0, cumulativeRejected = 0;

            var monthlyOverviews = new List<ClsServiceKPI.Overview>();
            // ตรวจสอบว่าปีที่เลือกเป็นปีปัจจุบันหรือไม่
            int currentYear = DateTime.Now.Year;
            int monthLimit = (year == currentYear) ? DateTime.Now.Month : 12;

            for (int month = 1; month <= monthLimit; month++) // เปลี่ยนจาก month <= 12 เป็น month <= currentMonth
            {
                var createdData = createdServicesByMonth.FirstOrDefault(c => c.Month == month);
                var updatedData = updatedServicesByMonth.FirstOrDefault(u => u.Month == month);

                int incoming = createdData?.Services.Count ?? 0;
                int completed = updatedData?.Services.Count(c => finishedStatus.Contains(c.Status_Id)) ?? 0;
                int closeAuto = updatedData?.Services.Count(s => s.Is_AutoClose && s.Status_Id == 4) ?? 0;
                int closeManual = updatedData?.Services.Count(s => !s.Is_AutoClose && s.Status_Id == 4) ?? 0;
                int closed = closeAuto + closeManual;
                int rejected = updatedData?.Services.Count(c => c.Status_Id == rejectedStatus) ?? 0;
                int overdue = updatedData?.Services.Count(s => s.Is_OverDue) ?? 0;
                int unsatisfied = CalculateUnsatisfiedServices(updatedData?.Services);

                cumulativeTotal += incoming;
                cumulativeCompleted += completed;
                cumulativeManualClose += closeManual;
                cumulativeClosed += closed;
                cumulativeRejected += rejected;
                cumulativeOverdue += overdue;
                cumulativeUnsatisfied += unsatisfied;

                double ontime = (cumulativeCompleted == 0) ? 1 : (double)(cumulativeCompleted - cumulativeOverdue) / cumulativeCompleted;
                double satisfied = (cumulativeManualClose == 0) ? 1 : (double)(cumulativeManualClose - cumulativeUnsatisfied) / cumulativeManualClose;

                monthlyOverviews.Add(new ClsServiceKPI.Overview
                {
                    NumberYear = year,
                    NumberMonth = month,
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month),
                    Incoming = incoming,
                    Completed = completed,
                    CloseAuto = closeAuto,
                    CloseManual = closeManual,
                    Closed = closed,
                    Rejected = rejected,
                    Total = cumulativeTotal,
                    CompletedTotal = cumulativeCompleted,
                    ClosedTotal = cumulativeClosed,
                    RejectedTotal = cumulativeRejected,
                    Overdue = cumulativeOverdue,
                    Ontime = ontime,
                    Unsatisfied = cumulativeUnsatisfied,
                    Satisfied = satisfied
                });
            }

            return monthlyOverviews;
        }


        private int CalculateUnsatisfiedServices(List<Services> services)
        {
            if (services == null) return 0;

            return services
                .Where(s => s.Status_Id == 4)
                .Join(db.Satisfactions.AsNoTracking(),
                    ser => ser.Service_Id,
                    sat => sat.Service_Id,
                    (ser, sat) => sat.Unsatisfied)
                .Count(unsatisfied => unsatisfied);
        }

        private class MonthlyServices
        {
            public int Month { get; set; }
            public List<Services> Services { get; set; }
        }

        public async Task SetUpdateFromCommentComplete()
        {
            var commentComplete = await db.ServiceComments
                .AsNoTracking()
                .Where(w => w.Comment_Content.StartsWith("Complete task") && w.Services.Status_Id == 4)
                .Select(s => new
                {
                    s.Service_Id,
                    s.Create
                }).ToListAsync();

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var item in commentComplete)
                {
                    var service = await db.Services.FindAsync(item.Service_Id);
                    if (service != null)
                    {
                        service.Update = item.Create;
                    }
                }

                try
                {
                    await db.SaveChangesAsync();
                    scope.Complete();
                }
                catch (Exception)
                {
                    // Handle the exception (e.g., log it)
                    throw;  // or handle accordingly
                }
            }
        }


        public async Task<ActionResult> YearlyReportIndividual(int year)
        {
            try
            {
                int[] finishedStatus = { 3, 4 };
                int rejectedStatus = 5;

                Guid departmentId = await GetDepartmentIdAsync(loginId);
                var userIds = await GetUserIdsForYearAsync(year, departmentId);
                var nonNullUserIds = userIds.Where(id => id.HasValue).Select(id => id.Value).ToList();

                var userList = await GetUserDetailsAsync(nonNullUserIds);

                var services = await GetServicesForYearAsync(nonNullUserIds, year, departmentId);
                var createdYear = GroupServicesByUserAndYear(services, year, s => s.Create.Year);
                var updatedYear = GroupServicesByUserAndYear(services, year, s => s.Update.Value.Year);

                ClsServiceKPI clsServiceKPI = new ClsServiceKPI
                {
                    Individuals = userList.Select(user =>
                    {
                        var createdData = createdYear.FirstOrDefault(c => c.User_Id == user.User_Id);
                        var updatedData = updatedYear.FirstOrDefault(u => u.User_Id == user.User_Id);

                        int incoming = createdData?.Services.Count ?? 0;
                        int completed = updatedData?.Services.Count(c => finishedStatus.Contains(c.Status_Id)) ?? 0;
                        int closeAuto = updatedData?.Services.Count(s => s.Is_AutoClose && s.Status_Id == 4) ?? 0;
                        int closeManual = updatedData?.Services.Count(s => !s.Is_AutoClose && s.Status_Id == 4) ?? 0;
                        int closed = closeAuto + closeManual;
                        int rejected = updatedData?.Services.Count(c => c.Status_Id == rejectedStatus) ?? 0;
                        int overdue = updatedData?.Services.Count(s => s.Is_OverDue) ?? 0;
                        int unsat = updatedData?.Services
                        .Where(w => w.Status_Id == 4)
                        .Join(db.Satisfactions,
                        ser => ser.Service_Id,
                        sat => sat.Service_Id,
                        (ser, sat) => sat.Unsatisfied)
                        .Where(w => w)
                        .Count() ?? 0;

                        double ontime = completed == 0 ? 1 : (double)(completed - overdue) / completed;
                        double satisfied = closeManual == 0 ? 1 : (double)(closeManual - unsat) / closeManual;

                        return new ClsServiceKPI.Individual
                        {
                            NumberYear = year,
                            UserId = user.User_Id,
                            User = $"{user.User_Code} [{user.Username}]",
                            Incoming = incoming,
                            Completed = completed,
                            CloseAuto = closeAuto,
                            CloseManual = closeManual,
                            Closed = closed,
                            Rejected = rejected,
                            Overdue = overdue,
                            Ontime = ontime,
                            Unsatisfied = unsat,
                            Satisfied = satisfied
                        };
                    }).ToList()
                };

                return View(clsServiceKPI.Individuals);
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, display an error message, etc.) For now, we rethrow
                // the exception
                throw ex;
            }
        }

        private async Task<List<Guid?>> GetUserIdsForYearAsync(int year, Guid departmentId)
        {
            return await db.Services
                          .Where(s => (s.Create.Year == year || s.Update.Value.Year == year) && s.Department_Id == departmentId)
                          .Select(s => s.Action_User_Id)
                          .Distinct()
                          .ToListAsync();
        }

        private async Task<List<UserDetail>> GetUserDetailsAsync(List<Guid> userIds)
        {
            return await db.Users
                          .Where(u => userIds.Contains(u.User_Id))
                          .Select(s => new UserDetail
                          {
                              User_Id = s.User_Id,
                              User_Code = s.User_Code,
                              Username = s.Username
                          })
                          .OrderBy(o => o.User_Code)
                          .ToListAsync();
        }

        private async Task<List<Services>> GetServicesForYearAsync(List<Guid> userIds, int year, Guid departmentId)
        {
            return await db.Services
                          .Where(s => userIds.Contains(s.Action_User_Id.Value) &&
                                      (s.Create.Year == year || s.Update.Value.Year == year) &&
                                      s.Department_Id == departmentId)
                          .ToListAsync();
        }

        private List<UserServices> GroupServicesByUserAndYear(List<Services> services, int year, Expression<Func<Services, int>> yearSelector)
        {
            return services
                .Where(s => yearSelector.Compile()(s) == year)
                .GroupBy(s => s.Action_User_Id.Value)
                .Select(group => new UserServices
                {
                    User_Id = group.Key,
                    Services = group.ToList()
                })
                .ToList();
        }

        private class UserDetail
        {
            public Guid User_Id { get; set; }
            public string User_Code { get; set; }
            public string Username { get; set; }
        }

        private class UserServices
        {
            public Guid User_Id { get; set; }
            public List<Services> Services { get; set; }
        }

        public async Task<ActionResult> YearlyReportExport(int year)
        {
            Guid departmentId = await db.Users
                .Where(w => w.User_Id == loginId)
                .Select(s => s.Master_Processes.Master_Sections.Department_Id)
                .FirstOrDefaultAsync();

            var services = await db.Services
                .Where(w => w.Create.Year == year && w.Department_Id == departmentId)
                .OrderBy(o => o.Create)
                .ToListAsync();

            var includedProperties = new List<string> { "Service_Key", "Service_Subject", "Create", "System_Statuses.Status_Name", "Service_Description", "System_Priorities.Priority_Name", "Service_DueDate", "Users.User_Code", "Update" };

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.Add("Yearly Report");

                // Adding headers and handling nested properties
                var headers = new List<string>();
                int columnIndex = 1;
                foreach (var propertyName in includedProperties)
                {
                    var header = GetPropertyDisplayName(typeof(Services), propertyName);
                    headers.Add(header);
                    sheet.Cells[1, columnIndex++].Value = header;
                }

                // Apply AutoFilter to the header row
                sheet.Cells[1, 1, 1, columnIndex - 1].AutoFilter = true;

                // Populate the worksheet with data
                int rowIndex = 2;
                foreach (var service in services)
                {
                    columnIndex = 1;
                    foreach (var propertyName in includedProperties)
                    {
                        object value = GetNestedPropertyValue(service, propertyName);
                        sheet.Cells[rowIndex, columnIndex].Value = value;
                        if (value is DateTime)
                        {
                            sheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "dd-MM-yyyy";
                        }
                        columnIndex++;
                    }
                    rowIndex++;
                }

                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

                package.Save();
            }

            stream.Position = 0;
            var fileName = $"YearlyReport_{year}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private string GetPropertyDisplayName(Type type, string propertyName)
        {
            var parts = propertyName.Split('.');
            PropertyInfo property = null;
            foreach (var part in parts)
            {
                property = type.GetProperty(part);
                if (property == null) return part; // Return the part name if not found
                type = property.PropertyType;
            }
            var displayName = property.GetCustomAttributes(typeof(DisplayAttribute), true)
                                      .FirstOrDefault() as DisplayAttribute;
            return displayName?.Name ?? property.Name;
        }

        private object GetNestedPropertyValue(object obj, string propertyName)
        {
            foreach (var part in propertyName.Split('.'))
            {
                if (obj == null) return null;
                var property = obj.GetType().GetProperty(part);
                if (property == null) return null;
                obj = property.GetValue(obj);
            }
            return obj;
        }
    }
}

public class MemoryPostedFile : HttpPostedFileBase
{
    private readonly byte[] fileBytes;

    public MemoryPostedFile(byte[] fileBytes, string fileName = null)
    {
        this.fileBytes = fileBytes;
        this.FileName = fileName;
        this.InputStream = new MemoryStream(fileBytes);
    }

    public override int ContentLength => fileBytes.Length;

    public override string FileName { get; }

    public override Stream InputStream { get; }
}

public class AllServiceViewModel
{
    public Dictionary<System_Statuses, List<ClsServiceViewTable>> GroupedTasks { get; set; }
    public List<System_Statuses> AllStatuses { get; set; }
}
