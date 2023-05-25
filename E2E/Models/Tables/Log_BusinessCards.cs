using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class Log_BusinessCards
    {
        public Log_BusinessCards()
        {
            Log_BusinessCard_Id = Guid.NewGuid();
        }

        public virtual Guid BusinessCard_Id { get; set; }

        public DateTime Create { get; set; }

        [Key]
        public Guid Log_BusinessCard_Id { get; set; }

        public string Remark { get; set; }
        public virtual int Status_Id { get; set; }
        public bool Undo { get; set; }
        public Guid? User_Id { get; set; }
    }
}
