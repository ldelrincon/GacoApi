using gaco_api.Models;
using gaco_api.Models.DTOs.Requests.Clientes;
using gaco_api.Models.DTOs.Responses.Clientes;
using gaco_api.Models.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gaco_api.Models.DTOs.Requests.Productos;
using gaco_api.Models.DTOs.Responses.Productos;

namespace gaco_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly GacoDbContext _context;

        public ProductosController(GacoDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Busqueda")]
        public async Task<IActionResult> BusquedaProducto(BusquedaProductoRequest request)
        {
            // Construir la consulta inicial
            var query = _context.Productos
                .Include(u => u.IdCatGrupoProductoNavigation)
                .Include(u => u.IdCatEstatusNavigation)
                .AsQueryable();

            // Filtrar si hay una búsqueda
            if (!string.IsNullOrEmpty(request.Busqueda))
            {
                query = query.Where(u => u.Codigo.Contains(request.Busqueda)
                    || u.Producto1.Contains(request.Busqueda)
                    || u.IdCatGrupoProductoNavigation.Grupo.Contains(request.Busqueda)
                );
            }

            // Seleccionar y aplicar paginación
            var productos = await query
                .Select(x => new ProductoResponse
                {
                    Id = x.Id,
                    Codigo = x.Codigo,
                    Descripcion = x.Descripcion,
                    FechaCreacion = x.FechaCreacion,
                    FechaModificacion = x.FechaModificacion,
                    GrupoProducto = x.IdCatGrupoProductoNavigation.Grupo,
                    IdCatEstatus = x.IdCatEstatus,
                    IdCatGrupoProducto = x.IdCatGrupoProducto,
                    Producto = x.Producto1,
                    Estatus = x.IdCatEstatusNavigation.Estatus
                })
                .Skip((request.NumeroPagina - 1) * request.CantidadPorPagina)
                .Take(request.CantidadPorPagina)
                .ToListAsync();

            // Crear la respuesta
            var response = new DefaultResponse<List<ProductoResponse>>
            {
                Success = true,
                Data = productos,
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("PorId/{id}")]
        public async Task<IActionResult> ProductoPorId(long id)
        {
            var response = new DefaultResponse<EditarProductoResponse>();

            // Seleccionar y aplicar paginación
            var cliente = await _context.Productos
                .Where(x => x.Id == id)
                .Select(x => new EditarProductoResponse
                {
                    Id = x.Id,
                    Codigo = x.Codigo,
                    Descripcion = x.Descripcion,
                    FechaCreacion = x.FechaCreacion,
                    FechaModificacion = x.FechaModificacion,
                    IdCatEstatus = x.IdCatEstatus,
                    IdCatGrupoProducto = x.IdCatGrupoProducto,
                    Producto = x.Producto1,
                }).FirstOrDefaultAsync();

            if (cliente != null)
            {
                response = new DefaultResponse<EditarProductoResponse>
                {
                    Success = true,
                    Data = cliente,
                };
            }
            else
            {
                response = new DefaultResponse<EditarProductoResponse>
                {
                    Success = false,
                    Message = "No se encontro el producto."
                };
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("Nuevo")]
        public async Task<ActionResult> NuevoProducto(NuevoProductoRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(DefaultResponse<List<string>>.FromModelState(ModelState));
                }

                var existeGrupoProducto = await _context.CatGrupoProductos.AnyAsync(m => m.Id == request.IdCatGrupoProducto);
                if (!existeGrupoProducto)
                {
                    return Conflict(new DefaultResponse<object> { Message = "El grupo de productos no exite o no se encontro." });
                }

                var existeProducto = await _context.Productos.AnyAsync(m => m.Producto1 == request.Producto);
                if (existeProducto)
                {
                    return Conflict(new DefaultResponse<object> { Message = "El producto ya está registrado." });
                }

                var existeCodigo = await _context.Productos.AnyAsync(m => m.Codigo == request.Codigo);
                if (existeCodigo)
                {
                    return Conflict(new DefaultResponse<object> { Message = "El codigo ya está registrado." });
                }

                var nuevoProducto = new Producto()
                {
                    IdCatGrupoProducto = request.IdCatGrupoProducto,
                    Producto1 = request.Producto,
                    Codigo = request.Codigo,
                    Descripcion = request.Descripcion,
                    IdCatEstatus = 1,
                    
                };

                await _context.Productos.AddAsync(nuevoProducto);
                await _context.SaveChangesAsync();

                if (nuevoProducto.Id != 0)
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
        public async Task<ActionResult> ActualizarProducto(ActualizarProductoRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(DefaultResponse<List<string>>.FromModelState(ModelState));
                }

                // Buscar al cliente existente por su ID
                var producto = await _context.Productos.FindAsync(request.Id);
                if (producto == null)
                {
                    return NotFound(new DefaultResponse<object> { Message = "Producto no encontrado." });
                }

                var existeGrupoProducto = await _context.CatGrupoProductos.AnyAsync(m => m.Id == request.IdCatGrupoProducto);
                if (!existeGrupoProducto)
                {
                    return Conflict(new DefaultResponse<object> { Message = "El grupo de productos no exite o no se encontro." });
                }

                var existeProducto = await _context.Productos.AnyAsync(m => m.Producto1 == request.Producto);
                if (existeProducto)
                {
                    return Conflict(new DefaultResponse<object> { Message = "El producto ya está registrado." });
                }

                var existeCodigo = await _context.Productos.AnyAsync(m => m.Codigo == request.Codigo && m.Id != request.Id);
                if (existeCodigo)
                {
                    return Conflict(new DefaultResponse<object> { Message = "El codigo ya está registrado." });
                }

                // Actualizar los datos del produto
                producto.IdCatGrupoProducto = request.IdCatGrupoProducto;
                producto.Codigo = request.Codigo;
                producto.Producto1 = request.Producto;
                producto.Descripcion = producto.Descripcion;
                producto.FechaModificacion = DateTime.Now;

                // Guardar los cambios
                _context.Productos.Update(producto);
                await _context.SaveChangesAsync();

                return Ok(new DefaultResponse<object>
                {
                    Success = true,
                    Message = "Producto actualizado correctamente."
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
