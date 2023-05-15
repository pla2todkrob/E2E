using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class BusinessCardFiles
    {
        public BusinessCardFiles()
        {
            BusinessCardFiles_Id = Guid.NewGuid();
        }
        public Guid BusinessCard_Id { get; set; }

        public string Extension { get; set; }

        [Key]
        public Guid BusinessCardFiles_Id { get; set; }

        public string FileName { get; set; }
        public bool Confirm { get; set; }
        public string FilePath { get; set; }
        public string Remark { get; set; }
        public DateTime Create { get; set; }
        public virtual BusinessCards BusinessCards { get; set; }
    }
}