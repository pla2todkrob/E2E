using System.Drawing;

namespace E2E.Models.Views
{
    public class clsImage
    {
        public string FtpPath { get; set; }
        public Image Image { get; set; }
        public string OriginalPath { get; set; }
        public string ThumbnailPath { get; set; }
    }
}