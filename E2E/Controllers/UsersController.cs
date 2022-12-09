﻿using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Security;

namespace E2E.Controllers
{
    public class UsersController : Controller
    {
        private readonly ClsManageMaster data = new ClsManageMaster();
        private readonly ClsContext db = new ClsContext();

        public ActionResult ChangePassword()
        {
            try
            {
                ClsPassword clsPassword = new ClsPassword
                {
                    User_Id = Guid.Parse(System.Web.HttpContext.Current.User.Identity.Name)
                };

                return View(clsPassword);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ClsPassword model)
        {
            ClsSwal swal = new ClsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.SaveChangePassword(model))
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
                            swal.Text = "รหัสผ่านเก่าไม่ถูกต้อง";
                            swal.Title = "Warning";
                        }
                    }
                    catch (DbEntityValidationException ex)
                    {
                        swal.Title = ex.TargetSite.Name;
                        foreach (var item in ex.EntityValidationErrors)
                        {
                            foreach (var item2 in item.ValidationErrors)
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
                    catch (Exception ex)
                    {
                        swal.Title = ex.TargetSite.Name;
                        swal.Text = ex.Message;
                        if (ex.InnerException != null)
                        {
                            swal.Text = ex.InnerException.Message;
                            if (ex.InnerException.InnerException != null)
                            {
                                swal.Text = ex.InnerException.InnerException.Message;
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

                return View(new ClsLogin());
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(ClsLogin model, string returnUrl)
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

                    if (data.LoginDomain(users.User_Email.Trim(), model.Password.Trim()))
                    {
                        goto SetAuthen;
                    }
                    else
                    {
                        string password = db.UserDetails
                        .Where(w => w.User_Id == users.User_Id)
                        .Select(s => s.Detail_Password)
                        .FirstOrDefault();

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
                            ClsDefaultSystem.Generate();
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

        public ActionResult ShowChangePassword()
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

        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            return Redirect(FormsAuthentication.DefaultUrl);
        }

        public ActionResult UploadHistory()
        {
            return PartialView("_UploadHistory", db.UserUploadHistories.OrderByDescending(o => o.Create).ToList());
        }

        public ActionResult UserInfomation(string val)
        {
            try
            {
                return Json(data.ClsUsers_GetView(val), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
