using gaco_api.Customs;
using gaco_api.Models;
using gaco_api.Models.DTOs.Requests;
using gaco_api.Models.DTOs.Responses;
using gaco_api.Models.DTOs.Responses.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gaco_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly GacoDbContext _context;
        private readonly Utilidades _utilidades;
        private readonly IConfiguration _configuration;

        public AuthController(GacoDbContext context, Utilidades utilidades, IConfiguration configuration)
        {
            _context = context;
            _utilidades = utilidades;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("loginA")]
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
                        Apellidos = m.Apellidos,
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

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> LoginAcceso(LoginRequest loginDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(DefaultResponse<List<string>>.FromModelState(ModelState));
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
                        Apellidos = m.Apellidos,
                        Correo = m.Correo,
                        CorreoConfirmado = m.CorreoConfirmado,
                        FechaCreacion = m.FechaCreacion,
                        IdCatEstatus = m.IdCatEstatus,
                    }).FirstOrDefaultAsync();

                if (usuarioDTO == null)
                {
                    return Unauthorized(new DefaultResponse<object>
                    {
                        Success = false,
                        Message = "Correo o contraseña incorrectos.",
                        Data = new { Token = string.Empty }
                    });
                }

                return Ok(new DefaultResponse<object>
                {
                    Success = true,
                    Message = "Login exitoso.",
                    Data = new
                    {
                        Username = string.IsNullOrEmpty($"{usuarioDTO.Nombres} {usuarioDTO.Apellidos}") ? "Anonymous" : $"{usuarioDTO.Nombres} {usuarioDTO.Apellidos}",
                        Correo = usuarioDTO.Correo,
                        Rol = usuarioDTO.IdCatTipoUsuario,
                        Token = _utilidades.GenerarJWT(usuarioDTO),
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

        [HttpGet]
        [Route("ValidarToken")]
        public async Task<IActionResult> ValidarToken([FromQuery] string token)
        {
            bool response = _utilidades.ValidarToken(token);
            return Ok(new DefaultResponse<bool> { Success = response, Data = response });
        }
    }
}
