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
            // Construir la consulta inicial
            var query = _context.Gastos
                .Include(u => u.DetalleGastos)
                .Include(u => u.IdCatEstatusNavigation)
                .AsQueryable();

            if (request.CantidadPorPagina == -1)
            {
                request.CantidadPorPagina = await _context.Gastos.CountAsync(x => x.IdCatEstatus == 1);
            }

            // Seleccionar y aplicar paginación
            var Gastos = await query
                .Select(x => new GastoResponse
                {
                    Id = x.Id,
                    FechaCreacion = x.FechaCreacion,
                    FechaModificacion = x.FechaModificacion,
                    IdCatEstatus = x.IdCatEstatus,
                    Estatus = x.IdCatEstatusNavigation.Estatus,
                    Factura = x.Factura,
                    IdUsuarioCreacion = x.IdUsuarioCreacion,
                    productos = (int)x.DetalleGastos.Sum(dg => dg.Cantidad),
                    RutaPdffactura = x.RutaPdffactura,
                    RutaXmlfactura = x.RutaXmlfactura,
                    Total = x.DetalleGastos.Sum(dg => dg.Monto),
                    UsuarioCreacion = $"{x.IdUsuarioCreacionNavigation.Nombres} {x.IdUsuarioCreacionNavigation.Apellidos}",
                })
                .Skip((request.NumeroPagina - 1) * request.CantidadPorPagina)
                .Take(request.CantidadPorPagina)
                .ToListAsync();

            // Crear la respuesta
            var response = new DefaultResponse<List<GastoResponse>>
            {
                Success = true,
                Data = Gastos,
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("PorId/{id}")]
        public async Task<IActionResult> GastoPorId(long id)
        {
            var response = new DefaultResponse<EditarGastoResponse>();

            // Seleccionar y aplicar paginación
            var cliente = await _context.Gastos
                .Where(x => x.Id == id)
                .Select(x => new EditarGastoResponse
                {
                    Id = x.Id,
                    Factura = x.Factura,
                    FechaCreacion = x.FechaCreacion,
                    FechaModificacion = x.FechaModificacion,
                    IdCatEstatus = x.IdCatEstatus,
                    IdUsuarioCreacion = x.IdUsuarioCreacion,
                    RutaPdffactura = x.RutaPdffactura,
                    RutaXmlfactura = x.RutaXmlfactura
                }).FirstOrDefaultAsync();

            if (cliente != null)
            {
                response = new DefaultResponse<EditarGastoResponse>
                {
                    Success = true,
                    Data = cliente,
                };
            }
            else
            {
                response = new DefaultResponse<EditarGastoResponse>
                {
                    Success = false,
                    Message = "No se encontro el Gasto."
                };
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("Nuevo")]
        public async Task<ActionResult> NuevoGasto(NuevoGastoRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(DefaultResponse<List<string>>.FromModelState(ModelState));
                }

                long userId = 0;
                var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!long.TryParse(nameIdentifier, out userId) || userId <= 0)
                {
                    return BadRequest(new DefaultResponse<object>
                    {
                        Success = false,
                        Message = "Usuario no válido."
                    });
                }

                // Iniciar la transacción
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var nuevoGasto = new Gasto()
                    {
                        Factura = request.Factura,
                        IdUsuarioCreacion = userId,
                        RutaPdffactura = request.RutaPdffactura,
                        RutaXmlfactura = request.RutaXmlfactura,
                        IdCatEstatus = 1,
                        DetalleGastos = new List<DetalleGasto>() // Inicializar la lista
                    };

                    await _context.Gastos.AddAsync(nuevoGasto);
                    await _context.SaveChangesAsync(); // Guardar primero el gasto

                    // Verificar que el gasto se haya guardado correctamente
                    if (nuevoGasto.Id == 0)
                    {
                        throw new Exception("No se pudo registrar el gasto.");
                    }

                    // Validar y agregar DetalleGastos
                    if (request.DetalleGastos != null && request.DetalleGastos.Any())
                    {
                        // Obtener los Ids de productos que existen en la base de datos
                        var productosExistentes = await _context.Productos
                            .Where(p => request.DetalleGastos.Select(d => d.IdProducto).Contains(p.Id))
                            .Select(p => p.Id)
                            .ToListAsync();

                        foreach (var detalle in request.DetalleGastos)
                        {
                            if (!productosExistentes.Contains(detalle.IdProducto))
                            {
                                throw new Exception($"El producto con Id {detalle.IdProducto} no existe.");
                            }

                            nuevoGasto.DetalleGastos.Add(new DetalleGasto
                            {
                                Cantidad = detalle.Cantidad,
                                IdCatEstatus = 1,
                                IdGasto = nuevoGasto.Id,
                                IdProducto = detalle.IdProducto,
                                Monto = detalle.Monto,
                            });
                        }

                        await _context.SaveChangesAsync(); // Guardar los detalles de gasto
                    }

                    // Confirmar la transacción
                    await transaction.CommitAsync();

                    return Ok(new DefaultResponse<object>
                    {
                        Success = true,
                        Message = "Registrado correctamente."
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(); // Revertir los cambios en caso de error

                    return BadRequest(new DefaultResponse<object>
                    {
                        Success = false,
                        Message = $"Error al registrarlo: {ex.Message}"
                    });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new DefaultResponse<object> { Message = ex.Message });
            }
        }


        [HttpPost]
        [Route("Actualizar")]
        public async Task<ActionResult> ActualizarGasto(ActualizarGastoRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(DefaultResponse<List<string>>.FromModelState(ModelState));
                }

                long userId = 0;
                var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!long.TryParse(nameIdentifier, out userId) || userId <= 0)
                {
                    return BadRequest(new DefaultResponse<object>
                    {
                        Success = false,
                        Message = "Usuario no válido."
                    });
                }

                // Iniciar transacción
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Buscar el gasto existente por su ID
                    var gasto = await _context.Gastos.FindAsync(request.Id);
                    if (gasto == null)
                    {
                        return NotFound(new DefaultResponse<object> { Message = "Gasto no encontrado." });
                    }

                    // Actualizar los datos del gasto
                    gasto.RutaPdffactura = request.RutaPdffactura;
                    gasto.RutaXmlfactura = request.RutaXmlfactura;
                    gasto.Factura = request.Factura;
                    gasto.FechaModificacion = DateTime.Now;

                    _context.Gastos.Update(gasto);
                    await _context.SaveChangesAsync();

                    // Confirmar la transacción
                    await transaction.CommitAsync();

                    return Ok(new DefaultResponse<object>
                    {
                        Success = true,
                        Message = "Gasto actualizado correctamente."
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(); // Revertir cambios si hay un error
                    return BadRequest(new DefaultResponse<object>
                    {
                        Success = false,
                        Message = $"Error al actualizar el gasto: {ex.Message}"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new DefaultResponse<object> { Message = ex.Message });
            }
        }


        [HttpDelete]
        [Route("Eliminar/{id}")]
        public async Task<ActionResult> EliminarGasto(long id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new DefaultResponse<object>
                    {
                        Success = false,
                        Message = "ID de gasto no válido."
                    });
                }

                // Iniciar transacción
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Buscar el gasto existente junto con sus detalles
                    var gasto = await _context.Gastos
                        .Include(g => g.DetalleGastos)
                        .FirstOrDefaultAsync(g => g.Id == id);

                    if (gasto == null)
                    {
                        return NotFound(new DefaultResponse<object> { Message = "Gasto no encontrado." });
                    }

                    // Marcar el gasto como eliminado (en lugar de borrarlo físicamente)
                    gasto.IdCatEstatus = 2; // Estado 'Eliminado'
                    gasto.FechaModificacion = DateTime.Now;

                    // Marcar los detalles del gasto como eliminados (si aplica en tu lógica)
                    foreach (var detalle in gasto.DetalleGastos)
                    {
                        detalle.IdCatEstatus = 2;
                    }

                    _context.Gastos.Update(gasto);
                    await _context.SaveChangesAsync();

                    // Confirmar la transacción
                    await transaction.CommitAsync();

                    return Ok(new DefaultResponse<object>
                    {
                        Success = true,
                        Message = "Gasto eliminado correctamente."
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(); // Revertir cambios en caso de error
                    return BadRequest(new DefaultResponse<object>
                    {
                        Success = false,
                        Message = $"Error al eliminar el gasto: {ex.Message}"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new DefaultResponse<object> { Message = ex.Message });
            }
        }

    }
}
