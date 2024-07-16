using E2E.Models.Tables;
using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Views
{
    public class AuditReport_Filter
    {
        public AuditReport_Filter()
        {
            Date_From = DateTime.Today.AddYears(-3);
            Date_To = DateTime.Today;
        }

        [Display(Name = "From"), DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime Date_From { get; set; }

        [Display(Name = "To"), DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime Date_To { get; set; }
    }

    public class ClsAuditReport
    {
        public ClsAuditReport()
        {
            WorkRoots = new WorkRoots();
        }

        public Guid Service_Id { get; set; }
        public string Service_Key { get; set; }

        [Display(Name = "Subject")]
        public string Service_Subject { get; set; }

        public DateTime Update { get; set; }

        [Display(Name = "Username")]
        public string User_Name { get; set; }

        public WorkRoots WorkRoots { get; set; }
    }
}
