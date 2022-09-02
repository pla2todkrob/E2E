using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class clsDocuments
    {
        public Master_Documents Master_Documents { get; set; }
        public List<Master_DocumentVersions> Master_DocumentVersions { get; set; }

        public clsDocuments()
        {
            Master_Documents = new Master_Documents();
            Master_DocumentVersions = new List<Master_DocumentVersions>();
        }
    }
}