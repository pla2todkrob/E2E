using E2E.Models.Tables;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace E2E.Models
{
    public class clsMail
    {
        private clsContext db = new clsContext();
        public Guid SendFrom { get; set; }
        public List<Guid> SendTo { get; set; }
        public List<Guid?> SendCC { get; set; }
        public List<Guid?> SendBCC { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public bool SendMail(List<Guid> sendTo, string strSubject, string strContent, List<Guid?> sendCC = null, List<Guid?> sendBCC = null)
        {
            try
            {
                bool res = new bool();
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
                        msg.CC.Add(new MailAddress(item.Email, item.FullNameEN));
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
                        msg.Bcc.Add(new MailAddress(item.Email, item.FullNameEN));
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

                foreach (var item in receiveDatas)
                {
                    msg.To.Add(new MailAddress(item.Email, item.FullNameEN));
                }

                msg.From = new MailAddress(ConfigurationManager.AppSettings["Mail"]);
                msg.Subject = strSubject;
                msg.Body = new MessageBody(BodyType.HTML, strBody);
                msg.IsBodyHtml = true;

                //ใช้ในกรณี ส่งเมลไม่ออกทั้งที่ Code ถูกต้องหมดทุกอย่าง
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                              | SecurityProtocolType.Tls11
                              | SecurityProtocolType.Tls12;

                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Mail"], ConfigurationManager.AppSettings["Mail_Password"]);
                client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Mail_Port"]); // You can use Port 25 for online, Port 587 is for local
                client.Host = ConfigurationManager.AppSettings["Mail_Host"];
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                try
                {
                    client.Send(msg);
                    res = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool SendMail(Guid sendTo, string strSubject, string strContent, List<Guid?> sendCC = null, List<Guid?> sendBCC = null)
        {
            try
            {
                bool res = new bool();
                MailMessage msg = new MailMessage();

                string dear = "Dear ";
                List<ReceiveData> receiveDatas = db.UserDetails
                    .Where(w => w.User_Id == sendTo)
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
                        msg.CC.Add(new MailAddress(item.Email, item.FullNameEN));
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
                        msg.Bcc.Add(new MailAddress(item.Email, item.FullNameEN));
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

                foreach (var item in receiveDatas)
                {
                    msg.To.Add(new MailAddress(item.Email, item.FullNameEN));
                }

                msg.From = new MailAddress(ConfigurationManager.AppSettings["Mail"]);
                msg.Subject = strSubject;
                msg.Body = new MessageBody(BodyType.HTML, strBody);
                msg.IsBodyHtml = true;

                //ใช้ในกรณี ส่งเมลไม่ออกทั้งที่ Code ถูกต้องหมดทุกอย่าง
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                              | SecurityProtocolType.Tls11
                              | SecurityProtocolType.Tls12;

                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Mail"], ConfigurationManager.AppSettings["Mail_Password"]);
                client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Mail_Port"]); // You can use Port 25 for online, Port 587 is for local
                client.Host = ConfigurationManager.AppSettings["Mail_Host"];
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                try
                {
                    client.Send(msg);
                    res = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public class ReceiveData
        {
            public string Email { get; set; }
            public string NameEN { get; set; }
            public string NameTH { get; set; }
            public string FullNameEN { get; set; }
            public string FullNameTH { get; set; }
        }
    }
}