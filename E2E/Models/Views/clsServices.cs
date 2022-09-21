using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Views
{
    public class clsServices
    {
        public clsServices()
        {
            ServiceFiles = new List<ServiceFiles>();
            ClsServiceComments = new List<clsServiceComments>();
            ClsServiceTeams = new List<clsServiceTeams>();
        }

        [Display(Name = "Email")]
        public string Action_Email { get; set; }

        [Display(Name = "Action by")]
        public string Action_Name { get; set; }

        public List<clsServiceComments> ClsServiceComments { get; set; }

        public List<clsServiceTeams> ClsServiceTeams { get; set; }

        [Display(Name = "Create by")]
        public string Create_Name { get; set; }

        public Guid Service_Id { get; set; }

        public ServiceChangeDueDate ServiceChangeDueDate { get; set; }

        public List<ServiceFiles> ServiceFiles { get; set; }

        public Services Services { get; set; }

        [Display(Name = "Assign to")]
        public Guid User_Id { get; set; }

        [Display(Name = "Request by")]
        public string User_Name { get; set; }
    }
}
