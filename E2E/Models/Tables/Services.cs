﻿using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class Services
    {
        public Services()
        {
            Service_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        public Guid? Action_User_Id { get; set; }
        public Guid? Assign_User_Id { get; set; }
        public DateTime Create { get; set; }

        public Guid Create_User_Id { get; set; }

        [Display(Name = "Department")]
        public Guid? Department_Id { get; set; }

        public bool Is_Action { get; set; }

        public bool Is_Approval { get; set; }

        public bool Is_Commit { get; set; }

        public bool Is_FreePoint { get; set; }

        public bool Is_MustBeApproved { get; set; }
        public bool Is_OverDue { get; set; }

        [Display(Name = "Priority")]
        public int Priority_Id { get; set; }

        [Display(Name = "Service Reference")]
        public Guid? Ref_Service_Id { get; set; }

        [Display(Name = "Actual time")]
        public int Service_ActualTime { get; set; }

        [Display(Name = "Contact")]
        public string Service_Contact { get; set; }

        [Display(Name = "Description")]
        public string Service_Description { get; set; }

        [Display(Name = "Due date"), DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? Service_DueDate { get; set; }

        [Display(Name = "Estimate time")]
        public int Service_EstimateTime { get; set; }

        public int Service_FileCount { get; set; }

        [Key]
        public Guid Service_Id { get; set; }

        [Display(Name = "Key")]
        public string Service_Key { get; set; }

        [Display(Name = "Subject")]
        public string Service_Subject { get; set; }

        //public virtual Master_Departments Master_Departments { get; set; }

        [Display(Name = "Status")]
        public int Status_Id { get; set; }

        public virtual System_Priorities System_Priorities { get; set; }

        public virtual System_Statuses System_Statuses { get; set; }

        public DateTime? Update { get; set; }

        [Display(Name = "User")]
        public Guid User_Id { get; set; }

        public virtual Users Users { get; set; }

        [Display(Name = "Work root")]
        public Guid? WorkRoot_Id { get; set; }

        public virtual WorkRoots WorkRoots { get; set; }
    }
}
