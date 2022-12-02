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
        private readonly clsContext db = new clsContext();
        private readonly clsManageMaster master = new clsManageMaster();

        [HttpPost]
        public clsApi ChangePassword(clsPassword clsPassword)
        {
            clsApi clsApi = new clsApi();
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
                                            clsApi.isSuccess = true;
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
                    var inner = ex.InnerException;
                    clsApi.Message = ex.Message;

                    while (inner != null)
                    {
                        inner = inner.InnerException;
                        clsApi.Message += "\n" + inner.Message;
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
        public clsApi CheckLogin(clsLogin model)
        {
            clsApi clsApi = new clsApi();
            if (ModelState.IsValid)
            {
                responseUser responseUser = new responseUser();
                try
                {
                    bool loginPass = new bool();
                    string passEncrypt = new clsManageMaster().Users_Password(model.Password);
                    Users users = new Users();
                    users = db.Users
                        .Where(w => w.User_Code == model.Username || w.User_Email == model.Username)
                        .FirstOrDefault();
                    if (users != null)
                    {
                        string password = db.UserDetails
                                .Where(w => w.User_Id == users.User_Id)
                                .Select(s => s.Detail_Password)
                                .FirstOrDefault();

                        if (!string.IsNullOrEmpty(users.User_Email))
                        {
                            if (master.LoginDomain(users.User_Email, model.Password))
                            {
                                loginPass = true;
                            }
                            else
                            {
                                if (string.Equals(password, passEncrypt))
                                {
                                    loginPass = true;
                                }
                            }
                        }
                        else
                        {
                            if (string.Equals(password, passEncrypt))
                            {
                                loginPass = true;
                            }
                        }
                    }
                    else
                    {
                        clsApi.Message = "Username not found";
                        return clsApi;
                    }

                    if (loginPass)
                    {
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
                        clsApi.isSuccess = true;
                    }
                    else
                    {
                        clsApi.Message = "Invalid password";
                        return clsApi;
                    }
                }
                catch (Exception ex)
                {
                    var inner = ex.InnerException;
                    clsApi.Message = ex.Message;

                    while (inner != null)
                    {
                        inner = inner.InnerException;
                        clsApi.Message += "\n" + inner.Message;
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
