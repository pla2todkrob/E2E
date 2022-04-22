using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class TopicComments
    {
        [Key]
        public Guid TopicComment_Id { get; set; }
        public Guid Topic_Id { get; set; }
        [Display(Name ="Content")]
        public string Comment_Content { get; set; }
        public Guid User_Id { get; set; }
        public virtual Users Users { get; set; }
        public DateTime Create { get; set; }
        public DateTime? Update { get; set; }
        public Guid? Ref_TopicComment_Id { get; set; }
        public TopicComments()
        {
            TopicComment_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }
    }
}