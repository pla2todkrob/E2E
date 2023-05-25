using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace E2E.Models
{
    public class ClsManageBusinessCard
    {
        private readonly ClsContext db = new ClsContext();
        private readonly ClsMail mail = new ClsMail();
        private readonly ClsManageMaster master = new ClsManageMaster();

        public int GradeNumber(ClsBusinessCard Model)
        {

            var BypassGA = db.Users.Where(w => w.User_Id == Model.User_id).Select(s => s.Master_Grades.Grade_Name).FirstOrDefault();

            int Num = Convert.ToInt32(BypassGA.Substring(1));

            return Num;
        }

        public bool BusinessCard_SaveCreate(ClsBusinessCard Model)
        {
            bool res = new bool();
            try
            {
                string JP = string.Empty;
                Guid MyUserID = Guid.Parse(HttpContext.Current.User.Identity.Name);

                string ChkJP = db.Users.Where(w => w.User_Id == Model.User_id).Select(s => s.Master_Grades.Master_LineWorks.LineWork_Name).FirstOrDefault();
                string Year = DateTime.Now.ToString("yyyy");
                var CountNum = db.BusinessCards.Where(w => w.Key.ToString().StartsWith(Year)).Count()+1;



                Year += CountNum.ToString("0000");
                BusinessCards businessCards = new BusinessCards
                {
                    Amount = Model.Amount,
                    BothSided = Model.BothSided,
                    Key = long.Parse(Year),
                    Tel_External = Model.Tel_External,
                    Tel_Internal = Model.Tel_Internal,
                    User_id = Model.User_id.Value,
                    Status_Id = 1, //Pending
                };

                if (GradeNumber(Model) <= 4)
                {
                    businessCards.Status_Id = 7; //Approved MG User

                    DateTime currentDate = DateTime.Now;
                    int daysToAdd = 3;

                    while (daysToAdd > 0)
                    {
                        currentDate = currentDate.AddDays(1);

                        if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                        {
                            daysToAdd--;
                        }
                    }

                    businessCards.DueDate = currentDate;
                }

                //เช็คว่ามีการ Create แทนกันไหม
                if (businessCards.User_id != MyUserID)
                {
                    businessCards.UserRef_id = MyUserID;
                }

                db.BusinessCards.Add(businessCards);

                if (db.SaveChanges() > 0)
                {
                    res = BusinessCard_SaveLog(businessCards);
                    SendMail(businessCards);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool BusinessCard_SaveLog(BusinessCards Model,string remark = "",bool undo = false)
        {
            bool res = new bool();
            DateTime dateTime = new DateTime();
            if (Model.Update.HasValue)
            {
                dateTime = Model.Update.Value;
            }
            else
            {
                dateTime = DateTime.Now;
            }

            Log_BusinessCards log_BusinessCards = new Log_BusinessCards
            {
                BusinessCard_Id = Model.BusinessCard_Id,
                Status_Id = Model.Status_Id,
                User_Id = Model.Status_Id == 1 ? Model.User_id: Guid.Parse(HttpContext.Current.User.Identity.Name),
                Create = dateTime,
                Undo = undo
            };

            if (!string.IsNullOrEmpty(remark))
            {
                log_BusinessCards.Remark = remark;
            }

            db.Log_BusinessCards.Add(log_BusinessCards);

            if (db.SaveChanges() > 0)
            {
                res = true;
            }

            return res;
        }

        public List<SelectListItem> SelectListItems_CardGroup()
        {
            try
            {
                return db.UserDetails
                .Where(w => w.Users.Active && w.Users.BusinessCardGroup == true && w.Users.Master_Grades.Master_LineWorks.Authorize_Id == 3)
                .OrderBy(o => o.Users.User_Code)
                .Select(s => new SelectListItem()
                {
                    Value = s.User_Id.ToString(),
                    Text = s.Users.User_Code + " [" + s.Detail_EN_FirstName + " " + s.Detail_EN_LastName + "][" + s.Users.User_Point + "]"
                }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SendMail(BusinessCards Model, Guid? SelectId = null, BusinessCardFiles ModelFile = null, string filepath = "", string remark = "", string pseudo = "")
        {
            bool res = new bool();

            Guid DeptId = db.Users.Where(w => w.User_Id == Model.User_id).Select(s => s.Master_Processes.Master_Sections.Master_Departments.Department_Id).FirstOrDefault();
            var GetMgApp = db.Users.Where(w => w.Master_Processes.Master_Sections.Department_Id == DeptId && w.Master_Grades.Master_LineWorks.Authorize_Id == 2).Select(s => s.User_Id).ToList();

            var linkUrl = HttpContext.Current.Request.Url.OriginalString;
            bool found = linkUrl.Contains("Upload");
            linkUrl = linkUrl.Replace("BusinessCard_Create", "BusinessCard_Detail/" + Model.BusinessCard_Id);

            string subject = string.Format("[Business Card][Require approve] {0}", Model.Key);

            string content = "<p>Request Business Card";
            content += "<br/>";
            content += string.Format("<b>Requester:</b> {0}", master.Users_GetInfomation(Model.User_id));
            content += "<br/>";
            content += string.Format("<b>Amount:</b> {0} pcs.", Model.Amount);
            if (Model.DueDate.HasValue)
            {
                content += "<br/>";
                content += string.Format("<b>Due date:</b> {0}", Model.DueDate.Value.ToString("D"));
            }

            mail.SendToIds = GetMgApp;
            mail.SendFrom = Model.User_id;
            mail.Subject = subject;

            if (Model.UserRef_id.HasValue)
            {
                mail.SendCC = Model.UserRef_id;
            }

            //Mg User Approved
            if (Model.Status_Id == 7 && string.IsNullOrEmpty(pseudo))
            {
                linkUrl = linkUrl.Replace("ManagerUserApprove", "BusinessCard_Detail/");

                GetMgApp = db.Users.Where(w => w.BusinessCardGroup == true && w.Master_Grades.Master_LineWorks.Authorize_Id == 2).Select(s => s.User_Id).ToList();
                mail.SendToIds = GetMgApp;
                mail.SendCC = Model.User_id;
            }
            //Staff Undo
            else if (Model.Status_Id == 7 && pseudo == "7")
            {

                string keyword = "BusinessCards";
                string pattern = $"{keyword}.*";
                string result = Regex.Replace(linkUrl, pattern, keyword);
                result = result + "/BusinessCard_Detail/" + Model.BusinessCard_Id;
                linkUrl = result;

                subject = string.Format("[Business Card][Staff Undo] {0}", Model.Key);
                
                content += string.Format("<p>Undo remark: {0}</p>", remark);

                mail.Subject = subject;
                GetMgApp = db.Users.Where(w => w.BusinessCardGroup == true && w.Master_Grades.Master_LineWorks.Authorize_Id == 2).Select(s => s.User_Id).ToList();
                mail.SendToIds = GetMgApp;
            }

            //User Undo
            else if (Model.Status_Id == 2 && pseudo == "2")
            {
                string keyword = "BusinessCards";
                string pattern = $"{keyword}.*";
                string result = Regex.Replace(linkUrl, pattern, keyword);
                result = result + "/BusinessCard_Detail/" + Model.BusinessCard_Id;
                linkUrl = result;
             

                subject = string.Format("[Business Card][Requester Undo] {0}", Model.Key);

                content += string.Format("<p>Comment: {0}</p>", remark);
                mail.SendToIds.Clear();
                mail.Subject = subject;
                mail.SendToId = Model.UserAction;
            }

            //Rejected
            else if (Model.Status_Id == 5)
            {

                string keyword = "BusinessCards";
                string pattern = $"{keyword}.*";
                string result = Regex.Replace(linkUrl, pattern, keyword);
                result = result + "/BusinessCard_Detail/" + Model.BusinessCard_Id;
                linkUrl = result;

                Guid ActionId = Guid.Parse(HttpContext.Current.User.Identity.Name);

                subject = string.Format("[Business Card][Rejected] {0}", Model.Key);
                content = string.Format("<p>Comment: {0}", remark);
                mail.SendToId = Model.User_id;
                mail.SendFrom = ActionId;
                mail.Subject = subject;
                mail.SendToIds.Clear();

                var ChkMgUserRejected = GetMgApp.Any(a => a == ActionId);

                if (!ChkMgUserRejected)
                {
                    mail.SendBCC = GetMgApp;
                }

            }
            //[M] GA Assign
            else if (Model.Status_Id == 8)
            {
                linkUrl = linkUrl.Replace("ManagerGaApprove", "BusinessCard_Detail/");


                Guid ActionId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                List<Users> users = db.Users.Where(w => w.BusinessCardGroup == true && w.Master_Grades.Master_LineWorks.Authorize_Id == 3).ToList();
                if (SelectId.HasValue)
                {
                    mail.SendToId = SelectId.Value;
                    mail.SendToIds.Clear();
                }
                else
                {
                    mail.SendToIds = users.Where(w => !w.Master_Grades.Grade_Name.Contains("6")).Select(s => s.User_Id).ToList();
                }


                subject = string.Format("[Business Card][Assign] {0}", Model.Key);
                content = "<p>Comment: Assign task to Department General Affair";

                mail.SendFrom = ActionId;
                mail.Subject = subject;

                //CC Email Grade 5 or 6
                if (users.Any(w => w.Master_Grades.Grade_Name.Contains("6")) || users.Any(w => w.Master_Grades.Grade_Name.Contains("5")))
                {
                    mail.SendCCs = users.Where(w => w.Master_Grades.Grade_Name.Contains("6") || w.Master_Grades.Grade_Name.Contains("5")).Select(s => s.User_Id).ToList();
                }
           

            }

            //Staff Send Confirm
            else if (Model.Status_Id == 2 && ModelFile == null || found)
            {
                linkUrl = linkUrl.Replace("Upload", "BusinessCard_Detail/" + Model.BusinessCard_Id);


                Guid ActionId = Guid.Parse(HttpContext.Current.User.Identity.Name);

                subject = string.Format("[Business Card][Please Confirm] {0}", Model.Key);
                content += string.Format("<p>Please confirm, check the correctness of the business card.</p>");
                mail.SendToId = Model.User_id;
                mail.SendToIds.Clear();
                mail.SendFrom = ActionId;
                mail.Subject = subject;
                mail.SendCC = null;
                mail.AttachPaths.Add(filepath);

            }
                
            //User Confirm
            else if (Model.Status_Id == 9)
            {
                string keyword = "BusinessCards";
                string pattern = $"{keyword}.*";
                string result = Regex.Replace(linkUrl, pattern, keyword);
                result = result + "/BusinessCard_Detail/" + Model.BusinessCard_Id;
                linkUrl = result;

                Guid ActionId = Guid.Parse(HttpContext.Current.User.Identity.Name);


                subject = string.Format("[Business Card][Requester Confirm] {0}", Model.Key);
                mail.SendToId = Model.UserAction;
                mail.SendToIds.Clear();
                mail.SendFrom = ActionId;
                mail.Subject = subject;
                mail.SendCC = null;
            }

            //User Cancel Confirm
            else if (Model.Status_Id == 2 && found == false)
            {

                string keyword = "BusinessCards";
                string pattern = $"{keyword}.*";
                string result = Regex.Replace(linkUrl, pattern, keyword);
                result = result + "/BusinessCard_Detail/" + Model.BusinessCard_Id;
                linkUrl = result;

                Guid ActionId = Guid.Parse(HttpContext.Current.User.Identity.Name);

                content = string.Empty;
                subject = string.Format("[Business Card][Cancel Confirm {1}] {0}", Model.Key, ModelFile.FileName);
                content = string.Format("<p>Requester Comment: {0}", remark);
                
                mail.SendToId = Model.UserAction;
                mail.SendToIds.Clear();
                mail.SendFrom = ActionId;
                mail.Subject = subject;
                mail.SendCC = null;
            }

            //User Close
            else if (Model.Status_Id == 4)
            {
                linkUrl = linkUrl.Replace("UserClose", "BusinessCard_Detail/");


                Guid ActionId = Guid.Parse(HttpContext.Current.User.Identity.Name);

                content = string.Empty;
                subject = string.Format("[Business Card][Closed] {0}", Model.Key);
                mail.SendToId = Model.UserAction;
                mail.SendToIds.Clear();
                mail.SendFrom = ActionId;
                mail.Subject = subject;
                mail.SendCC = null;
            }

            //Staff Completed
            else if (Model.Status_Id == 3)
            {
                linkUrl = linkUrl.Replace("StaffComplete", "BusinessCard_Detail/");

                Guid ActionId = Guid.Parse(HttpContext.Current.User.Identity.Name);

                content = string.Empty;
                subject = string.Format("[Business Card][Please Close] {0}", Model.Key);
                
                mail.SendToId = Model.User_id;
                mail.SendToIds.Clear();
                mail.SendFrom = ActionId;
                mail.Subject = subject;
                mail.SendCC = null;
            }

            content += "</p>";
            content += string.Format("<a href='{0}' target='_blank'>Please, click here to more detail.</a>", linkUrl);
            content += "<p>Thank you for your consideration</p>";
            mail.Body = content;
            res = mail.SendMail(mail);

            return res;
        }

        public bool BusinessCard_SaveFile(string filepath, BusinessCards model)
        {
            bool res = new bool();

            try
            {
                BusinessCardFiles cardFiles = new BusinessCardFiles
                {
                    BusinessCard_Id = model.BusinessCard_Id,
                    Create = DateTime.Now,
                    Extension = Path.GetExtension(filepath),
                    FilePath = filepath,
                    FileName = Path.GetFileName(filepath)
                };

                db.BusinessCardFiles.Add(cardFiles);

                if (db.SaveChanges() > 0)
                {
                    res = SendMail(model, null, cardFiles, filepath);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return res;
        }

        public int CountJob(Guid? id)
        {
            int jobCount = 0;

            if (id.HasValue)
            {
                var authorIndex = db.Users.Where(w => w.User_Id == id).Select(s => new { s.Master_Grades.Master_LineWorks.Authorize_Id, s.Master_Processes.Master_Sections.Department_Id, s.Role_Id });
                int author = authorIndex.Select(s => s.Authorize_Id).FirstOrDefault();
                var ChkGA = db.Users.Where(w => w.BusinessCardGroup == true && w.User_Id == id);
                //int RoleId = authorIndex.Select(s => s.Role_Id).FirstOrDefault();

                //Mg User
                if (author == 2 && ChkGA.Count() == 0)
                {
                    jobCount = db.BusinessCards.Where(w => w.Status_Id == 1).Count();
                }
                //Mg GA
                else if (author == 2 && ChkGA.Count() > 0)
                {
                    jobCount = db.BusinessCards.Where(w => w.Status_Id == 7 || w.Status_Id == 1).Count();
                }

                //Staff GA
                else if (author == 3 && ChkGA.Count() > 0)
                {
                    jobCount = db.BusinessCards.Where(w => w.Status_Id == 8).Count();
                }

            }


            return jobCount;
        }

    }
}
