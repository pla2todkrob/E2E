using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class Master_Divisions
    {
        public Master_Divisions()
        {
            Division_Id = Guid.NewGuid();
            Active = true;
            Create = DateTime.Now;
        }

        public bool Active { get; set; }

        public DateTime Create { get; set; }

        [Key]
        public Guid Division_Id { get; set; }

        [Description("Division"), Display(Name = "Division"), StringLength(100), Index]
        public string Division_Name { get; set; }

        [Display(Name = "Plant")]
        public Guid? Plant_Id { get; set; }

        public DateTime? Update { get; set; }
    }
}
