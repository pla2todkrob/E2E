﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required]
        public string Question { get; set; }
    }
}
