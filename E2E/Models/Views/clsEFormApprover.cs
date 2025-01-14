﻿using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Views
{
    public class ClsEFormApprover
    {
        public string ApproverName { get; set; }
        public DateTime Create { get; set; }
        public string EForm_Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime EForm_End { get; set; }

        public Guid EForm_Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime EForm_Start { get; set; }

        public string EForm_Title { get; set; }
        public string UserName { get; set; }
    }
}
