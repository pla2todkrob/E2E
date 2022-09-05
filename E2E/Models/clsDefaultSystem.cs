﻿using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace E2E.Models
{
    public static class clsDefaultSystem
    {
        private static clsContext db = new clsContext();

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
            else
            {
                Master_LineWorks master_LineWorks = new Master_LineWorks();
                master_LineWorks.Active = true;
                master_LineWorks.Authorize_Id = 3;
                master_LineWorks.LineWork_Name = "E-Engineering";

                db.Master_LineWorks.Add(master_LineWorks);

                if (db.SaveChanges() > 0)
                {
                    Master_Grades master_Grades = new Master_Grades();
                    master_Grades.Active = true;
                    master_Grades.Grade_Name = "E8";
                    master_Grades.Grade_Position = "Engineer";
                    master_Grades.LineWork_Id = master_LineWorks.LineWork_Id;
                    db.Master_Grades.Add(master_Grades);
                    if (db.SaveChanges() > 0)
                    {
                        Master_Plants master_Plants = new Master_Plants();
                        master_Plants.Active = true;
                        master_Plants.Plant_Name = "Bangpoo12";

                        db.Master_Plants.Add(master_Plants);
                        if (db.SaveChanges() > 0)
                        {
                            Master_Divisions master_Divisions = new Master_Divisions();
                            master_Divisions.Active = true;
                            master_Divisions.Division_Name = "Administrative";
                            master_Divisions.Plant_Id = master_Plants.Plant_Id;
                            db.Master_Divisions.Add(master_Divisions);

                            if (db.SaveChanges() > 0)
                            {
                                Master_Departments master_Departments = new Master_Departments();
                                master_Departments.Active = true;
                                master_Departments.Department_Name = "Information Technology";
                                master_Departments.Division_Id = master_Divisions.Division_Id;

                                db.Master_Departments.Add(master_Departments);

                                if (db.SaveChanges() > 0)
                                {
                                    Master_Sections master_Sections = new Master_Sections();
                                    master_Sections.Active = true;
                                    master_Sections.Section_Name = "SAP & Application";
                                    master_Sections.Department_Id = master_Departments.Department_Id;

                                    db.Master_Sections.Add(master_Sections);

                                    if (db.SaveChanges() > 0)
                                    {
                                        Master_Processes master_Processes = new Master_Processes();
                                        master_Processes.Active = true;
                                        master_Processes.Process_Name = "Information Technology Bangpoo";
                                        master_Processes.Section_Id = master_Sections.Section_Id;

                                        db.Master_Processes.Add(master_Processes);

                                        if (db.SaveChanges() > 0)
                                        {
                                            Users users = new Users();
                                            users.Active = true;
                                            users.Grade_Id = master_Grades.Grade_Id;
                                            users.Process_Id = master_Processes.Process_Id;

                                            users.Role_Id = 1;
                                            users.User_Code = "1640488";
                                            users.User_CostCenter = "41100";
                                            users.User_Email = "nopparat@thaiparker.co.th";
                                            users.User_Point = 50;
                                            users.YearSetPoint = DateTime.Today.Year;

                                            db.Users.Add(users);

                                            if (db.SaveChanges() > 0)
                                            {
                                                System_Prefix_EN system_Prefix_EN = new System_Prefix_EN();
                                                system_Prefix_EN.Prefix_EN_Name = "Mr.";

                                                db.System_Prefix_ENs.Add(system_Prefix_EN);

                                                if (db.SaveChanges() > 0)
                                                {
                                                    System_Prefix_TH system_Prefix_TH = new System_Prefix_TH();
                                                    system_Prefix_TH.Prefix_TH_Name = "นาย";

                                                    db.System_Prefix_THs.Add(system_Prefix_TH);

                                                    if (db.SaveChanges() > 0)
                                                    {
                                                        UserDetails userDetails = new UserDetails();

                                                        userDetails.Detail_EN_FirstName = "Nopparat";
                                                        userDetails.Detail_EN_LastName = "Thongnuapad";
                                                        userDetails.Detail_TH_FirstName = "นพรัตน์";
                                                        userDetails.Detail_TH_LastName = "ทองเนื้อแปด";
                                                        userDetails.Prefix_EN_Id = system_Prefix_EN.Prefix_EN_Id;
                                                        userDetails.Prefix_TH_Id = system_Prefix_TH.Prefix_TH_Id;
                                                        userDetails.User_Id = users.User_Id;

                                                        db.UserDetails.Add(userDetails);

                                                        db.SaveChanges();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}