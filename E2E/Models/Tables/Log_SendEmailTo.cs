using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class Log_SendEmailTo
    {
        public Log_SendEmailTo()
        {
            SendEmailTo_Id = Guid.NewGuid();
        }

        public Guid SendEmail_Id { get; set; }

        [Key]
        public Guid SendEmailTo_Id { get; set; }

        public string SendEmailTo_Type { get; set; }

        [Index]
        public Guid User_Id { get; set; }
    }
}
