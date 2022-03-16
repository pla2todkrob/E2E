using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class System_Configurations
    {
        [Key]
        public Guid Configuration_Id { get; set; }

        public string Configuration_Brand { get; set; }
        public Guid User_Id { get; set; }
        public virtual Users Users { get; set; }
        public DateTime CreateDateTime { get; set; }

        public System_Configurations()
        {
            Configuration_Id = Guid.NewGuid();
            CreateDateTime = DateTime.Now;
        }
    }
}