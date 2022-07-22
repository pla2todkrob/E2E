using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class Master_Plants
    {
        [Key]
        public Guid Plant_Id { get; set; }

        [Description("Plant name"), Display(Name = "Plant"), Required]
        public string Plant_Name { get; set; }


        public bool Active { get; set; }
        public DateTime Create { get; set; }
        public DateTime? Update { get; set; }

        public Master_Plants()
        {
            Plant_Id = Guid.NewGuid();
            Active = true;
            Create = DateTime.Now;
        }
    }
}