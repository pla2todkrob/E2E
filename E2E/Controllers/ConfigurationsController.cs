using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    [AllowAnonymous]
    public class ConfigurationsController : Controller
    {
        private readonly ClsManageService clsManageService = new ClsManageService();
        private readonly ClsManageService data = new ClsManageService();
        private readonly ClsContext db = new ClsContext();
        private readonly ClsManageBusinessCard jobCount = new ClsManageBusinessCard();
        private readonly ClsManageMaster master = new ClsManageMaster();
        private readonly ClsUsers users = new ClsUsers();

        public ActionResult _Copyright()
        {
            System_Configurations system_Configurations = new System_Configurations();

            system_Configurations = db.System_Configurations.OrderByDescending(o => o.CreateDateTime).FirstOrDefault();
            return PartialView("_Copyright", system_Configurations);
        }

        public ActionResult _Navbar()
        {
            ClsCountNavbar res = new ClsCountNavbar
            {
                Admin = null
            };

            if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                res.ChangeDue = data.ServiceChangeDues_ListCount().Count;
                Guid id = Guid.Parse(HttpContext.User.Identity.Name);
                if (db.Users.Any(a => a.User_Id == id))
                {
                    res.Admin = db.Users
                    .Where(w => w.User_Id == id)
                    .Select(s => s.Role_Id)
                    .FirstOrDefault();
                }
                else
                {
                    users.RemoveCookie();
                }
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
                    if (db.Users.Any(a => a.User_Id == userId))
                    {
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
                    else
                    {
                        users.RemoveCookie();
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
                    if (db.Users.Any(a => a.User_Id == userId))
                    {
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
                    else
                    {
                        users.RemoveCookie();
                    }
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

            try
            {
                if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
                {
                    Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                    if (db.Users.Any(a => a.User_Id == userId))
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
                    else
                    {
                        users.RemoveCookie();
                    }
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
            ClsJobCount clsJobCount = new ClsJobCount();
            try
            {
                int? res = null;

                if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
                {
                    Guid? userId = Guid.Parse(HttpContext.User.Identity.Name);

                    if (db.Users.Any(a => a.User_Id == userId))
                    {
                        int authorIndex = db.Users
                        .Where(w => w.User_Id == userId)
                        .Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id)
                        .FirstOrDefault();

                        if (authorIndex == 3)
                        {
                            res = new ClsManageService().Services_GetWaitActionCount(Guid.Parse(HttpContext.User.Identity.Name));
                        }
                        else
                        {
                            res = new ClsManageService().Services_GetWaitCommitCount();
                            res += new ClsManageService().Services_GetWaitActionCount(Guid.Parse(HttpContext.User.Identity.Name));
                        }
                    }
                    else
                    {
                        users.RemoveCookie();
                    }

                    clsJobCount.business = jobCount.CountJob(userId.Value);
                    clsJobCount.service = res;
                    clsJobCount.total = clsJobCount.business + clsJobCount.service;
                }
                return PartialView("_NavService", clsJobCount);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult _NavReport()
        {

            try
            {
                return PartialView("_NavReport");
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
                ClsUsers clsUsers = new ClsUsers();
                if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
                {
                    Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                    if (db.Users.Any(a => a.User_Id == userId))
                    {
                        clsUsers = db.Users
                        .Where(w => w.User_Id == userId)
                        .AsEnumerable()
                        .Select(s => new ClsUsers()
                        {
                            User_Code = s.User_Code,
                            User_Point = s.User_Point
                        }).FirstOrDefault();
                    }
                    else
                    {
                        users.RemoveCookie();
                    }
                }

                return PartialView("_Profile", clsUsers);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete]
        public ActionResult Deletelogo()
        {
            ClsSwal swal = new ClsSwal();
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

                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "ลบข้อมูลเรียบร้อย";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "ลบข้อมูลไม่สำเร็จ";
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

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Table(System_Configurations model)
        {
            ClsSwal swal = new ClsSwal();
            bool res = new bool();
            HttpFileCollectionBase files = Request.Files;
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
                        System_Configurations system_Configurations = new System_Configurations();

                        if (files[0].ContentLength != 0)
                        {
                            HttpPostedFileBase file = files[0];
                            string dir = "Configurations/" + system_Configurations.Configuration_Id;
                            string FileName = file.FileName;
                            string filepath = await clsManageService.UploadFileToString(dir, file, FileName);

                            system_Configurations.Configuration_Brand = filepath;
                        }
                        else
                        {
                            system_Configurations.Configuration_Brand = model.Configuration_Brand;
                        }
                        if (!string.IsNullOrEmpty(model.Copyright))
                        {
                            system_Configurations.Copyright = model.Copyright.Trim('©').Trim();
                        }

                        if (!string.IsNullOrEmpty(model.SystemName))
                        {
                            system_Configurations.SystemName = model.SystemName.Trim();
                        }

                        system_Configurations.User_Id = Guid.Parse(System.Web.HttpContext.Current.User.Identity.Name);
                        system_Configurations.Configuration_Point = model.Configuration_Point;

                        db.System_Configurations.Add(system_Configurations);
                        if (db.SaveChanges() > 0)
                        {
                            res = true;
                        }

                        if (res)
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

        public ActionResult Table()
        {
            try
            {
                try
                {
                    List<System_Configurations> system_Configurations = db.System_Configurations
                        .OrderByDescending(o => o.CreateDateTime)
                        .ToList();
                    system_Configurations.ForEach(f => f.Users.User_Code = master.Users_GetInfomation(f.User_Id));
                    return View(system_Configurations);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
