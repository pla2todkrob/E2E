using E2E.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class OpenAIController : Controller
    {
        private readonly ClsOpenAI clsOpenAI = new ClsOpenAI();
        private readonly ClsContext db = new ClsContext();

        public ActionResult History()
        {
            List<ClsOpenAI> clsOpenAIs = new List<ClsOpenAI>();
            if (Guid.TryParse(HttpContext.User.Identity.Name, out Guid userId))
            {
                clsOpenAIs = db.ChatGPTs
                .Where(w => w.User_Id.Equals(userId))
                .OrderBy(o => o.Create)
                .Select(s => new ClsOpenAI()
                {
                    Answer = s.Answer,
                    AnswerDateTime = s.AnswerDateTime,
                    Question = s.Question,
                    QuestionDateTime = s.QuestionDateTime
                }).ToList();
            }
            return View(clsOpenAIs);
        }

        public ActionResult Index()
        {
            try
            {
                HttpCookie openAI_Key = Request.Cookies["OpenAI_Key"];

                if (openAI_Key == null)
                {
                    openAI_Key = new HttpCookie("OpenAI_Key")
                    {
                        Value = ConfigurationManager.AppSettings["OpenAI_Key"],
                        Expires = DateTime.Today.AddDays(1)
                    };
                    Response.Cookies.Add(openAI_Key);
                }

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Question()
        {
            return View(new ClsOpenAI());
        }

        [HttpPost]
        public ActionResult Question(ClsOpenAI model)
        {
            try
            {
                return Json(clsOpenAI.Response(model), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
