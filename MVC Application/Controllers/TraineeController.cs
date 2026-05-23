using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MVC_Application.Hubs;
using MVC_Application.Models.ViewModels;
using MVC_Application.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TrainingCertificationPlatform;
using TrainingCertificationPlatform.Models;
using static System.Collections.Specialized.BitVector32;
using QRCoder;

namespace MVC_Application.Controllers
{
    [Authorize(Roles = "TRAINEE")]
    public class TraineeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PaymentTrackingService _paymentTrackingService;
        private readonly IHubContext<EnrollmentHub> _enrollmentHub;
        private readonly NotificationService _notificationService;

        // Constructor injection gives the controller access to the database and required services.
        public TraineeController(AppDbContext context, 
            PaymentTrackingService paymentTrackingService, 
            IHubContext<EnrollmentHub> enrollmentHub,
            NotificationService notificationService)
        {
            _context = context;
            _paymentTrackingService = paymentTrackingService;
            _enrollmentHub = enrollmentHub;
            _notificationService = notificationService;
        }

        // Gets the logged-in trainee's ID from the authentication claims. so each trainee can only access their own data and dashboard.
        private int GetTraineeId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        //trainee dashboard 
        public async Task<IActionResult> Dashboard()
        {
            var traineeId = GetTraineeId();

            var model = new TraineeDashboardViewModel
            {
                CoursesEnrolled = await _context.Enrollments.CountAsync(e => e.TraineeId == traineeId && e.Status != EnrollmentStatus.DROPPED && e.Status != EnrollmentStatus.COMPLETED),
                CoursesCompleted = await _context.Enrollments.CountAsync(e => e.TraineeId == traineeId && e.Status == EnrollmentStatus.COMPLETED),
                ActivitiesCompleted = await _context.Assessments.CountAsync(a => a.Enrollment.TraineeId == traineeId && a.Status == AssessmentStatus.PASS),
                ActivitiesDue = await _context.Assessments.CountAsync(a => a.Enrollment.TraineeId == traineeId && a.Status == AssessmentStatus.PENDING),

                MyCourses = await _context.Enrollments
                    .Include(e => e.Session).ThenInclude(s => s.Course)
                    .Where(e => e.TraineeId == traineeId)
                    .OrderBy(e => e.Session.Course.Title)
                    .Select(e => new TraineeCourseViewModel
                    {
                        EnrollmentId = e.Id,
                        CourseId = e.Session.Course.Id,
                        Title = e.Session.Course.Title,
                        Category = e.Session.Course.Category.ToString(),
                        Description = e.Session.Course.Description,
                        Status = e.Status.ToString(),
                        SessionDate = e.Session.SessionDate,
                        StartTime = e.Session.StartTime,
                        EndTime = e.Session.EndTime
                    })
                    .Take(6) //6 records on the dashboard only. 
                    .ToListAsync()
            };

            return View(model);
        }

        // Displays all courses the trainee is enrolled in, with search, status filter, and card/table view mode.
        public async Task<IActionResult> MyCourses(string searchString, string statusFilter, string viewMode = "card")
        {
            var traineeId = GetTraineeId();

            // Base query gets only the enrollments belonging to the logged-in trainee.
            var query = _context.Enrollments
                .Include(e => e.Session).ThenInclude(s => s.Course)
                .Where(e => e.TraineeId == traineeId)
                .AsQueryable();

            // Applies search filter if the trainee typed a course title or description.
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(e =>
                    e.Session.Course.Title.Contains(searchString) ||
                    e.Session.Course.Description.Contains(searchString));
            }

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                query = query.Where(e => e.Status.ToString() == statusFilter);
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentStatus"] = statusFilter;
            ViewData["ViewMode"] = viewMode;

