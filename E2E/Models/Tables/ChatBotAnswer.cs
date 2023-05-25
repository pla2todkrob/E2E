using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class ChatBotAnswer
    {
        public ChatBotAnswer()
        {
            ChatBotAnswer_Id = Guid.NewGuid();
        }

        [Required]
        public string Answer { get; set; }

        [Key]
        public Guid ChatBotAnswer_Id { get; set; }

        public virtual ChatBotQuestion ChatBotQuestion { get; set; }

        [Index]
        public Guid ChatBotQuestion_Id { get; set; }
    }
}
