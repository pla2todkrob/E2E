using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class System_Language
    {
        public System_Language()
        {
            Language_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        public DateTime Create { get; set; }

        [Key]
        public Guid Language_Id { get; set; }

        [Display(Name = "Language")]
        public string Language_Name { get; set; }

        public static List<System_Language> DefaultList()
        {
            List<System_Language> list = new List<System_Language>
            {
                new System_Language() { Language_Name = "TH", Create = DateTime.Now },
                new System_Language() { Language_Name = "EN", Create = DateTime.Now }
            };
            return list;
        }
    }
}
