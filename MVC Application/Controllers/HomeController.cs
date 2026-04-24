using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC_Application.Models;
using Microsoft.AspNetCore.Authorization;

namespace MVC_Application.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("TRAINING_COORDINATOR"))
                    return RedirectToAction("Dashboard", "TrainingCoordinator");

                if (User.IsInRole("INSTRUCTOR"))
                    return RedirectToAction("Dashboard", "Instructor");

                if (User.IsInRole("TRAINEE"))
                    return RedirectToAction("Dashboard", "Trainee");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
