using gaco_api.Models;
using gaco_api.Models.DTOs.Responses;
using gaco_api.Models.DTOs.Responses.Catalogos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        [Route("ListaCatRegimenFiscales")]
        public async Task<IActionResult> ListaCatRegimenFiscales()
        {
            var response = await _context.CatRegimenFiscales
                .Select(x => new CatRegimenFiscalResponse
                {
                    Id = x.Id,
                    Clave = x.Clave,
                    Descripcion = x.Descripcion,
                    FechaCreacion = x.FechaCreacion,
                    FechaModificacion = x.FechaModificacion,
                    IdCatEstatus = x.IdCatEstatus
                }).ToListAsync();

            return Ok(new DefaultResponse<List<CatRegimenFiscalResponse>>
            {
                Success = true,
                Data = response
            });
        }

        [HttpGet]
        [Route("ListaCatTipoSolicitudes")]
        public async Task<IActionResult> ListaCatTipoSolicitudes()
        {
            var response = await _context.CatTipoSolicitudes
                .Select(x => new CatTipoSolicitudResponse
                {
                    Id = x.Id,
                    Descripcion = x.Descripcion,
                    FechaCreacion = x.FechaCreacion,
                    TipoSolicitud = x.TipoSolicitud,
                    FechaModificacion = x.FechaModificacion,
                    IdCatEstatus = x.IdCatEstatus
                }).ToListAsync();

            return Ok(new DefaultResponse<List<CatTipoSolicitudResponse>>
            {
                Success = true,
                Data = response
            });
        }

        [HttpGet]
        [Route("ListaCatEntidadesFederativas")]
        public async Task<IActionResult> ListaCatEntidadesFederativas()
        {
            var response = await _context.CatEntidadesFederativas
                .Select(x => new CatEntidadesFederativaResponse
                {
                    Id = x.Id,
                    Abreviatura = x.Abreviatura,
                    CatalogKey = x.CatalogKey,
                    EntidadFederativa = x.EntidadFederativa,
                    IdCatEstatus = x.IdCatEstatus
                }).ToListAsync();

            return Ok(new DefaultResponse<List<CatEntidadesFederativaResponse>>
            {
                Success = true,
                Data = response
            });
        }

        [HttpGet]
        [Route("ListaCatMunicipioPorEfeKey/{efeKey}")]
        public async Task<IActionResult> ListaCatMunicipioPorEfeKey(string efeKey)
        {
            var response = await _context.CatMunicipios
                .Where(x => x.EfeKey == efeKey)
                .Select(x => new CatMunicipioResponse
                {
                    Id = x.Id,
                    CatalogKey = x.CatalogKey,
                    EfeKey = x.EfeKey,
                    Estatus = x.Estatus,
                    Municipio = x.Municipio,
                    IdCatEstatus = x.IdCatEstatus
                }).ToListAsync();

            return Ok(new DefaultResponse<List<CatMunicipioResponse>>
            {
                Success = true,
                Data = response
            });
        }

        [HttpGet]
        [Route("ListaCatGrupoProductos")]
        public async Task<IActionResult> ListaCatGrupoProductos()
        {
            var response = await _context.CatGrupoProductos
                .Select(x => new CatGrupoProductosResponse
                {
                    Id = x.Id,
                    Descripcion = x.Descripcion,
                    FechaCreacion = x.FechaCreacion,
                    FechaModificacion = x.FechaModificacion,
                    Grupo = x.Grupo,
                    IdCatEstatus = x.IdCatEstatus
                }).ToListAsync();

            return Ok(new DefaultResponse<List<CatGrupoProductosResponse>>
            {
                Success = true,
                Data = response
            });
        }

        [HttpGet]
        [Route("ListaCatEstatus")]
        public async Task<IActionResult> ListaCatEstatus()
        {
            var catalogo = await _context.CatEstatuses.ToListAsync();

            // Crear la respuesta
            var response = new DefaultResponse<List<CatEstatus>>
            {
                Success = true,
                Data = catalogo,
            };

            return Ok(response);
        }
    }
}