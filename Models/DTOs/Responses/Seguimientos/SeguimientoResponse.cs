using gaco_api.Models.DTOs.Responses.Evidencias;
using gaco_api.Models.DTOs.Responses.Productos;
using gaco_api.Models.DTOs.Responses.Relaciones;

namespace gaco_api.Models.DTOs.Responses.Seguimientos
{
    public class SeguimientoResponse
    {
        public long Id { get; set; }

        public long IdReporteServicio { get; set; }
        public string? TituloReporteServicio { get; set; }

        public long IdUsuario { get; set; }
        public string? Usuario { get; set; }

        public string Seguimento1 { get; set; } = null!;

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public int IdCatEstatus { get; set; }
        public string? Estatus { get; set; }

        public DateTime? ProximaVisita { get; set; }

        public string? DescripcionProximaVisita { get; set; }

        public List<RelSeguimentoProductoResponse>? Productos { get; set; }
        public List<EvidenciaResponse>? Evidencias { get; set; }
    }
}
