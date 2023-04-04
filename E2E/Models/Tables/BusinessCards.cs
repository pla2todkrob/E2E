using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class BusinessCards
    {

        public BusinessCards()
        {
            BusinessCard_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        [Key]
        public Guid BusinessCard_Id { get; set; }
        public Guid User_id { get; set; }
        public long Key { get; set; }
        public int Amount { get; set; }
        public string Tel_Internal { get; set; }
        public string Tel_External { get; set; }
        public Guid? UserRef_id { get; set; }
        public bool BothSided { get; set; }
        public DateTime Create { get; set; }
        public int Status_Id { get; set; }
        public virtual System_Statuses System_Statuses{get;set;}
        public Guid? UserAction { get; set; }

    }
}