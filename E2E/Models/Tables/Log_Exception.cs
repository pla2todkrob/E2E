using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class Log_Exception
    {
        public string ActionName { get; set; }

        public string ControllerName { get; set; }

        public string ExceptionMessage { get; set; }

        [Key]
        public int Id { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
