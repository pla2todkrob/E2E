using System.Diagnostics;
using System.Web.Mvc;

namespace E2E.Models.Filter
{
    public class TimingFilterAttribute : ActionFilterAttribute
    {
        private Stopwatch stopwatch;

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            stopwatch.Stop();
            Debug.WriteLine("Load {0} in {1} milliseconds.", filterContext.ActionDescriptor.ActionName, stopwatch.ElapsedMilliseconds);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            stopwatch = Stopwatch.StartNew();
        }
    }
}
