using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace E2E.Models
{
    public class clsManageEForm
    {
        private clsContext db = new clsContext();
        private clsServiceFTP ftp = new clsServiceFTP();
        public bool EForm_Save(EForms model, HttpFileCollectionBase files)
        {
            try
            {
                bool res = new bool();
                string status = string.Empty;
                EForms eForms = new EForms();
                eForms = db.EForms.Where(w => w.EForm_Id == model.EForm_Id).FirstOrDefault();

                if (eForms != null)
                {
                    status = "U";
                    res = Board_Update(model, files, status);
                }
                else
                {
                    status = "I";
                    res = Board_Insert(model, files, status);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected bool Board_Insert(EForms model, HttpFileCollectionBase files, string status)
        {
            try
            {
                bool res = new bool();
                EForms eForms = new EForms();

                eForms.EForm_Title = model.EForm_Title;
                eForms.EForm_Link = model.EForm_Link.Trim();
                eForms.EForm_Description = model.EForm_Description;
                eForms.Active = model.Active;
                eForms.EForm_Start = model.EForm_Start;
                eForms.EForm_End = model.EForm_End;
                eForms.User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);

                db.EForms.Add(eForms);
                if (db.SaveChanges() > 0)
                {
                    res = true;
                    if (files[0].ContentLength != 0)
                    {
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];

                            bool CK_IMG = IsRecognisedImageFile(file.FileName);
                            string dir = "EForm/" + model.EForm_Id;

                            if (CK_IMG)
                            {
                                string FileName = file.FileName;

                                EForm_Galleries eForm_Galleries = new EForm_Galleries();
                                eForm_Galleries = db.EForm_Galleries.Where(w => w.EForm_Id == model.EForm_Id && w.EForm_Gallery_Name == file.FileName).FirstOrDefault();
                                if (eForm_Galleries != null)
                                {
                                    FileName = string.Concat("_", file.FileName);
                                }
                                string filepath = ftp.Ftp_UploadFileToString(dir, file, FileName);
                                if (filepath != "")
                                {
                                    Galleries_Save(eForms, filepath, FileName, status);
                                }
                            }
                            else
                            {
                                //string FileName = file.FileName;

                                //TopicFiles topicFiles = new TopicFiles();
                                //topicFiles = db.TopicFiles.Where(w => w.Topic_Id == model.Topic_Id && w.TopicFile_Name == file.FileName).FirstOrDefault();
                                //if (topicFiles != null)
                                //{
                                //    FileName = string.Concat("_", file.FileName);
                                //}
                                //string filepath = ftp.Ftp_UploadFileToString(dir, file, FileName);
                                //if (filepath != "")
                                //{
                                //    File_Save(topics, filepath, FileName, status);
                                //}
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

        protected bool Board_Update(EForms model, HttpFileCollectionBase files, string status)
        {
            try
            {
                bool res = new bool();
                EForms EForms = new EForms();
                EForms = db.EForms
                    .Where(w => w.EForm_Id == model.EForm_Id)
                    .FirstOrDefault();

                EForms.EForm_Title = model.EForm_Title.Trim();
                EForms.EForm_Description = model.EForm_Description.Trim();
                EForms.EForm_Link = model.EForm_Link;
                EForms.Active = model.Active;
                EForms.Update = DateTime.Now;

                if (db.SaveChanges() > 0)
                {
                    res = true;
                    if (files[0].ContentLength != 0)
                    {

                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];

                            bool CK_IMG = IsRecognisedImageFile(file.FileName);
                            string dir = "EForm/" + model.EForm_Id;

                            if (CK_IMG)
                            {
                                string FileName = file.FileName;

                                EForm_Galleries eForm_Galleries = new EForm_Galleries();
                                eForm_Galleries = db.EForm_Galleries.Where(w => w.EForm_Id == model.EForm_Id && w.EForm_Gallery_Name == file.FileName).FirstOrDefault();
                                if (eForm_Galleries != null)
                                {
                                    FileName = string.Concat("_", file.FileName);
                                }
                                string filepath = ftp.Ftp_UploadFileToString(dir, file, FileName);
                                if (filepath != "")
                                {
                                    Galleries_Save(EForms, filepath, FileName, status);
                                }
                            }
                            else
                            {
                                //string FileName = file.FileName;

                                //TopicFiles topicFiles = new TopicFiles();
                                //topicFiles = db.TopicFiles.Where(w => w.Topic_Id == model.Topic_Id && w.TopicFile_Name == file.FileName).FirstOrDefault();
                                //if (topicFiles != null)
                                //{
                                //    FileName = string.Concat("_", file.FileName);
                                //}
                                //string filepath = ftp.Ftp_UploadFileToString(dir, file, FileName);
                                //if (filepath != "")
                                //{
                                //    File_Save(topics, filepath, FileName, status);
                                //}
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

        public bool IsRecognisedImageFile(string fileName)
        {
            string targetExtension = System.IO.Path.GetExtension(fileName);
            if (String.IsNullOrEmpty(targetExtension))
                return false;
            else
                targetExtension = "*" + targetExtension.ToLowerInvariant();

            List<string> recognisedImageExtensions = new List<string>();

            foreach (System.Drawing.Imaging.ImageCodecInfo imageCodec in System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders())
                recognisedImageExtensions.AddRange(imageCodec.FilenameExtension.ToLowerInvariant().Split(";".ToCharArray()));

            foreach (string extension in recognisedImageExtensions)
            {
                if (extension.Equals(targetExtension))
                {
                    return true;
                }
            }
            return false;
        }

        public void Galleries_Save(EForms model, string filepath, string file, string status)
        {
            try
            {
                bool res = new bool();
                EForms eForms = new EForms();
                res = Galleries_Insert(model, filepath, file);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Galleries_Insert(EForms model, string filepath, string file)
        {
            try
            {

                bool res = new bool();
                EForm_Galleries eForm_Galleries = new EForm_Galleries();
                var Count = db.EForm_Galleries.Where(w => w.EForm_Id == model.EForm_Id).OrderByDescending(o => o.EForm_Gallery_Seq).FirstOrDefault();

                eForm_Galleries.EForm_Id = model.EForm_Id;
                eForm_Galleries.EForm_Gallery_Original = filepath;
                eForm_Galleries.EForm_Gallery_Name = file;
                eForm_Galleries.EForm_Gallery_Extension = Path.GetExtension(file);
                if (Count == null)
                {
                    eForm_Galleries.EForm_Gallery_Seq = 1;
                }
                else
                {
                    eForm_Galleries.EForm_Gallery_Seq = Count.EForm_Gallery_Seq++;
                }
            


                db.EForm_Galleries.Add(eForm_Galleries);
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



    }
}