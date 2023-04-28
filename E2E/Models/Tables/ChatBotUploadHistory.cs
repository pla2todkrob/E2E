using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class ChatBotUploadHistory
    {
        public ChatBotUploadHistory()
        {
            ChatBotUploadHistoryId = Guid.NewGuid();
            Create = DateTime.Now;
        }

        public string ChatBotUploadHistoryFile { get; set; }

        [Display(Name = "File name")]
        public string ChatBotUploadHistoryFileName { get; set; }

        [Key]
        public Guid ChatBotUploadHistoryId { get; set; }

        public DateTime Create { get; set; }

        [Index]
        public Guid User_Id { get; set; }

        public virtual Users Users { get; set; }
    }
}
