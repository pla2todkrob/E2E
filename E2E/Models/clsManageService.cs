using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace E2E.Models
{
    public class clsManageService
    {
        private clsContext db = new clsContext();
        private clsServiceFTP ftp = new clsServiceFTP();
        private clsManageMaster master = new clsManageMaster();
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

        private IQueryable<Services> Services_GetWaitCommit_IQ()
        {
            try
            {
                Guid statusId = db.System_Statuses
                .Where(w => w.Status_Index == 1)
                .Select(s => s.Status_Id)
                .FirstOrDefault();

                return db.Services
                    .Where(w => !w.Is_Commit && w.Status_Id == statusId && (!w.Is_MustBeApproved || (w.Is_Approval && w.Is_MustBeApproved)))
                    .OrderByDescending(o => o.System_Priorities.Priority_Index)
                    .ThenBy(o => new { o.Create, o.Service_DueDate });
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

        private IQueryable<Services> Services_GetWaitAction_IQ(Guid? id)
        {
            try
            {
                Guid statusId = db.System_Statuses
                .Where(w => w.Status_Index == 1)
                .Select(s => s.Status_Id)
                .FirstOrDefault();

                IQueryable<Services> query = db.Services
                    .Where(w => w.Is_Commit && w.Status_Id == statusId)
                    .OrderByDescending(o => o.System_Priorities.Priority_Index)
                    .ThenBy(o => new { o.Create, o.Service_DueDate });

                if (id.HasValue)
                {
                    Guid? deptId = db.Users
                        .Where(w => w.User_Id == id.Value)
                        .Select(s => s.Master_Processes.Master_Sections.Department_Id)
                        .FirstOrDefault();
                    if (deptId.HasValue)
                    {
                        query = query.Where(w => (w.Department_Id == deptId.Value && !w.Is_Action) || w.Action_User_Id == id);
                    }
                }

                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Services> Services_GetNoPendingList()
        {
            return Services_GetNoPending_IQ().ToList();
        }

        public int Services_GetNoPendingCount()
        {
            return Services_GetNoPending_IQ().Count();
        }

        private IQueryable<Services> Services_GetNoPending_IQ()
        {
            Guid statusId = db.System_Statuses
                .Where(w => w.Status_Index == 1)
                .Select(s => s.Status_Id)
                .FirstOrDefault();

            return db.Services
                .Where(w => w.Status_Id != statusId)
                .OrderByDescending(o => o.System_Priorities.Priority_Index)
                .ThenBy(o => new { o.Create, o.Service_DueDate });
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

        private IQueryable<Services> Services_GetAllRequest_IQ()
        {
            try
            {
                Guid id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                Guid departmentId = db.Users
                    .Where(w => w.User_Id == id)
                    .Select(s => s.Master_Processes.Master_Sections.Department_Id.Value)
                    .FirstOrDefault();
                List<Guid> userIds = db.Users
                    .Where(w => w.Master_Processes.Master_Sections.Department_Id == departmentId)
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
        public List<Services> Services_GetMyTask()
        {
            try
            {
                Guid id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                return Services_GetAllTask_IQ()
                    .Where(w => w.Action_User_Id == id)
                    .ToList();
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

        private IQueryable<Services> Services_GetAllTask_IQ()
        {
            try
            {
                Guid id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                Guid departmentId = db.Users
                    .Where(w => w.User_Id == id)
                    .Select(s => s.Master_Processes.Master_Sections.Department_Id.Value)
                    .FirstOrDefault();

                IQueryable<Services> query = db.Services
                    .Where(w => w.Department_Id == departmentId);

                return query;
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

        public clsServices ClsServices_View(Guid id)
        {
            try
            {
                clsServices clsServices = new clsServices();
                clsServices =  db.Services
                    .Where(w => w.Service_Id == id)
                    .GroupJoin(db.ServiceFiles, m => m.Service_Id, j => j.Service_Id, (m, gj) => new clsServices() {
                        ServiceFiles = gj.ToList(),
                        Services = m
                    }).FirstOrDefault();
                clsServices.User_Name = master.Users_GetInfomation(clsServices.Services.User_Id);
                if (clsServices.Services.Action_User_Id.HasValue)
                {
                    UserDetails userDetails = new UserDetails();
                    userDetails = db.UserDetails
                        .Where(w => w.User_Id == clsServices.Services.Action_User_Id)
                        .FirstOrDefault();
                    clsServices.Action_Email = userDetails.Users.User_Email;
                    clsServices.Action_Name = master.Users_GetInfomation(userDetails.User_Id);
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

                clsServices.ClsServiceComments.ForEach(f => f.UserInfomation = master.Users_GetInfomation(f.ServiceComments.User_Id.Value));

                return clsServices;
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

        public List<Services> Services_GetRequiredApprove(bool val)
        {
            try
            {
                Guid statusId = db.System_Statuses
                .Where(w => w.Status_Index == 1)
                .Select(s => s.Status_Id)
                .FirstOrDefault();

                Guid id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                Guid deptId = db.Users.Find(id).Master_Processes.Master_Sections.Department_Id.Value;
                List<Guid> userIdList = db.Users
                    .Where(w => w.Master_Processes.Master_Sections.Department_Id == deptId).Select(s => s.User_Id).ToList();
                return db.Services.Where(w => w.Is_MustBeApproved && w.Is_Approval == val && userIdList.Contains(w.User_Id) && w.Status_Id == statusId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_Save(Services model, HttpFileCollectionBase files)
        {
            try
            {
                bool res = new bool();
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
                    res = Services_Insert(model, files);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Services_Insert(Services model, HttpFileCollectionBase files)
        {
            try
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

                bool res = new bool();
                int todayCount = db.Services
                    .Where(w => w.Create >= DateTime.Today)
                    .Count();
                todayCount += 1;
                model.Service_Key = string.Concat(DateTime.Now.ToString("yyMMdd"), todayCount.ToString().PadLeft(3, '0'));
                model.Status_Id = db.System_Statuses
                    .Where(w => w.Status_Index == 1)
                    .Select(s => s.Status_Id)
                    .FirstOrDefault();
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
                    res = true;
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

        public bool Services_SetRequired(Guid id)
        {
            try
            {
                bool res = new bool();
                Services services = new Services();
                services = db.Services.Find(id);
                services.Is_MustBeApproved = true;
                services.Update = DateTime.Now;
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    serviceComments.Service_Id = id;
                    serviceComments.Comment_Content = "Approval required";
                    res = Services_Comment(serviceComments);
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
                Guid userActionId = Guid.Parse(HttpContext.Current.User.Identity.Name);

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
                    res = Services_Comment(serviceComments);
                }
                

                

                return res;
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

        public bool Services_SetApprove(Guid id)
        {
            try
            {
                bool res = new bool();
                Services services = new Services();
                services = db.Services.Find(id);
                services.Is_Approval = true;
                services.Update = DateTime.Now;
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    serviceComments.Service_Id = id;
                    serviceComments.Comment_Content = "Approved";
                    res = Services_Comment(serviceComments);
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
                    .Where(w => w.Status_Index == 2)
                    .FirstOrDefault();

                services.Service_EstimateTime = model.Service_EstimateTime;
                services.Status_Id = system_Statuses.Status_Id;
                services.Update = DateTime.Now;
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ServiceComments serviceComments = new ServiceComments();
                    serviceComments.Service_Id = services.Service_Id;
                    serviceComments.Comment_Content = string.Format("Start task, Status update to {0}", system_Statuses.Status_Name);
                    res = Services_Comment(serviceComments);
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
                    .Where(w => w.Status_Index == 3)
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
                    res = Services_Comment(serviceComments);
                }


                return res;
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
                    .Where(w => w.Status_Index == 5)
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
                    res = Services_Comment(serviceComments);
                }

                return res;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<SelectListItem> SelectListItems_Priority()
        {
            try
            {
                return db.System_Priorities
                .OrderBy(o => o.Priority_Index)
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

        public List<SelectListItem> SelectListItems_RefService()
        {
            try
            {
                return Services_GetNoPending_IQ()
                .Select(s => new SelectListItem()
                {
                    Value = s.Service_Id.ToString(),
                    Text = s.Service_Key + "(" + s.Service_Subject + ")"
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
                return db.Users
                .Where(w => w.Active)
                .OrderBy(o => o.User_Code)
                .Select(s => new SelectListItem()
                {
                    Value = s.User_Id.ToString(),
                    Text = s.User_Code + " [Point: " + s.User_Point + "]"
                }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}