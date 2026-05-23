using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Application.Models.ViewModels;
using MVC_Application.Services;
using TrainingCertificationPlatform;
using TrainingCertificationPlatform.Models;
//using Microsoft.AspNetCore.Authorization;

namespace MVC_Application.Controllers
{
   // [Authorize(Roles = "TRAINING_COORDINATOR")]
    public class SessionsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly SessionSchedulingService _schedulingService;
        private readonly NotificationService _notificationService;

        public SessionsController(AppDbContext context, SessionSchedulingService schedulingService, NotificationService notificationService)
        {
            _context = context;
            _schedulingService = schedulingService;
            _notificationService = notificationService;
        }

        // TABLE VIEW
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.Sessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .Include(s => s.Classroom)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(s =>
                    s.Course.Title.Contains(searchString) ||
                    s.Classroom.Name.Contains(searchString) ||
                    s.Instructor.FirstName.Contains(searchString) ||
                    s.Instructor.LastName.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;

            var sessions = await query
                .OrderBy(s => s.SessionDate)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            return View(sessions);
        }

        // CARD VIEW
        public async Task<IActionResult> CardIndex(string searchString)
        {
            var query = _context.Sessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .Include(s => s.Classroom)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(s =>
                    s.Course.Title.Contains(searchString) ||
                    s.Classroom.Name.Contains(searchString) ||
                    s.Instructor.FirstName.Contains(searchString) ||
                    s.Instructor.LastName.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;

            var sessions = await query
                .OrderBy(s => s.SessionDate)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            return View(sessions);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateFormDropdownsAsync();
            return View(new SessionFormViewModel
            {
                SessionDate = DateTime.Today,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(11, 0)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SessionFormViewModel model)
        {
            //Validate session creation
            var errors = await _schedulingService.ValidateSessionAsync(
                model.CourseId,
                model.InstructorId,
                model.ClassroomId,
                model.SessionDate,
                model.StartTime,
                model.EndTime);

            foreach (var error in errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            if (!ModelState.IsValid)
            {
                await PopulateFormDropdownsAsync(model.CourseId, model.InstructorId, model.ClassroomId);
                return View(model);
            }

            var session = new Session
            {
                CourseId = model.CourseId,
                InstructorId = model.InstructorId,
                ClassroomId = model.ClassroomId,
                SessionDate = model.SessionDate.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            //Send notif to instructor about new session

            //Get course title for notification
            var course = await _context.Courses
        .FirstOrDefaultAsync(c => c.Id == model.CourseId);

            var courseTitle = course?.Title ?? "a course";

            //Create notification message
            var message = $"You have been assigned to teach a new session for the course '{courseTitle}' on {session.SessionDate:MMMM dd, yyyy}.";

            //Send notification to instructor
            await _notificationService.CreateNotificationAsync(session.InstructorId, message);

            TempData["SuccessMessage"] = "Session scheduled successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var session = await _context.Sessions.FindAsync(id);
            if (session == null) return NotFound();

            var model = new SessionFormViewModel
            {
                Id = session.Id,
                CourseId = session.CourseId,
                InstructorId = session.InstructorId,
                ClassroomId = session.ClassroomId,
                SessionDate = session.SessionDate,
                StartTime = session.StartTime,
                EndTime = session.EndTime
            };

            await PopulateFormDropdownsAsync(model.CourseId, model.InstructorId, model.ClassroomId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SessionFormViewModel model)
        {
            if (id != model.Id) return NotFound();

            //Validate session editing
            var errors = await _schedulingService.ValidateSessionAsync(
                model.CourseId,
                model.InstructorId,
                model.ClassroomId,
                model.SessionDate,
                model.StartTime,
                model.EndTime,
                model.Id);

            foreach (var error in errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            if (!ModelState.IsValid)
            {
                await PopulateFormDropdownsAsync(model.CourseId, model.InstructorId, model.ClassroomId);
                return View(model);
            }

            var session = await _context.Sessions
                            .Include(s => s.Course)
                            .FirstOrDefaultAsync(s => s.Id == id); if (session == null) return NotFound();

            session.CourseId = model.CourseId;
            session.InstructorId = model.InstructorId;
            session.ClassroomId = model.ClassroomId;
            session.SessionDate = model.SessionDate.Date;
            session.StartTime = model.StartTime;
            session.EndTime = model.EndTime;

            await _context.SaveChangesAsync();
            await _context.Entry(session).Reference(s => s.Course).LoadAsync();
            //Send notif to instructor about session update
            var message = $"The session for the course '{session.Course.Title}' scheduled on {session.SessionDate:MMMM dd, yyyy} has been updated.";
            var instructorId = session.InstructorId;
            await _notificationService.CreateNotificationAsync(instructorId, message);

            TempData["SuccessMessage"] = "Session updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var session = await _context.Sessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .Include(s => s.Classroom)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null) return NotFound();

            return View(session);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var session = await _context.Sessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .Include(s => s.Classroom)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null) return NotFound();

            return View(session);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _context.Sessions
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null)
            {
                TempData["ErrorMessage"] = "Session not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var message = $"The session for the course '{session.Course.Title}' scheduled on {session.SessionDate:MMMM dd, yyyy} has been cancelled.";
                var instructorId = session.InstructorId;

                _context.Sessions.Remove(session);
                await _context.SaveChangesAsync();

                //Send notif to instructor about session cancellation
                await _notificationService.CreateNotificationAsync(instructorId, message);

                TempData["SuccessMessage"] = "Session deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] =
                    "This session cannot be deleted because it is linked to other records such as enrollments.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateFormDropdownsAsync(int? selectedCourseId = null, int? selectedInstructorId = null, int? selectedClassroomId = null)
        {
            ViewBag.CourseList = new SelectList(
                await _context.Courses
                    .AsNoTracking()
                    .OrderBy(c => c.Title)
                    .ToListAsync(),
                "Id",
                "Title",
                selectedCourseId);

            ViewBag.InstructorList = new SelectList(
                await _context.Users
                    .Where(u => u.Role == UserRole.INSTRUCTOR)
                    .AsNoTracking()
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .Select(u => new
                    {
                        u.Id,
                        FullName = u.FirstName + " " + u.LastName
                    })
                    .ToListAsync(),
                "Id",
                "FullName",
                selectedInstructorId);

            ViewBag.ClassroomList = new SelectList(
                await _context.Classrooms
                    .AsNoTracking()
                    .OrderBy(c => c.Name)
                    .ToListAsync(),
                "Id",
                "Name",
                selectedClassroomId);
        }
    }
}