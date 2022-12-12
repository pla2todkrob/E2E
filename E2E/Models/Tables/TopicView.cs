using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class TopicView
    {
        public TopicView()
        {
            TopicView_Id = Guid.NewGuid();
            LastTime = DateTime.Now;
        }

        public int Count { get; set; }

        public DateTime LastTime { get; set; }

        public Guid Topic_Id { get; set; }
        public virtual Topics Topics { get; set; }

        [Key]
        public Guid TopicView_Id { get; set; }

        public Guid? User_Id { get; set; }
    }
}
