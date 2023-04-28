using E2E.Models.Tables;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text;
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
                        res = ConfigurationManager.AppSettings["UrlLocal"];
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

        public FileResponse Delete_File(string fileUrl)
        {
            FileResponse res = new FileResponse();
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
                Uri ApiUrl = new Uri(GetDomainURL() + "api/Service_Email/Send");

                var multiClass = new
                {
                    clsServiceEmail.SendFrom,
                    clsServiceEmail.SendTo,
                    clsServiceEmail.SendCC,
                    clsServiceEmail.SendBCC,
                    clsServiceEmail.Subject,
                    clsServiceEmail.Body
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

                    if (files != null)
                    {
                        foreach (var item in files.AllKeys)
                        {
                            if (files[item].ContentLength > 0)
                            {
                                request.AddFile("fileAttach", GetByteFileBase(files[item]), HttpUtility.UrlEncode(files[item].FileName, Encoding.UTF8), files[item].ContentType);
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
                Uri ApiUrl = new Uri(GetDomainURL() + "api/Service_File/Upload");

                RestClientOptions options = new RestClientOptions(ApiUrl)
                {
                    ThrowOnAnyError = true
                };

                using (RestClient client = new RestClient(options))
                {
                    RestRequest request = new RestRequest()
                        .AddHeader("Token", TokenKey)
                        .AddParameter("FolderPath", clsServiceFile.FolderPath)
                        .AddFile("fileUpload", GetByteFileBase(file), HttpUtility.UrlEncode(file.FileName, Encoding.UTF8), file.ContentType);
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

    public class ResponseUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Users Users { get; set; }
    }
}
