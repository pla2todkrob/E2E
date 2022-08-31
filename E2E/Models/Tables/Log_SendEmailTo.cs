using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
        public Guid User_Id { get; set; }
    }
}