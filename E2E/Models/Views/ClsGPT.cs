using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class ClsGPT
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public int Tokens { get; set; }
        public int Amount { get; set; }

    }

    public class ClsGPT_Sum
    {
        public List<ClsGPT> ClsGPTs { get; set; }
        public int TotalAmount { get; set; }
        public int TotalToken { get; set; }
        public ClsGPT_Sum() {
            ClsGPTs = new List<ClsGPT>();
        }

    }
}