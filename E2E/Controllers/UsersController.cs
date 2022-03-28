using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace E2E.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private clsContext db = new clsContext();
        private clsManageData data = new clsManageData();
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
                if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                    {
                        Response.Redirect(Request.QueryString["ReturnUrl"]);
                    }
                    else
                    {
                        Response.Redirect(FormsAuthentication.DefaultUrl);
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
        public ActionResult Login(clsLogin model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Users users = new Users();
                    users = db.Users
                        .Where(w => w.User_Code == model.Username.Trim())
                        .FirstOrDefault();
                    if (users == null)
                    {
                        users = new Users();
                        users = db.Users
                            .Where(w => w.User_Email == model.Username.Trim())
                            .FirstOrDefault();
                        if (users == null)
                        {
                            ModelState.AddModelError("Username", string.Format("Username {0} not found", model.Username));
                            return View(model);
                        }
                    }

                    string password = db.UserDetails
                        .Where(w => w.User_Id == users.User_Id)
                        .Select(s => s.Detail_Password)
                        .FirstOrDefault();
                    if (string.IsNullOrEmpty(password))
                    {
                        if (LoginDomain(users.User_Email.Trim(), model.Password.Trim()))
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
                        if (string.Equals(password, data.User_Password(model.Password.Trim())))
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
                    if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                    {
                        FormsAuthentication.SetAuthCookie(users.User_Id.ToString(), model.Remember);
                        Response.Redirect(Request.QueryString["ReturnUrl"]);
                    }
                    else
                    {
                        FormsAuthentication.RedirectFromLoginPage(users.User_Id.ToString(), model.Remember);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

            return View(model);
        }

        private bool LoginDomain(string email, string password)
        {
            try
            {
                bool res = new bool();
                string domainName = ConfigurationManager.AppSettings["DomainName"];
                using (var context = new PrincipalContext(ContextType.Domain, domainName))
                {
                    res = context.ValidateCredentials(email, password);
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