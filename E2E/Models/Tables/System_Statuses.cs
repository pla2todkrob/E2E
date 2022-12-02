using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class System_Statuses
    {
        public string Status_Class { get; set; }

        [Key]
        public int Status_Id { get; set; }

        [Display(Name = "Status")]
        public string Status_Name { get; set; }

        public static List<System_Statuses> DefaultList()
        {
            List<System_Statuses> list = new List<System_Statuses>
            {
                new System_Statuses() { Status_Name = "Pending", Status_Class = "badge badge-secondary" },
                new System_Statuses() { Status_Name = "In progress", Status_Class = "badge badge-warning" },
                new System_Statuses() { Status_Name = "Complete", Status_Class = "badge badge-success" },
                new System_Statuses() { Status_Name = "Close", Status_Class = "badge badge-light" },
                new System_Statuses() { Status_Name = "Reject", Status_Class = "badge badge-danger" },
                new System_Statuses() { Status_Name = "Cancel", Status_Class = "badge badge-danger" }
            };

            return list;
        }
    }
}
