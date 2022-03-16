using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class Master_Sections
    {
        [Key]
        public Guid Section_Id { get; set; }

        [Description("Section name"), Display(Name = "Section"), Required]
        public string Section_Name { get; set; }

        [Description("Autorun number")]
        public int Code { get; set; }
        public Guid? Department_Id { get; set; }
        public virtual Master_Departments Master_Departments { get; set; }
        public bool Active { get; set; }
        public DateTime Create { get; set; }
        public DateTime? Update { get; set; }

        public Master_Sections()
        {
            Section_Id = Guid.NewGuid();
            Active = true;
            Create = DateTime.Now;
        }
    }
}