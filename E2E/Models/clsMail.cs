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
        private readonly ClsManageMaster clsManage = new ClsManageMaster();

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
            string dear = "Dear ";
            var validSendTos = await GetValidEmails(model.SendTos ?? new List<Guid>());
            if (!validSendTos.Any())
            {
                return true;  // No valid emails to send to
            }

            var validSendCCs = await GetValidEmails(model.SendCCs ?? new List<Guid>());
            var validSendBCCs = await GetValidEmails(model.SendBCCs ?? new List<Guid>());

            var emailTos = validSendTos.Select(u => u.Email).ToList();
            dear += string.Join(", ", validSendTos.Select(u => u.FullNameEN));
            dear = dear.Trim().TrimEnd(',');

            ClsServiceEmail clsServiceEmail = new ClsServiceEmail
            {
                SendTo = emailTos.ToArray(),
                SendCC = validSendCCs.Select(u => u.Email).ToArray(),
                SendBCC = validSendBCCs.Select(u => u.Email).ToArray(),
                ClsFileAttaches = model.AttachPaths.Select(p => new ClsFileAttach { FilePath = p }).ToList(),
                Body = FormatEmailBody(dear, model.Body),
                Subject = model.Subject,
                SendFrom = (await GetUserDetails(HttpContext.Current?.User?.Identity?.Name ?? model.SendFrom.ToString())).Users.User_Email
            };

            return await clsApi.SendMail(clsServiceEmail, files);
        }

        private async Task<List<ReceiveData>> GetValidEmails(List<Guid> userIds)
        {
            // Return empty list immediately if there are no user IDs
            if (userIds == null || !userIds.Any())
            {
                return new List<ReceiveData>();
            }

            // First, get the relevant user details from the database
            var userDetails = await db.UserDetails
                .Where(u => userIds.Contains(u.User_Id))
                .Select(u => new ReceiveData
                {
                    Email = u.Users.User_Email,
                    NameEN = u.Detail_EN_FirstName,
                    NameTH = u.Detail_TH_FirstName,
                    FullNameEN = u.Detail_EN_FirstName + " " + u.Detail_EN_LastName,
                    FullNameTH = u.Detail_TH_FirstName + " " + u.Detail_TH_LastName,
                    UserCode = u.Users.User_Code
                }).ToListAsync();

            // Create an instance of EmailAddressAttribute to use for validation
            var emailValidator = new EmailAddressAttribute();

            // Then filter them in-memory using the custom method and the email validator
            var validEmails = userDetails.Where(u => emailValidator.IsValid(u.Email) && clsManage.EmailExistsInAD(u.Email)).ToList();

            return validEmails;
        }

        private string FormatEmailBody(string header, string body)
        {
            return $"<html><head></head><body><p><b>{header}</b></p>{body}</body></html>";
        }

        private async Task<UserDetails> GetUserDetails(string userId)
        {
            return await db.UserDetails.FirstOrDefaultAsync(u => u.User_Id.ToString() == userId);
        }

        //API Complete
        public class ReceiveData
        {
            public string Email { get; set; }
            public string FullNameEN { get; set; }
            public string FullNameTH { get; set; }
            public string NameEN { get; set; }
            public string NameTH { get; set; }
            public string UserCode { get; set; }
        }
    }
}
