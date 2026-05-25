using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using TrainingCertificationPlatform;
using TrainingCertificationPlatform.Models;

namespace MVC_Application.Services
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
    int? currentSessionId = null)
        {
            var errors = new Dictionary<string, string>();

            // 1. Validate selected course
            var course = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                errors.Add("CourseId", "Selected course is invalid.");
                return errors;
            }

            // 2. Validate selected instructor
            var instructor = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == instructorId && u.Role == UserRole.INSTRUCTOR);

            if (instructor == null)
            {
                errors.Add("InstructorId", "Selected instructor is invalid.");
                return errors;
            }

            // 3. Validate selected classroom
            var classroom = await _context.Classrooms
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == classroomId);

            if (classroom == null)
            {
                errors.Add("ClassroomId", "Selected room is invalid.");
                return errors;
            }

            // 4. Validate end time is after start time
            if (endTime <= startTime)
            {
                errors.Add("EndTime", "End time must be later than start time.");
                return errors;
            }

            // 5. Validate session date is not in the past
            if (sessionDate.Date < DateTime.Today)
            {
                errors.Add("SessionDate", "Session date cannot be in the past.");
            }

            // 6. Validate classroom capacity
            if (classroom.Seats < course.Capacity)
            {
                errors.Add("ClassroomId",
                    $"This room cannot be assigned because it has only {classroom.Seats} seats while the course capacity is {course.Capacity}.");
            }

            // 7. Validate instructor expertise/course subject
            var instructorCanTeachCourse = await _context.InstructorExpertises
                .AsNoTracking()
                .AnyAsync(e => e.InstructorId == instructorId && e.CourseId == courseId);

            if (!instructorCanTeachCourse)
            {
                errors.Add("InstructorId",
                    "This instructor is not assigned as an expert for the selected course.");
            }

            // 8. Convert C# DayOfWeek to your custom Day enum
            var sessionDay = sessionDate.DayOfWeek switch
            {
                DayOfWeek.Sunday => Day.SUNDAY,
                DayOfWeek.Monday => Day.MONDAY,
                DayOfWeek.Tuesday => Day.TUESDAY,
                DayOfWeek.Wednesday => Day.WEDNESDAY,
                DayOfWeek.Thursday => Day.THURSDAY,
                DayOfWeek.Friday => Day.FRIDAY,
                _ => Day.SATURDAY
            };

            // 9. Validate instructor availability
            var availability = await _context.InstructorAvailabilities
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.InstructorId == instructorId);

            if (availability == null)
            {
                errors.Add("InstructorId",
                    "This instructor does not have an availability schedule yet.");
            }
            else
            {
                bool dayIsAvailable =
                    sessionDay >= availability.DayStart &&
                    sessionDay <= availability.DayEnd;

                bool timeIsAvailable =
                    startTime >= availability.StartTime &&
                    endTime <= availability.EndTime;

                if (!dayIsAvailable || !timeIsAvailable)
                {
                    errors.Add("StartTime",
                        $"This instructor is only available from {availability.DayStart} to {availability.DayEnd}, between {availability.StartTime} and {availability.EndTime}.");
                }
            }

            // 10. Validate instructor double-booking
            var instructorConflict = await _context.Sessions
                .AnyAsync(s =>
                    s.InstructorId == instructorId &&
                    s.SessionDate.Date == sessionDate.Date &&
                    (!currentSessionId.HasValue || s.Id != currentSessionId.Value) &&
                    startTime < s.EndTime &&
                    endTime > s.StartTime);

            if (instructorConflict)
            {
                errors.Add("InstructorId",
                    "This instructor is already booked for another session during the selected time.");
            }

            // 11. Validate classroom double-booking
            var classroomConflict = await _context.Sessions
                .AnyAsync(s =>
                    s.ClassroomId == classroomId &&
                    s.SessionDate.Date == sessionDate.Date &&
                    (!currentSessionId.HasValue || s.Id != currentSessionId.Value) &&
                    startTime < s.EndTime &&
                    endTime > s.StartTime);

            if (classroomConflict)
            {
                errors.Add("ClassroomId",
                    "This room is already booked for another session during the selected time.");
            }

            return errors;
        }

    }
}
