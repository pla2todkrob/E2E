using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class Log_Login
    {
        public Log_Login()
        {
            Log_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        public DateTime Create { get; set; }

        [Key]
        public Guid Log_Id { get; set; }

        [Index]
        public Guid User_Id { get; set; }
    }
}
