using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class UserDetails
    {
        [Key]
        public Guid Detail_Id { get; set; }

        public Guid User_Id { get; set; }
        public virtual Users Users { get; set; }
        public string Detail_TH_Prefix { get; set; }
        public string Detail_TH_FirstName { get; set; }
        public string Detail_TH_LastName { get; set; }
        public string Detail_EN_Prefix { get; set; }
        public string Detail_EN_FirstName { get; set; }
        public string Detail_EN_LastName { get; set; }
        public string Detail_Tel { get; set; }
        public string Detail_Password { get; set; }
    }
}