using gaco_api.Models;
using gaco_api.Models.DTOs.Responses.Clientes;
using gaco_api.Models.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gaco_api.Models.DTOs.Responses.Usuarios;
using gaco_api.Models.DTOs.Requests.Usuarios;
using System.Linq;

namespace gaco_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly GacoDbContext _context;

        public UsuariosController(GacoDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Busqueda")]
        public async Task<IActionResult> BusquedaUsuario(BusquedaUsuarioRequest request)
        {
            // Construir la consulta inicial
            var query = _context.Usuarios
                .Include(u => u.IdCatTipoUsuarioNavigation)
                .Include(u => u.IdCatEstatusNavigation)
                .AsQueryable();

            // Filtrar si hay una búsqueda
            if (!string.IsNullOrEmpty(request.Busqueda))
            {
                query = query.Where(u => u.Correo.Contains(request.Busqueda) || u.Nombres.Contains(request.Busqueda));
            }

            // Seleccionar y aplicar paginación
            var usuarios = await query
                .Select(ur => new UsuarioResponse
                {
                    Id = ur.Id,
                    IdTipoUsuario = ur.IdCatTipoUsuario,
                    TipoUsuario = ur.IdCatTipoUsuarioNavigation.TipoUsuario,
                    Usuario = ur.Nombres,
                    Correo = ur.Correo,
                    CorreoConfirmado = ur.CorreoConfirmado,
                    NombreCompleto = ur.Nombres + " " + ur.Apellidos,
                    FechaCreacion = ur.FechaCreacion,
                    IdEstatus = ur.IdCatEstatus,
                    Estatus = ur.IdCatEstatusNavigation.Estatus,
                })
                .Skip((request.NumeroPagina - 1) * request.CantidadPorPagina)
                .Take(request.CantidadPorPagina)
                .ToListAsync();

            // Crear la respuesta
            var response = new DefaultResponse<List<UsuarioResponse>>
            {
                Success = true,
                Data = usuarios,
            };

            return Ok(response);
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(long id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(long id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(long id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(long id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
