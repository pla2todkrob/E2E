using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class clsInquiryTopics
    {
        public Services Services { get; set; }
        public Satisfactions Satisfactions { get; set; }
        public SatisfactionDetails SatisfactionDetails { get; set; }
        public List<SatisfactionDetails> List_SatisfactionDetails { get; set; }
        public List<Master_InquiryTopics> List_Master_InquiryTopics { get; set; }

        public clsInquiryTopics()
        {
            List_SatisfactionDetails = new List<SatisfactionDetails>();
            List_Master_InquiryTopics = new List<Master_InquiryTopics>();
        }
    }
}