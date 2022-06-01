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
        [Display(Name = "Name")]
        public string Action_Name { get; set; }
        [Display(Name = "Email")]
        public string Action_Email { get; set; }
        public Services Services { get; set; }
        public List<ServiceFiles> ServiceFiles { get; set; }
        public List<clsServiceComments> ClsServiceComments { get; set; }

        public clsServices()
        {
            ServiceFiles = new List<ServiceFiles>();
            ClsServiceComments = new List<clsServiceComments>();
        }
    }
}