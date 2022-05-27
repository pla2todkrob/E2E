using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E2E.Models
{
    public class clsManageTopic
    {
        private clsContext db = new clsContext();
        private clsServiceFTP ftp = new clsServiceFTP();
        private clsImage clsImag = new clsImage();

        public bool UpdateView(Guid? id)
        {
            try
            {
                bool res = new bool();

                if (!id.HasValue)
                {
                    return res;
                }

                Topics topics = new Topics();
                topics = db.Topics
                    .Where(w => w.Topic_Id == id)
                    .FirstOrDefault();

                topics.Count_View += 1;

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

        public bool Board_Save(Topics model, HttpFileCollectionBase files)
        {
            try
            {
                bool res = new bool();
                string status = string.Empty;
                Topics topics = new Topics();
                topics = db.Topics.Where(w => w.Topic_Id == model.Topic_Id).FirstOrDefault();

                if (topics != null)
                {

                    res = Board_Update(model, files);
                }
                else
                {

                    res = Board_Insert(model, files);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Board_Delete(Guid id, List<string> File_ = null)
        {
            try
            {
                bool res = new bool();
                Topics topics = new Topics();
                topics = db.Topics.Where(w => w.Topic_Id == id).FirstOrDefault();
                Board_Delete_CommentTopics(id);
                db.Topics.Remove(topics);
                if (db.SaveChanges() > 0)
                {
                    if (File_.Count > 0)
                    {
                        foreach (var item in File_)
                        {
                            ftp.Ftp_DeleteFile(item);
                        }
                    }
                    res = true;
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Board_Delete_CommentTopics(Guid id)
        {
            try
            {
                clsTopic clsTopic = new clsTopic();
                clsTopic.TopicComments = db.TopicComments.Where(w => w.Topic_Id == id || w.Ref_TopicComment_Id == id).ToList();
                clsTopic.TopicFiles = db.TopicFiles.Where(w => w.Topic_Id == id).ToList();
                clsTopic.TopicGalleries = db.TopicGalleries.Where(w => w.Topic_Id == id).ToList();

                if (clsTopic.TopicComments.Count > 0)
                {
                    db.TopicComments.RemoveRange(clsTopic.TopicComments);
                }
                if (clsTopic.TopicFiles.Count > 0)
                {
                    db.TopicFiles.RemoveRange(clsTopic.TopicFiles);
                }
                if (clsTopic.TopicGalleries.Count > 0)
                {
                    db.TopicGalleries.RemoveRange(clsTopic.TopicGalleries);
                }


                if (db.SaveChanges() > 0)
                {

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Board_Comment_Save(TopicComments model)
        {
            try
            {
                bool res = new bool();
                TopicComments topicComments = new TopicComments();
                topicComments = db.TopicComments.Where(w => w.TopicComment_Id == model.TopicComment_Id).FirstOrDefault();
                if (topicComments != null)
                {
                    res = Board_Comment_Update(model);
                }
                else
                {
                    res = Board_Comment_Insert(model);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Board_Comment_Insert(TopicComments model)
        {
            try
            {

                bool res = new bool();
                TopicComments topicComments = new TopicComments();
                topicComments.Topic_Id = model.Topic_Id;
                topicComments.Comment_Content = model.Comment_Content;
                topicComments.User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);


                db.TopicComments.Add(topicComments);
                if (db.SaveChanges() > 0)
                {
                    res = true;
                    Board_CountComment_Update(model);
                }

                return res;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool Board_Comment_Update(TopicComments model)
        {
            try
            {

                bool res = new bool();
                TopicComments topicComments = new TopicComments();
                topicComments = db.TopicComments
                    .Where(w => w.TopicComment_Id == model.TopicComment_Id)
                    .FirstOrDefault();

                topicComments.Comment_Content = model.Comment_Content;
                topicComments.Update = DateTime.Now;


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

        protected bool Board_CountComment_Update(TopicComments model)
        {
            try
            {
                bool res = new bool();
                Topics topics = new Topics();
                topics = db.Topics
                    .Where(w => w.Topic_Id == model.Topic_Id)
                    .FirstOrDefault();

                topics.Count_Comment += 1;
                topics.Update = DateTime.Now;

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

        protected bool Board_Insert(Topics model, HttpFileCollectionBase files)
        {
            try
            {
                bool res = new bool();
                Topics topics = new Topics();

                topics.Topic_Title = model.Topic_Title.Trim();
                topics.Topic_Content = model.Topic_Content.Trim();
                topics.Topic_Pin = model.Topic_Pin;
                topics.Topic_Pin_EndDate = model.Topic_Pin_EndDate;
                topics.Create = DateTime.Now;
                topics.User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);

                db.Topics.Add(topics);
                if (db.SaveChanges() > 0)
                {
                    res = true;
                    if (files[0].ContentLength != 0)
                    {
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];

                            bool CK_IMG = IsRecognisedImageFile(file.FileName);
                            string dir = "Topic/" + topics.Topic_Id;

                            if (CK_IMG)
                            {
                                string FileName = file.FileName;

                                TopicGalleries topicGalleries = new TopicGalleries();
                                topicGalleries = db.TopicGalleries.Where(w => w.Topic_Id == model.Topic_Id && w.TopicGallery_Name == file.FileName).FirstOrDefault();
                                if (topicGalleries != null)
                                {
                                    FileName = string.Concat("_", file.FileName);
                                }
                                clsImag = ftp.Ftp_UploadImageFixSizeToString(dir, file, FileName);
                                if (clsImag != null)
                                {
                                    Galleries_Save(topics, clsImag, FileName);
                                }
                            }
                            else
                            {
                                string FileName = file.FileName;

                                TopicFiles topicFiles = new TopicFiles();
                                topicFiles = db.TopicFiles.Where(w => w.Topic_Id == model.Topic_Id && w.TopicFile_Name == file.FileName).FirstOrDefault();
                                if (topicFiles != null)
                                {
                                    FileName = string.Concat("_", file.FileName);
                                }
                                string filepath = ftp.Ftp_UploadFileToString(dir, file, FileName);
                                if (filepath != "")
                                {
                                    File_Save(topics, filepath, FileName);
                                }
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

        protected bool Board_Update(Topics model, HttpFileCollectionBase files)
        {
            try
            {
                bool res = new bool();
                Topics topics = new Topics();
                topics = db.Topics
                    .Where(w => w.Topic_Id == model.Topic_Id)
                    .FirstOrDefault();

                topics.Topic_Title = model.Topic_Title.Trim();
                topics.Topic_Content = model.Topic_Content.Trim();
                topics.Topic_Pin = model.Topic_Pin;
                topics.Topic_Pin_EndDate = model.Topic_Pin_EndDate;
                topics.Update = DateTime.Now;

                if (db.SaveChanges() > 0)
                {
                    res = true;
                    if (files[0].ContentLength != 0)
                    {

                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];

                            bool CK_IMG = IsRecognisedImageFile(file.FileName);
                            string dir = "Topic/" + model.Topic_Id;

                            if (CK_IMG)
                            {
                                string FileName = file.FileName;

                                TopicGalleries topicGalleries = new TopicGalleries();
                                topicGalleries = db.TopicGalleries.Where(w => w.Topic_Id == model.Topic_Id && w.TopicGallery_Name == file.FileName).FirstOrDefault();
                                if (topicGalleries != null)
                                {
                                    FileName = string.Concat("_", file.FileName);
                                }
                                clsImag = ftp.Ftp_UploadImageFixSizeToString(dir, file, FileName);
                                if (clsImag != null)
                                {
                                    Galleries_Save(topics, clsImag, FileName);
                                }
                            }
                            else
                            {
                                string FileName = file.FileName;

                                TopicFiles topicFiles = new TopicFiles();
                                topicFiles = db.TopicFiles.Where(w => w.Topic_Id == model.Topic_Id && w.TopicFile_Name == file.FileName).FirstOrDefault();
                                if (topicFiles != null)
                                {
                                    FileName = string.Concat("_", file.FileName);
                                }
                                string filepath = ftp.Ftp_UploadFileToString(dir, file, FileName);
                                if (filepath != "")
                                {
                                    File_Save(topics, filepath, FileName);
                                }
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

        public bool Board_Reply_Save(TopicComments model, string Boards_Reply)
        {
            try
            {
                bool res = new bool();

                if (Boards_Reply == "U")
                {
                    res = Board_Reply_Update(model);
                }
                else
                {
                    res = Board_Reply_Insert(model);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Board_Reply_Insert(TopicComments model)
        {
            try
            {

                bool res = new bool();
                var ID = db.TopicComments.Where(w => w.TopicComment_Id == model.TopicComment_Id).FirstOrDefault();
                TopicComments topicComments = new TopicComments();

                topicComments.Topic_Id = ID.Topic_Id;
                topicComments.Ref_TopicComment_Id = model.TopicComment_Id;
                topicComments.Comment_Content = model.Comment_Content;
                topicComments.User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name);


                db.TopicComments.Add(topicComments);
                if (db.SaveChanges() > 0)
                {
                    res = true;
                    Board_CountComment_Update(ID);
                }

                return res;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public bool Board_Reply_Update(TopicComments model)
        {
            try
            {

                bool res = new bool();
                TopicComments topicComments = new TopicComments();
                topicComments = db.TopicComments
                    .Where(w => w.TopicComment_Id == model.TopicComment_Id)
                    .FirstOrDefault();

                topicComments.Comment_Content = model.Comment_Content;
                topicComments.Update = DateTime.Now;


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

        public bool Delete_Reply(Guid id)
        {
            try
            {
                bool res = new bool();

                var topicComments = db.TopicComments.Where(w => w.TopicComment_Id == id || w.Ref_TopicComment_Id == id).ToList();

                foreach (var item in topicComments)
                {
                    db.TopicComments.Remove(item);
                }

                if (db.SaveChanges() > 0)
                {
                    res = true;
                    Board_CountComment_Delete(topicComments[0].Topic_Id, topicComments.Count);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected bool Board_CountComment_Delete(Guid id, int num)
        {
            try
            {
                bool res = new bool();
                Topics topics = new Topics();
                topics = db.Topics
                    .Where(w => w.Topic_Id == id)
                    .FirstOrDefault();

                topics.Count_Comment -= num;
                topics.Update = DateTime.Now;

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

        public void Galleries_Save(Topics model, clsImage clsImage, string file)
        {
            try
            {
                bool res = new bool();
                res = Galleries_Insert(model, clsImage, file);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Galleries_Insert(Topics model, clsImage clsImage, string file)
        {
            try
            {

                bool res = new bool();
                TopicGalleries topicGalleries = new TopicGalleries();
                var Count = db.TopicGalleries.Where(w => w.Topic_Id == model.Topic_Id).OrderByDescending(o => o.TopicGallery_Seq).FirstOrDefault();

                topicGalleries.Topic_Id = model.Topic_Id;
                topicGalleries.TopicGallery_Original = clsImage.OriginalPath;
                topicGalleries.TopicGallery_Thumbnail = clsImage.ThumbnailPath;
                topicGalleries.TopicGallery_Name = file;
                topicGalleries.TopicGallery_Extension = Path.GetExtension(file);

                if (Count == null)
                {
                    topicGalleries.TopicGallery_Seq = 1;
                }
                else
                {
                    topicGalleries.TopicGallery_Seq = Count.TopicGallery_Seq++;
                }

                db.TopicGalleries.Add(topicGalleries);
                if (db.SaveChanges() > 0)
                {
                    res = true;
                    Board_CountImage_Update(model);
                }

                return res;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void File_Save(Topics model, string filepath, string file)
        {
            try
            {
                File_Insert(model, filepath, file);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool File_Insert(Topics model, string filepath, string file)
        {
            try
            {

                bool res = new bool();
                TopicFiles topicFiles = new TopicFiles();

                topicFiles.Topic_Id = model.Topic_Id;
                topicFiles.TopicFile_Path = filepath;
                topicFiles.TopicFile_Name = file;
                topicFiles.TopicFile_Extension = Path.GetExtension(file);


                db.TopicFiles.Add(topicFiles);
                if (db.SaveChanges() > 0)
                {
                    res = true;
                    Board_CountFiles_Update(model);
                }

                return res;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        protected bool Board_CountFiles_Update(Topics model)
        {
            try
            {
                bool res = new bool();
                Topics topics = new Topics();
                topics = db.Topics
                    .Where(w => w.Topic_Id == model.Topic_Id)
                    .FirstOrDefault();

                topics.Topic_FileCount += 1;
                topics.Update = DateTime.Now;

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

        protected bool Board_CountImage_Update(Topics model)
        {
            try
            {
                bool res = new bool();
                Topics topics = new Topics();
                topics = db.Topics
                    .Where(w => w.Topic_Id == model.Topic_Id)
                    .FirstOrDefault();

                topics.Topic_GalleryCount += 1;
                topics.Update = DateTime.Now;

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

        public bool DeleteGallery(Guid id, bool status = true)
        {
            try
            {

                bool res = new bool();

                var TopicGalleries = db.TopicGalleries.Where(w => w.TopicGallery_Id == id).FirstOrDefault();

                Guid topic_id = TopicGalleries.Topic_Id;

                db.TopicGalleries.Remove(TopicGalleries);


                if (db.SaveChanges() > 0)
                {
                    if (status)
                    {
                        ftp.Ftp_DeleteFile(TopicGalleries.TopicGallery_Original);
                        ftp.Ftp_DeleteFile(TopicGalleries.TopicGallery_Thumbnail);
                    }

                    res = DeleteGallery_Count(topic_id);

                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected bool DeleteGallery_Count(Guid id)
        {
            try
            {
                bool res = new bool();
                Topics topics = new Topics();
                topics = db.Topics
                    .Where(w => w.Topic_Id == id)
                    .FirstOrDefault();

                topics.Topic_GalleryCount -= 1;
                topics.Update = DateTime.Now;

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

        public bool DeleteFile(Guid id, bool status = true)
        {
            try
            {

                bool res = new bool();

                var TopicFiles = db.TopicFiles.Where(w => w.TopicFile_Id == id).FirstOrDefault();

                Guid topic_id = TopicFiles.Topic_Id;

                db.TopicFiles.Remove(TopicFiles);

                if (db.SaveChanges() > 0)
                {
                    if (status)
                    {
                        ftp.Ftp_DeleteFile(TopicFiles.TopicFile_Path);
                    }

                    res = DeleteFile_Count(topic_id);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected bool DeleteFile_Count(Guid id)
        {
            try
            {
                bool res = new bool();
                Topics topics = new Topics();
                topics = db.Topics
                    .Where(w => w.Topic_Id == id)
                    .FirstOrDefault();

                topics.Topic_FileCount -= 1;
                topics.Update = DateTime.Now;

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

        public bool Galleries_SaveSeq(List<TopicGalleries> model)
        {
            try
            {
                bool res = new bool();

                foreach (var item in model)
                {
                    TopicGalleries topicGalleries = new TopicGalleries();
                    topicGalleries = db.TopicGalleries.Where(w => w.TopicGallery_Id == item.TopicGallery_Id).FirstOrDefault();
                    if (topicGalleries != null)
                    {
                        res = Galleries_SaveSeq_Update(item);
                    }
                }
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected bool Galleries_SaveSeq_Update(TopicGalleries model)
        {
            try
            {
                bool res = new bool();
                TopicGalleries topicGalleries = new TopicGalleries();
                topicGalleries = db.TopicGalleries
                    .Where(w => w.TopicGallery_Id == model.TopicGallery_Id)
                    .FirstOrDefault();

                topicGalleries.TopicGallery_Seq = model.TopicGallery_Seq;

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

        public bool Delete_Attached(Guid id)
        {
            try
            {
                bool res = new bool();
                clsTopic clsTopic = new clsTopic();

                List<string> FilePath = new List<string>();

                clsTopic.TopicFiles = db.TopicFiles.Where(w => w.Topic_Id == id).ToList();
                FilePath.AddRange(clsTopic.TopicFiles.Select(s => s.TopicFile_Path).ToList());
                if (clsTopic.TopicFiles.Count > 0)
                {
                    foreach (var item in clsTopic.TopicFiles)
                    {
                        DeleteFile(item.TopicFile_Id, false);
                    }

                }

                clsTopic.TopicGalleries = db.TopicGalleries.Where(w => w.Topic_Id == id).ToList();
                FilePath.AddRange(clsTopic.TopicGalleries.Select(s => s.TopicGallery_Original).ToList());
                FilePath.AddRange(clsTopic.TopicGalleries.Select(s => s.TopicGallery_Thumbnail).ToList());
                if (clsTopic.TopicGalleries.Count > 0)
                {
                    foreach (var item in clsTopic.TopicGalleries)
                    {
                        DeleteGallery(item.TopicGallery_Id, false);
                    }

                }


                res = Board_Delete(id, FilePath);


                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

}