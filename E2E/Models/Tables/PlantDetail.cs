using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class PlantDetail
    {
        public virtual Master_Plants Master_Plants { get; set; }

        public string OfficeAddress1 { get; set; }

        public string OfficeAddress2 { get; set; }

        public string OfficeFax { get; set; }

        public string OfficeName { get; set; }

        [DataType(DataType.PhoneNumber)] public string OfficeNumber { get; set; }

        public Guid Plant_Id { get; set; }

        [Key]
        public Guid PlantDetail_Id { get; set; } = Guid.NewGuid();
    }
}
