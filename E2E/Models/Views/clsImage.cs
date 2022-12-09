using System.Drawing;

namespace E2E.Models.Views
{
    public class ClsImage
    {
        public string FtpPath { get; set; }
        public Image Image { get; set; }
        public string OriginalPath { get; set; }
        public string ThumbnailPath { get; set; }
    }
}
