using System;
using System.Collections.Generic;
using System.Configuration;
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

        public string Ftp_UploadFileToString(string fullDir, HttpPostedFileBase filePost, string fileName = "")
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

        private bool CheckDirectory(string fullDir)
        {
            bool res = new bool();
        StartMethod:
            string dir = url;

            try
            {
                string[] splitDir = fullDir.Trim().Split('/');

                foreach (var item in splitDir)
                {
                    dir += string.Concat(item, "/");
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(dir));
                    request.Method = WebRequestMethods.Ftp.ListDirectory;
                    request.Credentials = new NetworkCredential(user, pass);

                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        if (true)
                        {
                            res = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                if (CreateDirectory(dir))
                {
                    goto StartMethod;
                }
            }

            return res;
        }

        private bool CreateDirectory(string path)
        {
            try
            {
                bool res = new bool();

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}