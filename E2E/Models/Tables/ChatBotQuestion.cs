using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class ChatBotQuestion
    {
        public ChatBotQuestion()
        {
            ChatBotQuestion_Id = Guid.NewGuid();
        }

        public virtual ChatBot ChatBot { get; set; }

        [Index]
        public Guid ChatBot_Id { get; set; }

        [Key]
        public Guid ChatBotQuestion_Id { get; set; }

        [Index, Display(Name = "Level")]
        public int ChatBotQuestion_Level { get; set; }

        public Guid? ChatBotQuestion_ParentId { get; set; }

        [Index, StringLength(100), Required]
        public string Question { get; set; }
    }
}
