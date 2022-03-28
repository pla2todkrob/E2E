using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class Tasks
    {
        [Key]
        public Guid Task_Id { get; set; }
        public string Task_Key { get; set; }

        public string Task_Subject { get; set; }
        public string Task_Description { get; set; }
        public Guid Department_Id { get; set; }
        public virtual Master_Departments Master_Departments { get; set; }
        public Guid User_Id { get; set; }
        public virtual Users Users { get; set; }
        public Guid? Ref_Task_Id { get; set; }
        public string Remark { get; set; }
        public DateTime Create { get; set; }
        public DateTime? Update { get; set; }
        public bool GetJob { get; set; }
        public Guid Status_Id { get; set; }
        public virtual System_Statuses System_Statuses { get; set; }
        public Tasks()
        {
            Task_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }
    }
}