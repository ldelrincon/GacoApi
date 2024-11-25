using gaco_api.Customs;
using gaco_api.Models;
using gaco_api.Models.DTOs.Requests.Clientes;
using gaco_api.Models.DTOs.Responses;
using gaco_api.Models.DTOs.Responses.Clientes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gaco_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly GacoDbContext _context;

        public ClientesController(GacoDbContext context)
        {
            _context = context;
        }

        // GET: api/Clientes
        [HttpGet]
        [Route("ListaClientes")]
        public async Task<ActionResult> GetClientes()
        {
            var clientes = await _context.Clientes
                .Select(c => new ClienteResponse {
                    Id = c.Id,
                    Codigo = c.Codigo,
                    CodigoPostal = c.CodigoPostal,
                    Correo = c.Correo,
                    Direccion = c.Direccion,
                    FechaCreacion = c.FechaCreacion,
                    FechaModificacion= c.FechaModificacion,
                    IdCatEstatus = c.IdCatEstatus,
                    IdCatMunicipio = c.IdCatMunicipio,
                    IdRegimenFiscal = c.IdRegimenFiscal,
                    Nombre = c.Nombre,
                    RazonSocial = c.RazonSocial,
                    Rfc = c.Rfc,
                    Telefono = c.Telefono
                }).ToListAsync();

            var response = new DefaultResponse<List<ClienteResponse>>
            {
                Success = true,
                Data = clientes,
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("ListaCatalogoClientes")]
        public async Task<ActionResult> ListaCatalogoClientes()
        {
            var response = await _context.Clientes
                .Select(c => new ClienteCatalogoResponse
                {
                    Id = c.Id,
                    Codigo = c.Codigo,
                    Nombre = c.Nombre,
                }).ToListAsync();

            return Ok(new DefaultResponse<List<ClienteCatalogoResponse>>
            {
                Success = true,
                Data = response,
            });
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(long id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        [HttpPost]
        [Route("Nuevo")]
        public async Task<ActionResult> NuevoCliente(NuevoClienteRequest request)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(DefaultResponse<List<string>>.FromModelState(ModelState));
                //}

                // Validar si ya existe el correo que se quiere registrar.
                var existeCorreo = await _context.Clientes.AnyAsync(m => m.Correo == request.Correo);
                if (existeCorreo)
                {
                    return Conflict(new DefaultResponse<object> { Message = "El correo ingresado ya está registrado." });
                }

                var existeRFC = await _context.Clientes.AnyAsync(m => m.Rfc == request.Rfc);
                if (existeRFC)
                {
                    return Conflict(new DefaultResponse<object> { Message = "El RFC ingresado ya está registrado." });
                }

                var nuevoCliente = new Cliente()
                {
                    Telefono = request.Telefono,
                    Rfc = request.Rfc,
                    Direccion = request.Direccion,
                    IdCatEstatus = 1,
                    Nombre = request.Nombre,
                    Codigo = request.Codigo,
                    IdCatMunicipio = request.IdCatMunicipio,
                    CodigoPostal = request.CodigoPostal,
                    RazonSocial = request.RazonSocial,
                    IdRegimenFiscal = request.IdRegimenFiscal,
                    Correo = request.Correo
                };

                await _context.Clientes.AddAsync(nuevoCliente);
                await _context.SaveChangesAsync();

                if (nuevoCliente.Id != 0)
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
    }
}
