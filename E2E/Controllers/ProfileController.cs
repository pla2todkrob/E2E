using E2E.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class ProfileController : Controller
    {
        private clsManageService service = new clsManageService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyRequest()
        {
            return View();
        }

        public ActionResult MyRequest_Table()
        {
            return View(service.Services_GetMyRequest());
        }

        public ActionResult MyTask()
        {
            return View();
        }
    }
}