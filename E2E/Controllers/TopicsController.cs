using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    [Authorize]
    public class TopicsController : Controller
    {
        private clsManageTopic data = new clsManageTopic();
        private clsContext db = new clsContext();
        private clsServiceFTP ftp = new clsServiceFTP();

        // GET: Topics
        public ActionResult Index()
        {
            return RedirectToAction("Boards");
        }

        public ActionResult Boards()
        {
            //clsTopic clsTopic = new clsTopic();
            //clsTopic.Topics = db.Topics.Where(w => w.Topic_Id == ).FirstOrDefault();
            //clsTopic.TopicGalleries = db.TopicGalleries.Where(w => w.Topic_Id ==).ToList();

            return View();
        }

        public ActionResult Boards_Table(int res)
        {
            try
            {
                bool val = new bool();

                if (res == 1)
                {
                    val = true;
                }

                Guid id = Guid.Parse(HttpContext.User.Identity.Name);
                ViewBag.Usercode = db.Users
                    .Where(w => w.User_Id == id)
                    .Select(s => s.User_Code)
                    .FirstOrDefault();

                var query = db.Topics.Where(w => w.Topic_Pin == val).OrderByDescending(o => new { o.Update, o.Create }).ToList();
                if (val)
                {
                    foreach (var item in query.Where(w => w.Topic_Pin_EndDate < DateTime.Today))
                    {
                        item.Topic_Pin = false;
                        item.Topic_Pin_EndDate = null;
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    query = query.Where(w => w.Topic_Pin_EndDate >= DateTime.Today).ToList();
                }
                if (res == 3)
                {
                    query = db.Topics.OrderByDescending(o => o.Count_View).Take(10).ToList();
                }
                return View(query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Boards_Create(Guid? id)
        {
            try
            {
                Topics topics = new Topics();
                bool isNew = true;
                if (id.HasValue)
                {
                    topics = db.Topics.Find(id);
                    isNew = false;
                }
                ViewBag.isNew = isNew;

                return View(topics);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Boards_Create(Topics model)
        {
            clsSwal swal = new clsSwal();
            if (model.Topic_Title != string.Empty && model.Topic_Content != string.Empty)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Board_Save(model, Request.Files))
                        {
                            scope.Complete();

                            swal.dangerMode = false;
                            swal.icon = "success";
                            swal.text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.title = "Successful";
                        }
                        else
                        {
                            swal.icon = "warning";
                            swal.text = "บันทึกข้อมูลไม่สำเร็จ";
                            swal.title = "Warning";
                        }
                    }
                    catch (DbEntityValidationException ex)
                    {
                        swal.title = ex.TargetSite.Name;
                        foreach (var item in ex.EntityValidationErrors)
                        {
                            foreach (var item2 in item.ValidationErrors)
                            {
                                if (string.IsNullOrEmpty(swal.text))
                                {
                                    swal.text = item2.ErrorMessage;
                                }
                                else
                                {
                                    swal.text += "\n" + item2.ErrorMessage;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        swal.title = ex.TargetSite.Name;
                        swal.text = ex.Message;
                        if (ex.InnerException != null)
                        {
                            swal.text = ex.InnerException.Message;
                            if (ex.InnerException.InnerException != null)
                            {
                                swal.text = ex.InnerException.InnerException.Message;
                            }
                        }
                    }
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                   .Where(y => y.Count > 0)
                                   .ToList();
                swal.icon = "warning";
                swal.title = "Warning";
                foreach (var item in errors)
                {
                    foreach (var item2 in item)
                    {
                        if (string.IsNullOrEmpty(swal.text))
                        {
                            swal.text = item2.ErrorMessage;
                        }
                        else
                        {
                            swal.text += "\n" + item2.ErrorMessage;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete_Boards_Create(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                clsSwal swal = new clsSwal();
                try
                {
                    if (data.Board_Delete(id))
                    {
                        scope.Complete();
                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.title = "Successful";
                    }
                    else
                    {
                        swal.icon = "warning";
                        swal.text = "ลบข้อมูลไม่สำเร็จ";
                        swal.title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.title = ex.TargetSite.Name;
                    swal.text = ex.Message;
                    if (ex.InnerException != null)
                    {
                        swal.text = ex.InnerException.Message;
                        if (ex.InnerException.InnerException != null)
                        {
                            swal.text = ex.InnerException.InnerException.Message;
                        }
                    }
                }

                return Json(swal, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Boards_Form(Guid? id)
        {
            Guid id_emp = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.Usercode = db.Users
                .Where(w => w.User_Id == id_emp)
                .Select(s => s.User_Code)
                .FirstOrDefault();

            clsTopic clsTopic = new clsTopic();

            if (id.HasValue)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (data.UpdateView(id))
                    {
                        scope.Complete();
                    }
                }

                clsTopic.Topics = db.Topics.Where(w => w.Topic_Id == id).FirstOrDefault();
                clsTopic.TopicGalleries = db.TopicGalleries.Where(w => w.Topic_Id == id).ToList();
                clsTopic.TopicComments = db.TopicComments.Where(w => w.Topic_Id == id).OrderBy(o => o.Create).ToList();
                clsTopic.TopicFiles = db.TopicFiles.Where(w => w.Topic_Id == id).ToList();
            }

            return View(clsTopic);
        }

        public ActionResult Boards_Comment(Guid id, Guid? comment_id)
        {
            TopicComments topicComments = new TopicComments();
            topicComments.Topic_Id = id;
            if (comment_id.HasValue)
            {
                topicComments = db.TopicComments.Find(comment_id);
                ViewBag.isNew = false;
                return View(topicComments);
            }
            ViewBag.isNew = true;
            return View(topicComments);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Boards_Comment(TopicComments model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Board_Comment_Save(model))
                        {
                            scope.Complete();
                            swal.dangerMode = false;
                            swal.icon = "success";
                            swal.text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.title = "Successful";
                        }
                        else
                        {
                            swal.icon = "warning";
                            swal.text = "บันทึกข้อมูลไม่สำเร็จ";
                            swal.title = "Warning";
                        }
                    }
                    catch (DbEntityValidationException ex)
                    {
                        swal.title = ex.TargetSite.Name;
                        foreach (var item in ex.EntityValidationErrors)
                        {
                            foreach (var item2 in item.ValidationErrors)
                            {
                                if (string.IsNullOrEmpty(swal.text))
                                {
                                    swal.text = item2.ErrorMessage;
                                }
                                else
                                {
                                    swal.text += "\n" + item2.ErrorMessage;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        swal.title = ex.TargetSite.Name;
                        swal.text = ex.Message;
                        if (ex.InnerException != null)
                        {
                            swal.text = ex.InnerException.Message;
                            if (ex.InnerException.InnerException != null)
                            {
                                swal.text = ex.InnerException.InnerException.Message;
                            }
                        }
                    }
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                   .Where(y => y.Count > 0)
                                   .ToList();
                swal.icon = "warning";
                swal.title = "Warning";
                foreach (var item in errors)
                {
                    foreach (var item2 in item)
                    {
                        if (string.IsNullOrEmpty(swal.text))
                        {
                            swal.text = item2.ErrorMessage;
                        }
                        else
                        {
                            swal.text += "\n" + item2.ErrorMessage;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Boards_Reply(Guid comment_id, Guid? id)
        {
            Session["Boards_Reply"] = "I";
            TopicComments topicComments = new TopicComments();
            topicComments.TopicComment_Id = comment_id;
            if (id.HasValue)
            {
                topicComments = db.TopicComments.Find(comment_id);
                Session["Boards_Reply"] = "U";
                ViewBag.isNew = false;
                return View(topicComments);
            }
            ViewBag.isNew = true;
            return View(topicComments);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Boards_Reply(TopicComments model)
        {
            string Boards_Reply = Session["Boards_Reply"].ToString();
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Board_Reply_Save(model, Boards_Reply))
                        {
                            scope.Complete();
                            swal.dangerMode = false;
                            swal.icon = "success";
                            swal.text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.title = "Successful";
                        }
                        else
                        {
                            swal.icon = "warning";
                            swal.text = "บันทึกข้อมูลไม่สำเร็จ";
                            swal.title = "Warning";
                        }
                    }
                    catch (DbEntityValidationException ex)
                    {
                        swal.title = ex.TargetSite.Name;
                        foreach (var item in ex.EntityValidationErrors)
                        {
                            foreach (var item2 in item.ValidationErrors)
                            {
                                if (string.IsNullOrEmpty(swal.text))
                                {
                                    swal.text = item2.ErrorMessage;
                                }
                                else
                                {
                                    swal.text += "\n" + item2.ErrorMessage;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        swal.title = ex.TargetSite.Name;
                        swal.text = ex.Message;
                        if (ex.InnerException != null)
                        {
                            swal.text = ex.InnerException.Message;
                            if (ex.InnerException.InnerException != null)
                            {
                                swal.text = ex.InnerException.InnerException.Message;
                            }
                        }
                    }
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                   .Where(y => y.Count > 0)
                                   .ToList();
                swal.icon = "warning";
                swal.title = "Warning";
                foreach (var item in errors)
                {
                    foreach (var item2 in item)
                    {
                        if (string.IsNullOrEmpty(swal.text))
                        {
                            swal.text = item2.ErrorMessage;
                        }
                        else
                        {
                            swal.text += "\n" + item2.ErrorMessage;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete_Reply(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                clsSwal swal = new clsSwal();
                try
                {
                    if (data.Delete_Reply(id))
                    {
                        scope.Complete();
                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.title = "Successful";
                    }
                    else
                    {
                        swal.icon = "warning";
                        swal.text = "ลบข้อมูลไม่สำเร็จ";
                        swal.title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.title = ex.TargetSite.Name;
                    swal.text = ex.Message;
                    if (ex.InnerException != null)
                    {
                        swal.text = ex.InnerException.Message;
                        if (ex.InnerException.InnerException != null)
                        {
                            swal.text = ex.InnerException.InnerException.Message;
                        }
                    }
                }

                return Json(swal, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteGallery(Guid id)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.DeleteGallery(id))
                    {
                        scope.Complete();
                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "ลบไฟล์สำเร็จ";
                        swal.title = "Successful";
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    swal.title = ex.TargetSite.Name;
                    foreach (var item in ex.EntityValidationErrors)
                    {
                        foreach (var item2 in item.ValidationErrors)
                        {
                            if (string.IsNullOrEmpty(swal.text))
                            {
                                swal.text = item2.ErrorMessage;
                            }
                            else
                            {
                                swal.text += "\n" + item2.ErrorMessage;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    swal.title = ex.TargetSite.Name;
                    swal.text = ex.Message;
                    if (ex.InnerException != null)
                    {
                        swal.text = ex.InnerException.Message;
                        if (ex.InnerException.InnerException != null)
                        {
                            swal.text = ex.InnerException.InnerException.Message;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteFiles(Guid id)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (data.DeleteFile(id))
                    {
                        scope.Complete();
                        swal.dangerMode = false;
                        swal.icon = "success";
                        swal.text = "ลบไฟล์สำเร็จ";
                        swal.title = "Successful";
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    swal.title = ex.TargetSite.Name;
                    foreach (var item in ex.EntityValidationErrors)
                    {
                        foreach (var item2 in item.ValidationErrors)
                        {
                            if (string.IsNullOrEmpty(swal.text))
                            {
                                swal.text = item2.ErrorMessage;
                            }
                            else
                            {
                                swal.text += "\n" + item2.ErrorMessage;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    swal.title = ex.TargetSite.Name;
                    swal.text = ex.Message;
                    if (ex.InnerException != null)
                    {
                        swal.text = ex.InnerException.Message;
                        if (ex.InnerException.InnerException != null)
                        {
                            swal.text = ex.InnerException.InnerException.Message;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _FileCollection(Guid id)
        {
            clsTopic clsTopic = new clsTopic();

            clsTopic.TopicGalleries = db.TopicGalleries.Where(w => w.Topic_Id == id).ToList();

            clsTopic.TopicFiles = db.TopicFiles.Where(w => w.Topic_Id == id).ToList();

            return View(clsTopic);
        }

        public ActionResult ReloadModel(Guid id)
        {
            clsTopic clsTopic = new clsTopic();

            clsTopic.TopicGalleries = db.TopicGalleries.Where(w => w.Topic_Id == id).ToList();

            clsTopic.TopicFiles = db.TopicFiles.Where(w => w.Topic_Id == id).ToList();

            return View(clsTopic);
        }
    }
}