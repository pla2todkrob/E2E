using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class TopicSections
    {
        public TopicSections()
        {
            TopicSection_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        public DateTime Create { get; set; }

        public Guid Topic_Id { get; set; }

        public virtual Topics Topics { get; set; }

        public string TopicSection_ContentType { get; set; }

        [Required, Display(Name = "Description")]
        public string TopicSection_Description { get; set; }

        public string TopicSection_Extension { get; set; }

        [Key]
        public Guid TopicSection_Id { get; set; }

        [Display(Name = "Attach link")]
        public string TopicSection_Link { get; set; }

        public string TopicSection_Name { get; set; }

        public string TopicSection_Path { get; set; }

        [Required, Display(Name = "Title")]
        public string TopicSection_Title { get; set; }

        public DateTime? Update { get; set; }
    }
}
