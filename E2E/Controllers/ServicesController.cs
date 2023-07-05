using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ClsManageService data = new ClsManageService();
        private readonly ClsContext db = new ClsContext();
        private readonly ClsServiceFTP ftp = new ClsServiceFTP();
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
                    MethodBase methodBase = MethodBase.GetCurrentMethod();
                    if (await data.Service_AddTeam(model, methodBase.Name))
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }

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
                    MethodBase methodBase = MethodBase.GetCurrentMethod();
                    if (await data.Service_DeleteTeam(id, methodBase.Name))
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
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
                ViewBag.Is_MustBeApproved = db.Services.Where(w => w.Service_Id == id).Select(s => s.Is_MustBeApproved).FirstOrDefault();
                Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                ViewBag.AuthorizeIndex = db.Users
                .Where(w => w.User_Id == userId)
                .Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id)
                .FirstOrDefault();

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
                return View(data.Services_GetDepartmentRequest());
            }
            catch (Exception)
            {
                throw;
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
                List<ClsServiceUserActionName> clsServiceUserActionName = data.Services_GetAllTask_IQ()
                    .AsEnumerable()
                    .Select(s => new ClsServiceUserActionName()
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
                        System_Statuses = s.System_Statuses,
                        Is_OverDue = s.Is_OverDue
                    }).ToList();

                return View(clsServiceUserActionName);
            }
            catch (Exception)
            {
                throw;
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
                Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                ViewBag.AuthorizeIndex = db.Users
                .Where(w => w.User_Id == userId)
                .Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id)
                .FirstOrDefault();
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

            Guid userId = Guid.Parse(HttpContext.User.Identity.Name);

            var ID_service = data.Service_CHK_CloseJob(userId);

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

        public bool Check_ReferenceClose_Job(Guid userId)
        {
            bool res = new bool();

            Guid Id = Guid.Parse(HttpContext.User.Identity.Name);
            if (Id != userId)
            {
                var ID_service = data.Service_CHK_CloseJob(userId);

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
                        MethodBase methodBase = MethodBase.GetCurrentMethod();
                        if (await data.Services_SetCommit(model.Services, methodBase.Name))
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
                        swal.Text = ex.Message;
                        Exception inner = ex.InnerException;
                        while (inner != null)
                        {
                            swal.Title = inner.Source;
                            swal.Text += string.Format("\n{0}", inner.Message);
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
                    if (await data.Services_SetToDepartment(id))
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public async Task Download_Zipfiles(List<string> Urls, string key)
        {
            ClsApi clsApi = new ClsApi();
            ClsServiceFile clsServiceFile = new ClsServiceFile();
            string ZipName = string.Format("{0}_{1}.zip", key, Urls.Count());
            ZipName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ZipName);
            using (FileStream zipToCreate = new FileStream(ZipName, FileMode.Create))
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

                byte[] fileBytes = System.IO.File.ReadAllBytes(ZipName);
                HttpPostedFileBase objFile = (HttpPostedFileBase)new MemoryPostedFile(fileBytes);

                clsServiceFile.FolderPath = string.Format("Service/{0}/DocumentControls", key);
                clsServiceFile.Filename = Path.GetFileName(ZipName);

                //เก็บไฟล์ที่ User Download ไว้
                var res = await clsApi.UploadFile(clsServiceFile, objFile);
                if (res.IsSuccess)
                {
                    System.IO.File.Delete(ZipName);
                }
                Response.ContentType = "application/zip";
                Response.AddHeader("content-disposition", "attachment; filename=" + Path.GetFileName(ZipName));
                Response.BufferOutput = true;
                Response.OutputStream.Write(fileBytes, 0, fileBytes.Length);
                Response.End();
            }
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
                Guid userId = Guid.Parse(HttpContext.User.Identity.Name);

                ViewBag.PriorityList = data.SelectListItems_Priority();
                ViewBag.RefServiceList = data.SelectListItems_RefService(userId);
                ViewBag.UserList = data.SelectListItems_User();
                bool isNew = true;
                Services services = new Services
                {
                    User_Id = userId
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
                        swal.Text = ex.Message;
                        Exception inner = ex.InnerException;
                        while (inner != null)
                        {
                            swal.Title = inner.Source;
                            swal.Text += string.Format("\n{0}", inner.Message);
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }

                return Json(swal, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Form_Forward(Guid id)
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
        public async Task<ActionResult> Form_Forward(Services model)
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
                        swal.Text = ex.Message;
                        Exception inner = ex.InnerException;
                        while (inner != null)
                        {
                            swal.Title = inner.Source;
                            swal.Text += string.Format("\n{0}", inner.Message);
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
                Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                ViewBag.AuthorizeIndex = db.Users
                .Where(w => w.User_Id == userId)
                .Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id)
                .FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }

            return View();
        }

        public ActionResult Index_Table_WaitAction()
        {
            try
            {
                List<ClsServiceUserActionName> clsServiceUserActionName = data.Services_GetWaitAction_IQ(Guid.Parse(HttpContext.User.Identity.Name))
                    .AsEnumerable()
                     .Select(s => new ClsServiceUserActionName()
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

                ViewBag.UserNames = Users_GetName(Guid.Parse(HttpContext.User.Identity.Name));

                return View(clsServiceUserActionName);
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

        public ActionResult RenameFolder()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    var serviceList = db.Services
                        .Select(s => new
                        {
                            Id = s.Service_Id,
                            Key = s.Service_Key
                        }).ToList();

                    foreach (var item in serviceList)
                    {
                        List<ServiceFiles> serviceFiles = new List<ServiceFiles>();
                        serviceFiles = db.ServiceFiles
                            .Where(w => w.Service_Id == item.Id && w.ServiceFile_Path.Contains(item.Id.ToString()))
                            .ToList();
                        if (serviceFiles.Count > 0)
                        {
                            foreach (var item2 in serviceFiles)
                            {
                                item2.ServiceFile_Path = item2.ServiceFile_Path.Replace(item.Id.ToString(), item.Key);
                                db.Entry(item2).State = System.Data.Entity.EntityState.Modified;
                            }
                        }

                        List<Guid> serviceCommentsIds = new List<Guid>();
                        serviceCommentsIds = db.ServiceComments
                            .Where(w => w.Service_Id == item.Id)
                            .Select(s => s.ServiceComment_Id)
                            .ToList();
                        List<ServiceCommentFiles> serviceCommentFiles = new List<ServiceCommentFiles>();
                        serviceCommentFiles = db.ServiceCommentFiles
                            .Where(w => serviceCommentsIds.Contains(w.ServiceComment_Id) && w.ServiceCommentFile_Path.Contains(item.Id.ToString()))
                            .ToList();
                        if (serviceCommentFiles.Count > 0)
                        {
                            foreach (var item2 in serviceCommentFiles)
                            {
                                item2.ServiceCommentFile_Path = item2.ServiceCommentFile_Path.Replace(item.Id.ToString(), item.Key);
                                db.Entry(item2).State = System.Data.Entity.EntityState.Modified;
                            }
                        }
                    }

                    if (db.SaveChanges() > 0)
                    {
                        if (ftp.Ftp_RenameFolder("Service"))
                        {
                            scope.Complete();
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public ActionResult Report_KPI(ReportKPI_Filter model)
        {
            try
            {
                return View(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Report_KPI_Filter(string filter)
        {
            try
            {
                ReportKPI_Filter _Filter = reportKPI_Filter.DeserializeFilter(filter);

                Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                ViewBag.AuthorizeId = db.Users
                    .Where(w => w.User_Id == userId)
                    .Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id)
                    .FirstOrDefault();

                ViewBag.UserList = data.SelectListItems_UsersDepartment();

                return View(_Filter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Report_KPI_JoinTeam(Guid id, string filter)
        {
            try
            {
                ReportKPI_Filter _Filter = reportKPI_Filter.DeserializeFilter(filter);

                return View(data.Services_ViewJoinTeamList(id, _Filter));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Report_KPI_Table(string filter)
        {
            try
            {
                ReportKPI_Filter _Filter = reportKPI_Filter.DeserializeFilter(filter);

                return View(data.ClsReportKPI_ViewList(_Filter));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Report_KPI_Unsatisfied(string filter)
        {
            try
            {
                ReportKPI_Filter _Filter = reportKPI_Filter.DeserializeFilter(filter);

                return View(data.ClsReport_KPI_Unsatisfied(_Filter));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Report_KPI_View(Guid id, string filter)
        {
            try
            {
                ReportKPI_Filter _Filter = reportKPI_Filter.DeserializeFilter(filter);

                return View(data.ReportKPI_User_Views(id, _Filter));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult RequestChangeDue()
        {
            return View();
        }

        public async Task<ActionResult> RequestChangeDue_Accept(Guid id)
        {
            ClsSwal swal = new ClsSwal();
            MethodBase methodBase = MethodBase.GetCurrentMethod();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.ServiceChangeDueDate_Accept(id, methodBase.Name))
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
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
                    MethodBase methodBase = MethodBase.GetCurrentMethod();
                    if (await data.ServiceChangeDueDate_Request(model, methodBase.Name))
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> RequestChangeDue_Reject(Guid id)
        {
            ClsSwal swal = new ClsSwal();
            MethodBase methodBase = MethodBase.GetCurrentMethod();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.ServiceChangeDueDate_Reject(id, methodBase.Name))
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
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

        public async Task<JsonResult> ResendEmail(Guid id, string method)
        {
            ClsSwal swal = new ClsSwal();
            try
            {
                Log_SendEmail log_SendEmail = new Log_SendEmail();
                log_SendEmail = db.Log_SendEmails
                    .Where(w => w.SendEmail_MethodName == method && w.SendEmail_Ref_Id == id)
                    .FirstOrDefault();
                if (log_SendEmail != null)
                {
                    if (await mail.ResendMail(log_SendEmail.SendEmail_Id))
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
                else
                {
                    swal.Icon = "warning";
                    swal.Text = "ไม่พบประวัติการส่งอีเมล";
                    swal.Title = "Warning";
                }
            }
            catch (Exception ex)
            {
                swal.Title = ex.Source;
                swal.Text = ex.Message;
                Exception inner = ex.InnerException;
                while (inner != null)
                {
                    swal.Title = inner.Source;
                    swal.Text += string.Format("\n{0}", inner.Message);
                    inner = inner.InnerException;
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ServiceInfomation(Guid id)
        {
            try
            {
                Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                ViewBag.AuthorizeIndex = db.Users
                .Where(w => w.User_Id == userId)
                .Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id)
                .FirstOrDefault();

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
                        swal.Text = ex.Message;
                        Exception inner = ex.InnerException;
                        while (inner != null)
                        {
                            swal.Title = inner.Source;
                            swal.Text += string.Format("\n{0}", inner.Message);
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
                    MethodBase methodBase = MethodBase.GetCurrentMethod();
                    if (await data.Services_SetApprove(model, methodBase.Name))
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetAssign(Guid id)
        {
            try
            {
                Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                string deptName = db.Users.Find(userId).Master_Processes.Master_Sections.Master_Departments.Department_Name;
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
                        MethodBase methodBase = MethodBase.GetCurrentMethod();
                        if (await data.Services_SetToUser(model.Service_Id, model.User_Id, methodBase.Name))
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
                        swal.Text = ex.Message;
                        Exception inner = ex.InnerException;
                        while (inner != null)
                        {
                            swal.Title = inner.Source;
                            swal.Text += string.Format("\n{0}", inner.Message);
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
                        db.Entry(services).State = System.Data.Entity.EntityState.Modified;
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetClose(Guid id)
        {
            ClsInquiryTopics clsInquiryTopics = new ClsInquiryTopics
            {
                Services = db.Services.Find(id),
                List_Master_InquiryTopics = db.Master_InquiryTopics.OrderBy(o => o.InquiryTopic_Index).ToList()
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }
            return Json(swal, JsonRequestBehavior.AllowGet);
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

                    MethodBase methodBase = MethodBase.GetCurrentMethod();
                    if (await data.Services_SetComplete(model, methodBase.Name))
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetInProgress(Guid id)
        {
            try
            {
                Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                string secName = db.Users.Find(userId)
                    .Master_Processes.Master_Sections.Section_Name;
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
                        swal.Text = ex.Message;
                        Exception inner = ex.InnerException;
                        while (inner != null)
                        {
                            swal.Title = inner.Source;
                            swal.Text += string.Format("\n{0}", inner.Message);
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

            ViewBag.sendTo = db.UserDetails.Where(w => sendTo.Contains(w.User_Id)).Select(s => s.Detail_EN_FirstName + " " + s.Detail_EN_LastName);

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
                    MethodBase methodBase = MethodBase.GetCurrentMethod();
                    if (await data.Services_SetRequired(model, methodBase.Name))
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
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
                    foreach (var item in db.Services.Where(w => statusId.Contains(w.Status_Id)))
                    {
                        DateTime completeDate = db.ServiceComments
                            .Where(w => w.Service_Id == item.Service_Id && w.Comment_Content.StartsWith(msg))
                            .Select(s => s.Create)
                            .FirstOrDefault();

                        if (completeDate.Date > item.Service_DueDate)
                        {
                            item.Is_OverDue = true;
                            db.Entry(item).State = System.Data.Entity.EntityState.Modified;
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
                    MethodBase methodBase = MethodBase.GetCurrentMethod();
                    if (await data.Services_SetPending(model, methodBase.Name))
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
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
                    MethodBase methodBase = MethodBase.GetCurrentMethod();
                    if (await data.Services_SetReject(model, methodBase.Name))
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
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
                    MethodBase methodBase = MethodBase.GetCurrentMethod();
                    if (await data.Services_SetReturnAssign(model, methodBase.Name))
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
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
                    MethodBase methodBase = MethodBase.GetCurrentMethod();
                    if (await data.Services_SetReturnJob(model, methodBase.Name))
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
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public string Users_GetName(Guid id)
        {
            try
            {
                return db.UserDetails
                    .Where(w => w.User_Id == id)
                    .Select(s => new { Data = s.Detail_EN_FirstName })
                    .Select(s => s.Data)
                    .FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
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
