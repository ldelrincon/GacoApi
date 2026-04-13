using gaco_api.Customs;
using gaco_api.Models;
using gaco_api.Models.DTOs.Requests.Calendario;
using gaco_api.Models.DTOs.Responses;
using gaco_api.Models.DTOs.Responses.Calendario;
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
    public class CalendarioController : ControllerBase
    {
        private readonly GacoDbContext _context;
        private readonly Utilidades _utilidades;
        private readonly NotificacionCorreo _NotificacionCorreo;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public CalendarioController(GacoDbContext context, Utilidades utilidades, NotificacionCorreo NotificacionCorreo, IConfiguration configuration, IWebHostEnvironment env)
        {
            _context = context;
            _utilidades = utilidades;
            _NotificacionCorreo = NotificacionCorreo;
            _configuration = configuration;
            _env = env;
        }

        [HttpPost]
        [Route("Nuevo")]
        public async Task<ActionResult> NuevaTarea(AddTarea request)
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

                request.IdUsuarioCreacion = userId;

                if (!await _context.Usuarios.AnyAsync(m => m.Id == request.IdUsuarioCreacion))
                {
                    return Conflict(new DefaultResponse<object> { Message = "No se encontró el usuario." });
                }

                var nuevo = new Calendario
                {
                    Descripcion = request.Descripcion,
                    IdUsuarioCreacion = request.IdUsuarioCreacion,
                    IdUsuarioTarea = request.IdUsuarioTarea,
                    FechaTarea = request.FechaTarea,
                  
                };
                await _context.Calendario.AddAsync(nuevo);
                await _context.SaveChangesAsync();

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

        [HttpGet]
        [Route("ListaPendientes")]
        public async Task<IActionResult> ListaPendientes()
        {
            var response = new DefaultResponse<List<CalendarioResponse>>();

            try
            {
                var tareas = await _context.Calendario
                    .Include(c => c.IdUsuarioTareaNavigation)
                    .Where(c => c.Terminado == false || c.Terminado == null)
                    .OrderBy(c => c.FechaTarea)
                    .Select(c => new CalendarioResponse
                    {
                        IdCalendario = c.IdCalendario,
                        Descripcion = c.Descripcion,
                        FechaTarea = c.FechaTarea,
                        IdUsuarioTarea = c.IdUsuarioTarea,
                        UsuarioTarea = c.IdUsuarioTareaNavigation != null
                            ? (c.IdUsuarioTareaNavigation.Nombres + " " + c.IdUsuarioTareaNavigation.Apellidos)
                            : string.Empty,
                        FechaCreacion = c.FechaCreacion,
                        IdUsuarioCreacion = c.IdUsuarioCreacion,
                        Terminado = c.Terminado,
                        FechaTerminado = c.FechaTerminado
                    })
                    .ToListAsync();

                response.Success = true;
                response.Data = tareas;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("Terminar")]
        public async Task<IActionResult> TerminarTarea([FromBody] TerminarCalendarioRequest request)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(DefaultResponse<List<string>>.FromModelState(ModelState));
                }

                var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!long.TryParse(nameIdentifier, out long userId))
                {
                    return Conflict(new DefaultResponse<object> { Message = "No se tiene permisos para esta acción." });
                }

                var calendario = await _context.Calendario
                    .FirstOrDefaultAsync(c => c.IdCalendario == request.IdCalendario);

                if (calendario == null)
                {
                    return NotFound(new DefaultResponse<object> { Message = "No se encontró la tarea del calendario." });
                }

                // Actualizar campos solicitados
                calendario.IdUsuarioCreacion = userId;
                calendario.Terminado = true; // equivalente a 1
                calendario.FechaTerminado = DateTime.Now;

                _context.Calendario.Update(calendario);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new DefaultResponse<object>
                {
                    Success = true,
                    Message = "Tarea marcada como terminada correctamente."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new DefaultResponse<object> { Message = ex.Message });
            }
        }
    }
}
