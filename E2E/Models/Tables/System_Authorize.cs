using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class System_Authorize
    {
        [Key]
        public int Authorize_Id { get; set; }

        [Display(Name = "Authorize"), StringLength(100), Index(IsUnique = true)]
        public string Authorize_Name { get; set; }

        public static List<System_Authorize> DefaultList()
        {
            List<System_Authorize> list = new List<System_Authorize>
            {
                new System_Authorize() { Authorize_Name = "Acknowledge" },
                new System_Authorize() { Authorize_Name = "Approved" },
                new System_Authorize() { Authorize_Name = "Requestor" }
            };

            return list;
        }
    }
}
