using System.Net;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Nanoka.Web.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("error")]
        public Result Handle()
        {
            var builder = new StringBuilder()
               .Append("An internal server error caused this request to fail.");

            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

            if (exception != null)
                builder.AppendLine()
                       .Append("Message: ")
                       .AppendLine(exception.Message ?? "<null>")
                       .Append("Trace: ")
                       .Append(exception.StackTrace.Substring(0, exception.StackTrace.IndexOf('\n')).Trim());

            return Result.InternalServerError(builder.ToString());
        }

        [Route("error/{status}")]
        public Result Handle(HttpStatusCode status)
            => Result.StatusCode(status, null);
    }
}