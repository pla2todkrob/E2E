﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public Guid Prefix_TH_Id { get; set; }
        public virtual System_Prefix_TH System_Prefix_TH { get; set; }
        public string Detail_TH_FirstName { get; set; }
        public string Detail_TH_LastName { get; set; }
        public Guid Prefix_EN_Id { get; set; }
        public virtual System_Prefix_EN System_Prefix_EN { get; set; }
        public string Detail_EN_FirstName { get; set; }
        public string Detail_EN_LastName { get; set; }
        public string Detail_Tel { get; set; }
        [DataType(DataType.Password)]
        public string Detail_Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Detail_Password")]
        [NotMapped]
        public string Detail_ConfirmPassword { get; set; }
        public UserDetails()
        {
            Detail_Id = Guid.NewGuid();
        }
    }
}