using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class Log_DbChange
    {
        public Log_DbChange()
        {
            Id = Guid.NewGuid();
            ChangeTime = DateTime.Now;
        }

        public DateTime ChangeTime { get; set; }

        public string ColumnName { get; set; }

        public string CurrentValue { get; set; }

        [Key]
        public Guid Id { get; set; }

        [Index, StringLength(100)]
        public string IP_Address { get; set; }

        public string OriginalValue { get; set; }

        [Index, StringLength(100)]
        public string TableName { get; set; }

        [Index]
        public Guid? User_Id { get; set; }

        public virtual Users Users { get; set; }
    }
}
