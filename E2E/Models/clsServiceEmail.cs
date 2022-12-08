using System.Collections.Generic;

namespace E2E.Models
{
    public class ClsFile
    {
        public string Base64 { get; set; }
        public string FileName { get; set; }
    }

    public class ClsServiceEmail
    {
        public string Body { get; set; }
        public List<ClsFile> ClsFiles { get; set; }
        public string[] FilePath { get; set; }
        public string[] SendBCC { get; set; }
        public string[] SendCC { get; set; }
        public string SendFrom { get; set; }
        public string[] SendTo { get; set; }
        public string Subject { get; set; }
    }
}
