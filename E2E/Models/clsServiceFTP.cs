using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Web;

namespace E2E.Models
{
    public class clsServiceFTP
    {
        private static string saveToPath = string.Empty;
        private string dir = ConfigurationManager.AppSettings["FTP_Dir"];
        private string pass = ConfigurationManager.AppSettings["FTP_Password"];
        private string url = ConfigurationManager.AppSettings["FTP_Url"];
        private string urlDomain = ConfigurationManager.AppSettings["UrlDomain"];
        private string user = ConfigurationManager.AppSettings["FTP_User"];
        private string webPath = AppDomain.CurrentDomain.BaseDirectory;

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

        private string replaceName(string name)
        {
            return name.Replace("!", "_").Replace("@", "_").Replace("#", "_").Replace("$", "_").Replace("%", "_")
                .Replace("^", "_").Replace("&", "_").Replace("'", "_");
        }

        public static string finalPath { get; set; }

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

        public void Ftp_DownloadFile(string pathFile)
        {
            try
            {
                finalPath = GetFinallyPath(string.Concat(dir, pathFile));
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(finalPath));
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(user, pass);
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Stream stream = response.GetResponseStream();
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.Buffer = true;
                        HttpContext.Current.Response.ContentType = MediaTypeNames.Application.Octet;
                        HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", Path.GetFileName(pathFile)));
                        HttpContext.Current.Response.BinaryWrite(memoryStream.ToArray());
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Ftp_DownloadFileToLocal(string fileName, string keyFolder)
        {
            try
            {
                saveToPath = string.Concat(webPath, "Download\\");

                if (!Directory.Exists(saveToPath))
                {
                    Directory.CreateDirectory(saveToPath);
                }

                saveToPath = string.Concat(saveToPath, keyFolder, "\\");

                if (!Directory.Exists(saveToPath))
                {
                    Directory.CreateDirectory(saveToPath);
                }

                string pathFile = string.Concat(finalPath, fileName);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(pathFile));
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(user, pass);
                request.UseBinary = true;
                request.UsePassive = true;

                using (FtpWebResponse responseFile = (FtpWebResponse)request.GetResponse())
                {
                    using (Stream streamFile = responseFile.GetResponseStream())
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            streamFile.CopyTo(memoryStream);
                            string saveToFile = string.Concat(saveToPath, fileName);
                            using (Stream targetStream = File.Create(saveToFile))
                            {
                                byte[] buffer = memoryStream.ToArray();
                                targetStream.Write(buffer, 0, buffer.Length);
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Ftp_DownloadFileToLocal(List<string> filesName, string keyFolder)
        {
            try
            {
                saveToPath = string.Concat(webPath, "Download\\");

                if (!Directory.Exists(saveToPath))
                {
                    Directory.CreateDirectory(saveToPath);
                }

                saveToPath = string.Concat(saveToPath, keyFolder, "\\");

                if (!Directory.Exists(saveToPath))
                {
                    Directory.CreateDirectory(saveToPath);
                }

                foreach (var item in filesName)
                {
                    string pathFile = string.Concat(finalPath, item);
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(pathFile));
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.Credentials = new NetworkCredential(user, pass);
                    request.UseBinary = true;
                    request.UsePassive = true;

                    using (FtpWebResponse responseFile = (FtpWebResponse)request.GetResponse())
                    {
                        using (Stream streamFile = responseFile.GetResponseStream())
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                streamFile.CopyTo(memoryStream);
                                string saveToFile = string.Concat(saveToPath, item);
                                using (Stream targetStream = File.Create(saveToFile))
                                {
                                    byte[] buffer = memoryStream.ToArray();
                                    targetStream.Write(buffer, 0, buffer.Length);
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Ftp_DownloadFolder(string pathFolder, string zipName)
        {
            try
            {
                zipName = zipName.Replace(".zip", "");
                List<string> files = new List<string>();
                finalPath = GetFinallyPath(string.Concat(dir, pathFolder));
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(finalPath));
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(user, pass);
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    StreamReader streamReader = new StreamReader(response.GetResponseStream());
                    string line = streamReader.ReadLine();
                    while (!string.IsNullOrEmpty(line))
                    {
                        files.Add(line);
                        line = streamReader.ReadLine();
                    }
                    streamReader.Close();
                }

                if (Ftp_DownloadFileToLocal(files, zipName))
                {
                    string zipPath = string.Format("{0}{1}.zip", saveToPath, zipName);
                    ZipFile.CreateFromDirectory(saveToPath, zipPath, CompressionLevel.Optimal, true);
                    if (Local_DownloadZip(zipPath))
                    {
                        if (Directory.Exists(saveToPath))
                        {
                            Directory.Delete(saveToPath);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string Ftp_UploadFileToString(string fullDir, HttpPostedFileBase filePost, string fileName = "")
        {
            try
            {
                string res = string.Empty;
                finalPath = GetFinallyPath(string.Concat(dir, fullDir));
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = filePost.FileName;
                }

                fileName = replaceName(fileName);
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
                request.ContentLength = bytes.Length;

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

        public clsImage Ftp_UploadImageFixSizeToString(string fullDir, HttpPostedFileBase filePost, string fileName = "")
        {
            try
            {
                clsImage res = new clsImage();
                finalPath = GetFinallyPath(string.Concat(dir, fullDir));
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = filePost.FileName;
                }
                fileName = replaceName(fileName);
                fileName = string.Concat(finalPath, fileName);

                List<clsImage> clsImages = new List<clsImage>();
                Image originalFile = Image.FromStream(filePost.InputStream, true, true);
                clsImage clsImage = new clsImage();
                clsImage.Image = originalFile;
                clsImage.FtpPath = fileName;
                clsImages.Add(clsImage);

                Image thumbnailFile = originalFile.GetThumbnailImage(192, 108, null, IntPtr.Zero);
                clsImage = new clsImage();
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

        public clsImage Ftp_UploadImageToString(string fullDir, HttpPostedFileBase filePost, string fileName = "")
        {
            try
            {
                clsImage res = new clsImage();
                finalPath = GetFinallyPath(string.Concat(dir, fullDir));
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = filePost.FileName;
                }

                fileName = replaceName(fileName);

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

        public bool Local_DownloadZip(string zipPath)
        {
            try
            {
                bool res = new bool();
                FileInfo fileInfo = new FileInfo(zipPath);
                using (FileStream fileStream = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    byte[] fileBytes = new byte[fileStream.Length];
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.Buffer = true;
                    HttpContext.Current.Response.ContentType = MediaTypeNames.Application.Octet;
                    HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", Path.GetFileName(fileInfo.Name)));
                    HttpContext.Current.Response.BinaryWrite(fileBytes);
                    res = true;
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