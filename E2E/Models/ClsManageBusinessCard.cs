using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models
{
    public class ClsManageBusinessCard
    {
        private readonly ClsContext db = new ClsContext();
        ClsMail mail = new ClsMail();
        private readonly ClsManageMaster master = new ClsManageMaster();
        public bool BusinessCard_SaveCreate(ClsBusinessCard Model)
        {
            bool res = new bool();
            try
            {
                Guid MyUserID = Guid.Parse(HttpContext.Current.User.Identity.Name);

                BusinessCards businessCards = new BusinessCards();

                businessCards.Amount = Model.Amount;
                businessCards.BothSided = Model.BothSided;
                businessCards.Key = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                businessCards.Tel_External = Model.Tel_External;
                businessCards.Tel_Internal = Model.Tel_Internal;
                businessCards.User_id = Model.User_id.Value;
                businessCards.Status_Id = 1; //Pending

                //เช็คว่ามีการ Create แทนกันไหม
                if (businessCards.User_id != MyUserID)
                {
                    businessCards.UserRef_id = MyUserID;
                }


                db.BusinessCards.Add(businessCards);

                if (db.SaveChanges() > 0)
                {
                    res = BusinessCard_SaveLog(businessCards);
                    SendMail_MgUser(businessCards);
                }

            }
            catch (Exception ex)
            {

                throw;
            }

            return res;
        }

        public bool BusinessCard_SaveLog(BusinessCards Model)
        {
            bool res = new bool();
            Log_BusinessCards log_BusinessCards = new Log_BusinessCards();

            log_BusinessCards.BusinessCard_Id = Model.BusinessCard_Id;
            log_BusinessCards.Status_Id = Model.Status_Id;
            log_BusinessCards.User_Id = Model.User_id;
            log_BusinessCards.Create = Model.Create;


            db.Log_BusinessCards.Add(log_BusinessCards);

            if (db.SaveChanges() > 0)
            {
                res = true;
            }

            return res;
        }

        public bool SendMail_MgUser(BusinessCards Model)
        {
            bool res = new bool();

            Guid DeptId = db.Users.Where(w => w.User_Id == Model.User_id).Select(s => s.Master_Processes.Master_Sections.Master_Departments.Department_Id).FirstOrDefault();
            var GetMgApp = db.Users.Where(w => w.Master_Processes.Master_Sections.Department_Id == DeptId && w.Master_Grades.Master_LineWorks.Authorize_Id == 2).Select(s => s.User_Id).ToList();

            var linkUrl = HttpContext.Current.Request.Url.OriginalString;
            linkUrl = linkUrl.Replace("BusinessCard_Create", "BusinessCard_Detail/" + Model.BusinessCard_Id);


            string subject = string.Format("[Business Card][Require approve] {0}", Model.Key);
            string content = "<p>Request Business Card";
            content += "<br/>";
            content += string.Format("<b>Requester:</b> {0}", master.Users_GetInfomation(Model.User_id));
            content += "<br/>";
            content += string.Format("<b>Amount:</b> {0} pcs.", Model.Amount);
            content += "</p>";
            content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
            content += "<p>Thank you for your consideration</p>";
            mail.SendToIds = GetMgApp;
            mail.SendFrom = Model.User_id;
            mail.Subject = subject;

            if (Model.UserRef_id.HasValue)
            {
                mail.SendCC = Model.UserRef_id;
            }

            mail.Body = content;
            res = mail.SendMail(mail);

            return res;
        }
    }
}