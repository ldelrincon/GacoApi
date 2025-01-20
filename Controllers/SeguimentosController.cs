using gaco_api.Customs;
using gaco_api.Models;
using gaco_api.Models.DTOs.Responses;
using gaco_api.Models.DTOs.Responses.Evidencias;
using gaco_api.Models.DTOs.Responses.Relaciones;
using gaco_api.Models.DTOs.Responses.Seguimientos;
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
        private readonly Utilidades _utilidades;

        public SeguimentosController(GacoDbContext context, Utilidades utilidades)
        {
            _context = context;
            _utilidades = utilidades;
        }

        [HttpGet]
        [Route("ReporteServicioSeguimentoPorId/{id}")]
        public async Task<IActionResult> ReporteServicioSeguimentoPorId(long id)
        {
            var response = new DefaultResponse<List<SeguimientoResponse>>();

            var seguimento = await _context.Seguimentos
                .Include(x => x.Evidencia)
                .Include(x => x.RelSeguimentoProductos).ThenInclude(x => x.IdProductoNavigation)
                .Where(x => x.IdReporteServicio == id)
                .OrderBy(x => x.FechaCreacion)
                .Select(x => new SeguimientoResponse
                {
                    Id = x.Id,
                    FechaCreacion = x.FechaCreacion,
                    DescripcionProximaVisita = x.DescripcionProximaVisita,
                    Estatus = x.IdCatEstatusNavigation.Estatus,
                    FechaModificacion = x.FechaModificacion,
                    IdCatEstatus = x.IdCatEstatus,
                    IdReporteServicio = x.IdReporteServicio,
                    IdUsuario = x.IdUsuario,
                    ProximaVisita =x.ProximaVisita,
                    Seguimento1 = x.Seguimento1,
                    TituloReporteServicio = x.IdReporteServicioNavigation.Titulo,
                    Usuario = $"{x.IdUsuarioNavigation.Nombres} {x.IdUsuarioNavigation.Apellidos}",
                    Evidencias = x.Evidencia.Select(e=> new EvidenciaResponse
                    {
                        Extension = e.Extension ?? "",
                        FechaCreacion = e.FechaCreacion,
                        FechaModificacion = e.FechaModificacion,
                        Id = e.Id,
                        IdCatEstatus = e.IdCatEstatus,
                        IdSeguimento = e.IdSeguimento,
                        Nombre = e.Nombre,
                        Ruta = _utilidades.GetFullUrl(e.Ruta),
                    }).ToList(),
                    Productos = x.RelSeguimentoProductos.Select(p => new RelSeguimentoProductoResponse
                    {
                        Cantidad = p.Cantidad,
                        FechaCreacion = p.FechaCreacion,
                        FechaModificacion = p.FechaModificacion,
                        Id = p.IdProducto,
                        IdCatEstatus = p.IdCatEstatus,
                        IdProducto = p.IdProducto,
                        IdSeguimento = p.IdSeguimento,
                        IdUsuario = p.IdUsuario,
                        MontoGasto = p.MontoGasto,
                        MontoVenta = p.MontoVenta,
                        Codigo = p.IdProductoNavigation.Codigo,
                        Producto = p.IdProductoNavigation.Producto1,
                        Unidad = p.Unidad,
                        Porcentaje = p.Porcentaje,
                    }).ToList()

                }).ToListAsync();

            if (seguimento != null)
            {
                response.Success = true;
                response.Data = seguimento;
            }
            else
            {
                response.Success = false;
                response.Message = "No se encontro el Reporte Servicio.";
            }

            return Ok(response);
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
