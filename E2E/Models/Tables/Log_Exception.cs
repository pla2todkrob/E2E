using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class Log_Exception
    {
        public string ActionName { get; set; }

        public string ControllerName { get; set; }

        public string ExceptionMessage { get; set; }
        public string ExceptionStackTrace { get; set; }
        public string ExceptionType { get; set; }

        [Key]
        public int Id { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
