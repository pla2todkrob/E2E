using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class Master_Plants
    {
        public Master_Plants()
        {
            Plant_Id = Guid.NewGuid();
            Active = true;
            Create = DateTime.Now;
        }

        public bool Active { get; set; }

        public DateTime Create { get; set; }

        [Key]
        public Guid Plant_Id { get; set; }

        [Description("Plant name"), Display(Name = "Plant"), Required]
        public string Plant_Name { get; set; }

        public DateTime? Update { get; set; }
    }
}
