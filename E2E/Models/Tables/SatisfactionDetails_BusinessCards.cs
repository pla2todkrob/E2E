using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class SatisfactionDetails_BusinessCards
    {
        public SatisfactionDetails_BusinessCards()
        {
            SatisfactionDetails_BusinessCards_id = Guid.NewGuid();
        }

        [Index]
        public Guid InquiryTopic_Id { get; set; }

        public virtual Master_InquiryTopics Master_InquiryTopics { get; set; }

        public int Point { get; set; }

        public Guid Satisfactions_BusinessCard_id { get; set; }

        [Key]
        public Guid SatisfactionDetails_BusinessCards_id { get; set; }

        public virtual Satisfactions_BusinessCards Satisfactions_BusinessCards { get; set; }
    }
}