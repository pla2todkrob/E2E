using System.IO.Compression;
using System.Web.Mvc;

namespace E2E.Models.Filter
{
    public class CompressionFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var response = filterContext.HttpContext.Response;
            var acceptEncoding = request.Headers["Accept-Encoding"];

            if (string.IsNullOrEmpty(acceptEncoding)) return;

            acceptEncoding = acceptEncoding.ToLower();

            if (acceptEncoding.Contains("gzip"))
            {
                if (response.Filter == null)
                {
                    response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                }
                response.AppendHeader("Content-Encoding", "gzip");
            }
            else if (acceptEncoding.Contains("deflate"))
            {
                if (response.Filter == null)
                {
                    response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                }
                response.AppendHeader("Content-Encoding", "deflate");
            }
        }
    }
}
