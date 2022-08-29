using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class Master_Departments
    {
        public Master_Departments()
        {
            Department_Id = Guid.NewGuid();
            Active = true;
            Create = DateTime.Now;
        }

        public bool Active { get; set; }

        public DateTime Create { get; set; }

        [Key]
        public Guid Department_Id { get; set; }

        [Description("Department"), Display(Name = "Department")]
        public string Department_Name { get; set; }

        [Display(Name = "Division")]
        public Guid Division_Id { get; set; }

        public virtual Master_Divisions Master_Divisions { get; set; }
        public DateTime? Update { get; set; }
    }
}