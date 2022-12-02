using E2E.Models;
using System;
using System.Linq;
using System.Web.Http;

namespace E2E.Controllers
{
    public class GetController : ApiController
    {
        private readonly clsContext db = new clsContext();

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
