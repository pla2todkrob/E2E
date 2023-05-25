using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class ManagementController : Controller
    {
        private readonly ClsManageManagement data = new ClsManageManagement();
        private readonly ClsContext db = new ClsContext();
        private readonly ClsServiceFTP ftp = new ClsServiceFTP();
        private readonly ClsManageMaster master = new ClsManageMaster();

        public ActionResult AuditReport()
        {
            return View();
        }

        public ActionResult AuditReport_Action(string id, string emails = "")
        {
            try
            {
                List<Guid> guids = JsonConvert.DeserializeObject<List<Guid>>(id);
                List<string> arrEmail = new List<string>();
                List<string> dirList = new List<string>();
                if (!string.IsNullOrEmpty(emails))
                {
                    arrEmail = emails.Split(';').ToList();
                }

                foreach (var item in guids)
                {
                    if (db.ServiceDocuments.Any(a => a.Service_Id == item && !string.IsNullOrEmpty(a.ServiceDocument_Path)))
                    {
                        string key = db.Services.Find(item).Service_Key;
                        string dir = string.Format("Service/{0}/DocumentControls/", key);
                        dirList.Add(dir);
                    }
                }

                string zipName = DateTime.Now.ToString("d").Replace("/", "");

                if (arrEmail.Count > 0)
                {
                    string subject = "[E2E][Document controls] นำส่งเอกสาร";
                    string content = string.Format("<p><b>Attached file: </b> {0}.zip</p>", zipName);
                    content += string.Format("<b>Include {0} job.</b><br />", dirList.Count());
                    content += "<p>";
                    for (int i = 1; i <= dirList.Count(); i++)
                    {
                        content += string.Format("{0}. {1}<br />", i, dirList[i - 1].Split('/').ElementAt(1));
                    }
                    content += "</p>";
                    content += "<b>Please do not reply to this mail. Thank you</b>";

                    ftp.Ftp_DownloadFolder(dirList, string.Format("AuditReport\\{0}", zipName), arrEmail, subject, content);
                }
                else
                {
                    ftp.Ftp_DownloadFolder(dirList, string.Format("AuditReport\\{0}", zipName));
                }

                return RedirectToAction("AuditReport");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult AuditReport_Email()
        {
            return View();
        }

        public ActionResult AuditReport_Filter(string filter)
        {
            try
            {
                AuditReport_Filter _Filter = new AuditReport_Filter();
                if (!string.IsNullOrEmpty(filter))
                {
                    _Filter = JsonConvert.DeserializeObject<AuditReport_Filter>(filter);
                }

                return View(_Filter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult AuditReport_Table(string filter)
        {
            try
            {
                AuditReport_Filter _Filter = new AuditReport_Filter();
                if (!string.IsNullOrEmpty(filter))
                {
                    _Filter = JsonConvert.DeserializeObject<AuditReport_Filter>(filter);
                }

                int[] finishIds = { 3, 4 };
                Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                string deptName = db.Users.Find(userId).Master_Processes.Master_Sections.Master_Departments.Department_Name;
                List<Guid> deptIds = db.Master_Departments.Where(w => w.Department_Name == deptName).Select(s => s.Department_Id).ToList();
                IQueryable<Services> query = db.Services
                    .Where(w => finishIds.Contains(w.Status_Id) && deptIds.Contains(w.Department_Id.Value));
                _Filter.Date_To = _Filter.Date_To.AddDays(1);
                query = query.Where(w => w.Create >= _Filter.Date_From);
                query = query.Where(w => w.Create <= _Filter.Date_To);

                List<Guid> hasDocIds = db.ServiceDocuments
                    .Where(w => query.Select(s => s.Service_Id).Contains(w.Service_Id))
                    .Select(s => s.Service_Id)
                    .Distinct()
                    .ToList();

                List<ClsAuditReport> clsAuditReports = query
                    .Where(w => hasDocIds.Contains(w.Service_Id))
                    .AsEnumerable()
                    .Select(s => new ClsAuditReport()
                    {
                        Create = s.Create,
                        Service_Id = s.Service_Id,
                        Service_Key = s.Service_Key,
                        Service_Subject = s.Service_Subject,
                        WorkRoots = s.WorkRoots,
                        User_Name = master.Users_GetInfomation(s.Action_User_Id.Value),
                        Update = s.Update.Value
                    }).OrderBy(o => o.Create).ToList();

                return View(clsAuditReports);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult DocumentControl()
        {
            return View();
        }

        public ActionResult DocumentControl_Create(Guid? id)
        {
            ViewBag.IsNew = true;
            ClsDocuments clsDocuments = new ClsDocuments();
            if (id.HasValue)
            {
                ViewBag.IsNew = false;
                clsDocuments.Master_Documents = db.Master_Documents.Find(id);
                clsDocuments.Master_DocumentVersions = db.Master_DocumentVersions
                    .Where(w => w.Document_Id == id)
                    .OrderByDescending(o => o.DocumentVersion_Number)
                    .ToList();
            }
            return View(clsDocuments);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DocumentControl_Create(ClsDocuments model)
        {
            ClsSwal swal = new ClsSwal();
            if (ModelState.IsValid)
            {
                TransactionOptions options = new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = TimeSpan.MaxValue
                };
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
                {
                    try
                    {
                        if (data.Document_Save(model, Request.Files))
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

        [HttpDelete]
        public ActionResult DocumentControl_Delete(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                ClsSwal swal = new ClsSwal();
                try
                {
                    if (data.Delete_Document(id))
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
                        swal.Text = "ข้อมูลถูกใช้งานอยู่";
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

        public ActionResult DocumentControl_Table()
        {
            Guid Id = Guid.Parse(System.Web.HttpContext.Current.User.Identity.Name);

            string DeptName = db.Users.Where(w => w.User_Id == Id).Select(s => s.Master_Processes.Master_Sections.Master_Departments.Department_Name).FirstOrDefault();
            List<Guid> guids = new List<Guid>();
            guids = db.Master_Departments.Where(w => w.Department_Name == DeptName).Select(s => s.Department_Id).ToList();

            var sql = db.Master_Documents.Where(w => guids.Contains(w.Users.Master_Processes.Master_Sections.Department_Id)).ToList();

            return View(sql);
        }

        public void DownloadTemplate(Guid id)
        {
            try
            {
                Master_DocumentVersions master_DocumentVersions = new Master_DocumentVersions();
                master_DocumentVersions = db.Master_DocumentVersions
                    .Where(w => w.Document_Id == id)
                    .OrderByDescending(o => o.DocumentVersion_Number)
                    .FirstOrDefault();
                if (master_DocumentVersions != null)
                {
                    new ClsServiceFTP().Ftp_DownloadFile(master_DocumentVersions.DocumentVersion_Path);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult WorkRoot()
        {
            return View();
        }

        [HttpDelete]
        public ActionResult WorkRoot_Delete(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                ClsSwal swal = new ClsSwal();
                try
                {
                    if (data.WorkRoot_Delete(id))
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
                        swal.Text = "ข้อมูลถูกใช้งานอยู่";
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

        public ActionResult WorkRoot_Form(Guid? id)
        {
            try
            {
                ClsWorkRoots clsWorkRoots = new ClsWorkRoots();
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
                    .Where(w => deptIds.Contains(w.Master_Processes.Master_Sections.Department_Id))
                    .Select(s => s.User_Id)
                    .ToList();

                List<SelectListItem> listItems = new List<SelectListItem>();
                listItems.AddRange(db.Master_Documents
                    .Where(w => userIds.Contains(w.User_Id) && w.Active)
                    .Select(s => new SelectListItem()
                    {
                        Text = s.Document_Name,
                        Value = s.Document_Id.ToString()
                    })
                    .OrderBy(o => o.Text)
                    .ToList());

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
                        listItems
                            .Where(w => clsWorkRoots.Document_Id.Contains(Guid.Parse(w.Value)))
                            .ToList()
                            .ForEach(f => f.Selected = true);
                    }
                }

                ViewBag.DocumentList = listItems;

                List<string> secNames = db.Master_Sections
                    .Where(w => deptIds.Contains(w.Department_Id) && w.Active)
                    .Select(s => s.Section_Name)
                    .OrderBy(o => o)
                    .Distinct()
                    .ToList();

                listItems = new List<SelectListItem>
                {
                    new SelectListItem()
                    {
                        Text = "Select section",
                        Value = ""
                    }
                };

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

                ViewBag.IsNew = isNew;

                return View(clsWorkRoots);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WorkRoot_Form(ClsWorkRoots model)
        {
            ClsSwal swal = new ClsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.WorkRoot_Save(model))
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
                    .Where(w => deptIds.Contains(w.Master_Processes.Master_Sections.Department_Id))
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
