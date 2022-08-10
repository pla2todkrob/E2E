using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class ManagementController : Controller
    {
        // GET: Management
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DocumentControl()
        {
            return View();
        }

        public ActionResult DocumentControl_Table()
        {
            return View();
        }
    }
}