using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Excel.File.Service.Middleware
{
    public class ExcelFileServiceMiddleware
    {
        readonly RequestDelegate _next;

        public ExcelFileServiceMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
            => await _next.Invoke(context);
    }
}
