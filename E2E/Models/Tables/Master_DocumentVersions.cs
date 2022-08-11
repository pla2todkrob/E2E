using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class Master_DocumentVersions
    {
        [Key]
        public Guid DocumentVersion_Id { get; set; }

        public Guid Document_Id { get; set; }
        public virtual Master_Documents Master_Documents { get; set; }
        public int DocumentVersion_Number { get; set; }
        public string DocumentVersion_Path { get; set; }
        public string DocumentVersion_Name { get; set; }
        public DateTime Create { get; set; }
        public Guid? User_Id { get; set; }
        public virtual Users Users { get; set; }

        public Master_DocumentVersions()
        {
            DocumentVersion_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }
    }
}