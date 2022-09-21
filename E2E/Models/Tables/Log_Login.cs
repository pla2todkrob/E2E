﻿using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class Log_Login
    {
        public Log_Login()
        {
            Log_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        public DateTime Create { get; set; }

        [Key]
        public Guid Log_Id { get; set; }

        public Guid User_Id { get; set; }
    }
}
