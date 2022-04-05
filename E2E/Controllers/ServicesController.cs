using E2E.Models;
using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    [Authorize]
    public class ServicesController : Controller
    {
        private clsManageService data = new clsManageService();
        private clsContext db = new clsContext();

        public ActionResult Index()
        {
            Guid userId = Guid.Parse(HttpContext.User.Identity.Name);

            ViewBag.AuthorizeIndex = db.Users
                .Where(w => w.User_Id == userId)
                .Select(s => s.Master_LineWorks.System_Authorize.Authorize_Index)
                .FirstOrDefault();

            return View();
        }

        public ActionResult Index_Table(bool val)
        {
            try
            {
                return View(data.Services_GetNoPending(val));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Form(Guid? id)
        {
            ViewBag.PriorityList = data.SelectListItems_Priority();
            ViewBag.RefServiceList = data.SelectListItems_RefService();
            ViewBag.UserList = data.SelectListItems_User();

            Services services = new Services();
            if (id.HasValue)
            {
                services = db.Services.Find(id);
            }

            return View(services);
        }
    }
}