using gaco_api.Customs;
using gaco_api.Models;
using gaco_api.Models.DTOs.Requests.Gastos;
using gaco_api.Models.DTOs.Responses.Gastos;
using gaco_api.Models.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace gaco_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GastosController : ControllerBase
    {
        private readonly GacoDbContext _context;
        private readonly Utilidades _utilidades;

        public GastosController(GacoDbContext context, Utilidades utilidades)
        {
            _context = context;
            _utilidades = utilidades;
        }

        [HttpPost]
        [Route("Busqueda")]
        public async Task<IActionResult> BusquedaGasto(BusquedaGastoRequest request)
        {
            var query = _context.Gastos
                .Include(g => g.IdCatEstatusNavigation)
                .AsQueryable();

            if (request.CantidadPorPagina == -1)
            {
                request.CantidadPorPagina = await _context.Gastos.CountAsync(x => x.IdCatEstatus == 1);
            }

            var gastos = await query.Select(g => new GastoResponse
            {
                Id = g.Id,
                FechaCreacion = g.FechaCreacion,
                FechaModificacion = g.FechaModificacion,
                IdCatEstatus = g.IdCatEstatus,
                NombreCatEstatus = g.IdCatEstatusNavigation.Estatus,
                IdUsuarioCreacion = g.IdUsuarioCreacion,
                NombreUsuarioCreacion = g.IdUsuarioCreacionNavigation.Nombres + " " + g.IdUsuarioCreacionNavigation.Apellidos,
                Concepto = g.Concepto,
                Monto = g.DetGastos.Sum(x => x.Monto)
            })
            .Where(x => x.IdCatEstatus == 1)
            .Skip((request.NumeroPagina - 1) * request.CantidadPorPagina)
            .Take(request.CantidadPorPagina)
            .ToListAsync();

            return Ok(new DefaultResponse<List<GastoResponse>> { Success = true, Data = gastos });
        }

        [HttpGet]
        [Route("PorId/{id}")]
        public async Task<IActionResult> GastoPorId(long id)
        {
            var gasto = await _context.Gastos
                .Where(g => g.Id == id)
                .Select(g => new EditarGastoResponse
                {
                    Id = g.Id,
                    FechaCreacion = g.FechaCreacion,
                    FechaModificacion = g.FechaModificacion,
                    IdCatEstatus = g.IdCatEstatus,
                    IdUsuarioCreacion = g.IdUsuarioCreacion,
                    Concepto = g.Concepto,
                    DetGastos = g.DetGastos.Select(d => new DetGastoResponse
                    {
                        Id = d.Id,
                        IdGasto = d.IdGasto,
                        Descripcion = d.Descripcion,
                        Factura = d.Factura,
                        Fecha = d.Fecha,
                        IdCatEstatus = d.IdCatEstatus,
                        RutaPdffactura = d.RutaPdffactura,
                        RutaXmlfactura = d.RutaXmlfactura,
                        Monto = d.Monto,
                        FechaCreacion = d.FechaCreacion,
                        FechaModificacion = d.FechaModificacion,
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (gasto == null)
            {
                return NotFound(new DefaultResponse<object> { Success = false, Message = "No se encontró el gasto." });
            }

            return Ok(new DefaultResponse<EditarGastoResponse> { Success = true, Data = gasto });
        }

        [HttpPost]
        [Route("Nuevo")]
        public async Task<IActionResult> NuevoGasto(NuevoGastoRequest request)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(nameIdentifier, out long userId) || userId <= 0)
            {
                return BadRequest(new DefaultResponse<object> { Success = false, Message = "Usuario no válido." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var nuevoGasto = new Gasto
                {
                    Concepto = request.Concepto,
                    IdUsuarioCreacion = userId,
                    IdCatEstatus = 1,
                    FechaCreacion = DateTime.UtcNow,
                };

                await _context.Gastos.AddAsync(nuevoGasto);
                await _context.SaveChangesAsync();

                // Insertar los detalles del gasto si existen
                if (request.DetGastos != null && request.DetGastos.Any())
                {
                    var detallesGasto = request.DetGastos.Select(detalle => new DetGasto
                    {
                        IdGasto = nuevoGasto.Id, // Asignamos el ID del gasto recién creado
                        Fecha = detalle.Fecha,
                        Descripcion = detalle.Descripcion,
                        Monto = detalle.Monto,
                        Factura = detalle.Factura,
                        RutaPdffactura = detalle.RutaPdffactura,
                        RutaXmlfactura = detalle.RutaXmlfactura,
                        IdCatEstatus = 1,
                        FechaCreacion = DateTime.UtcNow,
                    }).ToList();

                    await _context.DetGastos.AddRangeAsync(detallesGasto);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return Ok(new DefaultResponse<object> { Success = true, Message = "Gasto y detalles registrados correctamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new DefaultResponse<object> { Success = false, Message = ex.Message });
            }
        }


        [HttpPost]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarGasto(ActualizarGastoRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var gasto = await _context.Gastos
                    .Include(g => g.DetGastos) // Cargamos los detalles relacionados
                    .FirstOrDefaultAsync(g => g.Id == request.Id);

                if (gasto == null)
                {
                    return NotFound(new DefaultResponse<object> { Success = false, Message = "Gasto no encontrado." });
                }

                // Actualizar el gasto
                gasto.Concepto = request.Concepto;
                gasto.FechaModificacion = DateTime.UtcNow;

                // Eliminar todos los detalles existentes
                var listaDetalles = await _context.DetGastos.Where(d => d.IdGasto == gasto.Id).ToListAsync();
                _context.DetGastos.RemoveRange(listaDetalles);  // Eliminar los detalles obtenidos
                await _context.SaveChangesAsync(); // Guardar la eliminación antes de insertar los nuevos

                // Insertar los nuevos detalles
                if (request.DetGastos != null && request.DetGastos.Any())
                {
                    var nuevosDetalles = request.DetGastos.Select(detalle => new DetGasto
                    {
                        IdGasto = gasto.Id,
                        Fecha = detalle.Fecha,
                        Descripcion = detalle.Descripcion,
                        Monto = detalle.Monto,
                        Factura = detalle.Factura,
                        RutaPdffactura = detalle.RutaPdffactura,
                        RutaXmlfactura = detalle.RutaXmlfactura,
                        IdCatEstatus = detalle.IdCatEstatus,
                        FechaCreacion = DateTime.UtcNow
                    }).ToList();

                    await _context.DetGastos.AddRangeAsync(nuevosDetalles);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new DefaultResponse<object> { Success = true, Message = "Gasto y detalles actualizados correctamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new DefaultResponse<object> { Success = false, Message = ex.Message });
            }
        }


        [HttpGet]
        [Route("Eliminar/{id}")]
        public async Task<IActionResult> EliminarGasto(long id)
        {
            var gasto = await _context.Gastos
                .FirstOrDefaultAsync(g => g.Id == id);

            if (gasto == null)
            {
                return NotFound(new DefaultResponse<object> { Success = false, Message = "Gasto no encontrado." });
            }

            gasto.IdCatEstatus = 2;
            gasto.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new DefaultResponse<object> { Success = true, Message = "Gasto eliminado correctamente." });
        }

    }
}
