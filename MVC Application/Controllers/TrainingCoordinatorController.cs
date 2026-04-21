using System;
using MVC_Application.Models.ViewModels; // ViewModels for dashboard data
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCertificationPlatform;              // Shared DbContext from API project
using MVC_Application.Models;      // Shared domain models

namespace MVC_Application.Controllers
{
    public class TrainingCoordinatorController : Controller
    {
        private readonly AppDbContext _context;

        public TrainingCoordinatorController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> Dashboard()
        {
            // Get the next scheduled sessions from seed data.
            // Using ordered sessions instead of DateTime.Today avoids an empty dashboard
            // when your seeded dates are older than the current date.
            var upcomingSessions = await _context.Sessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .Include(s => s.Classroom)
                .OrderBy(s => s.SessionDate)
                .ThenBy(s => s.StartTime)
                .Take(5)
                .Select(s => new UpcomingCourseViewModel
                {
                    CourseName = s.Course.Title,
                    InstructorName = s.Instructor.FirstName + " " + s.Instructor.LastName,
                    RoomName = s.Classroom.Name,
                    ScheduleDate = s.SessionDate.ToString("dd MMM yyyy"),
                    ScheduleTime = s.StartTime.ToString("hh\\:mm") + " - " + s.EndTime.ToString("hh\\:mm"),
                    Capacity = s.Course.Capacity,
                    EnrolledCount = _context.Enrollments.Count(e => e.SessionId == s.Id)
                })
                .ToListAsync();

            var recentEnrollments = await _context.Enrollments
                .Include(e => e.Trainee)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .OrderByDescending(e => e.EnrollmentDate)
                .Take(5)
                .Select(e => new RecentEnrollmentViewModel
                {
                    TraineeName = e.Trainee.FirstName + " " + e.Trainee.LastName,
                    CourseName = e.Session.Course.Title,
                    Status = e.Status.ToString(),
                    EnrolledDate = e.EnrollmentDate.ToString("dd MMM yyyy")
                })
                .ToListAsync();

            var model = new DashboardViewModel
            {
                TotalTrainees = await _context.Users.CountAsync(u => u.Role == UserRole.TRAINEE),
                TotalInstructors = await _context.Users.CountAsync(u => u.Role == UserRole.INSTRUCTOR),
                TotalCourses = await _context.Courses.CountAsync(),
                ActiveSchedules = await _context.Sessions.CountAsync(),
                PendingPayments = await _context.Balances.CountAsync(b => b.AmountDue > 0),
                CertificatesIssued = await _context.TraineeCertifications.CountAsync(c => c.Status == TraineeCertificationStatus.SUCCESS),

                UpcomingCourses = upcomingSessions,
                RecentEnrollments = recentEnrollments
            };

            return View(model);
        }
    }
}