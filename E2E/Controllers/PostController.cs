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
        private readonly ClsApi clsApi = new ClsApi();
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
                    clsApi.Message = ex.GetBaseException().Message;
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
            if (ModelState.IsValid)
            {
                try
                {
                    string passEncrypt = master.Users_Password(model.Password);
                    Users users = new Users();
                    users = db.Users
                        .Where(w => w.User_Code == model.Username || w.User_Email == model.Username || w.Username == model.Username)
                        .FirstOrDefault();
                    if (users != null)
                    {
                        if (!users.Active)
                        {
                            new Exception("This account has been suspended.\nPlease contact the system administrator.");
                        }
                        if (string.IsNullOrEmpty(users.Username))
                        {
                            ClsActiveDirectoryInfo adInfo = master.GetAdInfo(users.User_Code);
                            if (!string.IsNullOrEmpty(adInfo.SamAccountName))
                            {
                                users.Username = adInfo.SamAccountName;
                                users.User_Email = adInfo.EmailAddress;
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
                            else
                            {
                                throw new Exception("Username not found");
                            }
                        }
                        else if (string.Equals(password, passEncrypt))
                        {
                            goto LoginPass;
                        }
                        else
                        {
                            throw new Exception("Password is incorrect");
                        }
                    }
                    else
                    {
                        throw new Exception("Username not found");
                    }
                LoginPass:
                    clsApi.Value = users.User_Id;
                    clsApi.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    Exception inner = ex.InnerException;
                    clsApi.Message = ex.GetBaseException().Message;
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
        public ClsApi GetUserData(Guid[] ids)
        {
            try
            {
                clsApi.Value = db.Users
                    .Where(w => ids.Contains(w.User_Id))
                    .Join(db.UserDetails,
                    u => u.User_Id,
                    ud => ud.User_Id,
                    (u, ud) => new UserResponse()
                    {
                        Users = u,
                        FirstName = ud.Detail_EN_FirstName,
                        LastName = ud.Detail_EN_LastName
                    }).ToList();
                clsApi.IsSuccess = true;
            }
            catch (Exception ex)
            {
                Exception inner = ex.InnerException;
                clsApi.Message = ex.GetBaseException().Message;
            }

            return clsApi;
        }
    }
}
