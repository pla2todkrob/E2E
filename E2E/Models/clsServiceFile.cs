using System.IO;
using System.Web;

namespace E2E.Models
{
    public class ClsServiceFile
    {
        public string Base64 { get; set; }
        public string Filename { get; set; }
        public string FolderPath { get; set; }
    }

    public class FileResponse
    {
        public string ErrorMessage { get; set; }
        public string FileThumbnailUrl { get; set; }
        public string FileUrl { get; set; }
        public string FilePath { get; set; }
        public string FileThumbnailPath { get; set; }
        public bool IsSuccess { get; set; }
        public byte[] FileBytes { get; set; }
    }

    public class FileStreamPostedFile : HttpPostedFileBase
    {
        private readonly Stream _stream;
        private readonly string _contentType;
        private readonly string _fileName;
        private readonly int _contentLength;

        public FileStreamPostedFile(Stream stream, string fileName, string contentType)
        {
            _stream = stream;
            _fileName = fileName;
            _contentType = contentType;
            _contentLength = (int)stream.Length;
        }

        public override int ContentLength => _contentLength;
        public override string ContentType => _contentType;
        public override string FileName => _fileName;
        public override Stream InputStream => _stream;

        public override void SaveAs(string filename)
        {
            using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                _stream.CopyTo(fileStream);
            }
        }
    }

}
