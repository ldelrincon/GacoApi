using gaco_api.Customs;
using gaco_api.Models;
using gaco_api.Models.DTOs.Requests.Seguimentos;
using gaco_api.Models.DTOs.Responses;
using gaco_api.Models.DTOs.Responses.Evidencias;
using gaco_api.Models.DTOs.Responses.Relaciones;
using gaco_api.Models.DTOs.Responses.Seguimientos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        [HttpPost]
        [Route("NuevoAdjuntos")]
        public async Task<ActionResult> NuevoAdjuntos(SeguimientoProductoEvidenciaRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(DefaultResponse<List<string>>.FromModelState(ModelState));
                }

                // Obtener el ID del usuario conectado
                var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!long.TryParse(nameIdentifier, out long userId))
                {
                    return Conflict(new DefaultResponse<object> { Message = "No se tiene permisos para esta acción." });
                }

                // Crear el primer seguimiento
                var seguimiento = new Seguimento
                {
                    IdReporteServicio = request.IdReporteServicio,
                    IdUsuario = userId,
                    Seguimento1 = request.Seguimiento,
                    IdCatEstatus = 1,
                    ProximaVisita = request.ProximaVisita,
                    DescripcionProximaVisita = request.DescripcionProximaVisita,
                };

                await _context.Seguimentos.AddAsync(seguimiento);
                await _context.SaveChangesAsync();

                // Validar y relacionar productos
                if (request.Productos != null && request.Productos.Any())
                {
                    foreach (var producto in request.Productos)
                    {
                        if (!await _context.Productos.AnyAsync(x => x.Id == producto.Id))
                        {
                            await transaction.RollbackAsync();
                            return Conflict(new DefaultResponse<object> { Message = "El producto no se encontró." });
                        }

                        if (producto.Cantidad <= 0)
                        {
                            await transaction.RollbackAsync();
                            return Conflict(new DefaultResponse<object> { Message = "La cantidad de producto debe ser mayor a 0." });
                        }

                        var relSeguimentoProducto = new RelSeguimentoProducto
                        {
                            IdSeguimento = seguimiento.Id,
                            IdProducto = producto.Id,
                            MontoGasto = producto.MontoGasto,
                            IdUsuario = userId,
                            IdCatEstatus = 1,
                            Cantidad = producto.Cantidad,
                            Unidad = "",
                            Porcentaje = producto.Porcentaje,
                            MontoVenta = producto.MontoVenta
                        };
                        await _context.RelSeguimentoProductos.AddAsync(relSeguimentoProducto);
                        await _context.SaveChangesAsync();
                    }
                }

                // Procesar evidencias
                if (request.Evidencias != null && request.Evidencias.Any())
                {
                    foreach (var evidenciaRS in request.Evidencias)
                    {
                        string rutaEvidencia;
                        try
                        {
                            rutaEvidencia = await _utilidades.GuardarArchivoBase64Async(
                                $"/Evidencias/Seguimento_{seguimiento.Id}",
                                evidenciaRS.Extension,
                                evidenciaRS.Base64
                            );
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            return Conflict(new DefaultResponse<object> { Message = $"Error al guardar evidencia: {ex.Message}" });
                        }

                        var evidencia = new Evidencia
                        {
                            IdSeguimento = seguimiento.Id,
                            Nombre = evidenciaRS.Nombre,
                            Extension = evidenciaRS.Extension,
                            Ruta = rutaEvidencia,
                            IdCatEstatus = 1,
                        };
                        await _context.Evidencias.AddAsync(evidencia);
                        await _context.SaveChangesAsync();
                    }
                }

                // await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new DefaultResponse<object>
                {
                    Success = true,
                    Message = "Registrado correctamente."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new DefaultResponse<object> { Message = ex.Message });
            }
        }
    }
}
