using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class Master_Documents
    {
        public Master_Documents()
        {
            Document_Id = Guid.NewGuid();
            Create = DateTime.Now;
            Active = true;
        }

        public bool Active { get; set; }

        public DateTime Create { get; set; }

        [Key]
        public Guid Document_Id { get; set; }

        [Display(Name = "Document control")]
        public string Document_Name { get; set; }

        public bool Required { get; set; }
        public DateTime? Update { get; set; }

        [Index]
        public Guid User_Id { get; set; }

        public virtual Users Users { get; set; }
    }
}
