using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace E2E.Models
{
    public class clsManageData
    {
        private clsContext db = new clsContext();

        public Guid? Process_GetId(Guid sectionId,string val,bool create = false)
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
                            if (Process_Save(sectionId,val))
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

        public bool Process_Save(Guid sectionId,string val)
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
        public Guid? Section_GetId(Guid departmentId,string val,bool create = false)
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
                            if (Section_Save(departmentId,val))
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

        public bool Section_Save(Guid departmentId,string val)
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
                            if (Department_Save(divisionId,val))
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

        public bool Department_Save(Guid divisionId,string val)
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
                            if (Division_Save(plantId,val))
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

        public bool Division_Save(Guid plantId,string val)
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
                            if (Grade_Save(grade, position))
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

        public bool Grade_Save(string grade, string position)
        {
            try
            {
                bool res = new bool();
                Master_Grades master_Grades = new Master_Grades();
                master_Grades.Grade_Name = grade.Trim();
                master_Grades.Grade_Position = position.Trim();
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
                            if (LineWork_Save(val))
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

        public bool LineWork_Save(string val)
        {
            try
            {
                bool res = new bool();
                Master_LineWorks master_LineWorks = new Master_LineWorks();
                master_LineWorks.Code = db.Master_LineWorks.Count() + 1;
                master_LineWorks.LineWork_Name = val.Trim();
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
                            if (Plant_Save(val))
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

        public bool Plant_Save(string val)
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

        public Users User_Get(Guid id)
        {
            return db.Users.Find(id);
        }

        public List<Users> User_GetAll()
        {
            return db.Users.ToList();
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

        protected bool User_Insert(UserDetails model)
        {
            try
            {
                bool res = new bool();

                Users users = new Users();
                users.Department_Id = model.Users.Department_Id;
                users.Division_Id = model.Users.Division_Id;
                users.Grade_Id = model.Users.Grade_Id;
                users.Nationality_Id = model.Users.Nationality_Id;
                users.Plant_Id = model.Users.Plant_Id;
                users.Process_Id = model.Users.Process_Id;
                users.Role_Id = model.Users.Role_Id;
                users.Section_Id = model.Users.Section_Id;
                users.User_Code = model.Users.User_Code.Trim();

                if (!string.IsNullOrEmpty(model.Users.User_Email))
                {
                    users.User_Email = model.Users.User_Email.Trim();
                }

                db.Users.Add(users);

                UserDetails userDetails = new UserDetails();
                userDetails.Detail_EN_FirstName = model.Detail_EN_FirstName.Trim();
                userDetails.Detail_EN_LastName = model.Detail_EN_LastName.Trim();
                userDetails.Detail_EN_Prefix = model.Detail_EN_Prefix.Trim();

                if (!string.IsNullOrEmpty(model.Detail_Password))
                {
                    userDetails.Detail_Password = User_Password(model.Detail_Password.Trim());
                }

                if (!string.IsNullOrEmpty(model.Detail_Tel))
                {
                    userDetails.Detail_Tel = model.Detail_Tel.Trim();
                }

                userDetails.Detail_TH_FirstName = model.Detail_TH_FirstName.Trim();
                userDetails.Detail_TH_LastName = model.Detail_TH_LastName.Trim();
                userDetails.Detail_TH_Prefix = model.Detail_TH_Prefix.Trim();
                userDetails.User_Id = model.Users.User_Id;
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

        protected string User_Password(string val)
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
                users.Nationality_Id = model.Users.Nationality_Id;
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
                userDetails.Detail_EN_Prefix = model.Detail_EN_Prefix.Trim();

                if (!string.IsNullOrEmpty(model.Detail_Tel))
                {
                    userDetails.Detail_Tel = model.Detail_Tel.Trim();
                }

                userDetails.Detail_TH_FirstName = model.Detail_TH_FirstName.Trim();
                userDetails.Detail_TH_LastName = model.Detail_TH_LastName.Trim();
                userDetails.Detail_TH_Prefix = model.Detail_TH_Prefix.Trim();

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
    }
}
