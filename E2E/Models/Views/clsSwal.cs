using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class clsSwal
    {
        public clsSwal()
        {
            title = "Error";
            icon = "error";
            button = "OK";
            dangerMode = true;
        }

        public string button { get; set; }
        public bool dangerMode { get; set; }
        public string icon { get; set; }
        public dynamic option { get; set; }
        public string text { get; set; }
        public string title { get; set; }
    }
}