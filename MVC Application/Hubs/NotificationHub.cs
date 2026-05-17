using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using TrainingCertificationPlatform.Models;

namespace MVC_Application.Hubs
{
    public class NotificationHub : Hub
    {
        // For sending notifications to specific users based on their user ID
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            }

            await base.OnConnectedAsync();
        }

        // Remove the user from their group when they disconnect
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}