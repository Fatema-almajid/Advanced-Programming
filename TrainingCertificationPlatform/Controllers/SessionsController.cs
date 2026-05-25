using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCertificationPlatform.Models;
using Microsoft.AspNetCore.Authorization;

namespace TrainingCertificationPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SessionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Sessions
        [HttpGet]
        [Authorize(Roles = "INSTRUCTOR,TRAINING_COORDINATOR")]
        public async Task<ActionResult<IEnumerable<Session>>> GetSessions()
        {
            return await _context.Sessions.ToListAsync();
        }

        // GET: api/Sessions/5
        [HttpGet("{id}")]
        [Authorize(Roles = "INSTRUCTOR,TRAINING_COORDINATOR")]
        public async Task<ActionResult<Session>> GetSession(int id)
        {
            var session = await _context.Sessions.FindAsync(id);

            if (session == null)
            {
                return NotFound();
            }

            return session;
        }
        // POST: api/Sessions
        [HttpPost]
        [Authorize(Roles = "TRAINING_COORDINATOR")]
        public async Task<ActionResult<Session>> CreateSession([FromBody] Session session)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSession), new { id = session.Id }, session);
        }
    }
}
