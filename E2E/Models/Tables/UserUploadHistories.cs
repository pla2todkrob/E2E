using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class UserUploadHistories
    {
        public UserUploadHistories()
        {
            UserUploadHistoryId = Guid.NewGuid();
            Create = DateTime.Now;
        }

        public DateTime Create { get; set; }

        [Index]
        public Guid User_Id { get; set; }

        public virtual Users Users { get; set; }

        public string UserUploadHistoryFile { get; set; }

        [Display(Name = "File name")]
        public string UserUploadHistoryFileName { get; set; }

        [Key]
        public Guid UserUploadHistoryId { get; set; }
    }
}
