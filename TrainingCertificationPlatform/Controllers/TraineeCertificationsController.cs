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
    }
}
