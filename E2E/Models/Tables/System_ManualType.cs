using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class System_ManualType
    {
        [Key]
        public Guid Manual_Type_Id { get; set; }
        public string Manual_TypeName { get; set; }
        public DateTime Create { get; set; }

        public static List<System_ManualType> DefaultList()
        {
            List<System_ManualType> list = new List<System_ManualType>();
            list.Add(new System_ManualType() { Manual_TypeName = "TOPIC", Create = DateTime.Now });
            list.Add(new System_ManualType() { Manual_TypeName = "EFORM", Create = DateTime.Now });
            list.Add(new System_ManualType() { Manual_TypeName = "SERVICE", Create = DateTime.Now });

            return list;
        }

        public System_ManualType()
        {
            Manual_Type_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }
    }
}