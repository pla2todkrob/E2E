using E2E.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class ChatBotController : Controller
    {
        private readonly ClsContext db = new ClsContext();
        private readonly ClsManageMaster master = new ClsManageMaster();

        public ActionResult ChatHistory()
        {
            Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
            return View(db.ChatBotHistories.Where(w => w.IsDisplay && w.User_Id == userId));
        }

        // GET: ChatBot
        public ActionResult Index()
        {
            ViewBag.IsAdmin = master.IsAdmin();
            return View();
        }
    }
}
