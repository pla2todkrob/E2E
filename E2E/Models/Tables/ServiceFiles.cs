﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class ServiceFiles
    {
        public ServiceFiles()
        {
            ServiceFile_Id = Guid.NewGuid();
        }

        [Index]
        public Guid Service_Id { get; set; }

        [Display(Name = "File extension")]
        public string ServiceFile_Extension { get; set; }

        [Key]
        public Guid ServiceFile_Id { get; set; }

        [Display(Name = "File name")]
        public string ServiceFile_Name { get; set; }

        [Display(Name = "Path")]
        public string ServiceFile_Path { get; set; }

        public int ServiceFile_Seq { get; set; }
    }
}
