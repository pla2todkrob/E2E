using E2E.Models.Tables;
using E2E.Models.Views;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace E2E.Models
{
    public class clsManageService
    {
        private clsContext db = new clsContext();
        private clsServiceFTP ftp = new clsServiceFTP();
        private clsMail mail = new clsMail();
        private clsManageMaster master = new clsManageMaster();

        private IQueryable<Services> Services_GetAllRequest_IQ()
        {
            try
            {
                Guid id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                string departmentName = db.Users
                    .Where(w => w.User_Id == id)
                    .Select(s => s.Master_Processes.Master_Sections.Master_Departments.Department_Name)
                    .FirstOrDefault();
                List<Guid> userIds = db.Users
                    .Where(w => w.Master_Processes.Master_Sections.Master_Departments.Department_Name == departmentName)
                    .Select(s => s.User_Id)
                    .ToList();
                IQueryable<Services> query = db.Services
                    .Where(w => userIds.Contains(w.User_Id));

                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IQueryable<Services> Services_GetAllTask_IQ()
        {
            try
            {
                Guid id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                string departmentName = db.Users
                    .Where(w => w.User_Id == id)
                    .Select(s => s.Master_Processes.Master_Sections.Master_Departments.Department_Name)
                    .FirstOrDefault();

                IQueryable<Services> query = db.Services
                    .Where(w => w.Master_Departments.Department_Name == departmentName);

                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IQueryable<Services> Services_GetNoPending_IQ()
        {
            return db.Services
                .Where(w => w.Status_Id != 1)
                .OrderByDescending(o => o.Priority_Id)
                .ThenBy(o => new { o.Create, o.Service_DueDate });
        }

        private IQueryable<Services> Services_GetWaitAction_IQ(Guid? id)
        {
            try
            {
                IQueryable<Services> query = db.Services
                    .Where(w => w.Is_Commit && w.Status_Id == 1 && (w.Is_Approval || !w.Is_MustBeApproved))
                    .OrderByDescending(o => o.Priority_Id)
                    .ThenBy(o => new { o.Create, o.Service_DueDate });

                if (id.HasValue)
                {
                    string deptName = db.Users
                        .Where(w => w.User_Id == id.Value)
                        .Select(s => s.Master_Processes.Master_Sections.Master_Departments.Department_Name)
                        .FirstOrDefault();
                    if (!string.IsNullOrEmpty(deptName))
                    {
                        query = query.Where(w => (w.Master_Departments.Department_Name == deptName && !w.Action_User_Id.HasValue) || w.Action_User_Id == id);
                    }
                }

                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IQueryable<Services> Services_GetWaitCommit_IQ()
        {
            try
            {
                return db.Services
                    .Where(w => !w.Is_Commit && w.Status_Id == 1 && (!w.Is_MustBeApproved || w.Is_Approval))
                    .OrderByDescending(o => o.Priority_Id)
                    .ThenBy(t => new { t.Create, t.Service_DueDate });
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IQueryable<ServiceTeams> ServiceTeams_IQ(Guid id)
        {
            return db.ServiceTeams
                    .Where(w => w.Service_Id == id);
        }

        public clsServices ClsServices_View(Guid id)
        {
            try
            {
                clsServices clsServices = new clsServices();
                clsServices = db.Services
                    .Where(w => w.Service_Id == id)
                    .GroupJoin(db.ServiceFiles, m => m.Service_Id, j => j.Service_Id, (m, gj) => new clsServices()
                    {
                        ServiceFiles = gj.ToList(),
                        Services = m
                    }).FirstOrDefault();

                clsServices.User_Name = master.Users_GetInfomation(clsServices.Services.User_Id);
                clsServices.Create_Name = master.Users_GetInfomation(clsServices.Services.Create_User_Id);

                if (clsServices.Services.Action_User_Id.HasValue)
                {
                    UserDetails userDetails = new UserDetails();
                    userDetails = db.UserDetails
                        .Where(w => w.User_Id == clsServices.Services.Action_User_Id)
                        .FirstOrDefault();
                    clsServices.Action_Email = userDetails.Users.User_Email;
                    clsServices.Action_Name = master.Users_GetInfomation(userDetails.User_Id);
                }

                clsServices.ServiceChangeDueDate = db.ServiceChangeDueDates
                    .Where(w => w.Service_Id == id && w.DueDateStatus_Id == 1)
                    .OrderByDescending(o => o.Create)
                    .FirstOrDefault();

                foreach (var item in ServiceTeams_IQ(id))
                {
                    clsServiceTeams clsServiceTeams = new clsServiceTeams();
                    clsServiceTeams.ServiceTeams = item;
                    clsServiceTeams.User_Name = master.Users_GetInfomation(item.User_Id);
                    clsServices.ClsServiceTeams.Add(clsServiceTeams);
                }

                return clsServices;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public clsServices ClsServices_ViewComment(Guid id)
        {
            try
            {
                clsServices clsServices = new clsServices();
                clsServices.Services = db.Services.Find(id);
                clsServices.ClsServiceComments = db.ServiceComments
                    .Where(w => w.Service_Id == id)
                    .GroupJoin(db.ServiceCommentFiles, m => m.ServiceComment_Id, j => j.ServiceComment_Id, (m, gj) => new clsServiceComments()
                    {
                        ServiceComments = m,
                        ServiceCommentFiles = gj.ToList()
                    })
                    .OrderBy(o => o.ServiceComments.Create)
                    .ToList();

                clsServices.ClsServiceComments.ForEach(f => f.User_Name = master.Users_GetInfomation(f.ServiceComments.User_Id.Value));

                return clsServices;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<clsServices> ClsServices_ViewList(Guid id)
        {
            try
            {
                List<clsServices> res = new List<clsServices>();
                Guid? refId = id;
                while (refId.HasValue)
                {
                    clsServices clsServices = new clsServices();
                    clsServices = db.Services
                        .Where(w => w.Service_Id == refId.Value)
                        .Select(s => new clsServices()
                        {
                            Services = s,
                            ServiceFiles = db.ServiceFiles.Where(w => w.Service_Id == s.Service_Id).ToList()
                        }).FirstOrDefault();
                    clsServices.User_Name = master.Users_GetInfomation(clsServices.Services.User_Id);

                    res.Add(clsServices);
                    refId = clsServices.Services.Ref_Service_Id;
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<clsServices> ClsServices_ViewRefList(Guid id)
        {
            try
            {
                List<clsServices> res = new List<clsServices>();
                Guid? refId = db.Services.Find(id).Ref_Service_Id;
                while (refId.HasValue)
                {
                    clsServices clsServices = new clsServices();
                    clsServices = db.Services
                        .Where(w => w.Service_Id == refId.Value)
                        .Select(s => new clsServices()
                        {
                            Services = s,
                            ServiceFiles = db.ServiceFiles.Where(w => w.Service_Id == s.Service_Id).ToList()
                        }).FirstOrDefault();
                    clsServices.User_Name = master.Users_GetInfomation(clsServices.Services.User_Id);

                    UserDetails userDetails = new UserDetails();
                    userDetails = db.UserDetails
                        .Where(w => w.User_Id == clsServices.Services.Action_User_Id)
                        .FirstOrDefault();

                    clsServices.Action_Email = userDetails.Users.User_Email;
                    clsServices.Action_Name = master.Users_GetInfomation(userDetails.User_Id);

                    foreach (var item in ServiceTeams_IQ(refId.Value))
                    {
                        clsServiceTeams clsServiceTeams = new clsServiceTeams();
                        clsServiceTeams.ServiceTeams = item;
                        clsServiceTeams.User_Name = master.Users_GetInfomation(item.User_Id);
                        clsServices.ClsServiceTeams.Add(clsServiceTeams);
                    }

                    res.Add(clsServices);
                    refId = clsServices.Services.Ref_Service_Id;
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SaveEstimate(Guid id, List<clsEstimate> score)
        {
            try
            {
                bool res = new bool();

                var average = score.Select(x => x.score).Average();

                Satisfactions satisfactions = new Satisfactions();
                satisfactions.Service_Id = id;
                satisfactions.Satisfaction_Average = average;
                db.Satisfactions.Add(satisfactions);

                foreach (var item in score)
                {
                    SatisfactionDetails satisfactionDetails = new SatisfactionDetails();
                    satisfactionDetails.Satisfaction_Id = satisfactions.Satisfaction_Id;
                    satisfactionDetails.InquiryTopic_Id = item.id;
                    satisfactionDetails.Point = item.score;

                    db.SatisfactionDetails.Add(satisfactionDetails);
                }

                if (db.SaveChanges() > 0)
                {
                    res = Services_SetClose(id);
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListItem> SelectListItems_Priority()
        {
            try
            {
                return db.System_Priorities
                .OrderBy(o => o.Priority_Id)
                .Select(s => new SelectListItem()
                {
                    Value = s.Priority_Id.ToString(),
                    Text = s.Priority_Name + " [Point: " + s.Priority_Point + "]"
                }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SelectListItem> SelectListItems_RefService(Guid userId)
        {
            try
            {
                List<Guid?> Ids = Services_GetNoPending_IQ()
                    .Where(w => w.Status_Id < 5 &&
                    w.User_Id == userId)
                    .Select(s => s.Ref_Service_Id)
                    .ToList();

                List<SelectListItem> res = new List<SelectListItem>();
                res.Add(new SelectListItem()
                {
                    Text = "Select Reference",
                    Value = ""
                });
                res.AddRange(Services_GetNoPending_IQ()
                    .Where(w => !Ids.Contains(w.Service_Id) &&
                    w.Status_Id < 5 &&
                    w.User_Id == userId)
                    .Select(s => new SelectListItem()
                    {
                        Text = s.Service_Key + "(" + s.Service_Subject + ")",
                        Value = s.Service_Id.ToString()
                    }).OrderByDescending(o => o.Text)
                    .ToList());

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SelectListItem> SelectListItems_Team(Guid id)
        {
            try
            {
                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                Guid departmentId = db.Users
                    .Where(w => w.User_Id == userId)
                    .Select(s => s.Master_Processes.Master_Sections.Department_Id.Value)
                    .FirstOrDefault();

                List<Guid> userIdInTeam = ServiceTeams_IQ(id)
                    .Select(s => s.User_Id)
                    .ToList();

                return db.UserDetails
                    .Where(w => w.Users.Master_Processes.Master_Sections.Department_Id == departmentId &&
                    w.Users.Active &&
                    !userIdInTeam.Contains(w.User_Id) &&
                    w.User_Id != userId)
                    .Select(s => new SelectListItem()
                    {
                        Text = s.Users.User_Code + " [" + s.Detail_EN_FirstName + " " + s.Detail_EN_LastName + "]",
                        Value = s.User_Id.ToString()
                    }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SelectListItem> SelectListItems_User()
        {
            try
            {
                return db.UserDetails
                .Where(w => w.Users.Active)
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

        public bool Service_AddTeam(clsServiceTeams model)
        {
            try
            {
                bool res = new bool();
                string Getteam = string.Empty;
                foreach (var item in model.User_Ids)
                {
                    ServiceTeams serviceTeams = new ServiceTeams();
                    serviceTeams.Service_Id = model.Service_Id;
                    serviceTeams.User_Id = item;
                    db.Entry(serviceTeams).State = System.Data.Entity.EntityState.Added;
                    if (db.SaveChanges() > 0)
                    {
                        ServiceComments serviceComments = new ServiceComments();
                        serviceComments.Service_Id = model.Service_Id;
                        serviceComments.Comment_Content = string.Format("Add {0} to join team", master.Users_GetInfomation(item));
                        res = Services_Comment(serviceComments);
                    }
                }

                var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                linkUrl += "/" + model.Service_Id;
                linkUrl = linkUrl.Replace("_AddTeam", "ServiceInfomation");

                Services services = new Services();
                services = db.Services.Find(model.Service_Id);

                List<Guid> listTeam = new List<Guid>();
                listTeam = db.ServiceTeams
                    .Where(w => w.Service_Id == services.Service_Id)
                    .Select(s => s.User_Id)
                    .ToList();

                foreach (var item in listTeam)
                {
                    Getteam += master.Users_GetInfomation(item) + "<br />";
                }

                string subject = string.Format("[E2E][Notify add team] {0} - {1}", services.Service_Key, services.Service_Subject);
                string content = string.Format("<p><b>Description:</b> {0}", services.Service_Description);
                content += "<br />";
                content += "<br />";
                content += "<b>Current member</b><br/>";
                content += Getteam;
                content += "</p>";
                content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                content += "<p>Thank you for your consideration</p>";
                res = mail.SendMail(listTeam, subject, content);

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Guid? Service_CHK_CloseJob(Guid id)
        {
            try
            {
                var res = db.Services.Where(w => w.User_Id == id && w.Status_Id == 3).FirstOrDefault();

                Guid? id2 = null;

                if (res != null)
                {
                    id2 = res.Service_Id;
                }

                return id2;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Service_DeleteTeam(Guid id)
        {
            try
            {
                bool res = new bool();
                string getTeam = string.Empty;
                ServiceTeams serviceTeams = new ServiceTeams();
                serviceTeams = db.ServiceTeams.Find(id);
                string userName = master.Users_GetInfomation(serviceTeams.User_Id);
                Guid serviceId = serviceTeams.Service_Id;
                db.Entry(serviceTeams).State = System.Data.Entity.EntityState.Deleted;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    serviceComments.Service_Id = serviceId;
                    serviceComments.Comment_Content = string.Format("Delete {0} from this team", userName);
                    if (Services_Comment(serviceComments))
                    {
                        var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                        linkUrl = linkUrl.Replace("_DeleteTeam", "ServiceInfomation");

                        Services services = new Services();
                        services = db.Services.Find(serviceId);

                        List<Guid> listTeam = new List<Guid>();
                        listTeam = db.ServiceTeams
                            .Where(w => w.Service_Id == serviceId)
                            .Select(s => s.User_Id)
                            .ToList();

                        foreach (var item in listTeam)
                        {
                            getTeam += master.Users_GetInfomation(item) + "<br />";
                        }

                        listTeam.Add(serviceTeams.User_Id);

                        string subject = string.Format("[E2E][Notify delete member] {0} - {1}", services.Service_Key, services.Service_Subject);
                        string content = "<p><b>Delete: </b>" + userName;
                        content += "<br />";
                        content += "<br />";
                        content += "<b>Current member</b><br/>";
                        content += getTeam;
                        content += "</p>";
                        content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                        content += "<p>Thank you for your consideration</p>";
                        res = mail.SendMail(listTeam, subject, content);
                    }
                }
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ServiceChangeDueDate_Accept(Guid id)
        {
            try
            {
                bool res = new bool();
                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                ServiceChangeDueDate serviceChangeDueDate = db.ServiceChangeDueDates.Find(id);
                serviceChangeDueDate.DueDateStatus_Id = 2;
                serviceChangeDueDate.Update = DateTime.Now;
                db.Entry(serviceChangeDueDate).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    serviceComments.Comment_Content = "Accept due date change request";
                    serviceComments.Service_Id = serviceChangeDueDate.Service_Id;
                    serviceComments.User_Id = userId;
                    if (Services_Comment(serviceComments))
                    {
                        Services services = db.Services.Find(serviceChangeDueDate.Service_Id);
                        services.Service_DueDate = serviceChangeDueDate.DueDate_New;
                        services.Update = DateTime.Now;
                        db.Entry(services).State = System.Data.Entity.EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            serviceComments = new ServiceComments();
                            serviceComments.Comment_Content = string.Format("Change due date from {0} to {1}", serviceChangeDueDate.DueDate.ToString("d"), serviceChangeDueDate.DueDate_New.Value.ToString("d"));
                            serviceComments.Service_Id = serviceChangeDueDate.Service_Id;
                            serviceComments.User_Id = userId;
                            res = Services_Comment(serviceComments);
                        }
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ServiceChangeDueDate_Cancel(Guid id)
        {
            try
            {
                bool res = new bool();
                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                ServiceChangeDueDate serviceChangeDueDate = db.ServiceChangeDueDates.Find(id);
                serviceChangeDueDate.DueDateStatus_Id = 4;
                serviceChangeDueDate.Update = DateTime.Now;
                db.Entry(serviceChangeDueDate).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    serviceComments.Comment_Content = "Cancel due date change request";
                    serviceComments.Service_Id = serviceChangeDueDate.Service_Id;
                    serviceComments.User_Id = userId;
                    res = Services_Comment(serviceComments);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ServiceChangeDueDate ServiceChangeDueDate_New(Guid id)
        {
            try
            {
                ServiceChangeDueDate serviceChangeDueDate = new ServiceChangeDueDate();
                serviceChangeDueDate.DueDate = db.Services.Find(id).Service_DueDate.Value;
                serviceChangeDueDate.Service_Id = id;
                return serviceChangeDueDate;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ServiceChangeDueDate_Reject(Guid id)
        {
            try
            {
                bool res = new bool();
                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                ServiceChangeDueDate serviceChangeDueDate = db.ServiceChangeDueDates.Find(id);
                serviceChangeDueDate.DueDateStatus_Id = 3;
                serviceChangeDueDate.Update = DateTime.Now;
                db.Entry(serviceChangeDueDate).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    serviceComments.Comment_Content = "Reject due date change request";
                    serviceComments.Service_Id = serviceChangeDueDate.Service_Id;
                    serviceComments.User_Id = userId;
                    res = Services_Comment(serviceComments);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ServiceChangeDueDate_Request(ServiceChangeDueDate model)
        {
            try
            {
                bool res = new bool();
                DateTime dateTime = DateTime.Now;
                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                model.User_Id = userId;
                db.Entry(model).State = System.Data.Entity.EntityState.Added;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    serviceComments.Service_Id = model.Service_Id;
                    serviceComments.Comment_Content = string.Format("Request change due date from {0} to {1}", model.DueDate.ToString("d"), model.DueDate_New.Value.ToString("d"));
                    serviceComments.User_Id = userId;
                    if (Services_Comment(serviceComments))
                    {
                        Services services = new Services();
                        services = db.Services.Find(model.Service_Id);

                        var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                        linkUrl = linkUrl.Replace("RequestChangeDue_Form", "RequestChangeDue");

                        string subject = string.Format("[E2E][Request change due date] {0} - {1}", services.Service_Key, services.Service_Subject);
                        string content = string.Format("<p><b>Description:</b> {0}", serviceComments.Comment_Content);
                        content += "</p>";
                        content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                        content += "<p>Thank you for your consideration</p>";
                        res = mail.SendMail(userId, subject, content);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ServiceChangeDueDate> ServiceChangeDues_List()
        {
            try
            {
                Guid id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                List<Guid> ids = Services_GetAllRequest_IQ()
                    .Where(w => w.User_Id == id)
                    .Select(s => s.Service_Id)
                    .ToList();
                return db.ServiceChangeDueDates
                    .Where(w => ids.Contains(w.Service_Id))
                    .ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ServiceFiles_Delete(Guid id)
        {
            try
            {
                bool res = new bool();
                ServiceFiles serviceFiles = new ServiceFiles();
                serviceFiles = db.ServiceFiles.Find(id);

                Services services = new Services();
                services = db.Services.Find(serviceFiles.Service_Id);
                services.Service_FileCount -= 1;
                services.Update = DateTime.Now;
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
                db.Entry(serviceFiles).State = System.Data.Entity.EntityState.Deleted;
                if (ftp.Ftp_DeleteFile(serviceFiles.ServiceFile_Path))
                {
                    if (db.SaveChanges() > 0)
                    {
                        res = true;
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ServiceFiles> ServiceFiles_View(Guid id)
        {
            try
            {
                return db.ServiceFiles
                    .Where(w => w.Service_Id == id)
                    .OrderBy(o => o.ServiceFile_Name)
                    .ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_Comment(ServiceComments model, HttpFileCollectionBase files = null)
        {
            try
            {
                bool res = new bool();
                model.User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                model.Create = DateTime.Now;
                db.Entry(model).State = System.Data.Entity.EntityState.Added;
                if (files != null)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        if (files[i].ContentLength == 0)
                        {
                            break;
                        }
                        ServiceCommentFiles serviceCommentFiles = new ServiceCommentFiles();
                        serviceCommentFiles.ServiceCommentFile_Name = files[i].FileName;
                        string dir = string.Format("Service/{0}/Comment/{1}/", model.Service_Id, DateTime.Today.ToString("yyMMdd"));
                        serviceCommentFiles.ServiceCommentFile_Path = ftp.Ftp_UploadFileToString(dir, files[i]);
                        serviceCommentFiles.ServiceComment_Id = model.ServiceComment_Id;
                        serviceCommentFiles.ServiceComment_Seq = i;
                        serviceCommentFiles.ServiceCommentFile_Extension = Path.GetExtension(files[i].FileName);
                        db.Entry(serviceCommentFiles).State = System.Data.Entity.EntityState.Added;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    res = true;
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Services> Services_GetDepartmentRequest()
        {
            try
            {
                return Services_GetAllRequest_IQ().ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Services> Services_GetDepartmentTask()
        {
            try
            {
                return Services_GetAllTask_IQ().ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Services> Services_GetMyRequest()
        {
            try
            {
                Guid id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                return Services_GetAllRequest_IQ()
                    .Where(w => w.User_Id == id)
                    .ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Services> Services_GetMyTask()
        {
            try
            {
                Guid id = Guid.Parse(HttpContext.Current.User.Identity.Name);

                var serviceTeams = db.ServiceTeams.Where(w => w.User_Id == id).Select(s => s.Service_Id).ToList();

                return Services_GetAllTask_IQ()
                    .Where(w => w.Action_User_Id == id || serviceTeams.Contains(w.Service_Id))
                    .ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Services_GetNoPendingCount()
        {
            return Services_GetNoPending_IQ().Count();
        }

        public List<Services> Services_GetNoPendingList()
        {
            return Services_GetNoPending_IQ().ToList();
        }

        public List<Services> Services_GetRequiredApprove(bool val)
        {
            try
            {
                Guid id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                string deptName = db.Users.Find(id).Master_Processes.Master_Sections.Master_Departments.Department_Name;
                List<Guid> userIdList = db.Users
                    .Where(w => w.Master_Processes.Master_Sections.Master_Departments.Department_Name == deptName).Select(s => s.User_Id).ToList();
                var sql = db.Services.Where(w => w.Is_MustBeApproved && w.Is_Approval == val && userIdList.Contains(w.User_Id) && w.Status_Id == 1).ToList();
                return db.Services.Where(w => w.Is_MustBeApproved && w.Is_Approval == val && userIdList.Contains(w.User_Id) && w.Status_Id == 1).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Services_GetWaitActionCount(Guid? id = null)
        {
            try
            {
                return Services_GetWaitAction_IQ(id).Count();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Services> Services_GetWaitActionList(Guid? id = null)
        {
            try
            {
                return Services_GetWaitAction_IQ(id).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Services_GetWaitCommitCount()
        {
            try
            {
                return Services_GetWaitCommit_IQ().Count();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Services> Services_GetWaitCommitList()
        {
            try
            {
                return Services_GetWaitCommit_IQ().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Services_Insert(Services model, HttpFileCollectionBase files, bool isForward = false)
        {
            try
            {
                if (isForward)
                {
                    model.Is_FreePoint = true;
                    goto InsertProcess;
                }
                else
                {
                    Users users = new Users();
                    users = db.Users.Find(model.User_Id);
                    int usePoint = db.System_Priorities.Find(model.Priority_Id).Priority_Point;
                    if (users.User_Point < usePoint)
                    {
                        return false;
                    }

                    users.User_Point -= usePoint;
                    db.Entry(users).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

            InsertProcess:

                bool res = new bool();
                int todayCount = db.Services
                    .Where(w => w.Create >= DateTime.Today)
                    .Count();
                todayCount += 1;
                model.Service_Key = string.Concat(DateTime.Now.ToString("yyMMdd"), todayCount.ToString().PadLeft(3, '0'));
                model.Status_Id = 1;
                if (files[0].ContentLength > 0)
                {
                    model.Service_FileCount = files.Count;
                    for (int i = 0; i < files.Count; i++)
                    {
                        ServiceFiles serviceFiles = new ServiceFiles();
                        serviceFiles.Service_Id = model.Service_Id;
                        serviceFiles.ServiceFile_Name = files[i].FileName;
                        string dir = string.Format("Service/{0}/", model.Service_Id);
                        serviceFiles.ServiceFile_Path = ftp.Ftp_UploadFileToString(dir, files[i]);
                        serviceFiles.ServiceFile_Extension = Path.GetExtension(files[i].FileName);
                        db.Entry(serviceFiles).State = System.Data.Entity.EntityState.Added;
                    }
                }

                db.Entry(model).State = System.Data.Entity.EntityState.Added;

                if (db.SaveChanges() > 0)
                {
                    if (model.Ref_Service_Id.HasValue)
                    {
                        System_Statuses system_Statuses = new System_Statuses();
                        system_Statuses = db.System_Statuses
                            .Where(w => w.Status_Id == 3)
                            .FirstOrDefault();

                        Services services = new Services();
                        services = db.Services.Find(model.Ref_Service_Id);
                        services.Status_Id = 3;
                        services.Update = DateTime.Now;
                        db.Entry(services).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        ServiceComments serviceComments = new ServiceComments();
                        serviceComments.Service_Id = model.Ref_Service_Id.Value;
                        serviceComments.Comment_Content = string.Format("Complete task, Status update to {0}", system_Statuses.Status_Name);

                        if (Services_Comment(serviceComments))
                        {
                            serviceComments = new ServiceComments();
                            serviceComments.Service_Id = model.Ref_Service_Id.Value;
                            serviceComments.Comment_Content = string.Format("Forward this job to new service key {0}", model.Service_Key);
                            res = Services_Comment(serviceComments);
                        }
                    }
                    else
                    {
                        res = true;
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_Save(Services model, HttpFileCollectionBase files, bool isForward = false)
        {
            try
            {
                bool res = new bool();
                model.Create_User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                Services services = new Services();
                services = db.Services.Find(model.Service_Id);
                if (services != null)
                {
                    services.Service_Subject = model.Service_Subject;
                    services.Service_Description = model.Service_Description;
                    if (model.Ref_Service_Id.HasValue)
                    {
                        services.Ref_Service_Id = model.Ref_Service_Id;
                    }

                    services.User_Id = model.User_Id;

                    services.Priority_Id = model.Priority_Id;
                    services.Service_DueDate = model.Service_DueDate;
                    res = Services_Update(services, files);
                }
                else
                {
                    res = Services_Insert(model, files, isForward);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetAction(Services model)
        {
            try
            {
                bool res = new bool();
                Services services = new Services();
                services = db.Services.Find(model.Service_Id);

                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);

                if (!services.Action_User_Id.HasValue)
                {
                    services.Action_User_Id = userId;
                }

                System_Statuses system_Statuses = new System_Statuses();
                system_Statuses = db.System_Statuses
                    .Where(w => w.Status_Id == 2)
                    .FirstOrDefault();

                if (model.WorkRoot_Id.HasValue)
                {
                    services.WorkRoot_Id = model.WorkRoot_Id;
                    foreach (var item in db.WorkRootDocuments.Where(w => w.WorkRoot_Id == model.WorkRoot_Id && w.Document_Id.HasValue).Select(s => s.Document_Id))
                    {
                        ServiceDocuments serviceDocuments = new ServiceDocuments();
                        serviceDocuments.Document_Id = item.Value;
                        serviceDocuments.Service_Id = services.Service_Id;
                        db.Entry(serviceDocuments).State = System.Data.Entity.EntityState.Added;
                    }
                }

                services.Service_EstimateTime = model.Service_EstimateTime;
                services.Status_Id = system_Statuses.Status_Id;
                services.Update = DateTime.Now;
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    serviceComments.Service_Id = services.Service_Id;
                    serviceComments.Comment_Content = string.Format("Start task, Estimate time about {0} days, Status update to {1}", services.Service_EstimateTime, system_Statuses.Status_Name);
                    res = Services_Comment(serviceComments);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetApprove(ServiceComments model)
        {
            try
            {
                bool res = new bool();
                Services services = new Services();
                services = db.Services.Find(model.Service_Id);
                services.Is_Approval = true;
                services.Update = DateTime.Now;
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    serviceComments.Service_Id = model.Service_Id;
                    serviceComments.Comment_Content = string.Format("Approved,\n{0}", model.Comment_Content);
                    if (Services_Comment(serviceComments))
                    {
                        var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                        linkUrl += "/" + services.Service_Id;
                        linkUrl = linkUrl.Replace("SetApproved", "Approve_Form");

                        string subject = string.Format("[E2E][Approval] {0} - {1}", services.Service_Key, services.Service_Subject);
                        string content = string.Format("<p><b>Comment:</b> {0}<br />{1}", model.Comment_Content, serviceComments.Comment_Content);
                        content += "</p>";
                        content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                        content += "<p>Thank you for your consideration</p>";
                        res = mail.SendMail(services.User_Id, subject, content);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetCancel(ServiceComments model)
        {
            try
            {
                bool res = new bool();
                Services services = new Services();
                services = db.Services.Find(model.Service_Id);

                System_Statuses system_Statuses = new System_Statuses();
                system_Statuses = db.System_Statuses
                    .Where(w => w.Status_Id == 6)
                    .FirstOrDefault();

                services.Update = DateTime.Now;
                services.Status_Id = system_Statuses.Status_Id;
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    if (!string.IsNullOrEmpty(model.Comment_Content))
                    {
                        serviceComments = new ServiceComments();
                        serviceComments.Service_Id = services.Service_Id;
                        serviceComments.Comment_Content = model.Comment_Content;
                        Services_Comment(serviceComments);
                    }

                    serviceComments = new ServiceComments();
                    serviceComments.Service_Id = services.Service_Id;
                    serviceComments.Comment_Content = string.Format("Cancel task, Status update to {0}", system_Statuses.Status_Name);
                    res = Services_Comment(serviceComments);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetClose(Guid id)
        {
            try
            {
                bool res = new bool();
                Guid? nextId = id;
                while (nextId.HasValue)
                {
                    Services services = new Services();
                    services = db.Services.Find(nextId.Value);
                    if (services.Status_Id == 3)
                    {
                        services.Status_Id = 4;
                        services.Update = DateTime.Now;
                        db.Entry(services).State = System.Data.Entity.EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            if (Services_SetSatisfaction(nextId.Value))
                            {
                                ServiceComments serviceComments = new ServiceComments();
                                serviceComments.Service_Id = nextId.Value;
                                serviceComments.Comment_Content = "Close job";
                                res = Services_Comment(serviceComments);
                            }
                        }
                    }

                    if (services.Ref_Service_Id.HasValue)
                    {
                        nextId = services.Ref_Service_Id.Value;
                    }
                    else
                    {
                        nextId = null;
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetCommit(Services model)
        {
            try
            {
                bool res = new bool();
                if (model.Action_User_Id.HasValue)
                {
                    res = Services_SetToUser(model.Service_Id, model.Department_Id.Value, model.Action_User_Id.Value);
                }
                else
                {
                    res = Services_SetToDepartment(model.Service_Id, model.Department_Id);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetComplete(ServiceComments model)
        {
            try
            {
                bool res = new bool();
                Services services = new Services();
                services = db.Services.Find(model.Service_Id);

                System_Statuses system_Statuses = new System_Statuses();
                system_Statuses = db.System_Statuses
                    .Where(w => w.Status_Id == 3)
                    .FirstOrDefault();

                services.Update = DateTime.Now;
                services.Status_Id = system_Statuses.Status_Id;
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    if (!string.IsNullOrEmpty(model.Comment_Content))
                    {
                        serviceComments = new ServiceComments();
                        serviceComments.Service_Id = services.Service_Id;
                        serviceComments.Comment_Content = model.Comment_Content;
                        Services_Comment(serviceComments);
                    }

                    serviceComments = new ServiceComments();
                    serviceComments.Service_Id = services.Service_Id;
                    serviceComments.Comment_Content = string.Format("Complete task, Status update to {0}", system_Statuses.Status_Name);
                    if (Services_Comment(serviceComments))
                    {
                        var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                        linkUrl += "/" + services.Service_Id;
                        linkUrl = linkUrl.Replace("SetComplete", "ServiceInfomation");

                        string subject = string.Format("[E2E][Require close job] {0} - {1}", services.Service_Key, services.Service_Subject);
                        string content = string.Format("<p><b>Comment:</b> {0}<br />{1}", model.Comment_Content, serviceComments.Comment_Content);
                        content += "</p>";
                        content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                        content += "<p>Thank you for your consideration</p>";
                        res = mail.SendMail(services.User_Id, subject, content);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetFreePoint(Guid id)
        {
            try
            {
                bool res = new bool();
                Services services = new Services();
                services = db.Services.Find(id);
                services.Is_FreePoint = true;
                services.Update = DateTime.Now;
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    serviceComments.Service_Id = id;
                    serviceComments.Comment_Content = "This request is not deducted points.";
                    if (Services_Comment(serviceComments))
                    {
                        int point = db.System_Priorities.Find(services.Priority_Id).Priority_Point;
                        Users users = db.Users.Find(services.User_Id);
                        users.User_Point += point;
                        db.Entry(users).State = System.Data.Entity.EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            serviceComments = new ServiceComments();
                            serviceComments.Service_Id = id;
                            serviceComments.Comment_Content = string.Format("Give back {0} points to {1}", point, master.Users_GetInfomation(services.User_Id));
                            res = Services_Comment(serviceComments);
                        }
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetPending(ServiceComments model)
        {
            try
            {
                bool res = new bool();
                Services services = new Services();
                services = db.Services.Find(model.Service_Id);

                services.Update = DateTime.Now;
                services.Action_User_Id = null;
                services.Status_Id = 1;
                services.Service_EstimateTime = 0;
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    if (!string.IsNullOrEmpty(model.Comment_Content))
                    {
                        serviceComments = new ServiceComments();
                        serviceComments.Service_Id = services.Service_Id;
                        serviceComments.Comment_Content = model.Comment_Content;

                        if (Services_Comment(serviceComments))
                        {
                            List<ServiceTeams> serviceTeams = db.ServiceTeams.Where(w => w.Service_Id == model.Service_Id).ToList();
                            foreach (var item in serviceTeams)
                            {
                                Service_DeleteTeam(item.Team_Id);
                            }
                        }
                    }

                    serviceComments = new ServiceComments();
                    serviceComments.Service_Id = services.Service_Id;
                    serviceComments.Comment_Content = string.Format("Return job to department {0}", services.Master_Departments.Department_Name);
                    res = Services_Comment(serviceComments);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetReject(ServiceComments model)
        {
            try
            {
                bool res = new bool();
                Services services = new Services();
                services = db.Services.Find(model.Service_Id);
                if (!services.Department_Id.HasValue)
                {
                    Services_SetToDepartment(model.Service_Id);
                }

                System_Statuses system_Statuses = new System_Statuses();
                system_Statuses = db.System_Statuses
                    .Where(w => w.Status_Id == 5)
                    .FirstOrDefault();

                services.Update = DateTime.Now;
                services.Status_Id = system_Statuses.Status_Id;
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    if (!string.IsNullOrEmpty(model.Comment_Content))
                    {
                        serviceComments = new ServiceComments();
                        serviceComments.Service_Id = services.Service_Id;
                        serviceComments.Comment_Content = model.Comment_Content;
                        Services_Comment(serviceComments);
                    }

                    serviceComments = new ServiceComments();
                    serviceComments.Service_Id = services.Service_Id;
                    serviceComments.Comment_Content = string.Format("Reject task, Status update to {0}", system_Statuses.Status_Name);
                    if (Services_Comment(serviceComments))
                    {
                        var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                        linkUrl += "/" + services.Service_Id;
                        linkUrl = linkUrl.Replace("SetReject", "ServiceInfomation");

                        string subject = string.Format("[E2E][Reject] {0} - {1}", services.Service_Key, services.Service_Subject);
                        string content = string.Format("<p><b>Comment:</b> {0}<br />{1}", model.Comment_Content, serviceComments.Comment_Content);
                        content += "</p>";
                        content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                        content += "<p>Thank you for your consideration</p>";
                        res = mail.SendMail(services.User_Id, subject, content);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetRequired(ServiceComments model)
        {
            try
            {
                bool res = new bool();

                Services services = new Services();
                services = db.Services.Find(model.Service_Id);
                services.Is_MustBeApproved = true;
                services.Update = DateTime.Now;
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    serviceComments.Service_Id = model.Service_Id;
                    serviceComments.Comment_Content = string.Format("Approval required, \n {0}", model.Comment_Content);
                    if (Services_Comment(serviceComments))
                    {
                        string deptName = db.Users.Find(services.User_Id).Master_Processes.Master_Sections.Master_Departments.Department_Name;
                        List<Guid> sendTo = db.Users
                            .Where(w => w.Master_Processes.Master_Sections.Master_Departments.Department_Name == deptName && w.Master_Grades.Master_LineWorks.Authorize_Id == 2)
                            .Select(s => s.User_Id)
                            .ToList();
                        sendTo.Add(services.User_Id);

                        var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                        linkUrl += "/" + services.Service_Id;
                        linkUrl = linkUrl.Replace("SetMustApprove", "Approve_Form");

                        string subject = string.Format("[E2E][Require approve] {0} - {1}", services.Service_Key, services.Service_Subject);
                        string content = string.Format("<p><b>Description:</b> {0}", services.Service_Description);
                        content += "<br />";
                        content += "<br />";
                        content += string.Format("<b>Comment:</b> {0}", serviceComments.Comment_Content);
                        content += "</p>";
                        content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                        content += "<p>Thank you for your consideration</p>";
                        res = mail.SendMail(sendTo: sendTo, strSubject: subject, strContent: content);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetReturnJob(ServiceComments model)
        {
            try
            {
                bool res = new bool();
                Services services = new Services();
                services = db.Services.Find(model.Service_Id);

                services.Update = DateTime.Now;
                services.Is_Commit = false;
                services.Department_Id = null;
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    if (!string.IsNullOrEmpty(model.Comment_Content))
                    {
                        serviceComments = new ServiceComments();
                        serviceComments.Service_Id = services.Service_Id;
                        serviceComments.Comment_Content = model.Comment_Content;
                        Services_Comment(serviceComments);
                    }

                    serviceComments = new ServiceComments();
                    serviceComments.Service_Id = services.Service_Id;
                    serviceComments.Comment_Content = "Return job to center room";
                    res = Services_Comment(serviceComments);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetSatisfaction(Guid id)
        {
            try
            {
                bool res = new bool();
                Satisfactions satisfactions = new Satisfactions();
                satisfactions.Service_Id = id;
                List<Master_InquiryTopics> master_InquiryTopics = db.Master_InquiryTopics.OrderBy(o => o.InquiryTopic_Index).ToList();
                List<SatisfactionDetails> dataList = new List<SatisfactionDetails>();
                foreach (var item in master_InquiryTopics)
                {
                    SatisfactionDetails satisfactionDetails = new SatisfactionDetails();
                    satisfactionDetails.Satisfaction_Id = satisfactions.Satisfaction_Id;
                    satisfactionDetails.InquiryTopic_Id = item.InquiryTopic_Id;
                    satisfactionDetails.Point = 5;
                    dataList.Add(satisfactionDetails);
                }
                satisfactions.Satisfaction_Average = dataList.Sum(s => s.Point) / master_InquiryTopics.Count();
                db.Entry(satisfactions).State = System.Data.Entity.EntityState.Added;
                db.SatisfactionDetails.AddRange(dataList);
                if (db.SaveChanges() > 0)
                {
                    res = true;
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetToDepartment(Guid id, Guid? deptId = null)
        {
            try
            {
                bool res = new bool();
                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                if (!deptId.HasValue)
                {
                    deptId = db.Users.Find(userId).Master_Processes.Master_Sections.Department_Id.Value;
                }

                string deptName = db.Master_Departments.Find(deptId).Department_Name;

                Services services = new Services();
                services = db.Services.Find(id);
                services.Department_Id = deptId;
                services.Is_Commit = true;
                services.Update = DateTime.Now;
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    serviceComments.Service_Id = services.Service_Id;
                    serviceComments.Comment_Content = string.Format("Commit Task, Assign task to the {0} department", deptName);
                    res = Services_Comment(serviceComments);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetToUser(Guid id, Guid deptId, Guid userId)
        {
            try
            {
                bool res = new bool();
                Guid userSetId = Guid.Parse(HttpContext.Current.User.Identity.Name);

                string deptName = db.Master_Departments.Find(deptId).Department_Name;

                Services services = new Services();
                services = db.Services.Find(id);
                services.Department_Id = deptId;
                services.Action_User_Id = userId;
                services.Is_Commit = true;
                services.Update = DateTime.Now;
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    serviceComments.Service_Id = services.Service_Id;
                    serviceComments.Comment_Content = string.Format("Commit Task, Assign task to the {0} department, Assign task to {1}", deptName, master.Users_GetInfomation(userId));

                    if (Services_Comment(serviceComments))
                    {
                        var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                        linkUrl = linkUrl.Replace("Commit", "Action");

                        string subject = string.Format("[E2E][Assign] {0} - {1}", services.Service_Key, services.Service_Subject);
                        string content = string.Format("<p><b>To:</b> {0}", master.Users_GetInfomation(userId));
                        content += "<br />";
                        content += string.Format("<b>Description:</b> {0}", services.Service_Description);
                        content += "<br />";
                        content += "<br />";
                        content += string.Format("<b>Comment:</b> {0}", serviceComments.Comment_Content);
                        content += "</p>";
                        content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                        content += "<p>Thank you for your consideration</p>";
                        res = mail.SendMail(userId, subject, content);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_Update(Services model, HttpFileCollectionBase files)
        {
            try
            {
                bool res = new bool();
                model.Update = DateTime.Now;

                if (files[0].ContentLength > 0)
                {
                    model.Service_FileCount += files.Count;
                    for (int i = 0; i < files.Count; i++)
                    {
                        ServiceFiles serviceFiles = new ServiceFiles();
                        serviceFiles.Service_Id = model.Service_Id;
                        serviceFiles.ServiceFile_Name = files[i].FileName;
                        string dir = string.Format("Service/{0}/", model.Service_Id);
                        serviceFiles.ServiceFile_Path = ftp.Ftp_UploadFileToString(dir, files[i]);
                        serviceFiles.ServiceFile_Extension = Path.GetExtension(files[i].FileName);
                        db.Entry(serviceFiles).State = System.Data.Entity.EntityState.Added;
                    }
                }

                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    res = true;
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Services Services_View(Guid id)
        {
            try
            {
                return db.Services.Find(id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}