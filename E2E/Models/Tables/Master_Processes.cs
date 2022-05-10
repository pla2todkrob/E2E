using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class Master_Processes
    {
        [Key]
        public Guid Process_Id { get; set; }

        [Description("Process name"), Display(Name = "Process")]
        public string Process_Name { get; set; }

        [Description("Autorun number")]
        public int Code { get; set; }
        public Guid? Section_Id { get; set; }
        public virtual Master_Sections Master_Sections { get; set; }
        public bool Active { get; set; }
        public DateTime Create { get; set; }
        public DateTime? Update { get; set; }

        public Master_Processes()
        {
            Process_Id = Guid.NewGuid();
            Active = true;
            Create = DateTime.Now;
        }
    }
}