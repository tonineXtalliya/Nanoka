using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Nanoka.Core.Controllers
{
    // https://medium.com/@matteocontrini/consistent-error-responses-in-asp-net-core-web-apis-bb70b435d1f8
    [ApiController]
    [Route("errors")]
    public class ErrorController : ControllerBase
    {
        [Route("{code}")]
        public ActionResult Error(int code) => Result.StatusCode((HttpStatusCode) code, null);
    }
}