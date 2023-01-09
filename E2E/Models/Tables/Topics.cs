using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class Topics
    {
        public Topics()
        {
            Topic_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        [Index]
        public Guid? Category_Id { get; set; }

        public int Count_Comment { get; set; }

        public int Count_View { get; set; }

        public DateTime Create { get; set; }

        [Display(Name = "Publicly available"), Index]
        public bool IsPublic { get; set; }

        public virtual Master_Categories Master_Categories { get; set; }

        [Display(Name = "Content")]
        public string Topic_Content { get; set; }

        public int Topic_FileCount { get; set; }

        public int Topic_GalleryCount { get; set; }

        [Key]
        public Guid Topic_Id { get; set; }

        [Display(Name = "Pin it")]
        public bool Topic_Pin { get; set; }

        [Display(Name = "Pinned to date")]
        public DateTime? Topic_Pin_EndDate { get; set; }

        [Display(Name = "Title")]
        public string Topic_Title { get; set; }

        public DateTime? Update { get; set; }

        [Index]
        public Guid User_Id { get; set; }

        public virtual Users Users { get; set; }
    }
}
