﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class ChatGPTHistory
    {
        public virtual ChatGPT ChatGPT { get; set; }

        public string Content { get; set; }

        [Index]
        public DateTime Create { get; set; } = DateTime.Now;

        [Key]
        public Guid GPTHistoryId { get; set; } = Guid.NewGuid();

        [Index]
        public Guid GPTId { get; set; }

        public string Role { get; set; }
    }
}
