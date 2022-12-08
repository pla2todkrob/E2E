using E2E.Models;
using System;
using System.Linq;
using System.Web.Http;

namespace E2E.Controllers
{
    public class GetController : ApiController
    {
        private readonly ClsContext db = new ClsContext();

        public ClsApi GetAllUser()
        {
            ClsApi clsApi = new ClsApi();
            try
            {
                clsApi.IsSuccess = true;
                clsApi.Value = db.UserDetails.ToList();
            }
            catch (Exception ex)
            {
                clsApi.IsSuccess = false;
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
