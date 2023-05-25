using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Transactions;

namespace E2E.Models
{
    public static class ClsDefaultSystem
    {
        private static readonly ClsContext db = new ClsContext();

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

        public static void Generate()
        {
            try
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
            }
            catch (Exception)
            {
                throw;
            }
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    // Check if there are any users in the Users table
                    if (db.Users.Count() > 0)
                    {
                        int thisYear = DateTime.Today.Year;
                        List<Users> users = db.Users
                            .Where(w => w.YearSetPoint != thisYear)
                            .ToList();

                        // Update User_Point and YearSetPoint for users that don't have the current
                        // year as their YearSetPoint
                        if (users.Count > 0)
                        {
                            int setPoint = db.System_Configurations
                                .OrderByDescending(o => o.CreateDateTime)
                                .Select(s => s.Configuration_Point)
                                .FirstOrDefault();

                            foreach (var user in users)
                            {
                                user.User_Point = setPoint;
                                user.YearSetPoint = thisYear;
                                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                            }
                        }
                    }
                    else
                    {
                        Master_LineWorks master_LineWorks = new Master_LineWorks
                        {
                            Active = true,
                            Authorize_Id = 3,
                            LineWork_Name = "E-Engineering"
                        };
                        db.Entry(master_LineWorks).State = System.Data.Entity.EntityState.Added;

                        Master_Grades master_Grades = new Master_Grades
                        {
                            Active = true,
                            Grade_Name = "E8",
                            Grade_Position = "Engineer",
                            LineWork_Id = master_LineWorks.LineWork_Id
                        };
                        db.Entry(master_Grades).State = System.Data.Entity.EntityState.Added;

                        Master_Plants master_Plants = new Master_Plants
                        {
                            Active = true,
                            Plant_Name = "Bangpoo12"
                        };
                        db.Entry(master_Plants).State = System.Data.Entity.EntityState.Added;

                        Master_Divisions master_Divisions = new Master_Divisions
                        {
                            Active = true,
                            Division_Name = "Administrative"
                        };
                        db.Entry(master_Divisions).State = System.Data.Entity.EntityState.Added;

                        Master_Departments master_Departments = new Master_Departments
                        {
                            Active = true,
                            Department_Name = "Information Technology",
                            Division_Id = master_Divisions.Division_Id
                        };
                        db.Entry(master_Departments).State = System.Data.Entity.EntityState.Added;

                        Master_Sections master_Sections = new Master_Sections
                        {
                            Active = true,
                            Section_Name = "SAP & Application",
                            Department_Id = master_Departments.Department_Id
                        };
                        db.Entry(master_Sections).State = System.Data.Entity.EntityState.Added;

                        Master_Processes master_Processes = new Master_Processes
                        {
                            Active = true,
                            Process_Name = "Information Technology Bangpoo",
                            Section_Id = master_Sections.Section_Id
                        };
                        db.Entry(master_Processes).State = System.Data.Entity.EntityState.Added;

                        System_Prefix_EN system_Prefix_EN = new System_Prefix_EN()
                        {
                            Prefix_EN_Name = "Mr."
                        };
                        db.Entry(system_Prefix_EN).State = System.Data.Entity.EntityState.Added;

                        System_Prefix_TH system_Prefix_TH = new System_Prefix_TH()
                        {
                            Prefix_TH_Name = "นาย"
                        };
                        db.Entry(system_Prefix_TH).State = System.Data.Entity.EntityState.Added;
                        int point = 50; // Default value if ConfigurationManager.AppSettings["Point"] is not present or not a valid integer

                        if (ConfigurationManager.AppSettings.AllKeys.Contains("Point"))
                        {
                            string pointConfigValue = ConfigurationManager.AppSettings["Point"];
                            if (!string.IsNullOrEmpty(pointConfigValue) && int.TryParse(pointConfigValue, out int parsedPoint) && parsedPoint > 0)
                            {
                                point = parsedPoint;
                            }
                        }
                        Users users = new Users()
                        {
                            Grade_Id = master_Grades.Grade_Id,
                            Plant_Id = master_Plants.Plant_Id,
                            Process_Id = master_Processes.Process_Id,
                            Role_Id = 1,
                            User_Code = "1640488",
                            User_Point = point
                        };
                        db.Entry(users).State = System.Data.Entity.EntityState.Added;

                        UserDetails userDetails = new UserDetails()
                        {
                            User_Id = users.User_Id,
                            Prefix_EN_Id = system_Prefix_EN.Prefix_EN_Id,
                            Prefix_TH_Id = system_Prefix_TH.Prefix_TH_Id
                        };
                        db.Entry(userDetails).State = System.Data.Entity.EntityState.Added;
                    }

                    if (db.SaveChanges() > 0)
                    {
                        scope.Complete();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
