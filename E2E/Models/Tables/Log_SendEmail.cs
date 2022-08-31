using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class Log_SendEmail
    {
        public Log_SendEmail()
        {
            SendEmail_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        public DateTime Create { get; set; }

        public string SendEmail_ClassName { get; set; }

        public string SendEmail_Content { get; set; }

        [Key]
        public Guid SendEmail_Id { get; set; }

        public string SendEmail_MethodName { get; set; }
        public Guid SendEmail_Ref_Id { get; set; }
        public string SendEmail_Subject { get; set; }
        public Guid User_Id { get; set; }
    }
}