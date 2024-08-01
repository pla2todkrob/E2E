using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace E2E.Models
{
    public class ClsManageService
    {
        private readonly ClsApi clsApi = new ClsApi();
        private readonly ClsMail clsMail = new ClsMail();
        private readonly ClsServiceFile clsServiceFile = new ClsServiceFile();
        private readonly ClsContext db = new ClsContext();
        private readonly ClsManageMaster master = new ClsManageMaster();
        private FileResponse fileResponse = new FileResponse();

        public IQueryable<Services> Services_GetAllRequest_IQ()
        {
            try
            {
                Guid id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                // รวมการค้นหาให้เป็นคำสั่งเดียว
                IQueryable<Guid> userIds = db.Users
                    .Where(w => w.Master_Processes.Master_Sections.Master_Departments.Department_Name ==
                                db.Users.FirstOrDefault(u => u.User_Id == id).Master_Processes.Master_Sections.Master_Departments.Department_Name)
                    .Select(s => s.User_Id);

                // ใช้ IQueryable เพื่อประสิทธิภาพที่ดีขึ้น
                IQueryable<Services> query = db.Services
                    .Where(w => userIds.Contains(w.User_Id))
                    .OrderBy(o => o.Status_Id)
                    .ThenByDescending(t => t.Priority_Id)
                    .ThenBy(t => t.Service_DueDate)
                    .ThenBy(t => t.Update);

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
        private async Task AddServiceComment(Guid serviceId, string commentContent, Guid? userId = null)
        {
            ServiceComments serviceComments = new ServiceComments
            {
                Service_Id = serviceId,
                Comment_Content = commentContent,
                User_Id = userId
            };

            await Services_Comment(serviceComments);
        }

        public async Task<bool> Api_DeleteFile(string path)
        {
            try
            {
                fileResponse = await clsApi.DeleteFile(path);

                if (!fileResponse.IsSuccess)
                {
                    throw new Exception(fileResponse.ErrorMessage);
                }
                return fileResponse.IsSuccess;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                swal.Text = ex.GetBaseException().Message;

            }

            return swal;
        }

        public ClsReportKPI ClsReportKPI_ViewList(ReportKPI_Filter filter)
        {
            ClsReportKPI res = new ClsReportKPI();
            Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
            int[] finishIds = { 3, 4 };

            var user = db.Users
                .Where(w => w.User_Id == userId)
                .Select(s => new { s.Master_Processes.Master_Sections.Department_Id })
                .FirstOrDefault();

            if (user == null)
                return res;

            Guid deptId = user.Department_Id;

            List<Guid> userIds = db.Users
                .Where(w => w.Master_Processes.Master_Sections.Department_Id == deptId && w.Active)
                .OrderBy(o => o.User_Code)
                .Select(s => s.User_Id)
                .ToList();

            IQueryable<Services> query = db.Services
                .Where(w => finishIds.Contains(w.Status_Id) && userIds.Contains(w.Action_User_Id.Value));

            if (filter != null)
            {
                query = query.Where(w => w.Update >= filter.Date_From);
                DateTime adjustedDateTo = filter.Date_To.AddDays(1);
                query = query.Where(w => w.Update < adjustedDateTo);
            }

            var allTasks = query
                .Select(s => new
                {
                    s.Service_Id,
                    s.Status_Id,
                    s.Is_OverDue,
                    s.Action_User_Id,
                    s.System_Priorities.Priority_Point
                }).ToList();

            var serviceIds = allTasks.Select(s => s.Service_Id).ToList();

            var joinTeam = db.ServiceTeams
                .Where(w => serviceIds.Contains(w.Service_Id))
                .Select(s => new { s.Service_Id, s.User_Id })
                .ToList();

            var closedServiceIds = allTasks
                .Where(w => w.Status_Id == 4)
                .Select(s => s.Service_Id)
                .ToList();

            var satList = db.Satisfactions
                .Where(w => closedServiceIds.Contains(w.Service_Id))
                .Select(s => new
                {
                    s.Satisfaction_Average,
                    s.Service_Id,
                    s.Unsatisfied
                }).ToList();

            foreach (Guid item in userIds)
            {
                var actionServices = allTasks
                    .Where(w => w.Action_User_Id == item)
                    .ToList();

                List<Guid> notActionIds = allTasks
                    .Where(w => w.Action_User_Id != item)
                    .Select(s => s.Service_Id)
                    .ToList();

                ReportKPI_User reportKPI_User = new ReportKPI_User
                {
                    User_Id = item,
                    User_Name = master.Users_GetInfomation(item),
                    Total = actionServices.Count(),
                    JoinTeam_Count = joinTeam.Count(w => w.User_Id == item)
                };

                if (reportKPI_User.Total > 0)
                {
                    reportKPI_User.Close_Count = actionServices.Count(w => w.Status_Id == 4);
                    reportKPI_User.Complete_Count = actionServices.Count(w => w.Status_Id == 3);
                    reportKPI_User.OverDue_Count = actionServices.Count(w => w.Is_OverDue);

                    List<Guid> serviceCloseId = actionServices
                        .Where(w => w.Status_Id == 4)
                        .Select(s => s.Service_Id)
                        .ToList();

                    reportKPI_User.Average_Score = satList
                        .Where(w => serviceCloseId.Contains(w.Service_Id))
                        .Average(a => (double?)a.Satisfaction_Average) ?? 0;

                    reportKPI_User.SuccessPoint = actionServices.Sum(s => (int?)s.Priority_Point) ?? 0;
                }

                res.ReportKPI_Users.Add(reportKPI_User);
            }

            int totalTasks = allTasks.Count;
            int onTimeCount = allTasks.Count(w => !w.Is_OverDue);
            int closedCount = allTasks.Count(w => w.Status_Id == 4);
            int unsatisfiedCount = satList.Count(w => w.Unsatisfied);

            res.ReportKPI_Overview = new ReportKPI_Overview()
            {
                Close_Count = closedCount,
                Complete_Count = allTasks.Count(w => w.Status_Id == 3),
                Total = totalTasks,
                OnTime_Count = onTimeCount,
                OnTime_Percent = (totalTasks == 0) ? 1 : (double)onTimeCount / totalTasks,
                OverDue_Count = allTasks.Count(w => w.Is_OverDue),
                Satisfied_Percent = (closedCount == 0) ? 1 : 1 - (double)unsatisfiedCount / closedCount,
                Unsatisfied_Count = unsatisfiedCount
            };

            return res;
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
                    UserDetails userDetails = db.UserDetails
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
                int[] finishIds = { 3, 4 };
                IQueryable<Services> services = db.Services
                    .Where(w => w.Action_User_Id == id && finishIds.Contains(w.Status_Id));

                if (filter != null)
                {
                    services = services.Where(w => w.Update >= filter.Date_From);

                    filter.Date_To = filter.Date_To.AddDays(1);
                    services = services.Where(w => w.Update <= filter.Date_To);
                }

                List<ReportKPI_User_Views> user_Views = services
                    .GroupJoin(
                    db.Satisfactions,
                    ser => ser.Service_Id,
                    sat => sat.Service_Id,
                    (ser, satGroup) => new { ser, satGroup })
                    .SelectMany(
                    temp => temp.satGroup.DefaultIfEmpty(),
                    (temp, sat) => new ReportKPI_User_Views
                    {
                        Service_Id = temp.ser.Service_Id,
                        Update = temp.ser.Update,
                        Service_Subject = temp.ser.Service_Subject,
                        Service_Key = temp.ser.Service_Key,
                        Priority_Point = temp.ser.System_Priorities.Priority_Point,
                        Satisfaction_Average = sat != null ? sat.Satisfaction_Average : (double?)null,
                        Status_Class = temp.ser.System_Statuses.Status_Class,
                        Status_Name = temp.ser.System_Statuses.Status_Name
                    }).ToList();

                return user_Views;
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

                Satisfactions satisfactions = new Satisfactions
                {
                    Service_Id = id,
                    Satisfaction_Average = average,
                    Unsatisfied = score.Any(w => w.Score < 3)
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

                if (await db.SaveChangesAsync() > 0)
                {
                    res = await Services_SetClose(id, score);
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveUserActionChangeDue(Guid Service_ID)
        {
            Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
            var Services = db.Services.Find(Service_ID);
            Services.Action_User_Id = userId;

            db.SaveChanges();
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

        public async Task<bool> Service_AddTeam(ClsServiceTeams model, string methodName)
        {
            try
            {
                foreach (var item in model.User_Ids)
                {
                    ServiceTeams serviceTeams = new ServiceTeams
                    {
                        Service_Id = model.Service_Id,
                        User_Id = item
                    };
                    db.Entry(serviceTeams).State = EntityState.Added;
                    if (await db.SaveChangesAsync() > 0)
                    {
                        await AddServiceComment(model.Service_Id, string.Format("Add {0} to join team", master.Users_GetInfomation(item)));
                    }
                }

                var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                linkUrl += "/" + model.Service_Id;
                linkUrl = linkUrl.Replace(methodName, "ServiceInfomation");

                Services services = await db.Services.FindAsync(model.Service_Id);

                var listTeam = await db.ServiceTeams
                    .Where(w => w.Service_Id == services.Service_Id)
                    .Select(s => s.User_Id).ToListAsync();

                var listTeamName = listTeam.Select(s => master.Users_GetInfomation(s)).ToList();


                string subject = string.Format("[Notify add team] {0} - {1}", services.Service_Key, services.Service_Subject);
                string content = string.Format("<p><b>Description:</b> {0}", services.Service_Description);
                content += "<br />";
                content += "<br />";
                content += "<b>Current member</b><br/>";
                content += string.Join("<br />", listTeamName);
                content += "</p>";
                content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                content += "<p>Thank you for your consideration</p>";
                clsMail.SendTos = listTeam;
                clsMail.Subject = subject;
                clsMail.Body = content;

                return await clsMail.SendMail(clsMail);
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

        public async Task<bool> Service_DeleteTeam(Guid id, string methodName)
        {
            try
            {
                ServiceTeams serviceTeams = await db.ServiceTeams.FindAsync(id);
                string userName = master.Users_GetInfomation(serviceTeams.User_Id);
                Guid serviceId = serviceTeams.Service_Id;
                db.Entry(serviceTeams).State = EntityState.Deleted;
                if (await db.SaveChangesAsync() > 0)
                {
                    await AddServiceComment(serviceId, string.Format("Delete {0} from this team", userName));
                    var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                    linkUrl = linkUrl.Replace(methodName, "ServiceInfomation");
                    if (!linkUrl.EndsWith(serviceId.ToString()))
                    {
                        linkUrl += string.Format("/{0}", serviceId);
                    }

                    Services services = await db.Services.FindAsync(serviceId);

                    List<Guid> listTeam = new List<Guid>();
                    listTeam = db.ServiceTeams
                        .Where(w => w.Service_Id == serviceId)
                        .Select(s => s.User_Id)
                        .ToList();

                    var listTeamName = listTeam.Select(s => master.Users_GetInfomation(s)).ToList();

                    listTeam.Add(serviceTeams.User_Id);

                    string subject = string.Format("[Notify delete member] {0} - {1}", services.Service_Key, services.Service_Subject);
                    string content = "<p><b>Delete: </b>" + userName;
                    content += "<br />";
                    content += "<br />";
                    content += "<b>Current member</b><br/>";
                    content += string.Join("<br />", listTeamName);
                    content += "</p>";
                    content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                    content += "<p>Thank you for your consideration</p>";
                    clsMail.SendTos = listTeam;
                    clsMail.Subject = subject;
                    clsMail.Body = content;
                }

                return await clsMail.SendMail(clsMail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ServiceChangeDueDate_Accept(Guid id, string methodName)
        {
            try
            {
                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                ServiceChangeDueDate serviceChangeDueDate = await db.ServiceChangeDueDates.FindAsync(id);
                serviceChangeDueDate.DueDateStatus_Id = 2;
                serviceChangeDueDate.Update = DateTime.Now;
                db.Entry(serviceChangeDueDate).State = EntityState.Modified;
                if (await db.SaveChangesAsync() > 0)
                {
                    await AddServiceComment(serviceChangeDueDate.Service_Id, "Accept due date change request");
                    Services services = await db.Services.FindAsync(serviceChangeDueDate.Service_Id);
                    services.Service_DueDate = serviceChangeDueDate.DueDate_New;
                    services.Update = DateTime.Now;
                    db.Entry(services).State = EntityState.Modified;
                    if (await db.SaveChangesAsync() > 0)
                    {
                        string commentContent = string.Format("Change due date from {0} to {1}", serviceChangeDueDate.DueDate.ToString("d"), serviceChangeDueDate.DueDate_New.Value.ToString("d"));
                        await AddServiceComment(serviceChangeDueDate.Service_Id, commentContent);

                        var Sendto = await db.ServiceComments.Where(w => w.Service_Id == services.Service_Id && w.Comment_Content.StartsWith("Request change due date from")).OrderByDescending(o => o.Create).FirstOrDefaultAsync();

                        var linkUrl = HttpContext.Current.Request.Url.OriginalString;

                        linkUrl = linkUrl.Replace(methodName + "/" + serviceChangeDueDate.ChangeDueDate_Id, "Action/" + services.Service_Id);

                        string subject = string.Format("[Accept requests to change due date] {0} - {1}", services.Service_Key, services.Service_Subject);
                        string content = string.Format("<p><b>Comment: </b> {0}<br />", commentContent);
                        content += "</p>";
                        content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                        content += "<p>Thank you for your consideration</p>";
                        clsMail.SendTos.Add(Sendto.User_Id.Value);
                        clsMail.Subject = subject;
                        clsMail.Body = content;
                    }
                }

                return await clsMail.SendMail(clsMail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ServiceChangeDueDate_Cancel(Guid id)
        {
            try
            {
                ServiceChangeDueDate serviceChangeDueDate = await db.ServiceChangeDueDates.FindAsync(id);
                serviceChangeDueDate.DueDateStatus_Id = 4;
                serviceChangeDueDate.Update = DateTime.Now;
                db.Entry(serviceChangeDueDate).State = EntityState.Modified;
                if (await db.SaveChangesAsync() > 0)
                {
                    await AddServiceComment(serviceChangeDueDate.Service_Id, "Cancel due date change request");
                }

                return true;
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

        public async Task<bool> ServiceChangeDueDate_Reject(Guid id, string methodName)
        {
            try
            {
                ServiceChangeDueDate serviceChangeDueDate = await db.ServiceChangeDueDates.FindAsync(id);
                serviceChangeDueDate.DueDateStatus_Id = 3;
                serviceChangeDueDate.Update = DateTime.Now;
                db.Entry(serviceChangeDueDate).State = EntityState.Modified;
                if (await db.SaveChangesAsync() > 0)
                {
                    await AddServiceComment(serviceChangeDueDate.Service_Id, "Reject due date change request");

                    var Sendto = await db.ServiceComments.Where(w => w.Service_Id == serviceChangeDueDate.Service_Id && w.Comment_Content.StartsWith("Request change due date from")).OrderByDescending(o => o.Create).FirstOrDefaultAsync();

                    var linkUrl = HttpContext.Current.Request.Url.OriginalString;

                    linkUrl = linkUrl.Replace(methodName + "/" + serviceChangeDueDate.ChangeDueDate_Id, "Action/" + serviceChangeDueDate.Service_Id);

                    string subject = string.Format("[Reject request to change due date] {0} - {1}", Sendto.Services.Service_Key, Sendto.Services.Service_Subject);
                    string content = string.Format("<p><b>Comment: </b> {0}<br />", "Reject due date change request");
                    content += "</p>";
                    content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                    content += "<p>Thank you for your consideration</p>";
                    clsMail.SendTos.Add(Sendto.User_Id.Value);
                    clsMail.Subject = subject;
                    clsMail.Body = content;
                }

                return await clsMail.SendMail(clsMail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ServiceChangeDueDate_Request(ServiceChangeDueDate model, string methodName)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                model.User_Id = userId;
                db.Entry(model).State = EntityState.Added;
                if (await db.SaveChangesAsync() > 0)
                {
                    string mailDes = string.Format("Request change due date from {0} to {1}", model.DueDate.ToString("d"), model.DueDate_New.Value.ToString("d"));
                    string commentContent = string.Format("{0}{1}{2}Remark: {3}", mailDes, Environment.NewLine, Environment.NewLine, model.Remark);
                    await AddServiceComment(model.Service_Id, commentContent);

                    Services services = await db.Services.FindAsync(model.Service_Id);
                    SaveUserActionChangeDue(model.Service_Id);

                    var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                    linkUrl = linkUrl.Replace(methodName, "RequestChangeDue");

                    string subject = string.Format("[Request to change due date] {0} - {1}", services.Service_Key, services.Service_Subject);
                    string content = string.Format("<p><b>Description:</b> {0}</p>", mailDes);
                    content += string.Format("<p><b>Remark:</b> {0}</p>", model.Remark);

                    content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                    content += "<p>Thank you for your consideration</p>";
                    clsMail.SendTos.Add(services.User_Id);
                    clsMail.Subject = subject;
                    clsMail.Body = content;
                }

                return await clsMail.SendMail(clsMail);
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

                return db.ServiceChangeDueDates
                    .Where(w => w.Services.User_Id == id)
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

        public async Task<bool> ServiceFiles_Delete(Guid id)
        {
            try
            {
                bool res = new bool();
                ServiceFiles serviceFiles = new ServiceFiles();
                serviceFiles = db.ServiceFiles.Find(id);

                Services services = await db.Services.FindAsync(serviceFiles.Service_Id);
                services.Service_FileCount -= 1;
                services.Update = DateTime.Now;
                db.Entry(services).State = EntityState.Modified;
                db.Entry(serviceFiles).State = EntityState.Deleted;
                if (await Api_DeleteFile(serviceFiles.ServiceFile_Path))
                {
                    if (await db.SaveChangesAsync() > 0)
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

        public async Task<bool> Services_Comment(ServiceComments model, HttpFileCollectionBase files = null)
        {
            try
            {
                if (!model.User_Id.HasValue)
                {
                    model.User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                }
                db.ServiceComments.Add(model);
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
                        string dir = Path.Combine("Service", db.Services.Find(model.Service_Id).Service_Key, "Comment");
                        serviceCommentFiles.ServiceCommentFile_Path = await UploadFileToString(dir, files[i]);
                        serviceCommentFiles.ServiceComment_Id = model.ServiceComment_Id;
                        serviceCommentFiles.ServiceComment_Seq = i;
                        serviceCommentFiles.ServiceCommentFile_Extension = Path.GetExtension(files[i].FileName);
                        db.ServiceCommentFiles.Add(serviceCommentFiles);
                    }
                }

                return await db.SaveChangesAsync() > 0;
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
                    throw new Exception("Invalid user id");
                }

                var query = db.Services
                    .AsNoTracking()
                    .Join(db.Master_Sections,
                          service => service.Department_Id,
                          section => section.Department_Id,
                          (service, section) => new { service, section })
                    .Join(db.Master_Processes,
                          combined => combined.section.Section_Id,
                          process => process.Section_Id,
                          (combined, process) => new { combined.service, process })
                    .Join(db.Users,
                          combined => combined.process.Process_Id,
                          user => user.Process_Id,
                          (combined, user) => new { combined.service, user })
                    .Where(joined => joined.user.User_Id == id)
                    .OrderBy(joined => joined.service.Status_Id)
                    .ThenByDescending(joined => joined.service.Priority_Id)
                    .ThenBy(joined => joined.service.Service_DueDate)
                    .ThenBy(joined => joined.service.Update)
                    .Select(joined => joined.service);

                return query;
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

        public List<Services> Services_GetRequiredApprove(bool isApprove)
        {
            try
            {
                Guid id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                string deptName = db.Users.Find(id).Master_Processes.Master_Sections.Master_Departments.Department_Name;
                List<Guid> userIdList = db.Users
                    .Where(w => w.Master_Processes.Master_Sections.Master_Departments.Department_Name == deptName).Select(s => s.User_Id).ToList();
                return db.Services.Where(w => w.Is_MustBeApproved && w.Is_Approval == isApprove && userIdList.Contains(w.User_Id) && w.Status_Id == 1).ToList();
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

        public async Task<bool> Services_Insert(Services model, HttpFileCollectionBase files, bool isForward = false)
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
                    Users users = await db.Users.FindAsync(model.User_Id);
                    int usePoint = await db.System_Priorities
                        .Where(w => w.Priority_Id == model.Priority_Id)
                        .Select(s => s.Priority_Point)
                        .FirstOrDefaultAsync();

                    if (users.User_Point < usePoint)
                    {
                        throw new Exception("Your point balance is insufficient.");
                    }

                    users.User_Point -= usePoint;
                    await db.SaveChangesAsync();
                }

            InsertProcess:

                int todayCount = await db.Services
                    .Where(w => w.Create >= DateTime.Today)
                    .CountAsync();
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
                        string dir = Path.Combine("Service", model.Service_Key);
                        serviceFiles.ServiceFile_Path = await UploadFileToString(dir, files[i]);
                        serviceFiles.ServiceFile_Extension = Path.GetExtension(files[i].FileName);
                        db.ServiceFiles.Add(serviceFiles);
                    }
                }

                db.Services.Add(model);

                if (await db.SaveChangesAsync() > 0)
                {
                    if (model.Ref_Service_Id.HasValue)
                    {
                        System_Statuses system_Statuses = await db.System_Statuses
                            .Where(w => w.Status_Id == 3)
                            .FirstOrDefaultAsync();

                        Services services = await db.Services.FindAsync(model.Ref_Service_Id);
                        if (services.Status_Id == 2)
                        {
                            services.Status_Id = 3;
                            services.Update = DateTime.Now;
                            await db.SaveChangesAsync();

                            await AddServiceComment(model.Ref_Service_Id.Value, string.Format("Complete task, Status update to {0}", system_Statuses.Status_Name));
                        }

                        await AddServiceComment(model.Ref_Service_Id.Value, string.Format("Forward this job to new service key {0}", model.Service_Key));
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_Save(Services model, HttpFileCollectionBase files, bool isForward = false)
        {
            try
            {
                bool res = new bool();
                model.Create_User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                Services services = await db.Services.FindAsync(model.Service_Id);
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
                    res = await Services_Update(services, files);
                }
                else
                {
                    res = await Services_Insert(model, files, isForward);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_SaveDocumentControl(ServiceDocuments model, HttpFileCollectionBase files)
        {
            try
            {
                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                bool res = new bool();
                ServiceDocuments serviceDocuments = await db.ServiceDocuments.FindAsync(model.ServiceDocument_Id);
                serviceDocuments.ServiceDocument_Remark = model.ServiceDocument_Remark;
                serviceDocuments.User_Id = userId;
                serviceDocuments.Update = DateTime.Now;

                HttpPostedFileBase fileBase = files[0];
                if (fileBase.ContentLength > 0)
                {
                    serviceDocuments.ServiceDocument_Name = fileBase.FileName;
                    string dir = Path.Combine("Service", db.Services.Find(model.Service_Id).Service_Key, "DocumentControls");
                    serviceDocuments.ServiceDocument_Path = await UploadFileToString(dir, fileBase);
                }

                db.Entry(serviceDocuments).State = EntityState.Modified;
                if (await db.SaveChangesAsync() > 0)
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

        public async Task<bool> Services_SetAction(Services model)
        {
            try
            {
                Services services = await db.Services.FindAsync(model.Service_Id);

                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);

                if (!services.Action_User_Id.HasValue)
                {
                    services.Action_User_Id = userId;
                }

                System_Statuses system_Statuses = await db.System_Statuses
                    .Where(w => w.Status_Id == 2)
                    .FirstOrDefaultAsync();

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
                if (await db.SaveChangesAsync() > 0)
                {
                    await AddServiceComment(services.Service_Id, string.Format("Start task, Estimate time about {0} days, Status update to {1}", services.Service_EstimateTime, system_Statuses.Status_Name));
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_SetApprove(ServiceComments model, string methodName)
        {
            try
            {
                Services services = await db.Services.FindAsync(model.Service_Id);
                services.Is_Approval = true;
                services.Update = DateTime.Now;
                db.Entry(services).State = EntityState.Modified;
                if (await db.SaveChangesAsync() > 0)
                {
                    string mailComment = string.Format("Approved,\n{0}", model.Comment_Content);
                    await AddServiceComment(model.Service_Id, mailComment);
                    var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                    linkUrl += "/" + services.Service_Id;
                    linkUrl = linkUrl.Replace(methodName, "Approve_Form");

                    string subject = string.Format("[Approved] {0} - {1}", services.Service_Key, services.Service_Subject);
                    string content = string.Format("<p><b>Comment:</b> {0}<br />{1}", model.Comment_Content, mailComment);
                    content += "</p>";
                    content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                    content += "<p>Thank you for your consideration</p>";
                    clsMail.SendTos.Add(services.User_Id);
                    clsMail.Subject = subject;
                    clsMail.Body = content;
                }

                return await clsMail.SendMail(clsMail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_SetCancel(ServiceComments model)
        {
            try
            {
                Services services = await db.Services.FindAsync(model.Service_Id);

                System_Statuses system_Statuses = await db.System_Statuses.FindAsync(6);

                services.Update = DateTime.Now;
                services.Status_Id = system_Statuses.Status_Id;
                db.Entry(services).State = EntityState.Modified;
                if (await db.SaveChangesAsync() > 0)
                {
                    if (!string.IsNullOrEmpty(model.Comment_Content))
                    {
                        await AddServiceComment(services.Service_Id, model.Comment_Content);
                    }

                    await AddServiceComment(services.Service_Id, string.Format("Cancel task, Status update to {0}", system_Statuses.Status_Name));
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_SetClose(Services services, bool isAuto = false)
        {
            try
            {
                System_Statuses system_Statuses = await db.System_Statuses.FindAsync(4);
                services.Status_Id = system_Statuses.Status_Id;
                services.Update = DateTime.Now.AddDays(-DateTime.Now.Day);
                services.Is_AutoClose = isAuto;
                db.Entry(services).State = EntityState.Modified;
                if (await db.SaveChangesAsync() > 0)
                {
                    string commentContent = string.Format("Automatically update status to {0} by system", system_Statuses.Status_Name);
                    await AddServiceComment(services.Service_Id, commentContent, services.Action_User_Id);

                    if (services.Ref_Service_Id.HasValue)
                    {
                        services = await db.Services.FindAsync(services.Ref_Service_Id.Value);
                        await Services_SetClose(services);
                    }

                    ClsMail clsMail = new ClsMail()
                    {
                        Body = commentContent,
                        SendFrom = services.Action_User_Id.Value,
                        Subject = string.Format("[Job is closed] {0} - {1}", services.Service_Key, services.Service_Subject)
                    };

                    clsMail.SendCCs.Add(services.User_Id);
                    clsMail.SendTos.Add(services.Action_User_Id.Value);

                    
                }

                return await clsMail.SendMail(clsMail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_SetClose(Guid id, List<ClsEstimate> score)
        {
            try
            {
                Services services = await db.Services.FindAsync(id);
                if (services.Status_Id == 3)
                {
                    System_Statuses system_Statuses = await db.System_Statuses.FindAsync(4);
                    services.Status_Id = system_Statuses.Status_Id;
                    services.Update = DateTime.Now;
                    db.Entry(services).State = EntityState.Modified;
                    if (await db.SaveChangesAsync() > 0)
                    {
                        string commentContent = string.Format("Status update to {0}", system_Statuses.Status_Name);
                        await AddServiceComment(id, commentContent);
                        if (services.Ref_Service_Id.HasValue)
                        {
                            id = services.Ref_Service_Id.Value;
                            if (db.Services.Any(a => a.Service_Id == id && a.Status_Id == 3))
                            {
                                await SaveEstimate(id, score);
                            }
                        }

                        ClsMail clsMail = new ClsMail()
                        {
                            Body = commentContent,
                            SendFrom = services.Action_User_Id.Value,
                            Subject = string.Format("[Job is closed] {0} - {1}", services.Service_Key, services.Service_Subject)
                        };

                        clsMail.SendCCs.Add(services.User_Id);
                        clsMail.SendTos.Add(services.Action_User_Id.Value);

                        
                    }
                }

                return await clsMail.SendMail(clsMail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_SetCommit(Services model, string methodName)
        {
            try
            {
                bool res = new bool();
                if (model.Action_User_Id.HasValue)
                {
                    res = await Services_SetToUser(model.Service_Id, model.Department_Id.Value, model.Action_User_Id.Value, methodName);
                }
                else
                {
                    res = await Services_SetToDepartment(model.Service_Id, methodName, model.Department_Id);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_SetComplete(ServiceComments model, string methodName)
        {
            try
            {
                Services services = await db.Services.FindAsync(model.Service_Id);
                DateTime inprogrssDate = services.Update.Value;
                System_Statuses system_Statuses = await db.System_Statuses.FindAsync(3);

                services.Update = DateTime.Now;
                TimeSpan timeDifference = services.Update.Value - inprogrssDate;
                services.Service_ActualTime = (int)timeDifference.TotalDays;
                if (services.Update.Value.Date > services.Service_DueDate)
                {
                    services.Is_OverDue = true;
                }
                services.Status_Id = system_Statuses.Status_Id;
                db.Entry(services).State = EntityState.Modified;
                if (await db.SaveChangesAsync() > 0)
                {
                    
                    await AddServiceComment(services.Service_Id, model.Comment_Content);
                    string commentContent = string.Format("Complete task, Status update to {0}", system_Statuses.Status_Name);
                    await AddServiceComment(services.Service_Id, commentContent);
                    commentContent = $"{model.Comment_Content}<br />{commentContent}";


                    var nextMonth = services.Update.Value.AddMonths(1);
                    var eighthOfNextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 8).ToShortDateString();
                    var lastDayOfMonth = new DateTime(services.Update.Value.Year, services.Update.Value.Month, DateTime.DaysInMonth(services.Update.Value.Year, services.Update.Value.Month)).ToShortDateString();

                    var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                    linkUrl += "/" + services.Service_Id;
                    linkUrl = linkUrl.Replace(methodName, "ServiceInfomation");

                    string subject = string.Format("[Request close the job] {0} - {1}", services.Service_Key, services.Service_Subject);
                    string content = string.Format("<p><b>Comment:</b> {0}<br />{1}", model.Comment_Content, commentContent);
                    content += "</p>";
                    content += $"<b>The system will automatically close the job on {eighthOfNextMonth}. The system will send you a total of 4 reminder emails. If you do not take action, the job will be closed automatically on the last day of this month ({lastDayOfMonth}).</b><br />";
                    content += $"<b>ระบบจะปิดงานนี้โดยอัตโนมัติ โดยที่ระบบจะส่งอีเมลแจ้งเตือนให้ท่านรวมทั้งสิ้น 4 ครั้ง หากท่านไม่ดำเนินการ ระบบจะปิดงานโดยอัตโนมัติในวันที่ {eighthOfNextMonth} แต่ระบบจะระบุว่าปิดในวันสุดท้ายของเดือนนี้ ({lastDayOfMonth})</b><br />";
                    content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                    content += "<p>Thank you for your consideration</p>";
                    clsMail.SendTos.Add(services.User_Id);
                    clsMail.SendCCs.Add(services.Action_User_Id.Value);
                    clsMail.Subject = subject;
                    clsMail.Body = content;

                    Log_SendEmail log_SendEmail = new Log_SendEmail
                    {
                        SendEmail_Content = content,
                        SendEmail_Ref_Id = model.Service_Id,
                        SendEmail_Subject = subject,
                        User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name)
                    };
                    db.Log_SendEmails.Add(log_SendEmail);

                    Log_SendEmailTo log_SendEmailTo = new Log_SendEmailTo
                    {
                        SendEmailTo_Type = "to",
                        SendEmail_Id = log_SendEmail.SendEmail_Id,
                        User_Id = services.User_Id
                    };
                    db.Log_SendEmailTos.Add(log_SendEmailTo);

                    await db.SaveChangesAsync();
                }

                return await clsMail.SendMail(clsMail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_SetFreePoint(Guid id)
        {
            try
            {
                Services services = await db.Services.FindAsync(id);
                services.Is_FreePoint = true;
                services.Update = DateTime.Now;
                db.Entry(services).State = EntityState.Modified;
                if (await db.SaveChangesAsync() > 0)
                {
                    await AddServiceComment(id, "This request is not deducted points.");
                    int point = await db.System_Priorities
                            .Where(w => w.Priority_Id == services.Priority_Id)
                            .Select(s => s.Priority_Point)
                            .FirstOrDefaultAsync();
                    await AddServiceComment(id, string.Format("Give back {0} points to {1}", point, master.Users_GetInfomation(services.User_Id)));
                    Users users = await db.Users.FindAsync(services.User_Id);
                    users.User_Point += point;
                    db.Entry(users).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_SetPending(ServiceComments model, string methodName)
        {
            try
            {
                Services services = await db.Services.FindAsync(model.Service_Id);
                Guid? actionUserId = services.Action_User_Id ?? null;

                services.Update = DateTime.Now;
                services.Action_User_Id = null;
                services.Status_Id = 1;
                services.Service_EstimateTime = 0;
                services.WorkRoot_Id = null;
                db.Entry(services).State = EntityState.Modified;
                if (await db.SaveChangesAsync() > 0)
                {
                    List<ServiceDocuments> serviceDocuments = new List<ServiceDocuments>();
                    serviceDocuments = await db.ServiceDocuments
                        .Where(w => w.Service_Id == model.Service_Id)
                        .ToListAsync();

                    foreach (var item in serviceDocuments)
                    {
                        if (!string.IsNullOrEmpty(item.ServiceDocument_Name))
                        {
                            if (await Api_DeleteFile(item.ServiceDocument_Path))
                            {
                                continue;
                            }
                        }
                    }

                    db.ServiceDocuments.RemoveRange(serviceDocuments);
                    await db.SaveChangesAsync();

                    await AddServiceComment(services.Service_Id, model.Comment_Content);

                    List<ServiceTeams> serviceTeams = await db.ServiceTeams.Where(w => w.Service_Id == model.Service_Id).ToListAsync();
                    foreach (var item in serviceTeams)
                    {
                        await Service_DeleteTeam(item.Team_Id, methodName);
                    }
                    string deptName = db.Master_Departments.Find(services.Department_Id).Department_Name;
                    await AddServiceComment(services.Service_Id, string.Format("Return job to department {0}", deptName));

                    var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                    linkUrl += "/" + services.Service_Id;
                    linkUrl = linkUrl.Replace(methodName, "ServiceInfomation");

                    string subject = string.Format("[Return the job to department] {0} - {1}", services.Service_Key, services.Service_Subject);
                    string content = string.Format("<p><b>Comment:</b> {0}", model.Comment_Content);
                    content += "</p>";
                    content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                    content += "<p>Thank you for your consideration</p>";
                    clsMail.SendTos.AddRange(await master.GetManagementOfDepartment());
                    if (actionUserId.HasValue)
                    {
                        clsMail.SendCCs.Add(actionUserId.Value);
                    }
                    clsMail.Subject = subject;
                    clsMail.Body = content;
                }

                return await clsMail.SendMail(clsMail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_SetRequestReject(ServiceComments model, string methodName)
        {
            try
            {
                Services services = await db.Services.FindAsync(model.Service_Id);

                Guid? actionUserId = services.Action_User_Id ?? null;

                services.Update = DateTime.Now;
                services.Action_User_Id = null;
                services.Status_Id = 1;
                db.Entry(services).State = EntityState.Modified;
                if (await db.SaveChangesAsync() > 0)
                {
                    await AddServiceComment(services.Service_Id, model.Comment_Content);
                    var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                    linkUrl += "/" + services.Service_Id;
                    linkUrl = linkUrl.Replace(methodName, "ServiceInfomation");

                    string subject = string.Format("[Request reject job] {0} - {1}", services.Service_Key, services.Service_Subject);
                    string content = string.Format("<p><b>Comment:</b> {0}", model.Comment_Content);
                    content += "</p>";
                    content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                    content += "<p>Thank you for your consideration</p>";
                    clsMail.SendTos.AddRange(await master.GetManagementOfDepartment());
                    if (actionUserId.HasValue)
                    {
                        clsMail.SendCCs.Add(actionUserId.Value);
                    }
                    clsMail.Subject = subject;
                    clsMail.Body = content;
                }

                return await clsMail.SendMail(clsMail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_SetReject(ServiceComments model, string methodName)
        {
            try
            {
                Services services = await db.Services.FindAsync(model.Service_Id);
                System_Statuses system_Statuses = await db.System_Statuses.FindAsync(5);

                services.Update = DateTime.Now;
                services.Status_Id = system_Statuses.Status_Id;
                db.Entry(services).State = EntityState.Modified;
                if (await db.SaveChangesAsync() > 0)
                {
                    await AddServiceComment(services.Service_Id, model.Comment_Content);
                    string commentContent = string.Format("Reject task, Status update to {0}", system_Statuses.Status_Name);
                    await AddServiceComment(services.Service_Id, commentContent);
                    commentContent = $"{model.Comment_Content}<br />{commentContent}";

                    var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                    linkUrl += "/" + services.Service_Id;
                    linkUrl = linkUrl.Replace(methodName, "ServiceInfomation");

                    string subject = string.Format("[Job rejected] {0} - {1}", services.Service_Key, services.Service_Subject);
                    string content = string.Format("<p><b>Comment:</b> {0}<br />{1}", model.Comment_Content, commentContent);
                    content += "</p>";
                    content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                    content += "<p>Thank you for your consideration</p>";
                    clsMail.SendTos.Add(services.User_Id);
                    if (services.User_Id != services.Create_User_Id)
                    {
                        clsMail.SendCCs.Add(services.Create_User_Id);
                    }

                    clsMail.Subject = subject;
                    clsMail.Body = content;
                }

                return await clsMail.SendMail(clsMail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_SetRequired(ServiceComments model, string methodName)
        {
            try
            {
                Services services = await db.Services.FindAsync(model.Service_Id);
                services.Is_MustBeApproved = true;
                services.Update = DateTime.Now;
                db.Entry(services).State = EntityState.Modified;
                if (await db.SaveChangesAsync() > 0)
                {
                    string commentContent = string.Format("Approval required, \n {0}", model.Comment_Content);
                    await AddServiceComment(model.Service_Id, commentContent);
                    var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                    linkUrl += "/" + services.Service_Id;
                    linkUrl = linkUrl.Replace(methodName, "Approve_Form");

                    string subject = string.Format("[Require approval] {0} - {1}", services.Service_Key, services.Service_Subject);
                    string content = string.Format("<p><b>Description:</b> {0}", services.Service_Description);
                    content += "<br />";
                    content += "<br />";
                    content += string.Format("<b>Comment:</b> {0}", commentContent);
                    content += "</p>";
                    content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                    content += "<p>Thank you for your consideration</p>";
                    clsMail.SendTos.AddRange(await master.GetManagementOfDepartment(services.User_Id));
                    clsMail.SendTos.Add(services.User_Id);
                    clsMail.Subject = subject;
                    clsMail.Body = content;
                }

                return await clsMail.SendMail(clsMail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_SetReturnAssign(ServiceComments model, string methodName)
        {
            try
            {

                Services services = await db.Services.FindAsync(model.Service_Id);
                Guid actionUserId = services.Action_User_Id.Value;
                services.Update = DateTime.Now;
                services.Action_User_Id = null;
                db.Entry(services).State = EntityState.Modified;
                if (await db.SaveChangesAsync() > 0)
                {
                    await AddServiceComment(services.Service_Id, model.Comment_Content);
                    string commentContent = string.Format("Return assignments to {0} department", db.Master_Departments.Find(services.Department_Id).Department_Name);
                    await AddServiceComment(services.Service_Id, commentContent);
                    commentContent = $"{model.Comment_Content}<br />{commentContent}";


                    var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                    linkUrl += "/" + services.Service_Id;
                    linkUrl = linkUrl.Replace(methodName, "Action");

                    string subject = string.Format("[Return assignments] {0} - {1}", services.Service_Key, services.Service_Subject);
                    string content = string.Format("<p><b>Comment:</b> {0}<br />{1}", model.Comment_Content, commentContent);
                    content += "</p>";
                    content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                    content += "<p>Thank you for your consideration</p>";
                    clsMail.SendTos.AddRange(await master.GetManagementOfDepartment(actionUserId));
                    clsMail.SendCCs.Add(actionUserId);
                    clsMail.Subject = subject;
                    clsMail.Body = content;

                }

                return await clsMail.SendMail(clsMail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_SetReturnJob(ServiceComments model, string methodName)
        {
            try
            {
                Services services = await db.Services.FindAsync(model.Service_Id);

                services.Update = DateTime.Now;
                services.Is_Commit = false;
                services.Department_Id = null;
                services.WorkRoot_Id = null;
                db.Entry(services).State = EntityState.Modified;

                if (await db.SaveChangesAsync() > 0)
                {
                    await AddServiceComment(services.Service_Id, model.Comment_Content);
                    string commentContent = "Return job to center room";
                    await AddServiceComment(services.Service_Id, commentContent);
                    commentContent = $"{model.Comment_Content}<br />{commentContent}";

                    if (services.Assign_User_Id.HasValue)
                    {
                        var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                        linkUrl += "/" + services.Service_Id;
                        linkUrl = linkUrl.Replace(methodName, "Commit");

                        string subject = string.Format("[Returned] {0} - {1}", services.Service_Key, services.Service_Subject);
                        string content = string.Format("<p><b>Comment:</b> {0}<br />{1}", model.Comment_Content, commentContent);
                        content += "</p>";
                        content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                        content += "<p>Thank you for your consideration</p>";
                        clsMail.SendTos.Add(services.Assign_User_Id.Value);
                        clsMail.SendCCs.Add(services.User_Id);
                        if (services.Create_User_Id != services.User_Id)
                        {
                            clsMail.SendCCs.Add(services.Create_User_Id);
                        }
                        clsMail.Subject = subject;
                        clsMail.Body = content;
                    }
                }

                return await clsMail.SendMail(clsMail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_SetToDepartment(Guid id, string methodName, Guid? deptId = null)
        {
            try
            {
                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                if (!deptId.HasValue)
                {
                    deptId = await db.Users
                        .Where(w => w.User_Id == userId)
                        .Select(s => s.Master_Processes.Master_Sections.Department_Id)
                        .FirstOrDefaultAsync();
                }

                List<Guid> listTeam = await db.Users
                    .Where(w => w.Master_Processes.Master_Sections.Department_Id == deptId)
                    .Select(s => s.User_Id)
                    .ToListAsync();

                var listTeamName = listTeam.Select(s => master.Users_GetInfomation(s)).ToList();

                string deptName = await db.Master_Departments
                    .Where(w => w.Department_Id == deptId)
                    .Select(s => s.Department_Name)
                    .FirstOrDefaultAsync();

                Services services = await db.Services.FindAsync(id);
                services.Department_Id = deptId;
                services.Is_Commit = true;
                services.Update = DateTime.Now;
                services.Assign_User_Id = userId;
                db.Entry(services).State = EntityState.Modified;
                if (await db.SaveChangesAsync() > 0)
                {
                    string commentContent = string.Format("Commit Task, Assign task to the {0} department", deptName);
                    await AddServiceComment(services.Service_Id,commentContent);

                    var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                    linkUrl = linkUrl.Replace(methodName, "Action");

                    string subject = string.Format("[Commit to department] {0} - {1}", services.Service_Key, services.Service_Subject);
                    string content = string.Format("<p><b>To:</b> {0}", string.Join("<br />", listTeamName));
                    content += "<br />";
                    content += string.Format("<b>Description:</b> {0}", services.Service_Description);
                    content += "<br />";
                    content += "<br />";
                    content += string.Format("<b>Comment:</b> {0}", commentContent);
                    content += "</p>";
                    content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                    content += "<p>Thank you for your consideration</p>";
                    clsMail.SendTos.Add(userId);
                    clsMail.Subject = subject;
                    clsMail.Body = content;
                }

                return await clsMail.SendMail(clsMail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_SetToUser(Guid id, Guid deptId, Guid userId, string methodName)
        {
            try
            {
                string deptName = await db.Master_Departments
                    .Where(w => w.Department_Id == deptId)
                    .Select(s => s.Department_Name)
                    .FirstOrDefaultAsync();

                Services services = await db.Services.FindAsync(id);
                services.Department_Id = deptId;
                services.Action_User_Id = userId;
                services.Is_Commit = true;
                services.Update = DateTime.Now;
                services.Assign_User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                db.Entry(services).State = EntityState.Modified;
                if (await db.SaveChangesAsync() > 0)
                {
                    string commentContent = string.Format("Commit Task, Assign task to the {0} department, Assign task to {1}", deptName, master.Users_GetInfomation(userId));
                    await AddServiceComment(services.Service_Id, commentContent);
                    

                    var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                    linkUrl = linkUrl.Replace(methodName, "Action");

                    string subject = string.Format("[Assigned] {0} - {1}", services.Service_Key, services.Service_Subject);
                    string content = string.Format("<p><b>To:</b> {0}", master.Users_GetInfomation(userId));
                    content += "<br />";
                    content += string.Format("<b>Description:</b> {0}", services.Service_Description);
                    content += "<br />";
                    content += "<br />";
                    content += string.Format("<b>Comment:</b> {0}", commentContent);
                    content += "</p>";
                    content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                    content += "<p>Thank you for your consideration</p>";
                    clsMail.SendTos.Add(userId);
                    clsMail.Subject = subject;
                    clsMail.Body = content;
                }

                return await clsMail.SendMail(clsMail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_SetToUser(Guid id, Guid userId, string methodName)
        {
            try
            {
                Services services = await db.Services.FindAsync(id);
                services.Action_User_Id = userId;
                services.Update = DateTime.Now;
                services.Assign_User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                db.Entry(services).State = EntityState.Modified;
                if (await db.SaveChangesAsync() > 0)
                {
                    string commentContent = string.Format("Assign task to {0}", master.Users_GetInfomation(userId));
                    await AddServiceComment(services.Service_Id, commentContent);

                    var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                    linkUrl = linkUrl.Replace(methodName, "Action");
                    linkUrl = string.Concat(linkUrl, "/", id);

                    string subject = string.Format("[Assigned] {0} - {1}", services.Service_Key, services.Service_Subject);
                    string content = string.Format("<p><b>To:</b> {0}", master.Users_GetInfomation(userId));
                    content += "<br />";
                    content += string.Format("<b>Description:</b> {0}", services.Service_Description);
                    content += "<br />";
                    content += "<br />";
                    content += string.Format("<b>Comment:</b> {0}", commentContent);
                    content += "</p>";
                    content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                    content += "<p>Thank you for your consideration</p>";
                    clsMail.SendTos.Add(userId);
                    clsMail.Subject = subject;
                    clsMail.Body = content;
                }

                return await clsMail.SendMail(clsMail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Services_Update(Services model, HttpFileCollectionBase files)
        {
            try
            {
                bool res = new bool();


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
                        string dir = Path.Combine("Service", model.Service_Key);
                        serviceFiles.ServiceFile_Path = await UploadFileToString(dir, files[i]);
                        serviceFiles.ServiceFile_Extension = Path.GetExtension(files[i].FileName);
                        db.ServiceFiles.Add(serviceFiles);
                    }
                }

                model.Update = DateTime.Now;

                if (await db.SaveChangesAsync() > 0)
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
                int[] finishIds = { 3, 4 };
                IQueryable<Services> query = db.ServiceTeams
                    .Where(w => w.User_Id == id && finishIds.Contains(w.Services.Status_Id))
                    .Select(s => s.Services);

                if (filter != null)
                {
                    query = query.Where(w => w.Update >= filter.Date_From);

                    filter.Date_To = filter.Date_To.AddDays(1);
                    query = query.Where(w => w.Update <= filter.Date_To);
                }

                return query.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //API Complete
        public async Task<string> UploadFileToString(string fullDir, HttpPostedFileBase filePost)
        {
            try
            {
                clsServiceFile.FolderPath = fullDir;

                clsServiceFile.Filename = filePost.FileName;

                fileResponse = await clsApi.UploadFile(clsServiceFile, filePost);

                return fileResponse.FileUrl;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //API Complete
        public async Task<string> UploadFileToString(string fullDir, HttpPostedFileBase filePost, string fileName)
        {
            try
            {
                clsServiceFile.FolderPath = fullDir;

                if (!string.IsNullOrEmpty(fileName))
                {
                    clsServiceFile.Filename = fileName;
                }
                else
                {
                    clsServiceFile.Filename = filePost.FileName;
                }

                fileResponse = await clsApi.UploadFile(clsServiceFile, filePost);

                return fileResponse.FileUrl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ClsImage> UploadImageToString(string fullDir, HttpPostedFileBase filePost, string fileName = "")
        {
            if (string.IsNullOrEmpty(fullDir))
            {
                throw new ArgumentException($"'{nameof(fullDir)}' cannot be null or empty.", nameof(fullDir));
            }

            if (filePost is null)
            {
                throw new ArgumentNullException(nameof(filePost));
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException($"'{nameof(fileName)}' cannot be null or empty.", nameof(fileName));
            }

            try
            {
                ClsImage res = new ClsImage();

                clsServiceFile.FolderPath = fullDir;

                if (!string.IsNullOrEmpty(fileName))
                {
                    clsServiceFile.Filename = fileName;
                }
                else
                {
                    clsServiceFile.Filename = filePost.FileName;
                }

                fileResponse = await clsApi.UploadFile(clsServiceFile, filePost);

                res.OriginalPath = fileResponse.FileUrl;
                res.ThumbnailPath = fileResponse.FileThumbnailUrl;

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task JobDaily()
        {
            var today = DateTime.Now;
            var services = await db.Services
                .Where(s => s.Status_Id == 3 && s.Update.HasValue)
                .Select(s => new
                {
                    s.Service_Id,
                    UpdateDate = s.Update.Value
                })
                .ToListAsync();

            foreach (var service in services)
            {
                //daysSinceChange default is 2 4 6
                //int[] difRange = { 2, 4, 6 };
                //daysSinceChange test is 1 2 3
                int[] difRange = { 1, 2, 3 };

                var daysSinceChange = (today.Date - service.UpdateDate.Date).Days;

                if (difRange.Contains(daysSinceChange))
                {
                    await clsMail.ResendMail(service.Service_Id);
                }
            }
        }


        public async Task JobMonthly()
        {
            var services = await db.Services
                .Where(s => s.Status_Id == 3 && s.Update.HasValue && s.Update.Value.Month < DateTime.Now.Month)
                .ToListAsync();
            foreach (var service in services)
            {
                await Services_SetClose(service, true);
            }
        }
    }
}
