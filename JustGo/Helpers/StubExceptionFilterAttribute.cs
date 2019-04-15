using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JustGo.Helpers
{
    //это нужно, чтобы не нагромождать логику контроллеров блоками try/catch
    internal class StubExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            await context.HttpContext.Response.WriteAsync(
                $"Something went wrong: {context.Exception.Message}\n"
                + $"Stacktrace: {context.Exception.StackTrace} ");
        }
    }
}