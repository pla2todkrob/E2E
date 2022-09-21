﻿using E2E.Models.Tables;
using System.Collections.Generic;

namespace E2E.Models.Views
{
    public class clsInquiryTopics
    {
        public clsInquiryTopics()
        {
            List_SatisfactionDetails = new List<SatisfactionDetails>();
            List_Master_InquiryTopics = new List<Master_InquiryTopics>();
        }

        public List<Master_InquiryTopics> List_Master_InquiryTopics { get; set; }
        public List<SatisfactionDetails> List_SatisfactionDetails { get; set; }
        public SatisfactionDetails SatisfactionDetails { get; set; }
        public Satisfactions Satisfactions { get; set; }
        public Services Services { get; set; }
    }
}
