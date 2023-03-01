using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
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
        private ClsOpenAI clsOpenAI = new ClsOpenAI();
        private ClsContext db = new ClsContext();
        private ClsManageMaster master = new ClsManageMaster();
        private SearchFilter  searchFilter = new SearchFilter();

        public ActionResult History()
        {
            List<ClsOpenAI> clsOpenAIs = new List<ClsOpenAI>();
            if (Guid.TryParse(HttpContext.User.Identity.Name, out Guid userId))
            {
                clsOpenAIs = db.ChatGPTs
                .Where(w => w.User_Id.Equals(userId) && w.Display == false)
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
            var userID = Guid.Parse(System.Web.HttpContext.Current.User.Identity.Name);
            ViewBag.History = db.ChatGPTs.Where(w => w.User_Id == userID  && w.Display == false).Count();

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

        public ActionResult Usage(SearchFilter filter)
        {
            return View(filter);
        }

        public ActionResult Usage_Table(string filter)
        {
            SearchFilter _Filter = searchFilter.DeserializeFilter(filter);


            Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
            //1=Acknowledge 2=Approved 3=Requestor
            var AuthorizeIndex = db.Users
            .Where(w => w.User_Id == userId)
            .Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id)
            .FirstOrDefault();

            //1=Amind 2=User
            var RoleIndex = db.Users
            .Where(w => w.User_Id == userId)
            .Select(s => s.Role_Id)
            .FirstOrDefault();

            _Filter.Date_To = _Filter.Date_To.AddDays(1);

            IQueryable<ChatGPT> chatGPTs = db.ChatGPTs.Where(w => w.Create >= _Filter.Date_From && w.Create <= _Filter.Date_To);


            List<Guid> userIds = new List<Guid>();
            if (RoleIndex == 2)
            {
                if (AuthorizeIndex == 3)
                {
                    userIds = db.ChatGPTs.Where(w=>w.User_Id == userId).Select(s => s.User_Id).ToList();
                }

                else
                {
                    var DeptId = db.Users.Where(w => w.User_Id.Equals(userId)).Select(s => s.Master_Processes.Master_Sections.Department_Id).FirstOrDefault();
                    userIds = db.ChatGPTs.Where(w => w.Users.Master_Processes.Master_Sections.Department_Id.Equals(DeptId)).Select(s => s.User_Id).ToList();
                }
            }
            else
            {
                userIds = db.ChatGPTs.Select(s => s.User_Id).ToList();
            }

            ClsGPT_Sum clsGPT_Sum = new ClsGPT_Sum();
            foreach (var item in userIds.Distinct())
            {
                List<ChatGPT> ChatGPTs = chatGPTs.Where(w => w.User_Id == item).ToList();

                if (ChatGPTs.Count > 0)
                {
                    ClsGPT clsGPT = new ClsGPT();
                    clsGPT.Amount = ChatGPTs.Count();
                    clsGPT.Tokens = ChatGPTs.Sum(s => s.Tokens);
                    clsGPT.UserID = item;
                    clsGPT.UserName = master.Users_GetInfomation(item);

                    clsGPT_Sum.ClsGPTs.Add(clsGPT);
                    clsGPT_Sum.TotalAmount += clsGPT.Amount;
                    clsGPT_Sum.TotalToken += clsGPT.Tokens;
                }

                

            }

            return View(clsGPT_Sum);
        }

        public void ClearTxt()
        {
            Guid UserID = Guid.Parse(System.Web.HttpContext.Current.User.Identity.Name);
            db.ChatGPTs
                .Where(w => w.User_Id == UserID && w.Display == false)
                .ToList()
                .ForEach(f => f.Display = true);

            db.SaveChanges();
        }

        public ActionResult Usage_Detail(Guid id,string filter)
        {
            SearchFilter _Filter = searchFilter.DeserializeFilter(filter);
            var Date_To = _Filter.Date_To.AddDays(1);

           var chatGPTs = db.ChatGPTs.Where(w=>w.User_Id == id &&  w.Create >= _Filter.Date_From && w.Create <= Date_To).ToList();

            return View(chatGPTs);
        }
    }
}
