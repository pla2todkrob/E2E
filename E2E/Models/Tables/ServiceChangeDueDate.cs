using System;
using System.ComponentModel.DataAnnotations;

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

        //public virtual Users Users { get; set; }
        public DateTime Create { get; set; }

        [Display(Name = "From date"), DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime DueDate { get; set; }

        [Display(Name = "To date"), DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? DueDate_New { get; set; }

        public int DueDateStatus_Id { get; set; }
        public Guid Service_Id { get; set; }
        public virtual Services Services { get; set; }
        public virtual System_DueDateStatuses System_DueDateStatuses { get; set; }
        public DateTime? Update { get; set; }
        public Guid? User_Id { get; set; }
        public string Remark { get; set; }
    }
}