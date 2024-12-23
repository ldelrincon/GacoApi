using gaco_api.Models;
using gaco_api.Models.DTOs.Requests.Clientes;
using gaco_api.Models.DTOs.Responses.Clientes;
using gaco_api.Models.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gaco_api.Models.DTOs.Requests.Productos;
using gaco_api.Models.DTOs.Responses.Productos;
using gaco_api.Models.DTOs.Responses.Catalogos;

namespace gaco_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GrupoProductoController : ControllerBase
    {
        private readonly GacoDbContext _context;

        public GrupoProductoController(GacoDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Busqueda")]
        public async Task<IActionResult> BusquedaGrupoProducto(BusquedaGrupoProductoRequest request)
        {
            // Construir la consulta inicial
            var query = _context.CatGrupoProductos
                .Include(u => u.IdCatEstatusNavigation)
                .AsQueryable();

            // Filtrar si hay una búsqueda
            if (!string.IsNullOrEmpty(request.Busqueda))
            {
                query = query.Where(u => u.Descripcion.Contains(request.Busqueda)
                    || u.Grupo.Contains(request.Busqueda)
                    && u.IdCatEstatus == 1
                );
            }

            if(request.CantidadPorPagina == -1)
            {
                request.CantidadPorPagina = await _context.CatGrupoProductos.CountAsync(x => x.IdCatEstatus == 1);
            }

            // Seleccionar y aplicar paginación
            var Grupoproductos = await query
                .Select(x => new CatGrupoProductosResponse
                {
                    Id = x.Id,
                    Grupo = x.Grupo,
                    Descripcion = x.Descripcion,
                    FechaCreacion = x.FechaCreacion,
                    FechaModificacion = x.FechaModificacion,
                    IdCatEstatus = x.IdCatEstatus,
                   
                })
                .Skip((request.NumeroPagina - 1) * request.CantidadPorPagina)
                .Take(request.CantidadPorPagina)
                .ToListAsync();

            // Crear la respuesta
            var response = new DefaultResponse<List<CatGrupoProductosResponse>>
            {
                Success = true,
                Data = Grupoproductos,
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("PorId/{id}")]
        public async Task<IActionResult> GrupoProductoPorId(long id)
        {
            var response = new DefaultResponse<EditarGrupoProductoResponse>();

            // Seleccionar y aplicar paginación
            var GrupoProducto = await _context.CatGrupoProductos
                .Where(x => x.Id == id)
                .Select(x => new EditarGrupoProductoResponse
                {
                    Id = x.Id,
                    Grupo = x.Grupo,
                    Descripcion = x.Descripcion,
                    FechaCreacion = x.FechaCreacion,
                    FechaModificacion = x.FechaModificacion,
                    IdCatEstatus = x.IdCatEstatus,
                  
                }).FirstOrDefaultAsync();

            if (GrupoProducto != null)
            {
                response = new DefaultResponse<EditarGrupoProductoResponse>
                {
                    Success = true,
                    Data = GrupoProducto,
                };
            }
            else
            {
                response = new DefaultResponse<EditarGrupoProductoResponse>
                {
                    Success = false,
                    Message = "No se encontro el grupo."
                };
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("Nuevo")]
        public async Task<ActionResult> NuevoGrupoProducto(NuevoGrupoProductoRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(DefaultResponse<List<string>>.FromModelState(ModelState));
                }

                var existeGrupoProducto = await _context.CatGrupoProductos.AnyAsync(m => m.Grupo == request.Grupo);
                if (existeGrupoProducto)
                {
                    return Conflict(new DefaultResponse<object> { Message = "El producto ya está registrado." });
                }

                var nuevoGrupoProducto = new CatGrupoProducto()
                {
                    Grupo = request.Grupo,
                    Descripcion = request.Descripcion,
                    IdCatEstatus = 1,
                    
                };

                await _context.CatGrupoProductos.AddAsync(nuevoGrupoProducto);
                await _context.SaveChangesAsync();

                if (nuevoGrupoProducto.Id != 0)
                {
                    return Ok(new DefaultResponse<object>
                    {
                        Success = true,
                        Message = "registrado correctamente."
                    });
                }
                return BadRequest(new DefaultResponse<object>
                {
                    Success = false,
                    Message = "Error al registrarlo."
                });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new DefaultResponse<object> { Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Actualizar")]
        public async Task<ActionResult> ActualizarGrupoProducto(ActualizarGrupoProductoRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(DefaultResponse<List<string>>.FromModelState(ModelState));
                }

                var grupoProducto = await _context.CatGrupoProductos.FindAsync(request.Id);
                if (grupoProducto == null)
                {
                    return NotFound(new DefaultResponse<object> { Message = "Grupo no encontrado." });
                }

                var existeProducto = await _context.CatGrupoProductos.AnyAsync(m => m.Grupo == request.Grupo && m.Id != request.Id);
                if (existeProducto)
                {
                    return Conflict(new DefaultResponse<object> { Message = "El grupo ya está registrado." });
                }

                // Actualizar los datos del grupo
                grupoProducto.Grupo = request.Grupo;
                grupoProducto.Descripcion = request.Descripcion;
                grupoProducto.FechaModificacion = DateTime.Now;

                // Guardar los cambios
                _context.CatGrupoProductos.Update(grupoProducto);
                await _context.SaveChangesAsync();

                return Ok(new DefaultResponse<object>
                {
                    Success = true,
                    Message = "Grupo actualizado correctamente."
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
