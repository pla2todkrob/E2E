﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class ServiceComments
    {
        public ServiceComments()
        {
            ServiceComment_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        [Display(Name = "Comment")]
        public string Comment_Content { get; set; }

        public DateTime Create { get; set; }

        [Index]
        public Guid Service_Id { get; set; }

        [Key]
        public Guid ServiceComment_Id { get; set; }

        public virtual Services Services { get; set; }

        [Index]
        public Guid? User_Id { get; set; }
    }
}
