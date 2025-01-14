﻿using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Security;

namespace E2E.Controllers
{
    public class TopicsController : BaseController
    {
        private readonly ClsMail clsMail = new ClsMail();
        private readonly ClsManageTopic data = new ClsManageTopic();
        private readonly ClsManageMaster master = new ClsManageMaster();
        private readonly ClsSwal swal = new ClsSwal();

        public ActionResult _FileCollection(Guid id)
        {
            ClsTopic clsTopic = new ClsTopic
            {
                TopicGalleries = db.TopicGalleries.Where(w => w.Topic_Id == id).OrderBy(o => o.TopicGallery_Seq).ToList(),

                TopicFiles = db.TopicFiles.Where(w => w.Topic_Id == id).OrderBy(o => o.TopicFile_Seq).ThenBy(t => t.TopicFile_Name).ToList()
            };

            return View(clsTopic);
        }

        [AllowAnonymous]
        public ActionResult _Newtopic()
        {
            DateTime Todate = DateTime.Today;
            int Count = db.Topics.Where(w => w.Create >= Todate).Count();

            return PartialView("_Newtopic", Count);
        }

        public ActionResult _SortTopicAnnounce()
        {
            DateTime Todate = DateTime.Today;
            int Count = db.Topics.Where(w => w.Create >= Todate & w.Topic_Pin == true).Count();

            return PartialView("_SortTopicAnnounce", Count);
        }

        public ActionResult _SortTopicNew()
        {
            DateTime Todate = DateTime.Today;
            int Count = db.Topics.Where(w => w.Create >= Todate & w.Topic_Pin != true).Count();

            return PartialView("_SortTopicNew", Count);
        }

        public ActionResult Boards()
        {
            ViewBag.Categories = SelectListItems_Category_Name();

            return View();
        }

