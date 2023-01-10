using E2E.Models;
using E2E.Models.Filter;
using System.Web.Mvc;

namespace E2E
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthorizeAttribute());
            filters.Add(new TimingFilterAttribute());
            filters.Add(new ExceptionFilterAttribute());
            filters.Add(new ClearCacheFilterAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
