using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class clsServiceComments
    {
        public ServiceComments ServiceComments { get; set; }
        public string UserInfomation { get; set; }
        public List<ServiceCommentFiles> ServiceCommentFiles { get; set; }

        public clsServiceComments()
        {
            ServiceCommentFiles = new List<ServiceCommentFiles>();
        }
    }
}