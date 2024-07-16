using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class MastersController : BaseController
    {
        private readonly ClsApi api = new ClsApi();
        private readonly ClsManageService clsManageService = new ClsManageService();
        private readonly ClsManageMaster data = new ClsManageMaster();

        public ActionResult Categories()
        {
            return View();
        }

        public ActionResult Categories_Create(Guid? id)
        {
            ViewBag.IsNew = true;
            if (id.HasValue)
            {
                var query = db.Master_Categories.Find(id);

                ViewBag.IsNew = false;

                return View(query);
            }

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Categories_Create(Master_Categories model)
        {
            ClsSwal swal = new ClsSwal();
            if (model != null)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Categories_Save(model))
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

        [HttpDelete]
        public ActionResult Categories_Delete(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                ClsSwal swal = new ClsSwal();
                try
                {
                    ClsSaveResult clsSaveResult = data.Categories_Delete(id);
                    if (clsSaveResult.IsSuccess)
                    {
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Text = clsSaveResult.Message;
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

        public ActionResult Categories_Table()
        {
            var query = db.Master_Categories.ToList();
            return View(query);
        }

        public ActionResult DeleteWrongData()
        {
            return View(new DateTime());
        }

        [HttpPost]
        public ActionResult DeleteWrongData(DateTime date)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    List<Master_LineWorks> master_LineWorks = new List<Master_LineWorks>();
                    master_LineWorks = db.Master_LineWorks
                        .Where(w => w.Create >= date)
                        .ToList();

                    db.Master_LineWorks.RemoveRange(master_LineWorks);

                    List<Master_Plants> master_Plants = new List<Master_Plants>();
                    master_Plants = db.Master_Plants
                        .Where(w => w.Create >= date)
                        .ToList();
                    db.Master_Plants.RemoveRange(master_Plants);

                    if (db.SaveChanges() > 0)
                    {
                        scope.Complete();
                    }

                    return View();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public ActionResult Departments()
        {
            return View();
        }

        public ActionResult Departments_Table()
        {
            return View(data.Department_GetAll());
        }

        public ActionResult Divisions()
        {
            return View();
        }

        public ActionResult Divisions_Table()
        {
            return View(data.Division_GetAll());
        }

        public ActionResult Grades()
        {
            return View();
        }

        public ActionResult Grades_Table()
        {
            return View(data.Grades_GetAll());
        }

        public ActionResult Index()
        {
            return RedirectToAction("LineWorks");
        }

        public ActionResult InquiryTopic()
        {
            return View();
        }

        public ActionResult CHK_INDEX(int id)
        {
            int Count = db.Master_InquiryTopics.Where(w => w.Program_Id == id).Count() + 1;

            return Json(Count, JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        public ActionResult InquiryTopic_Delete(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                ClsSwal swal = new ClsSwal();
                try
                {
                    ClsSaveResult clsSaveResult = data.InquiryTopic_Delete(id);
                    if (clsSaveResult.IsSuccess)
                    {
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Text = clsSaveResult.Message;
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

        public ActionResult InquiryTopic_Form(Guid? id)
        {
            bool isNew = new bool();
            Master_InquiryTopics master_InquiryTopics = new Master_InquiryTopics
            {
                InquiryTopic_Index = 1
            };

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
            ViewBag.ProgramList = db.System_Programs.Select(s => new SelectListItem()
            {
                Text = s.Program_Name,
                Value = s.Program_Id.ToString()
            }).ToList();

            return View(master_InquiryTopics);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult InquiryTopic_Form(Master_InquiryTopics model)
        {
            ClsSwal swal = new ClsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.InquiryTopic_Save(model))
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

        public ActionResult InquiryTopic_Table()
        {
            return View(data.InquiryTopics_GetAll());
        }

        public ActionResult LineWorks()
        {
            return View();
        }

        public ActionResult LineWorks_Table()
        {
            return View(data.LineWorks_GetAll());
        }

        public ActionResult Plants()
        {
            return View();
        }

        public ActionResult Plants_Form(Guid? id)
        {
            bool isNew = true;
            PlantDetail plantDetail = new PlantDetail();
            if (id.HasValue)
            {
                plantDetail = data.Plant_Get(id.Value);

                if (plantDetail == null)
                {
                    plantDetail = new PlantDetail
                    {
                        Plant_Id = id.Value,
                        Master_Plants = db.Master_Plants.Find(id)
                    };
                }

                isNew = false;
            }

            ViewBag.IsNew = isNew;

            return View(plantDetail);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Plants_Form(PlantDetail model)
        {
            ClsSwal swal = new ClsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Plant_Save(model))
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
            return View(data.Process_GetAll());
        }

        public ActionResult Sections()
        {
            return View();
        }

        public ActionResult Sections_Table()
        {
            return View(data.Section_GetAll());
        }

        public ActionResult Users()
        {
            return View();
        }

        [HttpDelete]
        public ActionResult Users_Delete(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                ClsSwal swal = new ClsSwal();
                try
                {
                    ClsSaveResult clsSaveResult = data.Users_Delete(id);
                    if (clsSaveResult.IsSuccess)
                    {
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Text = clsSaveResult.Message;
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

        public ActionResult Users_Form(Guid? id)
        {
            ViewBag.RoleList = data.SelectListItems_Role();
            ViewBag.LineWorkList = data.SelectListItems_LineWork();
            ViewBag.GradeList = new List<SelectListItem>();
            ViewBag.PlantList = data.SelectListItems_Plant();
            ViewBag.DivisionList = data.SelectListItems_Division();
            ViewBag.DepartmentList = new List<SelectListItem>();
            ViewBag.SectionList = new List<SelectListItem>();
            ViewBag.ProcessList = new List<SelectListItem>();

            ViewBag.PrefixTHList = data.SelectListItems_PrefixTH();
            ViewBag.PrefixENList = data.SelectListItems_PrefixEN();
            ViewBag.IsNew = true;

            UserDetails userDetails = new UserDetails
            {
                Users = new Users()
            };
            if (id.HasValue)
            {
                userDetails = data.UserDetails_Get(id.Value);
                ViewBag.GradeList = data.SelectListItems_Grade(userDetails.Users.Master_Grades.LineWork_Id);
                ViewBag.DivisionList = data.SelectListItems_Division();
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
            ClsSwal swal = new ClsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Users_Save(model))
                        {
                            scope.Complete();
                            swal.DangerMode = false;
                            swal.Icon = "success";
                            swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.Title = "Successful";
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

        public ActionResult Users_GetSelectDepartments(Guid? id)
        {
            return Json(data.SelectListItems_Department(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Users_GetSelectDivisions()
        {
            return Json(data.SelectListItems_Division(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Users_GetSelectGrades(Guid? id)
        {
            return Json(data.SelectListItems_Grade(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Users_GetSelectProcesses(Guid? id)
        {
            return Json(data.SelectListItems_Process(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Users_GetSelectSections(Guid? id)
        {
            return Json(data.SelectListItems_Section(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Users_GetSelectUsers(Guid? id)
        {
            return Json(data.SelectListItems_Users(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Users_Table()
        {
            return View(data.ClsUsers_GetAllView());
        }

        public ActionResult Users_Upload()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Users_UploadExcel()
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
                    var files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];

                        if (file.ContentLength > 0)
                        {
                            ClsServiceFile clsServiceFile = new ClsServiceFile()
                            {
                                Filename = file.FileName,
                                FolderPath = "Users"
                            };
                            
                            var fileRes = await api.UploadFile(clsServiceFile, file);
                            string fileUrl = fileRes.FileUrl;
                            UserUploadHistory userUploadHistory = new UserUploadHistory
                            {
                                UserUploadHistoryFile = fileUrl,
                                UserUploadHistoryFileName = Path.GetFileName(fileUrl),
                                User_Id = loginId
                            };
                            db.Entry(userUploadHistory).State = System.Data.Entity.EntityState.Added;
                            if (await db.SaveChangesAsync() > 0)
                            {
                                MemoryStream fileStream = new MemoryStream(fileRes.FileBytes);
                                List<string> userCodeList = await data.Users_ReadFile(fileStream);
                                if (await data.Users_AdjustMissing(userCodeList))
                                {
                                    scope.Complete();
                                    swal.Icon = "success";
                                    swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                                    swal.Title = "Successful";
                                    swal.DangerMode = false;
                                }
                                else
                                {
                                    swal.Icon = "warning";
                                    swal.Text = "กรุณาตรวจสอบข้อมูลอีกครั้ง";
                                    swal.Title = "Warning";
                                }
                            }
                        }
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
    }
}
