using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class Master_Nationalities
    {
        [Key]
        public Guid Nationality_Id { get; set; }

        [Description("Full name of Nationality"), Display(Name = "Nationality"), Required]
        public string Nationality_Name { get; set; }

        [Description("Shortname of Nationality"), Display(Name = "Shortname"), Required]
        public string Nationality_Shortname { get; set; }

        [Description("Autorun number")]
        public int Code { get; set; }

        public bool Active { get; set; }
        public DateTime Create { get; set; }
        public DateTime? Update { get; set; }

        public Master_Nationalities()
        {
            Nationality_Id = Guid.NewGuid();
            Active = true;
            Create = DateTime.Now;
        }
    }
}