﻿using E2E.Models.Views;
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
        private clsMail mail = new clsMail();
        private string pass = ConfigurationManager.AppSettings["FTP_Password"];
        private string urlDomain = ConfigurationManager.AppSettings["Domain_Url"];
        private string urlFtp = ConfigurationManager.AppSettings["FTP_Url"];
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
            string path = urlFtp;
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
                path = path.Replace(urlDomain, urlFtp);
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
                pathFile = pathFile.Replace(urlDomain, urlFtp);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(pathFile));
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

                List<string> subFolder = new List<string>();
                subFolder = keyFolder.Split('\\').ToList();
                foreach (var item in subFolder)
                {
                    saveToPath = string.Concat(saveToPath, item, "\\");

                    if (!Directory.Exists(saveToPath))
                    {
                        Directory.CreateDirectory(saveToPath);
                    }
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
                        request = null;
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

        public void Ftp_DownloadFolder(string ftpFileDir, string zirDir)
        {
            try
            {
                zirDir = zirDir.Replace(".zip", "");
                List<string> files = new List<string>();
                finalPath = GetFinallyPath(string.Concat(dir, ftpFileDir));
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(finalPath));
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(user, pass);
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    request = null;
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        string line = streamReader.ReadLine();
                        while (!string.IsNullOrEmpty(line))
                        {
                            files.Add(line);
                            line = streamReader.ReadLine();
                        }
                    }
                }

                if (Ftp_DownloadFileToLocal(files, zirDir))
                {
                    string zipPath = saveToPath.TrimEnd('\\');
                    zipPath = string.Concat(zipPath, ".zip");
                    if (File.Exists(zipPath))
                    {
                        File.Delete(zipPath);
                    }
                    ZipFile.CreateFromDirectory(saveToPath, zipPath, CompressionLevel.Optimal, true);
                    if (Local_DownloadZip(zipPath))
                    {
                        Directory.Delete(saveToPath, true);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Ftp_DownloadFolder(List<string> pathsFolder, string zirDir)
        {
            try
            {
                zirDir = zirDir.Replace(".zip", "");
                foreach (var item in pathsFolder)
                {
                    List<string> files = new List<string>();
                    finalPath = GetFinallyPath(string.Concat(dir, item));
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(finalPath));
                    request.Method = WebRequestMethods.Ftp.ListDirectory;
                    request.Credentials = new NetworkCredential(user, pass);
                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        request = null;
                        using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            string line = streamReader.ReadLine();
                            while (!string.IsNullOrEmpty(line))
                            {
                                files.Add(line);
                                line = streamReader.ReadLine();
                            }
                        }
                    }

                    if (Ftp_DownloadFileToLocal(files, string.Format("{0}\\{1}", zirDir, item.Split('/').ElementAt(1))))
                    {
                        continue;
                    }
                }
                saveToPath = saveToPath.TrimEnd('\\');
                saveToPath = saveToPath.Replace(saveToPath.Split('\\').LastOrDefault(), "");
                string zipPath = saveToPath.TrimEnd('\\');
                zipPath = string.Concat(zipPath, ".zip");
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }
                ZipFile.CreateFromDirectory(saveToPath, zipPath, CompressionLevel.Optimal, true);
                if (Local_DownloadZip(zipPath))
                {
                    Directory.Delete(saveToPath, true);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Ftp_DownloadFolder(List<string> pathsFolder, string zirDir, List<string> emails, string subject, string content)
        {
            try
            {
                zirDir = zirDir.Replace(".zip", "");
                foreach (var item in pathsFolder)
                {
                    List<string> files = new List<string>();
                    finalPath = GetFinallyPath(string.Concat(dir, item));
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(finalPath));
                    request.Method = WebRequestMethods.Ftp.ListDirectory;
                    request.Credentials = new NetworkCredential(user, pass);
                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        request = null;
                        using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            string line = streamReader.ReadLine();
                            while (!string.IsNullOrEmpty(line))
                            {
                                files.Add(line);
                                line = streamReader.ReadLine();
                            }
                        }
                    }

                    if (Ftp_DownloadFileToLocal(files, string.Format("{0}\\{1}", zirDir, item.Split('/').ElementAt(1))))
                    {
                        continue;
                    }
                }

                saveToPath = saveToPath.TrimEnd('\\');
                saveToPath = saveToPath.Replace(saveToPath.Split('\\').LastOrDefault(), "");
                string zipPath = saveToPath.TrimEnd('\\');
                zipPath = string.Concat(zipPath, ".zip");
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }
                ZipFile.CreateFromDirectory(saveToPath, zipPath, CompressionLevel.Optimal, true);

                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                List<string> attachPath = new List<string>();
                attachPath.Add(zipPath);
                if (mail.SendMail(emails, subject, content, userId, attachPath))
                {
                    Directory.Delete(saveToPath, true);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Ftp_RenameFolder(string folderName)
        {
            try
            {
                bool res = new bool();
                string path = string.Concat(urlFtp, dir, folderName, "/");
                List<Guid> dirList = new List<Guid>();
                Guid folderGuid;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(path));
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(user, pass);
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    request = null;
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        string line = streamReader.ReadLine();
                        while (!string.IsNullOrEmpty(line))
                        {
                            if (Guid.TryParse(line, out folderGuid))
                            {
                                dirList.Add(Guid.Parse(line));
                            }
                            line = streamReader.ReadLine();
                        }
                    }
                }

                foreach (var item in dirList)
                {
                    res = false;
                    string key = string.Empty;
                    string checkFolder = string.Empty;
                    string changeFolder = string.Concat(path, item.ToString());
                    using (clsContext db = new clsContext())
                    {
                        var data = db.Services.Find(item);
                        if (data == null)
                        {
                            res = true;
                            continue;
                        }

                        key = data.Service_Key;
                        checkFolder = string.Concat(path, key);
                    }

                    try
                    {
                        request = (FtpWebRequest)WebRequest.Create(new Uri(checkFolder));
                        request.Method = WebRequestMethods.Ftp.ListDirectory;
                        request.Credentials = new NetworkCredential(user, pass);
                        using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                        {
                            request = null;
                            if (response.StatusCode == FtpStatusCode.DataAlreadyOpen)
                            {
                                key = string.Concat(key, "_");
                                goto StartRename;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        goto StartRename;
                    }

                StartRename:
                    request = (FtpWebRequest)WebRequest.Create(new Uri(changeFolder));
                    request.Method = WebRequestMethods.Ftp.Rename;
                    request.Credentials = new NetworkCredential(user, pass);
                    request.RenameTo = key;

                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        request = null;
                        if (response.StatusCode == FtpStatusCode.FileActionOK)
                        {
                            res = true;
                            continue;
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

        public string Ftp_UploadFileToString(string fullDir, HttpPostedFileBase filePost)
        {
            try
            {
                string res = string.Empty;
                string fileName = filePost.FileName;
                finalPath = GetFinallyPath(string.Concat(dir, fullDir));
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
                        res = fileName.Replace(urlFtp, urlDomain);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string Ftp_UploadFileToString(string fullDir, HttpPostedFileBase filePost, string fileName)
        {
            try
            {
                string res = string.Empty;
                finalPath = GetFinallyPath(string.Concat(dir, fullDir));

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
                        res = fileName.Replace(urlFtp, urlDomain);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public clsImage Ftp_UploadImageToString(string fullDir, HttpPostedFileBase filePost, string fileName = "", int maxHeight = 256)
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

                int thumbW = originalFile.Width;
                int thumbH = originalFile.Height;

                if (originalFile.Height > maxHeight)
                {
                    decimal formula = Convert.ToDecimal(maxHeight) / Convert.ToDecimal(originalFile.Height);

                    thumbW = Convert.ToInt32(formula * Convert.ToDecimal(originalFile.Width));
                    thumbH = Convert.ToInt32(formula * Convert.ToDecimal(originalFile.Height));
                }

                Image thumbnailFile = originalFile.GetThumbnailImage(thumbW, thumbH, null, IntPtr.Zero);
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

                    request.KeepAlive = true;
                    request.UseBinary = true;
                    request.ContentLength = bytes.Length;

                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(bytes, 0, bytes.Length);
                        if (reqStream.CanWrite)
                        {
                            if (string.IsNullOrEmpty(res.OriginalPath))
                            {
                                res.OriginalPath = item.FtpPath.Replace(urlFtp, urlDomain);
                            }
                            else
                            {
                                res.ThumbnailPath = item.FtpPath.Replace(urlFtp, urlDomain);
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
                FileInfo fileInfo = new FileInfo(zipPath);
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.ContentType = MediaTypeNames.Application.Octet;
                HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", fileInfo.Name));
                HttpContext.Current.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                HttpContext.Current.Response.WriteFile(fileInfo.FullName);
                HttpContext.Current.Response.Flush();
                File.Delete(zipPath);
                HttpContext.Current.Response.End();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