            // Creates the dropdown list for enrollment statuses.
            ViewBag.StatusList = Enum.GetValues(typeof(EnrollmentStatus))
                .Cast<EnrollmentStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString(),
                    Selected = s.ToString() == statusFilter
                })
                .ToList();

                
            // Converts enrollment data into a simpler ViewModel for the view.
            var courses = await query
                .OrderBy(e => e.Session.Course.Title)
                .Select(e => new TraineeCourseViewModel
                {
                    EnrollmentId = e.Id,
                    CourseId = e.Session.Course.Id,
                    Title = e.Session.Course.Title,
                    Category = e.Session.Course.Category.ToString(),
                    Description = e.Session.Course.Description,
                    Status = e.Status.ToString(),
                    HasFeedback = _context.Feedbacks.Any(f => f.TraineeId == traineeId && f.CourseId == e.Session.Course.Id),
                    DroppedCount = _context.Enrollments.Count(en => en.TraineeId == traineeId && en.Session.CourseId == e.Session.Course.Id && en.Status == EnrollmentStatus.DROPPED),
                    SessionDate = e.Session.SessionDate,
                    StartTime = e.Session.StartTime,
                    EndTime = e.Session.EndTime
                })
                .ToListAsync();


            return View(courses);
        }

        // Displays all available courses in the catalog with search, category filter, and card/table view mode.
        public async Task<IActionResult> CourseCatalog(string searchString, CourseCategory? categoryFilter, string viewMode = "card")
        {
            var query = _context.Courses
                .Include(c => c.Prerequisite)
                .Include(c => c.Tracks)
                .Where(c => _context.Sessions.Any(s => s.CourseId == c.Id))
                .AsNoTracking()
                .AsQueryable();

            // Filters courses by title or description.
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(c =>
                    c.Title.Contains(searchString) ||
                    c.Description.Contains(searchString));
            }

            // Filters courses by category if a category was selected.
            if (categoryFilter.HasValue && categoryFilter.Value != CourseCategory.None)
            {
                query = query.Where(c => c.Category == categoryFilter.Value);
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = categoryFilter;
            ViewData["ViewMode"] = viewMode;

            ViewBag.CategoryFilterList = Enum.GetValues(typeof(CourseCategory))
                .Cast<CourseCategory>()
                .Select(c => new SelectListItem
                {
                    Value = c.ToString(),
                    Text = c.ToString(),
                    Selected = categoryFilter.HasValue && c == categoryFilter.Value
                })
                .ToList();

            var courses = await query
                .Include(c => c.Tracks)
                .OrderBy(c => c.Title)
                .ToListAsync();

            ViewBag.AvailableCourseIds = await _context.Sessions
                .Select(s => s.CourseId)
                .Distinct()
                .ToListAsync();
            return View(courses);
        }

        // Shows full details for one course, including prerequisite, tracks, next session, and enrollment status.
        public async Task<IActionResult> CourseDetails(int id)
        {
            var traineeId = GetTraineeId();

            var course = await _context.Courses
                .Include(c => c.Prerequisite)
                .Include(c => c.Tracks)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null) return NotFound();

            // Checks if the logged-in trainee is already enrolled in this course.
            ViewBag.IsEnrolled = await _context.Enrollments
                .AnyAsync(e => e.TraineeId == traineeId && e.Session.CourseId == id && e.Status != EnrollmentStatus.DROPPED && e.Status != EnrollmentStatus.COMPLETED);

            // Gets the next available session for this course.
            ViewBag.NextSession = await _context.Sessions
                .Include(s => s.Instructor)
                .Include(s => s.Classroom)
                .Where(s => s.CourseId == id)
                .OrderBy(s => s.SessionDate)
                .FirstOrDefaultAsync();

            // Counts how many trainees are enrolled in this course.
            ViewBag.EnrolledCount = await _context.Enrollments
                .CountAsync(e => e.Session.CourseId == id && e.Status != EnrollmentStatus.DROPPED && e.Status != EnrollmentStatus.COMPLETED);

            var droppedEnrollmentCount = await _context.Enrollments
           .CountAsync(e => e.TraineeId == traineeId && e.Session.CourseId == course.Id && e.Status == EnrollmentStatus.DROPPED);

            ViewBag.DroppedEnrollmentCount = droppedEnrollmentCount;

            var isCompleted = await _context.Enrollments
                .AnyAsync(e => e.TraineeId == traineeId && e.Session.CourseId == id && e.Status == EnrollmentStatus.COMPLETED);
            ViewBag.IsCompleted = isCompleted;


            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var traineeId = GetTraineeId();

            var session = await _context.Sessions
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.CourseId == courseId);

            if (session == null)
            {
                TempData["ErrorMessage"] = "No session is available for this course.";
                return RedirectToAction(nameof(CourseDetails), new { id = courseId });
            }

            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.TraineeId == traineeId && e.Session.CourseId == courseId && e.Status != EnrollmentStatus.DROPPED);

            if (alreadyEnrolled)
            {
                TempData["ErrorMessage"] = "You are already enrolled in this course.";
                return RedirectToAction(nameof(CourseDetails), new { id = courseId });
            }

            // Capacity validation
            var currentEnrollments = await _context.Enrollments
                .Where(e =>
                    e.SessionId == session.Id &&
                    e.Status != EnrollmentStatus.DROPPED && e.Status != EnrollmentStatus.COMPLETED)
                .CountAsync();

            if (currentEnrollments >= session.Course.Capacity)
            {
                TempData["ErrorMessage"] =
                    "This course session is full.";

                return RedirectToAction(
                    nameof(CourseDetails),
                    new { id = courseId });
            }

            // Schedule conflict validation
            var hasConflict = await _context.Enrollments
                .Include(e => e.Session)
                .AnyAsync(e =>
                    e.TraineeId == traineeId &&
                    e.Status != EnrollmentStatus.DROPPED && e.Status != EnrollmentStatus.COMPLETED &&
                    e.Session.SessionDate.Date == session.SessionDate.Date &&

                    session.StartTime < e.Session.EndTime &&
                    session.EndTime > e.Session.StartTime
                );

            if (hasConflict)
            {
                TempData["ErrorMessage"] =
                    "You already have another session at this time.";

                return RedirectToAction(
                    nameof(CourseDetails),
                    new { id = courseId });
            }

            var prerequisiteId = session.Course.PrerequisiteId;

            if (prerequisiteId != null)
            {
                var passedPrerequisite = await _context.Assessments
                    .AnyAsync(a =>
                        a.Enrollment.TraineeId == traineeId &&
                        a.Enrollment.Session.CourseId == prerequisiteId &&
                        a.Status == AssessmentStatus.PASS &&
                        a.Enrollment.Status == EnrollmentStatus.COMPLETED);

                if (!passedPrerequisite)
                {
                    var prerequisiteCourse = await _context.Courses
                        .FirstOrDefaultAsync(c => c.Id == prerequisiteId);

                    TempData["ErrorMessage"] =
                        $"You must complete '{prerequisiteCourse?.Title}' before enrolling in this course.";

                    return RedirectToAction(nameof(CourseDetails), new { id = courseId });
                }
            }

            var enrollment = new Enrollment
            {
                TraineeId = traineeId,
                SessionId = session.Id,
                Status = EnrollmentStatus.ENROLLED,
                EnrollmentDate = DateTime.Today,
                PaymentDueDate = DateTime.Today.AddDays(7)
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            _context.Balances.Add(new Balance
            {
                EnrollmentId = enrollment.Id,
                AmountDue = (int)session.Course.Fee,
                DueDate = DateTime.Today.AddDays(7)
            });

            await _context.SaveChangesAsync();

            // Notify real-time enrollment count updates
            //1) Get the current enrolled count for the session (excluding dropped)
            var enrolledCount = await _context.Enrollments
               .Where(e => e.Status != EnrollmentStatus.DROPPED && e.Status != EnrollmentStatus.COMPLETED)
                      .CountAsync(e => e.SessionId == session.Id);

            //2) Get the course capacity and calculate remaining seats
            var capacity = session.Course?.Capacity ?? 0;
            var remainingSeats = capacity - enrolledCount;

            //3) Create a payload with the updated enrollment info
            var payload = new
            {
                courseId = session.CourseId,
                sessionId = session.Id,
                enrolledCount = enrolledCount,
                isFull = remainingSeats <= 0
            };

            //4) Send the update to all clients subscribed to this course and session
            await _enrollmentHub.Clients.Group($"course-{session.CourseId}")
                .SendAsync("EnrollmentUpdated", payload);

            await _enrollmentHub.Clients.Group($"session-{session.Id}")
                .SendAsync("EnrollmentUpdated", payload);

            TempData["SuccessMessage"] = "Enrollment successful.";
            return RedirectToAction(nameof(CourseDetails), new { id = courseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DropCourse(int enrollmentId)
        {
            var traineeId = GetTraineeId();

            var session = await _context.Enrollments
                .Where(e => e.Id == enrollmentId && e.TraineeId == traineeId)
                .Select(e => e.Session)
                .FirstOrDefaultAsync();
            
            var courseId = session?.CourseId ?? 0;

            if (session == null)
            {
                TempData["ErrorMessage"] = "No session is available for this course.";
                return RedirectToAction(nameof(CourseDetails), new { id = courseId });
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Balance)
                .FirstOrDefaultAsync(e =>
                    e.Id == enrollmentId &&
                    e.TraineeId == traineeId);

            if (enrollment == null)
            {
                TempData["ErrorMessage"] = "Enrollment not found.";
                return RedirectToAction(nameof(MyCourses));
            }

            if (enrollment.Status == EnrollmentStatus.COMPLETED)
            {
                TempData["ErrorMessage"] =
                    "Completed courses cannot be dropped.";

                return RedirectToAction(nameof(MyCourses));
            }

            enrollment.Status = EnrollmentStatus.DROPPED;

            // cancel payment
            if (enrollment.Balance != null)
            {
                enrollment.Balance.AmountDue = 0;
            }

            await _context.SaveChangesAsync();

            // Notify real-time enrollment count updates
            //1) Get the current enrolled count for the session (excluding dropped)
            var enrolledCount = await _context.Enrollments
                .Where(e => e.Status != EnrollmentStatus.DROPPED && e.Status != EnrollmentStatus.COMPLETED)
                       .CountAsync(e => e.SessionId == session.Id);

            //2) Get the course capacity and calculate remaining seats
            var capacity = session.Course?.Capacity ?? 0;
            var remainingSeats = capacity - enrolledCount;

            //3) Create a payload with the updated enrollment info
            var payload = new
            {
                courseId = session.CourseId,
                sessionId = session.Id,
                enrolledCount = enrolledCount,
                isFull = remainingSeats <= 0
            };

            //4) Send the update to all clients subscribed to this course and session
            await _enrollmentHub.Clients.Group($"course-{session.CourseId}")
               .SendAsync("EnrollmentUpdated", payload);

            await _enrollmentHub.Clients.Group($"session-{session.Id}")
                .SendAsync("EnrollmentUpdated", payload);

            TempData["SuccessMessage"] =
                "Course dropped successfully.";

            return RedirectToAction(nameof(MyCourses));
        }

        public async Task<IActionResult> Certification()
        {
            var traineeId = GetTraineeId();

            var completedCourseIds = await _context.Assessments
                .Where(a =>
                    a.Enrollment.TraineeId == traineeId &&
                    a.Status == AssessmentStatus.PASS &&
                    a.Enrollment.Status == EnrollmentStatus.COMPLETED &&
                    a.Enrollment.Balance.AmountDue == 0)
                            .Select(a => a.Enrollment.Session.CourseId)
                .Distinct()
                .ToListAsync();

            var tracks = await _context.Tracks
                .Include(t => t.Courses)
                .OrderBy(t => t.Name)
                .ToListAsync();

            var model = tracks.Select(t =>
            {
                var required = t.Courses.Count;
                var completed = t.Courses.Count(c => completedCourseIds.Contains(c.Id));

                var certificate = _context.TraineeCertifications
                    .Where(tc =>
                        tc.TraineeId == traineeId &&
                        tc.TrackId == t.Id)
                    .OrderByDescending(tc => tc.Id)
                    .FirstOrDefault();

                return new TraineeCertificationProgressViewModel
                {
                    TrackId = t.Id,
                    TrackName = t.Name,
                    Description = t.Description,
                    RequiredCourses = required,
                    CompletedCourses = completed,
                    RemainingCourses = required - completed,
                    ProgressPercent = required == 0 ? 0 : (int)Math.Round((double)completed / required * 100),
                    IsEligible = required > 0 && completed == required,

                    CertificateReferenceNumber = certificate?.CertificateReferenceNumber,

                    Courses = t.Courses.Select(c =>
                    {
                        var assessment = _context.Assessments
                            .Include(a => a.Enrollment)
                            .ThenInclude(e => e.Balance)
                            .FirstOrDefault(a =>
                                a.Enrollment.TraineeId == traineeId &&
                                a.Enrollment.Session.CourseId == c.Id);

                        return new CertificationCourseItemViewModel
                        {
                            CourseTitle = c.Title,

                            IsCompleted = assessment != null &&
                                          assessment.Status == AssessmentStatus.PASS,

                            Status =
    assessment == null
        ? "PENDING"
        : assessment.Status == AssessmentStatus.FAIL
            ? "FAIL"
            : assessment.Status == AssessmentStatus.PASS &&
              assessment.Enrollment.Balance != null &&
              assessment.Enrollment.Balance.AmountDue > 0
                ? "UNPAID"
                : "PASS"
                        };
                    }).ToList()
                };
            }).ToList();

            return View(model);
        }

        public async Task<IActionResult> MyPayments()
        {
            int traineeId = GetTraineeId();

            if (traineeId <= 0)
                return RedirectToAction("Login", "Account");

            // Get all enrollments with payment info for the trainee
            var enrollments = await _paymentTrackingService.GetTraineePaymentsAsync(traineeId);

            // Create OVERDUE notifications if needed
            await _notificationService.CreateOverduePaymentNotificationsForUserAsync(traineeId);

            return View(enrollments);
        }

        public async Task<IActionResult> MyNotifications()
        {
            var traineeId = GetTraineeId();

            // Get all notifications for the trainee, ordered by most recent
            var notifications = await _context.Notifications
                .Where(n => n.UserId == traineeId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            return View(notifications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> GenerateCertificate(int trackId)
        {
            var traineeId = GetTraineeId();

            var certification = await _context.TraineeCertifications
                .FirstOrDefaultAsync(tc =>
                    tc.TraineeId == traineeId &&
                    tc.TrackId == trackId);

            if (certification == null)
            {
                certification = new TraineeCertification
                {
                    TraineeId = traineeId,
                    TrackId = trackId,
                    Status = TraineeCertificationStatus.SUCCESS,
                    CertificateReferenceNumber =
                        "CERT-" +
                        Guid.NewGuid()
                        .ToString("N")
                        .Substring(0, 8)
                        .ToUpper()
                };

                _context.TraineeCertifications.Add(certification);
            }
            else
            {
                certification.Status = TraineeCertificationStatus.SUCCESS;

                if (string.IsNullOrEmpty(certification.CertificateReferenceNumber))
                {
                    certification.CertificateReferenceNumber =
                        "CERT-" +
                        Guid.NewGuid()
                        .ToString("N")
                        .Substring(0, 8)
                        .ToUpper();
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Certification));
        }

        [HttpPost]
        public async Task<IActionResult> DownloadCertificatePdf(int trackId)
        {
            var traineeId = GetTraineeId();

            var trainee = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == traineeId);

            var track = await _context.Tracks
                .FirstOrDefaultAsync(t => t.Id == trackId);

            var certificate = await _context.TraineeCertifications
                .Where(tc =>
                    tc.TraineeId == traineeId &&
                    tc.TrackId == trackId)
                .OrderByDescending(tc => tc.Id)
                .FirstOrDefaultAsync();

            if (trainee == null || track == null || certificate == null)
            {
                return NotFound();
            }

            var verificationUrl =
                $"{Request.Scheme}://{Request.Host}/PublicCertification?referenceNumber={certificate.CertificateReferenceNumber}";
            byte[] qrCodeBytes;

            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrData = qrGenerator.CreateQrCode(
                    verificationUrl,
                    QRCodeGenerator.ECCLevel.Q);

                var pngQrCode = new PngByteQRCode(qrData);
                qrCodeBytes = pngQrCode.GetGraphic(20);
            }

            QuestPDF.Settings.License = LicenseType.Community;

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(25);

                    page.Background()
                        .Border(10)
                        .BorderColor("#1b6ec2");

                    page.Content().PaddingHorizontal(40).PaddingVertical(30).Column(col =>
                    {
                        col.Item().AlignCenter()
                            .Text("🏆 TRAINING CERTIFICATE")
                            .FontSize(34)
                            .Bold()
                            .FontColor("#1b6ec2");

                        col.Item().PaddingTop(10);

                        col.Item().AlignCenter()
                            .Text("This certificate is proudly awarded to")
                            .FontSize(18)
                            .FontColor("#555555");

                        col.Item().PaddingTop(15);

                        col.Item().AlignCenter()
                            .Text($"{trainee.FirstName} {trainee.LastName}")
                            .FontSize(36)
                            .Bold()
                            .FontColor("#212529");

                        col.Item().PaddingTop(15);

                        col.Item().AlignCenter()
                            .Text("For successfully completing")
                            .FontSize(18)
                            .FontColor("#555555");

                        col.Item().PaddingTop(10);

                        col.Item().AlignCenter()
                            .Text(track.Name)
                            .FontSize(28)
                            .Bold()
                            .FontColor("#198754");

                        col.Item().PaddingTop(25);

                        col.Item().LineHorizontal(1)
                            .LineColor("#d6d6d6");

                        col.Item().PaddingTop(20);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem(2).Column(left =>
                            {
                                left.Item()
                                    .Text("Certificate Reference")
                                    .Bold()
                                    .FontSize(11)
                                    .FontColor("#1b6ec2");

                                left.Item()
                                    .Text(certificate.CertificateReferenceNumber)
                                    .FontSize(11);

                                left.Item().PaddingTop(8);

                                left.Item()
                                    .Text("Issued Date")
                                    .Bold()
                                    .FontSize(11)
                                    .FontColor("#1b6ec2");

                                left.Item()
                                    .Text(DateTime.Today.ToString("dd MMM yyyy"))
                                    .FontSize(11);
                            });

                            row.RelativeItem(2).AlignCenter().Column(middle =>
                            {
                                middle.Item().PaddingTop(10);
                                middle.Item()
                                    .Text("Training Certification Platform")
                                    .Bold()
                                    .FontSize(14)
                                    .FontColor("#1b6ec2");

                                middle.Item()
                                    .Text("Authorized Digital Signature")
                                    .Italic()
                                    .FontSize(11)
                                    .FontColor("#777777");
                            });

                            row.RelativeItem(2).AlignRight().Column(right =>
                            {
                                right.Item().Width(90).Image(qrCodeBytes);

                                right.Item().PaddingTop(4);

                                right.Item().Width(120).AlignCenter()
                                    .Text("Scan to verify certificate")
                                    .FontSize(10)
                                    .FontColor("#555555");
                            });
                        });
                    });
                });
            });

            var pdfBytes = pdf.GeneratePdf();

            return File(
                pdfBytes,
                "application/pdf",
                "certificate.pdf");
        }

        public async Task<IActionResult> SubmitFeedback(int courseId)
        {
            var traineeId = GetTraineeId();

            var enrollment = await _context.Enrollments
                .Include(e => e.Session)
                .FirstOrDefaultAsync(e =>
                    e.TraineeId == traineeId &&
                    e.Session.CourseId == courseId &&
                    e.Status == EnrollmentStatus.COMPLETED);

            if (enrollment == null)
            {
                return RedirectToAction(nameof(MyCourses));
            }

            var model = new FeedbackViewModel
            {
                CourseId = courseId,
                InstructorId = enrollment.Session.InstructorId
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback(FeedbackViewModel model)
        {
            var traineeId = GetTraineeId();

            var feedback = new Feedback
            {
                TraineeId = traineeId,
                InstructorId = model.InstructorId,
                CourseId = model.CourseId,

                Rating = model.Rating,
                ContentRating = model.ContentRating,
                InstructorRating = model.InstructorRating,
                OrganizationRating = model.OrganizationRating,

                RecommendCourse = model.RecommendCourse,

                Comment = model.Comment
            };

            _context.Feedbacks.Add(feedback);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Feedback submitted successfully.";

            //Notify Instructor

            //course title
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == model.CourseId);
            //message content
            var message = course?.Title != null ? $"You received a new feedback for course {course.Title}." : "You received a new feedback for a course.";

            //create notification for the instructor
            await _notificationService.CreateNotificationAsync(model.InstructorId, message);

            return RedirectToAction(nameof(MyCourses));
        }
    }
}