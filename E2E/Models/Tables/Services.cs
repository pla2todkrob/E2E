using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class Services
    {
        [Key]
        public Guid Service_Id { get; set; }

        public string Service_Key { get; set; }

        public string Service_Subject { get; set; }
        public string Service_Description { get; set; }
        public Guid? Department_Id { get; set; }
        public virtual Master_Departments Master_Departments { get; set; }
        public Guid User_Id { get; set; }
        public virtual Users Users { get; set; }
        public Guid? Ref_Service_Id { get; set; }
        public DateTime Create { get; set; }
        public DateTime? Update { get; set; }
        public bool CommitService { get; set; }
        public Guid? Commit_User_Id { get; set; }
        public DateTime? Commit_DateTime { get; set; }
        public bool RequiredApprove { get; set; }
        public Guid? Approve_User_Id { get; set; }
        public DateTime? Approve_DateTime { get; set; }
        public Guid Status_Id { get; set; }
        public virtual System_Statuses System_Statuses { get; set; }
        public Guid Priority_Id { get; set; }
        public virtual System_Priorities System_Priorities { get; set; }
        public DateTime Service_DueDate { get; set; }

        public Services()
        {
            Service_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }
    }
}