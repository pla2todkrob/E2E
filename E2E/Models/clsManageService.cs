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
                    .Where(w => !w.Commit_User_Id.HasValue && w.Status_Id == statusId && (!w.Required_Approve_User_Id.HasValue || (w.Approved_User_Id.HasValue && w.Required_Approve_User_Id.HasValue)))
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
                    .Where(w => w.Commit_User_Id.HasValue && w.Status_Id == statusId)
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
                        query = query.Where(w => (w.Department_Id == deptId.Value && !w.Action_User_Id.HasValue) || w.Action_User_Id == id);
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
                return db.Services
                    .Where(w => w.User_Id == id)
                    .ToList();
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
                return db.Services
                    .Where(w => w.Service_Id == id)
                    .GroupJoin(db.UserDetails, m => m.Approved_User_Id, j => j.User_Id, (m, gj) => new
                    {
                        Services = m,
                        Approve = gj.FirstOrDefault()
                    })
                    .GroupJoin(db.UserDetails, m => m.Services.Action_User_Id, j => j.User_Id, (m, gj) => new
                    {
                        DataJoin = m,
                        Action = gj.FirstOrDefault()
                    })
                    .GroupJoin(db.UserDetails, m => m.DataJoin.Services.Commit_User_Id, j => j.User_Id, (m, gj) => new
                    {
                        DataJoin = m,
                        Commit = gj.FirstOrDefault()
                    })
                    .GroupJoin(db.ServiceFiles, m => m.DataJoin.DataJoin.Services.Service_Id, j => j.Service_Id, (m, gj) => new clsServices()
                    {
                        Action_User_Code = m.DataJoin.Action.Users.User_Code,
                        Action_User_Email = m.DataJoin.Action.Users.User_Email,
                        Action_User_Name = m.DataJoin.Action.Detail_EN_FirstName + " " + m.DataJoin.Action.Detail_EN_LastName,
                        Approved_User_Code = m.DataJoin.DataJoin.Approve.Users.User_Code,
                        Approved_User_Email = m.DataJoin.DataJoin.Approve.Users.User_Email,
                        Approved_User_Name = m.DataJoin.DataJoin.Approve.Detail_EN_FirstName + " " + m.DataJoin.DataJoin.Approve.Detail_EN_LastName,
                        Commit_User_Code = m.Commit.Users.User_Code,
                        Commit_User_Email = m.Commit.Users.User_Email,
                        Commit_User_Name = m.Commit.Detail_EN_FirstName + " " + m.Commit.Detail_EN_LastName,
                        ServiceFiles = gj.ToList(),
                        Services = m.DataJoin.DataJoin.Services,
                        User_Name = db.UserDetails.Where(w => w.User_Id == m.DataJoin.DataJoin.Services.User_Id).Select(s => new { Name = s.Detail_EN_FirstName + " " + s.Detail_EN_LastName }).Select(s => s.Name).FirstOrDefault()
                    }).FirstOrDefault();
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
                return db.Services.Where(w => w.Required_Approve_User_Id.HasValue && w.Approved_User_Id.HasValue == val && userIdList.Contains(w.User_Id.Value) && w.Status_Id == statusId).ToList();
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

                    if (model.User_Id.HasValue)
                    {
                        services.User_Id = model.User_Id;
                    }
                    else
                    {
                        services.User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                    }

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
                if (!model.User_Id.HasValue)
                {
                    model.User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                }

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
                        string dir = string.Format("Service/{0}/", model.Service_Key);
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
                        string dir = string.Format("Service/{0}/", model.Service_Key);
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
                services.Required_Approve_User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                services.Required_Approve_DateTime = DateTime.Now;
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
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

        public bool Services_SetToDepartment(Guid id)
        {
            try
            {
                bool res = new bool();
                Guid userId = Guid.Parse(HttpContext.Current.User.Identity.Name);
                Guid deptId = db.Users.Find(userId).Master_Processes.Master_Sections.Department_Id.Value;
                Services services = new Services();
                services = db.Services.Find(id);
                services.Department_Id = deptId;
                services.Commit_User_Id = userId;
                services.Commit_DateTime = DateTime.Now;
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
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
                services.Approved_User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                services.Approved_DateTime = DateTime.Now;
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
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

        public bool Services_SetAction(Guid id)
        {
            try
            {
                bool res = new bool();
                Services services = new Services();
                services = db.Services.Find(id);
                if (!services.Action_User_Id.HasValue)
                {
                    services.Action_User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                }

                services.Action_DateTime = DateTime.Now;
                services.Status_Id = db.System_Statuses
                    .Where(w => w.Status_Index == 2)
                    .Select(s => s.Status_Id)
                    .FirstOrDefault();
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
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

        public bool Services_SetCommit(Services model)
        {
            try
            {
                bool res = new bool();
                Services services = new Services();
                services = db.Services.Find(model.Service_Id);
                services.Commit_User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                services.Commit_DateTime = DateTime.Now;
                services.Department_Id = model.Department_Id;
                if (model.Action_User_Id.HasValue)
                {
                    services.Action_User_Id = model.Action_User_Id;
                }
                db.Entry(services).State = System.Data.Entity.EntityState.Modified;
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