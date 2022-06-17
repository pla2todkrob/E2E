using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class System_Language
    {
        [Key]
        public Guid Language_Id { get; set; }
        public string Language_Name { get; set; }
        public DateTime Create { get; set; }

        public static List<System_Language> DefaultList()
        {
            List<System_Language> list = new List<System_Language>();
            list.Add(new System_Language() { Language_Name = "TH", Create = DateTime.Now });
            list.Add(new System_Language() { Language_Name = "EN", Create = DateTime.Now });
            return list;
        }

        public System_Language()
        {
            Language_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }
    }
}