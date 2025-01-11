using gaco_api.Models.DTOs.Requests.Evidencias;
using gaco_api.Models.DTOs.Requests.Productos;

namespace gaco_api.Models.DTOs.Requests.ReporteSolicitudes
{
    public class ActualizarReporteServicioRequest
    {
        public long Id { get; set; }
        public int IdCatSolicitud { get; set; }
        public long IdUsuarioCreacion { get; set; }
        public long IdCliente { get; set; }
        public string Titulo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public DateTime? FechaInicio { get; set; }
        public string Accesorios { get; set; } = null!;
        public bool ServicioPreventivo { get; set; }
        public bool ServicioCorrectivo { get; set; }
        public string ObservacionesRecomendaciones { get; set; } = null!;
        // public long IdUsuarioTecnico { get; set; }
        public string UsuarioTecnico { get; set; }
        public string UsuarioEncargado { get; set; } = null!;

        public List<ProductoReporteServicioRequest>? Productos { get; set; }
        public List<EvidenciaReporteServicioRequest>? Evidencias { get; set; }
        public DateTime? ProximaVisita { get; set; }
        public string? DescripcionProximaVisita { get; set; }
    }
}
