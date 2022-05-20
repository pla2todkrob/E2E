using System.Drawing;

namespace E2E.Models.Views
{
    public class clsImage
    {
        public Image Image { get; set; }
        public string FtpPath { get; set; }
        public string OriginalPath { get; set; }
        public string ThumbnailPath { get; set; }
    }
}