using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Views
{
    public class clsLogin
    {
        public clsLogin()
        {
            Remember = true;
        }

        [Required]
        public string Password { get; set; }

        public bool Remember { get; set; }

        [Display(Name = "Email, Code"), Required]
        public string Username { get; set; }
    }
}