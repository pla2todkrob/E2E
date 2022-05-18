using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class clsEForm
    {
        public EForms EForms { get; set; }
        public List<EForm_Files> EForm_Files { get; set; }
        public List<EForm_Galleries> EForm_Galleries { get; set; }
        public clsEForm()
        {
            EForm_Files = new List<EForm_Files>();
            EForm_Galleries = new List<EForm_Galleries>();
        }
    }
}