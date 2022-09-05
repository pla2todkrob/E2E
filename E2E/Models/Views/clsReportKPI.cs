using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Views
{
    public class clsReportKPI
    {
        public clsReportKPI()
        {
            ReportKPI_Overview = new ReportKPI_Overview();
            ReportKPI_Users = new List<ReportKPI_User>();
        }

        public int Authorize_Id { get; set; }
        public ReportKPI_Overview ReportKPI_Overview { get; set; }
        public List<ReportKPI_User> ReportKPI_Users { get; set; }
    }

    public class ReportKPI_Filter
    {
        [Display(Name = "From")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? Date_From { get; set; }

        [Display(Name = "To")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? Date_To { get; set; }
    }

    public class ReportKPI_Overview
    {
        [Display(Name = "Average")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:F}")]
        public double? Average_Score { get; set; }

        [Display(Name = "Close")]
        public int Close_Count { get; set; }

        [Display(Name = "Complete")]
        public int Complete_Count { get; set; }

        [Display(Name = "In progress")]
        public int Inprogress_Count { get; set; }

        [Display(Name = "Over due")]
        public int OverDue_Count { get; set; }

        [Display(Name = "Pending")]
        public int Pending_Count { get; set; }

        [Display(Name = "Total")]
        public int Total { get; set; }
    }

    public class ReportKPI_User
    {
        [Display(Name = "Average")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:F}")]
        public double? Average_Score { get; set; }

        [Display(Name = "Close")]
        public int Close_Count { get; set; }

        [Display(Name = "Complete")]
        public int Complete_Count { get; set; }

        [Display(Name = "In progress")]
        public int Inprogress_Count { get; set; }

        [Display(Name = "Over due")]
        public int OverDue_Count { get; set; }

        [Display(Name = "Pending")]
        public int Pending_Count { get; set; }

        [Display(Name = "Total")]
        public int Total { get; set; }

        public Guid User_Id { get; set; }

        [Display(Name = "User")]
        public string User_Name { get; set; }
    }
}