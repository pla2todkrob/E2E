using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class Topics
    {
        [Key]
        public Guid Topic_Id { get; set; }
        [Display(Name = "Title")]
        public string Topic_Title { get; set; }
        [Display(Name = "Content")]
        public string Topic_Content { get; set; }
        [Display(Name = "Pin")]
        public bool Topic_Pin { get; set; }
        [Display(Name = "Pin EndDate")]
        public DateTime? Topic_Pin_EndDate { get; set; }
        public int Count_View { get; set; }
        public int Count_Comment { get; set; }
        public DateTime Create { get; set; }
        public DateTime? Update { get; set; }
        public Guid User_Id { get; set; }
        public virtual Users Users { get; set; }
        public int Topic_FileCount { get; set; }
        public int Topic_GalleryCount { get; set; }

        public Topics()
        {
            Topic_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }
    }
}