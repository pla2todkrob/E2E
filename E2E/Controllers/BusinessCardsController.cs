using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    [Authorize]
    public class BusinessCardsController : Controller
    {
        private readonly ClsContext db = new ClsContext();
        private readonly ClsManageService data = new ClsManageService();
        private readonly ClsManageBusinessCard dataCard = new ClsManageBusinessCard();
        readonly ClsApi clsApi = new ClsApi();
        private readonly ClsServiceFTP ftp = new ClsServiceFTP();
        private static Guid UserAuthorized { get; set; }

        // GET: BusinessCards
        public ActionResult Index()
        {
            UserAuthorized = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.GA = db.Users.Any(a => a.User_Id == UserAuthorized && a.BusinessCardGroup == true);
            ViewBag.authorized = db.Users.Where(w => w.User_Id == UserAuthorized && w.Master_Grades.Master_LineWorks.Authorize_Id == 2).Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id).FirstOrDefault();

            return View();
        }
        public List<ClsBusinessCard> queryClsBusinessCard()
        {

            List<ClsBusinessCard> clsBusinessCards = db.BusinessCards
                .OrderByDescending(o => o.Create)
                .Select(s => new ClsBusinessCard()
                {
                    Key = s.Key,
                    Create = s.Create,
                    User_id = s.User_id,
                    Status_Id = s.Status_Id,
                    BothSided = s.BothSided,
                    Amount = s.Amount,
                    UserAction = s.UserAction.Value,
                    UserRef_id = s.UserRef_id,
                    UserActionName = db.UserDetails.Where(w => w.User_Id == s.UserAction).Select(ss => ss.Users.Username).FirstOrDefault(),
                    Tel_External = s.Tel_External,
                    Tel_Internal = s.Tel_Internal,
                    System_Statuses = db.System_Statuses.Where(w => w.Status_Id == s.Status_Id).FirstOrDefault(),
                    UserDetails = db.UserDetails.Where(w => w.User_Id == s.User_id).FirstOrDefault(),
                    UserRefName = db.UserDetails.Where(w => w.User_Id == s.UserRef_id).Select(ss => ss.Users.Username).FirstOrDefault(),
                    BusinessCard_Id = s.BusinessCard_Id

                }).ToList();


            return clsBusinessCards;
        }
        public ActionResult Table_MyRequest()
        {
            Guid MyUserid = Guid.Parse(HttpContext.User.Identity.Name);

            var clsBusinessCard = queryClsBusinessCard().Where(w => w.User_id == MyUserid).OrderByDescending(o => o.Create).ToList();

            return View(clsBusinessCard);
        }
        public ActionResult Table_AllTask()
        {
            UserAuthorized = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.GA = db.Users.Any(a => a.User_Id == UserAuthorized && a.BusinessCardGroup == true);
            var Staff = db.Users.Where(w => w.User_Id == UserAuthorized).Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id).FirstOrDefault();

            var clsBusinessCard = queryClsBusinessCard().OrderByDescending(o => o.System_Statuses.OrderBusinessCard).ToList();

            //ถ้าเป็น Staff
            if (Staff == 3)
            {
                clsBusinessCard = queryClsBusinessCard().Where(w=>w.Status_Id == 8).OrderByDescending(o => o.System_Statuses.OrderBusinessCard).ToList();
            }

            return View(clsBusinessCard);
        }
        public ActionResult Table_Approval()
        {
            Guid MyDeptId = db.Users.Where(w => w.User_Id == UserAuthorized).Select(s => s.Master_Processes.Master_Sections.Department_Id).FirstOrDefault();
            var clsBusinessCard = queryClsBusinessCard().Where(w => w.UserDetails.Users.Master_Processes.Master_Sections.Master_Departments.Department_Id == MyDeptId).ToList();

            return View(clsBusinessCard);
        }
        public ActionResult BusinessCard_Detail(Guid id)
        {
            ViewBag.UserCardList = dataCard.SelectListItems_CardGroup();
            ViewBag.GA = db.Users.Any(a => a.User_Id == UserAuthorized && a.BusinessCardGroup == true);
            ViewBag.StatusId = db.BusinessCards.Where(w => w.BusinessCard_Id == id).Select(s => s.Status_Id).FirstOrDefault();
            ViewBag.authorized = db.Users.Where(w => w.User_Id == UserAuthorized).Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id).FirstOrDefault();
            var clsBusinessCard = queryClsBusinessCard().Where(w => w.BusinessCard_Id == id);

            return View(clsBusinessCard.FirstOrDefault());
        }
        public ActionResult BusinessCard_Create(Guid? id)
        {

            ClsBusinessCard businessCards;
            if (!id.HasValue)
            {
                Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                businessCards = new ClsBusinessCard()
                {
                    User_id = userId,
                    UserDetails = db.UserDetails.Where(w => w.User_Id == userId).FirstOrDefault()
                };
            }
            else
            {

                businessCards = new ClsBusinessCard()
                {
                    User_id = id.Value,
                    UserDetails = db.UserDetails.Where(w => w.User_Id == id.Value).FirstOrDefault()
                };
            }


            ViewBag.UserList = data.SelectListItems_User();
            return View(businessCards);
        }
        [HttpPost]
        public ActionResult BusinessCard_Create(ClsBusinessCard Model)
        {

            ClsSwal swal = new ClsSwal();
            if (Model.User_id.HasValue)
            {
                TransactionOptions options = new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = TimeSpan.MaxValue
                };
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
                {
                    try
                    {

                        if (dataCard.BusinessCard_SaveCreate(Model))
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
                        swal.Text = ex.Message;
                        Exception inner = ex.InnerException;
                        while (inner != null)
                        {
                            swal.Title = inner.Source;
                            swal.Text += string.Format("\n{0}", inner.Message);
                            inner = inner.InnerException;
                        }
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
        public ActionResult ManagerUserApprove(Guid? id)
        {
            try
            {
                BusinessCards businessCards = new BusinessCards();
                businessCards = db.BusinessCards.Find(id);
                businessCards.Status_Id = 7;
                businessCards.Create = DateTime.Now;

                if (db.SaveChanges() > 0)
                {
                    dataCard.BusinessCard_SaveLog(businessCards);
                    dataCard.SendMail(businessCards);
                }

            }
            catch (Exception)
            {

                throw;
            }

            return RedirectToAction("BusinessCard_Detail", "BusinessCards", new { @id = id });
        }
        public ActionResult ManagerUserReject(Guid? id, string remark)
        {
            try
            {
                BusinessCards businessCards = new BusinessCards();
                businessCards = db.BusinessCards.Find(id);
                businessCards.Status_Id = 5;
                businessCards.Create = DateTime.Now;

                if (db.SaveChanges() > 0)
                {
                    dataCard.BusinessCard_SaveLog(businessCards);
                    dataCard.SendMail(businessCards,null,null,"", remark);
                }

            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }
        public ActionResult ManagerGaApprove(Guid? id, Guid? SelectId)
        {
            try
            {
                BusinessCards businessCards = new BusinessCards();
                businessCards = db.BusinessCards.Find(id);
                businessCards.Status_Id = 8;
                businessCards.Create = DateTime.Now;

                if (db.SaveChanges() > 0)
                {
                    dataCard.BusinessCard_SaveLog(businessCards);
                    dataCard.SendMail(businessCards, SelectId);
                }

            }
            catch (Exception)
            {

                throw;
            }

            return RedirectToAction("BusinessCard_Detail", "BusinessCards", new { @id = id });
        }
        public ActionResult ManagerGaReject(Guid? id, string remark)
        {
            try
            {
                BusinessCards businessCards = new BusinessCards();
                businessCards = db.BusinessCards.Find(id);
                businessCards.Status_Id = 5;
                businessCards.Create = DateTime.Now;

                if (db.SaveChanges() > 0)
                {
                    dataCard.BusinessCard_SaveLog(businessCards);
                    dataCard.SendMail(businessCards,null,null,"",remark);
                }

            }
            catch (Exception)
            {
                throw;
            }

            return View();
        }
        public ActionResult StaffStart(Guid? id)
        {
            BusinessCards businessCards = new BusinessCards();
            businessCards = db.BusinessCards.Find(id);
            businessCards.Status_Id = 2;
            businessCards.Create = DateTime.Now;
            businessCards.UserAction = Guid.Parse(HttpContext.User.Identity.Name);

            if (db.SaveChanges() > 0)
            {
                dataCard.BusinessCard_SaveLog(businessCards);
            }

            return View();
        }
        public ActionResult StaffUndo(Guid? id,string remark)
        {
            BusinessCards businessCards = new BusinessCards();
            businessCards = db.BusinessCards.Find(id);
            businessCards.Status_Id = 7;
            businessCards.Create = DateTime.Now;
            businessCards.UserAction = null;

            if (db.SaveChanges() > 0)
            {
                dataCard.SendMail(businessCards,null,null,"",remark,"7");
                dataCard.BusinessCard_SaveLog(businessCards);
            }

            return View();
        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file, Guid? id)
        {

            if (file != null && file.ContentLength > 0 && id.HasValue)
            {
                string dir = "BusinessCard/" + id.Value;
                string FileName = file.FileName;

                bool cardFiles = db.BusinessCardFiles.Any(a => a.BusinessCard_Id == id.Value && a.FileName == file.FileName);

                if (cardFiles)
                {
                    FileName = string.Concat("_", file.FileName);
                }

                string filepath = ftp.Ftp_UploadFileToString(dir, file, FileName);

                if (!string.IsNullOrEmpty(filepath))
                {
                    var sql = db.BusinessCards.Find(id);
                    dataCard.BusinessCard_SaveFile(filepath, sql);
                }
            }
            return View();
        }
        public ActionResult UploadHistory(Guid? id)
        {
            var res = db.BusinessCardFiles.Where(w => w.BusinessCard_Id == id).OrderByDescending(o => o.Create).ToList();

            return View(res);
        }
        public ActionResult UserConfirmCancel(Guid? id, string remark)
        {
            BusinessCardFiles businessCardFiles = new BusinessCardFiles();
            businessCardFiles = db.BusinessCardFiles.Find(id);
            businessCardFiles.Confirm = false;
            businessCardFiles.Create = DateTime.Now;

            BusinessCards businessCards = new BusinessCards();
            businessCards = db.BusinessCards.Find(businessCardFiles.BusinessCard_Id);
            businessCards.Status_Id = 2;
            businessCards.Create = DateTime.Now;

            if (db.SaveChanges() > 0)
            {
                dataCard.SendMail(businessCards,null, businessCardFiles,"", remark);
                dataCard.BusinessCard_SaveLog(businessCards);
            }

            return View();
        }
        public ActionResult UserConfirmApprove(Guid? id)
        {
            BusinessCardFiles businessCardFiles = new BusinessCardFiles();
            businessCardFiles = db.BusinessCardFiles.Find(id);
            businessCardFiles.Confirm = true;
            businessCardFiles.Create = DateTime.Now;

            BusinessCards businessCards = new BusinessCards();
            businessCards = db.BusinessCards.Find(businessCardFiles.BusinessCard_Id);
            businessCards.Status_Id = 9;
            businessCards.Create = DateTime.Now;

            if (db.SaveChanges() > 0)
            {
                dataCard.SendMail(businessCards);
                dataCard.BusinessCard_SaveLog(businessCards);
            }

            return View();
        }
        public ActionResult StaffComplete(Guid? id)
        {
            BusinessCards businessCards = new BusinessCards();
            businessCards = db.BusinessCards.Find(id);
            businessCards.Status_Id = 3;
            businessCards.Create = DateTime.Now;

            if (db.SaveChanges() > 0)
            {
                dataCard.SendMail(businessCards);
                dataCard.BusinessCard_SaveLog(businessCards);
            }

            return View();
        }
        public ActionResult UserClose(Guid? id)
        {
            BusinessCards businessCards = new BusinessCards();
            businessCards = db.BusinessCards.Find(id);
            businessCards.Status_Id = 4;
            businessCards.Create = DateTime.Now;

            if (db.SaveChanges() > 0)
            {
                dataCard.SendMail(businessCards);
                dataCard.BusinessCard_SaveLog(businessCards);
            }

            return View();
        }
        public ActionResult Cancel(Guid? id)
        {
            BusinessCards businessCards = new BusinessCards();
            businessCards = db.BusinessCards.Find(id);
            businessCards.Status_Id = 6;
            businessCards.Create = DateTime.Now;

            if (db.SaveChanges() > 0)
            {
                dataCard.BusinessCard_SaveLog(businessCards);
            }

            return View();
        }
        public ActionResult BusinessCard_UploadFile(Guid id)
        {
            return View(id);
        }



    }
}