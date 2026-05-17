using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TrainingCertificationPlatform;
using TrainingCertificationPlatform.Models;

namespace MVC_Application.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Get the specific notification from the database for the current user
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            // If the notification doesn't exist or doesn't belong to the user,
            if (notification == null)
            {
                // return a 404 Not Found response
                return NotFound();
            }

            // Mark the notification as read
            notification.Status = NotificationStatus.READ;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}