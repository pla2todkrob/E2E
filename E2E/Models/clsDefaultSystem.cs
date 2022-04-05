using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models
{
    public static class clsDefaultSystem
    {
        private static clsContext db = new clsContext();
        public static void Generate()
        {
            if (db.System_Authorizes.Count() != System_Authorize.DefaultList().Count())
            {
                Authorize_Save();
            }

            if (db.System_Roles.Count() != System_Roles.DefaultList().Count())
            {
                Role_Save();
            }
            if (db.System_Statuses.Count() != System_Statuses.DefaultList().Count())
            {
                Status_Save();
            }

            if (db.System_Priorities.Count() != System_Priorities.DefaultList().Count())
            {
                Priority_Save();
            }
        }

        private static void Authorize_Save()
        {
            foreach (var item in System_Authorize.DefaultList())
            {
                if (db.System_Authorizes.Where(w => w.Authorize_Name == item.Authorize_Name).Count() == 0)
                {
                    db.System_Authorizes.Add(item);
                    db.SaveChanges();
                }
            }
        }

        private static void Role_Save()
        {
            foreach (var item in System_Roles.DefaultList())
            {
                if (db.System_Roles.Where(w => w.Role_Name == item.Role_Name).Count() == 0)
                {
                    db.System_Roles.Add(item);
                    db.SaveChanges();
                }
            }
        }

        private static void Status_Save()
        {
            foreach (var item in System_Statuses.DefaultList())
            {
                if (db.System_Statuses.Where(w => w.Status_Name == item.Status_Name).Count() == 0)
                {
                    db.System_Statuses.Add(item);
                    db.SaveChanges();
                }
            }
        }

        private static void Priority_Save()
        {
            foreach (var item in System_Priorities.DefaultList())
            {
                if (db.System_Priorities.Where(w => w.Priority_Name == item.Priority_Name).Count() == 0)
                {
                    db.System_Priorities.Add(item);
                    db.SaveChanges();
                }
            }
        }
    }
}