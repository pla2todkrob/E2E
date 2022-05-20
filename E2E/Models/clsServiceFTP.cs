using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace E2E.Models
{
    public class clsServiceFTP
    {
        private string url = ConfigurationManager.AppSettings["FTP_Url"];
        private string user = ConfigurationManager.AppSettings["FTP_User"];
        private string pass = ConfigurationManager.AppSettings["FTP_Password"];
        private string dir = ConfigurationManager.AppSettings["FTP_Dir"];
        private string urlDomain = ConfigurationManager.AppSettings["UrlDomain"];

        public string Ftp_UploadFileToString(string fullDir, HttpPostedFileBase filePost, string fileName = "")
        {
            try
            {
                string res = string.Empty;
                string finalPath = GetFinallyPath(string.Concat(dir, fullDir));
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = filePost.FileName;
                }
                else
                {
                    fileName += Path.GetExtension(filePost.FileName);
                }

                fileName = string.Concat(finalPath, fileName);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(fileName));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(user, pass);

                byte[] bytes = null;
                using (Stream fileStream = filePost.InputStream)
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        fileStream.CopyTo(memory);
                        bytes = memory.ToArray();
                    }
                }

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                    if (reqStream.CanWrite)
                    {
                        res = fileName.Replace(url, urlDomain);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string Ftp_UploadFileToString(string fullDir, string fileBase64, string fileName = "")
        {
            try
            {
                string extension = GetFileTypeBase64(fileBase64);
                byte[] fileByte = Convert.FromBase64String(fileBase64);

                return Ftp_UploadFileToString(fullDir, fileByte, extension, fileName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string Ftp_UploadFileToString(string fullDir, byte[] fileByte, string extension, string fileName = "")
        {
            try
            {
                return "";
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string Ftp_UploadImgThumbnail(string fullDir, HttpPostedFileBase filePost, string fileName = "")
        {
            try
            {
                string res = string.Empty;
                string finalPath = GetFinallyPath(string.Concat(dir, fullDir));
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = filePost.FileName;
                }
                else
                {
                    fileName += Path.GetExtension(filePost.FileName);
                }

                fileName = fileName.Replace(Path.GetExtension(fileName), string.Concat("_thumbnail", Path.GetExtension(fileName)));

                fileName = string.Concat(finalPath, fileName);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(fileName));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(user, pass);

                byte[] bytes = null;

                Image source = Image.FromStream(filePost.InputStream);
                double widthRatio = 192 / source.Width;
                double heightRatio = 108 / source.Height;
                double ratio = (widthRatio < heightRatio) ? widthRatio : heightRatio;
                Image thumbnail = source.GetThumbnailImage((int)(source.Width * ratio), (int)(source.Height * ratio), null, IntPtr.Zero);

                using (var memory = new MemoryStream())
                {
                    thumbnail.Save(memory, source.RawFormat);
                    bytes = memory.ToArray();
                }

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                    if (reqStream.CanWrite)
                    {
                        res = fileName.Replace(url, urlDomain);
                    }
                }

                return res;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string Ftp_UploadImgThumbnailFix(string fullDir, HttpPostedFileBase filePost, string fileName = "")
        {
            try
            {
                string res = string.Empty;
                string finalPath = GetFinallyPath(string.Concat(dir, fullDir));
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = filePost.FileName;
                }
                else
                {
                    fileName += Path.GetExtension(filePost.FileName);
                }

                fileName = fileName.Replace(Path.GetExtension(fileName), string.Concat("_thumbnail", Path.GetExtension(fileName)));

                fileName = string.Concat(finalPath, fileName);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(fileName));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(user, pass);

                byte[] bytes = null;

                Image source = Image.FromStream(filePost.InputStream);
                Image thumbnail = source.GetThumbnailImage(192,108, null, IntPtr.Zero);

                using (var memory = new MemoryStream())
                {
                    thumbnail.Save(memory, source.RawFormat);
                    bytes = memory.ToArray();
                }

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                    if (reqStream.CanWrite)
                    {
                        res = fileName.Replace(url, urlDomain);
                    }
                }

                return res;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool Ftp_DeleteFile(string path)
        {
            try
            {
                bool res = new bool();
                path = path.Replace(urlDomain, url);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(path));
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(user, pass);
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == FtpStatusCode.FileActionOK)
                    {
                        res = true;
                    }
                }
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private string GetFileTypeBase64(string base64)
        {
            try
            {
                var data = base64.Substring(0, 5);

                switch (data.ToUpper())
                {
                    case "IVBOR":
                        return "png";

                    case "/9J/4":
                        return "jpg";

                    case "AAAAF":
                        return "mp4";

                    case "JVBER":
                        return "pdf";

                    case "AAABA":
                        return "ico";

                    case "UMFYI":
                        return "rar";

                    case "E1XYD":
                        return "rtf";

                    case "U1PKC":
                        return "txt";

                    case "MQOWM":
                    case "77U/M":
                        return "srt";

                    default:
                        return string.Empty;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetFinallyPath(string fullDir)
        {
            string res = string.Empty;
        StartMethod:
            string path = url;
            try
            {
                string[] splitDir = fullDir.Trim().Split('/');

                foreach (var item in splitDir)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        path += string.Concat(item, "/");
                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(path));
                        request.Method = WebRequestMethods.Ftp.ListDirectory;
                        request.Credentials = new NetworkCredential(user, pass);

                        using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode == FtpStatusCode.DataAlreadyOpen || response.StatusCode == FtpStatusCode.OpeningData)
                            {
                                res = path;
                            }
                        }
                    }
                }
                return res;
            }
            catch (Exception)
            {
                if (CreateDirectory(path))
                {
                    goto StartMethod;
                }
                else
                {
                    throw;
                }
            }
        }

        private bool CreateDirectory(string path)
        {
            try
            {
                bool res = new bool();
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(path));
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(user, pass);
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == FtpStatusCode.PathnameCreated)
                    {
                        res = true;
                    }
                }
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}