using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace E2E.Models
{
    public class ClsManageBusinessCard
    {
        private readonly ClsContext db = new ClsContext();
        private readonly ClsMail mail = new ClsMail();
        private readonly ClsManageMaster master = new ClsManageMaster();

        public List<ClsBusinessCard> ClsReport_KPI_Unsatisfied(ReportKPI_Filter filter)
        {
            try
            {
                var JobUnsat = db.Satisfactions_BusinessCards.Where(w => w.Unsatisfied);
                if (filter != null)
                {
                    JobUnsat = JobUnsat.Where(w => w.Create >= filter.Date_From);

                    filter.Date_To = filter.Date_To.AddDays(1);
                    JobUnsat = JobUnsat.Where(w => w.Create <= filter.Date_To);

                    var BusinessCardId = JobUnsat.Select(s => s.BusinessCard_Id).ToList();
                    var BusinessCards = db.BusinessCards.Where(w => BusinessCardId.Contains(w.BusinessCard_Id)).AsEnumerable()
                   .Select(item => new ClsBusinessCard
                   {
                       Create = item.Create,
                       BusinessCard_Id = item.BusinessCard_Id,
                       Status_Id = item.Status_Id,
                       Key = item.Key,
                       UserRefName = item.UserRef_id.HasValue ? master.Users_GetInfomation(item.UserRef_id.Value) : null,
                       DueDate = item.DueDate,
                       Update = item.Update,
                       System_Statuses = item.System_Statuses
                   }).ToList();

                    return BusinessCards;
                }
                else
                {
                    return new List<ClsBusinessCard>();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> SaveEstimate(Guid id, List<ClsEstimate> score)
        {
            try
            {
                bool res = new bool();

                var average = score.Select(x => x.Score).Average();
                var Sum = score.Select(x => x.Score).Sum();
                Sum = Sum * 100 / 25;

                bool unsatisfied = Sum < (0.8 * 100);

                Satisfactions_BusinessCards Satisfactions_BusinessCard = new Satisfactions_BusinessCards
                {
                    BusinessCard_Id = id,
                    Satisfaction_Average = average,
                    Unsatisfied = unsatisfied
                };

                db.Satisfactions_BusinessCards.Add(Satisfactions_BusinessCard);

                foreach (var item in score)
                {
                    SatisfactionDetails_BusinessCards SatisfactionDetails_BusinessCard = new SatisfactionDetails_BusinessCards
                    {
                        Satisfactions_BusinessCard_id = Satisfactions_BusinessCard.Satisfactions_BusinessCard_id,
                        InquiryTopic_Id = item.Id,
                        Point = item.Score
                    };

                    db.SatisfactionDetails_BusinessCards.Add(SatisfactionDetails_BusinessCard);
                }

                if (db.SaveChanges() > 0)
                {
                    res = await BusinessCards_SetClose(id);
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> BusinessCards_SetClose(Guid id)
        {
            bool res = new bool();
            try
            {
                BusinessCards BusinessCard = new BusinessCards();
                BusinessCard = db.BusinessCards.Find(id);
                if (BusinessCard.Status_Id == 3)
                {
                    BusinessCard.Status_Id = 4;
                    BusinessCard.Update = DateTime.Now;

                    db.Entry(BusinessCard).State = EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        if (BusinessCard_SaveLog(BusinessCard))
                        {
                            res = await SendMail(BusinessCard);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return res;
        }

        public async Task<bool> BusinessCard_SaveCreate(ClsBusinessCard Model)
        {
            bool res = new bool();
            try
            {
                string JP = string.Empty;
                Guid MyUserID = Guid.Parse(HttpContext.Current.User.Identity.Name);

                string ChkJP = db.Users.Where(w => w.User_Id == Model.User_id).Select(s => s.Master_Grades.Master_LineWorks.LineWork_Name).FirstOrDefault();
                string Year = DateTime.Now.ToString("yyyy");
                var CountNum = db.BusinessCards.Where(w => w.Key.ToString().StartsWith(Year)).Count() + 1;

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

                if (GradeNumber(Model.User_id) <= 4)
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
                    await SendMail(businessCards);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> BusinessCard_SaveFile(string filepath, BusinessCards model)
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
                    res = await SendMail(model, null, cardFiles, filepath);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return res;
        }

        public bool BusinessCard_SaveLog(BusinessCards Model, string remark = "", bool undo = false)
        {
            bool res = new bool();

            Log_BusinessCards log_BusinessCards = new Log_BusinessCards
            {
                BusinessCard_Id = Model.BusinessCard_Id,
                Status_Id = Model.Status_Id,
                User_Id = Model.Status_Id == 1 ? Model.User_id : Guid.Parse(HttpContext.Current.User.Identity.Name),
                Create = Model.Update ?? DateTime.Now,
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

        public ClsSatisfaction ClsSatisfactionCard_View(Guid id)
        {
            try
            {
                ClsSatisfaction clsSatisfaction = new ClsSatisfaction();
                clsSatisfaction = db.Satisfactions_BusinessCards
                    .Where(w => w.BusinessCard_Id == id)
                    .GroupJoin(db.SatisfactionDetails_BusinessCards.OrderBy(o => o.Master_InquiryTopics.InquiryTopic_Index), m => m.Satisfactions_BusinessCard_id, j => j.Satisfactions_BusinessCard_id, (m, gj) => new ClsSatisfaction()
                    {
                        SatisfactionDetails_BusinessCards = gj.ToList(),
                        Satisfactions_BusinessCards = m
                    }).FirstOrDefault();

                return clsSatisfaction;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int CountJob(Guid? id)
        {
            int jobCount = 0;

            if (id.HasValue)
            {
                var authorIndex = db.Users.Where(w => w.User_Id == id).Select(s => new { s.Master_Grades.Master_LineWorks.Authorize_Id, s.Master_Processes.Master_Sections.Department_Id, s.Role_Id });
                int author = authorIndex.Select(s => s.Authorize_Id).FirstOrDefault();
                bool ChkGA = db.Users.Any(w => w.BusinessCardGroup == true && w.User_Id == id);
                //int RoleId = authorIndex.Select(s => s.Role_Id).FirstOrDefault();

                //Mg User
                if (author == 2 && !ChkGA)
                {
                    jobCount = db.BusinessCards.Where(w => w.Status_Id == 1).Count();
                }
                //Mg GA
                else if (author == 2 && ChkGA)
                {
                    jobCount = db.BusinessCards.Where(w => w.Status_Id == 7 || w.Status_Id == 1).Count();
                }

                //Staff GA
                else if (author == 3 && ChkGA)
                {
                    jobCount = db.BusinessCards.Where(w => w.Status_Id == 8).Count();
                }
            }

            return jobCount;
        }

        public int GradeNumber(Guid? UserID)
        {
            var BypassGA = db.Users.Where(w => w.User_Id == UserID).Select(s => s.Master_Grades.Grade_Name).FirstOrDefault();
            string strGrade = string.Empty;

            for (int i = 0; i < BypassGA.Length; i++)
            {
                if (int.TryParse(BypassGA[i].ToString(), out int num))
                {
                    strGrade += num.ToString();
                }
            }

            return string.IsNullOrEmpty(strGrade) ? 0 : Convert.ToInt32(strGrade);
        }

        public List<Guid> NoM3(List<Guid> IDs)
        {
            List<Guid> MGs = new List<Guid>();
            foreach (var item in IDs)
            {
                if (GradeNumber(item) >= 4)
                {
                    MGs.Add(item);
                }
            }

            return MGs;
        }

        public bool Same_department_check(Guid? id)
        {
            bool res = new bool();
            Guid MyUser = Guid.Parse(HttpContext.Current.User.Identity.Name);
            var businessCard = db.BusinessCards.Find(id);
            Guid DeptJOB = db.Users.Where(w => w.User_Id == businessCard.User_id).Select(s => s.Master_Processes.Master_Sections.Department_Id).FirstOrDefault();
            Guid DeptUser = db.Users.Where(w => w.User_Id == MyUser).Select(s => s.Master_Processes.Master_Sections.Department_Id).FirstOrDefault();

            if (DeptJOB == DeptUser)
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

        public async Task<bool> SendMail(BusinessCards Model, Guid? SelectId = null, BusinessCardFiles ModelFile = null, string filepath = "", string remark = "", string pseudo = "")
        {
            bool res = new bool();

            Guid DeptId = db.Users.Where(w => w.User_Id == Model.User_id).Select(s => s.Master_Processes.Master_Sections.Master_Departments.Department_Id).FirstOrDefault();
            var GetMgApp = db.Users.Where(w => w.Master_Processes.Master_Sections.Department_Id == DeptId && w.Master_Grades.Master_LineWorks.Authorize_Id == 2).Select(s => s.User_Id).ToList();

            var linkUrl = HttpContext.Current.Request.Url.OriginalString;
            bool found = linkUrl.Contains("Upload");
            bool reSend = linkUrl.Contains("Resend_Email");

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
                string keyword = "BusinessCards";
                string pattern = $"{keyword}.*";
                string result = Regex.Replace(linkUrl, pattern, keyword);
                result = result + "/BusinessCard_Detail/" + Model.BusinessCard_Id;
                linkUrl = result;

                GetMgApp = db.Users.Where(w => w.BusinessCardGroup == true && w.Master_Grades.Master_LineWorks.Authorize_Id == 2).Select(s => s.User_Id).ToList();
                mail.SendToIds = NoM3(GetMgApp);
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
            else if (Model.Status_Id == 9 && pseudo == "9")
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
                string keyword = "BusinessCards";
                string pattern = $"{keyword}.*";
                string result = Regex.Replace(linkUrl, pattern, keyword);
                result = result + "/BusinessCard_Detail/" + Model.BusinessCard_Id;
                linkUrl = result;

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
                if (Model.UserAction == null)
                {
                    content = string.Format("<p>Comment: {0}</p>", remark);
                    content += "<p>Assign task to Department General Affair";
                }
                else
                {
                    content = string.Format("<p>Comment: {0}</p>", remark);
                    content += "<p>Assign task to " + master.Users_GetInfomation(Model.UserAction.Value);
                }

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
                string keyword = "BusinessCards";
                string pattern = $"{keyword}.*";
                string result = Regex.Replace(linkUrl, pattern, keyword);
                result = result + "/BusinessCard_Detail/" + Model.BusinessCard_Id;
                linkUrl = result;

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
                content += string.Format("<p>Requester confirm business card is correct.</p>");
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
                string keyword = "BusinessCards";
                string pattern = $"{keyword}.*";
                string result = Regex.Replace(linkUrl, pattern, keyword);
                result = result + "/BusinessCard_Detail/" + Model.BusinessCard_Id;
                linkUrl = result;

                Guid ActionId = Guid.Parse(HttpContext.Current.User.Identity.Name);

                content = string.Empty;
                subject = string.Format("[Business Card][Requester Closed] {0}", Model.Key);
                content += string.Format("<p>Requester Closed job.</p>");
                mail.SendToId = Model.UserAction;
                mail.SendToIds.Clear();
                mail.SendFrom = ActionId;
                mail.Subject = subject;
                mail.SendCC = null;
            }

            //Staff Completed
            else if (Model.Status_Id == 3)
            {
                string keyword = "BusinessCards";
                string pattern = $"{keyword}.*";
                string result = Regex.Replace(linkUrl, pattern, keyword);
                result = result + "/BusinessCard_Detail/" + Model.BusinessCard_Id;
                linkUrl = result;

                Guid ActionId = Guid.Parse(HttpContext.Current.User.Identity.Name);

                content = string.Empty;
                subject = string.Format("[Business Card][Please Close job] {0}", Model.Key);

                mail.SendToId = Model.User_id;
                mail.SendToIds.Clear();
                mail.SendFrom = ActionId;
                mail.Subject = subject;
                mail.SendCC = null;
            }

            //Resend
            else if (reSend)
            {
                string keyword = "BusinessCards";
                string pattern = $"{keyword}.*";
                string result = Regex.Replace(linkUrl, pattern, keyword);
                result = result + "/BusinessCard_Detail/" + Model.BusinessCard_Id;
                linkUrl = result;
            }

            content += "</p>";
            content += string.Format("<a href='{0}' target='_blank'>Please, click here to more detail.</a>", linkUrl);
            content += "<p>Thank you for your consideration</p>";
            mail.Body = content;
            res = await mail.SendMail(mail);

            return res;
        }

        public List<ReportKPI_User_Cards_Views> ReportKPI_User_Views(Guid id, ReportKPI_Filter filter)
        {
            try
            {
                IQueryable<ReportKPI_User_Cards_Views> query = db.BusinessCards
                    .Where(w => w.UserAction == id)
                    .GroupJoin(db.Satisfactions_BusinessCards, ser => ser.BusinessCard_Id, sat => sat.BusinessCard_Id, (ser, g) => new
                    {
                        ser,
                        g
                    }).SelectMany(tmp => tmp.g.DefaultIfEmpty(), (tmp, sat) => new ReportKPI_User_Cards_Views()
                    {
                        BusinessCard_Id = tmp.ser.BusinessCard_Id,
                        Create = tmp.ser.Create,
                        Key = tmp.ser.Key.ToString(),
                        Priority_Point = tmp.ser.System_Priorities != null ? tmp.ser.System_Priorities.Priority_Point : 0,
                        Satisfaction_Average = sat != null ? sat.Satisfaction_Average : (double?)null,
                        Status_Name = tmp.ser.System_Statuses != null ? tmp.ser.System_Statuses.Status_Name : null,
                        Status_Class = tmp.ser.System_Statuses != null ? tmp.ser.System_Statuses.Status_Class : null
                    }).OrderBy(o => o.Create);

                if (filter != null)
                {
                    query = query.Where(w => w.Create >= filter.Date_From);

                    filter.Date_To = filter.Date_To.AddDays(1);
                    query = query.Where(w => w.Create <= filter.Date_To);
                }

                return query.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ClsReportKPI ClsReportKPI_ViewList(ReportKPI_Filter filter)
        {
            try
            {
                ClsReportKPI res = new ClsReportKPI();

                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                IQueryable<Guid> userIds;

                IQueryable<Guid> businessCardIds;
                IQueryable<BusinessCards> query = db.BusinessCards.OrderBy(o => o.Create).ThenBy(t => t.Update);

                res.Authorize_Id = db.Users
                    .Where(w => w.User_Id == userId)
                    .Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id)
                    .FirstOrDefault();

                int[] finishIds = { 3, 4 };

                Guid DeptId = db.Users.Find(userId).Master_Processes.Master_Sections.Department_Id;

                userIds = db.Users
                    .Where(w => w.Master_Processes.Master_Sections.Department_Id == DeptId)
                    .OrderBy(o => o.User_Code)
                    .Select(s => s.User_Id);

                query = query.Where(w => userIds.Contains(w.UserAction.Value));

                if (filter != null)
                {
                    query = query.Where(w => w.Create >= filter.Date_From);

                    filter.Date_To = filter.Date_To.AddDays(1);
                    query = query.Where(w => w.Create <= filter.Date_To);
                }

                foreach (var item in userIds)
                {
                    businessCardIds = query
                        .Where(w => w.UserAction == item)
                        .Select(s => s.BusinessCard_Id);

                    ReportKPI_User reportKPI_User = new ReportKPI_User();

                    if (businessCardIds.Count() > 0)
                    {
                        int countSatisfaction = db.Satisfactions_BusinessCards
                    .Where(w => businessCardIds.Contains(w.BusinessCard_Id))
                    .Count();
                        if (countSatisfaction > 0)
                        {
                            reportKPI_User.Average_Score = db.Satisfactions_BusinessCards
                    .Where(w => businessCardIds.Contains(w.BusinessCard_Id))
                    .Average(a => a.Satisfaction_Average);
                        }

                        if (query.Any(w => businessCardIds.Contains(w.BusinessCard_Id) && finishIds.Contains(w.Status_Id)))
                        {
                            int? SuccessPoint = query
                     .Where(w => businessCardIds.Contains(w.BusinessCard_Id) && finishIds.Contains(w.Status_Id))
                    .Sum(s => (int?)s.System_Priorities.Priority_Point);

                            if (SuccessPoint.HasValue)
                            {
                                reportKPI_User.SuccessPoint = SuccessPoint.Value;
                            }
                        }

                        reportKPI_User.Close_Count = query.Where(w => w.Status_Id == 4 && businessCardIds.Contains(w.BusinessCard_Id)).Count();
                        reportKPI_User.Complete_Count = query.Where(w => w.Status_Id == 3 && businessCardIds.Contains(w.BusinessCard_Id)).Count();
                        reportKPI_User.Total = query.Where(w => businessCardIds.Contains(w.BusinessCard_Id)).Count();
                        reportKPI_User.OverDue_Count = query.Where(w => w.Is_OverDue && businessCardIds.Contains(w.BusinessCard_Id)).Count();
                    }

                    businessCardIds = query
                    .Where(w => w.UserAction != item)
                    .Select(s => s.BusinessCard_Id);

                    reportKPI_User.User_Id = item;
                    reportKPI_User.User_Name = master.Users_GetInfomation(item);
                    res.ReportKPI_Users.Add(reportKPI_User);
                }

                res.ReportKPI_Overview.Close_Count = res.ReportKPI_Users.Select(s => s.Close_Count).Sum();
                res.ReportKPI_Overview.Complete_Count = res.ReportKPI_Users.Select(s => s.Complete_Count).Sum();
                res.ReportKPI_Overview.Total = res.ReportKPI_Users.Select(s => s.Total).Sum();
                res.ReportKPI_Overview.OverDue_Count = res.ReportKPI_Users.Select(s => s.OverDue_Count).Sum();
                int ontimeCount = res.ReportKPI_Overview.Total - res.ReportKPI_Overview.OverDue_Count;
                res.ReportKPI_Overview.OnTime_Count = ontimeCount;
                double ontimePercent = Convert.ToDouble(ontimeCount) / Convert.ToDouble(res.ReportKPI_Overview.Total);
                res.ReportKPI_Overview.OnTime_Percent = ontimePercent;

                var UnsatCount = db.Satisfactions_BusinessCards.Where(w => w.Unsatisfied);
                if (filter != null)
                {
                    UnsatCount = UnsatCount.Where(w => w.Create >= filter.Date_From);

                    filter.Date_To = filter.Date_To.AddDays(1);
                    UnsatCount = UnsatCount.Where(w => w.Create <= filter.Date_To);
                    res.ReportKPI_Overview.Unsatisfied_Count = UnsatCount.Count();
                }

                double? Avg = res.ReportKPI_Users.Select(s => s.Average_Score).Average();

                int CountTopic = db.Master_InquiryTopics.Where(w => w.Program_Id == 2).Count();

                if (Avg != null)
                {
                    res.ReportKPI_Overview.Satisfied_Percent = Math.Abs((Avg.Value / CountTopic) * 100) / 100;
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
