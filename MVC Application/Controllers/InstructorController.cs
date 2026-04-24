using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Application.Models.ViewModels;
using TrainingCertificationPlatform;
using TrainingCertificationPlatform.Models;

namespace MVC_Application.Controllers
{
    [Authorize(Roles = "INSTRUCTOR")]
    public class InstructorController : Controller
    {
        private readonly AppDbContext _context;

        public InstructorController(AppDbContext context)
        {
            _context = context;
        }

        private int GetInstructorId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        public async Task<IActionResult> Dashboard()
        {
            var instructorId = GetInstructorId();
            var today = DateTime.Today;

            var upcomingSessions = await _context.Sessions
                .Include(s => s.Course)
                .Include(s => s.Classroom)
                .Where(s => s.InstructorId == instructorId && s.SessionDate >= today)
                .OrderBy(s => s.SessionDate)
                .ThenBy(s => s.StartTime)
                .Take(5)
                .Select(s => new InstructorSessionViewModel
                {
                    SessionId = s.Id,
                    CourseId = s.CourseId,
                    CourseTitle = s.Course.Title,
                    CourseDescription = s.Course.Description,
                    Category = s.Course.Category.ToString(),
                    ClassroomName = s.Classroom.Name,
                    SessionDate = s.SessionDate,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Capacity = s.Course.Capacity,
                    EnrolledCount = _context.Enrollments.Count(e => e.SessionId == s.Id),
                    PendingAssessments = _context.Assessments.Count(a =>
                        a.Enrollment.SessionId == s.Id &&
                        a.Status == AssessmentStatus.PENDING)
                })
                .ToListAsync();

            var model = new InstructorDashboardViewModel
            {
                AssignedCourses = await _context.Sessions
                    .Where(s => s.InstructorId == instructorId)
                    .Select(s => s.CourseId)
                    .Distinct()
                    .CountAsync(),

                UpcomingSessions = await _context.Sessions
                    .CountAsync(s => s.InstructorId == instructorId && s.SessionDate >= today),

                PastSessions = await _context.Sessions
                    .CountAsync(s => s.InstructorId == instructorId && s.SessionDate < today),

                PendingAssessments = await _context.Assessments
                    .CountAsync(a =>
                        a.Enrollment.Session.InstructorId == instructorId &&
                        a.Status == AssessmentStatus.PENDING),

                UpcomingSessionList = upcomingSessions
            };

            return View(model);
        }

