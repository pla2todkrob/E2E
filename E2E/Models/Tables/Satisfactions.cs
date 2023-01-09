using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class Satisfactions
    {
        public Satisfactions()
        {
            Satisfaction_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        public DateTime Create { get; set; }

        [Display(Name = "Average")]
        public double Satisfaction_Average { get; set; }

        [Key]
        public Guid Satisfaction_Id { get; set; }

        [Index]
        public Guid Service_Id { get; set; }

        public virtual Services Services { get; set; }
        public bool Unsatisfied { get; set; }
    }
}
