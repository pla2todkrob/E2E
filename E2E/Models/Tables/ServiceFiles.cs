using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class ServiceFiles
    {
        [Key]
        public Guid ServiceFile_Id { get; set; }

        public Guid Service_Id { get; set; }

        [Display(Name = "Path")]
        public string ServiceFile_Path { get; set; }

        [Display(Name = "File name")]
        public string ServiceFile_Name { get; set; }

        [Display(Name = "File extension")]
        public string ServiceFile_Extension { get; set; }

        public ServiceFiles()
        {
            ServiceFile_Id = Guid.NewGuid();
        }
    }
}