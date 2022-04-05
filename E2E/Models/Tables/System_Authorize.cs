using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class System_Authorize
    {
        [Key]
        public Guid Authorize_Id { get; set; }

        public int Authorize_Index { get; set; }
        [Display(Name = "Authorize")]
        public string Authorize_Name { get; set; }

        public System_Authorize()
        {
            Authorize_Id = Guid.NewGuid();
        }

        public static List<System_Authorize> DefaultList()
        {
            List<System_Authorize> list = new List<System_Authorize>();
            list.Add(new System_Authorize() { Authorize_Index = 1, Authorize_Name = "Acknowledge" });
            list.Add(new System_Authorize() { Authorize_Index = 2, Authorize_Name = "Approved" });
            list.Add(new System_Authorize() { Authorize_Index = 3, Authorize_Name = "Requestor" });

            return list;
        }
    }
}