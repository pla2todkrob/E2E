using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Views
{
    public class ClsServices
    {
        public ClsServices()
        {
            ServiceFiles = new List<ServiceFiles>();
            ClsServiceComments = new List<ClsServiceComments>();
            ClsServiceTeams = new List<ClsServiceTeams>();
        }

        [Display(Name = "Email")]
        public string Action_Email { get; set; }

        [Display(Name = "Action by")]
        public string Action_Name { get; set; }

        public List<ClsServiceComments> ClsServiceComments { get; set; }

        public List<ClsServiceTeams> ClsServiceTeams { get; set; }

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

    public class ServiceAll
    {
        public DateTime Create { get; set; }

        [Display(Name = "Department")]
        public string Department_Name { get; set; }

        public bool Is_OverDue { get; set; }

        [Display(Name = "Plant")]
        public string Plant_Name { get; set; }

        public string Priority_Class { get; set; }
        public int Priority_Id { get; set; }

        [Display(Name = "Priority")]
        public string Priority_Name { get; set; }

        public Guid Service_Id { get; set; }
        public string Service_Key { get; set; }

        [Display(Name = "Subject")]
        public string Service_Subject { get; set; }

        public string Status_Class { get; set; }

        [Display(Name = "Status")]
        public string Status_Name { get; set; }

        public DateTime? Update { get; set; }
    }
}
