using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class System_Roles
    {
        [Key]
        public Guid Role_Id { get; set; }

        public int Role_Index { get; set; }
        public string Role_Name { get; set; }

        public System_Roles()
        {
            Role_Id = Guid.NewGuid();
        }

        public List<System_Roles> DefaultList()
        {
            List<System_Roles> list = new List<System_Roles>();
            list.Add(new System_Roles() { Role_Index = 1, Role_Name = "Admin" });
            list.Add(new System_Roles() { Role_Index = 2, Role_Name = "User" });

            return list;
        }
    }
}