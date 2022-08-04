using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace E2E.Controllers
{
    public class PostController : ApiController
    {
        private clsContext db = new clsContext();
        private clsManageMaster master = new clsManageMaster();

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