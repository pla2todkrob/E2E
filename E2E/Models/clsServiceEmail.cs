using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace E2E.Models
{
    public class ClsFileAttach
    {
        public string Base64 { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

    public class ClsServiceEmail
    {
        public ClsServiceEmail()
        {
            ClsFileAttaches = new List<ClsFileAttach>();
        }

        public string Body { get; set; }
        public List<ClsFileAttach> ClsFileAttaches { get; set; }
        public string[] SendBCC { get; set; }
        public string[] SendCC { get; set; }
        public string SendFrom { get; set; }
        public string[] SendTo { get; set; }
        public string Subject { get; set; }
    }

    public class MailResponse
    {
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
    }
}
