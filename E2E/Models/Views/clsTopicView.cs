using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Views
{
    public class ClsTopicView
    {
        [Display(Name = "View")]
        public int Count { get; set; }

        public string Department { get; set; }
        public DateTime LastTime { get; set; }
        public string Name { get; set; }
        public string UserCode { get; set; }
    }
}
