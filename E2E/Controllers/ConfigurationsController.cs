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
    public class ConfigurationsController : BaseController
    {
        private static readonly ClsAssembly clsAssembly = new ClsAssembly();
        private readonly ClsManageService clsManageService = new ClsManageService();
        private readonly ClsManageBusinessCard jobCount = new ClsManageBusinessCard();
        private readonly ClsManageMaster master = new ClsManageMaster();
        private readonly ClsUsers users = new ClsUsers();

        public ActionResult _Copyright()
        {
            return PartialView("_Copyright", $"{clsAssembly.Description} ({clsAssembly.Product}) Version {clsAssembly.Version} {clsAssembly.Copyright} - {clsAssembly.Company}");
        }

        public ActionResult _Navbar()
        {
            ClsCountNavbar res = new ClsCountNavbar
            {
                Admin = null
            };

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                res.ChangeDue = clsManageService.ServiceChangeDues_ListCount().Count;
                if (db.Users.Any(a => a.User_Id == loginId))
                {
                    res.Admin = db.Users
                    .Where(w => w.User_Id == loginId)
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
            string logo = db.System_Configurations
                .OrderByDescending(o => o.CreateDateTime)
                .Select(s => s.Configuration_Brand)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(logo))
            {
                logo = clsAssembly.Logo;
            }

            return PartialView("_NavbarBrand", logo);
        }

        public ActionResult _NavDepartment()
        {
            try
            {
                int? res = null;
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    if (db.Users.Any(a => a.User_Id == loginId))
                    {
                        Guid deptId = db.Users.Find(loginId).Master_Processes.Master_Sections.Department_Id;


                        if (authId != 3)
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

                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    if (db.Users.Any(a => a.User_Id == loginId))
                    {
                        

                        ViewBag.Author = authId;
                        string deptName = db.Users.Find(loginId).Master_Processes.Master_Sections.Master_Departments.Department_Name;
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
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    if (db.Users.Any(a => a.User_Id == loginId))
                    {
                        
                        if (authId == 2)
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

        public ActionResult _NavService()
        {
            ClsJobCount clsJobCount = new ClsJobCount();
            try
            {
                int? res = null;

                if (HttpContext.User.Identity.IsAuthenticated)
                {

                    if (db.Users.Any(a => a.User_Id == loginId))
                    {
                        

                        if (authId == 3)
                        {
                            res = clsManageService.Services_GetWaitActionCount(loginId);
                        }
                        else
                        {
                            res = clsManageService.Services_GetWaitCommitCount();
                            res += clsManageService.Services_GetWaitActionCount(loginId);
                        }
                    }
                    else
                    {
                        users.RemoveCookie();
                    }

                    clsJobCount.Business = jobCount.CountJob(loginId);
                    clsJobCount.Service = res;
                    clsJobCount.Total = clsJobCount.Business + clsJobCount.Service;
                }
                return PartialView("_NavService", clsJobCount);
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
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    if (db.Users.Any(a => a.User_Id == loginId))
                    {
                        clsUsers = db.Users
                        .Where(w => w.User_Id == loginId)
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
                    swal.Text = ex.GetBaseException().Message;
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
