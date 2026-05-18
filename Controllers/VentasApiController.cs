using gaco_api.Models;
using gaco_api.Models.DTOs.Requests.ReporteSolicitudes;
using gaco_api.Models.DTOs.Requests.Ventas;
using gaco_api.Models.DTOs.Responses.Ventas;
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
    public class VentasApiController : ControllerBase
    {
        private readonly GacoDbContext _context;

        public VentasApiController(GacoDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("BusquedaFiltros")]
        public async Task<IActionResult> BusquedaFiltrosVenta(BusquedaReporteFiltrosServicioRequest request)
        {
            try
            {
                var query = _context.Ventas
                    .Include(v => v.IdClienteNavigation)
                    .Include(v => v.EstatusNavigation)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.Busqueda.Cliente))
                {
                    query = query.Where(v => v.IdClienteNavigation != null && v.IdClienteNavigation.Nombre.Contains(request.Busqueda.Cliente));
                }
                if (request.Busqueda.FechaInicio.HasValue)
                {
                    query = query.Where(v => v.FechaVenta >= request.Busqueda.FechaInicio.Value);
                }
                if (request.Busqueda.FechaFin.HasValue)
                {
                    query = query.Where(v => v.FechaVenta <= request.Busqueda.FechaFin.Value);
                }
                if (request.Busqueda.Estatus.HasValue && request.Busqueda.Estatus.Value != 0)
                {
                    query = query.Where(v => v.Estatus == request.Busqueda.Estatus.Value);
                }

                // Conteo para paginación cuando piden todos
                if (request.CantidadPorPagina == -1)
                {
                    request.CantidadPorPagina = await query.CountAsync();
                }

                var ventas = await query
                    .OrderByDescending(v => v.IdVentas)
                    .Skip((request.NumeroPagina - 1) * request.CantidadPorPagina)
                    .Take(request.CantidadPorPagina)
                    .Select(v => new VentaResponse
                    {
                        IdVentas = v.IdVentas,
                        Descripcion = v.Descripcion,
                        IdCliente = v.IdCliente,
                        Cliente = v.IdClienteNavigation != null ? v.IdClienteNavigation.Nombre : string.Empty,
                        FechaVenta = v.FechaVenta,
                        Cantidad = v.Cantidad,
                        Precio = v.Precio,
                        Subtotal = v.Subtotal,
                        Iva = v.Iva,
                        Total = v.Total,
                        Estatus = v.Estatus,
                        EstatusStr = v.EstatusNavigation != null ? v.EstatusNavigation.Estatus : string.Empty,
                        Observaciones = v.Observaciones,
                        FechaCreacion = v.FechaCreacion,
                        IdUsuarioCreacion = v.IdUsuarioCreacion
                    })
                    .ToListAsync();

                return Ok(new DefaultResponse<List<VentaResponse>> { Success = true, Data = ventas });
            }
            catch (Exception ex)
            {
                return Ok(new DefaultResponse<List<VentaResponse>> { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("NuevoRegistro")]
        public async Task<IActionResult> NuevoRegistroVenta([FromBody] NuevoRegistroVentaRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(DefaultResponse<List<string>>
                        .FromModelState(ModelState));

                var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!long.TryParse(nameIdentifier, out long userId))
                {
                    return Conflict(new DefaultResponse<object>
                    {
                        Message = "No se tienen permisos para esta acción."
                    });
                }

                // Validar cliente
                if (request.IdCliente.HasValue)
                {
                    bool existeCliente = await _context.Clientes
                        .AnyAsync(c => c.Id == request.IdCliente.Value);

                    if (!existeCliente)
                    {
                        return Conflict(new DefaultResponse<object>
                        {
                            Message = "El cliente no existe."
                        });
                    }
                }

                // Validaciones básicas
                decimal cantidad = request.Cantidad ?? 0;
                decimal precio = request.Precio ?? 0;

                if (cantidad <= 0)
                {
                    return Conflict(new DefaultResponse<object>
                    {
                        Message = "La cantidad debe ser mayor a 0."
                    });
                }

                if (precio <= 0)
                {
                    return Conflict(new DefaultResponse<object>
                    {
                        Message = "El precio debe ser mayor a 0."
                    });
                }

                // Cálculos
                decimal subtotal = Math.Round(cantidad * precio, 2);
                decimal ivaRate = 0.16m;
                decimal iva = Math.Round(subtotal * ivaRate, 2);
                decimal total = Math.Round(subtotal + iva, 2);

                Venta venta;

                // =========================================
                // EDITAR
                // =========================================
                if (request.IdVentas.HasValue && request.IdVentas.Value > 0)
                {
                    venta = await _context.Ventas
                        .FirstOrDefaultAsync(v => v.IdVentas == request.IdVentas.Value);

                    if (venta == null)
                    {
                        return Conflict(new DefaultResponse<object>
                        {
                            Message = "La venta no existe."
                        });
                    }

                    venta.Descripcion = request.Descripcion;
                    venta.IdCliente = request.IdCliente;
                    venta.FechaVenta = request.FechaVenta ?? venta.FechaVenta;
                    venta.Cantidad = Convert.ToInt32(cantidad);
                    venta.Precio = precio;
                    venta.Subtotal = subtotal;
                    venta.Iva = iva;
                    venta.Total = total;
                    venta.Estatus = request.Estatus ?? venta.Estatus;
                    venta.Observaciones = request.Observaciones;

                    _context.Ventas.Update(venta);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(new DefaultResponse<object>
                    {
                        Success = true,
                        Message = "Venta actualizada correctamente."
                    });
                }

                // =========================================
                // NUEVO
                // =========================================
                venta = new Venta
                {
                    Descripcion = request.Descripcion,
                    IdCliente = request.IdCliente,
                    FechaVenta = request.FechaVenta ?? DateTime.Now,

                    Cantidad = Convert.ToInt32(cantidad),

                    Precio = precio,
                    Subtotal = subtotal,
                    Iva = iva,
                    Total = total,

                    Estatus = request.Estatus ?? 1,
                    Observaciones = request.Observaciones,

                    IdUsuarioCreacion = userId,
                    FechaCreacion = DateTime.Now
                };

                await _context.Ventas.AddAsync(venta);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new DefaultResponse<object>
                {
                    Success = true,
                    Message = "Venta registrada correctamente."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return StatusCode(StatusCodes.Status500InternalServerError,
                    new DefaultResponse<object>
                    {
                        Message = ex.Message
                    });
            }
        }
    }
}