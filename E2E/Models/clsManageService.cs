using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E2E.Models
{
    public class clsManageService
    {
        private clsContext db = new clsContext();
        private clsServiceFTP ftp = new clsServiceFTP();

        public List<Services> Services_GetAll(bool val)
        {
            try
            {
                return Services_IQ_GetAll(val).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<Services> Services_IQ_GetAll(bool val)
        {
            try
            {
                return db.Services
                .Where(w => w.CommitService == val)
                .OrderByDescending(o => o.System_Priorities.Priority_Index)
                .ThenBy(o => new { o.Create, o.Service_DueDate });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Services> Services_GetAllPending(bool val)
        {
            try
            {
                return Services_IQ_GetAllPending(val).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Count_GetAllPending(bool val)
        {
            try
            {
                return Services_IQ_GetAllPending(val).Count();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<Services> Services_IQ_GetAllPending(bool val)
        {
            try
            {
                Guid statusId = db.System_Statuses
                .Where(w => w.Status_Index == 1)
                .Select(s => s.Status_Id)
                .FirstOrDefault();

                return Services_IQ_GetAll(val)
                    .Where(w => w.Status_Id == statusId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Services> Services_GetAllNoPending(bool val)
        {
            try
            {
                return Services_IQ_GetAllNoPending(val).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<Services> Services_IQ_GetAllNoPending(bool val)
        {
            try
            {
                List<Guid> statusIds = db.System_Statuses
                .Where(w => w.Status_Index != 1)
                .Select(s => s.Status_Id)
                .ToList();

                return Services_IQ_GetAll(val)
                    .Where(w => statusIds.Contains(w.Status_Id));
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
                    .Select(s => new clsServices()
                    {
                        Services = s,
                        ServiceFiles = db.ServiceFiles.Where(w => w.Service_Id == s.Service_Id).ToList()
                    }).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<clsServices> ClsServices_ViewList(Guid id)
        {
            List<clsServices> res = new List<clsServices>();
            bool hasData = true;
            while (hasData)
            {
                clsServices clsServices = new clsServices();
                clsServices = db.Services
                    .Where(w => w.Service_Id == id)
                    .Select(s => new clsServices()
                    {
                        Services = s,
                        ServiceFiles = db.ServiceFiles.Where(w => w.Service_Id == s.Service_Id).ToList()
                    }).FirstOrDefault();
                res.Add(clsServices);
                if (clsServices.Services.Ref_Service_Id.HasValue)
                {
                    id = clsServices.Services.Ref_Service_Id.Value;
                }
                else
                {
                    hasData = false;
                }
            }

            return res;
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
                return Services_IQ_GetAllNoPending(true)
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