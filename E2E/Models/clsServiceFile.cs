namespace E2E.Models
{
    public class ClsServiceFile
    {
        public string Base64 { get; set; }
        public string Filename { get; set; }
        public string FolderPath { get; set; }
    }

    public class ReturnDelete
    {
        public bool CanDelete { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ReturnSend
    {
        public bool CanSend { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ReturnUpload
    {
        public bool CanUpload { get; set; }
        public string ErrorMessage { get; set; }
        public string FileThumbnailUrl { get; set; }
        public string FileUrl { get; set; }
    }
}
