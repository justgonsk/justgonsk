using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JustGo.Helpers
{
    /// <summary>
    /// Атрибут фильтра, отлавливающий все исключения и отправляющий в ответе ошибку.
    /// </summary>
    ///
    /// <remarks>
    /// В реальном приложении не очень круто отправлять stacktrace
    /// Здесь это сделано в диагностических целях
    /// </remarks>
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