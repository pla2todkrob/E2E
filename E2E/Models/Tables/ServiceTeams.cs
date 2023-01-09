using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class ServiceTeams
    {
        public ServiceTeams()
        {
            Team_Id = Guid.NewGuid();
        }

        [Index]
        public Guid Service_Id { get; set; }

        public virtual Services Services { get; set; }

        [Key]
        public Guid Team_Id { get; set; }

        [Index]
        public Guid User_Id { get; set; }
    }
}
