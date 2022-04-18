﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class System_Priorities
    {
        [Key]
        public Guid Priority_Id { get; set; }

        public string Priority_Name { get; set; }
        public int Priority_Index { get; set; }
        public string Priority_Class { get; set; }
        public int Priority_Point { get; set; }
        public int Priority_DateRange { get; set; }

        public System_Priorities()
        {
            Priority_Id = Guid.NewGuid();
        }

        public static List<System_Priorities> DefaultList()
        {
            List<System_Priorities> list = new List<System_Priorities>();
            list.Add(new System_Priorities() { Priority_Index = 1, Priority_Name = "Cool", Priority_Class = "badge badge-success", Priority_Point = 1, Priority_DateRange = 5 });
            list.Add(new System_Priorities() { Priority_Index = 2, Priority_Name = "Control", Priority_Class = "badge badge-warning", Priority_Point = 1, Priority_DateRange = 3 });
            list.Add(new System_Priorities() { Priority_Index = 3, Priority_Name = "Critical", Priority_Class = "badge badge-danger", Priority_Point = 2, Priority_DateRange = 0 });

            return list;
        }
    }
}