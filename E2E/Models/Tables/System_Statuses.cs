using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class System_Statuses
    {
        public int OrderBusinessCard { get; set; }
        public string Status_Class { get; set; }

        [Key]
        public int Status_Id { get; set; }

        [Display(Name = "Status"), StringLength(100), Index(IsUnique = true)]
        public string Status_Name { get; set; }

        public static List<System_Statuses> DefaultList()
        {
            List<System_Statuses> list = new List<System_Statuses>
            {
                new System_Statuses() { Status_Name = "Pending", Status_Class = "badge badge-secondary",OrderBusinessCard = 1},
                new System_Statuses() { Status_Name = "In progress", Status_Class = "badge badge-warning" ,OrderBusinessCard=4},
                new System_Statuses() { Status_Name = "Completed", Status_Class = "badge badge-success" ,OrderBusinessCard=6},
                new System_Statuses() { Status_Name = "Closed", Status_Class = "badge badge-light" ,OrderBusinessCard=7},
                new System_Statuses() { Status_Name = "Rejected", Status_Class = "badge badge-danger" ,OrderBusinessCard=9},
                new System_Statuses() { Status_Name = "Cancel", Status_Class = "badge badge-danger" ,OrderBusinessCard=8},
                new System_Statuses() { Status_Name = "Approved", Status_Class = "badge badge-success",OrderBusinessCard=2 },
                new System_Statuses() { Status_Name = "Assigned", Status_Class = "badge badge-secondary" ,OrderBusinessCard=3},
                new System_Statuses() { Status_Name = "Confirmed", Status_Class = "badge badge-info",OrderBusinessCard=5 }
            };

            return list;
        }
    }
}
