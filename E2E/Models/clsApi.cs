using E2E.Models.Tables;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Web;

namespace E2E.Models
{
    public class ClsApi
    {
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
                        break;
                }

                return res;
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
                string nameConn = ConfigurationManager.AppSettings["NameConn"];

                switch (nameConn)
                {
                    case "ConnPro":

                        return ConfigurationManager.AppSettings["TokenPro"];

                    case "ConnDev":

                        return ConfigurationManager.AppSettings["TokenDev"];

                    default:
                        return string.Empty;
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

        public ReturnDelete Delete_File(string fileUrl)
        {
            ReturnDelete res = new ReturnDelete();
            string TokenKey = GetToken();
            Uri ApiUrl = new Uri(GetDomainURL() + "api/Service_File/Delete_File");

            RestClientOptions options = new RestClientOptions(ApiUrl)
            {
                ThrowOnAnyError = true
            };

            using (RestClient client = new RestClient(options))
            {
                RestRequest request = new RestRequest()
                    .AddHeader("Token", TokenKey)
                    .AddParameter("path", fileUrl);
                RestResponse response = client.PostAsync(request).Result;
                res = JsonConvert.DeserializeObject<ReturnDelete>(response.Content);
            }

            return res;
        }

        public bool SendMail(ClsServiceEmail clsServiceEmail, HttpFileCollectionBase file = null)
        {
            try
            {
                ReturnSend returnSend = new ReturnSend();
                string resApi = string.Empty;
                List<ClsFile> clsFiles = new List<ClsFile>();
                string TokenKey = GetToken();
                Uri ApiUrl = new Uri(GetDomainURL() + "api/Service_Email/Send");

                var multiClass = new
                {
                    clsServiceEmail.SendFrom,
                    clsServiceEmail.SendTo,
                    clsServiceEmail.SendCC,
                    clsServiceEmail.SendBCC,
                    clsServiceEmail.Subject,
                    clsServiceEmail.Body,
                    clsServiceEmail.FilePath
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
                        .AddHeader("Token", TokenKey)
                        .AddBody(multiClass, "application/json");

                    if (file != null)
                    {
                        for (int i = 0; i < file.Count; i++)
                        {
                            HttpPostedFileBase fileBase = file.Get(i);

                            request.AddFile("fileAttach", GetByteFileBase(fileBase), fileBase.FileName, fileBase.ContentType);
                        }
                    }
                    RestResponse response = client.PostAsync(request).Result;
                    returnSend = JsonConvert.DeserializeObject<ReturnSend>(response.Content);
                }

                if (!returnSend.CanSend)
                {
                    throw new Exception(returnSend.ErrorMessage);
                }

                return returnSend.CanSend;
            }
            catch (Exception) { throw; }
        }

        public ReturnUpload UploadFile(ClsServiceFile clsServiceFile, HttpPostedFileBase file = null)
        {
            try
            {
                ReturnUpload returnUpload = new ReturnUpload();

                string resApi = string.Empty;
                string TokenKey = GetToken();
                Uri ApiUrl = new Uri(GetDomainURL() + "api/Service_File/Upload");

                RestClientOptions options = new RestClientOptions(ApiUrl)
                {
                    ThrowOnAnyError = true
                };

                using (RestClient client = new RestClient(options))
                {
                    RestRequest request = new RestRequest()
                        .AddHeader("Token", TokenKey)
                        .AddParameter("folderPath", clsServiceFile.FolderPath)
                        .AddFile("fileUpload", GetByteFileBase(file), file.FileName, file.ContentType);
                    RestResponse response = client.PostAsync(request).Result;
                    returnUpload = JsonConvert.DeserializeObject<ReturnUpload>(response.Content);
                }

                if (!returnUpload.CanUpload)
                {
                    throw new Exception(returnUpload.ErrorMessage);
                }
                return returnUpload;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class ResponseUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Users Users { get; set; }
    }
}
