using gaco_api.Customs;
using gaco_api.Models;
using gaco_api.Models.DTOs.Requests.Clientes;
using gaco_api.Models.DTOs.Responses;
using gaco_api.Models.DTOs.Responses.Clientes;
using gaco_api.Models.DTOs.Responses.Usuarios;
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

        [HttpPost]
        [Route("Busqueda")]
        public async Task<IActionResult> BusquedaCliente(BusquedaClienteRequest request)
        {
            // Construir la consulta inicial
            var query = _context.Clientes
                .Include(u => u.IdCatMunicipioNavigation)
                .Include(u => u.IdRegimenFiscalNavigation)
                .Include(u => u.IdCatEstatusNavigation)
                .AsQueryable();

            // Filtrar si hay una búsqueda
            if (!string.IsNullOrEmpty(request.Busqueda))
            {
                query = query.Where(u => u.Nombre.Contains(request.Busqueda)
                    || u.RazonSocial.Contains(request.Busqueda)
                    || u.Rfc.Contains(request.Busqueda)
                    || u.Telefono.Contains(request.Busqueda)
                    || u.Correo.Contains(request.Busqueda)
                );
            }

            // Seleccionar y aplicar paginación
            var clientes = await query
                .Select(x => new ClienteResponse
                {
                    Id = x.Id,
                    Codigo = x.Codigo,
                    CodigoPostal = x.CodigoPostal,
                    Correo = x.Correo,
                    Direccion = x.Direccion,
                    FechaCreacion = x.FechaCreacion,
                    FechaModificacion = x.FechaModificacion,
                    IdCatEstatus = x.IdCatEstatus,
                    IdCatMunicipio = x.IdCatMunicipio,
                    IdRegimenFiscal = x.IdRegimenFiscal,
                    Nombre = x.Nombre,
                    RazonSocial = x.RazonSocial,
                    Rfc = x.Rfc,
                    Telefono = x.Telefono, 
                    Estatus = x.IdCatEstatusNavigation.Estatus
                })
                .Skip((request.NumeroPagina - 1) * request.CantidadPorPagina)
                .Take(request.CantidadPorPagina)
                .ToListAsync();

            // Crear la respuesta
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

        [HttpGet]
        [Route("PorId/{id}")]
        public async Task<IActionResult> ClientePorId(long id)
        {
            var response = new DefaultResponse<EditarClienteResponse>();

            // Seleccionar y aplicar paginación
            var cliente = await _context.Clientes
                .Include(x => x.IdCatMunicipioNavigation)
                .Where(x => x.Id == id)
                .Select(x => new EditarClienteResponse
                {
                    Id = x.Id,
                    Codigo = x.Codigo,
                    CodigoPostal = x.CodigoPostal,
                    Correo = x.Correo,
                    FechaModificacion = x.FechaModificacion,
                    Direccion = x.Direccion,
                    FechaCreacion = x.FechaCreacion,
                    IdCatEstatus = x.IdCatEstatus,
                    IdCatMunicipio = x.IdCatMunicipio,
                    IdRegimenFiscal = x.IdRegimenFiscal,
                    Nombre = x.Nombre,
                    RazonSocial = x.RazonSocial,
                    Rfc = x.Rfc,
                    Telefono = x.Telefono,
                    EfeKey = x.IdCatMunicipioNavigation.EfeKey
                }).FirstOrDefaultAsync();

            if (cliente != null)
            {
                response = new DefaultResponse<EditarClienteResponse>
                {
                    Success = true,
                    Data = cliente,
                };
            }
            else
            {
                response = new DefaultResponse<EditarClienteResponse>
                {
                    Success = false,
                    Message = "No se encontro el cliente."
                };
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("Nuevo")]
        public async Task<ActionResult> NuevoCliente(NuevoClienteRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(DefaultResponse<List<string>>.FromModelState(ModelState));
                }

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

        [HttpPut]
        [Route("Actualizar")]
        public async Task<ActionResult> ActualizarCliente(ActualizarClienteRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(DefaultResponse<List<string>>.FromModelState(ModelState));
                }

                // Buscar al cliente existente por su ID
                var cliente = await _context.Clientes.FindAsync(request.Id);
                if (cliente == null)
                {
                    return NotFound(new DefaultResponse<object> { Message = "Cliente no encontrado." });
                }

                // Validar si el correo ya está registrado por otro cliente
                var existeCorreo = await _context.Clientes.AnyAsync(m => m.Correo == request.Correo && m.Id != request.Id);
                if (existeCorreo)
                {
                    return Conflict(new DefaultResponse<object> { Message = "El correo ingresado ya está registrado por otro cliente." });
                }

                // Validar si el RFC ya está registrado por otro cliente
                var existeRFC = await _context.Clientes.AnyAsync(m => m.Rfc == request.Rfc && m.Id != request.Id);
                if (existeRFC)
                {
                    return Conflict(new DefaultResponse<object> { Message = "El RFC ingresado ya está registrado por otro cliente." });
                }

                // Actualizar los datos del cliente
                cliente.Telefono = request.Telefono;
                cliente.Rfc = request.Rfc;
                cliente.Direccion = request.Direccion;
                cliente.IdCatEstatus = request.IdCatEstatus;
                cliente.Nombre = request.Nombre;
                cliente.Codigo = request.Codigo;
                cliente.IdCatMunicipio = request.IdCatMunicipio;
                cliente.CodigoPostal = request.CodigoPostal;
                cliente.RazonSocial = request.RazonSocial;
                cliente.IdRegimenFiscal = request.IdRegimenFiscal;
                cliente.Correo = request.Correo;

                // Guardar los cambios
                _context.Clientes.Update(cliente);
                await _context.SaveChangesAsync();

                return Ok(new DefaultResponse<object>
                {
                    Success = true,
                    Message = "Cliente actualizado correctamente."
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
