using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCertificationPlatform;
using TrainingCertificationPlatform.Services;
using System.Security.Claims;

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

        public async Task<IActionResult> Index() { 
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

        public async Task<IActionResult> Create(int? enrollmentId) {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int traineeId = int.Parse(userIdClaim);

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
        public async Task<IActionResult> Create( int enrollmentId, decimal amount)
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

            TempData["SuccessMessage"] = "Payment recorded syccessfully";
            return RedirectToAction(nameof(Create), new { enrollmentId });
        }

       
    }
}
