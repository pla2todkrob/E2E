using E2E.Models.Tables;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace E2E.Models
{
    public class ClsMail
    {
        private readonly ClsApi clsApi = new ClsApi();
        private readonly ClsContext db = new ClsContext();

        public ClsMail()
        {
            SendToIds = new List<Guid>();
            SendToStrs = new List<string>();
            AttachPaths = new List<string>();
            SendBCC = new List<Guid>();
            SendCCs = new List<Guid>();
        }

        public List<string> AttachPaths { get; set; }
        public string Body { get; set; }
        public List<Guid> SendBCC { get; set; }
        public Guid? SendCC { get; set; }
        public List<Guid> SendCCs { get; set; }
        public Guid SendFrom { get; set; }
        public Guid? SendToId { get; set; }
        public List<Guid> SendToIds { get; set; }
        public string SendToStr { get; set; }
        public List<string> SendToStrs { get; set; }
        public string Subject { get; set; }

        public bool ResendMail(Guid logId)
        {
            try
            {
                bool res = new bool();
                Log_SendEmail log_SendEmail = new Log_SendEmail();
                log_SendEmail = db.Log_SendEmails.Find(logId);

                ClsMail clsMail = new ClsMail();

                List<Log_SendEmailTo> log_SendEmailTos = new List<Log_SendEmailTo>();
                log_SendEmailTos = db.Log_SendEmailTos
                    .Where(w => w.SendEmail_Id == logId)
                    .ToList();
                foreach (var item in log_SendEmailTos)
                {
                    switch (item.SendEmailTo_Type)
                    {
                        case "to":
                            clsMail.SendToIds.Add(item.User_Id);
                            break;

                        case "cc":
                            clsMail.SendCCs.Add(item.User_Id);
                            break;

                        case "bcc":
                            clsMail.SendBCC.Add(item.User_Id);
                            break;

                        default:
                            break;
                    }
                }

                clsMail.Subject = log_SendEmail.SendEmail_Subject;
                clsMail.Body = log_SendEmail.SendEmail_Content;

                res = SendMail(clsMail);

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SendMail(ClsMail model, HttpFileCollectionBase files = null)
        {
            try
            {
                EmailAddressAttribute attribute = new EmailAddressAttribute();
                ClsServiceEmail clsServiceEmail = new ClsServiceEmail();
                string dear = "Dear ";

                if (model.SendToIds.Count > 0)
                {
                    List<ReceiveData> receiveDatas = db.UserDetails
                    .Where(w => model.SendToIds.Contains(w.User_Id))
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
                }
                else if (model.SendToId.HasValue)
                {
                    ReceiveData receiveData = db.UserDetails
                    .Where(w => w.User_Id == model.SendToId)
                    .Select(s => new ReceiveData()
                    {
                        Email = s.Users.User_Email,
                        NameEN = s.Detail_EN_FirstName,
                        NameTH = s.Detail_TH_FirstName,
                        FullNameEN = s.Detail_EN_FirstName + " " + s.Detail_EN_LastName,
                        FullNameTH = s.Detail_TH_FirstName + " " + s.Detail_TH_LastName
                    }).FirstOrDefault();

                    if (receiveData == null)
                    {
                        return true;
                    }
                    else if (string.IsNullOrEmpty(receiveData.Email))
                    {
                        return true;
                    }
                    else
                    {
                        attribute = new EmailAddressAttribute();
                        if (attribute.IsValid(receiveData.Email))
                        {
                            dear += receiveData.FullNameEN;
                            clsServiceEmail.SendTo = new string[] { receiveData.Email };
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(model.SendToStr))
                {
                    attribute = new EmailAddressAttribute();
                    if (attribute.IsValid(model.SendToStr))
                    {
                        clsServiceEmail.SendTo = new string[] { model.SendToStr };
                    }
                }
                else if (model.SendToStrs.Count > 0)
                {
                    List<string> EmailTos = new List<string>();
                    foreach (var item in model.SendToStrs)
                    {
                        attribute = new EmailAddressAttribute();
                        if (attribute.IsValid(item))
                        {
                            EmailTos.Add(item);
                        }
                    }

                    if (EmailTos.Count > 0)
                    {
                        clsServiceEmail.SendTo = EmailTos.ToArray();
                    }
                }
                else
                {
                    return true;
                }

                if (clsServiceEmail.SendTo.Length == 0)
                {
                    return true;
                }

                if (model.SendCCs.Count > 0)
                {
                    List<ReceiveData> receiveDatas = db.UserDetails
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
                else if (model.SendCC.HasValue)
                {
                    ReceiveData receiveData = db.UserDetails
                    .Where(w => model.SendCCs.Contains(w.User_Id))
                    .Select(s => new ReceiveData()
                    {
                        Email = s.Users.User_Email,
                        NameEN = s.Detail_EN_FirstName,
                        NameTH = s.Detail_TH_FirstName,
                        FullNameEN = s.Detail_EN_FirstName + " " + s.Detail_EN_LastName,
                        FullNameTH = s.Detail_TH_FirstName + " " + s.Detail_TH_LastName
                    }).FirstOrDefault();

                    if (receiveData != null && !string.IsNullOrEmpty(receiveData.Email))
                    {
                        attribute = new EmailAddressAttribute();
                        if (attribute.IsValid(receiveData.Email))
                        {
                            clsServiceEmail.SendCC = new string[] { receiveData.Email };
                        }
                    }
                }

                if (model.SendBCC.Count > 0)
                {
                    List<ReceiveData> receiveDatas = db.UserDetails
                    .Where(w => model.SendBCC.Contains(w.User_Id))
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

                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);

                UserDetails userDetails = new UserDetails();
                userDetails = db.UserDetails.Where(w => w.User_Id == userId).FirstOrDefault();
                string sendFrom = string.Format("<p>Send from: <a href='mailto:{0}'>{1} {2} <{3}></a></p>", userDetails.Users.User_Email, userDetails.Detail_EN_FirstName, userDetails.Detail_EN_LastName, userDetails.Users.User_Email);

                string strBody = "<html>";
                strBody += "<head>";
                strBody += "</head>";
                strBody += "<body>";
                strBody += string.Format("<p><b>{0}</b></p>", dear);
                strBody += sendFrom;
                strBody += model.Body;

                strBody += "</body>";
                strBody += "</html>";

                clsServiceEmail.Body = strBody;
                clsServiceEmail.Subject = model.Subject;
                clsServiceEmail.SendFrom = userDetails.Users.User_Email;

                return clsApi.SendMail(clsServiceEmail, files);
            }
            catch (Exception)
            {
                throw;
            }
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
