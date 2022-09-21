using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class ServiceTeams
    {
        public ServiceTeams()
        {
            Team_Id = Guid.NewGuid();
        }

        public Guid Service_Id { get; set; }

        public virtual Services Services { get; set; }

        [Key]
        public Guid Team_Id { get; set; }

        public Guid User_Id { get; set; }
    }
}
