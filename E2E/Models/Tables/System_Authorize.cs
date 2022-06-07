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
        public int Authorize_Id { get; set; }
        [Display(Name = "Authorize")]
        public string Authorize_Name { get; set; }


        public static List<System_Authorize> DefaultList()
        {
            List<System_Authorize> list = new List<System_Authorize>();
            list.Add(new System_Authorize() { Authorize_Name = "Acknowledge" });
            list.Add(new System_Authorize() { Authorize_Name = "Approved" });
            list.Add(new System_Authorize() { Authorize_Name = "Requestor" });

            return list;
        }
    }
}