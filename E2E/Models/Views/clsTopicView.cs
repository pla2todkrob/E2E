using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class clsTopicView
    {
        public string Name { get; set; }
        public string UserCode { get; set; }
        public string Department { get; set; }
        [Display(Name = "View")]
        public int Count { get; set; }
        public DateTime LastTime { get; set; }

    }
}