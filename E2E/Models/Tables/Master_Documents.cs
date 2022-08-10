using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class Master_Documents
    {
        [Key]
        public Guid Document_Id { get; set; }
        public Guid User_Id { get; set; }
        public virtual Users Users { get; set; }
        public string Document_Name { get; set; }
        public DateTime Create { get; set; }
        public DateTime? Update { get; set; }
        public bool Active { get; set; }
        public Master_Documents()
        {
            Document_Id = Guid.NewGuid();
            Create = DateTime.Now;
            Active = true;
        }
    }
}