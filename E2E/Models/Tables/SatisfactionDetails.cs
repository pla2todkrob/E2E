using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class SatisfactionDetails
    {
        public SatisfactionDetails()
        {
            SatisfactionDetail_Id = Guid.NewGuid();
        }

        [Index]
        public Guid InquiryTopic_Id { get; set; }

        public virtual Master_InquiryTopics Master_InquiryTopics { get; set; }

        public int Point { get; set; }

        public Guid Satisfaction_Id { get; set; }

        [Key]
        public Guid SatisfactionDetail_Id { get; set; }

        public virtual Satisfactions Satisfactions { get; set; }
    }
}
