using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Linq;
using System.Transactions;
using System.Web.Http;

namespace E2E.Controllers
{
    public class PostController : ApiController
    {
        private readonly ClsContext db = new ClsContext();
        private readonly ClsManageMaster master = new ClsManageMaster();

        [HttpPost]
        public ClsApi ChangePassword(ClsPassword clsPassword)
        {
            ClsApi clsApi = new ClsApi();
            if (ModelState.IsValid)
            {
                try
                {
                    Users users = new Users();
                    users = db.Users.Find(clsPassword.User_Id);
                    if (users != null)
                    {
                        string email = master.GetEmailAD(users.User_Code);
                        if (string.IsNullOrEmpty(email))
                        {
                            UserDetails userDetails = new UserDetails();
                            userDetails = db.UserDetails
                                .Where(w => w.User_Id == users.User_Id)
                                .FirstOrDefault();
                            string oldPassword = master.Users_Password(clsPassword.OldPassword);
                            string newPassword = master.Users_Password(clsPassword.NewPassword);
                            if (string.Equals(oldPassword, newPassword))
                            {
                                clsApi.Message = "Can't change password! \n Because old passwords and new passwords are the same.";
                            }
                            else
                            {
                                if (string.Equals(userDetails.Detail_Password, oldPassword))
                                {
                                    using (TransactionScope scope = new TransactionScope())
                                    {
                                        userDetails.Detail_Password = newPassword;
                                        userDetails.Detail_ConfirmPassword = newPassword;
                                        db.Entry(userDetails).State = System.Data.Entity.EntityState.Modified;
                                        if (db.SaveChanges() > 0)
                                        {
                                            scope.Complete();
                                            clsApi.IsSuccess = true;
                                            clsApi.Message = "Update password successful";
                                        }
                                    }
                                }
                                else
                                {
                                    clsApi.Message = "Can't change password! \n Because the old password is incorrect.";
                                }
                            }
                        }
                        else
                        {
                            clsApi.Message = "Can't change password! \n Because your account sync to Active Directory";
                        }
                    }
                    else
                    {
                        clsApi.Message = "User account is not found!";
                    }
                }
                catch (Exception ex)
                {
                    Exception inner = ex.InnerException;
                    clsApi.Message = ex.Message;

                    while (inner != null)
                    {
                        clsApi.Message += "\n" + inner.Message;
                        inner = inner.InnerException;
                    }
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                   .Where(y => y.Count > 0)
                                   .ToList();

                foreach (var item in errors)
                {
                    foreach (var item2 in item)
                    {
                        if (string.IsNullOrEmpty(clsApi.Message))
                        {
                            clsApi.Message = item2.ErrorMessage;
                        }
                        else
                        {
                            clsApi.Message += "\n" + item2.ErrorMessage;
                        }
                    }
                }
            }

            return clsApi;
        }

        [HttpPost]
        public ClsApi CheckLogin(ClsLogin model)
        {
            ClsApi clsApi = new ClsApi();
            if (ModelState.IsValid)
            {
                ResponseUser responseUser = new ResponseUser();
                try
                {
                    bool loginPass = new bool();
                    string passEncrypt = new ClsManageMaster().Users_Password(model.Password);
                    Users users = new Users();
                    users = db.Users
                        .Where(w => w.User_Code == model.Username || w.User_Email == model.Username || w.Username == model.Username)
                        .FirstOrDefault();
                    if (users != null)
                    {
                        if (string.IsNullOrEmpty(users.Username))
                        {
                            ClsActiveDirectoryInfo adInfo = new ClsManageMaster().GetAdInfo(users.User_Code);
                            if (!string.IsNullOrEmpty(adInfo.SamAccountName))
                            {
                                users.Username = adInfo.SamAccountName;
                                users.User_Email = adInfo.UserPrincipalName;
                                UserDetails userDetails = db.UserDetails.Where(w => w.User_Id == users.User_Id).FirstOrDefault();
                                userDetails.Detail_Password = string.Empty;
                                userDetails.Detail_ConfirmPassword = string.Empty;
                                db.SaveChanges();
                            }
                        }

                        string password = db.UserDetails
                                .Where(w => w.User_Id == users.User_Id)
                                .Select(s => s.Detail_Password)
                                .FirstOrDefault();

                        if (!string.IsNullOrEmpty(users.Username))
                        {
                            if (master.HaveAD(users.Username))
                            {
                                if (master.LoginDomain(users.Username, model.Password))
                                {
                                    goto LoginPass;
                                }
                            }
                        }
                        else if (string.Equals(password, passEncrypt))
                        {
                            goto LoginPass;
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        throw new Exception("Username not found");
                    }
                    LoginPass:
                    responseUser.Users = users;
                    var name = db.UserDetails
                        .Where(w => w.User_Id == users.User_Id)
                        .Select(s => new
                        {
                            s.Detail_EN_FirstName,
                            s.Detail_EN_LastName
                        }).FirstOrDefault();
                    responseUser.FirstName = name.Detail_EN_FirstName;
                    responseUser.LastName = name.Detail_EN_LastName;

                    clsApi.Value = responseUser;
                    clsApi.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    Exception inner = ex.InnerException;
                    clsApi.Message = ex.Message;

                    while (inner != null)
                    {
                        clsApi.Message += "\n" + inner.Message;
                        inner = inner.InnerException;
                    }
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                   .Where(y => y.Count > 0)
                                   .ToList();

                foreach (var item in errors)
                {
                    foreach (var item2 in item)
                    {
                        if (string.IsNullOrEmpty(clsApi.Message))
                        {
                            clsApi.Message = item2.ErrorMessage;
                        }
                        else
                        {
                            clsApi.Message += "\n" + item2.ErrorMessage;
                        }
                    }
                }
            }

            return clsApi;
        }
    }
}
