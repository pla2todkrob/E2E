using E2E.Models.Tables;
using System.Collections.Generic;

namespace E2E.Models.Views
{
    public class clsServiceComments
    {
        public clsServiceComments()
        {
            ServiceCommentFiles = new List<ServiceCommentFiles>();
        }

        public List<ServiceCommentFiles> ServiceCommentFiles { get; set; }
        public ServiceComments ServiceComments { get; set; }
        public string User_Name { get; set; }
    }
}