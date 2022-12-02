﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class System_Roles
    {
        [Key]
        public int Role_Id { get; set; }

        public string Role_Name { get; set; }

        public static List<System_Roles> DefaultList()
        {
            List<System_Roles> list = new List<System_Roles>
            {
                new System_Roles() { Role_Name = "Admin" },
                new System_Roles() { Role_Name = "User" }
            };

            return list;
        }
    }
}
