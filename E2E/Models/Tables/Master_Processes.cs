using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class Master_Processes
    {
        public Master_Processes()
        {
            Process_Id = Guid.NewGuid();
            Active = true;
            Create = DateTime.Now;
        }

        public bool Active { get; set; }

        public DateTime Create { get; set; }

        public virtual Master_Sections Master_Sections { get; set; }

        [Key]
        public Guid Process_Id { get; set; }

        [Description("Process name"), Display(Name = "Process"), StringLength(100), Index]
        public string Process_Name { get; set; }

        [Display(Name = "Section"), Index]
        public Guid Section_Id { get; set; }

        public DateTime? Update { get; set; }
    }
}
