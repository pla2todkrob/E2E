﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class System_Priorities
    {
        public string Priority_Class { get; set; }

        public int Priority_DateRange { get; set; }

        [Key]
        public int Priority_Id { get; set; }

        [Display(Name = "Priority"), StringLength(100), Index(IsUnique = true)]
        public string Priority_Name { get; set; }

        public int Priority_Point { get; set; }

        public static List<System_Priorities> DefaultList()
        {
            List<System_Priorities> list = new List<System_Priorities>
            {
                new System_Priorities() { Priority_Name = "Cool", Priority_Class = "badge badge-success", Priority_Point = 1, Priority_DateRange = 5 },
                new System_Priorities() { Priority_Name = "Control", Priority_Class = "badge badge-warning", Priority_Point = 2, Priority_DateRange = 3 },
                new System_Priorities() { Priority_Name = "Critical", Priority_Class = "badge badge-danger", Priority_Point = 3, Priority_DateRange = 0 }
            };

            return list;
        }
    }
}
