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
    public class ServicesController : Controller
    {
        private clsManageService data = new clsManageService();
        private clsContext db = new clsContext();
        private clsServiceFTP ftp = new clsServiceFTP();

        public ActionResult _AddTeam(Guid id)
        {
            try
            {
                ViewBag.TeamList = data.SelectListItems_Team(id);
                clsServiceTeams clsServiceTeams = new clsServiceTeams();
                clsServiceTeams.Service_Id = id;
                return PartialView("_AddTeam", clsServiceTeams);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult _AddTeam(clsServiceTeams model)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.Service_AddTeam(model))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _Comment(Guid id)
        {
            try
            {
                ServiceComments serviceComments = new ServiceComments();
                serviceComments.Service_Id = id;
                return PartialView("_Comment", serviceComments);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult _Comment(ServiceComments model)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.Services_Comment(model, Request.Files))
                    {
                        scope.Complete();
                        swal.icon = "success";

                        swal.option = model;
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

        public ActionResult _DeleteTeam(Guid id)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.Service_DeleteTeam(id))
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
                return View(data.ClsServices_View(id));
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
                return View(db.Services.OrderByDescending(o => o.Priority_Id).ThenBy(t => new { t.Service_DueDate, t.Create }).ToList());
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
                return View(data.Services_GetDepartmentTask());
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
            clsSwal res = new clsSwal();

            Guid userId = Guid.Parse(HttpContext.User.Identity.Name);

            var ID_service = data.Service_CHK_CloseJob(userId);

            if (ID_service != null)
            {
                res.icon = "warning";
                res.dangerMode = true;
                res.text = "You have a job that hasn't been closed.";
                res.title = "Please close job";
                res.option = ID_service;
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Commit(Guid id)
        {
            try
            {
                ViewBag.PlantList = new clsManageMaster().SelectListItems_Plant();
                return View(data.ClsServices_View(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Commit(clsServices model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Services_SetCommit(model.Services))
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

        public ActionResult Commit_ToDepartment(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                clsSwal swal = new clsSwal();
                try
                {
                    if (data.Services_SetToDepartment(id))
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
                return Json(swal, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteFile(Guid id)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.ServiceFiles_Delete(id))
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

        public void DownloadDocumentControl(Guid id)
        {
            try
            {
                string key = db.Services.Find(id).Service_Key;
                string dir = string.Format("Service/{0}/DocumentControls/", key);
                ftp.Ftp_DownloadFolder(dir, key);
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
                Services services = new Services();
                services.User_Id = userId;
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
        public ActionResult Form(Services model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Services_Save(model, Request.Files))
                        {
                            scope.Complete();
                            swal.dangerMode = false;
                            swal.icon = "success";
                            swal.text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.title = "Successful";
                            swal.option = model.Service_Id;
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

        public ActionResult Form_Forward(Guid id)
        {
            try
            {
                ViewBag.PriorityList = data.SelectListItems_Priority();
                Services services = new Services();
                services.Ref_Service_Id = id;
                services.User_Id = db.Services.Find(id).User_Id;

                return View(services);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Form_Forward(Services model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        swal = data.CheckMissingDocument(model.Ref_Service_Id.Value);
                        if (swal.option == false)
                        {
                            return Json(swal, JsonRequestBehavior.AllowGet);
                        }

                        if (data.Services_Save(model, Request.Files, true))
                        {
                            scope.Complete();
                            swal.option = null;
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
                return View(data.Services_GetWaitActionList(Guid.Parse(HttpContext.User.Identity.Name)));
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
                return View(data.Services_GetWaitCommitList());
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

        public ActionResult RequestChangeDue()
        {
            return View();
        }

        public ActionResult RequestChangeDue_Accept(Guid id)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.ServiceChangeDueDate_Accept(id))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RequestChangeDue_Cancel(Guid id)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.ServiceChangeDueDate_Cancel(id))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RequestChangeDue_Form(Guid id)
        {
            return View(data.ServiceChangeDueDate_New(id));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RequestChangeDue_Form(ServiceChangeDueDate model)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.ServiceChangeDueDate_Request(model))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RequestChangeDue_Reject(Guid id)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.ServiceChangeDueDate_Reject(id))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RequestChangeDue_Table()
        {
            try
            {
                return View(data.ServiceChangeDues_List());
            }
            catch (Exception)
            {
                throw;
            }
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
                return View(db.ServiceDocuments.Find(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ServiceInfomation_Document(ServiceDocuments model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Services_SaveDocumentControl(model, Request.Files))
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
        public ActionResult SetApproved(ServiceComments model)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.Services_SetApprove(model))
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

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetCancel(Guid id)
        {
            ServiceComments serviceComments = new ServiceComments();
            serviceComments.Service_Id = id;

            return View(serviceComments);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetCancel(ServiceComments model)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.Services_SetCancel(model))
                    {
                        scope.Complete();
                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                        swal.title = "Successful";
                        swal.option = model.Service_Id;
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetClose(Guid id)
        {
            clsInquiryTopics clsInquiryTopics = new clsInquiryTopics();
            clsInquiryTopics.Services = db.Services.Find(id);
            clsInquiryTopics.List_Master_InquiryTopics = db.Master_InquiryTopics.OrderBy(o => o.InquiryTopic_Index).ToList();

            return View(clsInquiryTopics);
        }

        [HttpPost]
        public ActionResult SetClose(Guid id, List<clsEstimate> score)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.SaveEstimate(id, score))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetComplete(Guid id)
        {
            ServiceComments serviceComments = new ServiceComments();
            serviceComments.Service_Id = id;

            return View(serviceComments);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetComplete(ServiceComments model)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    swal = data.CheckMissingDocument(model.Service_Id);
                    if (swal.option == false)
                    {
                        return Json(swal, JsonRequestBehavior.AllowGet);
                    }

                    if (data.Services_SetComplete(model))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetFreePoint(Guid id)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.Services_SetFreePoint(id))
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
        public ActionResult SetInProgress(Services model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Services_SetAction(model))
                        {
                            scope.Complete();
                            swal.dangerMode = false;
                            swal.icon = "success";
                            swal.text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.title = "Successful";
                            swal.option = model.Service_Id;
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
        public ActionResult SetMustApprove(ServiceComments model)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.Services_SetRequired(model))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetPending(ServiceComments model)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.Services_SetPending(model))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetReject(Guid id)
        {
            try
            {
                ServiceComments serviceComments = new ServiceComments();
                serviceComments.Service_Id = id;

                return View(serviceComments);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetReject(ServiceComments model)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.Services_SetReject(model))
                    {
                        scope.Complete();
                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                        swal.title = "Successful";
                        swal.option = model.Service_Id;
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetReturnJob(Guid id)
        {
            ServiceComments serviceComments = new ServiceComments();
            serviceComments.Service_Id = id;

            return View(serviceComments);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetReturnJob(ServiceComments model)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.Services_SetReturnJob(model))
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
            return Json(swal, JsonRequestBehavior.AllowGet);
        }
    }
}