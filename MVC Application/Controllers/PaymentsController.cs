using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCertificationPlatform;
using TrainingCertificationPlatform.Models;
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
            // If the user is not a training coordinator, forbid access to this page
            if (!User.IsInRole(UserRole.TRAINING_COORDINATOR.ToString()))
            {
                return Forbid();
            }

            //flag overdue balances before displaying the list
            await _paymentTrackingService.FlagOverdueBalancesAsync();

            // Get list of courses for filter dropdown
            ViewBag.Courses = await _context.Courses
                .OrderBy(c => c.Title)
                .ToListAsync();

            // Preserve selected filters in the view
            ViewBag.SelectedCourseId = courseId;

            // Validate and preserve selected balance statuses or set to empty list if null
            ViewBag.SelectedBalanceStatuses = balanceStatuses ?? new List<string>();

            // Get list of balance statuses for filter 
            ViewBag.BalanceStatuses = Enum.GetValues(typeof(BalanceStatus))
                .Cast<BalanceStatus>()
                .ToList();

            ViewBag.EnrollmentDroppedStatus = EnrollmentStatus.DROPPED;

            // Build the query to get payments with related data
            var paymentsQuery = _context.Payments
                .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Trainee)
                .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Session)
                        .ThenInclude(s => s.Course)
                .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Balance)
                .AsQueryable();

            // Apply course filter if selected
            if (courseId.HasValue)
            {
                paymentsQuery = paymentsQuery
                    .Where(p => p.Enrollment.Session.CourseId == courseId.Value);
            }

            // Apply balance status filter if selected
            if (balanceStatuses != null && balanceStatuses.Any())
            {
                var selectedStatuses = balanceStatuses
                    // Filter out any invalid status values
                    .Where(s => Enum.TryParse<BalanceStatus>(s, out _))
                    // Convert the valid status strings to BalanceStatus enum values
                    .Select(s => Enum.Parse<BalanceStatus>(s))
                    .ToList(); // Convert to list to avoid multiple enumeration

                // Filter payments where the related enrollment's balance status is in the selected statuses
                paymentsQuery = paymentsQuery
                    .Where(p => p.Enrollment.Balance != null &&
                                selectedStatuses.Contains(p.Enrollment.Balance.Status));
            }

            // Order the payments by date descending and execute the query
            var payments = await paymentsQuery
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return View(payments);
        }


        [HttpGet]
        [Authorize(Roles = "TRAINING_COORDINATOR,TRAINEE")]
        public async Task<IActionResult> Create(int? courseId, int? enrollmentId, string? returnUrl)
        {
            // Get the current user's ID and role from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdClaim);

            // Preserve the return URL and selected filters in the view
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.SelectedCourseId = courseId;
            ViewBag.SelectedEnrollmentId = enrollmentId;

            // Get unpaid enrollments with related data
            var unpaidEnrollmentsQuery = _context.Enrollments
                .Include(e => e.Trainee)
                .Include(e => e.Balance)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Where(e => e.Balance != null && e.Balance.AmountDue > 0);

            if (role == "TRAINEE")
            {
                // Trainees can only see their own unpaid enrollments
                unpaidEnrollmentsQuery = unpaidEnrollmentsQuery
                    .Where(e => e.TraineeId == userId);
            }
            else if (role != "TRAINING_COORDINATOR")
            {
                // Only training coordinators can access this page, so if the user is not a trainee or coordinator, forbid access
                return Forbid();
            }

            // Execute the query to get the list of unpaid enrollments
            var unpaidEnrollments = await unpaidEnrollmentsQuery.ToListAsync();

            // Get distinct list of courses from the unpaid enrollments for the filter dropdown
            ViewBag.Courses = unpaidEnrollments
                .Select(e => e.Session.Course)
                .DistinctBy(c => c.Id)
                .ToList();

            // Filter the enrollments by selected course if a course is selected
            if (courseId.HasValue)
            {
                // Only show enrollments for the selected course
                ViewBag.Enrollments = unpaidEnrollments
                    .Where(e => e.Session.CourseId == courseId.Value)
                    .ToList();
            }
            else
            {
                // If no course is selected, show all unpaid enrollments
                ViewBag.Enrollments = unpaidEnrollments;
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "TRAINING_COORDINATOR,TRAINEE")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int courseId, int enrollmentId, decimal amount, string? returnUrl)
        {
            //Validation:
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdClaim);

            // Check if the user is allowed to pay for this enrollment
            bool isAllowedToPay = false;

            if (role == "TRAINEE")
            {
                // Trainees can only pay for their own enrollments
                isAllowedToPay = await _context.Enrollments
                    .AnyAsync(e =>
                        e.Id == enrollmentId &&
                        e.TraineeId == userId &&
                        e.Session.CourseId == courseId);
            }
            else if (role == "TRAINING_COORDINATOR")
            {
                // Training coordinators can pay for any enrollment, but we still check if the enrollment exists for the specified course
                isAllowedToPay = await _context.Enrollments
                    .AnyAsync(e =>
                        e.Id == enrollmentId &&
                        e.Session.CourseId == courseId);
            }

            // If the user is not allowed to pay for this enrollment, redirect back with an error message
            if (!isAllowedToPay)
            {
                TempData["ErrorMessage"] = "You are not allowed to pay for this enrollment.";
                return RedirectToAction(nameof(Create));
            }

            // Call the payment tracking service to record the payment and get any error message
            var error = await _paymentTrackingService.RecordPaymentAsync(enrollmentId, amount);

            // If there was an error recording the payment, redirect back to the create page with the error message
            if (error != null)
            {
                TempData["ErrorMessage"] = error;
                return RedirectToAction(nameof(Create), new { courseId, enrollmentId, returnUrl });
            }


            
            var paidEnrollment = await _context.Enrollments
            .Include(e => e.Balance)
            .FirstOrDefaultAsync(e => e.Id == enrollmentId);

            if (paidEnrollment != null &&
                paidEnrollment.Balance.AmountDue <= 0)
            {
                paidEnrollment.Status = EnrollmentStatus.CONFIRMED;

                await _context.SaveChangesAsync();
            }


            TempData["SuccessMessage"] = "Payment recorded successfully";

            // Notify the trainee if the payment was made by the coordinator
            if (role == "TRAINING_COORDINATOR") {
                // Fetch the enrollment with related trainee and course information to include in the notification message
                var enrollment = await _context.Enrollments
                        .Include(e => e.Trainee)
                        .Include(e => e.Session)
                            .ThenInclude(s => s.Course)
                        .FirstOrDefaultAsync(e =>
                            e.Id == enrollmentId &&
                            e.Session.CourseId == courseId);

                if (enrollment != null)
                {
                    var message = $"A payment of BHD {amount:C} has been recorded for your course {enrollment.Session.Course.Title}.";
                    // Create a notification for the trainee about the payment
                    await _notificationService.CreateNotificationAsync(enrollment.TraineeId, message);
                }
            }

            // After recording the payment, redirect back to the return URL if it's valid,
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // otherwise redirect to the payments index page for the enrollment
            return RedirectToAction(nameof(Index), new { enrollmentId });
        }
    }
}
