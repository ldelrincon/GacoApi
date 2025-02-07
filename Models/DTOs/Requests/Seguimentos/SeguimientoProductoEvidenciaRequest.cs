using gaco_api.Models.DTOs.Requests.Evidencias;
using gaco_api.Models.DTOs.Requests.Productos;

namespace gaco_api.Models.DTOs.Requests.Seguimentos
{
    public class SeguimientoProductoEvidenciaRequest
    {
        public string Seguimiento { get; set; }
        public long IdReporteServicio { get; set; }
        public DateTime? ProximaVisita { get; set; }
        public string? DescripcionProximaVisita { get; set; }
        public List<ProductoReporteServicioRequest>? Productos { get; set; }
        public List<EvidenciaReporteServicioRequest>? Evidencias { get; set; }
    }
}
