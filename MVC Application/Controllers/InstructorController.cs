using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVC_Application.Controllers
{
    [Authorize(Roles = "INSTRUCTOR")]
    public class InstructorController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}