using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class ClsServiceUserActionName
    {

        public DateTime? Update { get; set; }
        public string Subject { get; set; }
        [Display(Name = "Action by")]
        public string ActionBy { get; set; }
        public string Requester { get; set; }
        public System_Priorities System_Priorities { get; set; }
        [Display(Name = "Due date"), DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? Duedate { get; set; }
        public System_Statuses  System_Statuses { get; set; }
        [Display(Name = "Estimate time")]
        public int Estimate_time { get; set; }
        public DateTime Create { get; set; }
        public string Key { get; set; }
        public Guid ServiceId { get; set; }
        public bool Is_OverDue { get; set; }



    }
}