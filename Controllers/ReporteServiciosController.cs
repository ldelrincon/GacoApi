using gaco_api.Models;
using gaco_api.Models.DTOs.Requests.ReporteSolicitudes;
using gaco_api.Models.DTOs.Responses;
using gaco_api.Models.DTOs.Responses.ReporteSolicitudes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [HttpPost]
        [Route("Busqueda")]
        public async Task<IActionResult> BusquedaReporteSolicitud(BusquedaReporteServicioRequest request)
        {
            // Construir la consulta inicial
            var query = _context.ReporteServicios
                .Include(x => x.IdCatEstatusNavigation)
                .AsQueryable();

            // Filtrar si hay una búsqueda
            if (!string.IsNullOrEmpty(request.Busqueda))
            {
                query = query.Where(x => x.UsuarioEncargado.Contains(request.Busqueda)
                    || x.IdCatSolicitudNavigation.TipoSolicitud.Contains(request.Busqueda)
                    || x.IdClienteNavigation.Nombre.Contains(request.Busqueda)
                    || (x.IdUsuarioCreacionNavigation.Nombres + ' ' + x.IdUsuarioCreacionNavigation.Apellidos).Contains(request.Busqueda)
                    || (x.IdUsuarioTecnicoNavigation.Nombres + ' ' + x.IdUsuarioTecnicoNavigation.Apellidos).Contains(request.Busqueda)
                    || x.IdClienteNavigation.Nombre.Contains(request.Busqueda)
                    && x.IdCatEstatus == 1
                );
            }

            if (request.CantidadPorPagina == -1)
            {
                request.CantidadPorPagina = await _context.ReporteServicios.CountAsync(x => x.IdCatEstatus == 1);
            }

            // Seleccionar y aplicar paginación
            var reporteServicios = await query
                .Select(x => new ReporteServicioResponse
                {
                    Id = x.Id,
                    IdCatSolicitud = x.IdCatSolicitud,
                    IdUsuarioCreacion = x.IdUsuarioCreacion,
                    IdCliente = x.IdCliente,
                    Titulo = x.Titulo,
                    Descripcion = x.Descripcion,
                    FechaCreacion = x.FechaCreacion,
                    FechaModificacion = x.FechaModificacion,
                    IdCatEstatus = x.IdCatEstatus,
                    FechaInicio = x.FechaInicio,
                    Accesorios = x.Accesorios,
                    ServicioPreventivo = x.ServicioPreventivo,
                    ServicioCorrectivo = x.ServicioCorrectivo,
                    ObservacionesRecomendaciones = x.ObservacionesRecomendaciones,
                    IdUsuarioTecnico = x.IdUsuarioTecnico,
                    UsuarioEncargado = x.UsuarioEncargado,
                    Estatus = x.IdCatEstatusNavigation.Estatus,
                    UsuarioCreacion = (x.IdUsuarioCreacionNavigation.Nombres + " " + x.IdUsuarioCreacionNavigation.Apellidos),
                    Cliente = x.IdClienteNavigation.Nombre,
                    CatSolicitud = x.IdCatSolicitudNavigation.TipoSolicitud,
                    UsuarioTecnico = (x.IdUsuarioTecnicoNavigation.Nombres + " " + x.IdUsuarioTecnicoNavigation.Apellidos),
                })
                .Skip((request.NumeroPagina - 1) * request.CantidadPorPagina)
                .Take(request.CantidadPorPagina)
                .ToListAsync();

            // Crear la respuesta
            var response = new DefaultResponse<List<ReporteServicioResponse>>
            {
                Success = true,
                Data = reporteServicios,
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("PorId/{id}")]
        public async Task<IActionResult> ReporteServicioPorId(long id)
        {
            var response = new DefaultResponse<EditarReporteServicioResponse>();

            // Seleccionar y aplicar paginación
            var reporteServicio = await _context.ReporteServicios
                .Where(x => x.Id == id)
                .Select(x => new EditarReporteServicioResponse
                {
                    Id = x.Id,
                    IdCatSolicitud = x.IdCatSolicitud,
                    IdUsuarioCreacion = x.IdUsuarioCreacion,
                    IdCliente = x.IdCliente,
                    Titulo = x.Titulo,
                    Descripcion = x.Descripcion,
                    FechaCreacion = x.FechaCreacion,
                    FechaModificacion = x.FechaModificacion,
                    IdCatEstatus = x.IdCatEstatus,
                    FechaInicio = x.FechaInicio,
                    Accesorios = x.Accesorios,
                    ServicioPreventivo = x.ServicioPreventivo,
                    ServicioCorrectivo = x.ServicioCorrectivo,
                    ObservacionesRecomendaciones = x.ObservacionesRecomendaciones,
                    IdUsuarioTecnico = x.IdUsuarioTecnico,
                    UsuarioEncargado = x.UsuarioEncargado,
                    // productos = 
                }).FirstOrDefaultAsync();

            if (reporteServicio != null)
            {
                // Productos.
                reporteServicio.Productos = new List<Producto>();
                // Evidencias.
                reporteServicio.Evidencias = new List<Evidencia>();
                // Obtener el primer seguimento.
                var primerSeguimento = await _context.Seguimentos.FirstOrDefaultAsync(x => x.IdReporteServicio == reporteServicio.Id);
                if (primerSeguimento != null) {
                    // Obtener relacion de productos.
                    var relSeguimentoProductos = primerSeguimento.RelSeguimentoProductos.ToList();
                    foreach (var item in relSeguimentoProductos)
                    {
                        reporteServicio.Productos.Add(item.IdProductoNavigation);
                    }
                    // Evidencias.
                    reporteServicio.Evidencias = primerSeguimento.Evidencia.ToList();
                    // Proxima Visita.
                    reporteServicio.FechaProximaVisita = primerSeguimento.ProximaVisita;
                    reporteServicio.DescripcionProximaVisita = primerSeguimento.DescripcionProximaVisita;
                }
            }
            
            if (reporteServicio != null)
            {
                response = new DefaultResponse<EditarReporteServicioResponse>
                {
                    Success = true,
                    Data = reporteServicio,
                };
            }
            else
            {
                response = new DefaultResponse<EditarReporteServicioResponse>
                {
                    Success = false,
                    Message = "No se encontro el Reporte Servicio."
                };
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("Nuevo")]
        public async Task<ActionResult> NuevoReporteServicio(NuevoReporteServicioRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(DefaultResponse<List<string>>.FromModelState(ModelState));
                }

                var existeCatTipoSolicitudes = await _context.CatTipoSolicitudes.AnyAsync(m => m.Id == request.IdCatSolicitud);
                if (!existeCatTipoSolicitudes)
                {
                    return Conflict(new DefaultResponse<object> { Message = "El tipo de solicitud no existe o no se encontró." });
                }

                var existeUsuarioCreacion = await _context.Usuarios.AnyAsync(m => m.Id == request.IdUsuarioCreacion);
                if (!existeUsuarioCreacion)
                {
                    return Conflict(new DefaultResponse<object> { Message = "No se encontró el usuario." });
                }

                var existeCliente = await _context.Clientes.AnyAsync(m => m.Id == request.IdCliente);
                if (!existeCliente)
                {
                    return Conflict(new DefaultResponse<object> { Message = "El cliente no existe o no se encontró." });
                }

                var existeUsuarioTecnico = await _context.Usuarios.AnyAsync(m => m.Id == request.IdUsuarioTecnico);
                if (!existeUsuarioTecnico)
                {
                    return Conflict(new DefaultResponse<object> { Message = "El técnico no existe o no se encontró." });
                }

                var nuevo = new ReporteServicio()
                {
                    IdCatSolicitud = request.IdCatSolicitud,
                    IdUsuarioCreacion = request.IdUsuarioCreacion,
                    IdCliente = request.IdCliente,
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    IdCatEstatus = 1,
                    FechaInicio = request.FechaInicio,
                    Accesorios = request.Accesorios,
                    ServicioPreventivo = request.ServicioPreventivo,
                    ServicioCorrectivo = request.ServicioCorrectivo,
                    ObservacionesRecomendaciones = request.ObservacionesRecomendaciones,
                    IdUsuarioTecnico = request.IdUsuarioTecnico,
                    UsuarioEncargado = request.UsuarioEncargado,
                };
                await _context.ReporteServicios.AddAsync(nuevo);

                // Crear el primer seguimento.
                var primerSeguimento = new Seguimento() {
                    IdReporteServicio = nuevo.Id,
                    // IdUsuario = #Obtener id desde la sesion.
                    Seguimento1 = "Primer Seguimento", 
                    IdCatEstatus = 1,
                    ProximaVisita = request.FechaProximaVisita,
                    DescripcionProximaVisita = request.DescripcionProximaVisita,
                };
                await _context.Seguimentos.AddAsync(primerSeguimento);

                // Relacion Seguimento Productos.
                if (request.Productos != null && request.Productos.Any())
                {
                    foreach (var producto in request.Productos)
                    {
                        var relSeguimentoProducto = new RelSeguimentoProducto() {
                            IdSeguimento = primerSeguimento.Id,
                            IdProducto = producto.Id,
                            // IdUsuario = #Obtener id desde la sesion.
                            IdCatEstatus =1,
                            //Cantidad = producto.Cantidad,
                            Unidad = "",
                        };
                        await _context.RelSeguimentoProductos.AddAsync(relSeguimentoProducto);
                    }
                }

                if (request.Evidencias != null && request.Evidencias.Any())
                {
                    foreach (var evidenciaRS in request.Evidencias)
                    {
                        var evidencia = new Evidencia()
                        {
                            IdSeguimento = primerSeguimento.Id,
                            Nombre = evidenciaRS.Nombre,
                            Extension = evidenciaRS.Extencion,
                            Ruta = $"/Evidencias/S{primerSeguimento.Id}/D{DateTime.Now.ToBinary()}E{evidenciaRS.Nombre}.{evidenciaRS.Extencion}",
                            IdCatEstatus = 1,
                        };
                        await _context.Evidencias.AddAsync(evidencia);
                        // Guardar base64.
                    }
                }

                await _context.SaveChangesAsync();

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
                // Revertir la transacción en caso de error
                await transaction.RollbackAsync();

                return StatusCode(StatusCodes.Status500InternalServerError,
                    new DefaultResponse<object> { Message = ex.Message });
            }
        }


        //[HttpPost]
        //[Route("Actualizar")]
        //public async Task<ActionResult> ActualizarProducto(ActualizarReporteServicioRequest request)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(DefaultResponse<List<string>>.FromModelState(ModelState));
        //        }

        //        var reporteServicio = await _context.ReporteServicios.FindAsync(request.Id);
        //        if (reporteServicio == null)
        //        {
        //            return NotFound(new DefaultResponse<object> { Message = "Reporte Servicio no encontrado." });
        //        }

        //        var existeCatTipoSolicitudes = await _context.CatTipoSolicitudes.AnyAsync(m => m.Id == request.IdCatSolicitud);
        //        if (!existeCatTipoSolicitudes)
        //        {
        //            return Conflict(new DefaultResponse<object> { Message = "El tipo de solicitud no exite o no se encontro." });
        //        }

        //        var existeUsuarioCreacion = await _context.Usuarios.AnyAsync(m => m.Id == request.IdUsuarioCreacion);
        //        if (existeUsuarioCreacion)
        //        {
        //            return Conflict(new DefaultResponse<object> { Message = "No se encontro el usuario." });
        //        }

        //        var existeCliente = await _context.Clientes.AnyAsync(m => m.Id == request.IdCliente);
        //        if (existeCliente)
        //        {
        //            return Conflict(new DefaultResponse<object> { Message = "El cliente no existe o no se encontro." });
        //        }

        //        var existeUsuarioTecnico = await _context.Usuarios.AnyAsync(m => m.Id == request.IdUsuarioTecnico);
        //        if (existeUsuarioTecnico)
        //        {
        //            return Conflict(new DefaultResponse<object> { Message = "El tecnico no existe o no se encontro." });
        //        }

        //        // Actualizar los datos
        //        // reporteServicio.Id { get; set; }
        //        reporteServicio.IdCatSolicitud = request.IdCatSolicitud;
        //        reporteServicio.IdUsuarioCreacion = request.IdUsuarioCreacion;
        //        reporteServicio.IdCliente = request.IdCliente;
        //        reporteServicio.Titulo = request.Titulo;
        //        reporteServicio.Descripcion = request.Descripcion;
        //        reporteServicio.IdCatEstatus = request.IdCatEstatus;
        //        reporteServicio.FechaInicio = request.FechaInicio;
        //        reporteServicio.Accesorios = request.Accesorios;
        //        reporteServicio.ServicioPreventivo = request.ServicioPreventivo;
        //        reporteServicio.ServicioCorrectivo = request.ServicioCorrectivo;
        //        reporteServicio.ObservacionesRecomendaciones = request.ObservacionesRecomendaciones;
        //        reporteServicio.IdUsuarioTecnico = request.IdUsuarioTecnico;
        //        reporteServicio.UsuarioEncargado = request.UsuarioEncargado;
        //        reporteServicio.IdCatEstatus = request.IdCatEstatus;
        //        reporteServicio.FechaModificacion = DateTime.Now;

        //        // Guardar los cambios
        //        _context.ReporteServicios.Update(reporteServicio);
        //        await _context.SaveChangesAsync();

        //        return Ok(new DefaultResponse<object>
        //        {
        //            Success = true,
        //            Message = "Reporte Solicitud actualizado correctamente."
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            new DefaultResponse<object> { Message = ex.Message });
        //    }
        //}
    }
}
