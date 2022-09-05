using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Views
{
    public class clsPassword
    {
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Old Password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        public Guid User_Id { get; set; }
    }
}