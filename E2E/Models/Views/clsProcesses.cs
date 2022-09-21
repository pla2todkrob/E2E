using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Views
{
    public class clsProcesses
    {
        public bool Active { get; set; }
        public DateTime Create { get; set; }

        [Display(Name = "Department")]
        public string Department_Name { get; set; }

        [Display(Name = "Division")]
        public string Division_Name { get; set; }

        [Display(Name = "Plant")]
        public string Plant_Name { get; set; }

        [Display(Name = "Process")]
        public string Process_Name { get; set; }

        public Guid Processes_Id { get; set; }

        [Display(Name = "Section")]
        public string Section_Name { get; set; }

        public DateTime? Update { get; set; }
    }
}
