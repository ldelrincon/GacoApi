using gaco_api.Customs;
using gaco_api.Models;
using gaco_api.Models.DTOs.Requests;
using gaco_api.Models.DTOs.Responses;
using Microsoft.AspNetCore.Http;
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
                        TipoUsuario = m.IdCatTipoUsuario,
                        NombreCompleto = $"{m.Nombres} {m.Apellidos}",
                        Correo = m.Correo,
                        CorreoConfirmado = m.CorreoConfirmado,
                        FechaCreacion = m.FechaCreacion,
                        Estatus = m.IdCatEstatus,
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
                        TokenModel = new
                        {
                            accessToken = _utilidades.GenerarJWT(usuarioDTO),
                            tokenType = "Bearer",
                            expiresIn = DateTime.UtcNow.AddHours(1).ToUniversalTime(),
                            issuedAt = DateTime.UtcNow,
                            // refreshToken = _utilidades.GenerateRefreshToken(),
                        },
                        Usuario = usuarioDTO
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
