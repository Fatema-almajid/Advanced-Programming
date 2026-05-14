using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCertificationPlatform;
using TrainingCertificationPlatform.Models;
using TrainingCertificationPlatform.Services;

namespace MVC_Application.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PaymentTrackingService _paymentTrackingService;

        public PaymentsController(AppDbContext context, PaymentTrackingService paymentTrackingService)
        {
            _context = context;
            _paymentTrackingService = paymentTrackingService;
        }

        [Authorize(Roles = "TRAINING_COORDINATOR,INSTRUCTOR")]
        public async Task<IActionResult> Index() { 

            if (User.IsInRole(UserRole.INSTRUCTOR.ToString()))
            {
                ViewBag.Role = "Instructor";
            }
            else if (User.IsInRole(UserRole.TRAINING_COORDINATOR.ToString())) { 
                ViewBag.Role = "TrainingCoordinator";
            }
            else
            {
                // If the user is authenticated but does not have the required role, show an error or redirect
                return Forbid();
            }
                await _paymentTrackingService.FlagOverdueBalancesAsync();

            var payments = await _context.Payments
                .Include(p => p.Enrollment)
                .ThenInclude(e => e.Trainee)
                .Include(p => p.Enrollment)
                .ThenInclude(e => e.Session)
                .ThenInclude(s => s.Course)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return View(payments);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? enrollmentId, string? returnUrl)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdClaim);

            ViewBag.ReturnUrl = returnUrl;

            var enrollmentsQuery = _context.Enrollments
                .Include(e => e.Trainee)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Where(e => e.Balance != null && e.Balance.AmountDue > 0);

            if (role == "TRAINEE")
            {
                enrollmentsQuery = enrollmentsQuery
                    .Where(e => e.TraineeId == userId);
            }
            else if ( role == "TRAINING_COORDINATOR")
            {
                // Coordinator can see all trainees with balance due
                enrollmentsQuery = enrollmentsQuery;
            }
            else
            {
                TempData["ErrorMessage"] = "You are not allowed to record payments.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Enrollments = await enrollmentsQuery.ToListAsync();
            ViewBag.SelectedEnrollmentId = enrollmentId;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int enrollmentId, decimal amount, string? returnUrl)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdClaim);

            bool isAllowedToPay = false;

            if (role == "TRAINEE")
            {
                isAllowedToPay = await _context.Enrollments
                    .AnyAsync(e => e.Id == enrollmentId && e.TraineeId == userId);
            }
            else if (role == "TRAINING_COORDINATOR")
            {
                isAllowedToPay = await _context.Enrollments
                    .AnyAsync(e => e.Id == enrollmentId);
            }

            if (!isAllowedToPay)
            {
                TempData["ErrorMessage"] = "You are not allowed to pay for this enrollment.";
                return RedirectToAction(nameof(Create));
            }

            var error = await _paymentTrackingService.RecordPaymentAsync(enrollmentId, amount);

            if (error != null)
            {
                TempData["ErrorMessage"] = error;
                return RedirectToAction(nameof(Create), new { enrollmentId, returnUrl });
            }

            TempData["SuccessMessage"] = "Payment recorded successfully";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index), new { enrollmentId });
        }
    }
}
