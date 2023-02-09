using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class ClsServiceUserActionName
    {
        public ClsServiceUserActionName()
        {
            UserDetails = new List<UserDetails>();
            serviceChangeDueDates = new List<ServiceChangeDueDate>();
        }

        public List<Services> services { get; set; }
        public List<ServiceChangeDueDate> serviceChangeDueDates { get; set; }
        public List<UserDetails> UserDetails { get; set; }
        public Guid UserId { get; set; }

    }
}