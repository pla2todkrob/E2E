using E2E.Models.Tables;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace E2E.Models.Views
{
    public class ClsChatBot
    {
        private readonly ClsContext db = new ClsContext();

        private async Task<List<ChatBotQuestion>> GetChildQuestionsAsync(Guid parentQuestionId)
        {
            return await db.ChatBotQuestions
                .Where(q => q.ChatBotQuestion_ParentId == parentQuestionId)
                .OrderBy(o => o.Question)
                .ToListAsync();
        }

        public ChatBot ChatBot { get; set; }

        public string EscapeHtml(string html)
        {
            return html.Replace("<", "&lt;")
                       .Replace(">", "&gt;")
                       .Replace("\"", "&quot;")
                       .Replace("'", "&apos;")
                       .Replace("&", "&amp;");
        }

        public async Task<List<ChatBotAnswer>> GetAnswersAsync(Guid questionId)
        {
            var answers = new List<ChatBotAnswer>();

            try
            {
                var nextQuestions = new List<ChatBotQuestion>();

                answers.AddRange(await db.ChatBotAnswers.Where(w => w.ChatBotQuestion_Id == questionId).OrderBy(o => o.Answer).ToListAsync());

                // Retrieve child questions recursively
                nextQuestions.AddRange(await GetChildQuestionsAsync(questionId));

                // Retrieve answers for child questions recursively
                while (nextQuestions.Any())
                {
                    var currentQuestion = nextQuestions.FirstOrDefault();

                    // Add answers from the current question
                    answers.AddRange(await db.ChatBotAnswers.Where(w => w.ChatBotQuestion_Id == currentQuestion.ChatBotQuestion_Id).OrderBy(o => o.Answer).ToListAsync());
                    // Retrieve child questions for the current question
                    var childQuestions = await GetChildQuestionsAsync(currentQuestion.ChatBotQuestion_Id);

                    // Add child questions to the list of next questions
                    nextQuestions.AddRange(childQuestions);

                    nextQuestions.RemoveAt(0);
                    nextQuestions = nextQuestions.OrderBy(o => o.Question).ToList();
                }

                return answers;
            }
            catch (Exception)
            {
                // Consider logging the exception before re-throwing
                throw;
            }
        }

        public async Task<ClsChatbotQuestion> GetQuestion(Guid chatBotId)
        {
            try
            {
                ClsChatbotQuestion clsQuestion = new ClsChatbotQuestion()
                {
                    ChatBot = await db.ChatBots.FindAsync(chatBotId),
                    ChatBotQuestions = await db.ChatBotQuestions
                    .AsNoTracking()
                    .Where(w =>
                    w.ChatBot_Id.Equals(chatBotId) &&
                    w.ChatBotQuestion_Level.Equals(1))
                    .OrderBy(o => o.ChatBotQuestion_Level)
                    .ThenBy(t => t.Question)
                    .ToListAsync()
                };

                return clsQuestion;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> SaveChatBotLearn(string fileUrl)
        {
            try
            {
                bool res = false;
                using (WebClient webClient = new WebClient())
                {
                    byte[] fileData = webClient.DownloadData(fileUrl);
                    using (var memoryStream = new MemoryStream(fileData))
                    {
                        List<string> OwnerNames = new List<string>();

                        using (ExcelPackage package = new ExcelPackage(memoryStream))
                        {
                            bool isNoData = true;

                            foreach (var sheet in package.Workbook.Worksheets)
                            {
                                var startData = sheet.Cells[2, 1].Text;
                                if (string.IsNullOrEmpty(startData))
                                {
                                    continue;
                                }

                                for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                                {
                                    var owner = sheet.Cells[row, 2].Text;
                                    if (string.IsNullOrEmpty(owner))
                                    {
                                        continue;
                                    }
                                    if (!OwnerNames.Contains(owner))
                                    {
                                        OwnerNames.Add(owner);
                                    }
                                }
                            }

                            if (OwnerNames.Count > 0)
                            {
                                db.ChatBots.RemoveRange(db.ChatBots.Where(w => OwnerNames.Contains(w.Owner)).ToList());
                                db.SaveChanges();
                            }

                            foreach (var sheet in package.Workbook.Worksheets)
                            {
                                var startData = sheet.Cells[2, 1].Text;
                                if (string.IsNullOrEmpty(startData))
                                {
                                    continue;
                                }

                                isNoData = false;
                                for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                                {
                                    var answer = sheet.Cells[row, 1].Text;
                                    var owner = sheet.Cells[row, 2].Text;
                                    var group = sheet.Cells[row, 3].Text;
                                    if (string.IsNullOrEmpty(answer) || string.IsNullOrEmpty(owner) || string.IsNullOrEmpty(group))
                                    {
                                        continue;
                                    }
                                    ChatBot chatBot = db.ChatBots.Where(w => w.Owner.Equals(owner) && w.Group.Equals(group)).FirstOrDefault();
                                    if (chatBot == null)
                                    {
                                        chatBot = new ChatBot()
                                        {
                                            Owner = owner,
                                            Group = group
                                        };
                                        db.Entry(chatBot).State = EntityState.Added;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        chatBot.Update = DateTime.Now;
                                    }

                                    Guid? parentId = null;
                                    int questionLv = 1;
                                    for (int col = 4; col <= sheet.Dimension.End.Column; col++)
                                    {
                                        var question = sheet.Cells[row, col].Text;
                                        var nextQuestion = sheet.Cells[row, col + 1].Text;

                                        ChatBotQuestion chatBotQuestion = db.ChatBotQuestions
                                            .Where(w =>
                                            w.ChatBot_Id.Equals(chatBot.ChatBot_Id) &&
                                            w.Question.Equals(question) &&
                                            w.ChatBotQuestion_Level == questionLv &&
                                            w.ChatBotQuestion_ParentId == parentId)
                                            .FirstOrDefault();
                                        if (chatBotQuestion == null && !string.IsNullOrEmpty(question))
                                        {
                                            chatBotQuestion = new ChatBotQuestion()
                                            {
                                                ChatBotQuestion_ParentId = parentId,
                                                ChatBot_Id = chatBot.ChatBot_Id,
                                                Question = question,
                                                ChatBotQuestion_Level = questionLv
                                            };
                                            string jsonQuestion = JsonConvert.SerializeObject(chatBotQuestion, Formatting.Indented);
                                            db.Entry(chatBotQuestion).State = EntityState.Added;
                                            db.SaveChanges();
                                        }

                                        parentId = chatBotQuestion.ChatBotQuestion_Id;

                                        if (string.IsNullOrEmpty(nextQuestion))
                                        {
                                            ChatBotAnswer chatBotAnswer = new ChatBotAnswer()
                                            {
                                                Answer = answer,
                                                ChatBotQuestion_Id = chatBotQuestion.ChatBotQuestion_Id
                                            };
                                            db.Entry(chatBotAnswer).State = EntityState.Added;
                                            db.SaveChanges();
                                            break;
                                        }
                                        questionLv++;
                                    }
                                }
                            }
                            if (isNoData)
                            {
                                throw new Exception("The document has no information.");
                            }
                            res = true;
                        }
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                FileResponse response = await new ClsApi().DeleteFile(fileUrl);
                if (!response.IsSuccess)
                {
                    throw new Exception(response.ErrorMessage, ex);
                }
                throw;
            }
        }

        public string UnescapeHtml(string escapedHtml)
        {
            return escapedHtml.Replace("&lt;", "<")
                              .Replace("&gt;", ">")
                              .Replace("&quot;", "\"")
                              .Replace("&apos;", "'")
                              .Replace("&amp;", "&");
        }
    }

    public class ClsChatbotQA
    {
        public List<ChatBotAnswer> ChatBotAnswers { get; set; } = new List<ChatBotAnswer>();
        public List<ChatBotQuestion> ChatBotQuestions { get; set; } = new List<ChatBotQuestion>();
        public List<ChatBot> ChatBots { get; set; } = new List<ChatBot>();
        public List<KeyValuePair<string, KeyValuePair<int, Guid>>> Parents { get; set; } = new List<KeyValuePair<string, KeyValuePair<int, Guid>>>();
    }

    public class ClsChatbotQuestion
    {
        public ChatBot ChatBot { get; set; }
        public List<ChatBotQuestion> ChatBotQuestions { get; set; } = new List<ChatBotQuestion>();
    }
}
