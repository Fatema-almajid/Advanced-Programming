using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using TrainingCertificationPlatform.Models;

namespace TrainingCertificationPlatform.Services
{
    public class SessionSchedulingService
    {
        private readonly AppDbContext _context;
        
        public SessionSchedulingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, string>> ValidateSessionAsync(
            int courseId,
            int instructorId,
            int classroomId,
            DateTime sessionDate,
            TimeOnly startTime,
            TimeOnly endTime,
            int? currentSessionId = null) {
                var errors = new Dictionary<string, string>();

            //validate course capacity
            var courseCapacity = await _context.Courses
                .Where(c => c.Id == courseId)
                .Select(c => c.Capacity)
                .FirstOrDefaultAsync();

            if (courseCapacity == 0)
            {
                errors.Add(nameof(courseId), "Selected course does not have available seats.");
                return errors;
            }

            //validate end time is after start time
            if (endTime <= startTime)
            {
                errors.Add(nameof(endTime), "End time must be later than start time.");
                return errors;
            }

            //validate session date is not in the past
            if (sessionDate.Date < DateTime.Today)
            {
                errors.Add(nameof(sessionDate),"Session date cannot be in the past.");
            }

            //validate instructor availability
            var instructorConflict = await _context.Sessions
                .AnyAsync(s => 
                       s.InstructorId == instructorId 
                    && s.SessionDate.Date == sessionDate.Date
                    && (!currentSessionId.HasValue || s.Id != currentSessionId.Value)
                    && startTime < s.EndTime && endTime > s.StartTime);

            if (instructorConflict)
            {
                errors.Add(nameof(instructorId),"This instructor is already booked for another session during the selected time.");
            }

            //validate classroom availability
            var classroomConflict = await _context.Sessions
                .AnyAsync(s => 
                       s.ClassroomId == classroomId 
                    && s.SessionDate.Date == sessionDate.Date
                    && (!currentSessionId.HasValue || s.Id != currentSessionId.Value)
                    && startTime < s.EndTime && endTime > s.StartTime);

            if (classroomConflict) {
                errors.Add(nameof(classroomId),"This room is already booked for another session during the selected time.");
            }

            


            return errors;
        }
    }
}
