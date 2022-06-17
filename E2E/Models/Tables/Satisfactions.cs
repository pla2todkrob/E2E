using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class Satisfactions
    {
        [Key]
        public Guid Satisfaction_Id { get; set; }
        public Guid Service_Id { get; set; }
        public virtual Services Services { get; set; }
        public decimal Satisfaction_Average { get; set; }
        public DateTime Create { get; set; }
        public Satisfactions()
        {
            Satisfaction_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }
    }
}