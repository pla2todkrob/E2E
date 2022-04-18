using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
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
            return Services_IQ_GetAll(val).ToList();
        }

        public IQueryable<Services> Services_IQ_GetAll(bool val)
        {
            return db.Services
                .Where(w => w.CommitService == val)
                .OrderBy(o => o.Create);
        }

        public List<Services> Services_GetAllPending(bool val)
        {
            return Services_IQ_GetAllPending(val).ToList();
        }

        public IQueryable<Services> Services_IQ_GetAllPending(bool val)
        {
            Guid statusId = db.System_Statuses
                .Where(w => w.Status_Index == 1)
                .Select(s => s.Status_Id)
                .FirstOrDefault();

            return Services_IQ_GetAll(val)
                .Where(w => w.Status_Id == statusId);
        }

        public List<Services> Services_GetAllNoPending(bool val)
        {
            return Services_IQ_GetAllNoPending(val).ToList();
        }

        public IQueryable<Services> Services_IQ_GetAllNoPending(bool val)
        {
            List<Guid> statusIds = db.System_Statuses
                .Where(w => w.Status_Index != 1)
                .Select(s => s.Status_Id)
                .ToList();

            return Services_IQ_GetAll(val)
                .Where(w => statusIds.Contains(w.Status_Id));
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
                    model.Create = services.Create;
                    model.Status_Id = services.Status_Id;
                    model.Service_Key = services.Service_Key;
                    model.RequiredApprove = services.RequiredApprove;
                    res = Services_Update(model, files);
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
                if (model.User_Id == Guid.Empty)
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
                db.Entry(model).State = System.Data.Entity.EntityState.Added;
                if (db.SaveChanges() > 0)
                {
                    if (files.Count > 0)
                    {
                        for (int i = 0; i < files.Count; i++)
                        {
                            ServiceFiles serviceFiles = new ServiceFiles();
                            serviceFiles.Service_Id = model.Service_Id;
                            serviceFiles.ServiceFile_Name = files[i].FileName;
                            serviceFiles.ServiceFile_Path = ftp.Ftp_UploadFileToString("", files[i]);
                        }
                    }
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

        public List<SelectListItem> SelectListItems_Priority()
        {
            return db.System_Priorities
                .OrderBy(o => o.Priority_Index)
                .Select(s => new SelectListItem()
                {
                    Value = s.Priority_Id.ToString(),
                    Text = s.Priority_Name + " [Point: " + s.Priority_Point + "]"
                }).ToList();
        }

        public List<SelectListItem> SelectListItems_RefService()
        {
            return Services_IQ_GetAllNoPending(true)
                .Select(s => new SelectListItem()
                {
                    Value = s.Service_Id.ToString(),
                    Text = s.Service_Key + "(" + s.Service_Subject + ")"
                }).ToList();
        }

        public List<SelectListItem> SelectListItems_User()
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
    }
}