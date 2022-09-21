﻿using E2E.Models.Tables;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace E2E.Models
{
    public class clsMail
    {
        private clsContext db = new clsContext();

        private bool SendMail(MailMessage model)
        {
            try
            {
                bool res = new bool();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                              | SecurityProtocolType.Tls11
                              | SecurityProtocolType.Tls12;

                using (SmtpClient client = new SmtpClient())
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Mail"], ConfigurationManager.AppSettings["Mail_Password"], ConfigurationManager.AppSettings["Mail_Domain"]);
                    client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Mail_Port"]); // You can use Port 25 for online, Port 587 is for local
                    client.Host = ConfigurationManager.AppSettings["Mail_Host"];
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = true;
                    client.Send(model);
                    res = true;
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string Content { get; set; }
        public List<Guid?> SendBCC { get; set; }
        public List<Guid?> SendCC { get; set; }
        public Guid SendFrom { get; set; }
        public List<Guid> SendTo { get; set; }
        public string Subject { get; set; }

        public bool ResendMail(Guid logId)
        {
            try
            {
                bool res = new bool();
                Log_SendEmail log_SendEmail = new Log_SendEmail();
                log_SendEmail = db.Log_SendEmails.Find(logId);

                List<Guid> mailTo = new List<Guid>();
                List<Guid?> mailCC = new List<Guid?>();
                List<Guid?> mailBCC = new List<Guid?>();

                List<Log_SendEmailTo> log_SendEmailTos = new List<Log_SendEmailTo>();
                log_SendEmailTos = db.Log_SendEmailTos
                    .Where(w => w.SendEmail_Id == logId)
                    .ToList();
                foreach (var item in log_SendEmailTos)
                {
                    switch (item.SendEmailTo_Type)
                    {
                        case "to":
                            mailTo.Add(item.User_Id);
                            break;

                        case "cc":
                            mailCC.Add(item.User_Id);
                            break;

                        case "bcc":
                            mailBCC.Add(item.User_Id);
                            break;

                        default:
                            break;
                    }
                }

                res = SendMail(mailTo, log_SendEmail.SendEmail_Subject, log_SendEmail.SendEmail_Content, mailCC, mailBCC);

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SendMail(List<Guid> sendTo, string strSubject, string strContent, List<Guid?> sendCC = null, List<Guid?> sendBCC = null, List<string> attachPaths = null)
        {
            try
            {
                MailMessage msg = new MailMessage();

                string dear = "Dear ";
                List<ReceiveData> receiveDatas = db.UserDetails
                    .Where(w => sendTo.Contains(w.User_Id))
                    .Select(s => new ReceiveData()
                    {
                        Email = s.Users.User_Email,
                        NameEN = s.Detail_EN_FirstName,
                        NameTH = s.Detail_TH_FirstName,
                        FullNameEN = s.Detail_EN_FirstName + " " + s.Detail_EN_LastName,
                        FullNameTH = s.Detail_TH_FirstName + " " + s.Detail_TH_LastName
                    }).ToList();

                for (int i = 0; i < receiveDatas.Count; i++)
                {
                    if (i != 0)
                    {
                        dear += ", ";
                    }

                    dear += receiveDatas[i].NameEN;
                }

                foreach (var item in receiveDatas.Where(w => !string.IsNullOrEmpty(w.Email)))
                {
                    var email = new EmailAddressAttribute();
                    if (email.IsValid(item.Email))
                    {
                        msg.To.Add(new MailAddress(item.Email, item.FullNameEN));
                    }
                }

                if (msg.To.Count == 0)
                {
                    return true;
                }

                if (sendCC != null)
                {
                    List<ReceiveData> receiveCC = db.UserDetails
                    .Where(w => sendCC.Contains(w.User_Id))
                    .Select(s => new ReceiveData()
                    {
                        Email = s.Users.User_Email,
                        NameEN = s.Detail_EN_FirstName,
                        NameTH = s.Detail_TH_FirstName,
                        FullNameEN = s.Detail_EN_FirstName + " " + s.Detail_EN_LastName,
                        FullNameTH = s.Detail_TH_FirstName + " " + s.Detail_TH_LastName
                    }).ToList();

                    foreach (var item in receiveCC.Where(w => !string.IsNullOrEmpty(w.Email)))
                    {
                        var email = new EmailAddressAttribute();
                        if (email.IsValid(item.Email))
                        {
                            msg.CC.Add(new MailAddress(item.Email, item.FullNameEN));
                        }
                    }
                }

                if (sendBCC != null)
                {
                    List<ReceiveData> receiveBCC = db.UserDetails
                    .Where(w => sendBCC.Contains(w.User_Id))
                    .Select(s => new ReceiveData()
                    {
                        Email = s.Users.User_Email,
                        NameEN = s.Detail_EN_FirstName,
                        NameTH = s.Detail_TH_FirstName,
                        FullNameEN = s.Detail_EN_FirstName + " " + s.Detail_EN_LastName,
                        FullNameTH = s.Detail_TH_FirstName + " " + s.Detail_TH_LastName
                    }).ToList();

                    foreach (var item in receiveBCC.Where(w => !string.IsNullOrEmpty(w.Email)))
                    {
                        var email = new EmailAddressAttribute();
                        if (email.IsValid(item.Email))
                        {
                            msg.Bcc.Add(new MailAddress(item.Email, item.FullNameEN));
                        }
                    }
                }

                if (attachPaths != null)
                {
                    foreach (var item in attachPaths)
                    {
                        System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(item, MediaTypeNames.Application.Octet);

                        msg.Attachments.Add(attachment);
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
                strBody += strContent;
                if (receiveDatas.Where(w => string.IsNullOrEmpty(w.Email)).Count() > 0)
                {
                    string warningContent = "Can't send email to: ";
                    List<ReceiveData> noEmail = new List<ReceiveData>();
                    noEmail = receiveDatas.Where(w => string.IsNullOrEmpty(w.Email)).ToList();
                    for (int i = 0; i < noEmail.Count; i++)
                    {
                        if (i == 0)
                        {
                            warningContent += noEmail[i].FullNameEN;
                        }
                        else
                        {
                            warningContent += ", " + noEmail[i].FullNameEN;
                        }
                    }

                    warningContent += " because there's no email.";
                    strBody += warningContent;
                }
                strBody += "</body>";
                strBody += "</html>";

                msg.From = new MailAddress(ConfigurationManager.AppSettings["Mail"]);
                msg.Subject = strSubject;
                msg.Body = new MessageBody(BodyType.HTML, strBody);
                msg.IsBodyHtml = true;

                return SendMail(msg);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SendMail(Guid sendTo, string strSubject, string strContent, List<Guid?> sendCC = null, List<Guid?> sendBCC = null, List<string> attachPaths = null)
        {
            try
            {
                MailMessage msg = new MailMessage();

                string dear = "Dear ";
                ReceiveData receiveDatas = db.UserDetails
                    .Where(w => w.User_Id == sendTo)
                    .Select(s => new ReceiveData()
                    {
                        Email = s.Users.User_Email,
                        NameEN = s.Detail_EN_FirstName,
                        NameTH = s.Detail_TH_FirstName,
                        FullNameEN = s.Detail_EN_FirstName + " " + s.Detail_EN_LastName,
                        FullNameTH = s.Detail_TH_FirstName + " " + s.Detail_TH_LastName
                    }).FirstOrDefault();

                if (string.IsNullOrEmpty(receiveDatas.Email))
                {
                    return true;
                }

                var email = new EmailAddressAttribute();
                if (!email.IsValid(receiveDatas.Email))
                {
                    return true;
                }

                msg.To.Add(new MailAddress(receiveDatas.Email, receiveDatas.FullNameEN));

                dear += receiveDatas.NameEN;

                if (sendCC != null)
                {
                    List<ReceiveData> receiveCC = db.UserDetails
                    .Where(w => sendCC.Contains(w.User_Id))
                    .Select(s => new ReceiveData()
                    {
                        Email = s.Users.User_Email,
                        NameEN = s.Detail_EN_FirstName,
                        NameTH = s.Detail_TH_FirstName,
                        FullNameEN = s.Detail_EN_FirstName + " " + s.Detail_EN_LastName,
                        FullNameTH = s.Detail_TH_FirstName + " " + s.Detail_TH_LastName
                    }).ToList();

                    foreach (var item in receiveCC)
                    {
                        email = new EmailAddressAttribute();
                        if (email.IsValid(item.Email))
                        {
                            msg.CC.Add(new MailAddress(item.Email, item.FullNameEN));
                        }
                    }
                }

                if (sendBCC != null)
                {
                    List<ReceiveData> receiveBCC = db.UserDetails
                    .Where(w => sendBCC.Contains(w.User_Id))
                    .Select(s => new ReceiveData()
                    {
                        Email = s.Users.User_Email,
                        NameEN = s.Detail_EN_FirstName,
                        NameTH = s.Detail_TH_FirstName,
                        FullNameEN = s.Detail_EN_FirstName + " " + s.Detail_EN_LastName,
                        FullNameTH = s.Detail_TH_FirstName + " " + s.Detail_TH_LastName
                    }).ToList();

                    foreach (var item in receiveBCC)
                    {
                        email = new EmailAddressAttribute();
                        if (email.IsValid(item.Email))
                        {
                            msg.Bcc.Add(new MailAddress(item.Email, item.FullNameEN));
                        }
                    }
                }

                if (attachPaths != null)
                {
                    foreach (var item in attachPaths)
                    {
                        System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(item, MediaTypeNames.Application.Octet);

                        msg.Attachments.Add(attachment);
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
                strBody += strContent;
                strBody += "</body>";
                strBody += "</html>";

                msg.From = new MailAddress(ConfigurationManager.AppSettings["Mail"]);
                msg.Subject = strSubject;
                msg.Body = new MessageBody(BodyType.HTML, strBody);
                msg.IsBodyHtml = true;

                return SendMail(msg);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SendMail(List<string> sendTo, string subject, string content, List<string> attachPaths = null)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(ConfigurationManager.AppSettings["Mail"]);
                foreach (var item in sendTo)
                {
                    var email = new EmailAddressAttribute();
                    if (email.IsValid(item))
                    {
                        msg.To.Add(item);
                    }
                }

                msg.Subject = subject;

                string strBody = "<html>";
                strBody += "<head>";
                strBody += "</head>";
                strBody += "<body>";
                strBody += content;
                strBody += "</body>";
                strBody += "</html>";

                if (attachPaths != null)
                {
                    foreach (var item in attachPaths)
                    {
                        System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(item, MediaTypeNames.Application.Octet);

                        msg.Attachments.Add(attachment);
                    }
                }

                msg.Body = new MessageBody(BodyType.HTML, strBody);
                msg.IsBodyHtml = true;

                return SendMail(msg);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SendMail(List<string> sendTo, string subject, string content, Guid sendCC, List<string> attachPaths = null)
        {
            try
            {
                MailMessage msg = new MailMessage();

                msg.From = new MailAddress(ConfigurationManager.AppSettings["Mail"]);

                var email = new EmailAddressAttribute();
                foreach (var item in sendTo)
                {
                    email = new EmailAddressAttribute();
                    if (email.IsValid(item))
                    {
                        msg.To.Add(item);
                    }
                }

                string emailCC = db.Users.Find(sendCC).User_Email;
                email = new EmailAddressAttribute();
                if (email.IsValid(emailCC))
                {
                    msg.CC.Add(emailCC);
                }

                msg.Subject = subject;

                string strBody = "<html>";
                strBody += "<head>";
                strBody += "</head>";
                strBody += "<body>";
                strBody += content;
                strBody += "</body>";
                strBody += "</html>";

                if (attachPaths != null)
                {
                    foreach (var item in attachPaths)
                    {
                        System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(item, MediaTypeNames.Application.Octet);

                        msg.Attachments.Add(attachment);
                    }
                }

                msg.Body = new MessageBody(BodyType.HTML, strBody);
                msg.IsBodyHtml = true;

                return SendMail(msg);
            }
            catch (Exception)
            {
                throw;
            }
        }

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
