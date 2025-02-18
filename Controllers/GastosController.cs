using gaco_api.Customs;
using gaco_api.Models;
using gaco_api.Models.DTOs.Requests.Gastos;
using gaco_api.Models.DTOs.Responses.Gastos;
using gaco_api.Models.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
                Factura = g.Factura,
                IdUsuarioCreacion = g.IdUsuarioCreacion,
                NombreUsuarioCreacion = g.IdUsuarioCreacionNavigation.Nombres + " " + g.IdUsuarioCreacionNavigation.Apellidos,
                RutaPdffactura = g.RutaPdffactura,
                RutaXmlfactura = g.RutaXmlfactura,
                Concepto = g.Concepto,
                Descripcion = g.Descripcion,
                Fecha = g.Fecha,
                Monto = g.Monto
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
                    Factura = g.Factura,
                    FechaCreacion = g.FechaCreacion,
                    FechaModificacion = g.FechaModificacion,
                    IdCatEstatus = g.IdCatEstatus,
                    IdUsuarioCreacion = g.IdUsuarioCreacion,
                    RutaPdffactura = g.RutaPdffactura,
                    RutaXmlfactura = g.RutaXmlfactura,
                    Monto = g.Monto,
                    Fecha = g.Fecha,
                    Descripcion = g.Descripcion,
                    Concepto = g.Concepto
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
                    Fecha = request.Fecha,
                    Descripcion = request.Descripcion,
                    Monto = request.Monto,
                    Factura = request.Factura,
                    IdUsuarioCreacion = userId,
                    RutaPdffactura = request.RutaPdffactura,
                    RutaXmlfactura = request.RutaXmlfactura,
                    IdCatEstatus = 1,
                    FechaCreacion = DateTime.UtcNow,
                };

                await _context.Gastos.AddAsync(nuevoGasto);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new DefaultResponse<object> { Success = true, Message = "Gasto registrado correctamente." });
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
            var gasto = await _context.Gastos.FindAsync(request.Id);
            if (gasto == null)
            {
                return NotFound(new DefaultResponse<object> { Success = false, Message = "Gasto no encontrado." });
            }

            gasto.Concepto = request.Concepto;
            gasto.Fecha = request.Fecha;
            gasto.Descripcion = request.Descripcion;
            gasto.Monto = request.Monto;
            gasto.Factura = request.Factura;
            gasto.RutaPdffactura = request.RutaPdffactura;
            gasto.RutaXmlfactura = request.RutaXmlfactura;
            gasto.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new DefaultResponse<object> { Success = true, Message = "Gasto actualizado correctamente." });
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
