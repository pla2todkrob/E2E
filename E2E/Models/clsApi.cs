using E2E.Models.Tables;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace E2E.Models
{
    public class ClsApi
    {
        private string GetApiUrl()
        {
            try
            {
                string target = ConfigurationManager.AppSettings["TargetHost"];
                string res = string.Empty;

                switch (target)
                {
                    case "Pro":
                        res = ConfigurationManager.AppSettings["ApiUrlPro"];
                        break;

                    case "Dev":
                        res = ConfigurationManager.AppSettings["ApiUrlDev"];
                        break;

                    default:
                        res = ConfigurationManager.AppSettings["ApiUrlLocal"];
                        break;
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private byte[] GetByteFileBase(HttpPostedFileBase file)
        {
            try
            {
                using (file.InputStream)
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        file.InputStream.CopyTo(memory);
                        return memory.ToArray();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetToken()
        {
            try
            {
                string target = ConfigurationManager.AppSettings["TargetHost"];

                switch (target)
                {
                    case "Pro":

                        return ConfigurationManager.AppSettings["TokenPro"];

                    case "Dev":

                        return ConfigurationManager.AppSettings["TokenDev"];

                    default:
                        return ConfigurationManager.AppSettings["TokenLocal"];
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ClsApi()
        {
            IsSuccess = new bool();
        }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public dynamic Value { get; set; }

        public byte[] ConvertByte(string filePath)
        {
            WebClient client = new WebClient();
            byte[] fileContent = client.DownloadData(filePath);
            return fileContent;
        }

        public FileResponse Delete_File(string fileUrl)
        {
            FileResponse res = new FileResponse();
            string TokenKey = GetToken();
            Uri ApiUrl = new Uri(GetApiUrl() + "api/Service_File/Delete_File");
            fileUrl = HttpUtility.UrlDecode(fileUrl, Encoding.UTF8);
            RestClientOptions options = new RestClientOptions(ApiUrl)
            {
                ThrowOnAnyError = true
            };

            using (RestClient client = new RestClient(options))
            {
                RestRequest request = new RestRequest()
                    .AddHeader("Token", TokenKey)
                    .AddHeader("FilePath", HttpUtility.UrlEncode(fileUrl, Encoding.UTF8));
                RestResponse response = client.PostAsync(request).Result;
                res = JsonConvert.DeserializeObject<FileResponse>(response.Content);
            }

            return res;
        }

        public bool SendMail(ClsServiceEmail clsServiceEmail, HttpFileCollectionBase files = null)
        {
            try
            {
                MailResponse mailResponse = new MailResponse();
                string resApi = string.Empty;
                List<ClsFileAttach> clsFiles = new List<ClsFileAttach>();
                string TokenKey = GetToken();
                Uri ApiUrl = new Uri(GetApiUrl() + "api/Service_Email/Send");

                var multiClass = new
                {
                    clsServiceEmail.SendFrom,
                    clsServiceEmail.SendTo,
                    clsServiceEmail.SendCC,
                    clsServiceEmail.SendBCC,
                    clsServiceEmail.Subject,
                    clsServiceEmail.Body,
                    ClsFileLists = clsServiceEmail.ClsFileAttaches
                };

                //Test text
                //string jsonText = JsonConvert.SerializeObject(multiClass);

                RestClientOptions options = new RestClientOptions(ApiUrl)
                {
                    ThrowOnAnyError = true,
                    ThrowOnDeserializationError = true
                };

                using (RestClient client = new RestClient(options))
                {
                    RestRequest request = new RestRequest()
                        //.AddJsonBody(clsServiceEmail)
                        .AddHeader("Token", TokenKey);
                    request.AddParameter("SendFrom", clsServiceEmail.SendFrom);

                    for (int i = 0; i < clsServiceEmail.SendTo.Length; i++)
                    {
                        request.AddParameter("SendTo", clsServiceEmail.SendTo[i]);
                    }
                    for (int i = 0; i < clsServiceEmail.SendCC?.Length; i++)
                    {
                        request.AddParameter("SendCC", clsServiceEmail.SendCC[i]);
                    }
                    for (int i = 0; i < clsServiceEmail.SendBCC?.Length; i++)
                    {
                        request.AddParameter("SendBCC", clsServiceEmail.SendBCC[i]);
                    }

                    request.AddParameter("Subject", clsServiceEmail.Subject);
                    request.AddParameter("Body", clsServiceEmail.Body);

                    if (files != null)
                    {
                        request.AlwaysMultipartFormData = true;
                        foreach (var item in files.AllKeys)
                        {
                            if (files[item].ContentLength > 0)
                            {
                                request.AddFile("fileAttach", GetByteFileBase(files[item]), HttpUtility.UrlEncode(files[item].FileName, Encoding.UTF8), files[item].ContentType);
                            }
                        }
                    }

                    if (clsServiceEmail.ClsFileAttaches.Count > 0)
                    {
                        request.AlwaysMultipartFormData = true;
                        foreach (var item in clsServiceEmail.ClsFileAttaches)
                        {
                            if (!string.IsNullOrEmpty(item.FilePath))
                            {
                                request.AddFile("fileAttach", ConvertByte(item.FilePath), HttpUtility.UrlEncode(Path.GetFileName(item.FilePath), Encoding.UTF8), MimeMapping.GetMimeMapping(item.FilePath));
                            }
                        }
                    }
                    RestResponse response = client.PostAsync(request).Result;
                    mailResponse = JsonConvert.DeserializeObject<MailResponse>(response.Content);
                }

                if (!mailResponse.IsSuccess)
                {
                    throw new Exception(mailResponse.ErrorMessage);
                }

                return mailResponse.IsSuccess;
            }
            catch (Exception) { throw; }
        }

        public FileResponse UploadFile(ClsServiceFile clsServiceFile, HttpPostedFileBase file)
        {
            try
            {
                FileResponse fileResponse = new FileResponse();

                string resApi = string.Empty;
                string TokenKey = GetToken();
                Uri ApiUrl = new Uri(GetApiUrl() + "api/Service_File/Upload");

                RestClientOptions options = new RestClientOptions(ApiUrl)
                {
                    ThrowOnAnyError = true
                };

                using (RestClient client = new RestClient(options))
                {
                    string fileName = file.FileName;
                    if (string.IsNullOrEmpty(fileName))
                    {
                        fileName = clsServiceFile.Filename;
                    }

                    string mimeType = MimeMapping.GetMimeMapping(fileName);

                    RestRequest request = new RestRequest()
                        .AddHeader("Token", TokenKey)
                        .AddParameter("FolderPath", clsServiceFile.FolderPath)
                        .AddFile("fileUpload", GetByteFileBase(file), HttpUtility.UrlEncode(fileName, Encoding.UTF8), mimeType);
                    RestResponse response = client.PostAsync(request).Result;
                    fileResponse = JsonConvert.DeserializeObject<FileResponse>(response.Content);
                }

                if (!fileResponse.IsSuccess)
                {
                    throw new Exception(fileResponse.ErrorMessage);
                }
                return fileResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public FileResponse UploadFile(HttpPostedFileBase file, string folderPath, string fileName = "")
        {
            try
            {
                FileResponse fileResponse = new FileResponse();

                string resApi = string.Empty;
                string TokenKey = GetToken();
                Uri ApiUrl = new Uri(GetApiUrl() + "api/Service_File/Upload");

                RestClientOptions options = new RestClientOptions(ApiUrl)
                {
                    ThrowOnAnyError = true
                };

                using (RestClient client = new RestClient(options))
                {
                    if (string.IsNullOrEmpty(fileName))
                    {
                        fileName = file.FileName;
                    }

                    string mimeType = MimeMapping.GetMimeMapping(fileName);

                    RestRequest request = new RestRequest()
                        .AddHeader("Token", TokenKey)
                        .AddParameter("FolderPath", folderPath)
                        .AddFile("fileUpload", GetByteFileBase(file), HttpUtility.UrlEncode(fileName, Encoding.UTF8), mimeType);
                    RestResponse response = client.PostAsync(request).Result;
                    fileResponse = JsonConvert.DeserializeObject<FileResponse>(response.Content);
                }

                if (!fileResponse.IsSuccess)
                {
                    throw new Exception(fileResponse.ErrorMessage);
                }
                return fileResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class UserResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Users Users { get; set; }
    }
}
