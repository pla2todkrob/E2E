using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class UserDetails
    {
        public UserDetails()
        {
            Detail_Id = Guid.NewGuid();
        }

        [DataType(DataType.Password)]
        [Compare("Detail_Password")]
        [NotMapped]
        public string Detail_ConfirmPassword { get; set; }

        [Display(Name = "EN First name")]
        public string Detail_EN_FirstName { get; set; }

        [Display(Name = "EN Last name")]
        public string Detail_EN_LastName { get; set; }

        [Key]
        public Guid Detail_Id { get; set; }

        [DataType(DataType.Password)]
        public string Detail_Password { get; set; }

        [Display(Name = "TH First name")]
        public string Detail_TH_FirstName { get; set; }

        [Display(Name = "TH Last name")]
        public string Detail_TH_LastName { get; set; }

        [Display(Name = "Prefix EN")]
        public int Prefix_EN_Id { get; set; }

        [Display(Name = "Prefix TH")]
        public int Prefix_TH_Id { get; set; }

        public virtual System_Prefix_EN System_Prefix_EN { get; set; }
        public virtual System_Prefix_TH System_Prefix_TH { get; set; }
        public Guid User_Id { get; set; }
        public virtual Users Users { get; set; }
    }
}