using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class Manuals
    {
        public Manuals()
        {
            Manual_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        public DateTime Create { get; set; }

        public Guid Language_Id { get; set; }

        public string Manual_Extension { get; set; }

        [Key]
        public Guid Manual_Id { get; set; }

        [Display(Name = "Name")]
        public string Manual_Name { get; set; }

        public string Manual_Path { get; set; }
        public Guid Manual_Type_Id { get; set; }
        public virtual System_Language System_Language { get; set; }
        public virtual System_ManualType System_ManualType { get; set; }
        public Guid User_Id { get; set; }
        public virtual Users Users { get; set; }
        public int Ver { get; set; }
    }
}