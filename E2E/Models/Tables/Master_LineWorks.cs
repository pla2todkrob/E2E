using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class Master_LineWorks
    {
        public Master_LineWorks()
        {
            LineWork_Id = Guid.NewGuid();
            Active = true;
            Create = DateTime.Now;
        }

        public bool Active { get; set; }

        [Description("Autorun number")]
        public int Code { get; set; }

        public DateTime Create { get; set; }

        [Key]
        public Guid LineWork_Id { get; set; }

        [Description("Line of work"), Display(Name = "Line of work"), Required]
        public string LineWork_Name { get; set; }

        public DateTime? Update { get; set; }
    }
}
