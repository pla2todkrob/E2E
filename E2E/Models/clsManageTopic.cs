using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace E2E.Models
{
    public class ClsManageTopic
    {
        private readonly ClsMail clsMail = new ClsMail();
        private readonly ClsContext db = new ClsContext();
        private readonly ClsServiceFTP ftp = new ClsServiceFTP();
        private readonly ClsManageMaster master = new ClsManageMaster();
        private ClsImage clsImag = new ClsImage();

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

        protected bool Board_Insert(Topics model, HttpFileCollectionBase files)
        {
            try
            {
                bool res = new bool();
                Topics topics = new Topics
                {
                    Topic_Title = model.Topic_Title.Trim(),
                    Topic_Content = model.Topic_Content.Trim(),
                    Topic_Pin = model.Topic_Pin,
                    Topic_Pin_EndDate = model.Topic_Pin_EndDate,
                    Create = DateTime.Now,
                    User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name),
                    Category_Id = model.Category_Id,
                    IsPublic = model.IsPublic
                };

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
                                clsImag = ftp.Ftp_UploadImageToString(dir, file, FileName);
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
                topics.Category_Id = model.Category_Id;
                topics.IsPublic = model.IsPublic;

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
                                clsImag = ftp.Ftp_UploadImageToString(dir, file, FileName);
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

        public bool Board_Comment_Insert(TopicComments model)
        {
            try
            {
                bool res = new bool();
                TopicComments topicComments = new TopicComments
                {
                    Topic_Id = model.Topic_Id,
                    Comment_Content = model.Comment_Content,
                    User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name)
                };

                db.TopicComments.Add(topicComments);
                if (db.SaveChanges() > 0)
                {
                    var query = db.Topics.Find(model.Topic_Id);

                    var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                    linkUrl = linkUrl.Replace("Boards_Comment", "Boards_Form");
                    linkUrl += "/" + query.Topic_Id;

                    string subject = string.Format("[Notify new comment] {0}", query.Topic_Title);
                    string content = string.Format("<p><b>To:</b> {0}", master.Users_GetInfomation(topicComments.User_Id.Value));
                    content += "<br />";
                    content += string.Format("<b>Comment:</b> {0}", model.Comment_Content);
                    content += "</p>";
                    content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                    content += "<p>Thank you for your consideration</p>";
                    res = clsMail.SendMail(query.User_Id, subject, content);

                    Board_CountComment_Update(model);
                }

                return res;
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
                            ftp.Api_DeleteFile(item);
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
                ClsTopic clsTopic = new ClsTopic
                {
                    TopicComments = db.TopicComments.Where(w => w.Topic_Id == id || w.Ref_TopicComment_Id == id).ToList(),
                    TopicFiles = db.TopicFiles.Where(w => w.Topic_Id == id).ToList(),
                    TopicGalleries = db.TopicGalleries.Where(w => w.Topic_Id == id).ToList()
                };

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

        public bool Board_Reply_Insert(TopicComments model)
        {
            try
            {
                bool res = new bool();
                var DBTopicComment = db.TopicComments.Where(w => w.TopicComment_Id == model.TopicComment_Id).FirstOrDefault();
                TopicComments topicComments = new TopicComments
                {
                    Topic_Id = DBTopicComment.Topic_Id,
                    Ref_TopicComment_Id = model.TopicComment_Id,
                    Comment_Content = model.Comment_Content,
                    User_Id = Guid.Parse(HttpContext.Current.User.Identity.Name)
                };

                db.TopicComments.Add(topicComments);
                if (db.SaveChanges() > 0)
                {
                    //var query = db.TopicComments.Where(w => w.Topic_Id == model.Topic_Id).Select(s=>s.).FirstOrDefault();

                    var linkUrl = HttpContext.Current.Request.Url.OriginalString;
                    linkUrl = linkUrl.Replace("Boards_Reply", "Boards_Form");
                    linkUrl += "/" + DBTopicComment.Topics.Topic_Id;

                    string subject = string.Format("[Notify new comment] {0}", DBTopicComment.Topics.Topic_Title);
                    string content = string.Format("<p><b>To:</b> {0}", master.Users_GetInfomation(DBTopicComment.User_Id.Value));
                    content += "<br />";
                    content += string.Format("<b>Comment:</b> {0}", model.Comment_Content);
                    content += "</p>";
                    content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                    content += "<p>Thank you for your consideration</p>";
                    res = clsMail.SendMail(DBTopicComment.User_Id.Value, subject, content);

                    Board_CountComment_Update(DBTopicComment);
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

        public bool Boards_Section_Insert(TopicSections model, HttpFileCollectionBase files)
        {
            try
            {
                bool res = new bool();
                TopicSections topicSections = new TopicSections
                {
                    TopicSection_Description = model.TopicSection_Description,
                    TopicSection_Link = model.TopicSection_Link,
                    TopicSection_Title = model.TopicSection_Title,
                    Topic_Id = model.Topic_Id
                };

                if (files[0].ContentLength > 0)
                {
                    HttpPostedFileBase file = files[0];
                    topicSections.TopicSection_ContentType = file.ContentType;
                    topicSections.TopicSection_Extension = Path.GetExtension(file.FileName);
                    topicSections.TopicSection_Name = file.FileName;

                    string fulldir = string.Format("Topic/{0}/Media/", model.Topic_Id);
                    topicSections.TopicSection_Path = ftp.Ftp_UploadFileToString(fulldir, file);
                }
                db.TopicSections.Add(topicSections);
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

        public bool Boards_Section_Save(TopicSections model, HttpFileCollectionBase files)
        {
            try
            {
                bool res = new bool();
                TopicSections topicSections = new TopicSections();
                topicSections = db.TopicSections.Find(model.TopicSection_Id);

                Boards_Section_UpdateTopics(model);

                if (topicSections == null)
                {
                    res = Boards_Section_Insert(model, files);
                }
                else
                {
                    res = Boards_Section_Update(model, files);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Boards_Section_Update(TopicSections model, HttpFileCollectionBase files)
        {
            try
            {
                bool res = new bool();
                TopicSections topicSections = new TopicSections();
                topicSections = db.TopicSections
                    .Where(w => w.TopicSection_Id == model.TopicSection_Id)
                    .FirstOrDefault();

                topicSections.TopicSection_Link = model.TopicSection_Link;
                topicSections.TopicSection_Title = model.TopicSection_Title;
                topicSections.TopicSection_Description = model.TopicSection_Description;
                if (files[0].ContentLength > 0)
                {
                    HttpPostedFileBase file = files[0];
                    topicSections.TopicSection_ContentType = file.ContentType;
                    topicSections.TopicSection_Extension = Path.GetExtension(file.FileName);
                    topicSections.TopicSection_Name = file.FileName;

                    string fulldir = string.Format("Topic/{0}/Media/", model.Topic_Id);
                    topicSections.TopicSection_Path = ftp.Ftp_UploadFileToString(fulldir, file);
                }
                topicSections.Update = DateTime.Now;

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

        public bool Boards_Section_UpdateTopics(TopicSections model)
        {
            try
            {
                bool res = new bool();
                Topics topics = new Topics();
                topics = db.Topics.Where(w => w.Topic_Id == model.Topic_Id).FirstOrDefault();
                topics.Update = model.Create;

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
                ClsTopic clsTopic = new ClsTopic();

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

        public bool Delete_Boards_Section(Guid id)
        {
            try
            {
                bool res = new bool();

                var TopicSections = db.TopicSections.Where(w => w.TopicSection_Id == id).FirstOrDefault();

                db.TopicSections.Remove(TopicSections);

                if (db.SaveChanges() > 0)
                {
                    res = true;
                    if (!string.IsNullOrEmpty(TopicSections.TopicSection_Path))
                    {
                        res = ftp.Api_DeleteFile(TopicSections.TopicSection_Path);
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Delete_Boards_Section_Attached(Guid id)
        {
            try
            {
                bool res = new bool();

                var TopicSections = db.TopicSections.Where(w => w.TopicSection_Id == id).FirstOrDefault();

                TopicSections.TopicSection_Path = string.Empty;
                TopicSections.TopicSection_Name = string.Empty;
                TopicSections.TopicSection_Extension = string.Empty;
                TopicSections.TopicSection_ContentType = string.Empty;

                res = true;
                if (db.SaveChanges() > 0)
                {
                    if (!string.IsNullOrEmpty(TopicSections.TopicSection_Path))
                    {
                        res = ftp.Api_DeleteFile(TopicSections.TopicSection_Path);
                    }
                }

                return res;
            }
            catch (Exception)
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
                        ftp.Api_DeleteFile(TopicFiles.TopicFile_Path);
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
                        ftp.Api_DeleteFile(TopicGalleries.TopicGallery_Original);
                        ftp.Api_DeleteFile(TopicGalleries.TopicGallery_Thumbnail);
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

        public bool File_Insert(Topics model, string filepath, string file)
        {
            try
            {
                bool res = new bool();
                TopicFiles topicFiles = new TopicFiles
                {
                    Topic_Id = model.Topic_Id,
                    TopicFile_Path = filepath,
                    TopicFile_Name = file,
                    TopicFile_Extension = Path.GetExtension(file)
                };

                db.TopicFiles.Add(topicFiles);
                if (db.SaveChanges() > 0)
                {
                    res = true;
                    Board_CountFiles_Update(model);
                }

                return res;
            }
            catch (Exception)
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

        public bool Galleries_Insert(Topics model, ClsImage clsImage, string file)
        {
            try
            {
                bool res = new bool();
                TopicGalleries topicGalleries = new TopicGalleries();
                int Count = db.TopicGalleries.Where(w => w.Topic_Id == model.Topic_Id).OrderByDescending(o => o.TopicGallery_Seq).Select(s => s.TopicGallery_Seq).FirstOrDefault();

                topicGalleries.Topic_Id = model.Topic_Id;
                topicGalleries.TopicGallery_Original = clsImage.OriginalPath;
                topicGalleries.TopicGallery_Thumbnail = clsImage.ThumbnailPath;
                topicGalleries.TopicGallery_Name = file;
                topicGalleries.TopicGallery_Extension = Path.GetExtension(file);

                if (Count == 0)
                {
                    topicGalleries.TopicGallery_Seq = 1;
                }
                else
                {
                    int sum = Count + 1;
                    topicGalleries.TopicGallery_Seq = sum;
                }

                db.TopicGalleries.Add(topicGalleries);
                if (db.SaveChanges() > 0)
                {
                    res = Board_CountImage_Update(model);
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Galleries_Save(Topics model, ClsImage clsImage, string file)
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

        public bool Galleries_SaveSeq(List<TopicGalleries> model)
        {
            try
            {
                bool res = new bool();

                foreach (var item in model)
                {
                    TopicGalleries topicGalleries = new TopicGalleries();
                    topicGalleries = db.TopicGalleries.Where(w => w.TopicGallery_Id == item.TopicGallery_Id).FirstOrDefault();
                    topicGalleries.TopicGallery_Seq = item.TopicGallery_Seq;
                    db.Entry(topicGalleries).State = System.Data.Entity.EntityState.Modified;
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

        public bool InsertUpdateView(Guid id, Guid? UserId)
        {
            try
            {
                bool res = new bool();

                TopicView topicView = new TopicView
                {
                    Topic_Id = id
                };
                topicView.Count += 1;
                if (UserId.HasValue)
                {
                    topicView.User_Id = UserId;
                }

                db.TopicView.Add(topicView);

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

        public bool UpdateView(Guid id)
        {
            try
            {
                TopicView topicView = new TopicView();
                bool res = new bool();

                string strUserId = HttpContext.Current.User.Identity.Name;

                Guid? userId = null;

                if (!string.IsNullOrEmpty(strUserId))
                {
                    userId = Guid.Parse(strUserId);
                    topicView = db.TopicView.Where(w => w.Topic_Id == id && w.User_Id == userId).FirstOrDefault();
                }

                Topics topics = new Topics();
                topics = db.Topics
                    .Where(w => w.Topic_Id == id)
                    .FirstOrDefault();

                if (topicView?.Count > 0)
                {
                    topicView.Count += 1;
                    topicView.LastTime = DateTime.Now;
                }
                else
                {
                    InsertUpdateView(id, userId);
                }

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
    }
}
