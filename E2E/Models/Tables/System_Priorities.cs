using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class System_Priorities
    {
        [Key]
        public int Priority_Id { get; set; }

        [Display(Name = "Priority")]
        public string Priority_Name { get; set; }

        public string Priority_Class { get; set; }
        public int Priority_Point { get; set; }
        public int Priority_DateRange { get; set; }

        public static List<System_Priorities> DefaultList()
        {
            List<System_Priorities> list = new List<System_Priorities>();
            list.Add(new System_Priorities() { Priority_Name = "Cool", Priority_Class = "badge badge-success", Priority_Point = 1, Priority_DateRange = 5 });
            list.Add(new System_Priorities() { Priority_Name = "Control", Priority_Class = "badge badge-warning", Priority_Point = 2, Priority_DateRange = 3 });
            list.Add(new System_Priorities() { Priority_Name = "Critical", Priority_Class = "badge badge-danger", Priority_Point = 3, Priority_DateRange = 0 });

            return list;
        }
    }
}