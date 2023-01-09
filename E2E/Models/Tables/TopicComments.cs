using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class TopicComments
    {
        public TopicComments()
        {
            TopicComment_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        [Display(Name = "Content")]
        public string Comment_Content { get; set; }

        public DateTime Create { get; set; }
        public Guid? Ref_TopicComment_Id { get; set; }

        [Index]
        public Guid Topic_Id { get; set; }

        [Key]
        public Guid TopicComment_Id { get; set; }

        public virtual Topics Topics { get; set; }
        public DateTime? Update { get; set; }

        [Index]
        public Guid? User_Id { get; set; }

        public virtual Users Users { get; set; }
    }
}
