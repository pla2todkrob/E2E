using E2E.Models.Tables;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Razor.Tokenizer;
using ThaiStringTokenizer;

namespace E2E.Models
{
    public class ClsOpenAI
    {
        public string Answer { get; set; }
        public DateTime AnswerDateTime { get; set; }
        public string Question { get; set; }
        public DateTime QuestionDateTime { get; set; }
        public bool Display { get; set; }
        public string Question_Hidden { get; set; }

        public ClsOpenAI Response(ClsOpenAI model)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    using (ClsContext db = new ClsContext())
                    {
                        ChatGPT chatGPT = new ChatGPT()
                        {
                            Answer = model.Answer.Trim(),
                            AnswerDateTime = model.AnswerDateTime,
                            Question = model.Question_Hidden.Trim(),
                            QuestionDateTime = model.QuestionDateTime,
                            User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name),
                            Display = true,
                            Tokens = CountTokens(model.Question_Hidden),
                        };
                        db.Entry(chatGPT).State = System.Data.Entity.EntityState.Added;
                        if (db.SaveChanges() > 0)
                        {
                            scope.Complete();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return model;
        }
        public class GptResponse
        {
            public string text { get; set; }
        }

        public static int CountTokens(string sentence)
        {
            // กำหนดข้อความที่ต้องการส่งไปให้ GPT
            string prompt = sentence;

            // กำหนด API key ของคุณ
            string apiKey = ConfigurationManager.AppSettings["OpenAI_Key"];

            // กำหนด URL ของ API endpoint
            string apiUrl = "https://api.openai.com/v1/engines/davinci-codex/completions";

            // กำหนดจำนวน tokens ที่ต้องการให้ GPT สร้าง
            int maxTokens = 2000;
            
            // สร้าง JSON payload สำหรับส่งข้อมูลไปยัง API endpoint
            var payload = new
            {
                prompt = prompt,
                max_tokens = maxTokens
            };
            string jsonPayload = JsonConvert.SerializeObject(payload);

            // ส่ง HTTP request ไปยัง API endpoint
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
            var response = client.PostAsync(apiUrl, content).Result;
            var jsonResponse = response.Content.ReadAsStringAsync().Result;

            // แปลง JSON response เป็น GptResponse object
            dynamic gptResponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            int numTokens = 0;

            // นับจำนวน token จาก response text
            if (gptResponse.usage.prompt_tokens != null)
            {
                 numTokens = Convert.ToInt32(gptResponse.usage.prompt_tokens);
            }


            return numTokens;
        }
    }
}
