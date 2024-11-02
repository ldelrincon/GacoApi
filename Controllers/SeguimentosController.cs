using gaco_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gaco_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SeguimentosController : ControllerBase
    {
        private readonly GacoDbContext _context;

        public SeguimentosController(GacoDbContext context)
        {
            _context = context;
        }

        // GET: api/Seguimentos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Seguimento>>> GetSeguimentos()
        {
            return await _context.Seguimentos.ToListAsync();
        }

        // GET: api/Seguimentos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Seguimento>> GetSeguimento(long id)
        {
            var seguimento = await _context.Seguimentos.FindAsync(id);

            if (seguimento == null)
            {
                return NotFound();
            }

            return seguimento;
        }

        // PUT: api/Seguimentos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSeguimento(long id, Seguimento seguimento)
        {
            if (id != seguimento.Id)
            {
                return BadRequest();
            }

            _context.Entry(seguimento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SeguimentoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Seguimentos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Seguimento>> PostSeguimento(Seguimento seguimento)
        {
            _context.Seguimentos.Add(seguimento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSeguimento", new { id = seguimento.Id }, seguimento);
        }

        // DELETE: api/Seguimentos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeguimento(long id)
        {
            var seguimento = await _context.Seguimentos.FindAsync(id);
            if (seguimento == null)
            {
                return NotFound();
            }

            _context.Seguimentos.Remove(seguimento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SeguimentoExists(long id)
        {
            return _context.Seguimentos.Any(e => e.Id == id);
        }
    }
}
