using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class ChatGPT
    {
        public ChatGPT()
        {
            Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        public string Answer { get; set; }
        public DateTime AnswerDateTime { get; set; }

        [Index]
        public DateTime Create { get; set; }

        public bool Display { get; set; }

        [Key]
        public Guid Id { get; set; }

        public string Question { get; set; }
        public DateTime QuestionDateTime { get; set; }
        public int Tokens { get; set; }

        [Index]
        public Guid User_Id { get; set; }

        public virtual Users Users { get; set; }
    }
}
