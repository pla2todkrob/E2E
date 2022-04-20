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
                Topics topics = new Topics();
                topics = db.Topics.Where(w => w.Topic_Id == model.Topic_Id).FirstOrDefault();

                if (topics != null)
                {
                  res = Board_Update(model);
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

        public bool Board_Delete(Guid id)
        {
            try
            {
                bool res = new bool();
                Topics topics = new Topics();
                topics = db.Topics.Where(w => w.Topic_Id == id).FirstOrDefault();

                db.Topics.Remove(topics);
                if (db.SaveChanges() > 0)
                {
                    res = true;
                    Board_Delete_CommentTopics(id);
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
                var TopicComments = db.TopicComments.Where(w => w.Topic_Id == id).ToList();
                if (TopicComments != null)
                {
                    foreach (var item in TopicComments)
                    {
                        db.TopicComments.Remove(item);
                    }

                    if (db.SaveChanges() > 0)
                    {
                  
                    }
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
                if (topicComments!= null)
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

                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    string dir = "Topic/" + model.Topic_Id;
                    string filepath = ftp.Ftp_UploadFileToString(dir, file);
                    Galleries_Save(topics, filepath, file.FileName);
                }

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
                }

                return res;
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected bool Board_Update(Topics model)
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
                }

                return res;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool Board_Reply_Save(TopicComments model,string Boards_Reply)
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

                var  topicComments = db.TopicComments.Where(w => w.TopicComment_Id == id || w.Ref_TopicComment_Id == id).ToList();

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

        protected bool Board_CountComment_Delete(Guid id,int num)
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

        public void Galleries_Save(Topics model,string filepath,string file)
        {
            try
            {
                bool res = new bool();
                Topics topics = new Topics();
                topics = db.Topics.Where(w => w.Topic_Id == model.Topic_Id).FirstOrDefault();

                if (topics != null)
                {
                    //res = Galleries_Update(model, filepath, file);
                }
                else
                {
                    res = Galleries_Insert(model, filepath, file);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Galleries_Insert(Topics model, string filepath, string file)
        {
            try
            {

                bool res = new bool();
                TopicGalleries topicGalleries = new TopicGalleries();

                topicGalleries.Topic_Id = model.Topic_Id;
                topicGalleries.TopicGallery_Original = filepath;
                topicGalleries.TopicGallery_Name = file;
                topicGalleries.TopicGallery_Extension = Path.GetExtension(file);


                db.TopicGalleries.Add(topicGalleries);
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

        //public bool Galleries_Update(Topics model, string filepath, string file)
        //{
        //    try
        //    {

        //        bool res = new bool();
        //        TopicGalleries topicGalleries = new TopicGalleries();
        //        topicGalleries = db.TopicComments
        //            .Where(w => w.TopicComment_Id == model.topicGalleries)
        //            .FirstOrDefault();

         


        //        if (db.SaveChanges() > 0)
        //        {
        //            res = true;
        //        }

        //        return res;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}
    }

}