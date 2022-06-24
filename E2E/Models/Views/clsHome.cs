using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class clsHome
    {
        public List<Topics> Topics { get; set; }
        public List<EForms> EForms { get; set; }

        public clsHome()
        {
            Topics = new List<Topics>();
            EForms = new List<EForms>();
        }
    }
}