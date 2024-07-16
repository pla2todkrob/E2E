using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class ManagementController : BaseController
    {
        private readonly ClsManageManagement data = new ClsManageManagement();
        private readonly ClsManageMaster master = new ClsManageMaster();
        private readonly ClsApi api = new ClsApi();

        public ActionResult AuditReport()
        {
            return View();
        }

        public async Task<ActionResult> AuditReport_Action(string id, string emails = "")
        {
            try
            {
                List<Guid> guids = JsonConvert.DeserializeObject<List<Guid>>(id);

                List<ZipKey> zipKeys = new List<ZipKey>();

                foreach (var item in guids)
                {
                    List<ServiceDocuments> serviceDocuments = await db.ServiceDocuments
                        .Where(w => w.Service_Id == item)
                        .ToListAsync();
                    if (serviceDocuments.Count() > 0)
                    {
                        ZipKey zipKey = new ZipKey
                        {
                            ServiceKey = await db.Services
                            .Where(w => w.Service_Id == item)
                            .Select(s => s.Service_Key)
                            .FirstOrDefaultAsync(),
                            ZipKeyItems = serviceDocuments
                            .Select(s => new ZipKeyItem()
                            {
                                DocumentFilePath = s.ServiceDocument_Path,
                                DocumentFileName = s.ServiceDocument_Name,
                                DocumentName = s.Master_Documents.Document_Name
                            }).ToList()
                        };
                        zipKeys.Add(zipKey);
                    }
                }

                string zipName = DateTime.Now.ToString("yyyyMMdd") + ".zip"; // Use a date format that is safe for file names
                string zipPath = Path.Combine(Path.GetTempPath(), zipName); // Temp path or any desired path

                using (var zipArchive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    foreach (var zipKey in zipKeys)
                    {
                        foreach (var item in zipKey.ZipKeyItems)
                        {
                            string entryName = Path.Combine(zipKey.ServiceKey, Path.GetFileName(item.DocumentFilePath));
                            zipArchive.CreateEntryFromFile(item.DocumentFilePath, entryName);
                        }
                    }
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(zipPath);

                if (string.IsNullOrEmpty(emails))
                {
                    return File(fileBytes, "application/zip", zipName);
                }
                else
                {
                    string[] arrEmail = emails.Split(';').ToArray();

                    string subject = "[E2E][Document controls] นำส่งเอกสาร";
                    string body = string.Format("<p><b>Attached file: </b> {0}.zip</p>", zipName);
                    body += string.Format("<b>Include {0} job.</b><br />", zipKeys.Count());
                    body += "<table>";

                    body += "<thead>";
                    body += "<tr>";
                    body += "<th>Key</th>";
                    body += "<th>Document type</th>";
                    body += "<th>Document name</th>";
                    body += "</tr>";
                    body += "</thead>";

                    body += "<tbody>";

                    var groupedItems = zipKeys.GroupBy(item => item.ServiceKey);

                    foreach (var group in groupedItems)
                    {
                        bool firstRow = true; // Flag to indicate the first row for each group
                        foreach (var item2 in group.SelectMany(g => g.ZipKeyItems))
                        {
                            body += "<tr>";

                            if (firstRow)
                            {
                                // Merge cell for ServiceKey column
                                body += $"<td rowspan=\"{group.SelectMany(g => g.ZipKeyItems).Count()}\">{group.Key}</td>";
                                firstRow = false; // Reset the flag after the first row
                            }

                            body += $"<td>{item2.DocumentName}</td>";
                            body += $"<td>{item2.DocumentFileName}</td>";
                            body += "</tr>";
                        }
                    }

                    body += "</tbody>";

                    body += "</table>";

                    body += "<b>Please do not reply to this mail. Thank you</b>";

                    ClsServiceEmail serviceEmail = new ClsServiceEmail()
                    {
                        Body = body,
                        Subject = subject,
                        SendTo = arrEmail
                    };
                    serviceEmail.ClsFileAttaches.Add(new ClsFileAttach()
                    {
                        Base64 = Convert.ToBase64String(fileBytes),
                        FileName = zipName
                    });

                    bool isSuccess = await api.SendMail(serviceEmail);
                    if (isSuccess)
                    {
                        return RedirectToAction("AuditReport");
                    }

                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Email sending failed. Please notify the administrator.");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.GetBaseException().Message);
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
                string deptName = db.Users.Find(loginId).Master_Processes.Master_Sections.Master_Departments.Department_Name;
                List<Guid> deptIds = db.Master_Departments.Where(w => w.Department_Name == deptName).Select(s => s.Department_Id).ToList();
                IQueryable<Services> query = db.Services
                    .Where(w => finishIds.Contains(w.Status_Id) && deptIds.Contains(w.Department_Id.Value));
                _Filter.Date_To = _Filter.Date_To.AddDays(1);
                query = query.Where(w => w.Update >= _Filter.Date_From && w.Update <= _Filter.Date_To);

                List<Guid> hasDocIds = db.ServiceDocuments
                    .Where(w => query.Select(s => s.Service_Id).Contains(w.Service_Id))
                    .Select(s => s.Service_Id)
                    .Distinct()
                    .ToList();

                var reportList = query
                    .Where(w => hasDocIds.Contains(w.Service_Id))
                    .Select(s => new
                    {
                        s.Service_Id,
                        s.Service_Key,
                        s.Service_Subject,
                        s.WorkRoots,
                        s.Action_User_Id,
                        s.Update
                    }).OrderBy(o => o.Update).ToList();

                List<ClsAuditReport> clsAuditReports = reportList
                    .Select(s => new ClsAuditReport()
                    {
                        Service_Id = s.Service_Id,
                        Service_Key = s.Service_Key,
                        Service_Subject = s.Service_Subject,
                        WorkRoots = s.WorkRoots,
                        User_Name = master.Users_GetInfomation(s.Action_User_Id.Value),
                        Update = s.Update.Value
                    }).OrderBy(o => o.Update).ToList();

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
        public async Task<ActionResult> DocumentControl_Create(ClsDocuments model)
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
                        if (await data.Document_Save(model, Request.Files))
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
                    swal.Text = ex.GetBaseException().Message;
                }

                return Json(swal, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DocumentControl_Table()
        {

            string DeptName = db.Users.Where(w => w.User_Id == loginId).Select(s => s.Master_Processes.Master_Sections.Master_Departments.Department_Name).FirstOrDefault();
            List<Guid> guids = new List<Guid>();
            guids = db.Master_Departments.Where(w => w.Department_Name == DeptName).Select(s => s.Department_Id).ToList();

            var sql = db.Master_Documents.Where(w => guids.Contains(w.Users.Master_Processes.Master_Sections.Department_Id)).ToList();

            return View(sql);
        }

        public ActionResult DownloadTemplate(Guid id)
        {
            // Fetch the document version based on the provided ID
            Master_DocumentVersions master_DocumentVersions = db.Master_DocumentVersions
                .Where(w => w.Document_Id == id)
                .OrderByDescending(o => o.DocumentVersion_Number)
                .FirstOrDefault();

            if (master_DocumentVersions != null)
            {
                string filePath = master_DocumentVersions.DocumentVersion_Path;
                string fileName = Path.GetFileName(filePath);

                // Check if the file exists
                if (System.IO.File.Exists(filePath))
                {
                    // Get the file content
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                    // Return the file for download
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {
                    // Handle file not found scenario
                    return HttpNotFound("File not found.");
                }
            }
            else
            {
                // Handle document version not found scenario
                return HttpNotFound("Document version not found.");
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
                    swal.Text = ex.GetBaseException().Message;
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
                string deptName = db.Users
                    .Where(w => w.User_Id == loginId)
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

        public ActionResult WorkRoot_Table()
        {
            try
            {
                string deptName = db.Users
                    .Where(w => w.User_Id == loginId)
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

    public class ZipKey
    {
        public string ServiceKey { get; set; }
        public List<ZipKeyItem> ZipKeyItems { get; set; }
    }

    public class ZipKeyItem
    {
        public string DocumentName { get; set; }
        public string DocumentFilePath { get; set; }
        public string DocumentFileName { get; set; }
    }
}
