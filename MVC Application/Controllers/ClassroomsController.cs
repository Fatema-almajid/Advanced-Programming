using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Application.Models.ViewModels;
using TrainingCertificationPlatform;
using TrainingCertificationPlatform.Models;
using Microsoft.AspNetCore.Authorization;

/* 
 * This controller manages the physical classroom infrastructure and asset allocations under the "TRAINING_COORDINATOR" role.
 * It provides asynchronous CRUD operations integrated with complex multi-field in-memory filtering (Search on Name, Seats, and child Equipment strings).
 * The codebase maintains data integrity through business-rule validations (preventing duplicate room names and verifying requested resource IDs),
 * manages many-to-many entity associations by dynamically clearing and re-mapping related infrastructure equipment components,
 * and implements defensive database exceptions handling (`DbUpdateException`) to safely capture and handle referential integrity conflicts.
 */

namespace MVC_Application.Controllers
{
    [Authorize(Roles = "TRAINING_COORDINATOR")]
    public class ClassroomsController : Controller
    {
        private readonly AppDbContext _context;

        public ClassroomsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var classrooms = await _context.Classrooms
                .Include(c => c.Equipments)
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();

            var model = classrooms.Select(c => new ClassroomListItemViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Seats = c.Seats,
                EquipmentNames = c.Equipments.Any()
                    ? string.Join(", ", c.Equipments.OrderBy(e => e.Name).Select(e => e.Name))
                    : "None"
            });

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                model = model.Where(r =>
                    r.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    r.EquipmentNames.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    r.Seats.ToString().Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;

            return View(model.ToList());
        }

        public async Task<IActionResult> Create()
        {
            await PopulateEquipmentDropdownAsync();
            return View(new ClassroomFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassroomFormViewModel model)
        {
            await ValidateClassroomAsync(model);

            if (!ModelState.IsValid)
            {
                await PopulateEquipmentDropdownAsync(model.SelectedEquipmentIds);
                return View(model);
            }

            var classroom = new Classroom
            {
                Name = model.Name.Trim(),
                Seats = model.Seats
            };

            if (model.SelectedEquipmentIds.Any())
            {
                classroom.Equipments = await _context.Equipments
                    .Where(e => model.SelectedEquipmentIds.Contains(e.Id))
                    .ToListAsync();
            }

            _context.Classrooms.Add(classroom);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Room created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var classroom = await _context.Classrooms
                .Include(c => c.Equipments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (classroom == null) return NotFound();

            var model = new ClassroomFormViewModel
            {
                Id = classroom.Id,
                Name = classroom.Name,
                Seats = classroom.Seats,
                SelectedEquipmentIds = classroom.Equipments.Select(e => e.Id).ToList()
            };

            await PopulateEquipmentDropdownAsync(model.SelectedEquipmentIds);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClassroomFormViewModel model)
        {
            if (id != model.Id) return NotFound();

            await ValidateClassroomAsync(model, id);

            if (!ModelState.IsValid)
            {
                await PopulateEquipmentDropdownAsync(model.SelectedEquipmentIds);
                return View(model);
            }

            var classroom = await _context.Classrooms
                .Include(c => c.Equipments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (classroom == null) return NotFound();

            classroom.Name = model.Name.Trim();
            classroom.Seats = model.Seats;

            classroom.Equipments.Clear();

            if (model.SelectedEquipmentIds.Any())
            {
                var selectedEquipments = await _context.Equipments
                    .Where(e => model.SelectedEquipmentIds.Contains(e.Id))
                    .ToListAsync();

                foreach (var equipment in selectedEquipments)
                {
                    classroom.Equipments.Add(equipment);
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Room updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var classroom = await _context.Classrooms
                .Include(c => c.Equipments)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (classroom == null) return NotFound();

            return View(classroom);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var classroom = await _context.Classrooms
                .Include(c => c.Equipments)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (classroom == null) return NotFound();

            return View(classroom);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classroom = await _context.Classrooms
                .Include(c => c.Equipments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (classroom == null)
            {
                TempData["ErrorMessage"] = "Room not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                classroom.Equipments.Clear();
                _context.Classrooms.Remove(classroom);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Room deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] =
                    "This room cannot be deleted because it is linked to other records such as sessions.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateEquipmentDropdownAsync(List<int>? selectedEquipmentIds = null)
        {
            ViewBag.EquipmentList = new MultiSelectList(
                await _context.Equipments
                    .AsNoTracking()
                    .OrderBy(e => e.Name)
                    .ToListAsync(),
                "Id",
                "Name",
                selectedEquipmentIds);
        }

        private async Task ValidateClassroomAsync(ClassroomFormViewModel model, int? currentId = null)
        {
            var trimmedName = model.Name.Trim();

            var roomNameExists = await _context.Classrooms.AnyAsync(c =>
                c.Name == trimmedName &&
                (!currentId.HasValue || c.Id != currentId.Value));

            if (roomNameExists)
            {
                ModelState.AddModelError(nameof(model.Name), "This room name already exists.");
            }

            var validEquipmentIds = await _context.Equipments
                .Where(e => model.SelectedEquipmentIds.Contains(e.Id))
                .Select(e => e.Id)
                .ToListAsync();

            if (validEquipmentIds.Count != model.SelectedEquipmentIds.Distinct().Count())
            {
                ModelState.AddModelError(nameof(model.SelectedEquipmentIds), "One or more selected equipment items are invalid.");
            }
        }
    }
}