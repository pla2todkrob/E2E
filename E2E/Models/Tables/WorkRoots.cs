using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class WorkRoots
    {
        public WorkRoots()
        {
            WorkRoot_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        public DateTime Create { get; set; }

        public virtual Master_Sections Master_Sections { get; set; }

        [Display(Name = "Section")]
        public Guid Section_Id { get; set; }

        public DateTime? Update { get; set; }
        public Guid? User_Id { get; set; }
        public virtual Users Users { get; set; }

        [Key]
        public Guid WorkRoot_Id { get; set; }

        [Display(Name = "Work root")]
        public string WorkRoot_Name { get; set; }
    }
}