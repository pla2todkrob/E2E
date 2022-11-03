using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace E2E.Models
{
    public class clsTP_Service
    {
        public bool SendMail(clsServiceEmail clsServiceEmail, HttpFileCollectionBase file = null)
        {
            bool res = new bool();
            string resApi = string.Empty;
            ReturnSend returnSend = new ReturnSend();

            try
            {
                List<clsFile> clsFiles = new List<clsFile>();
                string API_KEY = GetToken();
                //string ApiUrl = "https://tp-portal.thaiparker.co.th/TP_Service/api/Service_Email/send";
                string ApiUrl = GetDomainURL() + "api/Service_Email/send";

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ApiUrl);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Token", API_KEY);

                if (file?.Count > 0)
                {
                    for (int i = 0; i < file.Count; i++)
                    {
                        var fileBase = file.Get(i);

                        byte[] data = readFileContents(fileBase);

                        string B64 = Convert.ToBase64String(data);

                        clsFiles.Add(new clsFile() { Base64 = B64, FileName = fileBase.FileName });
                    }
                }

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        sendFrom = clsServiceEmail.sendFrom,
                        sendTo = clsServiceEmail.sendTo,
                        sendCC = clsServiceEmail.sendCC,
                        sendBCC = clsServiceEmail.sendBCC,
                        Subject = clsServiceEmail.Subject,
                        Body = clsServiceEmail.Body,
                        filePath = clsServiceEmail.filePath,
                        clsFiles = clsFiles
                    });

                    streamWriter.Write(json);
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    resApi = streamReader.ReadToEnd();
                }

                returnSend = JsonConvert.DeserializeObject<ReturnSend>(resApi);

                if (returnSend.canSend)
                {
                    res = returnSend.canSend;
                }
                else
                {
                    Exception ex = new Exception(returnSend.errorMessage);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }

        public ReturnUpload UploadFile(clsServiceFile clsServiceFile, HttpPostedFileBase file = null)
        {
            ReturnUpload returnUpload = new ReturnUpload();
            string resApi = string.Empty;

            try
            {
                string API_KEY = GetToken();
                //string ApiUrl = "https://tp-portal.thaiparker.co.th/TP_Service/api/Service_File/upload";
                string ApiUrl = GetDomainURL() + "api/Service_File/upload";

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ApiUrl);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Token", API_KEY);

                if (file?.ContentLength > 0)
                {
                    byte[] data = readFileContents(file);
                    string B64 = Convert.ToBase64String(data);

                    clsServiceFile.Base64 = B64;
                    if (string.IsNullOrEmpty(clsServiceFile.filename))
                    {
                        clsServiceFile.filename = file.FileName;
                    }
                }

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        folderPath = clsServiceFile.folderPath,
                        Base64 = clsServiceFile.Base64,
                        filename = clsServiceFile.filename
                    });

                    streamWriter.Write(json);
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    resApi = streamReader.ReadToEnd();
                }

                returnUpload = JsonConvert.DeserializeObject<ReturnUpload>(resApi);

                if (returnUpload.canUpload)
                {
                }
                else
                {
                    Exception ex = new Exception(returnUpload.errorMessage);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return returnUpload;
        }

        public ReturnDelete Delete_File(string path)
        {
            ReturnDelete res = new ReturnDelete();
            string API_KEY = GetToken();
            //string ApiUrl = "https://tp-portal.thaiparker.co.th/TP_Service/api/Service_Email/send";
            string ApiUrl = GetDomainURL() + "api/Service_File/Delete_File";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(ApiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Token", API_KEY);

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    path = path
                });

                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var resApi = streamReader.ReadToEnd();

                res = JsonConvert.DeserializeObject<ReturnDelete>(resApi);
            }

            return res;
        }

        private byte[] readFileContents(HttpPostedFileBase file)
        {
            Stream fileStream = file.InputStream;
            var mStreamer = new MemoryStream();
            mStreamer.SetLength(fileStream.Length);
            fileStream.Read(mStreamer.GetBuffer(), 0, (int)fileStream.Length);
            mStreamer.Seek(0, SeekOrigin.Begin);
            byte[] fileBytes = mStreamer.GetBuffer();

            return fileBytes;
        }

        private string GetDomainURL()
        {
            try
            {
                string nameConn = ConfigurationManager.AppSettings["NameConn"];
                string res = string.Empty;

                switch (nameConn)
                {
                    case "ConnPro":
                        res = ConfigurationManager.AppSettings["UrlPro"];
                        break;

                    case "ConnDev":
                        res = ConfigurationManager.AppSettings["UrlDev"];
                        break;

                    default:
                        res = "https://localhost:44317/";
                        break;
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetToken()
        {
            try
            {
                string nameConn = ConfigurationManager.AppSettings["NameConn"];

                switch (nameConn)
                {
                    case "ConnPro":

                        return ConfigurationManager.AppSettings["TokenPro"];

                    case "ConnDev":

                        return ConfigurationManager.AppSettings["TokenDev"];

                    default:
                        return "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}