using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class TopicGalleries
    {
        public TopicGalleries()
        {
            TopicGallery_Id = Guid.NewGuid();
        }

        [Index]
        public Guid Topic_Id { get; set; }

        public string TopicGallery_Extension { get; set; }

        [Key]
        public Guid TopicGallery_Id { get; set; }

        public string TopicGallery_Name { get; set; }
        public string TopicGallery_Original { get; set; }

        [Display(Name = "No.")]
        public int TopicGallery_Seq { get; set; }

        public string TopicGallery_Thumbnail { get; set; }
        public virtual Topics Topics { get; set; }
    }
}
