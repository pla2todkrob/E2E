using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class clsServiceTeams
    {
        public Guid Service_Id { get; set; }
        public Guid[] User_Ids { get; set; }
        public ServiceTeams ServiceTeams { get; set; }
        public string User_Name { get; set; }
    }
}