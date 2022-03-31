﻿using E2E.Models;
using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    [Authorize]
    public class ConfigurationsController : Controller
    {
        private clsContext db = new clsContext();

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
            int res = new int();
            res = 1;
            return PartialView("_Navbar", res);
        }

        public ActionResult _Profile()
        {
            try
            {
                string res = string.Empty;
                Guid id = Guid.Parse(HttpContext.User.Identity.Name);
                res = db.Users
                    .Where(w => w.User_Id == id)
                    .Select(s => s.User_Code)
                    .FirstOrDefault();

                return PartialView("_Profile", res);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}