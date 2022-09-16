using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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

    public class clsAuditReport
    {
        public clsAuditReport()
        {
            WorkRoots = new WorkRoots();
        }

        public DateTime Create { get; set; }
        public Guid Service_Id { get; set; }
        public string Service_Key { get; set; }
        public string Service_Subject { get; set; }
        public DateTime Update { get; set; }
        public string User_Name { get; set; }
        public WorkRoots WorkRoots { get; set; }
    }
}