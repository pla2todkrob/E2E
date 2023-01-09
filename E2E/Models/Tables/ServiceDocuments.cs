using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class ServiceDocuments
    {
        public ServiceDocuments()
        {
            ServiceDocument_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        public DateTime Create { get; set; }

        [Index]
        public Guid? Document_Id { get; set; }

        public virtual Master_Documents Master_Documents { get; set; }

        [Index]
        public Guid Service_Id { get; set; }

        [Key]
        public Guid ServiceDocument_Id { get; set; }

        [Display(Name = "Document file")]
        public string ServiceDocument_Name { get; set; }

        public string ServiceDocument_Path { get; set; }

        [Display(Name = "Remark")]
        public string ServiceDocument_Remark { get; set; }

        public virtual Services Services { get; set; }
        public DateTime? Update { get; set; }

        [Index]
        public Guid? User_Id { get; set; }

        //public virtual Users Users { get; set; }
    }
}
