using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class Master_Sections
    {
        public Master_Sections()
        {
            Section_Id = Guid.NewGuid();
            Active = true;
            Create = DateTime.Now;
        }

        public bool Active { get; set; }

        public DateTime Create { get; set; }

        [Display(Name = "Department"), Index]
        public Guid Department_Id { get; set; }

        public virtual Master_Departments Master_Departments { get; set; }

        [Key]
        public Guid Section_Id { get; set; }

        [Description("Section name"), Display(Name = "Section"), StringLength(100), Index]
        public string Section_Name { get; set; }

        public DateTime? Update { get; set; }
    }
}
