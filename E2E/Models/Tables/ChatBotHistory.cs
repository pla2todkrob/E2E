using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class ChatBotHistory
    {
        public ChatBotHistory()
        {
            Create = DateTime.Now;
            IsDisplay = true;
            ChatBotHistory_Id = Guid.NewGuid();
        }

        [Key]
        public Guid ChatBotHistory_Id { get; set; }

        [Index]
        public DateTime Create { get; set; }

        public string HumanChat { get; set; }
        public bool IsDisplay { get; set; }
        public string SystemChat { get; set; }

        [Index]
        public Guid User_Id { get; set; }
    }
}
