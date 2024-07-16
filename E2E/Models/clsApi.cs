using E2E.Models.Tables;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
            using (var binaryReader = new BinaryReader(file.InputStream))
            {
                return binaryReader.ReadBytes(file.ContentLength);
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

        private bool IsLocalPath(string path)
        {
            bool isLocalPath = Uri.TryCreate(path, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return !isLocalPath;
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
            byte[] fileContent;

            if (IsLocalPath(filePath))
            {
                fileContent = File.ReadAllBytes(filePath);
            }
            else
            {
                WebClient client = new WebClient();
                fileContent = client.DownloadData(filePath);
            }

            return fileContent;
        }

        public async Task<FileResponse> DeleteFile(string fileUrl)
        {
            FileResponse res = new FileResponse();
            string tokenKey = GetToken();
            Uri apiUrl = new Uri(GetApiUrl() + "api/Service_File/Delete_File");
            fileUrl = HttpUtility.UrlEncode(fileUrl);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Token", tokenKey);
                client.DefaultRequestHeaders.Add("FilePath", fileUrl);

                HttpResponseMessage response = await client.PostAsync(apiUrl, null);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                res = JsonConvert.DeserializeObject<FileResponse>(content);
            }

            return res;
        }

        public async Task<bool> SendMail(ClsServiceEmail clsServiceEmail, HttpFileCollectionBase files = null)
        {
            MailResponse mailResponse = new MailResponse();
            string resApi = string.Empty;
            List<ClsFileAttach> clsFiles = new List<ClsFileAttach>();
            string tokenKey = GetToken();

            RestClient client = new RestClient(GetApiUrl());
            RestRequest request = new RestRequest("api/Service_Email/Send", Method.Post);
            request.AddHeader("Token", tokenKey);

            request.AddParameter("SendFrom", clsServiceEmail.SendFrom);

            foreach (var recipient in clsServiceEmail.SendTo)
            {
                request.AddParameter("SendTo", recipient);
            }

            if (clsServiceEmail.SendCC != null)
            {
                foreach (var cc in clsServiceEmail.SendCC)
                {
                    request.AddParameter("SendCC", cc);
                }
            }

            if (clsServiceEmail.SendBCC != null)
            {
                foreach (var bcc in clsServiceEmail.SendBCC)
                {
                    request.AddParameter("SendBCC", bcc);
                }
            }

            request.AddParameter("Subject", clsServiceEmail.Subject);
            request.AddParameter("Body", clsServiceEmail.Body);

            if (files != null)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    if (file.ContentLength > 0)
                    {
                        request.AddFile("fileAttach", GetByteFileBase(file), HttpUtility.UrlEncode(file.FileName), file.ContentType);
                    }
                }
            }

            if (clsServiceEmail.ClsFileAttaches.Count > 0)
            {
                foreach (var item in clsServiceEmail.ClsFileAttaches)
                {
                    if (!string.IsNullOrEmpty(item.FilePath))
                    {
                        request.AddFile("fileAttach", ConvertByte(item.FilePath), HttpUtility.UrlEncode(Path.GetFileName(item.FilePath)), MimeMapping.GetMimeMapping(item.FilePath));
                    }
                }
            }

            // Execute the request
            RestResponse response = await client.ExecuteAsync(request);
            response.ThrowIfError();
            mailResponse = JsonConvert.DeserializeObject<MailResponse>(response.Content);
            if (!mailResponse.IsSuccess)
            {
                throw new Exception(mailResponse.ErrorMessage);
            }

            return mailResponse.IsSuccess;
        }

        public async Task<FileResponse> UploadFile(ClsServiceFile clsServiceFile, HttpPostedFileBase file)
        {
            try
            {
                FileResponse fileResponse = new FileResponse();
                string resApi = string.Empty;
                string tokenKey = GetToken();

                byte[] fileBytes = GetByteFileBase(file); // อ่านข้อมูลจากไฟล์เพียงครั้งเดียว

                RestClientOptions options = new RestClientOptions(GetApiUrl())
                {
                    MaxTimeout = -1,
                };
                RestClient client = new RestClient(options);
                RestRequest request = new RestRequest("api/Service_File/Upload", Method.Post);
                request.AddHeader("Token", tokenKey);
                request.AlwaysMultipartFormData = true;
                request.AddFile("fileUpload", fileBytes, HttpUtility.UrlEncode(file.FileName), file.ContentType); // ใช้ byte[] ที่อ่านมาแล้ว
                request.AddParameter("FolderPath", clsServiceFile.FolderPath);
                RestResponse response = await client.ExecuteAsync(request);
                response.ThrowIfError();
                fileResponse = JsonConvert.DeserializeObject<FileResponse>(response.Content);
                fileResponse.FileBytes = fileBytes;
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
