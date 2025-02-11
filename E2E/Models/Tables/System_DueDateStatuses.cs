﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class System_DueDateStatuses
    {
        public string DueDateStatus_Class { get; set; }

        [Key]
        public int DueDateStatus_Id { get; set; }

        [Display(Name = "Status"), StringLength(100), Index(IsUnique = true)]
        public string DueDateStatus_Name { get; set; }

        public static List<System_DueDateStatuses> DefaultList()
        {
            List<System_DueDateStatuses> list = new List<System_DueDateStatuses>
            {
                new System_DueDateStatuses() { DueDateStatus_Name = "Pending", DueDateStatus_Class = "badge badge-secondary" },
                new System_DueDateStatuses() { DueDateStatus_Name = "Accept", DueDateStatus_Class = "badge badge-success" },
                new System_DueDateStatuses() { DueDateStatus_Name = "Reject", DueDateStatus_Class = "badge badge-danger" },
                new System_DueDateStatuses() { DueDateStatus_Name = "Cancel", DueDateStatus_Class = "badge badge-danger" }
            };

            return list;
        }
    }
}
