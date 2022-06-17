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

        public ActionResult Manual_Table()
        {
            List<System_Manuals> system_Manuals = new List<System_Manuals>();
            List<Guid> List_Language = db.System_Language.Select(s => s.Language_Id).ToList();
            List<Guid> List_ManualType = db.System_ManualType.Select(s => s.Manual_Type_Id).ToList();

                foreach (var item1 in List_ManualType)
                {
                    foreach (var item2 in List_Language)
                    {
                        system_Manuals.Add(db.System_Manuals.Where(w => w.Manual_Type_Id == item1 & w.Language_Id == item2).OrderByDescending(o => o.Create).FirstOrDefault());
                    }
                   
                }

            return View(system_Manuals);
        }
    }
}