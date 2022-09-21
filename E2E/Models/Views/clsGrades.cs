using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Views
{
    public class clsGrades
    {
        public bool Active { get; set; }
        public DateTime Create { get; set; }
        public Guid Grade_Id { get; set; }

        [Display(Name = "Grade")]
        public string Grade_Name { get; set; }

        [Display(Name = "Position")]
        public string Grade_Position { get; set; }

        [Display(Name = "Line of work")]
        public string LineWork_Name { get; set; }

        public DateTime? Update { get; set; }
    }
}
