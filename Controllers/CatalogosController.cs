using gaco_api.Models.DTOs.Requests.Usuarios;
using gaco_api.Models.DTOs.Responses.Usuarios;
using gaco_api.Models.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gaco_api.Models;

namespace gaco_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CatalogosController : ControllerBase
    {
        private readonly GacoDbContext _context;

        public CatalogosController(GacoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("ListaCatTipoUsuarios")]
        public async Task<IActionResult> ListaCatTipoUsuarios()
        {
            var catalogo = await _context.CatTipoUsuarios
                .Select(ctu => new CatTipoUsuarioResponse
                {
                    Id = ctu.Id,
                    Descripcion = ctu.Descripcion,
                    Estatus = ctu.IdCatEstatusNavigation.Estatus,
                    FechaCreacion = ctu.FechaCreacion,
                    FechaModificacion = ctu.FechaModificacion,
                    IdCatEstatus = ctu.IdCatEstatus,
                    TipoUsuario = ctu.TipoUsuario
                }).ToListAsync();

            // Crear la respuesta
            var response = new DefaultResponse<List<CatTipoUsuarioResponse>>
            {
                Success = true,
                Data = catalogo,
            };

            return Ok(response);
        }
    }
}
