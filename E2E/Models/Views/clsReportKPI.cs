using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
            User_Id = new List<Guid?>();
        }

        [Display(Name = "From")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? Date_From { get; set; }

        [Display(Name = "To")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? Date_To { get; set; }

        [Display(Name = "Users")]
        public List<Guid?> User_Id { get; set; }
    }

    public class ReportKPI_Overview
    {
        public double? Average_Score { get; set; }
        public int Close_Count { get; set; }
        public int Complete_Count { get; set; }
        public int Inprogress_Count { get; set; }
        public int OverDue_Count { get; set; }
        public int Pending_Count { get; set; }
        public int Total { get; set; }
    }

    public class ReportKPI_User
    {
        public double? Average_Score { get; set; }
        public int Close_Count { get; set; }
        public int Complete_Count { get; set; }
        public int Inprogress_Count { get; set; }
        public int OverDue_Count { get; set; }
        public int Pending_Count { get; set; }
        public int Total { get; set; }
        public Guid User_Id { get; set; }
        public string User_Name { get; set; }
    }
}