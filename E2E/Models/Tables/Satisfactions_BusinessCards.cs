using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class Satisfactions_BusinessCards
    {
        public Satisfactions_BusinessCards()
        {
            Satisfactions_BusinessCard_id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        [Index]
        public Guid BusinessCard_Id { get; set; }

        public virtual BusinessCards BusinessCards { get; set; }
        public DateTime Create { get; set; }

        [Display(Name = "Average")]
        public double Satisfaction_Average { get; set; }

        [Key]
        public Guid Satisfactions_BusinessCard_id { get; set; }

        public bool Unsatisfied { get; set; }
    }
}
