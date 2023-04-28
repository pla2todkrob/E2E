using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Net.Http;
using System.IO;
using OfficeOpenXml;

namespace E2E.Models.Views
{
    public class ClsChatBot
    {
        private readonly ClsContext db = new ClsContext();
        public ChatBot ChatBot { get; set; }
        public List<QAView> QAViews { get; set; }

        public ClsChatBot GetChatBotViewModel(Guid chatBotId)
        {
            ClsChatBot clsChatBot = new ClsChatBot()
            {
                ChatBot = db.ChatBots.Find(chatBotId),
                QAViews = db.ChatBotQuestions
                .Where(w => w.ChatBot_Id.Equals(chatBotId))
                .Join(db.ChatBotAnswers,
                q => q.ChatBotQuestion_Id,
                a => a.ChatBotQuestion_Id,
                (q, a) => new QAView()
                {
                    ChatBotQuestion = q,
                    ChatBotAnswer = a
                }).OrderBy(o => o.ChatBotQuestion.ChatBotQuestion_ParentId)
                .ToList()
            };

            return clsChatBot;
        }

        public bool SaveChatBotLearn(string fileUrl)
        {
            try
            {
                bool res = new bool();
                List<string> userCodeList = new List<string>();
                using (HttpClient client = new HttpClient())
                {
                    using (Stream stream = client.GetStreamAsync(fileUrl).Result)
                    {
                        using (ExcelPackage package = new ExcelPackage(stream))
                        {
                            foreach (var sheet in package.Workbook.Worksheets)
                            {
                                for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                                {
                                    var recNo = sheet.Cells[row, 1].Text;
                                    if (string.IsNullOrEmpty(recNo))
                                    {
                                        throw new Exception("The document has no information.");
                                    }
                                    var answer = sheet.Cells[row, 1].Text;
                                    var group = sheet.Cells[row, 2].Text;

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
                                    string beforeQuestion = string.Empty;
                                    for (int col = 3; col <= sheet.Dimension.End.Column; col++)
                                    {
                                        var question = sheet.Cells[row, col].Text;
                                        var nextQuestion = sheet.Cells[row, col + 1].Text;

                                        ChatBotQuestion chatBotQuestion = db.ChatBotQuestions.Where(w => w.ChatBot_Id.Equals(chatBot.ChatBot_Id) && w.Question.Equals(question) && w.ChatBotQuestion_ParentId.Equals(parentId)).FirstOrDefault();
                                        if (chatBotQuestion == null)
                                        {
                                            chatBotQuestion = new ChatBotQuestion()
                                            {
                                                ChatBotQuestion_ParentId = parentId,
                                                ChatBot_Id = chatBot.ChatBot_Id,
                                                Question = question
                                            };
                                            db.Entry(chatBotQuestion).State = EntityState.Added;
                                            db.SaveChanges();
                                        }

                                        parentId = chatBotQuestion.ChatBotQuestion_Id;

                                        if (string.IsNullOrEmpty(nextQuestion))
                                        {
                                            ChatBotAnswer chatBotAnswer = db.ChatBotAnswers.Where(w => w.ChatBotQuestion_Id.Equals(chatBotQuestion.ChatBotQuestion_Id)).FirstOrDefault();
                                            if (chatBotAnswer == null)
                                            {
                                                chatBotAnswer = new ChatBotAnswer()
                                                {
                                                    Answer = answer,
                                                    ChatBotQuestion_Id = chatBotQuestion.ChatBotQuestion_Id
                                                };
                                                db.Entry(chatBotAnswer).State = EntityState.Added;
                                            }
                                            else
                                            {
                                                chatBotAnswer.Answer = answer;
                                                db.Entry(chatBotAnswer).State = EntityState.Modified;
                                            }
                                            db.SaveChanges();
                                            break;
                                        }
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

    public class QAView
    {
        public ChatBotAnswer ChatBotAnswer { get; set; }
        public ChatBotQuestion ChatBotQuestion { get; set; }
    }
}
