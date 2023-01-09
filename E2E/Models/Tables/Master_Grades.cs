using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class Master_Grades
    {
        public Master_Grades()
        {
            Grade_Id = Guid.NewGuid();
            Active = true;
            Create = DateTime.Now;
        }

        public bool Active { get; set; }

        public DateTime Create { get; set; }

        [Key]
        public Guid Grade_Id { get; set; }

        [Description("Grade"), Display(Name = "Grade"), StringLength(100), Index]
        public string Grade_Name { get; set; }

        [Description("Position"), Display(Name = "Position")]
        public string Grade_Position { get; set; }

        [Required, Display(Name = "Line work"), Index]
        public Guid LineWork_Id { get; set; }

        public virtual Master_LineWorks Master_LineWorks { get; set; }
        public DateTime? Update { get; set; }
    }
}
