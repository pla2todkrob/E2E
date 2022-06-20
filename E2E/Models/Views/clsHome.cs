using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class clsHome
    {
        public List<Topics> TopicAnnounce { get; set; }
        public List<Topics> TopicWeek { get; set; }
        public List<EForms> EForms { get; set; }

        public clsHome()
        {
            TopicAnnounce = new List<Topics>();
            TopicWeek = new List<Topics>();
            EForms = new List<EForms>();
        }
    }
}