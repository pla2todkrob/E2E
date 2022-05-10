﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class Users
    {
        public Users()
        {
            User_Id = Guid.NewGuid();
            Active = true;
            Create = DateTime.Now;
        }

        public bool Active { get; set; }

        public DateTime Create { get; set; }

        [Required]
        public Guid Grade_Id { get; set; }

        public virtual Master_Grades Master_Grades { get; set; }

        public virtual Master_Processes Master_Processes { get; set; }

        [Required]
        public Guid Process_Id { get; set; }

        [Required]
        public Guid Role_Id { get; set; }

        public virtual System_Roles System_Roles { get; set; }

        public DateTime? Update { get; set; }

        [Description("Employee code"), Display(Name = "Code"), Required]
        public string User_Code { get; set; }

        [Description("Employee email"), Display(Name = "Email")]
        public string User_Email { get; set; }

        [Key]
        public Guid User_Id { get; set; }

        [Display(Name = "Point")]
        public int User_Point { get; set; }

        [Display(Name = "Cost center")]
        public string User_CostCenter { get; set; }

        public int YearSetPoint { get; set; }
    }
}