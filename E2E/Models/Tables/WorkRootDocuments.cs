using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class WorkRootDocuments
    {
        public WorkRootDocuments()
        {
            WorkRootDocument_Id = Guid.NewGuid();
        }

        [Display(Name = "Document control")]
        public Guid? Document_Id { get; set; }

        //public virtual Master_Documents Master_Documents { get; set; }

        public Guid WorkRoot_Id { get; set; }

        [Key]
        public Guid WorkRootDocument_Id { get; set; }

        public virtual WorkRoots WorkRoots { get; set; }
    }
}