using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class WorkRootDocuments
    {
        public WorkRootDocuments()
        {
            WorkRootDocument_Id = Guid.NewGuid();
        }

        [Display(Name = "Document control"), Index]
        public Guid? Document_Id { get; set; }

        public virtual Master_Documents Master_Documents { get; set; }

        [Index]
        public Guid WorkRoot_Id { get; set; }

        [Key]
        public Guid WorkRootDocument_Id { get; set; }

        public virtual WorkRoots WorkRoots { get; set; }
    }
}
