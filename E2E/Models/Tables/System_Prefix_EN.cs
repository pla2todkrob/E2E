using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class System_Prefix_EN
    {
        [Key]
        public Guid Prefix_EN_Id { get; set; }
        public string Prefix_EN_Name { get; set; }
        public System_Prefix_EN()
        {
            Prefix_EN_Id = Guid.NewGuid();
        }
    }
}