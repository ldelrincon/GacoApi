using gaco_api.Models;
using gaco_api.Models.DTOs.Responses.Clientes;
using gaco_api.Models.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gaco_api.Models.DTOs.Responses.Usuarios;
using gaco_api.Models.DTOs.Requests.Usuarios;
using System.Linq;
using gaco_api.Customs;

namespace gaco_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly GacoDbContext _context;
        private readonly Utilidades _utilidades;

        public UsuariosController(GacoDbContext context, Utilidades utilidades)
        {
            _context = context;
            _utilidades = utilidades;
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

        [HttpPost]
        [Route("Nuevo")]
        public async Task<IActionResult> NuevoUsuario(UsuarioRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(DefaultResponse<List<string>>.FromModelState(ModelState));
                }

                // Validar si existe el tipo de usuario.
                var existeTipoUsuario = await _context.CatTipoUsuarios.AnyAsync(m => m.Id == request.TipoUsuario);
                if (!existeTipoUsuario)
                {
                    return Ok(new DefaultResponse<object> { Message = "Tipo de usuario no encontrado." });
                }

                // Validar si ya existe el correo que se quiere registrar.
                var existeCorreo = await _context.Usuarios.AnyAsync(m => m.Correo == request.Correo);
                if (existeCorreo)
                {
                    return Ok(new DefaultResponse<object> { Message = "El correo ingresado ya está registrado." });
                }

                if(request.Contrasena != request.ConfirmarContrasena)
                {
                    return Ok(new DefaultResponse<object> { Message = "Contraseña no concuerdan." });
                }

                var usuarioModel = new Usuario()
                {
                    IdCatTipoUsuario = request.TipoUsuario,
                    Correo = request.Correo,
                    Contrasena = _utilidades.EncriptarSHA256(request.Contrasena),
                    Nombres = request.Nombre,
                    Apellidos = request.Apellidos,
                    Telefono = request.Telefono,
                    IdCatEstatus = 1
                };

                await _context.Usuarios.AddAsync(usuarioModel);
                await _context.SaveChangesAsync();

                if (usuarioModel.Id != 0)
                {
                    return Ok(new DefaultResponse<object>
                    {
                        Success = true,
                        Message = "Usuario registrado correctamente."
                    });
                }
                return BadRequest(new DefaultResponse<object>
                {
                    Success = false,
                    Message = "Error al registrar el usuario."
                });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new DefaultResponse<object> { Message = ex.Message });
            }
        }

        [HttpGet]
        [Route("PorId/{id}")]
        public async Task<IActionResult> UsuarioPorId(long id)
        {
            var response = new DefaultResponse<EditarUsuarioResponse>();

            // Seleccionar y aplicar paginación
            var usuario = await _context.Usuarios
                .Where(u => u.Id == id)
                .Select(ur => new EditarUsuarioResponse
                {
                    Id = ur.Id,
                    IdCatTipoUsuario = ur.IdCatTipoUsuario,
                    Apellidos = ur.Apellidos,
                    Correo = ur.Correo,
                    Nombres = ur.Nombres,
                    IdCatEstatus = ur.IdCatEstatus,
                    Telefono = ur.Telefono,
                    FechaCreacion = ur.FechaCreacion,
                    FechaModificacion = ur.FechaModificacion,
                    CorreoConfirmado = ur.CorreoConfirmado,
                    Contrasena = string.Empty
                }).FirstOrDefaultAsync();

            if(usuario != null)
            {
                response = new DefaultResponse<EditarUsuarioResponse>
                {
                    Success = true,
                    Data = usuario,
                };
            }
            else
            {
                response = new DefaultResponse<EditarUsuarioResponse>
                {
                    Success = false,
                    Message = "No se encontro el usuario."
                };
            }

            return Ok(response);
        }
    }
}
