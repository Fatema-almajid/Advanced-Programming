using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Application.Models.ViewModels;
using TrainingCertificationPlatform;
using TrainingCertificationPlatform.Models;
using Microsoft.AspNetCore.Authorization;

namespace MVC_Application.Controllers
{
    [Authorize(Roles = "TRAINING_COORDINATOR")]
    public class SessionsController : Controller
    {
        private readonly AppDbContext _context;

        public SessionsController(AppDbContext context)
        {
            _context = context;
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
            await ValidateSessionAsync(model);

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

            await ValidateSessionAsync(model, id);

            if (!ModelState.IsValid)
            {
                await PopulateFormDropdownsAsync(model.CourseId, model.InstructorId, model.ClassroomId);
                return View(model);
            }

            var session = await _context.Sessions.FindAsync(id);
            if (session == null) return NotFound();

            session.CourseId = model.CourseId;
            session.InstructorId = model.InstructorId;
            session.ClassroomId = model.ClassroomId;
            session.SessionDate = model.SessionDate.Date;
            session.StartTime = model.StartTime;
            session.EndTime = model.EndTime;

            await _context.SaveChangesAsync();

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
            var session = await _context.Sessions.FindAsync(id);

            if (session == null)
            {
                TempData["ErrorMessage"] = "Session not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Sessions.Remove(session);
                await _context.SaveChangesAsync();
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

        private async Task ValidateSessionAsync(SessionFormViewModel model, int? currentSessionId = null)
        {
            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError(nameof(model.EndTime), "End time must be later than start time.");
                return;
            }

            if (model.SessionDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(nameof(model.SessionDate), "Session date cannot be in the past.");
            }

            var course = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == model.CourseId);

            if (course == null)
            {
                ModelState.AddModelError(nameof(model.CourseId), "Selected course is invalid.");
            }

            var instructor = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == model.InstructorId && u.Role == UserRole.INSTRUCTOR);

            if (instructor == null)
            {
                ModelState.AddModelError(nameof(model.InstructorId), "Selected instructor is invalid.");
            }

            var classroom = await _context.Classrooms
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == model.ClassroomId);

            if (classroom == null)
            {
                ModelState.AddModelError(nameof(model.ClassroomId), "Selected room is invalid.");
            }

            if (!ModelState.IsValid || course == null || instructor == null || classroom == null)
            {
                return;
            }

            // Capacity validation
            if (classroom.Seats < course.Capacity)
            {
                ModelState.AddModelError(nameof(model.ClassroomId),
                    $"This room cannot be assigned because it has only {classroom.Seats} seats while the course capacity is {course.Capacity}.");
            }

            // Instructor expertise validation
            var instructorCanTeachCourse = await _context.InstructorExpertises
                .AnyAsync(e => e.InstructorId == model.InstructorId && e.CourseId == model.CourseId);

            if (!instructorCanTeachCourse)
            {
                ModelState.AddModelError(nameof(model.InstructorId),
                    "This instructor is not assigned as an expert for the selected course.");
            }

            // Instructor availability validation
            var availability = await _context.InstructorAvailabilities
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.InstructorId == model.InstructorId);

            if (availability == null)
            {
                ModelState.AddModelError(nameof(model.InstructorId),
                    "This instructor does not have an availability schedule yet.");
            }
            else
            {
                var sessionDay = model.SessionDate.DayOfWeek switch
                {
                    DayOfWeek.Sunday => Day.SUNDAY,
                    DayOfWeek.Monday => Day.MONDAY,
                    DayOfWeek.Tuesday => Day.TUESDAY,
                    DayOfWeek.Wednesday => Day.WEDNESDAY,
                    DayOfWeek.Thursday => Day.THURSDAY,
                    DayOfWeek.Friday => Day.FRIDAY,
                    _ => Day.SATURDAY
                };

                if (sessionDay < availability.DayStart || sessionDay > availability.DayEnd)
                {
                    ModelState.AddModelError(nameof(model.SessionDate),
                        $"The instructor is only available from {availability.DayStart} to {availability.DayEnd}.");
                }

                if (model.StartTime < availability.StartTime || model.EndTime > availability.EndTime)
                {
                    ModelState.AddModelError(nameof(model.StartTime),
                        $"The instructor is only available between {availability.StartTime} and {availability.EndTime}.");
                }
            }

            // Instructor double-booking validation
            var instructorConflict = await _context.Sessions
                .AnyAsync(s =>
                    s.InstructorId == model.InstructorId &&
                    s.SessionDate.Date == model.SessionDate.Date &&
                    (!currentSessionId.HasValue || s.Id != currentSessionId.Value) &&
                    model.StartTime < s.EndTime &&
                    model.EndTime > s.StartTime);

            if (instructorConflict)
            {
                ModelState.AddModelError(nameof(model.InstructorId),
                    "This instructor is already booked for another session during the selected time.");
            }

            // Room double-booking validation
            var roomConflict = await _context.Sessions
                .AnyAsync(s =>
                    s.ClassroomId == model.ClassroomId &&
                    s.SessionDate.Date == model.SessionDate.Date &&
                    (!currentSessionId.HasValue || s.Id != currentSessionId.Value) &&
                    model.StartTime < s.EndTime &&
                    model.EndTime > s.StartTime);

            if (roomConflict)
            {
                ModelState.AddModelError(nameof(model.ClassroomId),
                    "This room is already booked for another session during the selected time.");
            }
        }
    }
}