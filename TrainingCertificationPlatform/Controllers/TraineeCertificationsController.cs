using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCertificationPlatform.Models;

namespace TrainingCertificationPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraineeCertificationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TraineeCertificationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TraineeCertifications
        [HttpGet]
        [Authorize(Roles = "INSTRUCTOR,TRAINING_COORDINATOR")]
        public async Task<ActionResult<IEnumerable<TraineeCertification>>> GetTraineeCertifications()
        {
            return await _context.TraineeCertifications.ToListAsync();
        }

        // GET: api/TraineeCertifications/5
        [HttpGet("{id}")]
        [Authorize(Roles = "INSTRUCTOR,TRAINING_COORDINATOR")]
        public async Task<ActionResult<TraineeCertification>> GetTraineeCertification(int id)
        {
            var traineeCertification = await _context.TraineeCertifications.FindAsync(id);

            if (traineeCertification == null)
            {
                return NotFound();
            }

            return traineeCertification;
        }
        //public certification refrence
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateCertificate(int trackId)
        {
            var traineeEmail = User.Identity!.Name;

            var trainee = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == traineeEmail);

            if (trainee == null)
            {
                return NotFound();
            }

            var existingCertificate = await _context.TraineeCertifications
                .FirstOrDefaultAsync(tc =>
                    tc.TraineeId == trainee.Id &&
                    tc.TrackId == trackId);

            if (existingCertificate != null)
            {
                return RedirectToAction("Certification");
            }

            var certification = new TraineeCertification
            {
                TraineeId = trainee.Id,
                TrackId = trackId,
                Status = TraineeCertificationStatus.SUCCESS,

                CertificateReferenceNumber =
                    "CERT-" + Guid.NewGuid().ToString("N")
                    .Substring(0, 8)
                    .ToUpper()
            };

            _context.TraineeCertifications.Add(certification);

            await _context.SaveChangesAsync();

            return RedirectToAction("Certification");
        }
    }
}
