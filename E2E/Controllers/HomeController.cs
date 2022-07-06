using E2E.Models;
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
    public class HomeController : Controller
    {
        private clsContext db = new clsContext();

        public ActionResult Index()
        {
            clsHome clsHome = new clsHome();
            clsHome.Topics = db.Topics.Where(w => w.Topic_Pin).OrderBy(o => o.Create).ToList();
            clsHome.Topics.AddRange(db.Topics.Where(w => !w.Topic_Pin).Take(10).OrderByDescending(o=>o.Create).ToList());
            clsHome.EForms = db.EForms.Take(10).OrderByDescending(o => o.Create).ToList();
            return View(clsHome);
        }

        public ActionResult Manual_Table()
        {
            List<Manuals> system_Manuals = new List<Manuals>();
            List<Guid> List_Language = db.System_Language.Select(s => s.Language_Id).ToList();
            List<Guid> List_ManualType = db.System_ManualType.Select(s => s.Manual_Type_Id).ToList();

            foreach (var item1 in List_ManualType)
            {
                foreach (var item2 in List_Language)
                {
                    var CHK = db.Manuals.Where(w => w.Manual_Type_Id == item1 & w.Language_Id == item2).OrderByDescending(o => o.Create).FirstOrDefault();

                    if (CHK != null)
                    {
                        system_Manuals.Add(CHK);
                    }
                }

            }

            return View(system_Manuals);
        }
    }
}