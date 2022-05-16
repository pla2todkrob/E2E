﻿using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    [AllowAnonymous]
    public class ConfigurationsController : Controller
    {
        private clsContext db = new clsContext();

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _NavbarBrand()
        {
            string res = string.Empty;
            System_Configurations system_Configurations = new System_Configurations();
            system_Configurations = db.System_Configurations
                .OrderByDescending(o => o.CreateDateTime)
                .FirstOrDefault();
            if (system_Configurations != null)
            {
                res = system_Configurations.Configuration_Brand;
            }
            return PartialView("_NavbarBrand", res);
        }

        public ActionResult _Navbar()
        {
            int? res = null;
            if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                Guid id = Guid.Parse(HttpContext.User.Identity.Name);
                res = db.Users
                    .Where(w => w.User_Id == id)
                    .Select(s => s.System_Roles.Role_Index)
                    .FirstOrDefault();
            }

            return PartialView("_Navbar", res);
        }

        public ActionResult _Profile()
        {
            try
            {
                clsUsers clsUsers = new clsUsers();
                Guid id = Guid.Parse(HttpContext.User.Identity.Name);
                clsUsers = db.Users
                    .Where(w => w.User_Id == id)
                    .Select(s => new clsUsers()
                    {
                        User_Code = s.User_Code,
                        User_Point = s.User_Point
                    }).FirstOrDefault();

                return PartialView("_Profile", clsUsers);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult _NavService()
        {
            try
            {
                int? res = null;
                if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
                {
                    Guid userId = Guid.Parse(HttpContext.User.Identity.Name);

                    int authorIndex = db.Users
                        .Where(w => w.User_Id == userId)
                        .Select(s => s.Master_Grades.Master_LineWorks.System_Authorize.Authorize_Index)
                        .FirstOrDefault();

                    if (authorIndex == 3)
                    {
                        res = new clsManageService().Services_GetWaitActionCount(Guid.Parse(HttpContext.User.Identity.Name));
                    }
                    else
                    {
                        res = new clsManageService().Services_GetWaitCommitCount();
                        res += new clsManageService().Services_GetWaitActionCount(Guid.Parse(HttpContext.User.Identity.Name));
                    }
                }

                return PartialView("_NavService", res);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult _NavDepartment()
        {
            try
            {
                int? res = null;
                if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
                {
                    Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                    Guid deptId = db.Users.Find(userId).Master_Processes.Master_Sections.Department_Id.Value;

                    int authorIndex = db.Users
                        .Where(w => w.User_Id == userId)
                        .Select(s => s.Master_Grades.Master_LineWorks.System_Authorize.Authorize_Index)
                        .FirstOrDefault();

                    if (authorIndex != 3)
                    {
                        res = db.Services
                            .Where(w => w.Required_Approve_User_Id.HasValue &&
                            !w.Approved_User_Id.HasValue &&
                            w.Users.Master_Processes.Master_Sections.Department_Id == deptId).Count();
                    }
                }

                return PartialView("_NavDepartment", res);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}