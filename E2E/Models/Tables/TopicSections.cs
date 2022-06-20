using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class TopicSections
    {
        [Key]
        public Guid TopicSection_Id { get; set; }
        public Guid Topic_Id { get; set; }
        public virtual Topics Topics { get; set; }
        [Required]
        public string TopicSection_Title { get; set; }
        [Required]
        public string TopicSection_Description { get; set; }
        public string TopicSection_Link { get; set; }
        public string TopicSection_Path { get; set; }
        public string TopicSection_Name { get; set; }
        public string TopicSection_Extension { get; set; }
        public string TopicSection_ContentType { get; set; }
        public DateTime Create { get; set; }
        public DateTime? Update { get; set; }

        public TopicSections()
        {
            TopicSection_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }
    }
}