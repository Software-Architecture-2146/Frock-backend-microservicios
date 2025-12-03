using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using suscriptions.shared.Infrastructure.Persistence.EFC.Configuration; // Tu DbContext
using Frock_backend.suscriptions.domain.model.aggregates;
using Microsoft.AspNetCore.Authorization; // Tu Entidad

namespace Frock_backend.suscriptions.Interfaces.REST 
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")] 
    public class SuscriptionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SuscriptionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Suscription>>> GetAll()
        {
            return await _context.Suscriptions.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Suscription>> GetById(int id)
        {
            var suscription = await _context.Suscriptions.FindAsync(id);

            if (suscription == null)
            {
                return NotFound();
            }

            return suscription;
        }
    }
}