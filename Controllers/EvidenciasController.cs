using gaco_api.Models;
using gaco_api.Models.DTOs.Responses.Productos;
using gaco_api.Models.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gaco_api.Models.DTOs.Responses.Evidencias;
using gaco_api.Customs;

namespace gaco_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EvidenciasController : ControllerBase
    {
        private readonly GacoDbContext _context;
        private readonly Utilidades _utilidades;

        public EvidenciasController(GacoDbContext context, Utilidades utilidades)
        {
            _context = context;
            _utilidades = utilidades;
        }

        [HttpGet]
        [Route("PorId/{id}")]
        public async Task<IActionResult> EvidenciaPorId(long id)
        {
            var response = new DefaultResponse<EvidenciaResponse>();

            var evidencia = await _context.Evidencias
                .Where(x => x.Id == id)
                .Select(item => new
                {
                    item.Extension,
                    item.FechaCreacion,
                    item.FechaModificacion,
                    item.Id,
                    item.IdCatEstatus,
                    item.IdSeguimento,
                    item.Nombre,
                    item.Ruta
                })
                .FirstOrDefaultAsync();

            if (evidencia == null)
            {
                response.Success = false;
                response.Message = "Evidencia no encontrada";
                return NotFound(response);
            }

            // Convertir la ruta a Base64 después de obtener el resultado de la BD
            var base64 = await _utilidades.ObtenerBase64Async(_utilidades.GetPhysicalPath(evidencia.Ruta));

            response = new DefaultResponse<EvidenciaResponse>
            {
                Success = true,
                Data = new EvidenciaResponse
                {
                    Extension = evidencia.Extension ?? "",
                    FechaCreacion = evidencia.FechaCreacion,
                    FechaModificacion = evidencia.FechaModificacion,
                    Id = evidencia.Id,
                    IdCatEstatus = evidencia.IdCatEstatus,
                    IdSeguimento = evidencia.IdSeguimento,
                    Nombre = evidencia.Nombre,
                    Ruta = evidencia.Ruta,
                    Base64 = base64
                }
            };

            return Ok(response);
        }

    }
}