        public ActionResult Boards_Comment(Guid id, Guid? comment_id)
        {
            TopicComments topicComments = new TopicComments
            {
                Topic_Id = id
            };
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
        public async Task<ActionResult> Boards_Comment(TopicComments model)
        {
            ClsSwal swal = new ClsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        if (await data.Board_Comment_Save(model))
                        {
                            scope.Complete();
                            swal.DangerMode = false;
                            swal.Icon = "success";
                            swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.Title = "Successful";
                        }
                        else
                        {
                            swal.Icon = "warning";
                            swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                            swal.Title = "Warning";
                        }
                    }
                    catch (Exception ex)
                    {
                        swal.Title = ex.Source;
                        swal.Text = ex.GetBaseException().Message;
                    }
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                   .Where(y => y.Count > 0)
                                   .ToList();
                swal.Icon = "warning";
                swal.Title = "Warning";
                foreach (var item in errors)
                {
                    foreach (var item2 in item)
                    {
                        if (string.IsNullOrEmpty(swal.Text))
                        {
                            swal.Text = item2.ErrorMessage;
                        }
                        else
                        {
                            swal.Text += "\n" + item2.ErrorMessage;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Boards_Create(Guid? id)
        {
            try
            {
                ViewBag.Create_Categories = SelectListItems_Create_Category_Name();
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
        public async Task<ActionResult> Boards_Create(Topics model)
        {
            ClsSwal swal = new ClsSwal();
            if (model.Topic_Title != string.Empty && model.Topic_Content != string.Empty)
            {
                TransactionOptions options = new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = TimeSpan.MaxValue
                };
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options, TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        if (await data.Board_Save(model, Request.Files))
                        {
                            scope.Complete();
                            swal.DangerMode = false;
                            swal.Icon = "success";
                            swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.Title = "Successful";
                        }
                        else
                        {
                            swal.Icon = "warning";
                            swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                            swal.Title = "Warning";
                        }
                    }
                    catch (Exception ex)
                    {
                        swal.Title = ex.Source;
                        swal.Text = ex.GetBaseException().Message;
                    }
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                   .Where(y => y.Count > 0)
                                   .ToList();
                swal.Icon = "warning";
                swal.Title = "Warning";
                foreach (var item in errors)
                {
                    foreach (var item2 in item)
                    {
                        if (string.IsNullOrEmpty(swal.Text))
                        {
                            swal.Text = item2.ErrorMessage;
                        }
                        else
                        {
                            swal.Text += "\n" + item2.ErrorMessage;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult Boards_Form(Guid? id)
        {
            ClsTopic clsTopic = new ClsTopic();

            if (id.HasValue)
            {
                clsTopic.Topics = db.Topics.Where(w => w.Topic_Id == id.Value).FirstOrDefault();
                clsTopic.TopicGalleries = db.TopicGalleries.Where(w => w.Topic_Id == id.Value).OrderBy(o => o.TopicGallery_Seq).ThenBy(t => t.TopicGallery_Name).ToList();
                clsTopic.TopicComments = db.TopicComments.Where(w => w.Topic_Id == id.Value).OrderBy(o => o.Create).ToList();
                clsTopic.TopicFiles = db.TopicFiles.Where(w => w.Topic_Id == id.Value).OrderBy(o => o.TopicFile_Seq).ThenBy(t => t.TopicFile_Name).ToList();
                clsTopic.TopicSections = db.TopicSections.Where(w => w.Topic_Id == id.Value).OrderBy(o => o.Create).ToList();
                if (!clsTopic.Topics.IsPublic)
                {
                    if (!HttpContext.User.Identity.IsAuthenticated)
                    {
                        FormsAuthentication.RedirectToLoginPage();
                    }
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    if (data.UpdateView(id.Value))
                    {
                        scope.Complete();
                    }
                }
                return View(clsTopic);
            }
            else
            {
                return RedirectToAction("Boards", "Topics");
            }
        }

        public ActionResult Boards_Reply(Guid comment_id, Guid? id)
        {
            Session["Boards_Reply"] = "I";
            TopicComments topicComments = new TopicComments
            {
                TopicComment_Id = comment_id
            };
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
        public async Task<ActionResult> Boards_Reply(TopicComments model)
        {
            string Boards_Reply = Session["Boards_Reply"].ToString();
            ClsSwal swal = new ClsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        if (await data.Board_Reply_Save(model, Boards_Reply))
                        {
                            scope.Complete();
                            swal.DangerMode = false;
                            swal.Icon = "success";
                            swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.Title = "Successful";
                        }
                        else
                        {
                            swal.Icon = "warning";
                            swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                            swal.Title = "Warning";
                        }
                    }
                    catch (Exception ex)
                    {
                        swal.Title = ex.Source;
                        swal.Text = ex.GetBaseException().Message;
                    }
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                   .Where(y => y.Count > 0)
                                   .ToList();
                swal.Icon = "warning";
                swal.Title = "Warning";
                foreach (var item in errors)
                {
                    foreach (var item2 in item)
                    {
                        if (string.IsNullOrEmpty(swal.Text))
                        {
                            swal.Text = item2.ErrorMessage;
                        }
                        else
                        {
                            swal.Text += "\n" + item2.ErrorMessage;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Boards_ReportComment(Guid id)
        {
            var sql = db.TopicComments.Find(id);
            return View(sql);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Boards_ReportComment(TopicComments model, string CommentReportUser)
        {
            if (!string.IsNullOrEmpty(CommentReportUser))
            {
                try
                {
                    var topicComments = await db.TopicComments.FindAsync(model.TopicComment_Id);
                    var linkUrl = System.Web.HttpContext.Current.Request.Url.OriginalString;
                    linkUrl = linkUrl.Replace("Boards_ReportComment", "Boards_Form");
                    linkUrl += "/" + topicComments.Topics.Topic_Id + "/#" + model.TopicComment_Id;

                    string subject = string.Format("[Notify inappropriate comment] {0}", topicComments.Topics.Topic_Title);
                    string content = string.Format("<p><b>Reporter:</b> {0} <b>Comment:</b> {1}", master.Users_GetInfomation(loginId), CommentReportUser);
                    content += "<br />";
                    content += string.Format("<b>Commentator:</b> {1} <b>Comment:</b> {0}", topicComments.Comment_Content, master.Users_GetInfomation(topicComments.User_Id.Value));
                    content += "</p>";
                    content += string.Format("<a href='{0}'>Please, click here to more detail.</a>", linkUrl);
                    content += "<p>Thank you for your consideration</p>";

                    clsMail.AttachPaths.Clear();
                    clsMail.SendBCCs.Clear();
                    clsMail.SendCCs.Clear();
                    clsMail.SendTos.Clear();

                    clsMail.SendTos.AddRange(await master.GetManagementOfDepartment(topicComments.User_Id.Value));
                    clsMail.Subject = subject;
                    clsMail.Body = content;
                    if (await clsMail.SendMail(clsMail))
                    {
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "ส่งรายงานเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "ส่งรายงานไม่สำเร็จ";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.GetBaseException().Message;
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                   .Where(y => y.Count > 0)
                                   .ToList();
                swal.Icon = "warning";
                swal.Title = "Warning";
                foreach (var item in errors)
                {
                    foreach (var item2 in item)
                    {
                        if (string.IsNullOrEmpty(swal.Text))
                        {
                            swal.Text = item2.ErrorMessage;
                        }
                        else
                        {
                            swal.Text += "\n" + item2.ErrorMessage;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Boards_Section(Guid topicId, Guid? id)
        {
            try
            {
                TopicSections topicSections = new TopicSections
                {
                    Topic_Id = topicId
                };

                ViewBag.IsNew = true;

                if (id.HasValue)
                {
                    ViewBag.IsNew = false;
                    topicSections = db.TopicSections.Find(id);
                }
                return View(topicSections);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Boards_Section(TopicSections model)
        {
            ClsSwal swal = new ClsSwal();
            if (ModelState.IsValid)
            {
                TransactionOptions options = new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = TimeSpan.MaxValue
                };
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options, TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        if (await data.Boards_Section_Save(model, Request.Files))
                        {
                            scope.Complete();
                            swal.DangerMode = false;
                            swal.Icon = "success";
                            swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.Title = "Successful";
                        }
                        else
                        {
                            swal.Icon = "warning";
                            swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                            swal.Title = "Warning";
                        }
                    }
                    catch (Exception ex)
                    {
                        swal.Title = ex.Source;
                        swal.Text = ex.GetBaseException().Message;
                    }
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                   .Where(y => y.Count > 0)
                                   .ToList();
                swal.Icon = "warning";
                swal.Title = "Warning";
                foreach (var item in errors)
                {
                    foreach (var item2 in item)
                    {
                        if (string.IsNullOrEmpty(swal.Text))
                        {
                            swal.Text = item2.ErrorMessage;
                        }
                        else
                        {
                            swal.Text += "\n" + item2.ErrorMessage;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Boards_Table(int? res, Guid? category = null)
        {
            try
            {
                bool val = new bool();

                if (res == 1)
                {
                    val = true;
                }

                ViewBag.Usercode = db.Users
                    .Where(w => w.User_Id == loginId)
                    .Select(s => s.User_Code)
                    .FirstOrDefault();

                IQueryable<Topics> query = db.Topics.AsNoTracking().OrderByDescending(o => o.Create).ThenByDescending(t => t.Update);

                if (val)
                {
                    query = query.Where(w => w.Topic_Pin == val);
                    foreach (var item in query.Where(w => w.Topic_Pin_EndDate < DateTime.Today).ToList())
                    {
                        item.Topic_Pin = false;
                        item.Topic_Pin_EndDate = null;
                        db.Topics.Attach(item);
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    query = query.Where(w => w.Topic_Pin_EndDate >= DateTime.Today);
                }

                if (res == 3)//top rate
                {
                    query = query.OrderByDescending(o => o.Count_View).Take(10);
                }

                if (category != null)
                {
                    var categorys = db.Master_Categories.Where(w => w.Category_Id == category).FirstOrDefault();
                    query = query.Where(w => w.Category_Id == categorys.Category_Id);
                }

                return View(query.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Boards_View(Guid id)
        {
            var topicView = db.TopicView.Where(w => w.Topic_Id == id)
                 .Join(db.UserDetails,
                 j => j.User_Id,
                 ud => ud.User_Id,
                 (j, ud) => new ClsTopicView()
                 {
                     Name = ud.Detail_EN_FirstName + " " + ud.Detail_EN_LastName,
                     UserCode = ud.Users.User_Code,
                     Count = j.Count,
                     LastTime = j.LastTime,
                     Department = ud.Users.Master_Processes.Master_Sections.Master_Departments.Department_Name
                 }).ToList();
            return View(topicView);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete_Boards_Create(Guid id)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                ClsSwal swal = new ClsSwal();
                try
                {
                    if (await data.Delete_Attached(id))
                    {
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "ลบข้อมูลไม่สำเร็จ";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.GetBaseException().Message;
                }

                return Json(swal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete_Boards_Section(Guid id)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                ClsSwal swal = new ClsSwal();
                try
                {
                    if (await data.Delete_Boards_Section(id))
                    {
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "ลบข้อมูลไม่สำเร็จ";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.GetBaseException().Message;
                }

                return Json(swal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete_Boards_Section_Attached(Guid id)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                ClsSwal swal = new ClsSwal();
                try
                {
                    if (await data.Delete_Boards_Section_Attached(id))
                    {
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                        swal.Option = id;
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "ลบข้อมูลไม่สำเร็จ";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.GetBaseException().Message;
                }

                return Json(swal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        public ActionResult Delete_Reply(Guid id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                ClsSwal swal = new ClsSwal();
                try
                {
                    if (data.Delete_Reply(id))
                    {
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "ลบข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "ลบข้อมูลไม่สำเร็จ";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.GetBaseException().Message;
                }

                return Json(swal, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> DeleteFiles(Guid id)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.DeleteFile(id))
                    {
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "ลบไฟล์สำเร็จ";
                        swal.Title = "Successful";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.GetBaseException().Message;
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> DeleteGallery(Guid id)
        {
            ClsSwal swal = new ClsSwal();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await data.DeleteGallery(id))
                    {
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "ลบไฟล์สำเร็จ";
                        swal.Title = "Successful";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.GetBaseException().Message;
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        // GET: Topics
        public ActionResult Index()
        {
            return RedirectToAction("Boards");
        }

        public ActionResult ReloadModel(Guid id)
        {
            ClsTopic clsTopic = new ClsTopic
            {
                TopicGalleries = db.TopicGalleries.Where(w => w.Topic_Id == id).OrderBy(o => o.TopicGallery_Seq).ToList(),

                TopicFiles = db.TopicFiles.Where(w => w.Topic_Id == id).OrderBy(o => o.TopicFile_Seq).ThenBy(t => t.TopicFile_Name).ToList()
            };

            return View(clsTopic);
        }

        public ActionResult SaveSeq(string model)
        {
            List<TopicGalleries> topicGalleries = new List<TopicGalleries>();
            topicGalleries = JsonConvert.DeserializeObject<List<TopicGalleries>>(model);

            ClsSwal swal = new ClsSwal();
            if (topicGalleries != null)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.Galleries_SaveSeq(topicGalleries))
                        {
                            scope.Complete();

                            swal.DangerMode = false;
                            swal.Icon = "success";
                            swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.Title = "Successful";
                        }
                        else
                        {
                            swal.Icon = "warning";
                            swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                            swal.Title = "Warning";
                        }
                    }
                    catch (Exception ex)
                    {
                        swal.Title = ex.Source;
                        swal.Text = ex.GetBaseException().Message;
                    }
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                   .Where(y => y.Count > 0)
                                   .ToList();
                swal.Icon = "warning";
                swal.Title = "Warning";
                foreach (var item in errors)
                {
                    foreach (var item2 in item)
                    {
                        if (string.IsNullOrEmpty(swal.Text))
                        {
                            swal.Text = item2.ErrorMessage;
                        }
                        else
                        {
                            swal.Text += "\n" + item2.ErrorMessage;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public List<SelectListItem> SelectListItems_Category_Name()
        {
            IQueryable<Master_Categories> query = db.Master_Categories;

            List<SelectListItem> item = new List<SelectListItem>
            {
                new SelectListItem() { Text = "All topics", Value = "" }
            };
            item.AddRange(query
                .Select(s => new SelectListItem()
                {
                    Value = s.Category_Id.ToString(),
                    Text = s.Category_Name
                }).OrderBy(o => o.Text).ToList());

            return item;
        }

        public List<SelectListItem> SelectListItems_Create_Category_Name()
        {
            IQueryable<Master_Categories> query = db.Master_Categories.Where(w => w.Active);

            List<SelectListItem> item = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Select category", Value = "" }
            };
            item.AddRange(query
                .Select(s => new SelectListItem()
                {
                    Value = s.Category_Id.ToString(),
                    Text = s.Category_Name
                }).OrderBy(o => o.Text).ToList());

            return item;
        }
    }
}
