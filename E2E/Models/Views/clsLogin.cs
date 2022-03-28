using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class clsLogin
    {
        [Display(Name = "Email, Code"), Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public bool Remember { get; set; }
        public clsLogin()
        {
            Remember = true;
        }
    }
}