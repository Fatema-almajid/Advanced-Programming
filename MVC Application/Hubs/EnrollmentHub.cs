using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MVC_Application.Hubs
{
    [Authorize]
    public class EnrollmentHub : Hub
    {
        //For trainees
        public async Task JoinCourseGroup(int courseId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"course-{courseId}");
        }

        //For instructors
        public async Task JoinSessionGroup(int sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"session-{sessionId}");
        }
    }
}