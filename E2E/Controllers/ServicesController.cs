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
        public ActionResult Form(Guid? id)
        {
            try
            {
                ViewBag.PriorityList = data.SelectListItems_Priority();
                ViewBag.RefServiceList = data.SelectListItems_RefService();
                ViewBag.UserList = data.SelectListItems_User();
                bool isNew = true;
                Services services = new Services();
                services.User_Id = Guid.Parse(HttpContext.User.Identity.Name);
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
        public ActionResult Commit_Required(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                clsSwal swal = new clsSwal();
                try
                {
                    if (data.Services_SetRequired(id))
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
        public ActionResult Action(Guid id)
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
        public ActionResult Action_Set(Guid id)
        {
            try
            {
                return View(data.Services_View(id));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Action_Set(Services model)
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
        public ActionResult Approve()
        {
            return View();
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
        public ActionResult Approve_Set(Guid id)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.Services_SetApprove(id))
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
        public ActionResult ServiceInfomation(Guid id)
        {
            try
            {
                
                return View(data.ClsServices_View(id));
            }
            catch (Exception)
            {
                throw;
            }
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
        public ActionResult SetComplete(Guid id)
        {
            ServiceComments serviceComments = new ServiceComments();
            serviceComments.Service_Id = id;

            return View(serviceComments);
        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult SetComplete(ServiceComments model)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
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
        public ActionResult RequestChangeDue()
        {
            return View();
        }
        public ActionResult RequestChangeDue_Form(Guid id)
        {
            return View(data.ServiceChangeDueDate_New(id));
        }

        [HttpPost,ValidateAntiForgeryToken]
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
        public ActionResult GetOwnerUser(Guid id)
        {
            try
            {
                Guid? res = db.Services.Find(id).User_Id;
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}