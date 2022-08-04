using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models
{
    public class clsApi
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public dynamic Value { get; set; }
        public clsApi()
        {
            isSuccess = new bool();
        }
    }

    public class responseUser
    {
        public Users Users { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}