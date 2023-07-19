using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class BusinessCards
    {
        public BusinessCards()
        {
            BusinessCard_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        public int Amount { get; set; }

        public bool BothSided { get; set; }

        [Key]
        public Guid BusinessCard_Id { get; set; }

        public DateTime Create { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? DueDate { get; set; }

        public long Key { get; set; }
        public int Status_Id { get; set; }
        public virtual System_Statuses System_Statuses { get; set; }
        public virtual System_Priorities System_Priorities { get; set; }
        public string Tel_External { get; set; }
        public string Tel_Internal { get; set; }
        public DateTime? Update { get; set; }
        public Guid User_id { get; set; }
        public Guid? UserAction { get; set; }
        public Guid? UserRef_id { get; set; }
        [Index]
        public bool Is_OverDue { get; set; }
    }
}
