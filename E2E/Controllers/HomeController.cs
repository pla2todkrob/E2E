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

            DateTime Last7 = DateTime.Today.AddDays(-7);
            clsHome clsHome = new clsHome();
            clsHome.TopicAnnounce = db.Topics.Where(w => w.Topic_Pin).ToList();
            clsHome.TopicWeek = db.Topics.Where(w => w.Create >= Last7).Take(10).OrderByDescending(o=>o.Count_View).ToList();
            clsHome.EForms = db.EForms.Where(w => w.Create >= Last7).Take(10).OrderByDescending(o => o.Create).ToList();
            return View(clsHome);
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
                    var CHK = db.System_Manuals.Where(w => w.Manual_Type_Id == item1 & w.Language_Id == item2).OrderByDescending(o => o.Create).FirstOrDefault();

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