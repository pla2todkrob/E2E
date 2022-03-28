﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class Master_Departments
    {
        [Key]
        public Guid Department_Id { get; set; }

        [Description("Department"), Display(Name = "Department"), Required]
        public string Department_Name { get; set; }

        [Description("Autorun number")]
        public int Code { get; set; }
        [Required,Display(Name ="Division")]
        public Guid? Division_Id { get; set; }
        public virtual Master_Divisions Master_Divisions { get; set; }
        public bool Active { get; set; }
        public DateTime Create { get; set; }
        public DateTime? Update { get; set; }

        public Master_Departments()
        {
            Department_Id = Guid.NewGuid();
            Active = true;
            Create = DateTime.Now;
        }
    }
}