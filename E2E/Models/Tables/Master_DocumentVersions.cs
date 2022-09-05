using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class Master_DocumentVersions
    {
        public Master_DocumentVersions()
        {
            DocumentVersion_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        public DateTime Create { get; set; }

        public Guid Document_Id { get; set; }

        [Key]
        public Guid DocumentVersion_Id { get; set; }

        [Display(Name = "File name")]
        public string DocumentVersion_Name { get; set; }

        [Display(Name = "Version")]
        public int DocumentVersion_Number { get; set; }

        [Display(Name = "File path")]
        public string DocumentVersion_Path { get; set; }

        public virtual Master_Documents Master_Documents { get; set; }
        public Guid? User_Id { get; set; }
        public virtual Users Users { get; set; }
    }
}