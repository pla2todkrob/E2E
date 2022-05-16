using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class clsServices
    {
        [Display(Name = "Name")]
        public string User_Name { get; set; }

        [Display(Name = "Notify required approved by")]
        public string Required_User_Name { get; set; }

        [Display(Name = "Notify required approved by")]
        public string Required_User_Code { get; set; }

        [Display(Name = "Notify required approved by")]
        public string Required_User_Email { get; set; }

        [Display(Name = "Commit by")]
        public string Commit_User_Name { get; set; }

        [Display(Name = "Commit by")]
        public string Commit_User_Code { get; set; }

        [Display(Name = "Commit by")]
        public string Commit_User_Email { get; set; }

        [Display(Name = "Approved by")]
        public string Approved_User_Name { get; set; }

        [Display(Name = "Approved by")]
        public string Approved_User_Code { get; set; }

        [Display(Name = "Approved by")]
        public string Approved_User_Email { get; set; }

        [Display(Name = "Action by")]
        public string Action_User_Name { get; set; }

        [Display(Name = "Action by")]
        public string Action_User_Code { get; set; }

        [Display(Name = "Action by")]
        public string Action_User_Email { get; set; }

        public Services Services { get; set; }
        public List<ServiceFiles> ServiceFiles { get; set; }

        public clsServices()
        {
            ServiceFiles = new List<ServiceFiles>();
        }
    }
}