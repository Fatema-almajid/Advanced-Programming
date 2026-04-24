using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVC_Application.Controllers
{
    [Authorize(Roles = "TRAINEE")]
    public class TraineeController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}