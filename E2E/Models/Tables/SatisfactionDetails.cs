using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class SatisfactionDetails
    {
        [Key]
        public Guid SatisfactionDetail_Id { get; set; }
        public Guid Satisfaction_Id { get; set; }
        public virtual Satisfactions Satisfactions { get; set; }
        public Guid InquiryTopic_Id { get; set; }
        public virtual Master_InquiryTopics Master_InquiryTopics { get; set; }
        public int Point { get; set; }
        public SatisfactionDetails()
        {
            SatisfactionDetail_Id = Guid.NewGuid();
        }
    }
}