using E2E.Models;
using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class EFormsController : Controller
    {
        private clsManageTopic data = new clsManageTopic();
        private clsContext db = new clsContext();
        private clsServiceFTP ftp = new clsServiceFTP();
        // GET: EForms
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EForms_Table(int res)
        {
            try
            {
                Guid id = Guid.Parse(HttpContext.User.Identity.Name);
                ViewBag.Usercode = db.Users
                    .Where(w => w.User_Id == id)
                    .Select(s => s.User_Code)
                    .FirstOrDefault();


                var query = db.EForms.Where(w => w.EForm_Start == DateTime.Today).OrderByDescending(o => new { o.Update, o.Create }).ToList();
            
                if (res == 1)
                {
                    query = query.Where(w => w.EForm_End >= DateTime.Today).ToList();
                    return View(query);
                }
                if (res == 2)
                {
                    query = db.EForms.Where(w => w.User_Id == id).OrderByDescending(o => new { o.Update, o.Create }).ToList();
                    return View(query);
                }
                if (res == 3)
                {
                    query = db.EForms.Where(w => w.EForm_End < DateTime.Today).OrderByDescending(o => new { o.Update, o.Create }).ToList();
                    return View(query);
                }
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult EForms_Create(Guid? id)
        {
            try
            {
                EForms eForms = new EForms();
                eForms.EForm_Start = DateTime.Now;

                bool isNew = true;
                if (id.HasValue)
                {
                    eForms = db.EForms.Find(id);
                    isNew = false;
                }
                ViewBag.isNew = isNew;

                return View(eForms);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}