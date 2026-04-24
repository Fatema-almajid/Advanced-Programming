using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Application.Models.ViewModels;
using TrainingCertificationPlatform;
using TrainingCertificationPlatform.Models;

namespace MVC_Application.Controllers
{
    [Authorize(Roles = "TRAINEE")]
    public class TraineeController : Controller
    {
        private readonly AppDbContext _context;

        public TraineeController(AppDbContext context)
        {
            _context = context;
        }

        private int GetTraineeId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        public async Task<IActionResult> Dashboard()
        {
            var traineeId = GetTraineeId();

            var model = new TraineeDashboardViewModel
            {
                CoursesEnrolled = await _context.Enrollments.CountAsync(e => e.TraineeId == traineeId),
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
                    .Take(6)
                    .ToListAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> MyCourses(string searchString, string statusFilter, string viewMode = "card")
        {
            var traineeId = GetTraineeId();

            var query = _context.Enrollments
                .Include(e => e.Session).ThenInclude(s => s.Course)
                .Where(e => e.TraineeId == traineeId)
                .AsQueryable();

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

            ViewBag.StatusList = Enum.GetValues(typeof(EnrollmentStatus))
                .Cast<EnrollmentStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString(),
                    Selected = s.ToString() == statusFilter
                })
                .ToList();

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
                    SessionDate = e.Session.SessionDate,
                    StartTime = e.Session.StartTime,
                    EndTime = e.Session.EndTime
                })
                .ToListAsync();

            return View(courses);
        }

        public async Task<IActionResult> CourseCatalog(string searchString, CourseCategory? categoryFilter, string viewMode = "card")
        {
            var query = _context.Courses
                .Include(c => c.Prerequisite)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(c =>
                    c.Title.Contains(searchString) ||
                    c.Description.Contains(searchString));
            }

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

            var courses = await query.OrderBy(c => c.Title).ToListAsync();
            return View(courses);
        }

        public async Task<IActionResult> CourseDetails(int id)
        {
            var traineeId = GetTraineeId();

            var course = await _context.Courses
                .Include(c => c.Prerequisite)
                .Include(c => c.Tracks)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null) return NotFound();

            ViewBag.IsEnrolled = await _context.Enrollments
                .AnyAsync(e => e.TraineeId == traineeId && e.Session.CourseId == id);

            ViewBag.NextSession = await _context.Sessions
                .Include(s => s.Instructor)
                .Include(s => s.Classroom)
                .Where(s => s.CourseId == id)
                .OrderBy(s => s.SessionDate)
                .FirstOrDefaultAsync();

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
                .AnyAsync(e => e.TraineeId == traineeId && e.Session.CourseId == courseId);

            if (alreadyEnrolled)
            {
                TempData["ErrorMessage"] = "You are already enrolled in this course.";
                return RedirectToAction(nameof(CourseDetails), new { id = courseId });
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

            TempData["SuccessMessage"] = "Enrollment successful. Payment balance has been created.";
            return RedirectToAction(nameof(MyCourses));
        }

        public async Task<IActionResult> Certification()
        {
            var traineeId = GetTraineeId();

            var completedCourseIds = await _context.Assessments
                .Where(a => a.Enrollment.TraineeId == traineeId && a.Status == AssessmentStatus.PASS)
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
                    Courses = t.Courses.Select(c => new CertificationCourseItemViewModel
                    {
                        CourseTitle = c.Title,
                        IsCompleted = completedCourseIds.Contains(c.Id)
                    }).ToList()
                };
            }).ToList();

            return View(model);
        }
    }
}