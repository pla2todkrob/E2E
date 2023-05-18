using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class ChatBotController : Controller
    {
        private readonly ClsChatBot chatBot = new ClsChatBot();
        private readonly ClsContext db = new ClsContext();
        private readonly ClsManageMaster master = new ClsManageMaster();

        public ActionResult ChatHistory()
        {
            Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
            return View(db.ChatBotHistories.Where(w => w.IsDisplay && w.User_Id == userId));
        }

        public ActionResult GetChatBot(Guid id)
        {
            try
            {
                return View(chatBot.GetChatBotViewModel(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult GroupTable()
        {
            return View(db.ChatBots.OrderBy(o => o.Group).ToList());
        }

        public ActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportExcel()
        {
            ClsSwal swal = new ClsSwal();
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.MaxValue
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    var files = HttpContext.Request.Files;
                    foreach (string item in files.AllKeys)
                    {
                        HttpPostedFileBase file = files[item];

                        if (file.ContentLength > 0)
                        {
                            //ChatBotUploadHistory chatBotUploadHistory = new ChatBotUploadHistory
                            //{
                            //    ChatBotUploadHistoryFile = filePath,
                            //    ChatBotUploadHistoryFileName = Path.GetFileName(filePath),
                            //    User_Id = Guid.Parse(HttpContext.User.Identity.Name)
                            //};
                            //db.Entry(chatBotUploadHistory).State = System.Data.Entity.EntityState.Added;
                            //if (db.SaveChanges() > 0)
                            //{
                            //    if (chatBot.SaveChatBotLearn(filePath))
                            //    {
                            //        scope.Complete();
                            //        swal.Icon = "success";
                            //        swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            //        swal.Title = "Successful";
                            //        swal.DangerMode = false;
                            //    }
                            //    else
                            //    {
                            //        swal.Icon = "warning";
                            //        swal.Text = "กรุณาตรวจสอบข้อมูลอีกครั้ง";
                            //        swal.Title = "Warning";
                            //    }
                            //}
                        }
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.TargetSite.Name;
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Text = inner.Message;
                        inner = inner.InnerException;
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            ViewBag.IsAdmin = master.IsAdmin();
            return View();
        }

        public ActionResult Manage()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
