using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace E2E.Controllers
{
    public class UsersController : Controller
    {
        private clsManageMaster data = new clsManageMaster();
        private clsContext db = new clsContext();

        public ActionResult _ShowChangePassword()
        {
            bool res = new bool();
            var id = Guid.Parse(System.Web.HttpContext.Current.User.Identity.Name);
            var Password = db.UserDetails.Where(w => w.User_Id == id).Select(s => s.Detail_Password).FirstOrDefault();

            if (!string.IsNullOrEmpty(Password))
            {
                res = true;
            }

            return PartialView("_ShowChangePassword", res);
        }

        public ActionResult _UploadHistory()
        {
            return PartialView("_UploadHistory", db.UserUploadHistories.OrderByDescending(o => o.Create).ToList());
        }

        public ActionResult _UserInfomation(string val)
        {
            try
            {
                return Json(data.clsUsers_GetView(val), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult ChangePassword()
        {
            try
            {
                clsPassword clsPassword = new clsPassword();
                clsPassword.User_Id = Guid.Parse(System.Web.HttpContext.Current.User.Identity.Name);

                return View(clsPassword);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(clsPassword model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.SaveChangePassword(model))
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
                            swal.text = "รหัสผ่านเก่าไม่ถูกต้อง";
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

        // GET: Users
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            try
            {
                ViewBag.ReturnUrl = Request.QueryString["ReturnUrl"];
                if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                    {
                        return Redirect(Request.QueryString["ReturnUrl"]);
                    }
                    else
                    {
                        return Redirect(FormsAuthentication.DefaultUrl);
                    }
                }

                return View(new clsLogin());
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(clsLogin model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Users users = new Users();
                    users = db.Users
                        .Where(w => w.User_Code == model.Username.Trim() || w.User_Email == model.Username.Trim())
                        .FirstOrDefault();
                    if (users == null)
                    {
                        ModelState.AddModelError("Username", string.Format("Username {0} not found", model.Username));
                        return View(model);
                    }

                    string password = db.UserDetails
                        .Where(w => w.User_Id == users.User_Id)
                        .Select(s => s.Detail_Password)
                        .FirstOrDefault();
                    if (string.IsNullOrEmpty(password))
                    {
                        if (data.LoginDomain(users.User_Email.Trim(), model.Password.Trim()))
                        {
                            goto SetAuthen;
                        }
                        else
                        {
                            ModelState.AddModelError("Password", "The password is incorrect.");
                            return View(model);
                        }
                    }
                    else
                    {
                        if (string.Equals(password, data.Users_Password(model.Password.Trim())))
                        {
                            goto SetAuthen;
                        }
                        else
                        {
                            ModelState.AddModelError("Password", "The password is incorrect.");
                            return View(model);
                        }
                    }

                SetAuthen:
                    Log_Login log_Login = new Log_Login();
                    log_Login.User_Id = users.User_Id;
                    db.Entry(log_Login).State = System.Data.Entity.EntityState.Added;
                    if (db.SaveChanges() > 0)
                    {
                        int year = DateTime.Today.Year;
                        if (!int.Equals(users.YearSetPoint, year))
                        {
                            clsDefaultSystem.Generate();
                        }
                        FormsAuthentication.SetAuthCookie(users.User_Id.ToString(), model.Remember);
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return Redirect(FormsAuthentication.DefaultUrl);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return View(model);
        }

        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            return Redirect(FormsAuthentication.DefaultUrl);
        }
    }
}