using gaco_api.Customs;
using gaco_api.Models;
using gaco_api.Models.DTOs.Requests;
using gaco_api.Models.DTOs.Requests.Clientes;
using gaco_api.Models.DTOs.Requests.ReporteSolicitudes;
using gaco_api.Models.DTOs.Responses;
using gaco_api.Models.DTOs.Responses.Evidencias;
using gaco_api.Models.DTOs.Responses.Relaciones;
using gaco_api.Models.DTOs.Responses.ReporteSolicitudes;
using gaco_api.Utilerias;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Buffers.Text;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace gaco_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReporteServiciosController : ControllerBase
    {
        private readonly GacoDbContext _context;
        private readonly Utilidades _utilidades;
        private readonly IConfiguration _configuration;
         private readonly IWebHostEnvironment _env;

        public ReporteServiciosController(GacoDbContext context, Utilidades utilidades, IConfiguration configuration, IWebHostEnvironment env)
        {
            _context = context;
            _utilidades = utilidades;
            _configuration = configuration;
            _env = env;
        }

        [HttpPost]
        [Route("Busqueda")]
        public async Task<IActionResult> BusquedaReporteSolicitud(BusquedaReporteServicioRequest request)
        {
            try
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
                        || x.UsuarioTecnico.Contains(request.Busqueda)
                        || x.IdClienteNavigation.Nombre.Contains(request.Busqueda)
                        && x.IdCatEstatus == 1
                    );
                }

                if (request.CantidadPorPagina == -1)
                {
                    request.CantidadPorPagina = await _context.ReporteServicios.CountAsync(x => x.IdCatEstatus == 1);
                }
                // Productos.


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
                        // IdUsuarioTecnico = x.IdUsuarioTecnico,
                        UsuarioEncargado = x.UsuarioEncargado,
                        Estatus = x.IdCatEstatusNavigation.Estatus,
                        UsuarioCreacion = (x.IdUsuarioCreacionNavigation.Nombres + " " + x.IdUsuarioCreacionNavigation.Apellidos),
                        Cliente = x.IdClienteNavigation.Nombre,
                        CatSolicitud = x.IdCatSolicitudNavigation.TipoSolicitud,
                        //UsuarioTecnico = (x.IdUsuarioTecnicoNavigation.Nombres + " " + x.IdUsuarioTecnicoNavigation.Apellidos),
                        UsuarioTecnico = x.UsuarioTecnico,

                    })
                    .Skip((request.NumeroPagina - 1) * request.CantidadPorPagina)
                    .Take(request.CantidadPorPagina)
                    .ToListAsync();

                foreach (ReporteServicioResponse objReporteServicioResponse in reporteServicios)
                {
                    //primer seguimiento
                    var primerSeguimento = await _context.Seguimentos
                     .Include(x => x.Evidencia)
                     .Include(x => x.RelSeguimentoProductos)
                     .ThenInclude(x => x.IdProductoNavigation)
                     .FirstOrDefaultAsync(x => x.IdReporteServicio == objReporteServicioResponse.Id);

                    objReporteServicioResponse.Total = 0;
                    foreach (var item in primerSeguimento.RelSeguimentoProductos)
                    {
                        objReporteServicioResponse.Total += (item.Cantidad * item.MontoGasto);
                    }
                    objReporteServicioResponse.Totalstr = objReporteServicioResponse.Total?.ToString("C2");
                    if (objReporteServicioResponse.Totalstr == null)
                    {
                        objReporteServicioResponse.Totalstr = "$0.00";
                    }
                }

                // Crear la respuesta
                var response = new DefaultResponse<List<ReporteServicioResponse>>
                {
                    Success = true,
                    Data = reporteServicios,
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new DefaultResponse<List<ReporteServicioResponse>>
                {
                    Success = false,
                    Message = ex.Message,
                });
            }
        }

        /// <summary>
        ///  Accion para guardar un nuevo gestor.
        /// </summary>
        /// <param name="ObjModGestor">Objeto con la informacion del nuevo gestor.</param>
        /// <returns>Objeto respuesta.</returns>
        [HttpGet]
        [Route("SendEmail/{id}")]

        public async Task<IActionResult> SendEmail(long Id)
        {

            // Construir la consulta inicial
            var query = _context.ReporteServicios
                .Include(x => x.IdCatEstatusNavigation)
                .AsQueryable();

            ClsModCorreo objCorreo = new ClsModCorreo();
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
                 UsuarioTecnico = x.UsuarioTecnico,
                 UsuarioEncargado = x.UsuarioEncargado,
                 Estatus = x.IdCatEstatusNavigation.Estatus,
                 UsuarioCreacion = (x.IdUsuarioCreacionNavigation.Nombres + " " + x.IdUsuarioCreacionNavigation.Apellidos),
                 Cliente = x.IdClienteNavigation.Nombre,
                 Telefono = x.IdClienteNavigation.Telefono,
                 RFC = x.IdClienteNavigation.Rfc,
                 RegimenFiscal = x.IdClienteNavigation.IdRegimenFiscalNavigation.Descripcion,
                 Correo = x.IdClienteNavigation.Correo,
                 RazonSocial = x.IdClienteNavigation.RazonSocial,
                 CodigoPostal = x.IdClienteNavigation.CodigoPostal,
                 Direccion = x.IdClienteNavigation.Direccion,
                 CatSolicitud = x.IdCatSolicitudNavigation.TipoSolicitud,
             }).Where(x => x.Id == Id)

             .FirstAsync();

            // Productos.
            reporteServicios.Productos = new List<RelSeguimentoProductoResponse>();
            // Evidencias.
            reporteServicios.Evidencias = new List<EvidenciaResponse>();
            // Obtener el primer seguimento.
            var primerSeguimento = await _context.Seguimentos
                .Include(x => x.Evidencia)
                .Include(x => x.RelSeguimentoProductos).ThenInclude(x => x.IdProductoNavigation)
                .FirstOrDefaultAsync(x => x.IdReporteServicio == reporteServicios.Id);

            if (primerSeguimento != null)
            {
                // Llenar Productos.
                foreach (var item in primerSeguimento.RelSeguimentoProductos)
                {
                    reporteServicios.Productos.Add(new RelSeguimentoProductoResponse()
                    {
                        Cantidad = item.Cantidad,
                        FechaCreacion = item.FechaCreacion,
                        FechaModificacion = item.FechaModificacion,
                        Id = item.Id,
                        IdCatEstatus = item.IdCatEstatus,
                        IdProducto = item.IdProducto,
                        IdSeguimento = item.IdSeguimento,
                        IdUsuario = item.IdUsuario,
                        MontoGasto = item.MontoGasto,
                        MontoVenta = item.MontoVenta,
                        Codigo = item.IdProductoNavigation.Codigo,
                        Producto = item.IdProductoNavigation.Producto1,
                        Unidad = item.Unidad
                    });
                }

                // Llenar Evidencias.
                foreach (var item in primerSeguimento.Evidencia)
                {
                    reporteServicios.Evidencias.Add(new EvidenciaResponse()
                    {
                        Extension = item.Extension,
                        FechaCreacion = item.FechaCreacion,
                        FechaModificacion = item.FechaModificacion,
                        Id = item.Id,
                        IdCatEstatus = item.IdCatEstatus,
                        IdSeguimento = item.IdSeguimento,
                        Nombre = item.Nombre,
                        Ruta = _utilidades.GetFullUrl(item.Ruta),
                        Base64 = await _utilidades.ObtenerBase64Async(item.Ruta),
                    });
                }

                // Proxima Visita.
                reporteServicios.ProximaVisita = primerSeguimento.ProximaVisita;
                reporteServicios.DescripcionProximaVisita = primerSeguimento.DescripcionProximaVisita;
            }


            ClsModResult result = new();

            try
            {
                var TargetCorreo = new ClsModCorreo();
                TargetCorreo.strTo = "luisdelrincon7@gmail.com"; //correo usuario
                TargetCorreo.strFrom = "notificaciones@gaco.com.mx"; //help@zivo.com.mx
                TargetCorreo.strFromNombre = string.Empty;
                TargetCorreo.strCC = string.Empty;

                TargetCorreo.strSubject = "Servicio a facturar";

                TargetCorreo.strBody = "Servicio para facturación";
                TargetCorreo.strPassword = "NotificacionesGACO1";
                TargetCorreo.intPuerto = 587;
                TargetCorreo.strHost = "smtp.ionos.com";
                TargetCorreo.usaSSL = false;

                NotificacionCorreo.Send(TargetCorreo, _env.ContentRootPath, reporteServicios);
            }
            catch (Exception ex)
            {
                result.MsgError = ex.ToString();
            }

            if (result.IsError) return BadRequest(result.MsgError);
            else return Ok(result);
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
                    // IdUsuarioTecnico = x.IdUsuarioTecnico,
                    UsuarioTecnico = x.UsuarioTecnico ?? string.Empty,
                    UsuarioEncargado = x.UsuarioEncargado,
                    // productos = 
                }).FirstOrDefaultAsync();

            if (reporteServicio != null)
            {
                // Productos.
                reporteServicio.Productos = new List<RelSeguimentoProductoResponse>();
                // Evidencias.
                reporteServicio.Evidencias = new List<EvidenciaResponse>();
                // Obtener el primer seguimento.
                var primerSeguimento = await _context.Seguimentos
                    .Include(x=> x.Evidencia)
                    .Include(x => x.RelSeguimentoProductos).ThenInclude(x=> x.IdProductoNavigation)
                    .FirstOrDefaultAsync(x => x.IdReporteServicio == reporteServicio.Id);

                if (primerSeguimento != null) {
                    // Llenar Productos.
                    foreach (var item in primerSeguimento.RelSeguimentoProductos)
                    {
                        reporteServicio.Productos.Add(new RelSeguimentoProductoResponse()
                        {
                            Cantidad = item.Cantidad,
                            FechaCreacion = item.FechaCreacion,
                            FechaModificacion = item.FechaModificacion,
                            Id = item.IdProducto,
                            IdCatEstatus = item.IdCatEstatus,
                            IdProducto = item.IdProducto,
                            IdSeguimento = item.IdSeguimento,
                            IdUsuario = item.IdUsuario,
                            MontoGasto = item.MontoGasto,
                            MontoVenta = item.MontoVenta,
                            Codigo = item.IdProductoNavigation.Codigo,
                            Producto = item.IdProductoNavigation.Producto1, 
                            Unidad = item.Unidad, 
                            Porcentaje = item.Porcentaje,
                        });
                    }

                    // Llenar Evidencias.
                    foreach (var item in primerSeguimento.Evidencia)
                    {
                        
                        var base64String = await _utilidades.ObtenerBase64Async(_utilidades.GetPhysicalPath(item.Ruta));
                        reporteServicio.Evidencias.Add(new EvidenciaResponse()
                        {
                            Extension = item.Extension,
                            FechaCreacion = item.FechaCreacion,
                            FechaModificacion = item.FechaModificacion,
                            Id = item.Id,
                            IdCatEstatus = item.IdCatEstatus,
                            IdSeguimento = item.IdSeguimento,
                            Nombre = item.Nombre,
                            Ruta = _utilidades.GetFullUrl(item.Ruta),
                            Base64 = base64String,
                            Size = _utilidades.GetBase64SizeInKB(base64String)
                        });
                    }

                    // Proxima Visita.
                    reporteServicio.ProximaVisita = primerSeguimento.ProximaVisita;
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

                request.IdCatSolicitud = 1;
               
                // Obtener el ID del usuario conectado
                var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!long.TryParse(nameIdentifier, out long userId))
                {
                    return Conflict(new DefaultResponse<object> { Message = "No se tiene permisos para esta acción." });
                }

                request.IdUsuarioCreacion = userId;

                // Validar existencia de entidades relacionadas
                if (!await _context.CatTipoSolicitudes.AnyAsync(m => m.Id == request.IdCatSolicitud))
                {
                    return Conflict(new DefaultResponse<object> { Message = "El tipo de solicitud no existe o no se encontró." });
                }

                if (!await _context.Usuarios.AnyAsync(m => m.Id == request.IdUsuarioCreacion))
                {
                    return Conflict(new DefaultResponse<object> { Message = "No se encontró el usuario." });
                }

                if (!await _context.Clientes.AnyAsync(m => m.Id == request.IdCliente))
                {
                    return Conflict(new DefaultResponse<object> { Message = "El cliente no existe o no se encontró." });
                }

                //if (!await _context.Usuarios.AnyAsync(m => m.Id == request.IdUsuarioTecnico))
                //{
                //    return Conflict(new DefaultResponse<object> { Message = "El técnico no existe o no se encontró." });
                //}

                // Crear el nuevo reporte de servicio
                var nuevo = new ReporteServicio
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
                    // IdUsuarioTecnico = request.IdUsuarioTecnico,
                    UsuarioTecnico = request.UsuarioTecnico,
                    UsuarioEncargado = request.UsuarioEncargado,
                };
                await _context.ReporteServicios.AddAsync(nuevo);
                await _context.SaveChangesAsync();

                // Crear el primer seguimiento
                var primerSeguimiento = new Seguimento
                {
                    IdReporteServicio = nuevo.Id,
                    IdUsuario = userId,
                    Seguimento1 = _configuration["ValoresDefault:NombrePrimerSeguimento"] ?? "",
                    IdCatEstatus = 1,
                    ProximaVisita = request.ProximaVisita,
                    DescripcionProximaVisita = request.DescripcionProximaVisita,
                };
                await _context.Seguimentos.AddAsync(primerSeguimiento);
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
                            IdSeguimento = primerSeguimiento.Id,
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
                                $"/Evidencias/Seguimento_{primerSeguimiento.Id}", 
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
                            IdSeguimento = primerSeguimiento.Id,
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
        
        [HttpPost]
        [Route("Actualizar")]
        public async Task<ActionResult> ActualizarReporteServicio(ActualizarReporteServicioRequest request)
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

                // Verificar existencia del reporte
                var reporte = await _context.ReporteServicios
                    .Include(r => r.Seguimentos).ThenInclude(r=> r.Evidencia)
                    .FirstOrDefaultAsync(r => r.Id == request.Id);

                if (reporte == null)
                {
                    return Conflict(new DefaultResponse<object> { Message = "El reporte no existe o no se encontró." });
                }

                // Validar existencia de entidades relacionadas
                if (!await _context.Clientes.AnyAsync(m => m.Id == request.IdCliente))
                {
                    return Conflict(new DefaultResponse<object> { Message = "El cliente no existe o no se encontró." });
                }

                //if (!await _context.Usuarios.AnyAsync(m => m.Id == request.IdUsuarioTecnico))
                //{
                //    return Conflict(new DefaultResponse<object> { Message = "El técnico no existe o no se encontró." });
                //}

                // Actualizar los datos del reporte
                reporte.IdCliente = request.IdCliente;
                reporte.Titulo = request.Titulo;
                reporte.Descripcion = request.Descripcion;
                reporte.ServicioPreventivo = request.ServicioPreventivo;
                reporte.ServicioCorrectivo = request.ServicioCorrectivo;
                reporte.ObservacionesRecomendaciones = request.ObservacionesRecomendaciones;
                reporte.UsuarioEncargado = request.UsuarioEncargado;
                reporte.FechaInicio = request.FechaInicio;
                reporte.Accesorios = request.Accesorios;
                //reporte.IdUsuarioModificacion = userId;
                reporte.FechaModificacion = DateTime.UtcNow;
                reporte.UsuarioTecnico = request.UsuarioTecnico;

                _context.ReporteServicios.Update(reporte);
                await _context.SaveChangesAsync();

                // Validar y actualizar productos relacionados
                var seguimiento = reporte.Seguimentos.FirstOrDefault();
                if (seguimiento != null && request.Productos != null && request.Productos.Any())
                {
                    var productosActuales = await _context.RelSeguimentoProductos
                        .Where(r => r.IdSeguimento == seguimiento.Id)
                        .ToListAsync();

                    _context.RelSeguimentoProductos.RemoveRange(productosActuales);
                    await _context.SaveChangesAsync();

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

                        var nuevoProducto = new RelSeguimentoProducto
                        {
                            //IdSeguimento = seguimiento.Id,
                            //IdProducto = producto.Id,
                            //IdUsuario = userId,
                            //IdCatEstatus = 1,
                            //Cantidad = producto.Cantidad,
                            //Unidad = "",
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

                        await _context.RelSeguimentoProductos.AddAsync(nuevoProducto);
                    }
                    await _context.SaveChangesAsync();
                }

                // Procesar evidencias
                if (request.Evidencias != null && request.Evidencias.Any())
                {
                    var evidenciasActuales = await _context.Evidencias
                        .Where(e => e.IdSeguimento == seguimiento!.Id)
                        .ToListAsync();

                    _context.Evidencias.RemoveRange(evidenciasActuales);
                    await _context.SaveChangesAsync();

                    foreach (var evidenciaRS in request.Evidencias)
                    {
                        string rutaEvidencia;
                        try
                        {
                            rutaEvidencia = await _utilidades.GuardarArchivoBase64Async(
                                $"/Evidencias/Seguimento_{seguimiento!.Id}",
                                evidenciaRS.Extension,
                                evidenciaRS.Base64
                            );
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            return Conflict(new DefaultResponse<object> { Message = $"Error al guardar evidencia: {ex.Message}" });
                        }

                        var nuevaEvidencia = new Evidencia
                        {
                            IdSeguimento = seguimiento.Id,
                            Nombre = evidenciaRS.Nombre,
                            Extension = evidenciaRS.Extension,
                            Ruta = rutaEvidencia,
                            IdCatEstatus = 1,
                        };

                        await _context.Evidencias.AddAsync(nuevaEvidencia);
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return Ok(new DefaultResponse<object>
                {
                    Success = true,
                    Message = "Reporte actualizado correctamente."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new DefaultResponse<object> { Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("BusquedaSeguimentoActivo")]
        public async Task<IActionResult> BusquedaSeguimentoActivo(BusquedaGenericoRequest request)
        {
            // Obtener el ID del usuario conectado
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(nameIdentifier, out long userId))
            {
                return Conflict(new DefaultResponse<object> { Message = "No se tiene permisos para esta acción." });
            }

            // Construir la consulta inicial
            var query = _context.ReporteServicios
                .Include(x => x.IdCatEstatusNavigation)
                .AsQueryable();

            query = query.Where(
                x => x.IdCatSolicitud == 1 
                && x.IdCatEstatus == 3 
                // && x.IdUsuarioCreacion == userId
            );

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
                    // IdUsuarioTecnico = x.IdUsuarioTecnico,
                    UsuarioEncargado = x.UsuarioEncargado,
                    Estatus = x.IdCatEstatusNavigation.Estatus,
                    UsuarioCreacion = (x.IdUsuarioCreacionNavigation.Nombres + " " + x.IdUsuarioCreacionNavigation.Apellidos),
                    Cliente = x.IdClienteNavigation.Nombre,
                    CatSolicitud = x.IdCatSolicitudNavigation.TipoSolicitud,
                    // UsuarioTecnico = (x.IdUsuarioTecnicoNavigation.Nombres + " " + x.IdUsuarioTecnicoNavigation.Apellidos),
                    UsuarioTecnico = x.UsuarioTecnico,
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

        [HttpPost]
        [Route("CambiarEstatusEnSeguimento")]
        public async Task<ActionResult> CambiarEstatus(CambiarEstatusEnSeguimentoRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(DefaultResponse<List<string>>.FromModelState(ModelState));
                }

                var reporte = await _context.ReporteServicios.FirstOrDefaultAsync(r => r.Id == request.Id);
                if (reporte == null)
                {
                    return Conflict(new DefaultResponse<object> { Message = "El reporte no existe o no se encontró." });
                }

                // Actualizar los datos
                reporte.IdCatEstatus = 3;
                reporte.FechaInicio = reporte.FechaInicio ?? DateTime.Now;

                // Guardar los cambios
                _context.ReporteServicios.Update(reporte);
                await _context.SaveChangesAsync();

                return Ok(new DefaultResponse<object>
                {
                    Success = true,
                    Message = "Reporte actualizado correctamente."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new DefaultResponse<object> { Message = ex.Message });
            }
        }
    }
}
