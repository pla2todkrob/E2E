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

        public ActionResult Boards_Table()
        {
            
            Guid id = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.Usercode = db.Users
                .Where(w => w.User_Id == id)
                .Select(s => s.User_Code)
                .FirstOrDefault();

            return View(db.Topics.OrderByDescending(o=> new {o.Topic_Pin,o.Create }).ToList());
        }

        public ActionResult Boards_Create()
        {
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