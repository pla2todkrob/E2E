using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class SatisfactionDetails
    {
        public SatisfactionDetails()
        {
            SatisfactionDetail_Id = Guid.NewGuid();
        }

        public Guid InquiryTopic_Id { get; set; }

        public virtual Master_InquiryTopics Master_InquiryTopics { get; set; }

        public int Point { get; set; }

        public Guid Satisfaction_Id { get; set; }

        [Key]
        public Guid SatisfactionDetail_Id { get; set; }

        public virtual Satisfactions Satisfactions { get; set; }
    }
}