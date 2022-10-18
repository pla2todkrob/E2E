using E2E.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace E2E.Controllers
{
    public class GetController : ApiController
    {
        private clsContext db = new clsContext();
        private clsManageMaster master = new clsManageMaster();

        public clsApi GetAllUser()
        {
            clsApi clsApi = new clsApi();
            List<responseUser> responseUsers = new List<responseUser>();
            try
            {
                responseUsers = db.Users
                    .AsEnumerable()
                    .Select(s => new
                    {
                        U = s,
                        UD = db.UserDetails
                        .Where(w => w.User_Id == s.User_Id)
                        .Select(s2 => new { s2.Detail_EN_FirstName, s2.Detail_EN_LastName })
                        .FirstOrDefault()
                    }).Select(s => new responseUser()
                    {
                        Users = s.U,
                        FirstName = s.UD.Detail_EN_FirstName,
                        LastName = s.UD.Detail_EN_LastName
                    }).ToList();

                clsApi.isSuccess = true;
                clsApi.Value = responseUsers;
            }
            catch (Exception ex)
            {
                clsApi.isSuccess = false;
                clsApi.Message = ex.Message;
                var inner = ex.InnerException;

                while (inner != null)
                {
                    clsApi.Message += string.Format("\n {0}", inner.Message);
                    inner = inner.InnerException;
                }
            }

            return clsApi;
        }
    }
}
