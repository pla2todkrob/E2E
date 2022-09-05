using E2E.Models.Tables;
using System;

namespace E2E.Models.Views
{
    public class clsServiceTeams
    {
        public Guid Service_Id { get; set; }
        public ServiceTeams ServiceTeams { get; set; }
        public Guid[] User_Ids { get; set; }
        public string User_Name { get; set; }
    }
}