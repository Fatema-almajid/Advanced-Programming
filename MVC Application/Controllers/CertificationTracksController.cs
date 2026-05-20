using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Application.Models.ViewModels;
using TrainingCertificationPlatform;
using TrainingCertificationPlatform.Models;

/* 
 * This controller implements full CRUD (Create, Read, Update, Delete) operations for managing Certification Tracks,
 * restricted strictly to the "TRAINING_COORDINATOR" role via the dynamic [Authorize] attribute. 
 * It manages many-to-many relationships by linking tracks with multiple corresponding courses using Entity Framework Core,
 * optimizes database lookups using eager loading (.Include) and non-tracking queries (.AsNoTracking) for data presentation,
 * and incorporates advanced server-side capabilities such as full-text search filtering and cross-site request forgery protection ([ValidateAntiForgeryToken]).
 */

namespace MVC_Application.Controllers
{
    [Authorize(Roles = "TRAINING_COORDINATOR")]
    public class CertificationTracksController : Controller
    {
        private readonly AppDbContext _context;

        public CertificationTracksController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.Tracks
                .Include(t => t.Courses)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(t =>
                    t.Name.Contains(searchString) ||
                    t.Description.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;

            var tracks = await query
                .OrderBy(t => t.Name)
                .ToListAsync();

            return View(tracks);
        }

        public async Task<IActionResult> CardIndex(string searchString)
        {
            var query = _context.Tracks
                .Include(t => t.Courses)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(t =>
                    t.Name.Contains(searchString) ||
                    t.Description.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;

            var tracks = await query
                .OrderBy(t => t.Name)
                .ToListAsync();

            return View(tracks);
        }

        public async Task<IActionResult> Details(int id)
        {
            var track = await _context.Tracks
                .Include(t => t.Courses)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (track == null)
                return NotFound();

            return View(track);
        }

        public async Task<IActionResult> Create()
        {
            var model = new CertificationTrackFormViewModel
            {
                CourseOptions = await GetCourseOptionsAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CertificationTrackFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CourseOptions = await GetCourseOptionsAsync();
                return View(model);
            }

            var selectedCourses = await _context.Courses
                .Where(c => model.SelectedCourseIds.Contains(c.Id))
                .ToListAsync();

            var track = new Track
            {
                Name = model.Name,
                Description = model.Description,
                Courses = selectedCourses
            };

            _context.Tracks.Add(track);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Certification track created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var track = await _context.Tracks
                .Include(t => t.Courses)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (track == null)
                return NotFound();

            var model = new CertificationTrackFormViewModel
            {
                Id = track.Id,
                Name = track.Name,
                Description = track.Description,
                SelectedCourseIds = track.Courses.Select(c => c.Id).ToList(),
                CourseOptions = await GetCourseOptionsAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CertificationTrackFormViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                model.CourseOptions = await GetCourseOptionsAsync();
                return View(model);
            }

            var track = await _context.Tracks
                .Include(t => t.Courses)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (track == null)
                return NotFound();

            track.Name = model.Name;
            track.Description = model.Description;

            track.Courses.Clear();

            var selectedCourses = await _context.Courses
                .Where(c => model.SelectedCourseIds.Contains(c.Id))
                .ToListAsync();

            foreach (var course in selectedCourses)
            {
                track.Courses.Add(course);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Certification track updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var track = await _context.Tracks
                .Include(t => t.Courses)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (track == null)
                return NotFound();

            return View(track);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var track = await _context.Tracks
                .Include(t => t.Courses)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (track == null)
                return NotFound();

            track.Courses.Clear();
            _context.Tracks.Remove(track);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Certification track deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> GetCourseOptionsAsync()
        {
            return await _context.Courses
                .OrderBy(c => c.Title)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Title
                })
                .ToListAsync();
        }
    }
}