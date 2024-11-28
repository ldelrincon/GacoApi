using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using gaco_api.Customs;
using gaco_api.Models;
using gaco_api.Models.DTOs.Requests;
using gaco_api.Models.DTOs.Responses;
using gaco_api.Models.DTOs.Requests.Usuarios;
using gaco_api.Models.DTOs.Responses.Usuarios;

namespace gaco_api.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccesoController : ControllerBase
    {
        private readonly GacoDbContext _context;
        private readonly Utilidades _utilidades;
        private readonly IConfiguration _configuration;

        public AccesoController(GacoDbContext context, Utilidades utilidades, IConfiguration configuration)
        {
            _context = context;
            _utilidades = utilidades;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Registrarse")]
        //[Authorize]
        public async Task<IActionResult> Registrarse(UsuarioRequest usuarioDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(DefaultResponse<List<string>>.FromModelState(ModelState));
                }

                // Validar si existe el tipo de usuario.
                var existeTipoUsuario = await _context.CatTipoUsuarios.AnyAsync(m => m.Id == usuarioDTO.TipoUsuario);
                if (!existeTipoUsuario)
                {
                    return NotFound(new DefaultResponse<object> { Message = "Tipo de usuario no encontrado." });
                }

                // Validar si ya existe el correo que se quiere registrar.
                var existeCorreo = await _context.Usuarios.AnyAsync(m => m.Correo == usuarioDTO.Correo);
                if (existeCorreo)
                {
                    return Conflict(new DefaultResponse<object> { Message = "El correo ingresado ya está registrado." });
                }

                var usuarioModel = new Usuario()
                {
                    IdCatTipoUsuario = usuarioDTO.TipoUsuario,
                    Correo = usuarioDTO.Correo,
                    Contrasena = _utilidades.EncriptarSHA256(usuarioDTO.Contrasena),
                    Nombres = usuarioDTO.Nombre,
                    Apellidos = string.Empty,
                    Telefono = string.Empty,
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

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginRequest loginDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(GenericResponse<List<string>>.FromModelState(ModelState));
                }

                var usuarioDTO = await _context.Usuarios
                .Where(m =>
                        m.Correo == loginDTO.Correo &&
                        m.Contrasena == _utilidades.EncriptarSHA256(loginDTO.Password)
                    )
                    .Select(m => new UsuarioResponse
                    {
                        Id = m.Id,
                        IdCatTipoUsuario = m.IdCatTipoUsuario,
                        Nombres = m.Nombres,
                        Telefono = m.Apellidos,
                        Correo = m.Correo,
                        CorreoConfirmado = m.CorreoConfirmado,
                        FechaCreacion = m.FechaCreacion,
                        IdCatEstatus = m.IdCatEstatus,
                    }).FirstOrDefaultAsync();

                if (usuarioDTO == null)
                {
                    return Unauthorized(new GenericResponse<object>
                    {
                        isError = true,
                        MsgError = "Correo o contraseña incorrectos.",
                        Object = new { Token = string.Empty }
                    });
                }

                return Ok(new GenericResponse<object>
                {
                    isError = false,
                    MsgError = "Login exitoso.",
                    //Object = new
                    //{
                    //    TokenModel = new
                    //    {
                    //        accessToken = _utilidades.GenerarJWT(usuarioDTO),
                    //        tokenType = "Bearer",
                    //        expiresIn = DateTime.UtcNow.AddHours(1).ToUniversalTime(),
                    //        issuedAt = DateTime.UtcNow,
                    //        // refreshToken = _utilidades.GenerateRefreshToken(),
                    //    },
                    //    Usuario = usuarioDTO
                    //}
                    Object = new
                    {
                        Username = string.IsNullOrEmpty($"{usuarioDTO.Nombres} {usuarioDTO.Apellidos}") ? "Anonymous" : $"{usuarioDTO.Nombres} {usuarioDTO.Apellidos}",
                        Correo = usuarioDTO.Correo,
                        Rol = usuarioDTO.TipoUsuario,
                        jwtToken = _utilidades.GenerarJWT(usuarioDTO),
                        IdEmpresa = "",
                        NombreEmpresa = "",

                        Rol2 = ""
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new GenericResponse<object> { MsgError = ex.Message, isError = true });
            }
        }
    }
}
