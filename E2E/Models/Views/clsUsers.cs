using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace E2E.Models.Views
{
    public class ClsActiveDirectoryInfo
    {
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string DistinguishedName { get; set; }
        public Guid? Guid { get; set; }
        public string Name { get; set; }
        public string SamAccountName { get; set; }
        public SecurityIdentifier Sid { get; set; }
        public string StructuralObjectClass { get; set; }
        public string UserPrincipalName { get; set; }
    }

    public class ClsUsers
    {
        public bool Active { get; set; }
        public bool BusinessCardGroup { get; set; }
        public DateTime Create { get; set; }

        [Display(Name = "Department")]
        public string Department_Name { get; set; }

        [Display(Name = "Division")]
        public string Division_Name { get; set; }

        [Display(Name = "Grade")]
        public string Grade_Name { get; set; }

        [Display(Name = "Position")]
        public string Grade_Position { get; set; }

        [Display(Name = "Line of work")]
        public string LineWork_Name { get; set; }

        [Display(Name = "Plant")]
        public string Plant_Name { get; set; }

        [Display(Name = "Process")]
        public string Process_Name { get; set; }

        public string Role { get; set; }

        [Display(Name = "Section")]
        public string Section_Name { get; set; }

        public DateTime? Update { get; set; }

        [Display(Name = "Code")]
        public string User_Code { get; set; }

        [Display(Name = "Cost center")]
        public string User_CostCenter { get; set; }

        [Display(Name = "Email")]
        public string User_Email { get; set; }

        public Guid User_Id { get; set; }

        [Display(Name = "Name EN")]
        public string User_Name_EN { get; set; }

        [Display(Name = "Name TH")]
        public string User_Name_TH { get; set; }

        [Display(Name = "Point")]
        public int User_Point { get; set; }

        public string Username { get; set; }

        public void RemoveCookie()
        {
            try
            {
                var cookies = HttpContext.Current.Request.Cookies.AllKeys;
                foreach (var item in cookies)
                {
                    HttpCookie myCookie = new HttpCookie(item)
                    {
                        Expires = DateTime.Now.AddDays(-1d)
                    };
                    HttpContext.Current.Response.Cookies.Add(myCookie);
                }

                FormsAuthentication.RedirectToLoginPage();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
