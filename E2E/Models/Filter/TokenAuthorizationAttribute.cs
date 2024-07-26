using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace E2E.Models.Filter
{
    public class TokenAuthorizationAttribute : AuthorizationFilterAttribute
    {
        private bool IsValidToken(string token)
        {
            ClsApi clsApi = new ClsApi();

            // Implement your token validation logic here
            return !string.IsNullOrEmpty(token) && token == clsApi.GetToken();
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Contains("Token"))
            {
                var token = actionContext.Request.Headers.GetValues("Token").FirstOrDefault();

                if (IsValidToken(token))
                {
                    // Token is valid, continue processing
                    return;
                }
            }

            // If token is not valid or missing, return unauthorized response
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Unauthorized request.");
        }
    }
}
