using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class BusinessCardFiles
    {
        public BusinessCardFiles()
        {
            BusinessCardFiles_Id = Guid.NewGuid();
        }

        public Guid BusinessCard_Id { get; set; }

        [Key]
        public Guid BusinessCardFiles_Id { get; set; }

        public virtual BusinessCards BusinessCards { get; set; }
        public bool Confirm { get; set; }
        public DateTime Create { get; set; }
        public string Extension { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Remark { get; set; }
    }
}
