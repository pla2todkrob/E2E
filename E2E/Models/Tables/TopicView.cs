using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class TopicView
    {
        public TopicView()
        {
            TopicView_Id = Guid.NewGuid();
            LastTime = DateTime.Now;
        }

        [Key]
        public Guid TopicView_Id { get; set; }
        public Guid? Topic_Id { get; set; }
        public Guid? User_Id { get; set; }
        public int Count { get; set; }
        public DateTime LastTime { get; set; }
    }
}