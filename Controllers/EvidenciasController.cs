using gaco_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gaco_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EvidenciasController : ControllerBase
    {
        private readonly GacoDbContext _context;

        public EvidenciasController(GacoDbContext context)
        {
            _context = context;
        }

        // GET: api/Evidencias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Evidencia>>> GetEvidencias()
        {
            return await _context.Evidencias.ToListAsync();
        }

        // GET: api/Evidencias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Evidencia>> GetEvidencia(long id)
        {
            var evidencia = await _context.Evidencias.FindAsync(id);

            if (evidencia == null)
            {
                return NotFound();
            }

            return evidencia;
        }

        // PUT: api/Evidencias/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvidencia(long id, Evidencia evidencia)
        {
            if (id != evidencia.Id)
            {
                return BadRequest();
            }

            _context.Entry(evidencia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EvidenciaExists(id))
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

        // POST: api/Evidencias
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Evidencia>> PostEvidencia(Evidencia evidencia)
        {
            _context.Evidencias.Add(evidencia);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEvidencia", new { id = evidencia.Id }, evidencia);
        }

        // DELETE: api/Evidencias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvidencia(long id)
        {
            var evidencia = await _context.Evidencias.FindAsync(id);
            if (evidencia == null)
            {
                return NotFound();
            }

            _context.Evidencias.Remove(evidencia);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EvidenciaExists(long id)
        {
            return _context.Evidencias.Any(e => e.Id == id);
        }
    }
}
