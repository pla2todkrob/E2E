using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class ServiceChangeDueDate
    {
        public ServiceChangeDueDate()
        {
            ChangeDueDate_Id = Guid.NewGuid();
            Create = DateTime.Now;
            DueDateStatus_Id = 1;
        }

        [Key]
        public Guid ChangeDueDate_Id { get; set; }

        [Index]
        public DateTime Create { get; set; }

        [Display(Name = "From date"), DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime DueDate { get; set; }

        [Display(Name = "To date"), DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? DueDate_New { get; set; }

        [Index]
        public int DueDateStatus_Id { get; set; }

        public string Remark { get; set; }

        [Index]
        public Guid Service_Id { get; set; }

        public virtual Services Services { get; set; }
        public virtual System_DueDateStatuses System_DueDateStatuses { get; set; }
        public DateTime? Update { get; set; }

        [Index]
        public Guid User_Id { get; set; }
    }
}
