using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class ServiceTeams
    {
        [Key]
        public Guid Team_Id { get; set; }
        public Guid Service_Id { get; set; }
        public virtual Services Services { get; set; }
        public Guid User_Id { get; set; }
        public ServiceTeams()
        {
            Team_Id = Guid.NewGuid();
        }
    }
}