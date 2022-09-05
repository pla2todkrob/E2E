using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Views
{
    public class clsChangeDueDates
    {
        public Guid ChangeDueDate_Id { get; set; }
        public DateTime Create { get; set; }

        [Display(Name = "To date")]
        public DateTime? DueDate_New { get; set; }

        [Display(Name = "From date")]
        public DateTime DueDate_Old { get; set; }

        public string DueDateStatus_Class { get; set; }

        [Display(Name = "Status")]
        public string DueDateStatus_Name { get; set; }

        [Display(Name = "Key")]
        public string Service_Key { get; set; }

        [Display(Name = "Subject")]
        public string Service_Subject { get; set; }

        public DateTime? Update { get; set; }

        [Display(Name = "User")]
        public string User_Name { get; set; }
    }
}