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
        public ReportKPI_Filter()
        {
            Date_To = DateTime.Today;
        }

        [Display(Name = "From"), DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? Date_From { get; set; }

        [Display(Name = "To"), DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime Date_To { get; set; }
    }

    public class ReportKPI_Overview
    {
        [Display(Name = "Close")]
        public int Close_Count { get; set; }

        [Display(Name = "Complete")]
        public int Complete_Count { get; set; }

        [Display(Name = "In progress")]
        public int Inprogress_Count { get; set; }

        [Display(Name = "Ontime")]
        public int OnTime_Count { get; set; }

        [Display(Name = "Ontime %")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:p}")]
        public double OnTime_Percent { get; set; }

        [Display(Name = "Over due")]
        public int OverDue_Count { get; set; }

        [Display(Name = "Pending")]
        public int Pending_Count { get; set; }

        [Display(Name = "Satisfied %")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:p}")]
        public double Satisfied_Percent { get; set; }

        [Display(Name = "Total")]
        public int Total { get; set; }

        [Display(Name = "Unsatisfied")]
        public int Unsatisfied_Count { get; set; }
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

        [Display(Name = "Join team")]
        public int JoinTeam_Count { get; set; }

        [Display(Name = "Over due")]
        public int OverDue_Count { get; set; }

        [Display(Name = "Pending")]
        public int Pending_Count { get; set; }

        [Display(Name = "Point")]
        public int SuccessPoint { get; set; }

        [Display(Name = "Total")]
        public int Total { get; set; }

        public Guid User_Id { get; set; }

        [Display(Name = "User")]
        public string User_Name { get; set; }
    }

    public class ReportKPI_User_Views
    {
        public DateTime Create { get; set; }

        [Display(Name = "Point")]
        public int Priority_Point { get; set; }

        [Display(Name = "Average")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:F}")]
        public double? Satisfaction_Average { get; set; }

        public Guid Service_Id { get; set; }
        public string Service_Key { get; set; }

        [Display(Name = "Subject")]
        public string Service_Subject { get; set; }

        public string Status_Class { get; set; }

        [Display(Name = "Status")]
        public string Status_Name { get; set; }
    }
}