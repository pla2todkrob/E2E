using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E2E.Models
{
    public class clsManageTopic
    {
        private clsContext db = new clsContext();

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

        public bool Board_Save(Topics model)
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
                  res = Board_Insert(model);
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

        protected bool Board_Insert(Topics model)
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
    }

}