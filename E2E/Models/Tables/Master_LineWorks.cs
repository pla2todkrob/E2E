using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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

        [Display(Name = "Authorize")]
        public int Authorize_Id { get; set; }

        public DateTime Create { get; set; }

        [Key]
        public Guid LineWork_Id { get; set; }

        [Description("Line of work"), Display(Name = "Line of work"), Required]
        public string LineWork_Name { get; set; }

        public virtual System_Authorize System_Authorize { get; set; }
        public DateTime? Update { get; set; }
    }
}