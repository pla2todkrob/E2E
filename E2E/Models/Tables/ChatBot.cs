﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class ChatBot
    {
        public ChatBot()
        {
            Create = DateTime.Now;
            ChatBot_Id = Guid.NewGuid();
        }

        [Key]
        public Guid ChatBot_Id { get; set; }

        [Index]
        public DateTime Create { get; set; }

        [Index(IsUnique = true), StringLength(100)]
        public string Group { get; set; }

        public DateTime? Update { get; set; }
    }
}
