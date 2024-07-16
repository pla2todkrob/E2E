using E2E.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class BaseController : Controller
    {
        protected Guid loginId;
        protected int authId;
        protected ClsContext db;

        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (HttpContext != null && HttpContext.User.Identity.IsAuthenticated)
            {
                loginId = Guid.Parse(HttpContext.User.Identity.Name);
            }
            db = new ClsContext();
            authId = db.Users
                .Where(w => w.User_Id == loginId)
                .Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id)
                .FirstOrDefault();
        }
    }
}
