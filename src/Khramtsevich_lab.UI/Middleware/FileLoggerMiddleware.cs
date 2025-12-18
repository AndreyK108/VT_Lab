using Microsoft.AspNetCore.Http;
using Serilog;
using System.Threading.Tasks;

namespace Khramtsevich_lab.UI.Middleware
{
    public class FileLoggerMiddleware
    {
        private readonly RequestDelegate _next;

        public FileLoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            var statusCode = context.Response.StatusCode;

            // логируем только НЕ 2xx
            if (statusCode < 200 || statusCode >= 300)
            {
                var path = context.Request.Path + context.Request.QueryString;
                Log.Information("---> request {Path} returns {StatusCode}", path, statusCode);
            }
        }
    }
}
