using E2E.Models.Tables;
using System.Collections.Generic;

namespace E2E.Models.Views
{
    public class ClsDocuments
    {
        public ClsDocuments()
        {
            Master_Documents = new Master_Documents();
            Master_DocumentVersions = new List<Master_DocumentVersions>();
        }

        public Master_Documents Master_Documents { get; set; }
        public List<Master_DocumentVersions> Master_DocumentVersions { get; set; }
    }
}
