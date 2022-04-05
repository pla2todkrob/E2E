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

        public List<Services> Services_GetPending(bool val)
        {
            return Services_IQ_GetPending(val).ToList();
        }

        public IQueryable<Services> Services_IQ_GetPending(bool val)
        {
            Guid statusId = db.System_Statuses
                .Where(w => w.Status_Index == 1)
                .Select(s => s.Status_Id)
                .FirstOrDefault();

            return Services_IQ_GetAll(val)
                .Where(w => w.Status_Id == statusId);
        }

        public List<Services> Services_GetNoPending(bool val)
        {
            return Services_IQ_GetNoPending(val).ToList();
        }

        public IQueryable<Services> Services_IQ_GetNoPending(bool val)
        {
            List<Guid> statusIds = db.System_Statuses
                .Where(w => w.Status_Index != 1)
                .Select(s => s.Status_Id)
                .ToList();

            return Services_IQ_GetAll(val)
                .Where(w => statusIds.Contains(w.Status_Id));
        }

        public List<SelectListItem> SelectListItems_Priority()
        {
            return db.System_Statuses
                .OrderBy(o => o.Status_Index)
                .Select(s => new SelectListItem()
                {
                    Value = s.Status_Id.ToString(),
                    Text = s.Status_Name
                }).ToList();
        }

        public List<SelectListItem> SelectListItems_RefService()
        {
            return Services_IQ_GetNoPending(true)
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
                    Text = s.User_Code + "[P:" + s.User_Point + "]"
                }).ToList();
        }
    }
}