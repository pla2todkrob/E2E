namespace E2E.Models
{
    public class clsServiceFile
    {
        public string Base64 { get; set; }
        public string filename { get; set; }
        public string folderPath { get; set; }
    }

    public class ReturnDelete
    {
        public bool canDelete { get; set; }
        public string errorMessage { get; set; }
    }

    public class ReturnSend
    {
        public bool canSend { get; set; }
        public string errorMessage { get; set; }
    }

    public class ReturnUpload
    {
        public bool canUpload { get; set; }
        public string errorMessage { get; set; }
        public string fileThumbnailUrl { get; set; }
        public string fileUrl { get; set; }
    }
}
