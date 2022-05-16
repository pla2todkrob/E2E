using E2E.Models;
using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private clsContext db = new clsContext();

        public ActionResult Index()
        {
            return View();
        }
    }
}