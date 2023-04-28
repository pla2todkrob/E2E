﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class ChatBotQuestion
    {
        public virtual ChatBot ChatBot { get; set; }

        [Index]
        public Guid ChatBot_Id { get; set; }

        [Key]
        public Guid ChatBotQuestion_Id { get; set; }

        public Guid? ChatBotQuestion_ParentId { get; set; }

        [Index, StringLength(100)]
        public string Question { get; set; }
    }
}