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
        public int IdCatEstatus { get; set; }
        public DateTime? FechaInicio { get; set; }
        public string Accesorios { get; set; } = null!;
        public bool ServicioPreventivo { get; set; }
        public bool ServicioCorrectivo { get; set; }
        public string ObservacionesRecomendaciones { get; set; } = null!;
        public long IdUsuarioTecnico { get; set; }
        public string UsuarioEncargado { get; set; } = null!;
    }
}
