using E2E.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class DepartmentsController : Controller
    {
        private clsManageService service = new clsManageService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AllTask()
        {
            return View();
        }

        public ActionResult AllRequest()
        {
            return View();
        }

        public ActionResult Approve()
        {
            return View();
        }

        public ActionResult Approve_Table(bool type)
        {
            return View(service.Services_GetAllRequiredApprove(type));
        }

        public ActionResult Approve_Form(Guid id)
        {
            return View(service.ClsServices_View(id));
        }
    }
}