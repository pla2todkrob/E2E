using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class Master_InquiryTopics
    {
        [Key]
        public Guid InquiryTopic_Id { get; set; }
        [Display(Name ="Index")]
        public int InquiryTopic_Index { get; set; }
        [Display(Name = "TH Inquiry topic")]
        public string Description_TH { get; set; }
        [Display(Name = "EN  Inquiry topic")]
        public string Description_EN { get; set; }
        public DateTime Create { get; set; }
        public DateTime? Update { get; set; }
        public Master_InquiryTopics()
        {
            InquiryTopic_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }
    }
}