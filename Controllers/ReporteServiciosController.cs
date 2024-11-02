using gaco_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gaco_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReporteServiciosController : ControllerBase
    {
        private readonly GacoDbContext _context;

        public ReporteServiciosController(GacoDbContext context)
        {
            _context = context;
        }

        // GET: api/ReporteServicios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReporteServicio>>> GetReporteServicios()
        {
            return await _context.ReporteServicios.ToListAsync();
        }

        // GET: api/ReporteServicios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReporteServicio>> GetReporteServicio(long id)
        {
            var reporteServicio = await _context.ReporteServicios.FindAsync(id);

            if (reporteServicio == null)
            {
                return NotFound();
            }

            return reporteServicio;
        }

        // PUT: api/ReporteServicios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReporteServicio(long id, ReporteServicio reporteServicio)
        {
            if (id != reporteServicio.Id)
            {
                return BadRequest();
            }

            _context.Entry(reporteServicio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReporteServicioExists(id))
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

        // POST: api/ReporteServicios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ReporteServicio>> PostReporteServicio(ReporteServicio reporteServicio)
        {
            _context.ReporteServicios.Add(reporteServicio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReporteServicio", new { id = reporteServicio.Id }, reporteServicio);
        }

        // DELETE: api/ReporteServicios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReporteServicio(long id)
        {
            var reporteServicio = await _context.ReporteServicios.FindAsync(id);
            if (reporteServicio == null)
            {
                return NotFound();
            }

            _context.ReporteServicios.Remove(reporteServicio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReporteServicioExists(long id)
        {
            return _context.ReporteServicios.Any(e => e.Id == id);
        }
    }
}
