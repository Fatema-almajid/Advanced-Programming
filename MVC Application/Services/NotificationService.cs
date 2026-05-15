using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MVC_Application.Hubs;
using TrainingCertificationPlatform;
using TrainingCertificationPlatform.Models;

namespace MVC_Application.Services
{
    public class NotificationService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _notificationHub;

        public NotificationService(AppDbContext context, IHubContext<NotificationHub> notificationHub)
        {
            _context = context;
            _notificationHub = notificationHub;
        }

        public async Task CreateNotificationAsync(int userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                CreatedDate = DateTime.Now,
                Status = NotificationStatus.UNREAD
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await _notificationHub.Clients
                .Group($"user-{userId}")
                .SendAsync("ReceiveNotification", new
                {
                    id = notification.Id,
                    message = notification.Message,
                    createdDate = notification.CreatedDate,
                    status = notification.Status.ToString()
                });
        }

        public async Task CreateOverduePaymentNotificationsForUserAsync(int traineeId)
        {
            var today = DateTime.Today;

            var overdueBalances = await _context.Balances
                .Include(b => b.Enrollment)
                    .ThenInclude(e => e.Session)
                        .ThenInclude(s => s.Course)
                .Where(b =>
                    b.Enrollment.TraineeId == traineeId &&
                    b.AmountDue > 0 &&
                    b.Status == BalanceStatus.OVERDUE)
                .ToListAsync();

            foreach (var balance in overdueBalances)
            {
                var courseTitle = balance.Enrollment.Session.Course.Title;

                var alreadyNotifiedToday = await _context.Notifications
                    .AnyAsync(n =>
                        n.UserId == traineeId &&
                        n.Message.Contains("overdue payment") &&
                        n.Message.Contains(courseTitle) &&
                        n.CreatedDate.Date == today);

                if (alreadyNotifiedToday)
                {
                    continue;
                }

                await CreateNotificationAsync(
                    traineeId,
                    $"You have an overdue payment for {courseTitle}. Remaining balance: BHD {balance.AmountDue}."
                );
            }
        }
    }
}