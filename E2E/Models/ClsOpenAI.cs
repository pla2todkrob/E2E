using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

namespace E2E.Models
{
    public class ClsOpenAI
    {
        public string Answer { get; set; }
        public DateTime AnswerDateTime { get; set; }
        public string Question { get; set; }
        public DateTime QuestionDateTime { get; set; }

        public ClsOpenAI Response(ClsOpenAI model)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    using (ClsContext db = new ClsContext())
                    {
                        ChatGPT chatGPT = new ChatGPT()
                        {
                            Answer = model.Answer.Trim(),
                            AnswerDateTime = model.AnswerDateTime,
                            Question = model.Question.Trim(),
                            QuestionDateTime = model.QuestionDateTime,
                            User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name)
                        };
                        db.Entry(chatGPT).State = System.Data.Entity.EntityState.Added;
                        if (db.SaveChanges() > 0)
                        {
                            scope.Complete();
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return model;
        }
    }
}
