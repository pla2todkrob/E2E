using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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

        [Description("Autorun number")]
        public int Code { get; set; }

        public DateTime Create { get; set; }

        [Key]
        public Guid Division_Id { get; set; }

        [Description("Division"), Display(Name = "Division"), Required]
        public string Division_Name { get; set; }

        public virtual Master_Plants Master_Plants { get; set; }
        [Required,Display(Name ="Plant")]
        public Guid? Plant_Id { get; set; }
        public DateTime? Update { get; set; }
    }
}
