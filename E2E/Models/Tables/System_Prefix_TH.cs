using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class System_Prefix_TH
    {
        [Key]
        public Guid Prefix_TH_Id { get; set; }
        public string Prefix_TH_Name { get; set; }
        public System_Prefix_TH()
        {
            Prefix_TH_Id = Guid.NewGuid();
        }
    }
}