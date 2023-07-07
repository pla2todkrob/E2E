using E2E.Models;
using System;
using System.Linq;
using System.Web.Http;

namespace E2E.Controllers
{
    public class GetController : ApiController
    {
        private readonly ClsApi clsApi = new ClsApi();
        private readonly ClsContext db = new ClsContext();

        public ClsApi GetAllUser()
        {
            try
            {
                clsApi.IsSuccess = true;
                clsApi.Value = db.UserDetails.ToList();
            }
            catch (Exception ex)
            {
                clsApi.IsSuccess = false;
                clsApi.Message = ex.Message;
                Exception inner = ex.InnerException;

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
