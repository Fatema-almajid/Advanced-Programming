using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Application.Models.ViewModels;
using TrainingCertificationPlatform;
using TrainingCertificationPlatform.Models;
using Microsoft.AspNetCore.Authorization;

/* 
 * This controller governs the academic course catalog under the authorized "TRAINING_COORDINATOR" role.
 * It features dual-view presentation interfaces (Table-based Index and Card-based Index) supported by asynchronous,
 * multi-parameter LINQ queries that execute server-side full-text searches across entities and self-referencing relationships (.Include for Prerequisites).
 * The implementation enforces strict educational business rules (such as blocking self-prerequisite assignment loops),
 * handles standard data persistency cycles with anti-forgery validation tokens, dynamically populates dynamic dropdown structures from enums,
 * and incorporates robust exception handling to cleanly capture both concurrent data access anomalies (`DbUpdateConcurrencyException`) and relational database constraint violations (`DbUpdateException`).
 */

namespace MVC_Application.Controllers
{
    [Authorize(Roles = "TRAINING_COORDINATOR")]
    public class CoursesController : Controller
    {
        private readonly AppDbContext _context;

        public CoursesController(AppDbContext context)
        {
            _context = context;
        }

        // TABLE VIEW
        public async Task<IActionResult> Index(string searchString, CourseCategory? categoryFilter)
        {
            var query = _context.Courses
                .Include(c => c.Prerequisite)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(c =>
                    c.Title.Contains(searchString) ||
                    c.Description.Contains(searchString) ||
                    (c.Prerequisite != null && c.Prerequisite.Title.Contains(searchString)));
            }

            if (categoryFilter.HasValue && categoryFilter.Value != CourseCategory.None)
            {
                query = query.Where(c => c.Category == categoryFilter.Value);
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = categoryFilter;

            PopulateCategoryFilter(categoryFilter);

            var courses = await query
                .OrderBy(c => c.Title)
                .ToListAsync();

            return View(courses);
        }

        // CARD VIEW
        public async Task<IActionResult> CardIndex(string searchString, CourseCategory? categoryFilter)
        {
            var query = _context.Courses
                .Include(c => c.Prerequisite)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                //searches in the title, description, and prerequisite title for the search string
                query = query.Where(c =>
                    c.Title.Contains(searchString) ||
                    c.Description.Contains(searchString) ||
                    (c.Prerequisite != null && c.Prerequisite.Title.Contains(searchString)));
            }

            if (categoryFilter.HasValue && categoryFilter.Value != CourseCategory.None)
            {
                query = query.Where(c => c.Category == categoryFilter.Value);
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = categoryFilter;

            PopulateCategoryFilter(categoryFilter);

            var courses = await query
                .OrderBy(c => c.Title)
                .ToListAsync();

            return View(courses);
        }

        // GET: Courses/Create
        public async Task<IActionResult> Create()
        {
            await PopulateFormDropdownsAsync();
            return View(new CourseFormViewModel());
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateFormDropdownsAsync(model.PrerequisiteId, model.Category);
                return View(model);
            }

            var course = new Course
            {
                Title = model.Title,
                Description = model.Description,
                Category = model.Category,
                Duration = model.Duration,
                Capacity = model.Capacity,
                Fee = model.Fee,
                PrerequisiteId = model.PrerequisiteId
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Course created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            var model = new CourseFormViewModel
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Category = course.Category,
                Duration = course.Duration,
                Capacity = course.Capacity,
                Fee = course.Fee,
                PrerequisiteId = course.PrerequisiteId
            };

            await PopulateFormDropdownsAsync(model.PrerequisiteId, model.Category, model.Id);
            return View(model);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseFormViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (model.PrerequisiteId == model.Id)
            {
                ModelState.AddModelError(nameof(model.PrerequisiteId), "A course cannot be its own prerequisite.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateFormDropdownsAsync(model.PrerequisiteId, model.Category, model.Id);
                return View(model);
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            course.Title = model.Title;
            course.Description = model.Description;
            course.Category = model.Category;
            course.Duration = model.Duration;
            course.Capacity = model.Capacity;
            course.Fee = model.Fee;
            course.PrerequisiteId = model.PrerequisiteId;

            try
            {
                _context.Update(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CourseExists(model.Id))
                    return NotFound();

                throw;
            }
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Prerequisite)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Prerequisite)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null) return NotFound();

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                TempData["ErrorMessage"] = "Course not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] =
                    "This course cannot be deleted because it is linked to other records such as sessions, prerequisites, tracks, or instructor expertise.";
            }

            return RedirectToAction(nameof(Index));
        }

        private void PopulateCategoryFilter(CourseCategory? selectedCategory)
        {
            var categories = Enum.GetValues(typeof(CourseCategory))
                .Cast<CourseCategory>()
                .Select(c => new SelectListItem
                {
                    Value = c.ToString(),
                    Text = c.ToString(),
                    Selected = selectedCategory.HasValue && c == selectedCategory.Value
                })
                .ToList();

            categories.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "All Categories",
                Selected = !selectedCategory.HasValue
            });

            ViewBag.CategoryFilterList = categories;
        }

        private async Task PopulateFormDropdownsAsync(
            int? selectedPrerequisiteId = null,
            CourseCategory? selectedCategory = null,
            int? excludeCourseId = null)
        {
            var prerequisiteQuery = _context.Courses
                .AsNoTracking()
                .OrderBy(c => c.Title)
                .AsQueryable();

            if (excludeCourseId.HasValue)
            {
                prerequisiteQuery = prerequisiteQuery.Where(c => c.Id != excludeCourseId.Value);
            }

            ViewBag.PrerequisiteId = new SelectList(
                await prerequisiteQuery.ToListAsync(),
                "Id",
                "Title",
                selectedPrerequisiteId
            );

            ViewBag.CategoryList = Enum.GetValues(typeof(CourseCategory))
                .Cast<CourseCategory>()
                .Where(c => c != CourseCategory.None)
                .Select(c => new SelectListItem
                {
                    Value = c.ToString(),
                    Text = c.ToString(),
                    Selected = selectedCategory.HasValue && c == selectedCategory.Value
                })
                .ToList();
        }

        private async Task<bool> CourseExists(int id)
        {
            return await _context.Courses.AnyAsync(e => e.Id == id);
        }
    }
}