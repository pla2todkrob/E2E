using E2E.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class BaseController : Controller, IDisposable
    {
        protected int authId;
        protected ClsContext db;
        protected Guid loginId;

        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            db = new ClsContext();
            if (HttpContext != null && HttpContext.User.Identity.IsAuthenticated)
            {
                loginId = Guid.Parse(HttpContext.User.Identity.Name);
                authId = db.Users
                .Where(w => w.User_Id == loginId)
                .Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id)
                .FirstOrDefault();
            }
            
            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
