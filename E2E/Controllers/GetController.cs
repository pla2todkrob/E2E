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
            try
            {
                clsApi.isSuccess = true;
                clsApi.Value = db.UserDetails.ToList();
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
