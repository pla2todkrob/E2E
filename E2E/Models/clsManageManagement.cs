using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models
{
    public class clsManageManagement
    {
        private clsContext db = new clsContext();
        private clsServiceFTP ftp = new clsServiceFTP();

        protected bool Document_Insert(clsDocuments model, HttpFileCollectionBase files)
        {
            try
            {
                bool res = new bool();
                Master_Documents master_Documents = new Master_Documents();
                Master_DocumentVersions master_DocumentVersions = new Master_DocumentVersions();

                Guid userId = Guid.Parse(System.Web.HttpContext.Current.User.Identity.Name);

                master_Documents.Document_Name = model.Master_Documents.Document_Name;
                master_Documents.Active = true;
                master_Documents.User_Id = userId;

                db.Master_Documents.Add(master_Documents);

                if (files[0].ContentLength != 0)
                {
                    HttpPostedFileBase file = files[0];

                    var Deptname = db.Users.Where(w => w.User_Id == userId).FirstOrDefault();

                    string dir = "DocumentControl/" + Deptname.Master_Processes.Master_Sections.Master_Departments.Department_Name.Trim() + "/" + model.Master_Documents.Document_Name;

                    string[] extension = file.FileName.Split('.');

                    string FileName = model.Master_Documents.Document_Name;

                    int Count = db.Master_DocumentVersions.Where(w => w.Document_Id == model.Master_Documents.Document_Id).Count() + 1;

                    FileName = FileName + "_V" + Count + "." + extension[1];
                    //FileName = string.Concat("_", FileName);

                    var clsfile = ftp.Ftp_UploadFileToString(dir, file, FileName);

                    master_DocumentVersions.Document_Id = master_Documents.Document_Id;
                    master_DocumentVersions.DocumentVersion_Name = FileName;
                    master_DocumentVersions.DocumentVersion_Path = clsfile;
                    master_DocumentVersions.User_Id = userId;
                    master_DocumentVersions.DocumentVersion_Number = Count;

                    db.Master_DocumentVersions.Add(master_DocumentVersions);
                }

                if (db.SaveChanges() > 0)
                {
                    res = true;
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected bool Document_Update(clsDocuments model, HttpFileCollectionBase files)
        {
            try
            {
                bool res = new bool();
                Master_Documents master_Documents = new Master_Documents();
                Master_DocumentVersions master_DocumentVersions = new Master_DocumentVersions();

                Guid userId = Guid.Parse(System.Web.HttpContext.Current.User.Identity.Name);

                master_Documents = db.Master_Documents
                    .Where(w => w.Document_Id == model.Master_Documents.Document_Id)
                    .FirstOrDefault();

                master_Documents.Document_Name = model.Master_Documents.Document_Name;
                master_Documents.Active = model.Master_Documents.Active;
                master_Documents.Update = DateTime.Now;

                if (files[0].ContentLength != 0)
                {
                    HttpPostedFileBase file = files[0];

                    var Deptname = db.Users.Where(w => w.User_Id == userId).FirstOrDefault();

                    string dir = "DocumentControl/" + Deptname.Master_Processes.Master_Sections.Master_Departments.Department_Name.Trim() + "/" + model.Master_Documents.Document_Name;

                    string[] extension = file.FileName.Split('.');

                    string FileName = model.Master_Documents.Document_Name;

                    int Count = db.Master_DocumentVersions.Where(w => w.Document_Id == model.Master_Documents.Document_Id).Count() + 1;

                    FileName = FileName + "_V" + Count + "." + extension[1];

                    var clsfile = ftp.Ftp_UploadFileToString(dir, file, FileName);

                    master_DocumentVersions.Document_Id = master_Documents.Document_Id;
                    master_DocumentVersions.DocumentVersion_Name = FileName;
                    master_DocumentVersions.DocumentVersion_Path = clsfile;
                    master_DocumentVersions.User_Id = userId;

                    master_DocumentVersions.DocumentVersion_Number = Count;
                    db.Master_DocumentVersions.Add(master_DocumentVersions);
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

        public bool Delete_Document(Guid id)
        {
            bool res = new bool();

            var chk = db.Master_DocumentVersions.Where(w => w.Document_Id == id).ToList();

            if (chk.Count > 0)
            {
                return res;
            }
            else
            {
                var sql = db.Master_Documents.Find(id);
                db.Master_Documents.Remove(sql);
                db.SaveChanges();
                res = true;
            }

            return res;
        }

        public bool Document_Save(clsDocuments model, HttpFileCollectionBase files)
        {
            try
            {
                bool res = new bool();
                string status = string.Empty;
                Master_Documents master_Documents = new Master_Documents();
                master_Documents = db.Master_Documents.Where(w => w.Document_Id == model.Master_Documents.Document_Id).FirstOrDefault();

                if (master_Documents != null)
                {
                    res = Document_Update(model, files);
                }
                else
                {
                    res = Document_Insert(model, files);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool WorkRoot_Insert(clsWorkRoots model)
        {
            try
            {
                bool res = new bool();
                WorkRoots workRoots = new WorkRoots();
                workRoots.User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);
                workRoots.Section_Id = model.WorkRoots.Section_Id;
                workRoots.WorkRoot_Name = model.WorkRoots.WorkRoot_Name;
                db.Entry(workRoots).State = System.Data.Entity.EntityState.Added;
                if (db.SaveChanges() > 0)
                {
                    if (model.Document_Id.Count > 0)
                    {
                        foreach (var item in model.Document_Id)
                        {
                            WorkRootDocuments documents = new WorkRootDocuments();
                            documents.WorkRoot_Id = workRoots.WorkRoot_Id;
                            documents.Document_Id = item;
                            db.Entry(documents).State = System.Data.Entity.EntityState.Added;
                        }

                        if (db.SaveChanges() > 0)
                        {
                            res = true;
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

        public bool WorkRoot_Save(clsWorkRoots model)
        {
            try
            {
                bool res = new bool();
                WorkRoots workRoots = new WorkRoots();
                workRoots = db.WorkRoots.Find(model.WorkRoots.WorkRoot_Id);

                if (workRoots == null)
                {
                    res = WorkRoot_Insert(model);
                }
                else
                {
                    res = WorkRoot_Update(model);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool WorkRoot_Update(clsWorkRoots model)
        {
            try
            {
                bool res = new bool();
                WorkRoots workRoots = new WorkRoots();
                workRoots = db.WorkRoots.Find(model.WorkRoots.WorkRoot_Id);
                workRoots.WorkRoot_Name = model.WorkRoots.WorkRoot_Name;
                workRoots.Update = DateTime.Now;
                db.Entry(workRoots).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    if (model.Document_Id.Count > 0)
                    {
                        List<WorkRootDocuments> workRootDocuments = new List<WorkRootDocuments>();
                        workRootDocuments = db.WorkRootDocuments
                            .Where(w => w.WorkRoot_Id == model.WorkRoots.WorkRoot_Id)
                            .ToList();
                        if (workRootDocuments.Count > 0)
                        {
                            foreach (var item in model.Document_Id)
                            {
                                var findHas = workRootDocuments
                                    .Where(w => w.Document_Id == item.Value)
                                    .FirstOrDefault();

                                if (findHas == null)
                                {
                                    WorkRootDocuments documents = new WorkRootDocuments();
                                    documents.WorkRoot_Id = model.WorkRoots.WorkRoot_Id;
                                    documents.Document_Id = item;
                                    db.Entry(documents).State = System.Data.Entity.EntityState.Added;
                                }
                            }

                            foreach (var item in workRootDocuments)
                            {
                                var findKeep = model.Document_Id
                                    .Where(w => w == item.Document_Id)
                                    .FirstOrDefault();
                                if (findKeep == null)
                                {
                                    WorkRootDocuments documents = new WorkRootDocuments();
                                    documents = db.WorkRootDocuments.Find(item.Document_Id);
                                    db.Entry(documents).State = System.Data.Entity.EntityState.Deleted;
                                }
                            }
                        }

                        if (db.SaveChanges() > 0)
                        {
                            res = true;
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
    }
}