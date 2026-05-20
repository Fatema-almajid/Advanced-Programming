using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC_Application.Models;
using Microsoft.AspNetCore.Authorization;

/* 
 * This component acts as the centralized landing routing mechanism and root controller for the application ecosystem.
 * It implements a dynamic role-based redirection workflow within the landing `Index` action, evaluationally intercepts authenticated HTTP context identities,
 * and programmatically routes distinct operational profiles ("TRAINING_COORDINATOR", "INSTRUCTOR", and "TRAINEE") to their respective domain-specific management dashboards.
 * Additionally, the infrastructure establishes built-in error diagnostics and operational tracing by integrating automated platform logging (`ILogger`),
 * and configures standard `ResponseCache` control attributes to strictly prevent sensitive diagnostic data caching during server-side application anomalies.
 */

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
