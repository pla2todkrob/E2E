﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class clsUsers
    {
        public Guid User_Id { get; set; }
        public DateTime Create { get; set; }
        [Display(Name = "Code")]
        public string User_Code { get; set; }
        [Display(Name = "Email")]
        public string User_Email { get; set; }
        [Display(Name = "Line of work")]
        public string LineWork_Name { get; set; }
        [Display(Name = "Grade")]
        public string Grade_Name { get; set; }
        [Display(Name = "Position")]
        public string Grade_Position { get; set; }
        [Display(Name = "Plant")]
        public string Plant_Name { get; set; }
        [Display(Name = "Division")]
        public string Division_Name { get; set; }
        [Display(Name = "Department")]
        public string Department_Name { get; set; }
        [Display(Name = "Section")]
        public string Section_Name { get; set; }
        [Display(Name = "Process")]
        public string Process_Name { get; set; }
        public bool Active { get; set; }
        public DateTime? Update { get; set; }
    }
}