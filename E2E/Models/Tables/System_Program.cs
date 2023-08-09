using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class System_Program
    {

        [Key]
        public int Program_Id { get; set; }

        [Description("Program name"), Display(Name = "Program"), Required, StringLength(100), Index(IsUnique = true)]
        public string Program_Name { get; set; }

        public static List<System_Program> DefaultList()
        {
            List<System_Program> list = new List<System_Program>
            {
                new System_Program() { Program_Name = "Service" },
                new System_Program() { Program_Name = "Business Card"}
            };
            return list;
        }

    }
}