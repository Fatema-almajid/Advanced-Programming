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
    public class InstructorsController : Controller
    {
        private readonly AppDbContext _context;

        public InstructorsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var instructorUsers = await _context.Users
                .Where(u => u.Role == UserRole.INSTRUCTOR)
                .AsNoTracking()
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();

            var instructorIds = instructorUsers.Select(u => u.Id).ToList();

            // fix instructor 
            var availabilityLookup = await _context.InstructorAvailabilities
                .Where(a => instructorIds.Contains(a.InstructorId))
                .AsNoTracking()
                .GroupBy(a => a.InstructorId)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.First());

            var expertiseLookup = await _context.InstructorExpertises
                .Where(e => instructorIds.Contains(e.InstructorId))
                .Include(e => e.Course)
                .AsNoTracking()
                .GroupBy(e => e.InstructorId)
                .ToDictionaryAsync(
                    group => group.Key,
                    group => string.Join(", ", group.OrderBy(x => x.Course.Title).Select(x => x.Course.Title)));

            var instructors = instructorUsers.Select(u =>
            {
                availabilityLookup.TryGetValue(u.Id, out var availability);
                expertiseLookup.TryGetValue(u.Id, out var expertiseCourses);

                return new InstructorListItemViewModel
                {
                    Id = u.Id,
                    FullName = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    Phone = u.Phone,
                    RegistrationDate = u.RegistrationDate,
                    Availability = availability == null
                        ? "Not set"
                        : $"{availability.DayStart} - {availability.DayEnd} | {availability.StartTime} - {availability.EndTime}",
                    ExpertiseCourses = string.IsNullOrWhiteSpace(expertiseCourses) ? "None" : expertiseCourses
                };
            });

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                instructors = instructors.Where(i =>
                    i.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    i.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    i.Phone.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    i.ExpertiseCourses.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }

            ViewData["CurrentFilter"] = searchString;

            return View(instructors.ToList());
        }

        public async Task<IActionResult> Create()
        {
            await PopulateFormDropdownsAsync();
            return View(new InstructorFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InstructorFormViewModel model)
        {
            await ValidateInstructorAsync(model);

            if (!ModelState.IsValid)
            {
                await PopulateFormDropdownsAsync(model.SelectedCourseIds);
                return View(model);
            }

            var instructor = new User
            {
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                Email = model.Email.Trim(),
                Phone = model.Phone.Trim(),
                RegistrationDate = model.RegistrationDate,
                Password = model.Password!,
                Role = UserRole.INSTRUCTOR
            };

            _context.Users.Add(instructor);
            await _context.SaveChangesAsync();

            var availability = new InstructorAvailability
            {
                InstructorId = instructor.Id,
                DayStart = model.DayStart,
                DayEnd = model.DayEnd,
                StartTime = model.StartTime,
                EndTime = model.EndTime
            };

            _context.InstructorAvailabilities.Add(availability);

            if (model.SelectedCourseIds.Any())
            {
                var expertises = model.SelectedCourseIds.Distinct().Select(courseId => new InstructorExpertise
                {
                    InstructorId = instructor.Id,
                    CourseId = courseId
                });

                _context.InstructorExpertises.AddRange(expertises);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Instructor created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var instructor = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && u.Role == UserRole.INSTRUCTOR);

            if (instructor == null) return NotFound();

            var availability = await _context.InstructorAvailabilities
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.InstructorId == instructor.Id);

            var selectedCourseIds = await _context.InstructorExpertises
                .Where(e => e.InstructorId == instructor.Id)
                .Select(e => e.CourseId)
                .ToListAsync();

            var model = new InstructorFormViewModel
            {
                Id = instructor.Id,
                FirstName = instructor.FirstName,
                LastName = instructor.LastName,
                Email = instructor.Email,
                Phone = instructor.Phone,
                RegistrationDate = instructor.RegistrationDate,
                DayStart = availability?.DayStart ?? Day.SUNDAY,
                DayEnd = availability?.DayEnd ?? Day.THURSDAY,
                StartTime = availability?.StartTime ?? new TimeOnly(9, 0),
                EndTime = availability?.EndTime ?? new TimeOnly(17, 0),
                SelectedCourseIds = selectedCourseIds
            };

            await PopulateFormDropdownsAsync(model.SelectedCourseIds);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InstructorFormViewModel model)
        {
            if (id != model.Id) return NotFound();

            await ValidateInstructorAsync(model, id);

            if (!ModelState.IsValid)
            {
                await PopulateFormDropdownsAsync(model.SelectedCourseIds);
                return View(model);
            }

            var instructor = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.Role == UserRole.INSTRUCTOR);

            if (instructor == null) return NotFound();

            instructor.FirstName = model.FirstName.Trim();
            instructor.LastName = model.LastName.Trim();
            instructor.Email = model.Email.Trim();
            instructor.Phone = model.Phone.Trim();
            instructor.RegistrationDate = model.RegistrationDate;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                instructor.Password = model.Password;
            }

            var availability = await _context.InstructorAvailabilities
                .FirstOrDefaultAsync(a => a.InstructorId == instructor.Id);

            if (availability == null)
            {
                availability = new InstructorAvailability { InstructorId = instructor.Id };
                _context.InstructorAvailabilities.Add(availability);
            }

            availability.DayStart = model.DayStart;
            availability.DayEnd = model.DayEnd;
            availability.StartTime = model.StartTime;
            availability.EndTime = model.EndTime;

            var existingExpertises = await _context.InstructorExpertises
                .Where(e => e.InstructorId == instructor.Id)
                .ToListAsync();

            _context.InstructorExpertises.RemoveRange(existingExpertises);

            if (model.SelectedCourseIds.Any())
            {
                var newExpertises = model.SelectedCourseIds.Distinct().Select(courseId => new InstructorExpertise
                {
                    InstructorId = instructor.Id,
                    CourseId = courseId
                });

                _context.InstructorExpertises.AddRange(newExpertises);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Instructor updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var instructor = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && u.Role == UserRole.INSTRUCTOR);

            if (instructor == null) return NotFound();

            ViewBag.Availability = await _context.InstructorAvailabilities
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.InstructorId == instructor.Id);

            ViewBag.ExpertiseCourses = await _context.InstructorExpertises
                .Where(e => e.InstructorId == instructor.Id)
                .Select(e => e.Course.Title)
                .OrderBy(title => title)
                .ToListAsync();

            return View(instructor);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var instructor = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && u.Role == UserRole.INSTRUCTOR);

            if (instructor == null) return NotFound();

            ViewBag.Availability = await _context.InstructorAvailabilities
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.InstructorId == instructor.Id);

            ViewBag.ExpertiseCourses = await _context.InstructorExpertises
                .Where(e => e.InstructorId == instructor.Id)
                .Select(e => e.Course.Title)
                .OrderBy(title => title)
                .ToListAsync();

            return View(instructor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.Role == UserRole.INSTRUCTOR);

            if (instructor == null)
            {
                TempData["ErrorMessage"] = "Instructor not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var availabilities = await _context.InstructorAvailabilities
                    .Where(a => a.InstructorId == id)
                    .ToListAsync();

                var expertises = await _context.InstructorExpertises
                    .Where(e => e.InstructorId == id)
                    .ToListAsync();

                _context.InstructorAvailabilities.RemoveRange(availabilities);
                _context.InstructorExpertises.RemoveRange(expertises);
                _context.Users.Remove(instructor);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Instructor deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "This instructor cannot be deleted because they are linked to other records such as sessions or assessments.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateFormDropdownsAsync(List<int>? selectedCourseIds = null)
        {
            ViewBag.CourseList = new MultiSelectList(
                await _context.Courses
                    .AsNoTracking()
                    .OrderBy(c => c.Title)
                    .ToListAsync(),
                "Id",
                "Title",
                selectedCourseIds);

            ViewBag.DayList = Enum.GetValues(typeof(Day))
                .Cast<Day>()
                .Select(day => new SelectListItem
                {
                    Value = day.ToString(),
                    Text = day.ToString()
                })
                .ToList();
        }

        private async Task ValidateInstructorAsync(InstructorFormViewModel model, int? currentInstructorId = null)
        {
            var trimmedEmail = model.Email.Trim();
            var trimmedPhone = model.Phone.Trim();

            var emailExists = await _context.Users.AnyAsync(u =>
                u.Email == trimmedEmail &&
                (!currentInstructorId.HasValue || u.Id != currentInstructorId.Value));

            if (emailExists)
            {
                ModelState.AddModelError(nameof(model.Email), "This email address is already in use.");
            }

            var phoneExists = await _context.Users.AnyAsync(u =>
                u.Phone == trimmedPhone &&
                (!currentInstructorId.HasValue || u.Id != currentInstructorId.Value));

            if (phoneExists)
            {
                ModelState.AddModelError(nameof(model.Phone), "This phone number is already in use.");
            }

            var validCourseIds = await _context.Courses
                .Where(c => model.SelectedCourseIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync();

            if (validCourseIds.Count != model.SelectedCourseIds.Distinct().Count())
            {
                ModelState.AddModelError(nameof(model.SelectedCourseIds), "One or more selected expertise courses are invalid.");
            }
        }
    }
}