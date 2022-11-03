using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models
{
    public class clsServiceEmail
    {
        public string sendFrom { get; set; }
        public string[] sendTo { get; set; }
        public string[] sendCC { get; set; }
        public string[] sendBCC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string[] filePath { get; set; }
        public List<clsFile> clsFiles { get; set; }
    }

    public class clsFile
    {
        public string Base64 { get; set; }
        public string FileName { get; set; }
    }
}