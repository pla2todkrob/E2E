using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class ServiceDocuments
    {
        [Key]
        public Guid ServiceDocument_Id { get; set; }
        public Guid Document_Id { get; set; }
        public virtual Master_Documents Master_Documents { get; set; }
        public string ServiceDocument_Path { get; set; }
        public string ServiceDocument_Name { get; set; }
        public string ServiceDocument_Version { get; set; }
        public Guid Service_Id { get; set; }
        public Guid? User_Id { get; set; }
        public virtual Users Users { get; set; }
        public DateTime Create { get; set; }
        public ServiceDocuments()
        {
            ServiceDocument_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }
    }
}