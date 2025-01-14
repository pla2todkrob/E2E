﻿using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Views
{
    public class ClsDivisions
    {
        public bool Active { get; set; }
        public DateTime Create { get; set; }
        public Guid Division_Id { get; set; }

        [Display(Name = "Division")]
        public string Division_Name { get; set; }

        public DateTime? Update { get; set; }
    }
}
