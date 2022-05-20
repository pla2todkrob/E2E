using E2E.Models.Views;
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

        public clsImage Ftp_UploadImageToString(string fullDir, HttpPostedFileBase filePost, string fileName = "")
        {
            try
            {

                clsImage res = new clsImage();
                string finalPath = GetFinallyPath(string.Concat(dir, fullDir));
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = filePost.FileName;
                }

                fileName = string.Concat(finalPath, fileName);

                List<clsImage> clsImages = new List<clsImage>();
                Image originalFile = Image.FromStream(filePost.InputStream, true, true);
                clsImage clsImage = new clsImage();
                clsImage.Image = originalFile;
                clsImage.FtpPath = fileName;
                clsImages.Add(clsImage);

                double widthRatio = 192 / originalFile.Width;
                double heightRatio = 108 / originalFile.Height;
                double ratio = (widthRatio < heightRatio) ? widthRatio : heightRatio;
                Image thumbnailFile = originalFile.GetThumbnailImage((int)(originalFile.Width * ratio), (int)(originalFile.Height * ratio), null, IntPtr.Zero);
                clsImage.Image = thumbnailFile;
                clsImage.FtpPath = fileName.Replace(Path.GetExtension(fileName), string.Concat("_thumbnail", Path.GetExtension(fileName)));
                clsImages.Add(clsImage);

                foreach (var item in clsImages)
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(item.FtpPath));
                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    request.Credentials = new NetworkCredential(user, pass);
                    byte[] bytes = null;

                    using (var memory = new MemoryStream())
                    {
                        item.Image.Save(memory, originalFile.RawFormat);
                        bytes = memory.ToArray();
                    }

                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(bytes, 0, bytes.Length);
                        if (reqStream.CanWrite)
                        {
                            if (string.IsNullOrEmpty(res.OriginalPath))
                            {
                                res.OriginalPath = item.FtpPath.Replace(url, urlDomain);
                            }
                            else
                            {
                                res.ThumbnailPath = item.FtpPath.Replace(url, urlDomain);
                            }
                        }
                    }
                }
                return res;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public clsImage Ftp_UploadImageFixSizeToString(string fullDir, HttpPostedFileBase filePost, string fileName = "")
        {
            try
            {

                clsImage res = new clsImage();
                string finalPath = GetFinallyPath(string.Concat(dir, fullDir));
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = filePost.FileName;
                }

                fileName = string.Concat(finalPath, fileName);

                List<clsImage> clsImages = new List<clsImage>();
                Image originalFile = Image.FromStream(filePost.InputStream,true,true);
                clsImage clsImage = new clsImage();
                clsImage.Image = originalFile;
                clsImage.FtpPath = fileName;
                clsImages.Add(clsImage);

                Image thumbnailFile = originalFile.GetThumbnailImage(192, 108, null, IntPtr.Zero);
                clsImage.Image = thumbnailFile;
                clsImage.FtpPath = fileName.Replace(Path.GetExtension(fileName), string.Concat("_thumbnail", Path.GetExtension(fileName)));
                clsImages.Add(clsImage);

                foreach (var item in clsImages)
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(item.FtpPath));
                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    request.Credentials = new NetworkCredential(user, pass);
                    byte[] bytes = null;

                    using (var memory = new MemoryStream())
                    {
                        item.Image.Save(memory, originalFile.RawFormat);
                        bytes = memory.ToArray();
                    }

                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(bytes, 0, bytes.Length);
                        if (reqStream.CanWrite)
                        {
                            if (string.IsNullOrEmpty(res.OriginalPath))
                            {
                                res.OriginalPath = item.FtpPath.Replace(url, urlDomain);
                            }
                            else
                            {
                                res.ThumbnailPath = item.FtpPath.Replace(url, urlDomain);
                            }
                        }
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