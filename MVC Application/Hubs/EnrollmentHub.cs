using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MVC_Application.Hubs
{
    [Authorize]
    public class EnrollmentHub : Hub
    {
        public async Task JoinCourseGroup(int courseId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"course-{courseId}");
        }

        public async Task LeaveCourseGroup(int courseId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"course-{courseId}");
        }

        public async Task JoinSessionGroup(int sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"session-{sessionId}");
        }

        public async Task LeaveSessionGroup(int sessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"session-{sessionId}");
        }
    }
}