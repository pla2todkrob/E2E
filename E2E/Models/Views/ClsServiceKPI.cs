using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Views
{
    public class ClsServiceKPI
    {
        public int Remaining { get; set; }
        public List<Overview> Overviews { get; set; }
        public List<Individual> Individuals { get; set; }

        public class Overview
        {
            public int NumberYear { get; set; }
            public int NumberMonth { get; set; }

            [Display(Name = "Month")]
            public string Month { get; set; }

            [Display(Name = "Incoming job")]
            public int Incoming { get; set; }

            

            [Display(Name = "Completed")]
            public int Completed { get; set; }

            [Display(Name = "Close auto")]
            public int CloseAuto { get; set; }

            [Display(Name = "Close manual")]
            public int CloseManual { get; set; }

            [Display(Name = "Closed")]
            public int Closed { get; set; }
            [Display(Name = "Rejected")]
            public int Rejected { get; set; }

            [Display(Name = "Total job")]
            public int Total { get; set; }

            [Display(Name = "Completed total")]
            public int CompletedTotal { get; set; }

            [Display(Name = "Closed total")]
            public int ClosedTotal { get; set; }
            [Display(Name = "Rejected total")]
            public int RejectedTotal { get; set; }

            [Display(Name = "Overdue")]
            public int Overdue { get; set; }

            [Display(Name = "Ontime >= 90%")]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:p}")]
            public double Ontime { get; set; }

            [Display(Name = "Unsatisfied")]
            public int Unsatisfied { get; set; }

            [Display(Name = "Satisfied >= 96%")]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:p}")]
            public double Satisfied { get; set; }

            public Overview()
            {
                Ontime = 1;
                Satisfied = 1;
            }
        }

        public class Individual
        {
            public Guid UserId { get; set; }
            public int NumberYear { get; set; }

            [Display(Name = "User")]
            public string User { get; set; }

            [Display(Name = "Incoming job")]
            public int Incoming { get; set; }

            [Display(Name = "Completed")]
            public int Completed { get; set; }

            [Display(Name = "Close auto")]
            public int CloseAuto { get; set; }

            [Display(Name = "Close manual")]
            public int CloseManual { get; set; }

            [Display(Name = "Closed")]
            public int Closed { get; set; }
            [Display(Name = "Rejected")]
            public int Rejected { get; set; }

            [Display(Name = "Overdue")]
            public int Overdue { get; set; }

            [Display(Name = "Ontime >= 90%")]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:p}")]
            public double Ontime { get; set; }

            [Display(Name = "Unsatisfied")]
            public int Unsatisfied { get; set; }

            [Display(Name = "Satisfied >= 96%")]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:p}")]
            public double Satisfied { get; set; }

            public Individual()
            {
                Ontime = 1;
                Satisfied = 1;
            }
        }
    }
}
