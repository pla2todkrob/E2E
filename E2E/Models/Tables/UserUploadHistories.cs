using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class UserUploadHistories
    {
        [Key]
        public Guid UserUploadHistoryId { get; set; }
        public Guid User_Id { get; set; }
        public virtual Users Users { get; set; }
        public string UserUploadHistoryFile { get; set; }
        [Display(Name ="File name")]
        public string UserUploadHistoryFileName { get; set; }
        public DateTime Create { get; set; }
        public UserUploadHistories()
        {
            UserUploadHistoryId = Guid.NewGuid();
            Create = DateTime.Now;
        }
    }
}