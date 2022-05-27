using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class TopicGalleries
    {
        [Key]
        public Guid TopicGallery_Id { get; set; }

        public Guid Topic_Id { get; set; }
        public virtual Topics Topics { get; set; }
        public string TopicGallery_Original { get; set; }
        public string TopicGallery_Thumbnail { get; set; }
        public string TopicGallery_Name { get; set; }
        public string TopicGallery_Extension { get; set; }
        public int TopicGallery_Seq { get; set; }

        public TopicGalleries()
        {
            TopicGallery_Id = Guid.NewGuid();
        }
    }
}