using E2E.Models.Tables;
using System.Collections.Generic;

namespace E2E.Models.Views
{
    public class clsHome
    {
        public clsHome()
        {
            Topics = new List<Topics>();
            EForms = new List<EForms>();
        }

        public List<EForms> EForms { get; set; }
        public List<Topics> Topics { get; set; }
    }
}