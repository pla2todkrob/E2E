using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Views
{
    public class ClsDepartments
    {
        public bool Active { get; set; }
        public DateTime Create { get; set; }
        public Guid? Department_Id { get; set; }

        [Display(Name = "Department")]
        public string Department_Name { get; set; }

        [Display(Name = "Division")]
        public string Division_Name { get; set; }

        public DateTime? Update { get; set; }
    }
}
