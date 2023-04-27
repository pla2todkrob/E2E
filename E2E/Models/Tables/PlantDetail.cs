using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class PlantDetail
    {

        [Key]
        public Guid PlantDetail_Id { get; set; } = Guid.NewGuid();
        public Guid Plant_Id { get; set; }
        public string OfficeFax { get; set; }
        public string OfficeAddress1 { get; set; }
        public string OfficeAddress2 { get; set; }
        [DataType(DataType.PhoneNumber)]public string OfficeNumber { get; set; }
        public string OfficeName { get; set; }
        public virtual Master_Plants Master_Plants { get; set; }

    }
}