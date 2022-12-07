using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace E2E.Models
{
    public class clsTP_Service
    {
        private byte[] GetByteFileBase(HttpPostedFileBase file)
        {
            try
            {
                byte[] data;
                using (Stream inputStream = file.InputStream)
                {
                    if (!(inputStream is MemoryStream memoryStream))
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    data = memoryStream.ToArray();
                }
                return data;
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

        public ReturnDelete Delete_File(string fileUrl)
        {
            ReturnDelete res = new ReturnDelete();
            string TokenKey = GetToken();
            //string ApiUrl = "https://tp-portal.thaiparker.co.th/TP_Service/api/Service_Email/send";
            string ApiUrl = GetDomainURL() + "api/Service_File/Delete_File";
            var keyVal = new
            {
                path = fileUrl
            };
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Token", TokenKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync(ApiUrl, keyVal).Result;
                response.EnsureSuccessStatusCode();
                var content = response.Content.ReadAsStringAsync().Result;
                res = JsonConvert.DeserializeObject<ReturnDelete>(content);
            }

            return res;
        }

        public bool SendMail(clsServiceEmail clsServiceEmail, HttpFileCollectionBase file = null)
        {
            try
            {
                ReturnSend returnSend = new ReturnSend();
                string resApi = string.Empty;
                List<clsFile> clsFiles = new List<clsFile>();
                string TokenKey = GetToken();
                //string ApiUrl = "https://tp-portal.thaiparker.co.th/TP_Service/api/Service_Email/send";
                string ApiUrl = GetDomainURL() + "api/Service_Email/send";

                if (file?.Count > 0)
                {
                    for (int i = 0; i < file.Count; i++)
                    {
                        var fileBase = file.Get(i);

                        byte[] data = GetByteFileBase(fileBase);

                        string B64 = Convert.ToBase64String(data);

                        clsFiles.Add(new clsFile() { Base64 = B64, FileName = fileBase.FileName });
                    }
                }

                var multiClass = new
                {
                    clsServiceEmail.sendFrom,
                    clsServiceEmail.sendTo,
                    clsServiceEmail.sendCC,
                    clsServiceEmail.sendBCC,
                    clsServiceEmail.Subject,
                    clsServiceEmail.Body,
                    clsServiceEmail.filePath,
                    clsFiles
                };

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Token", TokenKey);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.PostAsJsonAsync(ApiUrl, multiClass).Result;
                    response.EnsureSuccessStatusCode();
                    var content = response.Content.ReadAsStringAsync().Result;
                    returnSend = JsonConvert.DeserializeObject<ReturnSend>(resApi);
                }

                if (!returnSend.canSend)
                {
                    throw new Exception(returnSend.errorMessage);
                }

                return returnSend.canSend;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ReturnUpload UploadFile(clsServiceFile clsServiceFile, HttpPostedFileBase file = null)
        {
            try
            {
                ReturnUpload returnUpload = new ReturnUpload();

                string resApi = string.Empty;
                string TokenKey = GetToken();
                //string ApiUrl = "https://tp-portal.thaiparker.co.th/TP_Service/api/Service_File/upload";
                string ApiUrl = GetDomainURL() + "api/Service_File/upload";

                using (MultipartFormDataContent dataContent = new MultipartFormDataContent())
                {
                    dataContent.Add(new StringContent(file.FileName), "filename");
                    dataContent.Add(new StringContent(clsServiceFile.folderPath), "folderPath");
                    if (file.ContentLength > 0)
                    {
                        //StreamContent streamContent = new StreamContent(file.InputStream, file.ContentLength);
                        //streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType)
                        //{
                        //    CharSet = "utf-8"
                        //};
                        //streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        //{
                        //    Name = "fileUpload",
                        //    FileName = file.FileName
                        //};
                        //dataContent.Add(streamContent);

                        ByteArrayContent arrayContent = new ByteArrayContent(GetByteFileBase(file));
                        arrayContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType)
                        {
                            CharSet = "utf-8"
                        };
                        arrayContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "fileUpload",
                            FileName = file.FileName
                        };
                        dataContent.Add(arrayContent);
                    }

                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Token", TokenKey);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = client.PostAsync(ApiUrl, dataContent).Result;
                        response.EnsureSuccessStatusCode();
                        string content = response.Content.ReadAsStringAsync().Result;
                        returnUpload = JsonConvert.DeserializeObject<ReturnUpload>(content);
                    }

                    if (!returnUpload.canUpload)
                    {
                        throw new Exception(returnUpload.errorMessage);
                    }
                }
                return returnUpload;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
