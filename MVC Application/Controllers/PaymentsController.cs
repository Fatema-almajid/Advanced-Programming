using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCertificationPlatform;
using TrainingCertificationPlatform.Models;
using TrainingCertificationPlatform.Services;
using MVC_Application.Services;

namespace MVC_Application.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PaymentTrackingService _paymentTrackingService;
        private readonly NotificationService _notificationService;

        public PaymentsController(AppDbContext context, PaymentTrackingService paymentTrackingService, NotificationService notificationService)
        {
            _context = context;
            _paymentTrackingService = paymentTrackingService;
            _notificationService = notificationService;
        }

        [Authorize(Roles = "TRAINING_COORDINATOR")]
        public async Task<IActionResult> Index(int? courseId, List<string>? balanceStatuses)
        {
            if (!User.IsInRole(UserRole.TRAINING_COORDINATOR.ToString()))
            {
                return Forbid();
            }

            await _paymentTrackingService.FlagOverdueBalancesAsync();

            ViewBag.Courses = await _context.Courses
                .OrderBy(c => c.Title)
                .ToListAsync();

            ViewBag.SelectedCourseId = courseId;

            ViewBag.SelectedBalanceStatuses = balanceStatuses ?? new List<string>();

            ViewBag.BalanceStatuses = Enum.GetValues(typeof(BalanceStatus))
                .Cast<BalanceStatus>()
                .ToList();


            var paymentsQuery = _context.Payments
                .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Trainee)
                .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Session)
                        .ThenInclude(s => s.Course)
                .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Balance)
                .AsQueryable();

            if (courseId.HasValue)
            {
                paymentsQuery = paymentsQuery
                    .Where(p => p.Enrollment.Session.CourseId == courseId.Value);
            }

            if (balanceStatuses != null && balanceStatuses.Any())
            {
                var selectedStatuses = balanceStatuses
                    .Where(s => Enum.TryParse<BalanceStatus>(s, out _))
                    .Select(s => Enum.Parse<BalanceStatus>(s))
                    .ToList();

                paymentsQuery = paymentsQuery
                    .Where(p => p.Enrollment.Balance != null &&
                                selectedStatuses.Contains(p.Enrollment.Balance.Status));
            }

            var payments = await paymentsQuery
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return View(payments);
        }


        [HttpGet]
        [Authorize(Roles = "TRAINING_COORDINATOR, TRAINEE")]
        public async Task<IActionResult> Create(int? courseId, int? enrollmentId, string? returnUrl)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdClaim);

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.SelectedCourseId = courseId;
            ViewBag.SelectedEnrollmentId = enrollmentId;

            var unpaidEnrollmentsQuery = _context.Enrollments
                .Include(e => e.Trainee)
                .Include(e => e.Balance)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Where(e => e.Balance != null && e.Balance.AmountDue > 0);

            if (role == "TRAINEE")
            {
                unpaidEnrollmentsQuery = unpaidEnrollmentsQuery
                    .Where(e => e.TraineeId == userId);
            }
            else if (role != "TRAINING_COORDINATOR")
            {
                return Forbid();
            }

            var unpaidEnrollments = await unpaidEnrollmentsQuery.ToListAsync();

            ViewBag.Courses = unpaidEnrollments
                .Select(e => e.Session.Course)
                .DistinctBy(c => c.Id)
                .ToList();

            if (courseId.HasValue)
            {
                ViewBag.Enrollments = unpaidEnrollments
                    .Where(e => e.Session.CourseId == courseId.Value)
                    .ToList();
            }
            else
            {
                ViewBag.Enrollments = new List<Enrollment>();
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "TRAINING_COORDINATOR, TRAINEE")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int courseId, int enrollmentId, decimal amount, string? returnUrl)
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
                    .AnyAsync(e =>
                        e.Id == enrollmentId &&
                        e.TraineeId == userId &&
                        e.Session.CourseId == courseId);
            }
            else if (role == "TRAINING_COORDINATOR")
            {
                isAllowedToPay = await _context.Enrollments
                    .AnyAsync(e =>
                        e.Id == enrollmentId &&
                        e.Session.CourseId == courseId);
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

            // Notify the trainee if the payment was made by the coordinator
            if (role == "TRAINING_COORDINATOR") {
                var enrollment = await _context.Enrollments
                        .Include(e => e.Trainee)
                        .Include(e => e.Session)
                            .ThenInclude(s => s.Course)
                        .FirstOrDefaultAsync(e =>
                            e.Id == enrollmentId &&
                            e.Session.CourseId == courseId);

                if (enrollment != null)
                {
                    var message = $"A payment has been recorded for your course {enrollment.Session.Course.Title}.";
                    await _notificationService.CreateNotificationAsync(enrollment.TraineeId, message);
                }
            }

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index), new { enrollmentId });
        }
    }
}
