using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models
{
    public class clsServiceFile
    {
        public string folderPath { get; set; }
        public string Base64 { get; set; }
        public string filename { get; set; }
    }

    public class ReturnUpload
    {
        public string fileUrl { get; set; }
        public string fileThumbnailUrl { get; set; }
        public bool canUpload { get; set; }
        public string errorMessage { get; set; }
    }

    public class ReturnSend
    {
        public bool canSend { get; set; }
        public string errorMessage { get; set; }
    }

    public class ReturnDelete
    {
        public bool canDelete { get; set; }
        public string errorMessage { get; set; }
    }
}