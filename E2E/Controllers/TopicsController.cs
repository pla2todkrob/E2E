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
        // GET: Topics
        public ActionResult Index()
        {
          return   RedirectToAction("Boards");
        }

        public ActionResult Boards()
        {
            //clsTopic clsTopic = new clsTopic();
            //clsTopic.Topics = db.Topics.Where(w => w.Topic_Id == ).FirstOrDefault();
            //clsTopic.TopicGalleries = db.TopicGalleries.Where(w => w.Topic_Id ==).ToList();
                
            return View();
        }

        public ActionResult Boards_Table(bool val)
        {
            
            Guid id = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.Usercode = db.Users
                .Where(w => w.User_Id == id)
                .Select(s => s.User_Code)
                .FirstOrDefault();

            Topics topics = new Topics();
            var sql = db.Topics.Where(w => w.Topic_Pin == val).ToList();
            if (val)
            {
                foreach (var item in sql.Where(w=>w.Topic_Pin_EndDate < DateTime.Today))
                {
                    item.Topic_Pin = false;
                    item.Topic_Pin_EndDate = null;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                sql = sql.Where(w => w.Topic_Pin_EndDate > DateTime.Today).ToList();
            }
           

            return View(sql);
        }

        public ActionResult Boards_Create(Guid? id)
        {
            if (id.HasValue)
            {
                Topics topics = new Topics();
                topics = db.Topics.Where(w => w.Topic_Id == id).FirstOrDefault();
                return View(topics);
            }

            return View(new Topics());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Boards_Create(Topics model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {


                        if (data.Board_Save(model))
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

        public ActionResult Boards_Form(Guid? id)
        {


            bool isAdmin = true;
            Topics topics = new Topics();

            if (id.HasValue)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (data.UpdateView(id))
                    {
                        scope.Complete();
                    }
                }

                topics = db.Topics.Where(w => w.Topic_Id == id).FirstOrDefault();
                isAdmin = false;
            }

            ViewBag.IsAdmin = isAdmin;

            return View(topics);
        }

    }
}