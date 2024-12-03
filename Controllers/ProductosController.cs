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

        //// GET: api/Productoes
        //[HttpGet]
        //[Route("Lista")]
        //public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        //{
        //    return await _context.Productos.ToListAsync();
        //}

        //// GET: api/Productoes/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Producto>> GetProducto(long id)
        //{
        //    var producto = await _context.Productos.FindAsync(id);

        //    if (producto == null)
        //    {
        //        return NotFound();
        //    }

        //    return producto;
        //}

        //// PUT: api/Productoes/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutProducto(long id, Producto producto)
        //{
        //    if (id != producto.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(producto).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ProductoExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Productoes
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        //{
        //    _context.Productos.Add(producto);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetProducto", new { id = producto.Id }, producto);
        //}

        //// DELETE: api/Productoes/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteProducto(long id)
        //{
        //    var producto = await _context.Productos.FindAsync(id);
        //    if (producto == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Productos.Remove(producto);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool ProductoExists(long id)
        //{
        //    return _context.Productos.Any(e => e.Id == id);
        //}
    }
}
