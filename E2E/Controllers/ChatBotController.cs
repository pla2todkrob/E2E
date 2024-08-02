using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class ChatBotController : BaseController
    {
        private readonly ClsChatBot chatBot = new ClsChatBot();
        private readonly ClsApi clsApi = new ClsApi();
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
                        swal.Text = ex.GetBaseException().Message;
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
                            .OrderBy(o => o.Answer)
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
        public async Task<ActionResult> ImportExcel()
        {
            ClsSwal swal = new ClsSwal();
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.MaxValue
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options, TransactionScopeAsyncFlowOption.Enabled))
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
                            FileResponse fileResponse = await clsApi.UploadFile(clsServiceFile, file);
                            ChatBotUploadHistory chatBotUploadHistory = new ChatBotUploadHistory
                            {
                                ChatBotUploadHistoryFile = fileResponse.FileUrl,
                                ChatBotUploadHistoryFileName = Path.GetFileName(fileResponse.FileUrl),
                                User_Id = Guid.Parse(HttpContext.User.Identity.Name)
                            };
                            db.Entry(chatBotUploadHistory).State = EntityState.Added;
                            if (db.SaveChanges() > 0)
                            {
                                if (await chatBot.SaveChatBotLearn(fileResponse.FileUrl))
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
                    swal.Text = ex.GetBaseException().Message;
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
                                           .OrderBy(o => o.Answer)
                                           .ToListAsync();

                return Json(chatbotQA, JsonRequestBehavior.AllowGet);
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

        public async Task<JsonResult> OpenAI_ClearConversation()
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    ChatGPT chatGPT = await db.ChatGPTs.Where(w => w.User_Id == loginId && !w.IsEnd).FirstOrDefaultAsync();
                    chatGPT.IsEnd = true;
                    if (await db.SaveChangesAsync() > 0)
                    {
                        scope.Complete();
                        swal.Icon = "success";
                        swal.Text = "ล้างประวัติการแชทเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                        swal.DangerMode = false;
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "ไม่พบประวัติการแชท";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.TargetSite.Name;
                    swal.Text = ex.GetBaseException().Message;
                }
            }
            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> OpenAI_History()
        {
            List<ChatGPTHistory> chatGPTHistories = new List<ChatGPTHistory>();
            ChatGPT chatGPT = await db.ChatGPTs.Where(w => w.User_Id == loginId && !w.IsEnd).FirstOrDefaultAsync();
            if (chatGPT != null)
            {
                chatGPTHistories = await db.ChatGPTHistories.Where(w => w.GPTId == chatGPT.GPTId).OrderBy(o => o.Create).ToListAsync();
            }
            return View(chatGPTHistories);
        }

        public async Task<ActionResult> OpenAI_Request()
        {
            try
            {
                DateTime fromDate = DateTime.Today.AddDays(-(DateTime.Today.Day - 1));

                var chatGptQuery = db.ChatGPTs.Where(w => w.User_Id == loginId);
                var conversationTokenUsage = await chatGptQuery.Where(w => !w.IsEnd).Select(s => s.TokenUsage).FirstOrDefaultAsync();
                var monthlyYourTokenUsage = await chatGptQuery
    .Where(w => w.Create >= fromDate)
    .Select(s => (decimal?)s.TokenUsage) // Use nullable decimal here
    .SumAsync();

                var monthlyAllTokenUsage = await db.ChatGPTs
                    .Where(w => w.Create >= fromDate)
                    .Select(s => (decimal?)s.TokenUsage) // Use nullable decimal here
                    .SumAsync();

                ClsGptToken clsGptToken = new ClsGptToken()
                {
                    Conversation = conversationTokenUsage,
                    MonthlyYour = monthlyYourTokenUsage.GetValueOrDefault(),
                    MonthlyAll = monthlyAllTokenUsage.GetValueOrDefault()
                };

                return View(clsGptToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<JsonResult> OpenAI_Request(string question)
        {
            ClsAjax clsAjax = new ClsAjax();
            if (string.IsNullOrEmpty(question))
            {
                clsAjax.Error = true;
                clsAjax.Message = "Question cannot be empty.";
                return Json(clsAjax, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var client = CreateRestClient();
                var chatGPT = await GetOrCreateChatGPTAsync();

                var conversation = await BuildConversationAsync(chatGPT.GPTId, question);

                var requestBody = new
                {
                    model = ConfigurationManager.AppSettings["GPTModel"],
                    messages = conversation
                };

                var response = await SendRequestAsync(client, requestBody);

                return await ProcessResponseAsync(response, chatGPT, conversation);
            }
            catch (Exception ex)
            {
                return HandleException(clsAjax, ex);
            }
        }

        private RestClient CreateRestClient()
        {
            var options = new RestClientOptions("https://api.openai.com/")
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            return client;
        }

        private async Task<ChatGPT> GetOrCreateChatGPTAsync()
        {
            var chatGPT = await db.ChatGPTs.FirstOrDefaultAsync(w => w.User_Id == loginId && !w.IsEnd);
            if (chatGPT == null)
            {
                chatGPT = new ChatGPT { User_Id = loginId };
                db.ChatGPTs.Add(chatGPT);
            }
            return chatGPT;
        }

        private async Task<List<dynamic>> BuildConversationAsync(Guid gptId, string question)
        {
            var conversation = new List<dynamic>();
            var chatGPTHistories = await db.ChatGPTHistories
                .Where(w => w.GPTId == gptId)
                .OrderBy(o => o.Create)
                .ToListAsync();

            foreach (var item in chatGPTHistories)
            {
                conversation.Add(new { role = item.Role, content = item.Content });
            }

            conversation.Add(new { role = "user", content = question });
            var chatGPTHistory = new ChatGPTHistory
            {
                Content = question,
                GPTId = gptId,
                Role = "user"
            };
            db.ChatGPTHistories.Add(chatGPTHistory);
            return conversation;
        }

        private async Task<RestResponse> SendRequestAsync(RestClient client, object requestBody)
        {
            var request = new RestRequest("v1/chat/completions", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {ConfigurationManager.AppSettings["GPTkey"]}");
            request.AddJsonBody(requestBody);
            return await client.ExecuteAsync(request);
        }

        private async Task<JsonResult> ProcessResponseAsync(RestResponse response, ChatGPT chatGPT, List<dynamic> conversation)
        {
            ClsAjax clsAjax = new ClsAjax();

            if (!response.IsSuccessful)
            {
                return HandleErrorResponse(response);
            }

            dynamic jsonResponse = JObject.Parse(response.Content);
            dynamic choices = jsonResponse.choices[0];
            if (choices.finish_reason == "incomplete")
            {
                throw new Exception("AI's response was too long and got cut off.\nPlease try asking a shorter question or break your question up into smaller parts.");
            }

            dynamic messages = choices.message;
            string answer = messages.content;
            string role = messages.role;
            dynamic usage = jsonResponse.usage;
            decimal tokenUsage = usage.total_tokens;

            var chatGPTHistory = new ChatGPTHistory
            {
                Content = answer,
                GPTId = chatGPT.GPTId,
                Role = role
            };
            db.ChatGPTHistories.Add(chatGPTHistory);

            chatGPT.TokenUsage = tokenUsage;
            chatGPT.ConversationCost = 0;

            if (await db.SaveChangesAsync() > 0)
            {
                clsAjax.Message = chatGPTHistory.Content;
            }

            return Json(clsAjax, JsonRequestBehavior.AllowGet);
        }

        private JsonResult HandleErrorResponse(RestResponse response)
        {
            ClsAjax clsAjax = new ClsAjax { Error = true };
            dynamic jsonResponse = JObject.Parse(response.Content);

            if (jsonResponse.error != null)
            {
                if (jsonResponse.error is string errorMessage)
                {
                    clsAjax.Message = errorMessage;
                }
                else if (jsonResponse.error is JObject errorObject && errorObject["message"] != null)
                {
                    clsAjax.Message = errorObject["message"].ToString();
                }
                else
                {
                    clsAjax.Message = "Unknown error occurred.";
                }
            }
            else
            {
                clsAjax.Message = "Unknown error occurred.";
            }

            return Json(clsAjax, JsonRequestBehavior.AllowGet);
        }

        private JsonResult HandleException(ClsAjax clsAjax, Exception ex)
        {
            clsAjax.Error = true;
            clsAjax.Message = ex.Message;
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                clsAjax.Message += $"\n{ex.Message}";
            }
            return Json(clsAjax, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadHistory()
        {
            return PartialView("_UploadHistory", db.ChatBotUploadHistories.OrderByDescending(o => o.Create).ToList());
        }
    }
}
