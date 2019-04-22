using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Memorizer.Web.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();

            ViewData["statusCode"] = HttpContext.Response.StatusCode;
            ViewData["message"] = exception.Error.Message;

            return View("Error");
        }

        [Route("Error/404")]
        public IActionResult Error404()
        {
            return View();
        }

        [Route("Error/{code:int}")]
        public IActionResult Error(int code)
        {
            ViewData["statusCode"] = code;

            return View();
        }
    }
}