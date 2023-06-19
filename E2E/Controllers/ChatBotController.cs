using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public string SessionQuestionName { get; set; } = "QuestionLv";

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

        public async Task<ActionResult> GetAnswer(Guid? id)
        {
            try
            {
                ClsChatbotQA chatbotQA = new ClsChatbotQA();

                if (id.HasValue)
                {
                    if (db.ChatBots.Any(a => a.ChatBot_Id == id.Value))
                    {
                        chatbotQA.Parents.Add(new KeyValuePair<string, KeyValuePair<int, Guid>>(db.ChatBots.Find(id).Group, new KeyValuePair<int, Guid>(0, id.Value)));
                    }
                    else
                    {
                        List<ChatBotQuestion> questions = new List<ChatBotQuestion>();

                        questions = await db.ChatBotQuestions
                                .Where(q => q.ChatBotQuestion_ParentId == id)
                                .OrderBy(o => o.Question)
                                .ToListAsync();

                        chatbotQA.ChatBotAnswers = questions.Count > 0
                            ? await chatBot.GetAnswersAsync(id.Value)
                            : await db.ChatBotAnswers.Where(a => a.ChatBotQuestion_Id == id)
                                .ToListAsync();

                        ChatBotQuestion currentQuestion = db.ChatBotQuestions.Find(id);
                        chatbotQA.Parents.Add(new KeyValuePair<string, KeyValuePair<int, Guid>>(currentQuestion.ChatBot.Group, new KeyValuePair<int, Guid>(0, currentQuestion.ChatBot_Id)));
                        while (currentQuestion != null)
                        {
                            chatbotQA.Parents.Add(new KeyValuePair<string, KeyValuePair<int, Guid>>(currentQuestion.Question, new KeyValuePair<int, Guid>(currentQuestion.ChatBotQuestion_Level, currentQuestion.ChatBotQuestion_Id)));
                            currentQuestion = db.ChatBotQuestions.Find(currentQuestion.ChatBotQuestion_ParentId);
                        }
                        chatbotQA.Parents = chatbotQA.Parents.OrderBy(o => o.Value.Key).ToList();
                    }
                }

                return View(chatbotQA);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult GetQuestionList(Guid? id)
        {
            try
            {
                ClsChatbotQA chatbotQA = new ClsChatbotQA();
                if (id.HasValue)
                {
                    if (db.ChatBots.Any(a => a.ChatBot_Id == id.Value))
                    {
                        chatbotQA.ChatBotQuestions = db.ChatBotQuestions
                    .Where(w =>
                    w.ChatBot_Id.Equals(id.Value) &&
                    w.ChatBotQuestion_Level.Equals(1))
                    .OrderBy(o => o.ChatBotQuestion_Level)
                    .ThenBy(t => t.Question)
                    .ToList();
                    }
                    else
                    {
                        chatbotQA.ChatBotQuestions = db.ChatBotQuestions
                            .Where(w => w.ChatBotQuestion_ParentId == id.Value)
                            .OrderBy(o => o.ChatBotQuestion_Level)
                    .ThenBy(t => t.Question)
                    .ToList();
                    }
                }
                else
                {
                    chatbotQA.ChatBots = db.ChatBots.OrderBy(o => o.Group).ToList();
                }

                return View(chatbotQA);
            }
            catch (Exception)
            {
                throw;
            }
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

        public ActionResult Manage_Datalist()
        {
            List<ChatBot> chatBots = db.ChatBots.OrderBy(o => o.Group).ToList();
            return View(chatBots);
        }

        public async Task<JsonResult> Manage_ExpandLevel(Guid id)
        {
            try
            {
                ClsChatbotQA chatbotQA = new ClsChatbotQA
                {
                    ChatBotQuestions = db.ChatBotQuestions.AsNoTracking().Where(q => q.ChatBotQuestion_ParentId == id).OrderBy(o => o.Question).ToList()
                };

                chatbotQA.ChatBotAnswers = chatbotQA.ChatBotQuestions.Count > 0
                                           ? await chatBot.GetAnswersAsync(id)
                                           : await db.ChatBotAnswers.Where(a => a.ChatBotQuestion_Id == id)
                                           .ToListAsync();

                return this.Json(chatbotQA, JsonRequestBehavior.AllowGet, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            }
            catch (Exception)
            {
                // Consider logging the exception before re-throwing
                throw;
            }
        }

        public async Task<ActionResult> Manage_RootQuestion(Guid id)
        {
            try
            {
                ClsChatbotQuestion question = await chatBot.GetQuestion(id);

                return View(question);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult OpenAI()
        {
            return View();
        }

        public ActionResult UploadHistory()
        {
            return PartialView("_UploadHistory", db.ChatBotUploadHistories.OrderByDescending(o => o.Create).ToList());
        }
    }
}
