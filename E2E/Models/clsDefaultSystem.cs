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

            if (db.System_Language.Count() != System_Language.DefaultList().Count())
            {
                Language_Save();
            }

            if (db.System_ManualType.Count() != System_ManualType.DefaultList().Count())
            {
                ManualType_Save();
            }

            if (db.System_Roles.Count() != System_Roles.DefaultList().Count())
            {
                Role_Save();
            }

            if (db.System_Statuses.Count() != System_Statuses.DefaultList().Count())
            {
                Status_Save();
            }

            if (db.System_DueDateStatuses.Count() != System_DueDateStatuses.DefaultList().Count())
            {
                DueDateStatus_Save();
            }

            if (db.System_Priorities.Count() != System_Priorities.DefaultList().Count())
            {
                Priority_Save();
            }

            if (db.Users.Count() > 0)
            {
                int thisYear = DateTime.Today.Year;
                List<Users> users = new List<Users>();
                users = db.Users
                    .Where(w => w.YearSetPoint != thisYear)
                    .ToList();
                if (users.Count > 0)
                {
                    int setPoint = db.System_Configurations.OrderByDescending(o => o.CreateDateTime).Select(s => s.Configuration_Point).FirstOrDefault();

                    foreach (var item in users)
                    {
                        item.User_Point = setPoint;
                        item.YearSetPoint = thisYear;
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    }
                    db.SaveChanges();
                }
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

        private static void Language_Save()
        {
            foreach (var item in System_Language.DefaultList())
            {
                if (db.System_Language.Where(w => w.Language_Name == item.Language_Name).Count() == 0)
                {
                    db.System_Language.Add(item);
                    db.SaveChanges();
                }
            }
        }

        private static void ManualType_Save()
        {
            foreach (var item in System_ManualType.DefaultList())
            {
                if (db.System_ManualType.Where(w => w.Manual_TypeName == item.Manual_TypeName).Count() == 0)
                {
                    db.System_ManualType.Add(item);
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

        private static void DueDateStatus_Save()
        {
            foreach (var item in System_DueDateStatuses.DefaultList())
            {
                if (db.System_DueDateStatuses.Where(w => w.DueDateStatus_Name == item.DueDateStatus_Name).Count() == 0)
                {
                    db.System_DueDateStatuses.Add(item);
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