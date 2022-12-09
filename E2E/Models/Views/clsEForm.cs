using E2E.Models.Tables;
using System.Collections.Generic;

namespace E2E.Models.Views
{
    public class ClsEForm
    {
        public ClsEForm()
        {
            EForm_Files = new List<EForm_Files>();
            EForm_Galleries = new List<EForm_Galleries>();
        }

        public List<EForm_Files> EForm_Files { get; set; }
        public List<EForm_Galleries> EForm_Galleries { get; set; }
        public EForms EForms { get; set; }
    }
}
