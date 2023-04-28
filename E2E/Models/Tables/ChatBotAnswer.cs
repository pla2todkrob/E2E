using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class ChatBotAnswer
    {
        public ChatBotAnswer()
        {
            ChatBotAnswer_Id = Guid.NewGuid();
        }
        public string Answer { get; set; }

        [Key]
        public Guid ChatBotAnswer_Id { get; set; }

        public virtual ChatBotQuestion ChatBotQuestion { get; set; }

        [Index]
        public Guid ChatBotQuestion_Id { get; set; }
    }
}
