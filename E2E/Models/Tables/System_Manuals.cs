using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class System_Manuals
    {
        [Key]
        public Guid Manual_Id { get; set; }
        public string Manual_Path { get; set; }
        [Display(Name = "Name")]
        public string Manual_Name { get; set; }
        public string Manual_Extension { get; set; }
        public Guid Manual_Type_Id { get; set; }
        public virtual System_ManualType System_ManualType { get; set; }
        public Guid Language_Id { get; set; }
        public virtual System_Language System_Language { get; set; }
        public virtual Users Users { get; set; }
        public Guid User_Id { get; set; }
        public DateTime Create { get; set; }

        public System_Manuals()
        {
            Manual_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

    }
}