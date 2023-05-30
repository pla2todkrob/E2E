using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class System_ManualType
    {
        public System_ManualType()
        {
            Manual_Type_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        public DateTime Create { get; set; }

        [Key]
        public Guid Manual_Type_Id { get; set; }

        [Display(Name = "Type"), StringLength(100), Index(IsUnique = true)]
        public string Manual_TypeName { get; set; }

        public static List<System_ManualType> DefaultList()
        {
            List<System_ManualType> list = new List<System_ManualType>
            {
                new System_ManualType() { Manual_TypeName = "TOPIC", Create = DateTime.Now },
                new System_ManualType() { Manual_TypeName = "EFORM", Create = DateTime.Now },
                new System_ManualType() { Manual_TypeName = "SERVICE", Create = DateTime.Now },
                new System_ManualType() { Manual_TypeName = "BUSINESS CARD", Create = DateTime.Now }
            };

            return list;
        }
    }
}
