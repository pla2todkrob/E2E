using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class Log_DbDelete
    {
        public Log_DbDelete()
        {
            Id = Guid.NewGuid();
            DeleteTime = DateTime.Now;
        }

        public DateTime DeleteTime { get; set; }

        [Key]
        public Guid Id { get; set; }

        [Index, StringLength(100)]
        public string IP_Address { get; set; }

        [Index, StringLength(100)]
        public string KeyValues { get; set; }

        [Index, StringLength(100)]
        public string TableName { get; set; }

        [Index]
        public Guid? User_Id { get; set; }

        public virtual Users Users { get; set; }
    }
}
