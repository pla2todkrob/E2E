using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace E2E.Models
{
    public class clsManageData
    {
        private clsContext db = new clsContext();

        public Guid? Prefix_EN_GetId(string val, bool create = false)
        {
            try
            {
                Guid? res = null;
            FindModel:
                System_Prefix_EN system_Prefix_EN = new System_Prefix_EN();
                system_Prefix_EN = db.System_Prefix_ENs
                    .Where(w => w.Prefix_EN_Name.ToLower() == val.ToLower().Trim())
                    .FirstOrDefault();
                if (system_Prefix_EN != null)
                {
                    res = system_Prefix_EN.Prefix_EN_Id;
                }
                else
                {
                    if (create)
                    {
                        if (!string.IsNullOrEmpty(val))
                        {
                            if (Prefix_EN_Save(val))
                            {
                                goto FindModel;
                            }
                        }
                    }
                }
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Prefix_EN_Save(string val)
        {
            try
            {
                bool res = new bool();
                System_Prefix_EN system_Prefix_EN = new System_Prefix_EN();
                system_Prefix_EN.Prefix_EN_Name = val.Trim();
                db.System_Prefix_ENs.Add(system_Prefix_EN);
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

        public Guid? Prefix_TH_GetId(string val, bool create = false)
        {
            try
            {
                Guid? res = null;
            FindModel:
                System_Prefix_TH system_Prefix_TH = new System_Prefix_TH();
                system_Prefix_TH = db.System_Prefix_THs
                    .Where(w => w.Prefix_TH_Name.ToLower() == val.ToLower().Trim())
                    .FirstOrDefault();
                if (system_Prefix_TH != null)
                {
                    res = system_Prefix_TH.Prefix_TH_Id;
                }
                else
                {
                    if (create)
                    {
                        if (!string.IsNullOrEmpty(val))
                        {
                            if (Prefix_TH_Save(val))
                            {
                                goto FindModel;
                            }
                        }
                    }
                }
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Prefix_TH_Save(string val)
        {
            try
            {
                bool res = new bool();
                System_Prefix_TH system_Prefix_TH = new System_Prefix_TH();
                system_Prefix_TH.Prefix_TH_Name = val.Trim();
                db.System_Prefix_THs.Add(system_Prefix_TH);
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

        public Guid Role_AdminId()
        {
            try
            {
                return db.System_Roles
                    .Where(w => w.Role_Index == 1)
                    .Select(s => s.Role_Id)
                    .FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Guid Role_UserId()
        {
            try
            {
                return db.System_Roles
                    .Where(w => w.Role_Index == 2)
                    .Select(s => s.Role_Id)
                    .FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Master_Processes Process_Get(Guid id)
        {
            return db.Master_Processes.Find(id);
        }
        public List<Master_Processes> Process_GetAll()
        {
            return db.Master_Processes.ToList();
        }
        public List<clsProcesses> Process_GetAllView()
        {
            return db.Master_Processes
                .Select(s => new clsProcesses()
                {
                    Active = s.Active,
                    Create = s.Create,
                    Department_Name = s.Master_Sections.Master_Departments.Department_Name,
                    Division_Name = s.Master_Sections.Master_Departments.Master_Divisions.Division_Name,
                    Plant_Name = s.Master_Sections.Master_Departments.Master_Divisions.Master_Plants.Plant_Name,
                    Process_Name = s.Process_Name,
                    Section_Name = s.Master_Sections.Section_Name,
                    Update = s.Update
                }).ToList();
        }
        public Guid? Process_GetId(Guid sectionId, string val, bool create = false)
        {
            try
            {
                Guid? res = null;
            FindModel:
                Master_Processes master_Processes = new Master_Processes();
                master_Processes = db.Master_Processes
                    .Where(w => w.Process_Name.ToLower() == val.ToLower().Trim() &&
                    w.Section_Id == sectionId)
                    .FirstOrDefault();
                if (master_Processes != null)
                {
                    res = master_Processes.Process_Id;
                }
                else
                {
                    if (create)
                    {
                        if (!string.IsNullOrEmpty(val))
                        {
                            if (Process_Save(sectionId, val))
                            {
                                goto FindModel;
                            }
                        }
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Process_Save(Guid sectionId, string val)
        {
            try
            {
                bool res = new bool();
                Master_Processes master_Processes = new Master_Processes();
                master_Processes.Code = db.Master_Processes.Count() + 1;
                master_Processes.Process_Name = val.Trim();
                master_Processes.Section_Id = sectionId;
                db.Master_Processes.Add(master_Processes);
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
        public Master_Sections Section_Get(Guid id)
        {
            return db.Master_Sections.Find(id);
        }
        public List<Master_Sections> Section_GetAll()
        {
            return db.Master_Sections.ToList();
        }
        public List<clsSections> Section_GetAllView()
        {
            return db.Master_Sections
                .Select(s => new clsSections()
                {
                    Active = s.Active,
                    Create = s.Create,
                    Department_Name = s.Master_Departments.Department_Name,
                    Division_Name = s.Master_Departments.Master_Divisions.Division_Name,
                    Plant_Name = s.Master_Departments.Master_Divisions.Master_Plants.Plant_Name,
                    Section_Name = s.Section_Name,
                    Update = s.Update
                }).ToList();
        }
        public Guid? Section_GetId(Guid departmentId, string val, bool create = false)
        {
            try
            {
                Guid? res = null;
            FindModel:
                Master_Sections master_Sections = new Master_Sections();
                master_Sections = db.Master_Sections
                    .Where(w => w.Section_Name.ToLower() == val.ToLower().Trim() &&
                    w.Department_Id == departmentId)
                    .FirstOrDefault();
                if (master_Sections != null)
                {
                    res = master_Sections.Section_Id;
                }
                else
                {
                    if (create)
                    {
                        if (!string.IsNullOrEmpty(val))
                        {
                            if (Section_Save(departmentId, val))
                            {
                                goto FindModel;
                            }
                        }
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Section_Save(Guid departmentId, string val)
        {
            try
            {
                bool res = new bool();
                Master_Sections master_Sections = new Master_Sections();
                master_Sections.Code = db.Master_Sections.Count() + 1;
                master_Sections.Department_Id = departmentId;
                master_Sections.Section_Name = val.Trim();
                db.Master_Sections.Add(master_Sections);
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

        public Master_Departments Department_Get(Guid id)
        {
            return db.Master_Departments.Find(id);
        }

        public List<Master_Departments> Department_GetAll()
        {
            return db.Master_Departments.ToList();
        }

        public List<clsDepartments> Department_GetAllView()
        {
            return db.Master_Departments
                .Select(s => new clsDepartments()
                {
                    Active = s.Active,
                    Create = s.Create,
                    Department_Name = s.Department_Name,
                    Division_Name = s.Master_Divisions.Division_Name,
                    Plant_Name = s.Master_Divisions.Master_Plants.Plant_Name,
                    Update = s.Update
                }).ToList();
        }

        public Guid? Department_GetId(Guid divisionId, string val, bool create = false)
        {
            try
            {
                Guid? res = null;
            FindModel:
                Master_Departments master_Departments = new Master_Departments();
                master_Departments = db.Master_Departments
                    .Where(w => w.Department_Name.ToLower() == val.ToLower().Trim() &&
                    w.Division_Id == divisionId)
                    .FirstOrDefault();
                if (master_Departments != null)
                {
                    res = master_Departments.Department_Id;
                }
                else
                {
                    if (create)
                    {
                        if (!string.IsNullOrEmpty(val))
                        {
                            if (Department_Save(divisionId, val))
                            {
                                goto FindModel;
                            }
                        }
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Department_Save(Guid divisionId, string val)
        {
            try
            {
                bool res = new bool();
                Master_Departments master_Departments = new Master_Departments();
                master_Departments.Code = db.Master_Departments.Count() + 1;
                master_Departments.Department_Name = val.Trim();
                master_Departments.Division_Id = divisionId;
                db.Master_Departments.Add(master_Departments);
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

        public Master_Divisions Division_Get(Guid id)
        {
            return db.Master_Divisions.Find(id);
        }

        public List<Master_Divisions> Division_GetAll()
        {
            return db.Master_Divisions.ToList();
        }

        public List<clsDivisions> Division_GetAllView()
        {
            return db.Master_Divisions
                .Select(s => new clsDivisions()
                {
                    Division_Id = s.Division_Id,
                    Active = s.Active,
                    Create = s.Create,
                    Division_Name = s.Division_Name,
                    Plant_Name = s.Master_Plants.Plant_Name,
                    Update = s.Update
                }).ToList();
        }

        public Guid? Division_GetId(Guid plantId, string val, bool create = false)
        {
            try
            {
                Guid? res = null;
            FindModel:
                Master_Divisions master_Divisions = new Master_Divisions();
                master_Divisions = db.Master_Divisions
                    .Where(w => w.Division_Name.ToLower() == val.ToLower().Trim() &&
                    w.Plant_Id == plantId)
                    .FirstOrDefault();
                if (master_Divisions != null)
                {
                    res = master_Divisions.Division_Id;
                }
                else
                {
                    if (create)
                    {
                        if (!string.IsNullOrEmpty(val))
                        {
                            if (Division_Save_GetId(plantId, val))
                            {
                                goto FindModel;
                            }
                        }
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool Division_Save_GetId(Guid plantId, string val)
        {
            try
            {
                bool res = new bool();
                Master_Divisions master_Divisions = new Master_Divisions();
                master_Divisions.Code = db.Master_Divisions.Count() + 1;
                master_Divisions.Division_Name = val.Trim();
                master_Divisions.Plant_Id = plantId;
                db.Master_Divisions.Add(master_Divisions);
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
        public bool Division_Save(Master_Divisions model)
        {
            try
            {
                bool res = new bool();
                Master_Divisions master_Divisions = new Master_Divisions();
                master_Divisions = db.Master_Divisions.Where(w => w.Division_Id == model.Division_Id).FirstOrDefault();

                if (master_Divisions != null)
                {
                    master_Divisions = db.Master_Divisions.Where(w => w.Division_Id != model.Division_Id && w.Division_Name.ToLower() == model.Division_Name.ToLower().Trim() && w.Plant_Id == model.Plant_Id).FirstOrDefault();
                    if (master_Divisions != null)
                    {
                        return false;
                    }
                    else
                    {
                        res = Division_Update(model);
                    }

                }
                else
                {
                    master_Divisions = db.Master_Divisions.Where(w => w.Division_Name.ToLower() == model.Division_Name.ToLower().Trim() && w.Plant_Id == model.Plant_Id).FirstOrDefault();

                    if (master_Divisions != null)
                    {
                        return false;
                    }
                    else
                    {
                        res = Division_Insert(model);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected bool Division_Insert(Master_Divisions model)
        {
            try
            {
                bool res = new bool();
                Master_Divisions master_Divisions = new Master_Divisions();
                master_Divisions.Division_Name = model.Division_Name;
                master_Divisions.Active = model.Active;
                master_Divisions.Plant_Id = model.Plant_Id;

                var query = db.Master_Plants.FirstOrDefault();

                if (query == null)
                {
                    master_Divisions.Code = 1;
                }
                else
                {
                    master_Divisions.Code = db.Master_Plants.Max(m => m.Code) + 1;
                }

                db.Master_Divisions.Add(master_Divisions);
                if (db.SaveChanges() > 0)
                {
                    res = true;
                }

                return res;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        protected bool Division_Update(Master_Divisions model)
        {
            try
            {
                bool res = new bool();
                Master_Divisions master_Divisions = new Master_Divisions();
                master_Divisions = db.Master_Divisions.Where(w => w.Division_Id == model.Division_Id).FirstOrDefault();

                master_Divisions.Division_Name = model.Division_Name.Trim();
                master_Divisions.Active = model.Active;
                master_Divisions.Update = DateTime.Now;

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

        public clsSaveResult Division_Delete(Guid id)
        {
            clsSaveResult res = new clsSaveResult();
            try
            {
                Master_Divisions master_Divisions = new Master_Divisions();
                master_Divisions = db.Master_Divisions.Where(w => w.Division_Id == id).FirstOrDefault();

                int userCount = db.Users.Where(w => w.Division_Id == id).Count();
                int deptCount = db.Master_Departments.Where(w => w.Division_Id == id).Count();

                if (userCount > 0 || deptCount > 0)
                {
                    res.Message = "ข้อมูลถูกใช้งานอยู่";
                    res.CanSave = false;
                }
                else
                {
                    db.Master_Divisions.Remove(master_Divisions);
                    if (db.SaveChanges() > 0)
                    {
                        res.CanSave = true;
                    }
                }
                return res;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Master_Grades Grades_Get(Guid id)
        {
            return db.Master_Grades.Find(id);
        }

        public List<Master_Grades> Grades_GetAll()
        {
            return db.Master_Grades.ToList();
        }

        public List<clsGrades> Grades_GetAllView()
        {
            return db.Master_Grades
                .Select(s => new clsGrades()
                {
                    Active = s.Active,
                    Create = s.Create,
                    Grade_Name = s.Grade_Name,
                    Grade_Position = s.Grade_Position,
                    LineWork_Name = s.Master_LineWorks.LineWork_Name,
                    Update = s.Update,
                    Grade_Id = s.Grade_Id
                }).ToList();
        }

        public Guid? Grade_GetId(Guid lineWorkId, string grade, string position, bool create = false)
        {
            try
            {
                Guid? res = null;

            FindModel:
                Master_Grades master_Grades = new Master_Grades();
                master_Grades = db.Master_Grades
                    .Where(w => w.Grade_Name.ToLower() == grade.ToLower().Trim() &&
                    w.Grade_Position.ToLower() == position.ToLower().Trim() &&
                    w.LineWork_Id == lineWorkId)
                    .FirstOrDefault();
                if (master_Grades != null)
                {
                    res = master_Grades.Grade_Id;
                }
                else
                {
                    if (create)
                    {
                        if (!string.IsNullOrEmpty(grade) && !string.IsNullOrEmpty(position))
                        {
                            master_Grades = new Master_Grades();
                            master_Grades.Grade_Name = grade;
                            master_Grades.Grade_Position = position;
                            master_Grades.LineWork_Id = lineWorkId;

                            if (Grade_Save(master_Grades))
                            {
                                goto FindModel;
                            }
                        }
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Grade_Save(Master_Grades model)
        {
            try
            {
                bool res = new bool();
                Master_Grades master_Grades = new Master_Grades();
                master_Grades = db.Master_Grades.Where(w => w.Grade_Id == model.Grade_Id).FirstOrDefault();

                if (master_Grades != null)
                {
                    master_Grades = db.Master_Grades.Where(w => w.Grade_Id != model.Grade_Id && w.Grade_Name.ToLower() == model.Grade_Name.ToLower().Trim() && w.Grade_Position.ToLower() == model.Grade_Position.ToLower().Trim() && w.LineWork_Id == model.LineWork_Id).FirstOrDefault();
                    if (master_Grades != null)
                    {
                        return false;
                    }
                    else
                    {
                        res = Grade_Update(model);
                    }

                }
                else
                {
                    master_Grades = db.Master_Grades.Where(w => w.Grade_Name.ToLower() == model.Grade_Name.ToLower().Trim() && w.Grade_Position.ToLower() == model.Grade_Position.Trim().ToLower() && w.LineWork_Id == model.LineWork_Id).FirstOrDefault();

                    if (master_Grades != null)
                    {
                        return false;
                    }
                    else
                    {
                        res = Grade_Insert(model);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected bool Grade_Insert(Master_Grades model)
        {
            try
            {
                bool res = new bool();
                Master_Grades master_Grades = new Master_Grades();
                master_Grades.LineWork_Id = model.LineWork_Id;
                master_Grades.Grade_Name = model.Grade_Name.Trim();
                master_Grades.Grade_Position = model.Grade_Position.Trim();
                master_Grades.Active = model.Active;

                var query = db.Master_Grades.FirstOrDefault();

                if (query == null)
                {
                    master_Grades.Code = 1;
                }
                else
                {
                    master_Grades.Code = db.Master_Grades.Max(m => m.Code) + 1;
                }
                
                db.Master_Grades.Add(master_Grades);
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

        protected bool Grade_Update(Master_Grades model)
        {
            try
            {
                bool res = new bool();
                Master_Grades master_Grades = new Master_Grades();
                master_Grades = db.Master_Grades.Where(w => w.Grade_Id == model.Grade_Id).FirstOrDefault();

                master_Grades.LineWork_Id = model.LineWork_Id;
                master_Grades.Grade_Name = model.Grade_Name.Trim();
                master_Grades.Grade_Position = model.Grade_Position.Trim();
                master_Grades.Active = model.Active;
                master_Grades.Update = DateTime.Now;

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

        public clsSaveResult Grades_Delete(Guid id)
        {
            clsSaveResult res = new clsSaveResult();
            try
            {
                Master_Grades master_Grades = new Master_Grades();
                master_Grades = db.Master_Grades.Where(w => w.Grade_Id == id).FirstOrDefault();

                int userCount = db.Users.Where(w => w.Grade_Id == id).Count();

                if (userCount > 0)
                {
                    res.Message = "ข้อมูลถูกใช้งานอยู่";
                    res.CanSave = false;
                }
                else
                {
                    db.Master_Grades.Remove(master_Grades);
                    if (db.SaveChanges() > 0)
                    {
                        res.CanSave = true;
                    }
                }
                return res;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Master_LineWorks LineWorks_Get(Guid id)
        {
            return db.Master_LineWorks.Find(id);
        }

        public List<Master_LineWorks> LineWorks_GetAll()
        {
            return db.Master_LineWorks.ToList();
        }

        public Guid? LineWork_GetId(string val, bool create = false)
        {
            try
            {
                Guid? res = null;
            FindModel:
                Master_LineWorks master_LineWorks = new Master_LineWorks();
                master_LineWorks = db.Master_LineWorks
                    .Where(w => w.LineWork_Name.ToLower() == val.ToLower().Trim())
                    .FirstOrDefault();
                if (master_LineWorks != null)
                {
                    res = master_LineWorks.LineWork_Id;
                }
                else
                {
                    if (create)
                    {
                        if (!string.IsNullOrEmpty(val))
                        {
                            master_LineWorks = new Master_LineWorks();
                            master_LineWorks.LineWork_Name = val;
                            if (LineWork_Save(master_LineWorks))
                            {
                                goto FindModel;
                            }
                        }
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool LineWork_Save(Master_LineWorks model)
        {
            try
            {
                bool res = new bool();
                Master_LineWorks master_LineWorks = new Master_LineWorks();
                master_LineWorks = db.Master_LineWorks.Where(w => w.LineWork_Id == model.LineWork_Id).FirstOrDefault();

                if (master_LineWorks != null)
                {
                    master_LineWorks = db.Master_LineWorks.Where(w => w.LineWork_Id != model.LineWork_Id && w.LineWork_Name.ToLower() == model.LineWork_Name.ToLower().Trim() && w.Authorize_Id == model.Authorize_Id).FirstOrDefault();
                    if (master_LineWorks != null)
                    {
                        return false;
                    }
                    else
                    {
                        res = LineWork_Update(model);
                    }

                }
                else
                {
                    master_LineWorks = db.Master_LineWorks.Where(w => w.LineWork_Name.ToLower() == model.LineWork_Name.ToLower().Trim() && w.Authorize_Id == model.Authorize_Id).FirstOrDefault();

                    if (master_LineWorks != null)
                    {
                        return false;
                    }
                    else
                    {
                        res = LineWork_Insert(model);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected bool LineWork_Insert(Master_LineWorks model)
        {
            try
            {
                bool res = new bool();
                Master_LineWorks master_LineWorks = new Master_LineWorks();
                master_LineWorks.Code = db.Master_LineWorks.Count() + 1;
                master_LineWorks.LineWork_Name = model.LineWork_Name.Trim();
                if (model.Authorize_Id.HasValue)
                {
                    master_LineWorks.Authorize_Id = model.Authorize_Id;
                }
                db.Master_LineWorks.Add(master_LineWorks);
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

        protected bool LineWork_Update(Master_LineWorks model)
        {
            try
            {
                bool res = new bool();
                Master_LineWorks master_LineWorks = new Master_LineWorks();
                master_LineWorks = db.Master_LineWorks
                    .Where(w => w.LineWork_Id == model.LineWork_Id)
                    .FirstOrDefault();
                master_LineWorks.Active = model.Active;
                master_LineWorks.Authorize_Id = model.Authorize_Id;
                master_LineWorks.LineWork_Name = model.LineWork_Name;
                master_LineWorks.Update = DateTime.Now;
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

        public clsSaveResult Lineworks_Delete(Guid id)
        {
            clsSaveResult res = new clsSaveResult();
            try
            {
                int gradeCount = db.Master_Grades.Where(w => w.Grade_Id == id).Count();
                int userCount = db.Users.Where(w => w.Grade_Id == id).Count();
                if (gradeCount > 0 && userCount > 0)
                {
                    res.Message = "ข้อมูลถูกใช้งานอยู่";
                    res.CanSave = false;
                }
                else
                {
                    
                    Master_LineWorks master_LineWorks = new Master_LineWorks();

                    master_LineWorks = db.Master_LineWorks
                            .Where(w => w.LineWork_Id == id).FirstOrDefault();
                    db.Master_LineWorks.Remove(master_LineWorks);
                    if (db.SaveChanges() > 0)
                    {
                        res.CanSave = true;
                    }
                }
                return res;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Master_Plants Plant_Get(Guid id)
        {
            return db.Master_Plants.Find(id);
        }

        public List<Master_Plants> Plant_GetAll()
        {
            return db.Master_Plants.ToList();
        }

        public Guid? Plant_GetId(string val, bool create = false)
        {
            try
            {
                Guid? res = null;
            FindModel:
                Master_Plants master_Plants = new Master_Plants();
                master_Plants = db.Master_Plants
                    .Where(w => w.Plant_Name.ToLower() == val.ToLower().Trim())
                    .FirstOrDefault();
                if (master_Plants != null)
                {
                    res = master_Plants.Plant_Id;
                }
                else
                {
                    if (create)
                    {
                        if (!string.IsNullOrEmpty(val))
                        {
                            if (Plant_Save_GetID(val))
                            {
                                goto FindModel;
                            }
                        }
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Plant_Save_GetID(string val)
        {
            try
            {
                bool res = new bool();
                Master_Plants master_Plants = new Master_Plants();
                master_Plants.Code = db.Master_Plants.Count() + 1;
                master_Plants.Plant_Name = val.Trim();
                db.Master_Plants.Add(master_Plants);
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

        public bool Plant_Save(Master_Plants model)
        {
            try
            {
                bool res = new bool();
                Master_Plants master_Plants = new Master_Plants();
                master_Plants = db.Master_Plants.Where(w => w.Plant_Id == model.Plant_Id).FirstOrDefault();

                if (master_Plants != null)
                {
                    master_Plants = db.Master_Plants.Where(w => w.Plant_Id != model.Plant_Id && w.Plant_Name.ToLower() == model.Plant_Name.ToLower().Trim()).FirstOrDefault();
                    if (master_Plants != null)
                    {
                        return false;
                    }
                    else
                    {
                        res = Plant_Update(model);
                    }
                  
                }
                else
                {
                    master_Plants = db.Master_Plants.Where(w => w.Plant_Name.ToLower() == model.Plant_Name.ToLower().Trim()).FirstOrDefault();

                    if (master_Plants != null)
                    {
                        return false;
                    }
                    else
                    {
                        res = Plant_Insert(model);
                    }
                }
 
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected bool Plant_Insert(Master_Plants model)
        {
            try
            {
                bool res = new bool();
                Master_Plants master_Plants = new Master_Plants();
                master_Plants.Plant_Name = model.Plant_Name;
                master_Plants.Active = model.Active;

                var query = db.Master_Plants.FirstOrDefault();

                if (query == null)
                {
                    master_Plants.Code = 1;
                }
                else
                {
                    master_Plants.Code = db.Master_Plants.Max(m => m.Code) + 1;
                }

                db.Master_Plants.Add(master_Plants);
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

        protected bool Plant_Update(Master_Plants model)
        {
            try
            {
                bool res = new bool();
                Master_Plants master_Plants = new Master_Plants();
                master_Plants = db.Master_Plants.Where(w => w.Plant_Id == model.Plant_Id).FirstOrDefault();

                master_Plants.Plant_Name = model.Plant_Name.Trim();
                master_Plants.Active = model.Active;
                master_Plants.Update = DateTime.Now;

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

        public clsSaveResult Plants_Delete(Guid id)
        {
            clsSaveResult res = new clsSaveResult();
            try
            {
                Master_Plants master_Plants = new Master_Plants();
                master_Plants = db.Master_Plants.Where(w => w.Plant_Id == id).FirstOrDefault();

                int divisionCount = db.Master_Divisions.Where(w => w.Plant_Id == id).Count();
                int userCount = db.Users.Where(w => w.Plant_Id == id).Count();

                if (userCount > 0 || divisionCount > 0)
                {
                    res.Message = "ข้อมูลถูกใช้งานอยู่";
                    res.CanSave = false;
                }
                else
                {
                    db.Master_Plants.Remove(master_Plants);
                    if (db.SaveChanges() > 0)
                    {
                        res.CanSave = true;
                    }
                }
                return res;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Users User_Get(Guid id)
        {
            return db.Users.Find(id);
        }

        public List<Users> User_GetAll()
        {
            return db.Users.ToList();
        }

        public List<clsUsers> User_GetAllView()
        {
            return db.Users
                .Select(s => new clsUsers()
                {
                    User_Id = s.User_Id,
                    Active = s.Active,
                    Create = s.Create,
                    Department_Name = s.Master_Departments.Department_Name,
                    Division_Name = s.Master_Divisions.Division_Name,
                    Grade_Name = s.Master_Grades.Grade_Name,
                    Grade_Position = s.Master_Grades.Grade_Position,
                    LineWork_Name = s.Master_LineWorks.LineWork_Name,
                    Plant_Name = s.Master_Plants.Plant_Name,
                    Process_Name = s.Master_Processes.Process_Name,
                    Section_Name = s.Master_Sections.Section_Name,
                    Update = s.Update,
                    User_Code = s.User_Code,
                    User_Email = s.User_Email
                }).ToList();
        }

        public bool User_Save(UserDetails model)
        {
            try
            {
                bool res = new bool();
                if (db.Users.Where(w => w.User_Code == model.Users.User_Code.Trim()).FirstOrDefault() == null)
                {
                    res = User_Insert(model);
                }
                else
                {
                    res = User_Update(model);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public UserDetails UserDetail_Get(Guid id)
        {
            return db.UserDetails.Where(w => w.User_Id == id).FirstOrDefault();
        }
        public clsSaveResult User_Delete(Guid id)
        {
            clsSaveResult res = new clsSaveResult();
            try
            {
                db.UserDetails.Remove(db.UserDetails.FirstOrDefault(f => f.User_Id == id));
                db.Users.Remove(db.Users.Find(id));
                if (db.SaveChanges() > 0)
                {
                    res.CanSave = true;
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var item in ex.EntityValidationErrors)
                {
                    foreach (var item2 in item.ValidationErrors)
                    {
                        if (string.IsNullOrEmpty(res.Message))
                        {
                            res.Message = item2.ErrorMessage;
                        }
                        else
                        {
                            res.Message += "\n" + item2.ErrorMessage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                if (ex.InnerException != null)
                {
                    res.Message = ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        res.Message = ex.InnerException.InnerException.Message;
                    }
                }
            }

            return res;
        }
        protected bool User_Insert(UserDetails model)
        {
            try
            {
                bool res = new bool();

                Users users = new Users();
                users.User_Code = model.Users.User_Code.Trim();
                users.LineWork_Id = model.Users.LineWork_Id;
                users.Grade_Id = model.Users.Grade_Id;
                users.Plant_Id = model.Users.Plant_Id;
                users.Division_Id = model.Users.Division_Id;
                users.Department_Id = model.Users.Department_Id;
                users.Section_Id = model.Users.Section_Id;
                users.Process_Id = model.Users.Process_Id;
                users.Role_Id = model.Users.Role_Id;

                if (!string.IsNullOrEmpty(model.Users.User_Email))
                {
                    users.User_Email = model.Users.User_Email.Trim();
                }
                else
                {
                    users.User_Email = GetEmailAD(model.Users.User_Code);
                }

                db.Users.Add(users);

                UserDetails userDetails = new UserDetails();
                userDetails.Detail_EN_FirstName = model.Detail_EN_FirstName.Trim();
                userDetails.Detail_EN_LastName = model.Detail_EN_LastName.Trim();
                userDetails.Prefix_EN_Id = model.Prefix_EN_Id;

                if (!string.IsNullOrEmpty(model.Detail_Password))
                {
                    userDetails.Detail_Password = User_Password(model.Detail_Password.Trim());
                    userDetails.Detail_ConfirmPassword = userDetails.Detail_Password;
                }

                if (!string.IsNullOrEmpty(model.Detail_Tel))
                {
                    userDetails.Detail_Tel = model.Detail_Tel.Trim();
                }

                userDetails.Detail_TH_FirstName = model.Detail_TH_FirstName.Trim();
                userDetails.Detail_TH_LastName = model.Detail_TH_LastName.Trim();
                userDetails.Prefix_TH_Id = model.Prefix_TH_Id;
                userDetails.User_Id = users.User_Id;
                db.UserDetails.Add(userDetails);

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

        public string User_Password(string val)
        {
            try
            {
                StringBuilder res = new StringBuilder();
                if (!string.IsNullOrEmpty(val))
                {
                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    byte[] vs;
                    UTF8Encoding encode = new UTF8Encoding();

                    vs = md5.ComputeHash(encode.GetBytes(val));

                    for (int i = 0; i < vs.Length; i++)
                    {
                        res.Append(vs[i].ToString("X2"));
                    }
                }
                return res.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected bool User_Update(UserDetails model)
        {
            try
            {
                bool res = new bool();

                Users users = db.Users.Where(w => w.User_Code == model.Users.User_Code).FirstOrDefault();
                users.Department_Id = model.Users.Department_Id;
                users.Division_Id = model.Users.Division_Id;
                users.Grade_Id = model.Users.Grade_Id;
                users.Plant_Id = model.Users.Plant_Id;
                users.Process_Id = model.Users.Process_Id;
                users.Role_Id = model.Users.Role_Id;
                users.Section_Id = model.Users.Section_Id;
                users.User_Code = model.Users.User_Code.Trim();

                if (!string.IsNullOrEmpty(model.Users.User_Email))
                {
                    users.User_Email = model.Users.User_Email.Trim();
                }

                users.Update = DateTime.Now;
                users.Active = model.Users.Active;

                UserDetails userDetails = db.UserDetails.Where(w => w.User_Id == users.User_Id).FirstOrDefault();
                userDetails.Detail_EN_FirstName = model.Detail_EN_FirstName.Trim();
                userDetails.Detail_EN_LastName = model.Detail_EN_LastName.Trim();
                userDetails.Prefix_EN_Id = model.Prefix_EN_Id;

                if (!string.IsNullOrEmpty(model.Detail_Tel))
                {
                    userDetails.Detail_Tel = model.Detail_Tel.Trim();
                }

                userDetails.Detail_TH_FirstName = model.Detail_TH_FirstName.Trim();
                userDetails.Detail_TH_LastName = model.Detail_TH_LastName.Trim();
                userDetails.Prefix_TH_Id = model.Prefix_TH_Id;
                userDetails.Detail_ConfirmPassword = userDetails.Detail_Password;
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

        private string GetEmailAD(string code)
        {
            try
            {
                string res = string.Empty;
                string domainName = ConfigurationManager.AppSettings["DomainName"];
                using (var context = new PrincipalContext(ContextType.Domain, domainName))
                {
                    UserPrincipal user = new UserPrincipal(context);
                    user.Description = code.Trim();

                    PrincipalSearcher searcher = new PrincipalSearcher();
                    searcher.QueryFilter = user;
                    Principal principal = searcher.FindOne();
                    if (principal != null)
                    {
                        DirectoryEntry entry = principal.GetUnderlyingObject() as DirectoryEntry;
                        res = entry.Properties["mail"].Value.ToString();
                    }
                }

                return res;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<SelectListItem> SelectListItems_Role()
        {
            return db.System_Roles
                .Select(s => new SelectListItem()
                {
                    Value = s.Role_Id.ToString(),
                    Text = s.Role_Name
                }).OrderBy(o => o.Text).ToList();
        }
        public List<SelectListItem> SelectListItems_LineWork()
        {
            return db.Master_LineWorks
                .Where(w => w.Active)
                .Select(s => new SelectListItem()
                {
                    Value = s.LineWork_Id.ToString(),
                    Text = s.LineWork_Name
                }).OrderBy(o => o.Text).ToList();
        }
        public List<SelectListItem> SelectListItems_Grade(Guid? lineworkId)
        {
            IQueryable<Master_Grades> query = db.Master_Grades
                .Where(w => w.Active);
            if (lineworkId != null)
            {
                query = query
                    .Where(w => w.LineWork_Id == lineworkId.Value);
            }

            return query
                .Select(s => new SelectListItem()
                {
                    Value = s.Grade_Id.ToString(),
                    Text = s.Grade_Name + " (" + s.Grade_Position + ")"
                }).OrderBy(o => o.Text).ToList();
        }

        public List<SelectListItem> SelectListItems_Plant()
        {
            return db.Master_Plants
                .Where(w => w.Active)
                .Select(s => new SelectListItem()
                {
                    Value = s.Plant_Id.ToString(),
                    Text = s.Plant_Name
                }).OrderBy(o => o.Text).ToList();
        }

        public List<SelectListItem> SelectListItems_Division(Guid? plantId)
        {
            IQueryable<Master_Divisions> query = db.Master_Divisions
                .Where(w => w.Active);
            if (plantId != null)
            {
                query = query
                    .Where(w => w.Plant_Id == plantId.Value);
            }

            return query
                .Select(s => new SelectListItem()
                {
                    Value = s.Division_Id.ToString(),
                    Text = s.Division_Name
                }).OrderBy(o => o.Text).ToList();
        }

        public List<SelectListItem> SelectListItems_Department(Guid? divisionId)
        {
            IQueryable<Master_Departments> query = db.Master_Departments
                .Where(w => w.Active);
            if (divisionId != null)
            {
                query = query
                    .Where(w => w.Division_Id == divisionId.Value);
            }

            return query
                .Select(s => new SelectListItem()
                {
                    Value = s.Department_Id.ToString(),
                    Text = s.Department_Name
                }).OrderBy(o => o.Text).ToList();
        }
        public List<SelectListItem> SelectListItems_Section(Guid? departmentId)
        {
            IQueryable<Master_Sections> query = db.Master_Sections
                .Where(w => w.Active);
            if (departmentId != null)
            {
                query = query
                    .Where(w => w.Department_Id == departmentId.Value);
            }

            return query
                .Select(s => new SelectListItem()
                {
                    Value = s.Section_Id.ToString(),
                    Text = s.Section_Name
                }).OrderBy(o => o.Text).ToList();
        }

        public List<SelectListItem> SelectListItems_Process(Guid? sectionId)
        {
            IQueryable<Master_Processes> query = db.Master_Processes
                .Where(w => w.Active);
            if (sectionId != null)
            {
                query = query
                    .Where(w => w.Section_Id == sectionId.Value);
            }

            return query
                .Select(s => new SelectListItem()
                {
                    Value = s.Process_Id.ToString(),
                    Text = s.Process_Name
                }).OrderBy(o => o.Text).ToList();
        }

        public List<SelectListItem> SelectListItems_PrefixTH()
        {
            return db.System_Prefix_THs
                .Select(s => new SelectListItem()
                {
                    Value = s.Prefix_TH_Id.ToString(),
                    Text = s.Prefix_TH_Name
                }).OrderBy(o => o.Text).ToList();
        }

        public List<SelectListItem> SelectListItems_PrefixEN()
        {
            return db.System_Prefix_ENs
                .Select(s => new SelectListItem()
                {
                    Value = s.Prefix_EN_Id.ToString(),
                    Text = s.Prefix_EN_Name
                }).OrderBy(o => o.Text).ToList();
        }
    }
}