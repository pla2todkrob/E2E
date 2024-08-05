using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace E2E.Models
{
    public class ClsMail
    {
        private readonly ClsApi clsApi = new ClsApi();
        private readonly ClsContext db = new ClsContext();

        public ClsMail()
        {
            SendTos = new List<Guid>();
            AttachPaths = new List<string>();
            SendBCCs = new List<Guid>();
            SendCCs = new List<Guid>();
        }

        public List<string> AttachPaths { get; set; }
        public string Body { get; set; }
        public List<Guid> SendBCCs { get; set; }
        public List<Guid> SendCCs { get; set; }
        public Guid SendFrom { get; set; }
        public List<Guid> SendTos { get; set; }
        public string Subject { get; set; }

        public async Task<bool> ResendMail(Guid refId)
        {
            bool res = new bool();
            Log_SendEmail log_SendEmail = await db.Log_SendEmails
                .Where(w => w.SendEmail_Ref_Id == refId)
                .FirstOrDefaultAsync();

            ClsMail clsMail = new ClsMail();

            List<Log_SendEmailTo> log_SendEmailTos = new List<Log_SendEmailTo>();
            log_SendEmailTos = db.Log_SendEmailTos
                .Where(w => w.SendEmail_Id == log_SendEmail.SendEmail_Id)
                .ToList();
            foreach (var item in log_SendEmailTos)
            {
                switch (item.SendEmailTo_Type)
                {
                    case "to":
                        clsMail.SendTos.Add(item.User_Id);
                        break;

                    case "cc":
                        clsMail.SendCCs.Add(item.User_Id);
                        break;

                    case "bcc":
                        clsMail.SendBCCs.Add(item.User_Id);
                        break;

                    default:
                        break;
                }
            }

            clsMail.Subject = log_SendEmail.SendEmail_Subject;
            clsMail.Body = log_SendEmail.SendEmail_Content;
            clsMail.SendFrom = log_SendEmail.User_Id;

            res = await SendMail(clsMail);
            if (res)
            {
                ServiceComments comments = new ServiceComments()
                {
                    Comment_Content = "The system has sent an email to the service requestor again to have the service requestor close the job.",
                    Service_Id = refId,
                    User_Id = log_SendEmail.User_Id
                };
                db.ServiceComments.Add(comments);
                await db.SaveChangesAsync();
            }

            return res;
        }

        public async Task<bool> SendMail(ClsMail model, HttpFileCollectionBase files = null)
        {
            EmailAddressAttribute attribute = new EmailAddressAttribute();
            ClsServiceEmail clsServiceEmail = new ClsServiceEmail();
            string dear = "Dear ";

            List<ReceiveData> receiveDatas = db.UserDetails
                .Where(w => model.SendTos.Contains(w.User_Id))
                .Select(s => new ReceiveData()
                {
                    Email = s.Users.User_Email,
                    NameEN = s.Detail_EN_FirstName,
                    NameTH = s.Detail_TH_FirstName,
                    FullNameEN = s.Detail_EN_FirstName + " " + s.Detail_EN_LastName,
                    FullNameTH = s.Detail_TH_FirstName + " " + s.Detail_TH_LastName
                }).ToList();

            if (receiveDatas.Count == 0)
            {
                return true;
            }

            List<string> EmailTos = new List<string>();
            foreach (var item in receiveDatas.Where(w => !string.IsNullOrEmpty(w.Email)))
            {
                attribute = new EmailAddressAttribute();
                if (attribute.IsValid(item.Email))
                {
                    dear += string.Format("{0}, ", item.FullNameEN);
                    EmailTos.Add(item.Email);
                }
            }
            dear = dear.Trim().TrimEnd(',');

            if (EmailTos.Count > 0)
            {
                clsServiceEmail.SendTo = EmailTos.ToArray();
            }
            else
            {
                return true;
            }

            if (model.SendCCs.Count > 0)
            {
                receiveDatas = new List<ReceiveData>();
                receiveDatas = db.UserDetails
                .Where(w => model.SendCCs.Contains(w.User_Id))
                .Select(s => new ReceiveData()
                {
                    Email = s.Users.User_Email,
                    NameEN = s.Detail_EN_FirstName,
                    NameTH = s.Detail_TH_FirstName,
                    FullNameEN = s.Detail_EN_FirstName + " " + s.Detail_EN_LastName,
                    FullNameTH = s.Detail_TH_FirstName + " " + s.Detail_TH_LastName
                }).ToList();

                List<string> EmailCCs = new List<string>();
                foreach (var item in receiveDatas.Where(w => !string.IsNullOrEmpty(w.Email)))
                {
                    attribute = new EmailAddressAttribute();
                    if (attribute.IsValid(item.Email))
                    {
                        EmailCCs.Add(item.Email);
                    }
                }
                if (EmailCCs.Count > 0)
                {
                    clsServiceEmail.SendCC = EmailCCs.ToArray();
                }
            }

            if (model.SendBCCs.Count > 0)
            {
                receiveDatas = new List<ReceiveData>();
                receiveDatas = db.UserDetails
                .Where(w => model.SendBCCs.Contains(w.User_Id))
                .Select(s => new ReceiveData()
                {
                    Email = s.Users.User_Email,
                    NameEN = s.Detail_EN_FirstName,
                    NameTH = s.Detail_TH_FirstName,
                    FullNameEN = s.Detail_EN_FirstName + " " + s.Detail_EN_LastName,
                    FullNameTH = s.Detail_TH_FirstName + " " + s.Detail_TH_LastName
                }).ToList();

                List<string> EmailBCCs = new List<string>();
                foreach (var item in receiveDatas.Where(w => !string.IsNullOrEmpty(w.Email)))
                {
                    attribute = new EmailAddressAttribute();
                    if (attribute.IsValid(item.Email))
                    {
                        EmailBCCs.Add(item.Email);
                    }
                }

                if (EmailBCCs.Count > 0)
                {
                    clsServiceEmail.SendBCC = EmailBCCs.ToArray();
                }
            }

            if (model.AttachPaths.Count > 0)
            {
                foreach (var item in model.AttachPaths)
                {
                    clsServiceEmail.ClsFileAttaches.Add(new ClsFileAttach() { FilePath = item });
                }
            }

            Guid userId = (HttpContext.Current?.User?.Identity?.IsAuthenticated == true)
                ? Guid.Parse(HttpContext.Current.User.Identity.Name)
                : model.SendFrom;

            UserDetails userDetails = await db.UserDetails.Where(w => w.User_Id == userId).FirstOrDefaultAsync();

            string strBody = "<html>";
            strBody += "<head>";
            strBody += "</head>";
            strBody += "<body>";
            strBody += string.Format("<p><b>{0}</b></p>", dear);
            strBody += model.Body;

            strBody += "</body>";
            strBody += "</html>";

            clsServiceEmail.Body = strBody;
            clsServiceEmail.Subject = model.Subject;
            clsServiceEmail.SendFrom = userDetails.Users.User_Email;

            return await clsApi.SendMail(clsServiceEmail, files);
        }

        //API Complete
        public class ReceiveData
        {
            public string Email { get; set; }
            public string FullNameEN { get; set; }
            public string FullNameTH { get; set; }
            public string NameEN { get; set; }
            public string NameTH { get; set; }
        }
    }
}
