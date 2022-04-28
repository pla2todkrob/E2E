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
    [Authorize]
    public class ServicesController : Controller
    {
        private clsManageService data = new clsManageService();
        private clsContext db = new clsContext();

        public ActionResult Index()
        {
            Guid userId = Guid.Parse(HttpContext.User.Identity.Name);

            ViewBag.AuthorizeIndex = db.Users
                .Where(w => w.User_Id == userId)
                .Select(s => s.Master_LineWorks.System_Authorize.Authorize_Index)
                .FirstOrDefault();

            return View();
        }

        public ActionResult Index_Table(bool val)
        {
            try
            {
                return View(data.Services_GetAllPending(val));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Form(Guid? id)
        {
            ViewBag.PriorityList = data.SelectListItems_Priority();
            ViewBag.RefServiceList = data.SelectListItems_RefService();
            ViewBag.UserList = data.SelectListItems_User();
            bool isNew = true;
            Services services = new Services();
            if (id.HasValue)
            {
                services = data.Services_View(id.Value);
                isNew = false;
            }

            ViewBag.IsNew = isNew;

            return View(services);
        }

        public ActionResult _File(Guid id)
        {
            return PartialView("_File", data.ServiceFiles_View(id));
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
            ViewBag.PlantList = new clsManageMaster().SelectListItems_Plant();
            ViewBag.DivisionList = new List<SelectListItem>();
            ViewBag.DepartmentList = new List<SelectListItem>();
            var res = data.ClsServices_View(id);
            return View(res);
        }

        [HttpPost]
        public ActionResult Commit(clsServices model)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                clsSwal swal = new clsSwal();
                try
                {
                }
                catch (Exception)
                {
                    throw;
                }

                return Json(swal, JsonRequestBehavior.AllowGet);
            }
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

        public ActionResult _RefService(Guid id)
        {
            return PartialView("_RefService", data.ClsServices_ViewRefList(id));
        }
    }
}