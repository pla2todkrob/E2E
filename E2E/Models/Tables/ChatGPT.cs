using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class ChatGPT
    {
        public decimal ConversationCost { get; set; }

        [Index]
        public DateTime Create { get; set; } = DateTime.Now;

        [Key]
        public Guid GPTId { get; set; } = Guid.NewGuid();

        public bool IsEnd { get; set; }
        public decimal TokenUsage { get; set; }

        [Index]
        public Guid User_Id { get; set; }

        public virtual Users Users { get; set; }
    }
}
