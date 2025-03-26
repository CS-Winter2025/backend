using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/404")]
        public IActionResult NotFound()
        {
            return View("~/Views/Shared/404.cshtml");
        }

        [Route("Error/403")]
        public IActionResult Forbidden()
        {
            return View("~/Views/Shared/403.cshtml");
        }
    }
}
