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

        public async Task<IActionResult> Create(int? enrollmentId, string? returnUrl) {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int traineeId = int.Parse(userIdClaim);

            ViewBag.ReturnUrl = returnUrl;

            ViewBag.Enrollments = await _context.Enrollments
                .Include(e => e.Trainee)
                .Include(e => e.Session)
                .ThenInclude(s => s.Course)
                .Where(e => e.TraineeId == traineeId && e.Balance != null  && e.Balance.AmountDue > 0)
                .ToListAsync();

            ViewBag.SelectedEnrollmentId = enrollmentId;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( int enrollmentId, decimal amount, string? returnUrl)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int traineeId = int.Parse(userIdClaim);

            var enrollmentBelongsToTrainee = await _context.Enrollments
                .AnyAsync(e => e.Id == enrollmentId && e.TraineeId == traineeId);

            if (!enrollmentBelongsToTrainee)
            {
                TempData["ErrorMessage"] = "You can only pay for your own enrollment";
                return RedirectToAction(nameof(Create));
            }

            var error = await _paymentTrackingService.RecordPaymentAsync(enrollmentId, amount);

            if (error != null)
            {
                TempData["ErrorMessage"] = error;
                return RedirectToAction(nameof(Create));
            }

            TempData["SuccessMessage"] = "Payment recorded successfully";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Re-populate the enrollments list
            ViewBag.Enrollments = await _context.Enrollments
                .Include(e => e.Trainee)
                .Include(e => e.Session)
                .ThenInclude(s => s.Course)
                .Where(e => e.TraineeId == traineeId && e.Balance != null && e.Balance.AmountDue > 0)
                .ToListAsync();

            ViewBag.SelectedEnrollmentId = enrollmentId;

            return RedirectToAction(nameof(Create), new
            {
                enrollmentId
            });
        }
    }
}
