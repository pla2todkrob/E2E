using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class System_DueDateStatuses
    {
        [Key]
        public int DueDateStatus_Id { get; set; }
        [Display(Name ="Status")]
        public string DueDateStatus_Name { get; set; }
        public string DueDateStatus_Class { get; set; }
        public static List<System_DueDateStatuses> DefaultList()
        {
            List<System_DueDateStatuses> list = new List<System_DueDateStatuses>();
            list.Add(new System_DueDateStatuses() { DueDateStatus_Name = "Pending", DueDateStatus_Class = "badge badge-secondary" });
            list.Add(new System_DueDateStatuses() { DueDateStatus_Name = "Accept", DueDateStatus_Class = "badge badge-success" });
            list.Add(new System_DueDateStatuses() { DueDateStatus_Name = "Reject", DueDateStatus_Class = "badge badge-danger" });
            list.Add(new System_DueDateStatuses() { DueDateStatus_Name = "Cancel", DueDateStatus_Class = "badge badge-danger" });

            return list;
        }

    }
}