        public async Task<IActionResult> MyCourses(string searchString, string sessionFilter = "all", string viewMode = "card")
        {
            var instructorId = GetInstructorId();
            var today = DateTime.Today;

            var query = _context.Sessions
                .Include(s => s.Course)
                .Include(s => s.Classroom)
                .Where(s => s.InstructorId == instructorId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(s =>
                    s.Course.Title.Contains(searchString) ||
                    s.Course.Description.Contains(searchString) ||
                    s.Classroom.Name.Contains(searchString));
            }

            if (sessionFilter == "upcoming")
            {
                query = query.Where(s => s.SessionDate >= today);
            }
            else if (sessionFilter == "past")
            {
                query = query.Where(s => s.SessionDate < today);
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["SessionFilter"] = sessionFilter;
            ViewData["ViewMode"] = viewMode;

            var sessions = await query
                .OrderByDescending(s => s.SessionDate)
                .ThenBy(s => s.StartTime)
                .Select(s => new InstructorSessionViewModel
                {
                    SessionId = s.Id,
                    CourseId = s.CourseId,
                    CourseTitle = s.Course.Title,
                    CourseDescription = s.Course.Description,
                    Category = s.Course.Category.ToString(),
                    ClassroomName = s.Classroom.Name,
                    SessionDate = s.SessionDate,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Capacity = s.Course.Capacity,
                    EnrolledCount = _context.Enrollments.Count(e => e.SessionId == s.Id),
                    PendingAssessments = _context.Assessments.Count(a =>
                        a.Enrollment.SessionId == s.Id &&
                        a.Status == AssessmentStatus.PENDING),
                    SessionType = s.SessionDate >= today ? "Upcoming" : "Past"
                })
                .ToListAsync();

            return View(sessions);
        }

        public async Task<IActionResult> CourseDetails(int id)
        {
            var instructorId = GetInstructorId();

            var session = await _context.Sessions
                .Include(s => s.Course)
                .Include(s => s.Classroom)
                .FirstOrDefaultAsync(s => s.Id == id && s.InstructorId == instructorId);

            if (session == null)
                return NotFound();

            var trainees = await _context.Enrollments
                .Include(e => e.Trainee)
                .Include(e => e.Session)
                .Where(e => e.SessionId == id)
                .OrderBy(e => e.Trainee.FirstName)
                .ThenBy(e => e.Trainee.LastName)
                .Select(e => new InstructorTraineeAssessmentViewModel
                {
                    EnrollmentId = e.Id,
                    TraineeName = e.Trainee.FirstName + " " + e.Trainee.LastName,
                    TraineeEmail = e.Trainee.Email,
                    EnrollmentStatus = e.Status,
                    AssessmentStatus = _context.Assessments
                        .Where(a => a.EnrollmentId == e.Id)
                        .Select(a => a.Status)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var model = new InstructorCourseDetailsViewModel
            {
                SessionId = session.Id,
                CourseTitle = session.Course.Title,
                CourseDescription = session.Course.Description,
                Category = session.Course.Category.ToString(),
                ClassroomName = session.Classroom.Name,
                SessionDate = session.SessionDate,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                Trainees = trainees
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAssessment(int enrollmentId, AssessmentStatus status)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Session)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId);

            if (enrollment == null)
                return NotFound();

            var assessment = await _context.Assessments
                .FirstOrDefaultAsync(a => a.EnrollmentId == enrollmentId);

            if (assessment == null)
            {
                assessment = new Assessment
                {
                    EnrollmentId = enrollmentId,
                    DueDate = DateTime.Today
                };

                _context.Assessments.Add(assessment);
            }

            assessment.Status = status;
            assessment.CompletedBy = DateTime.Now;

            if (status == AssessmentStatus.PASS)
            {
                enrollment.Status = EnrollmentStatus.COMPLETED;
                enrollment.CompletionDate = DateTime.Today;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Assessment updated successfully.";
            return RedirectToAction(nameof(CourseDetails), new { id = enrollment.SessionId });
        }

        public async Task<IActionResult> Schedule()
        {
            var instructorId = GetInstructorId();
            var today = DateTime.Today;

            var sessions = await _context.Sessions
                .Include(s => s.Course)
                .Include(s => s.Classroom)
                .Where(s => s.InstructorId == instructorId)
                .OrderBy(s => s.SessionDate)
                .ThenBy(s => s.StartTime)
                .Select(s => new InstructorSessionViewModel
                {
                    SessionId = s.Id,
                    CourseId = s.CourseId,
                    CourseTitle = s.Course.Title,
                    CourseDescription = s.Course.Description,
                    Category = s.Course.Category.ToString(),
                    ClassroomName = s.Classroom.Name,
                    SessionDate = s.SessionDate,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Capacity = s.Course.Capacity,
                    EnrolledCount = _context.Enrollments.Count(e => e.SessionId == s.Id),
                    SessionType = s.SessionDate >= today ? "Upcoming" : "Past"
                })
                .ToListAsync();

            var model = new InstructorScheduleViewModel
            {
                UpcomingSessions = sessions.Where(s => s.SessionDate >= today).ToList(),
                PastSessions = sessions.Where(s => s.SessionDate < today).ToList(),

                Availabilities = await _context.InstructorAvailabilities
                    .Where(a => a.InstructorId == instructorId)
                    .OrderBy(a => a.DayStart)
                    .ToListAsync()
            };

            return View(model);
        }
    }
}