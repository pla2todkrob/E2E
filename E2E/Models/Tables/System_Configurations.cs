using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class System_Configurations
    {
        public System_Configurations()
        {
            Configuration_Id = Guid.NewGuid();
            CreateDateTime = DateTime.Now;
        }

        [Display(Name = "Brand")]
        public string Configuration_Brand { get; set; }

        [Key]
        public Guid Configuration_Id { get; set; }

        [Display(Name = "Point")]
        public int Configuration_Point { get; set; }

        public string Copyright { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string SystemName { get; set; }
        public Guid User_Id { get; set; }
        public virtual Users Users { get; set; }
    }
}
