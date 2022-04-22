using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class ServiceComments
    {
        [Key]
        public Guid ServiceComment_Id { get; set; }

        public Guid Service_Id { get; set; }

        [Display(Name = "Comment")]
        public string Comment_Content { get; set; }

        public Guid User_Id { get; set; }
        public virtual Users Users { get; set; }
        public DateTime Create { get; set; }
        public DateTime? Update { get; set; }

        public ServiceComments()
        {
            ServiceComment_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }
    }
}