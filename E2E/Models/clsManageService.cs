using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace E2E.Models
{
    public class ClsManageService
    {
        private readonly ClsMail clsMail = new ClsMail();
        private readonly ClsContext db = new ClsContext();
        private readonly ClsServiceFTP ftp = new ClsServiceFTP();
        private readonly ClsManageMaster master = new ClsManageMaster();

        private IQueryable<Services> Services_GetAllRequest_IQ()
        {
            try
            {
                Guid id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                string departmentName = db.Users
                    .Where(w => w.User_Id == id)
                    .Select(s => s.Master_Processes.Master_Sections.Master_Departments.Department_Name)
                    .FirstOrDefault();
                IQueryable<Guid> userIds = db.Users
                    .Where(w => w.Master_Processes.Master_Sections.Master_Departments.Department_Name == departmentName)
                    .Select(s => s.User_Id);
                IQueryable<Services> query = db.Services
                    .Where(w => userIds.Contains(w.User_Id));

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

        private IQueryable<ServiceTeams> ServiceTeams_IQ(Guid id)
        {
            return db.ServiceTeams
                    .Where(w => w.Service_Id == id);
        }

        public ClsSwal CheckMissingDocument(Guid id)
        {
            ClsSwal swal = new ClsSwal();
            try
            {
                List<ServiceDocuments> serviceDocuments = db.ServiceDocuments
                    .Where(w => w.Service_Id == id && string.IsNullOrEmpty(w.ServiceDocument_Name) && w.Master_Documents.Required)
                    .ToList();

                if (serviceDocuments.Count > 0)
                {
                    swal.Option = false;
                    swal.Icon = "warning";
                    swal.Title = "กรุณาอัพโหลดไฟล์ที่จำเป็น";
                    foreach (var item in serviceDocuments)
                    {
                        swal.Text += string.Format("- {0}\n", item.Master_Documents.Document_Name);
                    }
                }
                else
                {
                    swal.Option = true;
                }
            }
            catch (Exception ex)
            {
                swal.Title = ex.Source;
                swal.Text = ex.Message;
                Exception inner = ex.InnerException;
                while (inner != null)
                {
                    swal.Title = inner.Source;
                    swal.Text += string.Format("\n{0}", inner.Message);
                    inner = inner.InnerException;
                }
            }

            return swal;
        }

        public ClsReportKPI ClsReportKPI_ViewList(ReportKPI_Filter filter)
        {
            try
            {
                ClsReportKPI res = new ClsReportKPI();

                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                IQueryable<Guid> userIds;

                IQueryable<Guid> serviceIds;
                IQueryable<Services> query = db.Services.OrderBy(o => o.Create).ThenBy(t => t.Update);

                res.Authorize_Id = db.Users
                    .Where(w => w.User_Id == userId)
                    .Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id)
                    .FirstOrDefault();

                int[] finishIds = { 3, 4 };

                string deptName = db.Users
                        .Where(w => w.User_Id == userId)
                        .Select(s => s.Master_Processes.Master_Sections.Master_Departments.Department_Name)
                        .FirstOrDefault();
                List<Guid> deptIds = db.Master_Departments
                    .Where(w => w.Department_Name == deptName)
                    .Select(s => s.Department_Id)
                    .ToList();
                userIds = db.Users
                    .Where(w => deptIds.Contains(w.Master_Processes.Master_Sections.Department_Id))
                    .OrderBy(o => o.User_Code)
                    .Select(s => s.User_Id);

                query = query.Where(w => userIds.Contains(w.Action_User_Id.Value));

                if (filter != null)
                {
                    if (filter.Date_From.HasValue)
                    {
                        query = query.Where(w => w.Create >= filter.Date_From);
                    }

                    filter.Date_To = filter.Date_To.AddDays(1);
                    query = query.Where(w => w.Create <= filter.Date_To);
                }

                if (res.Authorize_Id != 3)
                {
                    foreach (var item in userIds)
                    {
                        serviceIds = query
                            .Where(w => w.Action_User_Id == item)
                            .Select(s => s.Service_Id);

                        ReportKPI_User reportKPI_User = new ReportKPI_User();

                        if (serviceIds.Count() > 0)
                        {
                            int countSatisfaction = db.Satisfactions
                        .Where(w => serviceIds.Contains(w.Service_Id))
                        .Count();
                            if (countSatisfaction > 0)
                            {
                                reportKPI_User.Average_Score = db.Satisfactions
                        .Where(w => serviceIds.Contains(w.Service_Id))
                        .Average(a => a.Satisfaction_Average);
                            }

                            if (query.Any(w => serviceIds.Contains(w.Service_Id) && finishIds.Contains(w.Status_Id)))
                            {
                                reportKPI_User.SuccessPoint = query
                                    .Where(w => serviceIds.Contains(w.Service_Id) && finishIds.Contains(w.Status_Id))
                                    .Sum(s => s.System_Priorities.Priority_Point);
                            }

                            reportKPI_User.Close_Count = query.Where(w => w.Status_Id == 4 && serviceIds.Contains(w.Service_Id)).Count();
                            reportKPI_User.Complete_Count = query.Where(w => w.Status_Id == 3 && serviceIds.Contains(w.Service_Id)).Count();
                            reportKPI_User.Inprogress_Count = query.Where(w => w.Status_Id == 2 && serviceIds.Contains(w.Service_Id)).Count();
                            reportKPI_User.Pending_Count = query.Where(w => w.Status_Id == 1 && serviceIds.Contains(w.Service_Id)).Count();
                            reportKPI_User.Total = query.Where(w => serviceIds.Contains(w.Service_Id)).Count();
                            reportKPI_User.OverDue_Count = query.Where(w => w.Is_OverDue && serviceIds.Contains(w.Service_Id)).Count();
                        }

                        serviceIds = query
                        .Where(w => w.Action_User_Id != item)
                        .Select(s => s.Service_Id);

                        reportKPI_User.JoinTeam_Count = db.ServiceTeams
                            .Where(w => serviceIds.Contains(w.Service_Id) && w.User_Id == item)
                            .Count();

                        reportKPI_User.User_Id = item;
                        reportKPI_User.User_Name = master.Users_GetInfomation(item);
                        res.ReportKPI_Users.Add(reportKPI_User);
                    }
                }
                else
                {
                    serviceIds = query
                        .Where(w => w.Action_User_Id == userId)
                        .Select(s => s.Service_Id);

                    ReportKPI_User reportKPI_User = new ReportKPI_User();

                    if (serviceIds.Count() > 0)
                    {
                        int countSatisfaction = db.Satisfactions
                        .Where(w => serviceIds.Contains(w.Service_Id))
                        .Count();

                        if (countSatisfaction > 0)
                        {
                            reportKPI_User.Average_Score = db.Satisfactions
                                .Where(w => serviceIds.Contains(w.Service_Id))
                                .Average(a => a.Satisfaction_Average);
                        }

                        int successCount = query
                            .Where(w => finishIds.Contains(w.Status_Id) && serviceIds.Contains(w.Service_Id))
                            .Count();
                        if (successCount > 0)
                        {
                            reportKPI_User.SuccessPoint = query
                            .Where(w => finishIds.Contains(w.Status_Id) && serviceIds.Contains(w.Service_Id))
                        .Sum(s => s.System_Priorities.Priority_Point);
                        }

                        reportKPI_User.Close_Count = query.Where(w => w.Status_Id == 4 && serviceIds.Contains(w.Service_Id)).Count();
                        reportKPI_User.Complete_Count = query.Where(w => w.Status_Id == 3 && serviceIds.Contains(w.Service_Id)).Count();
                        reportKPI_User.Inprogress_Count = query.Where(w => w.Status_Id == 2 && serviceIds.Contains(w.Service_Id)).Count();
                        reportKPI_User.Pending_Count = query.Where(w => w.Status_Id == 1 && serviceIds.Contains(w.Service_Id)).Count();
                        reportKPI_User.Total = query.Where(w => serviceIds.Contains(w.Service_Id)).Count();
                        reportKPI_User.OverDue_Count = query.Where(w => w.Is_OverDue && serviceIds.Contains(w.Service_Id)).Count();
                    }

                    serviceIds = query
                        .Where(w => w.Action_User_Id != userId)
                        .Select(s => s.Service_Id);

                    reportKPI_User.JoinTeam_Count = db.ServiceTeams
                        .Where(w => serviceIds.Contains(w.Service_Id) && w.User_Id == userId)
                        .Count();

                    reportKPI_User.User_Id = userId;
                    reportKPI_User.User_Name = master.Users_GetInfomation(userId);
                    res.ReportKPI_Users.Add(reportKPI_User);
                }

                res.ReportKPI_Overview.Close_Count = res.ReportKPI_Users.Select(s => s.Close_Count).Sum();
                res.ReportKPI_Overview.Complete_Count = res.ReportKPI_Users.Select(s => s.Complete_Count).Sum();
                res.ReportKPI_Overview.Inprogress_Count = res.ReportKPI_Users.Select(s => s.Inprogress_Count).Sum();
                res.ReportKPI_Overview.Pending_Count = res.ReportKPI_Users.Select(s => s.Pending_Count).Sum();
                res.ReportKPI_Overview.Total = res.ReportKPI_Users.Select(s => s.Total).Sum();
                res.ReportKPI_Overview.OverDue_Count = res.ReportKPI_Users.Select(s => s.OverDue_Count).Sum();
                int ontimeCount = res.ReportKPI_Overview.Total - res.ReportKPI_Overview.OverDue_Count;
                res.ReportKPI_Overview.OnTime_Count = ontimeCount;
                double ontimePercent = Convert.ToDouble(ontimeCount) / Convert.ToDouble(res.ReportKPI_Overview.Total);
                res.ReportKPI_Overview.OnTime_Percent = ontimePercent;

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ClsSatisfaction ClsSatisfaction_View(Guid id)
        {
            try
            {
                ClsSatisfaction clsSatisfaction = new ClsSatisfaction();
                clsSatisfaction = db.Satisfactions
                    .Where(w => w.Service_Id == id)
                    .GroupJoin(db.SatisfactionDetails.OrderBy(o => o.Master_InquiryTopics.InquiryTopic_Index), m => m.Satisfaction_Id, j => j.Satisfaction_Id, (m, gj) => new ClsSatisfaction()
                    {
                        SatisfactionDetails = gj.ToList(),
                        Satisfactions = m
                    }).FirstOrDefault();

                return clsSatisfaction;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ClsServices ClsServices_View(Guid id)
        {
            try
            {
                ClsServices clsServices = db.Services
                    .Where(w => w.Service_Id == id)
                    .GroupJoin(db.ServiceFiles, m => m.Service_Id, j => j.Service_Id, (m, gj) => new ClsServices
                    {
                        Services = m,
                        ServiceFiles = gj.ToList()
                    }).SingleOrDefault();

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

                clsServices.ClsServiceTeams = ServiceTeams_IQ(id).ToList()
                    .Select(s => new ClsServiceTeams()
                    {
                        ServiceTeams = s,
                        User_Name = master.Users_GetInfomation(s.User_Id)
                    }).ToList();

                return clsServices;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ClsServices ClsServices_ViewComment(Guid id)
        {
            try
            {
                ClsServices clsServices = new ClsServices
                {
                    Services = db.Services.Find(id),
                    ClsServiceComments = db.ServiceComments
                    .Where(w => w.Service_Id == id)
                    .GroupJoin(db.ServiceCommentFiles, m => m.ServiceComment_Id, j => j.ServiceComment_Id, (m, gj) => new ClsServiceComments()
                    {
                        ServiceComments = m,
                        ServiceCommentFiles = gj.ToList()
                    })
                    .OrderBy(o => o.ServiceComments.Create)
                    .ToList()
                };

                clsServices.ClsServiceComments.ForEach(f => f.User_Name = master.Users_GetInfomation(f.ServiceComments.User_Id.Value));

                return clsServices;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ClsServices> ClsServices_ViewList(Guid id)
        {
            try
            {
                List<ClsServices> res = new List<ClsServices>();
                Guid? refId = id;
                while (refId.HasValue)
                {
                    ClsServices clsServices = new ClsServices();
                    clsServices = db.Services
                        .Where(w => w.Service_Id == refId.Value)
                        .Select(s => new ClsServices()
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

        public List<ClsServices> ClsServices_ViewRefList(Guid id)
        {
            try
            {
                List<ClsServices> res = new List<ClsServices>();
                Guid? refId = db.Services.Find(id).Ref_Service_Id;
                while (refId.HasValue)
                {
                    ClsServices clsServices = new ClsServices();
                    clsServices = db.Services
                        .Where(w => w.Service_Id == refId.Value)
                        .Select(s => new ClsServices()
                        {
                            Services = s,
                            ServiceFiles = db.ServiceFiles.Where(w => w.Service_Id == s.Service_Id).ToList()
                        }).FirstOrDefault();
                    clsServices.User_Name = master.Users_GetInfomation(clsServices.Services.User_Id);
                    clsServices.Create_Name = master.Users_GetInfomation(clsServices.Services.Create_User_Id);

                    UserDetails userDetails = new UserDetails();
                    userDetails = db.UserDetails
                        .Where(w => w.User_Id == clsServices.Services.Action_User_Id)
                        .FirstOrDefault();

                    clsServices.Action_Email = userDetails.Users.User_Email;
                    clsServices.Action_Name = master.Users_GetInfomation(userDetails.User_Id);

                    foreach (var item in ServiceTeams_IQ(refId.Value))
                    {
                        ClsServiceTeams clsServiceTeams = new ClsServiceTeams
                        {
                            ServiceTeams = item,
                            User_Name = master.Users_GetInfomation(item.User_Id)
                        };
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

        public List<ReportKPI_User_Views> ReportKPI_User_Views(Guid id, ReportKPI_Filter filter)
        {
            try
            {
                IQueryable<ReportKPI_User_Views> query = db.Services
                    .Where(w => w.Action_User_Id == id)
                    .GroupJoin(db.Satisfactions, ser => ser.Service_Id, sat => sat.Service_Id, (ser, g) => new
                    {
                        ser,
                        g
                    }).SelectMany(tmp => tmp.g.DefaultIfEmpty(), (tmp, sat) => new ReportKPI_User_Views()
                    {
                        Service_Id = tmp.ser.Service_Id,
                        Create = tmp.ser.Create,
                        Service_Subject = tmp.ser.Service_Subject,
                        Service_Key = tmp.ser.Service_Key,
                        Priority_Point = tmp.ser.System_Priorities.Priority_Point,
                        Satisfaction_Average = sat.Satisfaction_Average,
                        Status_Name = tmp.ser.System_Statuses.Status_Name,
                        Status_Class = tmp.ser.System_Statuses.Status_Class
                    }).OrderBy(o => o.Create);

                if (filter != null)
                {
                    if (filter.Date_From.HasValue)
                    {
                        query = query.Where(w => w.Create >= filter.Date_From);
                    }

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

        public bool SaveEstimate(Guid id, List<ClsEstimate> score)
        {
            try
            {
                bool res = new bool();

                var average = score.Select(x => x.Score).Average();

                Satisfactions satisfactions = new Satisfactions
                {
                    Service_Id = id,
                    Satisfaction_Average = average
                };
                db.Satisfactions.Add(satisfactions);

                foreach (var item in score)
                {
                    SatisfactionDetails satisfactionDetails = new SatisfactionDetails
                    {
                        Satisfaction_Id = satisfactions.Satisfaction_Id,
                        InquiryTopic_Id = item.Id,
                        Point = item.Score
                    };

                    db.SatisfactionDetails.Add(satisfactionDetails);
                }

                if (db.SaveChanges() > 0)
                {
                    res = Services_SetClose(id, score);
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

                List<SelectListItem> res = new List<SelectListItem>
                {
                    new SelectListItem()
                    {
                        Text = "Select Reference",
                        Value = ""
                    }
                };
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
                string deptName = db.Users
                    .Where(w => w.User_Id == userId)
                    .Select(s => s.Master_Processes.Master_Sections.Master_Departments.Department_Name)
                    .FirstOrDefault();
                IQueryable<Guid> deptIds = db.Master_Departments
                    .Where(w => w.Department_Name == deptName)
                    .Select(s => s.Department_Id);

                IQueryable<Guid> userIdInTeam = ServiceTeams_IQ(id)
                    .Select(s => s.User_Id);

                return db.Users
                    .Where(w => deptIds.Contains(w.Master_Processes.Master_Sections.Department_Id) &&
                    w.Active &&
                    !userIdInTeam.Contains(w.User_Id) &&
                    w.User_Id != userId)
                    .AsEnumerable()
                    .Select(s => new SelectListItem()
                    {
                        Text = master.Users_GetInfomation(s.User_Id),
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

        public List<SelectListItem> SelectListItems_UsersDepartment()
        {
            try
            {
                List<SelectListItem> listItems = new List<SelectListItem>();
                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                string deptName = db.Users
                        .Where(w => w.User_Id == userId)
                        .Select(s => s.Master_Processes.Master_Sections.Master_Departments.Department_Name)
                        .FirstOrDefault();
                List<Guid> deptIds = db.Master_Departments
                    .Where(w => w.Department_Name == deptName)
                    .Select(s => s.Department_Id)
                    .ToList();
                listItems = db.Users
                    .Where(w => deptIds.Contains(w.Master_Processes.Master_Sections.Department_Id))
                    .OrderBy(o => o.User_Code)
                    .AsEnumerable()
                    .Select(s => new SelectListItem()
                    {
                        Text = master.Users_GetInfomation(s.User_Id),
                        Value = s.User_Id.ToString()
                    })
                    .ToList();

                return listItems;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Service_AddTeam(ClsServiceTeams model, string methodName)
        {
            try
            {
                bool res = new bool();
                string Getteam = string.Empty;
                foreach (var item in model.User_Ids)
                {
                    ServiceTeams serviceTeams = new ServiceTeams
                    {
                        Service_Id = model.Service_Id,
                        User_Id = item
                    };
                    db.Entry(serviceTeams).State = EntityState.Added;
                    if (db.SaveChanges() > 0)
                    {
                        ServiceComments serviceComments = new ServiceComments
                        {
                            Service_Id = model.Service_Id,
                            Comment_Content = string.Format("Add {0} to join team", master.Users_GetInfomation(item))
                        };
                        res = Services_Comment(serviceComments);
                    }
                }

                var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                linkUrl += "/" + model.Service_Id;
                linkUrl = linkUrl.Replace(methodName, "ServiceInfomation");

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

                string subject = string.Format("[Notify add team] {0} - {1}", services.Service_Key, services.Service_Subject);
                string content = string.Format("<p><b>Description:</b> {0}", services.Service_Description);
                content += "<br />";
                content += "<br />";
                content += "<b>Current member</b><br/>";
                content += Getteam;
                content += "</p>";
                content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                content += "<p>Thank you for your consideration</p>";
                clsMail.SendToIds = listTeam;
                clsMail.Subject = subject;
                clsMail.Body = content;
                res = clsMail.SendMail(clsMail);

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

        public bool Service_DeleteTeam(Guid id, string methodName)
        {
            try
            {
                bool res = new bool();
                string getTeam = string.Empty;
                ServiceTeams serviceTeams = new ServiceTeams();
                serviceTeams = db.ServiceTeams.Find(id);
                string userName = master.Users_GetInfomation(serviceTeams.User_Id);
                Guid serviceId = serviceTeams.Service_Id;
                db.Entry(serviceTeams).State = EntityState.Deleted;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments
                    {
                        Service_Id = serviceId,
                        Comment_Content = string.Format("Delete {0} from this team", userName)
                    };
                    if (Services_Comment(serviceComments))
                    {
                        var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                        linkUrl = linkUrl.Replace(methodName, "ServiceInfomation");
                        if (!linkUrl.EndsWith(serviceId.ToString()))
                        {
                            linkUrl += string.Format("/{0}", serviceId);
                        }

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

                        string subject = string.Format("[Notify delete member] {0} - {1}", services.Service_Key, services.Service_Subject);
                        string content = "<p><b>Delete: </b>" + userName;
                        content += "<br />";
                        content += "<br />";
                        content += "<b>Current member</b><br/>";
                        content += getTeam;
                        content += "</p>";
                        content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                        content += "<p>Thank you for your consideration</p>";
                        clsMail.SendToIds = listTeam;
                        clsMail.Subject = subject;
                        clsMail.Body = content;
                        res = clsMail.SendMail(clsMail);
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
                db.Entry(serviceChangeDueDate).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments
                    {
                        Comment_Content = "Accept due date change request",
                        Service_Id = serviceChangeDueDate.Service_Id,
                        User_Id = userId
                    };
                    if (Services_Comment(serviceComments))
                    {
                        Services services = db.Services.Find(serviceChangeDueDate.Service_Id);
                        services.Service_DueDate = serviceChangeDueDate.DueDate_New;
                        services.Update = DateTime.Now;
                        db.Entry(services).State = EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            serviceComments = new ServiceComments
                            {
                                Comment_Content = string.Format("Change due date from {0} to {1}", serviceChangeDueDate.DueDate.ToString("d"), serviceChangeDueDate.DueDate_New.Value.ToString("d")),
                                Service_Id = serviceChangeDueDate.Service_Id,
                                User_Id = userId
                            };
                            res = Services_Comment(serviceComments);

                            var Sendto = db.ServiceComments.Where(w => w.Service_Id == serviceComments.Service_Id && w.Comment_Content.StartsWith("Request change due date from")).OrderByDescending(o => o.Create).FirstOrDefault();

                            var linkUrl = HttpContext.Current.Request.Url.OriginalString;

                            string[] cut = linkUrl.Split('/');

                            linkUrl = linkUrl.Replace("RequestChangeDue_Accept/" + cut[5], "Action");
                            linkUrl += "/" + serviceComments.Service_Id;

                            string subject = string.Format("[Accept Request change due] {0} - {1}", services.Service_Key, services.Service_Subject);
                            string content = string.Format("<p><b>Comment: </b> {0}<br />", serviceComments.Comment_Content);
                            content += "</p>";
                            content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                            content += "<p>Thank you for your consideration</p>";
                            clsMail.SendToId = Sendto.User_Id;
                            clsMail.Subject = subject;
                            clsMail.Body = content;
                            res = clsMail.SendMail(clsMail);
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
                db.Entry(serviceChangeDueDate).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments
                    {
                        Comment_Content = "Cancel due date change request",
                        Service_Id = serviceChangeDueDate.Service_Id,
                        User_Id = userId
                    };
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
                ServiceChangeDueDate serviceChangeDueDate = new ServiceChangeDueDate
                {
                    DueDate = db.Services.Find(id).Service_DueDate.Value,
                    Service_Id = id
                };
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
                db.Entry(serviceChangeDueDate).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments
                    {
                        Comment_Content = "Reject due date change request",
                        Service_Id = serviceChangeDueDate.Service_Id,
                        User_Id = userId
                    };
                    res = Services_Comment(serviceComments);

                    var Sendto = db.ServiceComments.Where(w => w.Service_Id == serviceComments.Service_Id && w.Comment_Content.StartsWith("Request change due date from")).OrderByDescending(o => o.Create).FirstOrDefault();

                    var linkUrl = HttpContext.Current.Request.Url.OriginalString;

                    string[] cut = linkUrl.Split('/');

                    linkUrl = linkUrl.Replace("RequestChangeDue_Reject/" + cut[5], "Action");
                    linkUrl += "/" + serviceComments.Service_Id;

                    string subject = string.Format("[Reject Request change due] {0} - {1}", Sendto.Services.Service_Key, Sendto.Services.Service_Subject);
                    string content = string.Format("<p><b>Comment: </b> {0}<br />", "Reject due date change request");
                    content += "</p>";
                    content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                    content += "<p>Thank you for your consideration</p>";
                    clsMail.SendToId = Sendto.User_Id;
                    clsMail.Subject = subject;
                    clsMail.Body = content;
                    res = clsMail.SendMail(clsMail);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ServiceChangeDueDate_Request(ServiceChangeDueDate model, string methodName)
        {
            try
            {
                bool res = new bool();
                DateTime dateTime = DateTime.Now;
                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                model.User_Id = userId;
                db.Entry(model).State = EntityState.Added;
                if (db.SaveChanges() > 0)
                {
                    string Comment = string.Format("Request change due date from {0} to {1}", model.DueDate.ToString("d"), model.DueDate_New.Value.ToString("d"));

                    ServiceComments serviceComments = new ServiceComments
                    {
                        Service_Id = model.Service_Id,
                        Comment_Content = string.Format("{0}{1}{2}Remark: {3}", Comment, Environment.NewLine, Environment.NewLine, model.Remark),
                        User_Id = userId
                    };
                    if (Services_Comment(serviceComments))
                    {
                        Services services = new Services();
                        services = db.Services.Find(model.Service_Id);

                        var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                        linkUrl = linkUrl.Replace(methodName, "RequestChangeDue");

                        string subject = string.Format("[Request change due date] {0} - {1}", services.Service_Key, services.Service_Subject);
                        string content = string.Format("<p><b>Description:</b> {0}</p>", Comment);
                        content += string.Format("<p><b>Remark:</b> {0}</p>", model.Remark);

                        content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                        content += "<p>Thank you for your consideration</p>";
                        clsMail.SendToId = services.User_Id;
                        clsMail.Subject = subject;
                        clsMail.Body = content;
                        res = clsMail.SendMail(clsMail);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ClsChangeDueDates> ServiceChangeDues_List()
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
                    .AsEnumerable()
                    .Select(s => new ClsChangeDueDates()
                    {
                        ChangeDueDate_Id = s.ChangeDueDate_Id,
                        Service_Key = s.Services.Service_Key,
                        Service_Subject = s.Services.Service_Subject,
                        Create = s.Create,
                        DueDateStatus_Class = s.System_DueDateStatuses.DueDateStatus_Class,
                        DueDateStatus_Name = s.System_DueDateStatuses.DueDateStatus_Name,
                        DueDate_New = s.DueDate_New,
                        DueDate_Old = s.DueDate,
                        Update = s.Update,
                        User_Name = master.Users_GetInfomation(s.User_Id),
                        Remark = s.Remark
                    })
                    .ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ClsChangeDueDates> ServiceChangeDues_ListCount()
        {
            try
            {
                Guid id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                List<Guid> ids = Services_GetAllRequest_IQ()
                    .Where(w => w.User_Id == id)
                    .Select(s => s.Service_Id)
                    .ToList();
                return db.ServiceChangeDueDates
                    .Where(w => ids.Contains(w.Service_Id) && w.DueDateStatus_Id == 1)
                    .AsEnumerable()
                    .Select(s => new ClsChangeDueDates()
                    {
                        ChangeDueDate_Id = s.ChangeDueDate_Id,
                        Service_Key = s.Services.Service_Key,
                        Service_Subject = s.Services.Service_Subject,
                        Create = s.Create,
                        DueDateStatus_Class = s.System_DueDateStatuses.DueDateStatus_Class,
                        DueDateStatus_Name = s.System_DueDateStatuses.DueDateStatus_Name,
                        DueDate_New = s.DueDate_New,
                        DueDate_Old = s.DueDate,
                        Update = s.Update,
                        User_Name = master.Users_GetInfomation(s.User_Id),
                        Remark = s.Remark
                    })
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
                db.Entry(services).State = EntityState.Modified;
                db.Entry(serviceFiles).State = EntityState.Deleted;
                if (ftp.Api_DeleteFile(serviceFiles.ServiceFile_Path))
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
                db.Entry(model).State = EntityState.Added;
                if (files != null)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        if (files[i].ContentLength == 0)
                        {
                            break;
                        }
                        ServiceCommentFiles serviceCommentFiles = new ServiceCommentFiles
                        {
                            ServiceCommentFile_Name = files[i].FileName
                        };
                        string dir = string.Format("Service/{0}/Comment/{1}/", db.Services.Find(model.Service_Id).Service_Key, DateTime.Today.ToString("yyMMdd"));
                        serviceCommentFiles.ServiceCommentFile_Path = ftp.Ftp_UploadFileToString(dir, files[i]);
                        serviceCommentFiles.ServiceComment_Id = model.ServiceComment_Id;
                        serviceCommentFiles.ServiceComment_Seq = i;
                        serviceCommentFiles.ServiceCommentFile_Extension = Path.GetExtension(files[i].FileName);
                        db.Entry(serviceCommentFiles).State = EntityState.Added;
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

        public IQueryable<Services> Services_GetAllTask_IQ()
        {
            try
            {
                if (!Guid.TryParse(HttpContext.Current.User.Identity.Name, out Guid id))
                {
                    // Handle the error if the user id cannot be parsed
                    throw new Exception("Invalid user id");
                }

                string deptName = master.GetDepartmentNameForUser(id);

                IQueryable<Guid> deptIds = db.Master_Departments
                    .Where(w => w.Department_Name == deptName)
                    .Select(s => s.Department_Id);

                IQueryable<Services> query = db.Services
                    .Where(w => deptIds.Contains(w.Department_Id.Value));

                return query;
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

                IQueryable<Guid> serviceTeams = db.ServiceTeams.Where(w => w.User_Id == id).Select(s => s.Service_Id);

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
                IQueryable<Guid> userIdList = db.Users
                    .Where(w => w.Master_Processes.Master_Sections.Master_Departments.Department_Name == deptName).Select(s => s.User_Id);
                return db.Services.Where(w => w.Is_MustBeApproved && w.Is_Approval == val && userIdList.Contains(w.User_Id) && w.Status_Id == 1).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<Services> Services_GetWaitAction_IQ(Guid? id)
        {
            try
            {
                IQueryable<Services> query = db.Services
                    .Where(service => service.Is_Commit && service.Status_Id == 1 && (service.Is_Approval || !service.Is_MustBeApproved))
                    .OrderByDescending(service => service.Priority_Id)
                    .ThenBy(service => new { service.Create, service.Service_DueDate });

                if (id.HasValue)
                {
                    string departmentName = master.GetDepartmentNameForUser(id.Value);

                    IQueryable<Guid> departmentIds = db.Master_Departments
                        .Where(department => department.Department_Name == departmentName)
                        .Select(department => department.Department_Id);

                    query = query.Where(service => (departmentIds.Contains(service.Department_Id.Value) && !service.Action_User_Id.HasValue) || service.Action_User_Id == id);
                }

                return query;
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

        public IQueryable<Services> Services_GetWaitCommit_IQ()
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
                    db.Entry(users).State = EntityState.Modified;
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
                        ServiceFiles serviceFiles = new ServiceFiles
                        {
                            Service_Id = model.Service_Id,
                            ServiceFile_Name = files[i].FileName
                        };
                        string dir = string.Format("Service/{0}/", model.Service_Key);
                        serviceFiles.ServiceFile_Path = ftp.Ftp_UploadFileToString(dir, files[i]);
                        serviceFiles.ServiceFile_Extension = Path.GetExtension(files[i].FileName);
                        db.Entry(serviceFiles).State = EntityState.Added;
                    }
                }

                db.Entry(model).State = EntityState.Added;

                if (db.SaveChanges() > 0)
                {
                    if (model.Ref_Service_Id.HasValue)
                    {
                        System_Statuses system_Statuses = new System_Statuses();
                        system_Statuses = db.System_Statuses
                            .Where(w => w.Status_Id == 3)
                            .FirstOrDefault();

                        ServiceComments serviceComments = new ServiceComments();

                        Services services = new Services();
                        services = db.Services.Find(model.Ref_Service_Id);
                        if (services.Status_Id == 2)
                        {
                            services.Status_Id = 3;
                            services.Update = DateTime.Now;
                            db.Entry(services).State = EntityState.Modified;
                            db.SaveChanges();

                            serviceComments = new ServiceComments
                            {
                                Service_Id = model.Ref_Service_Id.Value,
                                Comment_Content = string.Format("Complete task, Status update to {0}", system_Statuses.Status_Name)
                            };
                            Services_Comment(serviceComments);
                        }

                        serviceComments = new ServiceComments
                        {
                            Service_Id = model.Ref_Service_Id.Value,
                            Comment_Content = string.Format("Forward this job to new service key {0}", model.Service_Key)
                        };
                        res = Services_Comment(serviceComments);
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

        public bool Services_SaveDocumentControl(ServiceDocuments model, HttpFileCollectionBase files)
        {
            try
            {
                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                bool res = new bool();
                ServiceDocuments serviceDocuments = new ServiceDocuments();
                serviceDocuments = db.ServiceDocuments.Find(model.ServiceDocument_Id);
                serviceDocuments.ServiceDocument_Remark = model.ServiceDocument_Remark;
                serviceDocuments.User_Id = userId;
                serviceDocuments.Update = DateTime.Now;

                HttpPostedFileBase fileBase = files[0];
                if (fileBase.ContentLength > 0)
                {
                    serviceDocuments.ServiceDocument_Name = fileBase.FileName;
                    string dir = string.Format("Service/{0}/DocumentControls/", db.Services.Find(model.Service_Id).Service_Key);
                    serviceDocuments.ServiceDocument_Path = ftp.Ftp_UploadFileToString(dir, fileBase);
                }

                db.Entry(serviceDocuments).State = EntityState.Modified;
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
                        ServiceDocuments serviceDocuments = new ServiceDocuments
                        {
                            Document_Id = item.Value,
                            Service_Id = services.Service_Id
                        };
                        db.Entry(serviceDocuments).State = EntityState.Added;
                    }
                }

                services.Service_EstimateTime = model.Service_EstimateTime;
                services.Status_Id = system_Statuses.Status_Id;
                services.Update = DateTime.Now;
                db.Entry(services).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments
                    {
                        Service_Id = services.Service_Id,
                        Comment_Content = string.Format("Start task, Estimate time about {0} days, Status update to {1}", services.Service_EstimateTime, system_Statuses.Status_Name)
                    };
                    res = Services_Comment(serviceComments);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetApprove(ServiceComments model, string methodName)
        {
            try
            {
                bool res = new bool();
                Services services = new Services();
                services = db.Services.Find(model.Service_Id);
                services.Is_Approval = true;
                services.Update = DateTime.Now;
                db.Entry(services).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments
                    {
                        Service_Id = model.Service_Id,
                        Comment_Content = string.Format("Approved,\n{0}", model.Comment_Content)
                    };
                    if (Services_Comment(serviceComments))
                    {
                        var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                        linkUrl += "/" + services.Service_Id;
                        linkUrl = linkUrl.Replace(methodName, "Approve_Form");

                        string subject = string.Format("[Approval] {0} - {1}", services.Service_Key, services.Service_Subject);
                        string content = string.Format("<p><b>Comment:</b> {0}<br />{1}", model.Comment_Content, serviceComments.Comment_Content);
                        content += "</p>";
                        content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                        content += "<p>Thank you for your consideration</p>";
                        clsMail.SendToId = services.User_Id;
                        clsMail.Subject = subject;
                        clsMail.Body = content;
                        res = clsMail.SendMail(clsMail);
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
                db.Entry(services).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    if (!string.IsNullOrEmpty(model.Comment_Content))
                    {
                        serviceComments = new ServiceComments
                        {
                            Service_Id = services.Service_Id,
                            Comment_Content = model.Comment_Content
                        };
                        Services_Comment(serviceComments);
                    }

                    serviceComments = new ServiceComments
                    {
                        Service_Id = services.Service_Id,
                        Comment_Content = string.Format("Cancel task, Status update to {0}", system_Statuses.Status_Name)
                    };
                    res = Services_Comment(serviceComments);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetClose(Guid id, List<ClsEstimate> score)
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
                        System_Statuses system_Statuses = new System_Statuses();
                        system_Statuses = db.System_Statuses.Find(4);
                        services.Status_Id = system_Statuses.Status_Id;
                        services.Update = DateTime.Now;
                        db.Entry(services).State = EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            ServiceComments serviceComments = new ServiceComments
                            {
                                Service_Id = nextId.Value,
                                Comment_Content = string.Format("Status update to {0}", system_Statuses.Status_Name)
                            };
                            if (Services_Comment(serviceComments))
                            {
                                res = true;
                                if (services.Ref_Service_Id.HasValue)
                                {
                                    nextId = services.Ref_Service_Id.Value;
                                    if (db.Services.Any(a => a.Service_Id == nextId && a.Status_Id == 3))
                                    {
                                        res = SaveEstimate(nextId.Value, score);
                                    }
                                    else
                                    {
                                        nextId = null;
                                    }
                                }
                                else
                                {
                                    nextId = null;
                                    res = true;
                                }
                            }
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

        public bool Services_SetCommit(Services model, string methodName)
        {
            try
            {
                bool res = new bool();
                if (model.Action_User_Id.HasValue)
                {
                    res = Services_SetToUser(model.Service_Id, model.Department_Id.Value, model.Action_User_Id.Value, methodName);
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

        public bool Services_SetComplete(ServiceComments model, string methodName)
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
                if (services.Update.Value.Date > services.Service_DueDate)
                {
                    services.Is_OverDue = true;
                }
                services.Status_Id = system_Statuses.Status_Id;
                db.Entry(services).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    if (!string.IsNullOrEmpty(model.Comment_Content))
                    {
                        serviceComments = new ServiceComments
                        {
                            Service_Id = services.Service_Id,
                            Comment_Content = model.Comment_Content
                        };
                        Services_Comment(serviceComments);
                    }

                    serviceComments = new ServiceComments
                    {
                        Service_Id = services.Service_Id,
                        Comment_Content = string.Format("Complete task, Status update to {0}", system_Statuses.Status_Name)
                    };
                    if (Services_Comment(serviceComments))
                    {
                        var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                        linkUrl += "/" + services.Service_Id;
                        linkUrl = linkUrl.Replace(methodName, "ServiceInfomation");

                        string subject = string.Format("[Require close job] {0} - {1}", services.Service_Key, services.Service_Subject);
                        string content = string.Format("<p><b>Comment:</b> {0}<br />{1}", model.Comment_Content, serviceComments.Comment_Content);
                        content += "</p>";
                        content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                        content += "<p>Thank you for your consideration</p>";
                        clsMail.SendToId = services.User_Id;
                        clsMail.Subject = subject;
                        clsMail.Body = content;
                        if (clsMail.SendMail(clsMail))
                        {
                            MethodBase methodBase = MethodBase.GetCurrentMethod();
                            Log_SendEmail log_SendEmail = new Log_SendEmail
                            {
                                SendEmail_ClassName = methodBase.ReflectedType.Name,
                                SendEmail_Content = content,
                                SendEmail_MethodName = methodBase.Name,
                                SendEmail_Ref_Id = model.Service_Id,
                                SendEmail_Subject = subject,
                                User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name)
                            };
                            db.Entry(log_SendEmail).State = EntityState.Added;

                            Log_SendEmailTo log_SendEmailTo = new Log_SendEmailTo
                            {
                                SendEmailTo_Type = "to",
                                SendEmail_Id = log_SendEmail.SendEmail_Id,
                                User_Id = services.User_Id
                            };
                            db.Entry(log_SendEmailTo).State = EntityState.Added;
                            if (db.SaveChanges() > 0)
                            {
                                res = true;
                            }
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

        public bool Services_SetFreePoint(Guid id)
        {
            try
            {
                bool res = new bool();
                Services services = new Services();
                services = db.Services.Find(id);
                services.Is_FreePoint = true;
                services.Update = DateTime.Now;
                db.Entry(services).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments
                    {
                        Service_Id = id,
                        Comment_Content = "This request is not deducted points."
                    };
                    if (Services_Comment(serviceComments))
                    {
                        int point = db.System_Priorities.Find(services.Priority_Id).Priority_Point;
                        Users users = db.Users.Find(services.User_Id);
                        users.User_Point += point;
                        db.Entry(users).State = EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            serviceComments = new ServiceComments
                            {
                                Service_Id = id,
                                Comment_Content = string.Format("Give back {0} points to {1}", point, master.Users_GetInfomation(services.User_Id))
                            };
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

        public bool Services_SetPending(ServiceComments model, string methodName)
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
                services.WorkRoot_Id = null;
                db.Entry(services).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    List<ServiceDocuments> serviceDocuments = new List<ServiceDocuments>();
                    serviceDocuments = db.ServiceDocuments
                        .Where(w => w.Service_Id == model.Service_Id)
                        .ToList();
                    foreach (var item in serviceDocuments)
                    {
                        db.Entry(item).State = EntityState.Deleted;
                        if (db.SaveChanges() > 0)
                        {
                            if (!string.IsNullOrEmpty(item.ServiceDocument_Name))
                            {
                                if (ftp.Api_DeleteFile(item.ServiceDocument_Path))
                                {
                                    continue;
                                }
                            }
                        }
                    }

                    ServiceComments serviceComments = new ServiceComments();
                    if (!string.IsNullOrEmpty(model.Comment_Content))
                    {
                        serviceComments = new ServiceComments
                        {
                            Service_Id = services.Service_Id,
                            Comment_Content = model.Comment_Content
                        };

                        if (Services_Comment(serviceComments))
                        {
                            List<ServiceTeams> serviceTeams = db.ServiceTeams.Where(w => w.Service_Id == model.Service_Id).ToList();
                            foreach (var item in serviceTeams)
                            {
                                Service_DeleteTeam(item.Team_Id, methodName);
                            }
                        }
                    }

                    string deptName = db.Master_Departments.Find(services.Department_Id).Department_Name;

                    serviceComments = new ServiceComments
                    {
                        Service_Id = services.Service_Id,
                        Comment_Content = string.Format("Return job to department {0}", deptName)
                    };
                    res = Services_Comment(serviceComments);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetReject(ServiceComments model, string methodName)
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
                db.Entry(services).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    if (!string.IsNullOrEmpty(model.Comment_Content))
                    {
                        serviceComments = new ServiceComments
                        {
                            Service_Id = services.Service_Id,
                            Comment_Content = model.Comment_Content
                        };
                        Services_Comment(serviceComments);
                    }

                    serviceComments = new ServiceComments
                    {
                        Service_Id = services.Service_Id,
                        Comment_Content = string.Format("Reject task, Status update to {0}", system_Statuses.Status_Name)
                    };
                    if (Services_Comment(serviceComments))
                    {
                        var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                        linkUrl += "/" + services.Service_Id;
                        linkUrl = linkUrl.Replace(methodName, "ServiceInfomation");

                        string subject = string.Format("[Reject] {0} - {1}", services.Service_Key, services.Service_Subject);
                        string content = string.Format("<p><b>Comment:</b> {0}<br />{1}", model.Comment_Content, serviceComments.Comment_Content);
                        content += "</p>";
                        content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                        content += "<p>Thank you for your consideration</p>";
                        clsMail.SendToId = services.User_Id;
                        clsMail.Subject = subject;
                        clsMail.Body = content;
                        res = clsMail.SendMail(clsMail);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetRequired(ServiceComments model, string methodName)
        {
            try
            {
                bool res = new bool();

                Services services = db.Services.Find(model.Service_Id);
                services.Is_MustBeApproved = true;
                services.Update = DateTime.Now;
                db.Entry(services).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments
                    {
                        Service_Id = model.Service_Id,
                        Comment_Content = string.Format("Approval required, \n {0}", model.Comment_Content)
                    };
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
                        linkUrl = linkUrl.Replace(methodName, "Approve_Form");

                        string subject = string.Format("[Require approve] {0} - {1}", services.Service_Key, services.Service_Subject);
                        string content = string.Format("<p><b>Description:</b> {0}", services.Service_Description);
                        content += "<br />";
                        content += "<br />";
                        content += string.Format("<b>Comment:</b> {0}", serviceComments.Comment_Content);
                        content += "</p>";
                        content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                        content += "<p>Thank you for your consideration</p>";
                        clsMail.SendToIds = sendTo;
                        clsMail.Subject = subject;
                        clsMail.Body = content;
                        res = clsMail.SendMail(clsMail);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetReturnAssign(ServiceComments model, string methodName)
        {
            try
            {
                bool res = new bool();
                Services services = new Services();
                services = db.Services.Find(model.Service_Id);

                services.Update = DateTime.Now;
                services.Action_User_Id = null;
                db.Entry(services).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    if (!string.IsNullOrEmpty(model.Comment_Content))
                    {
                        serviceComments = new ServiceComments
                        {
                            Service_Id = services.Service_Id,
                            Comment_Content = model.Comment_Content
                        };
                        Services_Comment(serviceComments);
                    }

                    serviceComments = new ServiceComments
                    {
                        Service_Id = services.Service_Id,
                        Comment_Content = string.Format("Return assignments to {0} department", db.Master_Departments.Find(services.Department_Id).Department_Name)
                    };
                    if (Services_Comment(serviceComments))
                    {
                        if (services.Assign_User_Id.HasValue)
                        {
                            var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                            linkUrl += "/" + services.Service_Id;
                            linkUrl = linkUrl.Replace(methodName, "Action");

                            string subject = string.Format("[Return assignments] {0} - {1}", services.Service_Key, services.Service_Subject);
                            string content = string.Format("<p><b>Comment:</b> {0}<br />{1}", model.Comment_Content, serviceComments.Comment_Content);
                            content += "</p>";
                            content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                            content += "<p>Thank you for your consideration</p>";
                            clsMail.SendToId = services.Assign_User_Id;
                            clsMail.Subject = subject;
                            clsMail.Body = content;
                            res = clsMail.SendMail(clsMail);
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

        public bool Services_SetReturnJob(ServiceComments model, string methodName)
        {
            try
            {
                bool res = new bool();
                Services services = new Services();
                services = db.Services.Find(model.Service_Id);

                services.Update = DateTime.Now;
                services.Is_Commit = false;
                services.Department_Id = null;
                services.WorkRoot_Id = null;
                db.Entry(services).State = EntityState.Modified;

                List<ServiceDocuments> serviceDocuments = new List<ServiceDocuments>();
                serviceDocuments = db.ServiceDocuments.Where(w => w.Service_Id == services.Service_Id).ToList();
                foreach (var item in serviceDocuments)
                {
                    if (ftp.Api_DeleteFile(item.ServiceDocument_Path))
                    {
                        db.Entry(item).State = EntityState.Deleted;
                    }
                }
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    if (!string.IsNullOrEmpty(model.Comment_Content))
                    {
                        serviceComments = new ServiceComments
                        {
                            Service_Id = services.Service_Id,
                            Comment_Content = model.Comment_Content
                        };
                        Services_Comment(serviceComments);
                    }

                    serviceComments = new ServiceComments
                    {
                        Service_Id = services.Service_Id,
                        Comment_Content = "Return job to center room"
                    };
                    if (Services_Comment(serviceComments))
                    {
                        if (services.Assign_User_Id.HasValue)
                        {
                            var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                            linkUrl += "/" + services.Service_Id;
                            linkUrl = linkUrl.Replace(methodName, "Commit");

                            string subject = string.Format("[Return] {0} - {1}", services.Service_Key, services.Service_Subject);
                            string content = string.Format("<p><b>Comment:</b> {0}<br />{1}", model.Comment_Content, serviceComments.Comment_Content);
                            content += "</p>";
                            content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                            content += "<p>Thank you for your consideration</p>";
                            clsMail.SendToId = services.Assign_User_Id;
                            clsMail.Subject = subject;
                            clsMail.Body = content;
                            res = clsMail.SendMail(clsMail);
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

        public bool Services_SetToDepartment(Guid id, Guid? deptId = null)
        {
            try
            {
                bool res = new bool();
                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                if (!deptId.HasValue)
                {
                    deptId = db.Users.Find(userId).Master_Processes.Master_Sections.Department_Id;
                }

                string deptName = db.Master_Departments.Find(deptId).Department_Name;

                Services services = db.Services.Find(id);
                services.Department_Id = deptId;
                services.Is_Commit = true;
                services.Update = DateTime.Now;
                services.Assign_User_Id = userId;
                db.Entry(services).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments
                    {
                        Service_Id = services.Service_Id,
                        Comment_Content = string.Format("Commit Task, Assign task to the {0} department", deptName)
                    };
                    res = Services_Comment(serviceComments);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetToUser(Guid id, Guid deptId, Guid userId, string methodName)
        {
            try
            {
                bool res = new bool();

                string deptName = db.Master_Departments.Find(deptId).Department_Name;

                Services services = db.Services.Find(id);
                services.Department_Id = deptId;
                services.Action_User_Id = userId;
                services.Is_Commit = true;
                services.Update = DateTime.Now;
                services.Assign_User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                db.Entry(services).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments
                    {
                        Service_Id = services.Service_Id,
                        Comment_Content = string.Format("Commit Task, Assign task to the {0} department, Assign task to {1}", deptName, master.Users_GetInfomation(userId))
                    };

                    if (Services_Comment(serviceComments))
                    {
                        var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                        linkUrl = linkUrl.Replace(methodName, "Action");

                        string subject = string.Format("[Assign] {0} - {1}", services.Service_Key, services.Service_Subject);
                        string content = string.Format("<p><b>To:</b> {0}", master.Users_GetInfomation(userId));
                        content += "<br />";
                        content += string.Format("<b>Description:</b> {0}", services.Service_Description);
                        content += "<br />";
                        content += "<br />";
                        content += string.Format("<b>Comment:</b> {0}", serviceComments.Comment_Content);
                        content += "</p>";
                        content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                        content += "<p>Thank you for your consideration</p>";
                        clsMail.SendToId = userId;
                        clsMail.Subject = subject;
                        clsMail.Body = content;
                        res = clsMail.SendMail(clsMail);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_SetToUser(Guid id, Guid userId, string methodName)
        {
            try
            {
                bool res = new bool();

                Services services = new Services();
                services = db.Services.Find(id);
                services.Action_User_Id = userId;
                services.Update = DateTime.Now;
                services.Assign_User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                db.Entry(services).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments
                    {
                        Service_Id = services.Service_Id,
                        Comment_Content = string.Format("Assign task to {0}", master.Users_GetInfomation(userId))
                    };

                    if (Services_Comment(serviceComments))
                    {
                        var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                        linkUrl = linkUrl.Replace(methodName, "Action");
                        linkUrl = string.Concat(linkUrl, "/", id);

                        string subject = string.Format("[Assign] {0} - {1}", services.Service_Key, services.Service_Subject);
                        string content = string.Format("<p><b>To:</b> {0}", master.Users_GetInfomation(userId));
                        content += "<br />";
                        content += string.Format("<b>Description:</b> {0}", services.Service_Description);
                        content += "<br />";
                        content += "<br />";
                        content += string.Format("<b>Comment:</b> {0}", serviceComments.Comment_Content);
                        content += "</p>";
                        content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                        content += "<p>Thank you for your consideration</p>";
                        clsMail.SendToId = userId;
                        clsMail.Subject = subject;
                        clsMail.Body = content;
                        res = clsMail.SendMail(clsMail);
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
                        ServiceFiles serviceFiles = new ServiceFiles
                        {
                            Service_Id = model.Service_Id,
                            ServiceFile_Name = files[i].FileName
                        };
                        string dir = string.Format("Service/{0}/", model.Service_Key);
                        serviceFiles.ServiceFile_Path = ftp.Ftp_UploadFileToString(dir, files[i]);
                        serviceFiles.ServiceFile_Extension = Path.GetExtension(files[i].FileName);
                        db.Entry(serviceFiles).State = EntityState.Added;
                    }
                }

                db.Entry(model).State = EntityState.Modified;
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

        public List<Services> Services_ViewJoinTeamList(Guid id, ReportKPI_Filter filter)
        {
            try
            {
                IQueryable<Services> query = db.ServiceTeams
                    .Where(w => w.User_Id == id)
                    .Select(s => s.Services);

                if (filter != null)
                {
                    if (filter.Date_From.HasValue)
                    {
                        query = query.Where(w => w.Create >= filter.Date_From);
                    }

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
    }
}
