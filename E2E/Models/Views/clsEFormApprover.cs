using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class clsEFormApprover
    {
        public Guid EForm_Id { get; set; }
        public DateTime Create { get; set; }
        public string EForm_Title { get; set; }
        public string EForm_Description { get; set; }
        public DateTime EForm_Start { get; set; }
        public DateTime EForm_End { get; set; }
        public string ApproverName { get; set; }
        public string UserName { get; set; }
    }
}