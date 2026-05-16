// for public certification lookup

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCertificationPlatform.Models;

namespace TrainingCertificationPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CertificationApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CertificationApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("verify")]
        public async Task<IActionResult> Verify(int traineeId, string referenceNumber)
        {
            var certification = await _context.TraineeCertifications
                .Include(tc => tc.Trainee)
                .Include(tc => tc.Track)
                .FirstOrDefaultAsync(tc =>
                    tc.TraineeId == traineeId &&
                    tc.CertificateReferenceNumber == referenceNumber);

            if (certification == null)
            {
                return NotFound(new
                {
                    message = "Certification not found."
                });
            }

            var completedCourses = await _context.Assessments
                .Where(a =>
                    a.Enrollment.TraineeId == traineeId &&
                    a.Status == AssessmentStatus.PASS)
                .Select(a => a.Enrollment.Session.Course.Title)
                .Distinct()
                .ToListAsync();

            return Ok(new
            {
                traineeName = certification.Trainee.FirstName + " " + certification.Trainee.LastName,
                track = certification.Track.Name,
                status = certification.Status.ToString(),
                completedCourses = completedCourses,

                certificateReference = certification.CertificateReferenceNumber,
                verificationDate = DateTime.Now,
                totalCompletedCourses = completedCourses.Count
            });
        }
    }
}