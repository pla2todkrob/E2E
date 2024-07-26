using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class Master_InquiryTopics
    {
        public Master_InquiryTopics()
        {
            InquiryTopic_Id = Guid.NewGuid();
            Create = DateTime.Now;
        }

        public DateTime Create { get; set; }

        [Display(Name = "EN  Inquiry topic")]
        public string Description_EN { get; set; }

        [Display(Name = "TH Inquiry topic")]
        public string Description_TH { get; set; }

        [Key]
        public Guid InquiryTopic_Id { get; set; }

        [Display(Name = "Index")]
        public int InquiryTopic_Index { get; set; }

        public int? Program_Id { get; set; }
        public virtual System_Program System_Program { get; set; }
        public DateTime? Update { get; set; }
    }
}
