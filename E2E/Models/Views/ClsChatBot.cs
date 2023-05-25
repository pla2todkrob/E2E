using E2E.Models.Tables;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace E2E.Models.Views
{
    public class ClsChatBot
    {
        private readonly ClsContext db = new ClsContext();
        public ChatBot ChatBot { get; set; }

        public List<ChatBotAnswer> GetAnswers(List<ChatBotQuestion> questions)
        {
            var answers = new List<ChatBotAnswer>();

            try
            {
                var nextQuestions = new List<ChatBotQuestion>();
                foreach (var question in questions)
                {
                    answers.AddRange(db.ChatBotAnswers.Where(a => a.ChatBotQuestion_Id == question.ChatBotQuestion_Id));
                    var childQuestions = db.ChatBotQuestions.Where(q => q.ChatBotQuestion_ParentId == question.ChatBotQuestion_Id).ToList();

                    if (childQuestions.Any())
                    {
                        nextQuestions.AddRange(childQuestions);
                    }
                }

                if (nextQuestions.Any())
                {
                    answers.AddRange(GetAnswers(nextQuestions));
                }

                return answers;
            }
            catch (Exception)
            {
                // Consider logging the exception before re-throwing
                throw;
            }
        }

        public ClsChatbotQuestion GetQuestion(Guid chatBotId)
        {
            try
            {
                ClsChatbotQuestion clsQuestion = new ClsChatbotQuestion()
                {
                    ChatBot = db.ChatBots.Find(chatBotId),
                    ChatBotQuestions = db.ChatBotQuestions
                    .Where(w =>
                    w.ChatBot_Id.Equals(chatBotId) &&
                    w.ChatBotQuestion_Level.Equals(1))
                    .OrderBy(o => o.ChatBotQuestion_Level)
                    .ThenBy(t => t.Question)
                    .ToList()
                };

                return clsQuestion;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SaveChatBotLearn(string fileUrl)
        {
            try
            {
                bool res = new bool();
                if (db.ChatBots.Count() > 0)
                {
                    db.ChatBots.RemoveRange(db.ChatBots.ToList());
                    db.SaveChanges();
                }

                using (HttpClient client = new HttpClient())
                {
                    using (Stream stream = client.GetStreamAsync(fileUrl).Result)
                    {
                        using (ExcelPackage package = new ExcelPackage(stream))
                        {
                            foreach (var sheet in package.Workbook.Worksheets)
                            {
                                var startData = sheet.Cells[2, 1].Text;
                                if (string.IsNullOrEmpty(startData))
                                {
                                    throw new Exception("The document has no information.");
                                }

                                for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                                {
                                    var answer = sheet.Cells[row, 1].Text;
                                    var group = sheet.Cells[row, 2].Text;
                                    if (string.IsNullOrEmpty(answer) || string.IsNullOrEmpty(group))
                                    {
                                        break;
                                    }
                                    ChatBot chatBot = db.ChatBots.Where(w => w.Group.Equals(group)).FirstOrDefault();
                                    if (chatBot == null)
                                    {
                                        chatBot = new ChatBot()
                                        {
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
                                    for (int col = 3; col <= sheet.Dimension.End.Column; col++)
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
                            res = true;
                        }
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class ClsChatbotQA
    {
        public List<ChatBotAnswer> ChatBotAnswers { get; set; } = new List<ChatBotAnswer>();
        public List<ChatBotQuestion> ChatBotQuestions { get; set; } = new List<ChatBotQuestion>();
    }

    public class ClsChatbotQuestion
    {
        public ChatBot ChatBot { get; set; }
        public List<ChatBotQuestion> ChatBotQuestions { get; set; } = new List<ChatBotQuestion>();
    }
}
