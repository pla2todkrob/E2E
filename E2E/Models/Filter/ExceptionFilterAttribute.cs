using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E2E.Models.Filter
{
    public class ExceptionFilterAttribute : HandleErrorAttribute
    {
        private void LogException(Exception exception, ExceptionContext filterContext)
        {
            using (var context = new ClsContext())
            {
                var exceptionLog = new Log_Exception()
                {
                    ExceptionMessage = exception.Message,
                    ControllerName = filterContext.RouteData.Values["controller"].ToString(),
                    ActionName = filterContext.RouteData.Values["action"].ToString(),
                    TimeStamp = DateTime.Now
                };

                context.Log_Exceptions.Add(exceptionLog);
                context.SaveChanges();

                if (exception.InnerException != null)
                {
                    LogException(exception.InnerException, filterContext);
                }
            }
        }

        public override void OnException(ExceptionContext filterContext)
        {
            // Log the exception
            LogException(filterContext.Exception, filterContext);

            // Show the error page
            filterContext.Result = new ViewResult
            {
                ViewName = "Error"
            };

            filterContext.ExceptionHandled = true;
        }
    }
}
