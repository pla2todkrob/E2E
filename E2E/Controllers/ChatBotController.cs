using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class ChatBotController : Controller
    {
        private readonly ClsChatBot chatBot = new ClsChatBot();
        private readonly ClsApi clsApi = new ClsApi();
        private readonly ClsContext db = new ClsContext();
        private readonly ClsManageMaster master = new ClsManageMaster();

        private bool ClearSession()
        {
            try
            {
                HttpContext.Session.Remove(SessionGroup);
                HttpContext.Session.Remove(SessionQuestion);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string SessionGroup { get; set; } = "GroupId";
        public static string SessionQuestion { get; set; } = "QuestionId";

        public ActionResult ChatHistory()
        {
            Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
            List<ChatBotHistory> chatBotHistories = db.ChatBotHistories
                .Where(w =>
                w.IsDisplay &&
                w.User_Id == userId)
                .OrderBy(o => o.Create)
                .ToList();
            return View(chatBotHistories);
        }

        public ActionResult ClearHistory()
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                    db.ChatBotHistories.Where(w => w.User_Id == userId && w.IsDisplay).ToList().ForEach(f => f.IsDisplay = false);
                    if (db.SaveChanges() > 0)
                    {
                        scope.Complete();
                        swal.Icon = "success";
                        swal.Text = "ลบประวัติการแชทเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                        swal.DangerMode = false;
                        ClearSession();
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "ไม่พบข้อมูลการแชท";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.TargetSite.Name;
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Text = inner.Message;
                        inner = inner.InnerException;
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditAnswer(Guid id)
        {
            return View(db.ChatBotAnswers.Find(id));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditAnswer(ChatBotAnswer model)
        {
            ClsSwal swal = new ClsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        ChatBotAnswer chatBotAnswer = db.ChatBotAnswers.Find(model.ChatBotAnswer_Id);
                        chatBotAnswer.Answer = model.Answer;
                        if (db.SaveChanges() > 0)
                        {
                            scope.Complete();
                            swal.Icon = "success";
                            swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.Title = "Successful";
                            swal.DangerMode = false;
                        }
                        else
                        {
                            swal.Icon = "warning";
                            swal.Text = "กรุณาตรวจสอบข้อมูลอีกครั้ง";
                            swal.Title = "Warning";
                        }
                    }
                    catch (Exception ex)
                    {
                        swal.Title = ex.TargetSite.Name;
                        swal.Text = ex.Message;
                        Exception inner = ex.InnerException;
                        while (inner != null)
                        {
                            swal.Text = inner.Message;
                            inner = inner.InnerException;
                        }
                    }
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                   .Where(y => y.Count > 0)
                                   .ToList();
                swal.Icon = "warning";
                swal.Title = "Warning";
                foreach (var item in errors)
                {
                    foreach (var item2 in item)
                    {
                        if (string.IsNullOrEmpty(swal.Text))
                        {
                            swal.Text = item2.ErrorMessage;
                        }
                        else
                        {
                            swal.Text += "\n" + item2.ErrorMessage;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExpandLevel(Guid id)
        {
            try
            {
                var chatbotQA = new ClsChatbotQA
                {
                    ChatBotQuestions = db.ChatBotQuestions.Where(q => q.ChatBotQuestion_ParentId == id).ToList()
                };

                chatbotQA.ChatBotAnswers = chatbotQA.ChatBotQuestions.Count > 0
                                           ? chatBot.GetAnswers(chatbotQA.ChatBotQuestions)
                                           : db.ChatBotAnswers.Where(a => a.ChatBotQuestion_Id == id).ToList();

                return Json(chatbotQA, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                // Consider logging the exception before re-throwing
                throw;
            }
        }

        public ActionResult GetChatBot(Guid id)
        {
            try
            {
                ClsChatbotQuestion question = chatBot.GetQuestion(id);

                return View(question);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult GroupTable()
        {
            List<ChatBot> chatBots = db.ChatBots.OrderBy(o => o.Group).ToList();
            return View(chatBots);
        }

        public ActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportExcel()
        {
            ClsSwal swal = new ClsSwal();
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.MaxValue
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    var files = HttpContext.Request.Files;
                    foreach (string item in files.AllKeys)
                    {
                        HttpPostedFileBase file = files[item];

                        if (file.ContentLength > 0)
                        {
                            ClsServiceFile clsServiceFile = new ClsServiceFile()
                            {
                                FolderPath = "ChatBot"
                            };
                            FileResponse fileResponse = clsApi.UploadFile(clsServiceFile, file);
                            ChatBotUploadHistory chatBotUploadHistory = new ChatBotUploadHistory
                            {
                                ChatBotUploadHistoryFile = fileResponse.FileUrl,
                                ChatBotUploadHistoryFileName = Path.GetFileName(fileResponse.FileUrl),
                                User_Id = Guid.Parse(HttpContext.User.Identity.Name)
                            };
                            db.Entry(chatBotUploadHistory).State = System.Data.Entity.EntityState.Added;
                            if (db.SaveChanges() > 0)
                            {
                                if (chatBot.SaveChatBotLearn(fileResponse.FileUrl))
                                {
                                    scope.Complete();
                                    swal.Icon = "success";
                                    swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                                    swal.Title = "Successful";
                                    swal.DangerMode = false;
                                }
                                else
                                {
                                    swal.Icon = "warning";
                                    swal.Text = "กรุณาตรวจสอบข้อมูลอีกครั้ง";
                                    swal.Title = "Warning";
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.TargetSite.Name;
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Text = inner.Message;
                        inner = inner.InnerException;
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            ViewBag.IsAdmin = master.IsAdmin();
            return View();
        }

        public ActionResult Manage()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult QuestionList(Guid? id, bool getAnswer = false)
        {
            try
            {
                ReStart:
                List<ChatBotQuestion> questions = new List<ChatBotQuestion>();
                ChatBotHistory chatBotHistory = new ChatBotHistory();
                if (id.HasValue)
                {
                    if (HttpContext.Session[SessionGroup] == null)
                    {
                        HttpContext.Session[SessionGroup] = id.Value;
                        questions = db.ChatBotQuestions
                            .Where(w => w.ChatBot_Id == id.Value && !w.ChatBotQuestion_ParentId.HasValue)
                            .OrderBy(o => o.Question)
                            .ToList();

                        chatBotHistory = new ChatBotHistory()
                        {
                            HumanChat = db.ChatBots.Find(id).Group,
                            IsDisplay = true,
                            User_Id = Guid.Parse(HttpContext.User.Identity.Name)
                        };
                        db.ChatBotHistories.Add(chatBotHistory);
                    }
                    else
                    {
                        HttpContext.Session[SessionQuestion] = id.Value;
                        questions = db.ChatBotQuestions
                            .Where(w => w.ChatBotQuestion_ParentId == id)
                            .OrderBy(o => o.Question)
                            .ToList();

                        chatBotHistory = new ChatBotHistory()
                        {
                            HumanChat = db.ChatBotQuestions.Find(id).Question,
                            IsDisplay = true,
                            User_Id = Guid.Parse(HttpContext.User.Identity.Name)
                        };
                        db.ChatBotHistories.Add(chatBotHistory);

                        if (questions.Count == 0)
                        {
                            string answers = string.Empty;
                            foreach (var item in db.ChatBotAnswers.Where(w => w.ChatBotQuestion_Id == id.Value).Select(s => s.Answer).ToList())
                            {
                                answers += string.Concat(item, Environment.NewLine);
                            }

                            chatBotHistory = new ChatBotHistory()
                            {
                                SystemChat = answers.Trim(Environment.NewLine.ToCharArray()),
                                IsDisplay = true,
                                User_Id = Guid.Parse(HttpContext.User.Identity.Name)
                            };
                            db.ChatBotHistories.Add(chatBotHistory);
                            if (ClearSession())
                            {
                                id = null;
                                goto ReStart;
                            }
                        }
                    }
                }
                else
                {
                    if (HttpContext.Session[SessionQuestion] == null)
                    {
                        if (HttpContext.Session[SessionGroup] == null)
                        {
                            ViewBag.ChatbotGroupList = db.ChatBots
                                .OrderBy(o => o.Group)
                                .Select(s => new SelectListItem()
                                {
                                    Value = s.ChatBot_Id.ToString(),
                                    Text = s.Group
                                })
                                .ToList();
                        }
                        else
                        {
                            id = Guid.Parse(HttpContext.Session[SessionGroup].ToString());
                            questions = db.ChatBotQuestions
                                .Where(w => w.ChatBot_Id == id && !w.ChatBotQuestion_ParentId.HasValue)
                                .OrderBy(o => o.Question)
                                .ToList();
                            if (getAnswer)
                            {
                                string answers = string.Empty;
                                foreach (var item in chatBot.GetAnswers(questions))
                                {
                                    answers += string.Concat(item.ChatBotQuestion.Question,": ",item.Answer, Environment.NewLine);
                                }

                                chatBotHistory = new ChatBotHistory()
                                {
                                    SystemChat = answers.Trim(Environment.NewLine.ToCharArray()),
                                    IsDisplay = true,
                                    User_Id = Guid.Parse(HttpContext.User.Identity.Name)
                                };
                                db.ChatBotHistories.Add(chatBotHistory);

                                if (ClearSession())
                                {
                                    id = null;
                                    goto ReStart;
                                }
                            }
                        }
                    }
                    else
                    {
                        id = Guid.Parse(HttpContext.Session[SessionQuestion].ToString());
                        questions = db.ChatBotQuestions
                            .Where(w => w.ChatBotQuestion_ParentId == id)
                            .OrderBy(o => o.Question)
                            .ToList();

                        if (questions.Count == 0)
                        {
                            string answers = string.Empty;
                            foreach (var item in db.ChatBotAnswers.Where(w => w.ChatBotQuestion_Id == id.Value).Select(s => s.Answer).ToList())
                            {
                                answers += string.Concat(item, Environment.NewLine);
                            }

                            chatBotHistory = new ChatBotHistory()
                            {
                                SystemChat = answers.Trim(Environment.NewLine.ToCharArray()),
                                IsDisplay = true,
                                User_Id = Guid.Parse(HttpContext.User.Identity.Name)
                            };
                            db.ChatBotHistories.Add(chatBotHistory);

                            if (ClearSession())
                            {
                                id = null;
                                goto ReStart;
                            }
                        }
                        else
                        {
                            if (getAnswer)
                            {
                                string answers = string.Empty;
                                foreach (var item in chatBot.GetAnswers(questions))
                                {
                                    answers += string.Concat(item.ChatBotQuestion.Question, ": ", item.Answer, Environment.NewLine);
                                }

                                chatBotHistory = new ChatBotHistory()
                                {
                                    SystemChat = answers.Trim(Environment.NewLine.ToCharArray()),
                                    IsDisplay = true,
                                    User_Id = Guid.Parse(HttpContext.User.Identity.Name)
                                };
                                db.ChatBotHistories.Add(chatBotHistory);

                                if (ClearSession())
                                {
                                    id = null;
                                    goto ReStart;
                                }
                            }
                        }
                    }
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    if (db.SaveChanges() > 0)
                    {
                        scope.Complete();
                    }
                }

                return View(questions);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult UploadHistory()
        {
            return PartialView("_UploadHistory", db.ChatBotUploadHistories.OrderByDescending(o => o.Create).ToList());
        }
    }
}
