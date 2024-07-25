using E2E.Models.Tables;
using E2E.Models.Views;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace E2E.Models
{
    public class ClsManageMaster
    {
        private readonly ClsContext db = new ClsContext();

        protected bool Department_Insert(Master_Departments model)
        {
            try
            {
                bool res = new bool();
                Master_Departments master_Departments = new Master_Departments
                {
                    Department_Name = model.Department_Name,
                    Active = model.Active,
                    Division_Id = model.Division_Id
                };

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

        protected bool Department_Update(Master_Departments model)
        {
            try
            {
                bool res = new bool();
                if (!model.Active)
                {
                    int DepartmentCount = db.Users.Where(w => w.Master_Processes.Master_Sections.Master_Departments.Department_Id == model.Department_Id).Count();

                    if (DepartmentCount > 0)
                    {
                        return res;
                    }
                }

                Master_Departments master_Departments = new Master_Departments();
                master_Departments = db.Master_Departments.Where(w => w.Department_Id == model.Department_Id).FirstOrDefault();

                master_Departments.Department_Name = model.Department_Name.Trim();
                master_Departments.Active = model.Active;
                master_Departments.Update = DateTime.Now;

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

        protected bool Division_Insert(Master_Divisions model)
        {
            try
            {
                bool res = new bool();
                Master_Divisions master_Divisions = new Master_Divisions
                {
                    Division_Name = model.Division_Name,
                    Active = model.Active
                };

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

        protected bool Division_Update(Master_Divisions model)
        {
            try
            {
                bool res = new bool();
                if (!model.Active)
                {
                    int DivisionCount = db.Users.Where(w => w.Master_Processes.Master_Sections.Master_Departments.Division_Id == model.Division_Id).Count();

                    if (DivisionCount > 0)
                    {
                        return res;
                    }
                }

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

        protected bool Plant_Insert(PlantDetail model)
        {
            try
            {
                bool res = new bool();
                Master_Plants master_Plants = db.Master_Plants.Find(model.Plant_Id);
                if (master_Plants == null)
                {
                    master_Plants = new Master_Plants
                    {
                        Plant_Name = model.Master_Plants.Plant_Name,
                        Active = model.Master_Plants.Active
                    };

                    db.Master_Plants.Add(master_Plants);
                }
                else
                {
                    master_Plants.Plant_Name = model.Master_Plants.Plant_Name;
                    master_Plants.Update = DateTime.Now;
                    master_Plants.Active = model.Master_Plants.Active;
                }

                PlantDetail plantDetail = db.PlantDetails.Find(model.PlantDetail_Id);
                if (plantDetail == null)
                {
                    plantDetail = new PlantDetail
                    {
                        OfficeAddress1 = model.OfficeAddress1,
                        OfficeAddress2 = model.OfficeAddress2,
                        OfficeFax = model.OfficeFax,
                        OfficeName = model.OfficeName,
                        OfficeNumber = model.OfficeNumber,
                        Plant_Id = master_Plants.Plant_Id
                    };

                    db.PlantDetails.Add(plantDetail);
                }
                else
                {
                    plantDetail.OfficeAddress1 = model.OfficeAddress1;
                    plantDetail.OfficeAddress2 = model.OfficeAddress2;
                    plantDetail.OfficeFax = model.OfficeFax;
                    plantDetail.OfficeName = model.OfficeName;
                    plantDetail.OfficeNumber = model.OfficeNumber;
                }

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

        protected bool Plant_Update(PlantDetail model)
        {
            try
            {
                bool res = new bool();

                Master_Plants master_Plants = db.Master_Plants.Where(w => w.Plant_Id == model.Plant_Id).FirstOrDefault();
                master_Plants.Plant_Name = model.Master_Plants.Plant_Name.Trim();
                master_Plants.Active = model.Master_Plants.Active;
                master_Plants.Update = DateTime.Now;

                PlantDetail plantDetail = db.PlantDetails.Where(w => w.PlantDetail_Id == model.PlantDetail_Id).FirstOrDefault();
                plantDetail.OfficeAddress1 = model.OfficeAddress1;
                plantDetail.OfficeAddress2 = model.OfficeAddress2;
                plantDetail.OfficeFax = model.OfficeFax;
                plantDetail.OfficeName = model.OfficeName;
                plantDetail.OfficeNumber = model.OfficeNumber;

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

        protected bool Process_Insert(Master_Processes model)
        {
            try
            {
                bool res = new bool();
                Master_Processes master_Processes = new Master_Processes
                {
                    Process_Name = model.Process_Name,
                    Active = model.Active,
                    Section_Id = model.Section_Id
                };

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

        protected bool Process_Update(Master_Processes model)
        {
            try
            {
                bool res = new bool();
                if (!model.Active)
                {
                    int ProcessCount = db.Users.Where(w => w.Process_Id == model.Process_Id).Count();

                    if (ProcessCount > 0)
                    {
                        return res;
                    }
                }

                Master_Processes master_Processes = new Master_Processes();
                master_Processes = db.Master_Processes.Where(w => w.Process_Id == model.Process_Id).FirstOrDefault();

                master_Processes.Process_Name = model.Process_Name.Trim();
                master_Processes.Section_Id = model.Section_Id;
                master_Processes.Active = model.Active;
                master_Processes.Update = DateTime.Now;

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

        protected bool Section_Insert(Master_Sections model)
        {
            try
            {
                bool res = new bool();
                Master_Sections master_Sections = new Master_Sections
                {
                    Section_Name = model.Section_Name,
                    Active = model.Active,
                    Department_Id = model.Department_Id
                };

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

        protected bool Section_Update(Master_Sections model)
        {
            try
            {
                bool res = new bool();
                if (!model.Active)
                {
                    int SectionCount = db.Users.Where(w => w.Master_Processes.Master_Sections.Section_Id == model.Section_Id).Count();

                    if (SectionCount > 0)
                    {
                        return res;
                    }
                }

                Master_Sections master_Sections = new Master_Sections();
                master_Sections = db.Master_Sections.Where(w => w.Section_Id == model.Section_Id).FirstOrDefault();

                master_Sections.Section_Name = model.Section_Name.Trim();
                master_Sections.Department_Id = model.Department_Id;
                master_Sections.Active = model.Active;
                master_Sections.Update = DateTime.Now;

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

        protected bool Users_Insert(UserDetails model)
        {
            // Create new Users instance and populate its properties
            Users users = new Users
            {
                User_Code = model.Users.User_Code.Trim(),
                Grade_Id = model.Users.Grade_Id,
                Plant_Id = model.Users.Plant_Id,
                Process_Id = model.Users.Process_Id,
                Role_Id = 2,
                User_CostCenter = model.Users.User_CostCenter.Trim(),
                YearSetPoint = DateTime.Now.Year,
                BusinessCardGroup = model.Users.BusinessCardGroup,
                User_Point = db.System_Configurations
                    .OrderByDescending(o => o.CreateDateTime)
                    .Select(s => s.Configuration_Point)
                    .FirstOrDefault()
            };

            // Fetch Active Directory info
            ClsActiveDirectoryInfo adInfo = GetAdInfo(model.Users.User_Code);
            if (adInfo != null)
            {
                users.Username = adInfo.SamAccountName;
                users.User_Email = adInfo.EmailAddress;
            }
            else
            {
                users.User_Email = model.Users.User_Email ?? "";
            }

            // Add user to the database
            db.Users.Add(users);

            // Create and populate UserDetails instance
            UserDetails userDetails = new UserDetails
            {
                Detail_EN_FirstName = model.Detail_EN_FirstName.Trim(),
                Detail_EN_LastName = model.Detail_EN_LastName.Trim(),
                Prefix_EN_Id = model.Prefix_EN_Id,
                Detail_TH_FirstName = model.Detail_TH_FirstName.Trim(),
                Detail_TH_LastName = model.Detail_TH_LastName.Trim(),
                Prefix_TH_Id = model.Prefix_TH_Id,
                User_Id = users.User_Id
            };

            // Set password if email address is empty
            if (adInfo == null)
            {
                string password = string.IsNullOrEmpty(model.Detail_Password)
                    ? users.User_Code.Trim()
                    : model.Detail_Password.Trim();
                string hashPassword = Users_Password(password);
                userDetails.Detail_Password = hashPassword;
                userDetails.Detail_ConfirmPassword = hashPassword;
            }

            // Add user details to the database
            db.UserDetails.Add(userDetails);

            return db.SaveChanges() > 0;
        }

        protected bool Users_Update(UserDetails model)
        {
            Users users = db.Users.Where(w => w.User_Code == model.Users.User_Code).FirstOrDefault();
            if (model.Users.Role_Id != 0)
            {
                users.Role_Id = model.Users.Role_Id;
            }
            users.Grade_Id = model.Users.Grade_Id;
            users.Plant_Id = model.Users.Plant_Id;
            users.Process_Id = model.Users.Process_Id;
            users.User_Code = model.Users.User_Code.Trim();
            users.User_CostCenter = model.Users.User_CostCenter.Trim();
            users.BusinessCardGroup = model.Users.BusinessCardGroup;

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

            userDetails.Detail_TH_FirstName = model.Detail_TH_FirstName.Trim();
            userDetails.Detail_TH_LastName = model.Detail_TH_LastName.Trim();
            userDetails.Prefix_TH_Id = model.Prefix_TH_Id;

            ClsActiveDirectoryInfo adInfo = GetAdInfo(model.Users.User_Code);
            if (adInfo != null)
            {
                users.Username = adInfo.SamAccountName;
                users.User_Email = adInfo.EmailAddress;
                userDetails.Detail_Password = null;
                userDetails.Detail_ConfirmPassword = null;
            }

            return db.SaveChanges() > 0;
        }

        public ClsSaveResult Categories_Delete(Guid id)
        {
            ClsSaveResult res = new ClsSaveResult();
            try
            {
                Master_Categories master_Categories = new Master_Categories();
                master_Categories = db.Master_Categories.Where(w => w.Category_Id == id).FirstOrDefault();

                int Count = db.Topics.Where(w => w.Category_Id == id).Count();

                if (Count > 0)
                {
                    res.Message = "ข้อมูลถูกใช้งานอยู่";
                    res.IsSuccess = false;
                }
                else
                {
                    db.Master_Categories.Remove(master_Categories);
                    if (db.SaveChanges() > 0)
                    {
                        res.IsSuccess = true;
                    }
                }
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Categories_Insert(Master_Categories model)
        {
            try
            {
                bool res = new bool();
                Master_Categories master_Categories = new Master_Categories
                {
                    Category_Name = model.Category_Name
                };

                db.Master_Categories.Add(master_Categories);

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

        public bool Categories_Save(Master_Categories model)
        {
            try
            {
                bool res = new bool();
                var master_Categories = db.Master_Categories.Find(model.Category_Id);

                if (master_Categories != null)
                {
                    res = Categories_Update(model);
                }
                else
                {
                    res = Categories_Insert(model);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Categories_Update(Master_Categories model)
        {
            bool res = new bool();
            var master_Categories = db.Master_Categories.Find(model.Category_Id);
            master_Categories.Category_Name = model.Category_Name;
            master_Categories.Active = model.Active;
            master_Categories.Update = DateTime.Now;

            if (db.SaveChanges() > 0)
            {
                res = true;
            }
            return res;
        }

        public List<ClsUsers> ClsUsers_GetAllView()
        {
            try
            {
                return db.UserDetails
                .Select(s => new ClsUsers()
                {
                    User_Id = s.Users.User_Id,
                    Active = s.Users.Active,
                    BusinessCardGroup = s.Users.BusinessCardGroup,
                    Create = s.Users.Create,
                    Department_Name = s.Users.Master_Processes.Master_Sections.Master_Departments.Department_Name,
                    Division_Name = s.Users.Master_Processes.Master_Sections.Master_Departments.Master_Divisions.Division_Name,
                    Grade_Name = s.Users.Master_Grades.Grade_Name,
                    Grade_Position = s.Users.Master_Grades.Grade_Position,
                    LineWork_Name = s.Users.Master_Grades.Master_LineWorks.LineWork_Name,
                    Plant_Name = s.Users.Master_Plants.Plant_Name,
                    Process_Name = s.Users.Master_Processes.Process_Name,
                    Section_Name = s.Users.Master_Processes.Master_Sections.Section_Name,
                    Update = s.Users.Update,
                    User_Code = s.Users.User_Code,
                    User_Email = s.Users.User_Email,
                    User_CostCenter = s.Users.User_CostCenter,
                    User_Point = s.Users.User_Point,
                    User_Name_EN = s.Detail_EN_FirstName + " " + s.Detail_EN_LastName,
                    User_Name_TH = s.Detail_TH_FirstName + " " + s.Detail_TH_LastName,
                    Role = s.Users.System_Roles.Role_Name,
                    Username = s.Users.Username
                }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ClsUsers ClsUsers_GetView(string val)
        {
            try
            {
                return db.UserDetails
                    .Where(w => w.Users.User_Code == val)
                .Select(s => new ClsUsers()
                {
                    User_Id = s.Users.User_Id,
                    Active = s.Users.Active,
                    Create = s.Users.Create,
                    Department_Name = s.Users.Master_Processes.Master_Sections.Master_Departments.Department_Name,
                    Division_Name = s.Users.Master_Processes.Master_Sections.Master_Departments.Master_Divisions.Division_Name,
                    Grade_Name = s.Users.Master_Grades.Grade_Name,
                    Grade_Position = s.Users.Master_Grades.Grade_Position,
                    LineWork_Name = s.Users.Master_Grades.Master_LineWorks.LineWork_Name,
                    Plant_Name = s.Users.Master_Plants.Plant_Name,
                    Process_Name = s.Users.Master_Processes.Process_Name,
                    Section_Name = s.Users.Master_Processes.Master_Sections.Section_Name,
                    Update = s.Users.Update,
                    User_Code = s.Users.User_Code,
                    User_Email = s.Users.User_Email,
                    User_CostCenter = s.Users.User_CostCenter,
                    User_Point = s.Users.User_Point,
                    User_Name_EN = s.Detail_EN_FirstName + " " + s.Detail_EN_LastName,
                    User_Name_TH = s.Detail_TH_FirstName + " " + s.Detail_TH_LastName,
                }).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ClsUsers ClsUsers_GetView(Guid id)
        {
            try
            {
                return db.UserDetails
                    .Where(w => w.User_Id == id)
                .Select(s => new ClsUsers()
                {
                    User_Id = s.Users.User_Id,
                    Active = s.Users.Active,
                    Create = s.Users.Create,
                    Department_Name = s.Users.Master_Processes.Master_Sections.Master_Departments.Department_Name,
                    Division_Name = s.Users.Master_Processes.Master_Sections.Master_Departments.Master_Divisions.Division_Name,
                    Grade_Name = s.Users.Master_Grades.Grade_Name,
                    Grade_Position = s.Users.Master_Grades.Grade_Position,
                    LineWork_Name = s.Users.Master_Grades.Master_LineWorks.LineWork_Name,
                    Plant_Name = s.Users.Master_Plants.Plant_Name,
                    Process_Name = s.Users.Master_Processes.Process_Name,
                    Section_Name = s.Users.Master_Processes.Master_Sections.Section_Name,
                    Update = s.Users.Update,
                    User_Code = s.Users.User_Code,
                    User_Email = s.Users.User_Email,
                    User_CostCenter = s.Users.User_CostCenter,
                    User_Point = s.Users.User_Point,
                    User_Name_EN = s.Detail_EN_FirstName + " " + s.Detail_EN_LastName,
                    User_Name_TH = s.Detail_TH_FirstName + " " + s.Detail_TH_LastName,
                }).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Master_Departments> Department_GetAll()
        {
            return db.Master_Departments
                .OrderBy(o => o.Master_Divisions.Division_Name)
                .ThenBy(t => t.Department_Name)
                .ToList();
        }

        public Guid Department_GetId(Guid divisionId, string val)
        {
            Master_Departments master_Departments = db.Master_Departments
                    .Where(w => w.Department_Name.ToLower() == val.ToLower().Trim() &&
                    w.Division_Id == divisionId)
                    .FirstOrDefault();
            if (master_Departments == null)
            {
                master_Departments = new Master_Departments()
                {
                    Department_Name = val,
                    Division_Id = divisionId
                };
                db.Master_Departments.Add(master_Departments);

            }
            else
            {
                master_Departments.Active = true;
            }
            db.SaveChanges();

            return master_Departments.Department_Id;
        }

        public List<Master_Divisions> Division_GetAll()
        {
            return db.Master_Divisions
                .OrderBy(o=>o.Division_Name)
                .ToList();
        }

        public Guid Division_GetId(string val)
        {
            Master_Divisions master_Divisions = db.Master_Divisions
                    .Where(w => w.Division_Name.ToLower() == val.ToLower().Trim())
                    .FirstOrDefault();
            if (master_Divisions == null)
            {
                master_Divisions = new Master_Divisions()
                {
                    Division_Name = val
                };
                db.Master_Divisions.Add(master_Divisions);
                
            }
            else
            {
                master_Divisions.Active = true;
            }
            db.SaveChanges();

            return master_Divisions.Division_Id;
        }

        public ClsActiveDirectoryInfo GetAdInfo(string code)
        {
            try
            {
                ClsActiveDirectoryInfo res = new ClsActiveDirectoryInfo();
                string domainName = ConfigurationManager.AppSettings["DomainName"];
                using (var context = new PrincipalContext(ContextType.Domain, domainName))
                {
                    UserPrincipal user = new UserPrincipal(context)
                    {
                        Description = code.Trim()
                    };

                    PrincipalSearcher searcher = new PrincipalSearcher
                    {
                        QueryFilter = user
                    };
                    Principal principal = searcher.FindOne();
                    if (principal != null)
                    {
                        if (principal is UserPrincipal userPrincipal)
                        {
                            res = new ClsActiveDirectoryInfo()
                            {
                                Description = userPrincipal.Description,
                                DisplayName = userPrincipal.DisplayName,
                                DistinguishedName = userPrincipal.DistinguishedName,
                                Guid = userPrincipal.Guid,
                                Name = userPrincipal.Name,
                                SamAccountName = userPrincipal.SamAccountName,
                                Sid = userPrincipal.Sid,
                                StructuralObjectClass = userPrincipal.StructuralObjectClass,
                                EmailAddress = userPrincipal.EmailAddress,
                            };
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

        public string GetDepartmentNameForUser(Guid userId)
        {
            return db.Users
                .Where(user => user.User_Id == userId)
                .Select(user => user.Master_Processes.Master_Sections.Master_Departments.Department_Name)
                .FirstOrDefault() ?? string.Empty;
        }

        public string GetEmailAD(string code)
        {
            try
            {
                string res = string.Empty;
                string domainName = ConfigurationManager.AppSettings["DomainName"];
                using (var context = new PrincipalContext(ContextType.Domain, domainName))
                {
                    UserPrincipal user = new UserPrincipal(context)
                    {
                        Description = code.Trim()
                    };

                    PrincipalSearcher searcher = new PrincipalSearcher
                    {
                        QueryFilter = user
                    };
                    Principal principal = searcher.FindOne();
                    if (principal != null)
                    {
                        res = principal.UserPrincipalName;
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetUsernameAD(string code)
        {
            try
            {
                try
                {
                    string res = string.Empty;
                    string domainName = ConfigurationManager.AppSettings["DomainName"];
                    using (var context = new PrincipalContext(ContextType.Domain, domainName))
                    {
                        UserPrincipal user = new UserPrincipal(context)
                        {
                            Description = code.Trim()
                        };

                        PrincipalSearcher searcher = new PrincipalSearcher
                        {
                            QueryFilter = user
                        };
                        Principal principal = searcher.FindOne();
                        if (principal != null)
                        {
                            res = principal.SamAccountName;
                        }
                    }

                    return res;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Guid Grade_GetId(Guid lineWorkId, string grade, string position)
        {
            Master_Grades master_Grades = db.Master_Grades
                    .Where(w => w.Grade_Name.ToLower() == grade.ToLower().Trim() &&
                    w.Grade_Position.ToLower() == position.ToLower().Trim() &&
                    w.LineWork_Id == lineWorkId)
                    .FirstOrDefault();
            if (master_Grades == null)
            {
                master_Grades = new Master_Grades
                {
                    Grade_Name = grade,
                    Grade_Position = position,
                    LineWork_Id = lineWorkId
                };
                db.Master_Grades.Add(master_Grades);
            }
            else
            {
                master_Grades.Active = true;
            }
            db.SaveChanges();

            return master_Grades.Grade_Id;
        }

        public List<Master_Grades> Grades_GetAll()
        {
            return db.Master_Grades
                .OrderBy(o => o.Master_LineWorks.LineWork_Name)
                .ThenBy(t => t.Grade_Name)
                .ToList();
        }

        public bool HaveAD(string username)
        {
            try
            {
                bool res = new bool();

                string domainName = ConfigurationManager.AppSettings["DomainName"];
                using (var context = new PrincipalContext(ContextType.Domain, domainName))
                {
                    UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username);
                    if (user != null)
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

        public ClsSaveResult InquiryTopic_Delete(Guid id)
        {
            ClsSaveResult res = new ClsSaveResult();
            try
            {
                Master_InquiryTopics master_InquiryTopics = new Master_InquiryTopics();
                master_InquiryTopics = db.Master_InquiryTopics.Find(id);

                int satCount = db.SatisfactionDetails
                    .Where(w => w.InquiryTopic_Id == master_InquiryTopics.InquiryTopic_Id)
                    .Count();

                if (satCount > 0)
                {
                    res.Message = "ข้อมูลถูกใช้งานอยู่";
                    res.IsSuccess = false;
                }
                else
                {
                    db.Entry(master_InquiryTopics).State = EntityState.Deleted;
                    if (db.SaveChanges() > 0)
                    {
                        res.IsSuccess = true;
                    }
                }
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool InquiryTopic_Insert(Master_InquiryTopics model)
        {
            try
            {
                bool res = new bool();
                db.Entry(model).State = EntityState.Added;
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

        public bool InquiryTopic_Save(Master_InquiryTopics model)
        {
            try
            {
                bool res = new bool();
                Master_InquiryTopics master_InquiryTopics = new Master_InquiryTopics();
                master_InquiryTopics = db.Master_InquiryTopics.Find(model.InquiryTopic_Id);
                if (master_InquiryTopics != null)
                {
                    model.Create = master_InquiryTopics.Create;
                    res = InquiryTopic_Update(model);
                }
                else
                {
                    res = InquiryTopic_Insert(model);
                }
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool InquiryTopic_Update(Master_InquiryTopics model)
        {
            try
            {
                bool res = new bool();
                var InquiryTopic = db.Master_InquiryTopics.Where(w => w.InquiryTopic_Id == model.InquiryTopic_Id).FirstOrDefault();
                InquiryTopic.Update = DateTime.Now;
                InquiryTopic.InquiryTopic_Index = model.InquiryTopic_Index;
                InquiryTopic.Program_Id = model.Program_Id;
                InquiryTopic.Description_TH = model.Description_TH;
                InquiryTopic.Description_EN = model.Description_EN;

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

        public List<Master_InquiryTopics> InquiryTopics_GetAll()
        {
            return db.Master_InquiryTopics
                .OrderBy(o => o.System_Program.Program_Name)
                .ThenBy(o => o.InquiryTopic_Index)
                .ToList();
        }

        public bool IsAdmin()
        {
            try
            {
                Guid userid = Guid.Parse(HttpContext.Current.User.Identity.Name);
                switch (db.Users.Find(userid).Role_Id)
                {
                    case 1:
                        return true;

                    default:
                        return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Guid LineWork_GetId(string val)
        {
            val = val.Trim();

            // Check if the line work already exists
            var master_LineWorks = db.Master_LineWorks
                .FirstOrDefault(w => w.LineWork_Name.ToLower() == val.ToLower());

            if (master_LineWorks == null)
            {
                master_LineWorks = new Master_LineWorks
                {
                    LineWork_Name = val,
                    Authorize_Id = val.StartsWith("J") ? 1 : (val.StartsWith("M") ? 2 : 3)
                };

                db.Master_LineWorks.Add(master_LineWorks);
            }
            else
            {
                master_LineWorks.Active = true;
            }
            db.SaveChanges();

            return master_LineWorks.LineWork_Id;
        }

        public List<Master_LineWorks> LineWorks_GetAll()
        {
            return db.Master_LineWorks
                .OrderBy(o => o.Authorize_Id)
                .ThenBy(t => t.LineWork_Name)
                .ToList();
        }

        public bool LoginDomain(string username, string password)
        {
            try
            {
                bool res = new bool();
                string domainName = ConfigurationManager.AppSettings["DomainName"];
                using (var context = new PrincipalContext(ContextType.Domain, domainName))
                {
                    if (!context.ValidateCredentials(username, password))
                    {
                        using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                        {
                            dynamic select = searcher.FindAll().Where(w => w.SamAccountName == username).FirstOrDefault();

                            if (select.BadLogonCount == 5)
                            {
                                throw new Exception(string.Format("Account is currently locked out\n Please contact IT"));
                            }

                            throw new Exception(string.Format("Invalid Password {0}/5", select.BadLogonCount));
                        }
                    }
                    else
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

        public PlantDetail Plant_Get(Guid id)
        {
            return db.PlantDetails.Where(w => w.Master_Plants.Plant_Id == id).FirstOrDefault();
        }

        public IEnumerable<Master_Plants> Plant_GetAll()
        {
            return db.Master_Plants.ToList();
        }

        public Guid Plant_GetId(string val)
        {
            Master_Plants master_Plants = db.Master_Plants
                    .Where(w => w.Plant_Name.ToLower() == val.ToLower().Trim())
                    .FirstOrDefault();
            if (master_Plants == null)
            {
                master_Plants = new Master_Plants()
                {
                    Plant_Name = val
                };

                db.Master_Plants.Add(master_Plants);
            }
            else
            {
                master_Plants.Active = true;
            }
            db.SaveChanges();
            return master_Plants.Plant_Id;
        }

        public bool Plant_Save(PlantDetail model)
        {
            try
            {
                bool res = new bool();
                PlantDetail plantDetail = db.PlantDetails.Where(w => w.PlantDetail_Id == model.PlantDetail_Id).FirstOrDefault();

                var masterPlants = db.Master_Plants.Where(w => w.Plant_Id != model.Plant_Id && w.Plant_Name.ToLower() == model.Master_Plants.Plant_Name.ToLower().Trim()).FirstOrDefault();

                if (plantDetail != null)
                {
                    if (masterPlants != null)
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
                    if (masterPlants != null && plantDetail == null)
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

        public int Prefix_EN_GetId(string val)
        {
            // Check if the value exists
            var system_Prefix_ = db.System_Prefix_ENs
                        .Where(w => w.Prefix_EN_Name == val)
                        .FirstOrDefault();

            // If the value doesn't exist and create is true
            if (system_Prefix_ == null)
            {
                system_Prefix_ = new System_Prefix_EN
                {
                    Prefix_EN_Name = val
                };

                db.System_Prefix_ENs.Add(system_Prefix_);
                db.SaveChanges();
            }

            return system_Prefix_.Prefix_EN_Id;
        }

        public int Prefix_TH_GetId(string val)
        {
            // Check if the value exists
            var system_Prefix_ = db.System_Prefix_THs
                        .Where(w => w.Prefix_TH_Name == val)
                        .FirstOrDefault();

            // If the value doesn't exist and create is true
            if (system_Prefix_ == null)
            {
                system_Prefix_ = new System_Prefix_TH
                {
                    Prefix_TH_Name = val
                };

                db.System_Prefix_THs.Add(system_Prefix_);
                db.SaveChanges();
            }

            return system_Prefix_.Prefix_TH_Id;
        }

        public ClsSaveResult Process_Delete(Guid id)
        {
            ClsSaveResult res = new ClsSaveResult();
            try
            {
                Master_Processes master_Processes = new Master_Processes();
                master_Processes = db.Master_Processes.Where(w => w.Process_Id == id).FirstOrDefault();

                int userCount = db.Users.Where(w => w.Process_Id == id).Count();

                if (userCount > 0)
                {
                    res.Message = "ข้อมูลถูกใช้งานอยู่";
                    res.IsSuccess = false;
                }
                else
                {
                    db.Master_Processes.Remove(master_Processes);
                    if (db.SaveChanges() > 0)
                    {
                        res.IsSuccess = true;
                    }
                }
                return res;
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
            return db.Master_Processes
                .OrderBy(o => o.Master_Sections.Master_Departments.Master_Divisions.Division_Name)
                .ThenBy(t => t.Master_Sections.Master_Departments.Department_Name)
                .ThenBy(t => t.Master_Sections.Section_Name)
                .ThenBy(t => t.Process_Name)
                .ToList();
        }

        public Guid Process_GetId(Guid sectionId, string val)
        {
            Master_Processes master_Processes = db.Master_Processes
                    .Where(w => w.Process_Name.ToLower() == val.ToLower().Trim() &&
                    w.Section_Id == sectionId)
                    .FirstOrDefault();
            if (master_Processes == null)
            {
                master_Processes = new Master_Processes()
                {
                    Process_Name = val,
                    Section_Id = sectionId
                };
                db.Master_Processes.Add(master_Processes);
            }
            else
            {
                master_Processes.Active = true;
            }
            db.SaveChanges();
            return master_Processes.Process_Id;
        }

        public bool Process_Save(Master_Processes model)
        {
            try
            {
                bool res = new bool();
                Master_Processes master_Processes = new Master_Processes();
                master_Processes = db.Master_Processes.Where(w => w.Process_Id == model.Process_Id).FirstOrDefault();

                if (master_Processes != null)
                {
                    master_Processes = db.Master_Processes.Where(w => w.Process_Id != model.Process_Id && w.Process_Name.ToLower() == model.Process_Name.ToLower().Trim() && w.Section_Id == model.Section_Id).FirstOrDefault();
                    if (master_Processes != null)
                    {
                        return false;
                    }
                    else
                    {
                        res = Process_Update(model);
                    }
                }
                else
                {
                    master_Processes = db.Master_Processes.Where(w => w.Process_Name.ToLower() == model.Process_Name.ToLower().Trim() && w.Section_Id == model.Section_Id).FirstOrDefault();

                    if (master_Processes != null)
                    {
                        return false;
                    }
                    else
                    {
                        res = Process_Insert(model);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SaveChangePassword(ClsPassword model)
        {
            try
            {
                bool res = new bool();
                var UserDetails = db.UserDetails.Where(w => w.User_Id == model.User_Id).FirstOrDefault();
                if (UserDetails.Detail_Password == Users_Password(model.OldPassword))
                {
                    UserDetails.Detail_Password = Users_Password(model.NewPassword);
                    UserDetails.Detail_ConfirmPassword = UserDetails.Detail_Password;
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

        public ClsSaveResult Section_Delete(Guid id)
        {
            ClsSaveResult res = new ClsSaveResult();
            try
            {
                Master_Sections master_Sections = new Master_Sections();
                master_Sections = db.Master_Sections.Where(w => w.Section_Id == id).FirstOrDefault();

                int userCount = db.Users.Where(w => w.Master_Processes.Master_Sections.Section_Id == id).Count();
                int ProcessesCount = db.Master_Processes.Where(w => w.Section_Id == id).Count();

                if (userCount > 0 || ProcessesCount > 0)
                {
                    res.Message = "ข้อมูลถูกใช้งานอยู่";
                    res.IsSuccess = false;
                }
                else
                {
                    db.Master_Sections.Remove(master_Sections);
                    if (db.SaveChanges() > 0)
                    {
                        res.IsSuccess = true;
                    }
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
            return db.Master_Sections
                .OrderBy(o => o.Master_Departments.Master_Divisions.Division_Name)
                .ThenBy(t => t.Master_Departments.Department_Name)
                .ThenBy(t => t.Section_Name)
                .ToList();
        }

        public Guid Section_GetId(Guid departmentId, string val)
        {
            Master_Sections master_Sections = db.Master_Sections
                     .Where(w => w.Section_Name.ToLower() == val.ToLower().Trim() &&
                     w.Department_Id == departmentId)
                     .FirstOrDefault();
            if (master_Sections == null)
            {
                master_Sections = new Master_Sections()
                {
                    Department_Id = departmentId,
                    Section_Name = val
                };
                db.Master_Sections.Add(master_Sections);
            }
            else
            {
                master_Sections.Active = true;
            }
            db.SaveChanges();

            return master_Sections.Section_Id;
        }

        public bool Section_Save(Master_Sections model)
        {
            try
            {
                bool res = new bool();
                Master_Sections master_Sections = new Master_Sections();
                master_Sections = db.Master_Sections.Where(w => w.Section_Id == model.Section_Id).FirstOrDefault();

                if (master_Sections != null)
                {
                    master_Sections = db.Master_Sections.Where(w => w.Section_Id != model.Section_Id && w.Section_Name.ToLower() == model.Section_Name.ToLower().Trim() && w.Department_Id == model.Department_Id).FirstOrDefault();
                    if (master_Sections != null)
                    {
                        return false;
                    }
                    else
                    {
                        res = Section_Update(model);
                    }
                }
                else
                {
                    master_Sections = db.Master_Sections.Where(w => w.Section_Name.ToLower() == model.Section_Name.ToLower().Trim() && w.Department_Id == model.Department_Id).FirstOrDefault();

                    if (master_Sections != null)
                    {
                        return false;
                    }
                    else
                    {
                        res = Section_Insert(model);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SelectListItem> SelectListItems_Authorize()
        {
            IQueryable<System_Authorize> query = db.System_Authorizes;

            List<SelectListItem> item = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Select Authorize", Value = "" }
            };
            item.AddRange(query
                .Select(s => new SelectListItem()
                {
                    Value = s.Authorize_Id.ToString(),
                    Text = s.Authorize_Name
                }).OrderBy(o => o.Text).ToList());

            return item;
        }

        public List<SelectListItem> SelectListItems_Department(Guid? divisionId)
        {
            List<SelectListItem> item = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Select Department", Value = "" }
            };

            if (divisionId.HasValue)
            {
                item.AddRange(db.Master_Departments
                .Where(w => w.Active &&
                w.Division_Id == divisionId.Value)
                .Select(s => new SelectListItem()
                {
                    Value = s.Department_Id.ToString(),
                    Text = s.Department_Name
                }).OrderBy(o => o.Text).ToList());
            }

            return item;
        }

        public List<SelectListItem> SelectListItems_Division()
        {
            List<SelectListItem> item = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Select Division", Value = "" }
            };

            item.AddRange(db.Master_Divisions
                .Where(w => w.Active)
                .Select(s => new SelectListItem()
                {
                    Value = s.Division_Id.ToString(),
                    Text = s.Division_Name
                }).OrderBy(o => o.Text).ToList());

            return item;
        }

        public List<SelectListItem> SelectListItems_Grade(Guid? lineworkId)
        {
            List<SelectListItem> item = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Select Grade", Value = "" }
            };

            if (lineworkId.HasValue)
            {
                item.AddRange(db.Master_Grades
                .Where(w => w.Active &&
                w.LineWork_Id == lineworkId.Value)
                .Select(s => new SelectListItem()
                {
                    Value = s.Grade_Id.ToString(),
                    Text = s.Grade_Name + " (" + s.Grade_Position + ")"
                }).OrderBy(o => o.Text).ToList());
            }

            return item;
        }

        public List<SelectListItem> SelectListItems_LineWork()
        {
            IQueryable<Master_LineWorks> query = db.Master_LineWorks
                .Where(w => w.Active);

            List<SelectListItem> item = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Select Line of work", Value = "" }
            };
            item.AddRange(query
                .Select(s => new SelectListItem()
                {
                    Value = s.LineWork_Id.ToString(),
                    Text = s.LineWork_Name
                }).OrderBy(o => o.Text).ToList());

            return item;
        }

        public List<SelectListItem> SelectListItems_Plant()
        {
            IQueryable<Master_Plants> query = db.Master_Plants
                .Where(w => w.Active);

            List<SelectListItem> item = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Select Plant", Value = "" }
            };
            item.AddRange(query
                .Select(s => new SelectListItem()
                {
                    Value = s.Plant_Id.ToString(),
                    Text = s.Plant_Name
                }).OrderBy(o => o.Text).ToList());

            return item;
        }

        public List<SelectListItem> SelectListItems_PrefixEN()
        {
            IQueryable<System_Prefix_EN> query = db.System_Prefix_ENs;

            List<SelectListItem> item = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Select Prefix", Value = "" }
            };
            item.AddRange(query
                .Select(s => new SelectListItem()
                {
                    Value = s.Prefix_EN_Id.ToString(),
                    Text = s.Prefix_EN_Name
                }).OrderBy(o => o.Text).ToList());

            return item;
        }

        public List<SelectListItem> SelectListItems_PrefixTH()
        {
            IQueryable<System_Prefix_TH> query = db.System_Prefix_THs;

            List<SelectListItem> item = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Select Prefix", Value = "" }
            };
            item.AddRange(query
                .Select(s => new SelectListItem()
                {
                    Value = s.Prefix_TH_Id.ToString(),
                    Text = s.Prefix_TH_Name
                }).OrderBy(o => o.Text).ToList());

            return item;
        }

        public List<SelectListItem> SelectListItems_Process(Guid? sectionId)
        {
            List<SelectListItem> item = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Select Process", Value = "" }
            };

            if (sectionId.HasValue)
            {
                item.AddRange(db.Master_Processes
                 .Where(w => w.Active &&
                 w.Section_Id == sectionId.Value)
                 .Select(s => new SelectListItem()
                 {
                     Value = s.Process_Id.ToString(),
                     Text = s.Process_Name
                 }).OrderBy(o => o.Text).ToList());
            }

            return item;
        }

        public List<SelectListItem> SelectListItems_Role()
        {
            IQueryable<System_Roles> query = db.System_Roles;

            List<SelectListItem> item = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Select Role", Value = "" }
            };
            item.AddRange(query
                .Select(s => new SelectListItem()
                {
                    Value = s.Role_Id.ToString(),
                    Text = s.Role_Name
                }).OrderBy(o => o.Text).ToList());

            return item;
        }

        public List<SelectListItem> SelectListItems_Section(Guid? departmentId)
        {
            List<SelectListItem> item = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Select Section", Value = "" }
            };

            if (departmentId.HasValue)
            {
                item.AddRange(db.Master_Sections
                .Where(w => w.Active &&
                w.Department_Id == departmentId.Value)
                .Select(s => new SelectListItem()
                {
                    Value = s.Section_Id.ToString(),
                    Text = s.Section_Name
                }).OrderBy(o => o.Text).ToList());
            }

            return item;
        }

        public List<SelectListItem> SelectListItems_Users(Guid? processId)
        {
            IQueryable<Users> query = db.Users
                .Where(w => w.Active)
                .OrderBy(o => o.Master_Grades.Grade_Name)
                .ThenBy(t => t.User_Code);
            if (processId.HasValue)
            {
                query = query
                    .Where(w => w.Process_Id == processId);
            }

            List<SelectListItem> item = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Select User", Value = "" }
            };

            item.AddRange(query
                .Select(s => new SelectListItem()
                {
                    Text = s.User_Code + " [" + s.Master_Grades.Grade_Name + "][" + db.UserDetails.Where(w => w.User_Id == s.User_Id).Select(s2 => s2.Detail_EN_FirstName).FirstOrDefault() + "]",
                    Value = s.User_Id.ToString()
                }).ToList());

            return item;
        }

        public UserDetails UserDetails_Get(Guid id)
        {
            try
            {
                return db.UserDetails.Where(w => w.User_Id == id).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public UserDetails UserDetails_Get(string val)
        {
            try
            {
                return db.UserDetails
                    .Where(w => w.Users.User_Code == val).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Users_AdjustMissing(List<string> userCodeList)
        {
            List<Users> users = await db.Users
                    .Where(w => !userCodeList.Contains(w.User_Code))
                    .ToListAsync();
            if (users.Count > 0)
            {
                foreach (var item in users)
                {
                    if (User_CanDelete(item.User_Id))
                    {
                        db.Entry(item).State = EntityState.Deleted;
                    }
                    else
                    {
                        item.Active = false;
                        item.Update = DateTime.Now;
                        db.Entry(item).State = EntityState.Modified;
                    }
                }

                if (await db.SaveChangesAsync() > 0)
                {
                    return true;
                }
            }


            return true;
        }

        private bool User_CanDelete(Guid id)
        {
            if (db.Log_Logins.Any(a => a.User_Id == id) || db.Log_DbChanges.Any(a => a.User_Id == id) || db.Log_DbDeletes.Any(a => a.User_Id == id))
            {
                return false;
            }

            return true;
        }

        public ClsSaveResult Users_Delete(Guid id)
        {
            ClsSaveResult res = new ClsSaveResult();
            try
            {
                db.UserDetails.Remove(db.UserDetails.FirstOrDefault(f => f.User_Id == id));
                db.Users.Remove(db.Users.Find(id));
                if (db.SaveChanges() > 0)
                {
                    res.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.GetBaseException().Message;
            }

            return res;
        }

        public Users Users_Get(Guid id)
        {
            try
            {
                return db.Users.Find(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Users Users_Get(string val)
        {
            try
            {
                return db.Users.Where(w => w.User_Code == val).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Users> Users_GetAll()
        {
            try
            {
                return db.Users.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string Users_GetInfomation(Guid id)
        {
            try
            {
                return db.UserDetails
                    .Where(w => w.User_Id == id)
                    .Select(s => new { Data = s.Users.User_Code + " [" + s.Detail_EN_FirstName + " " + s.Detail_EN_LastName + "]" })
                    .Select(s => s.Data)
                    .FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string Users_Password(string val)
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

        public async Task<List<string>> Users_ReadFile(Stream stream)
        {
            List<string> userCodeList = new List<string>();
            List<Guid> lineWorkList = new List<Guid>();
            List<Guid> gradeList = new List<Guid>();
            List<Guid> plantList = new List<Guid>();
            List<Guid> divisionList = new List<Guid>();
            List<Guid> departmentList = new List<Guid>();
            List<Guid> sectionList = new List<Guid>();
            List<Guid> processList = new List<Guid>();

            Dictionary<string, int> prefixEnCache = new Dictionary<string, int>();
            Dictionary<string, int> prefixThCache = new Dictionary<string, int>();
            Dictionary<string, Guid> lineWorkCache = new Dictionary<string, Guid>();
            Dictionary<string, Guid> gradeCache = new Dictionary<string, Guid>();
            Dictionary<string, Guid> plantCache = new Dictionary<string, Guid>();
            Dictionary<string, Guid> divisionCache = new Dictionary<string, Guid>();
            Dictionary<string, Guid> departmentCache = new Dictionary<string, Guid>();
            Dictionary<string, Guid> sectionCache = new Dictionary<string, Guid>();
            Dictionary<string, Guid> processCache = new Dictionary<string, Guid>();

            List<UserDetails> userDetailsList = new List<UserDetails>();

            using (ExcelPackage package = new ExcelPackage(stream))
            {
                foreach (var sheet in package.Workbook.Worksheets)
                {
                    for (int row = 1; row <= sheet.Dimension.End.Row; row++)
                    {
                        var recNo = sheet.Cells[row, 1].Text;
                        if (int.TryParse(recNo, out int startData))
                        {
                            if (string.IsNullOrEmpty(sheet.Cells[row, 1].Text))
                            {
                                throw new Exception("Data not found");
                            }

                            string prefixEnText = sheet.Cells[row, 3].Text;
                            if (!prefixEnCache.ContainsKey(prefixEnText))
                            {
                                prefixEnCache[prefixEnText] = Prefix_EN_GetId(prefixEnText);
                            }

                            string prefixThText = sheet.Cells[row, 6].Text;
                            if (!prefixThCache.ContainsKey(prefixThText))
                            {
                                prefixThCache[prefixThText] = Prefix_TH_GetId(prefixThText);
                            }

                            string lineWorkText = sheet.Cells[row, 10].Text;
                            if (!lineWorkCache.ContainsKey(lineWorkText))
                            {
                                lineWorkCache[lineWorkText] = LineWork_GetId(lineWorkText);
                            }
                            Guid lineworkId = lineWorkCache[lineWorkText];
                            lineWorkList.Add(lineworkId);

                            string gradeName = sheet.Cells[row, 11].Text;
                            string gradePosition = sheet.Cells[row, 12].Text;
                            string gradeKey = $"{lineworkId}-{gradeName}-{gradePosition}";
                            if (!gradeCache.ContainsKey(gradeKey))
                            {
                                gradeCache[gradeKey] = Grade_GetId(lineworkId, gradeName, gradePosition);
                            }
                            Guid gradeId = gradeCache[gradeKey];
                            gradeList.Add(gradeId);

                            string plantName = sheet.Cells[row, 13].Text;
                            if (!plantCache.ContainsKey(plantName))
                            {
                                plantCache[plantName] = Plant_GetId(plantName);
                            }
                            Guid plantId = plantCache[plantName];
                            plantList.Add(plantId);

                            string divisionName = sheet.Cells[row, 14].Text;
                            if (!divisionCache.ContainsKey(divisionName))
                            {
                                divisionCache[divisionName] = Division_GetId(divisionName);
                            }
                            Guid divisionId = divisionCache[divisionName];
                            divisionList.Add(divisionId);

                            string departmentName = sheet.Cells[row, 15].Text;
                            string departmentKey = $"{divisionId}-{departmentName}";
                            if (!departmentCache.ContainsKey(departmentKey))
                            {
                                departmentCache[departmentKey] = Department_GetId(divisionId, departmentName);
                            }
                            Guid departmentId = departmentCache[departmentKey];
                            departmentList.Add(departmentId);

                            string sectionName = sheet.Cells[row, 16].Text;
                            string sectionKey = $"{departmentId}-{sectionName}";
                            if (!sectionCache.ContainsKey(sectionKey))
                            {
                                sectionCache[sectionKey] = Section_GetId(departmentId, sectionName);
                            }
                            Guid sectionId = sectionCache[sectionKey];
                            sectionList.Add(sectionId);

                            string processName = sheet.Cells[row, 17].Text;
                            string processKey = $"{sectionId}-{processName}";
                            if (!processCache.ContainsKey(processKey))
                            {
                                processCache[processKey] = Process_GetId(sectionId, processName);
                            }
                            Guid processId = processCache[processKey];
                            processList.Add(processId);

                            UserDetails userDetails = new UserDetails
                            {
                                Detail_EN_FirstName = sheet.Cells[row, 4].Text,
                                Detail_EN_LastName = sheet.Cells[row, 5].Text,
                                Prefix_EN_Id = prefixEnCache[prefixEnText],
                                Detail_TH_FirstName = sheet.Cells[row, 7].Text,
                                Detail_TH_LastName = sheet.Cells[row, 8].Text,
                                Prefix_TH_Id = prefixThCache[prefixThText],
                                Users = new Users
                                {
                                    User_Code = sheet.Cells[row, 2].Text,
                                    User_CostCenter = sheet.Cells[row, 18].Text,
                                    Grade_Id = gradeId,
                                    Plant_Id = plantId,
                                    Process_Id = processId,
                                    BusinessCardGroup = sheet.Cells[row, 15].Text.Contains("General Affair")
                                }
                            };

                            userDetailsList.Add(userDetails);
                        }
                    }
                }
            }

            foreach (var userDetails in userDetailsList)
            {
                if (Users_Save(userDetails))
                {
                    userCodeList.Add(userDetails.Users.User_Code);
                }
            }

            await SetInactiveLinework(lineWorkList.Distinct().ToList());
            await SetInactiveGrade(gradeList.Distinct().ToList());
            await SetInactivePlant(plantList.Distinct().ToList());
            await SetInactiveDivision(divisionList.Distinct().ToList());
            await SetInactiveDepartment(departmentList.Distinct().ToList());
            await SetInactiveSection(sectionList.Distinct().ToList());
            await SetInactiveProcess(processList.Distinct().ToList());

            await db.SaveChangesAsync();
            return userCodeList;
        }

        private async Task SetInactiveLinework(List<Guid> guids)
        {
            var list = await db.Master_LineWorks
                .Where(w => !guids.Contains(w.LineWork_Id))
                .ToListAsync();

            list.ForEach(f => f.Active = false);
        }

        private async Task SetInactiveGrade(List<Guid> guids)
        {
            var list = await db.Master_Grades
                .Where(w => !guids.Contains(w.Grade_Id))
                .ToListAsync();

            list.ForEach(f => f.Active = false);
        }

        private async Task SetInactivePlant(List<Guid> guids)
        {
            var list = await db.Master_Plants
                .Where(w => !guids.Contains(w.Plant_Id))
                .ToListAsync();

            list.ForEach(f => f.Active = false); ;
        }

        private async Task SetInactiveDivision(List<Guid> guids)
        {
            var list = await db.Master_Divisions
                .Where(w => !guids.Contains(w.Division_Id))
                .ToListAsync();

            list.ForEach(f => f.Active = false);
        }

        private async Task SetInactiveDepartment(List<Guid> guids)
        {
            var list = await db.Master_Departments
                .Where(w => !guids.Contains(w.Department_Id))
                .ToListAsync();

            list.ForEach(f => f.Active = false);
        }

        private async Task SetInactiveSection(List<Guid> guids)
        {
            var list = await db.Master_Sections
                .Where(w => !guids.Contains(w.Section_Id))
                .ToListAsync();

            list.ForEach(f => f.Active = false);
        }

        private async Task SetInactiveProcess(List<Guid> guids)
        {
            var list = await db.Master_Processes
                   .Where(w => !guids.Contains(w.Process_Id))
                   .ToListAsync();

            list.ForEach(f => f.Active = false);
        }

        public bool Users_Save(UserDetails model)
        {
            model.Users.User_Code = model.Users.User_Code.Trim();
            if (db.Users.Any(a => a.User_Code == model.Users.User_Code))
            {
                return Users_Update(model);
            }
            else
            {
                return Users_Insert(model);
            }
        }

        public async Task<List<Guid>> GetManagementOfDepartment()
        {
            Guid loginId = Guid.Parse(HttpContext.Current.User.Identity.Name);
            Guid departmentId = await db.Users
                .Where(w => w.User_Id == loginId)
                .Select(s => s.Master_Processes.Master_Sections.Department_Id)
                .FirstOrDefaultAsync();

            return await db.Users
                .Where(w => w.Master_Processes.Master_Sections.Department_Id == departmentId && w.Master_Grades.Master_LineWorks.Authorize_Id == 2)
                .Select(s => s.User_Id)
                .ToListAsync();
        }

        public async Task<List<Guid>> GetManagementOfDepartment(Guid userId)
        {
            Guid departmentId = await db.Users
                .Where(w => w.User_Id == userId)
                .Select(s => s.Master_Processes.Master_Sections.Department_Id)
                .FirstOrDefaultAsync();

            return await db.Users
                .Where(w => w.Master_Processes.Master_Sections.Department_Id == departmentId && w.Master_Grades.Master_LineWorks.Authorize_Id == 2)
                .Select(s => s.User_Id)
                .ToListAsync();
        }
    }
}
