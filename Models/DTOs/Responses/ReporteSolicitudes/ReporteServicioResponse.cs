namespace gaco_api.Models.DTOs.Responses.ReporteSolicitudes
{
    public class ReporteServicioResponse
    {
        public long Id { get; set; }

        public int IdCatSolicitud { get; set; }

        public long IdUsuarioCreacion { get; set; }

        public long IdCliente { get; set; }

        public string Titulo { get; set; } = null!;

        public string Descripcion { get; set; } = null!;

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public int IdCatEstatus { get; set; }

        public DateTime? FechaInicio { get; set; }

        public string Accesorios { get; set; } = null!;

        public bool ServicioPreventivo { get; set; }

        public bool ServicioCorrectivo { get; set; }

        public string ObservacionesRecomendaciones { get; set; } = null!;

        public long IdUsuarioTecnico { get; set; }

        public string UsuarioEncargado { get; set; } = null!;

        public string? Estatus { get; set; }
        public string? UsuarioCreacion { get; set; }
        public string? Cliente { get; set; }
        public string? CatSolicitud { get; set; }
        public string? UsuarioTecnico { get; set; }
    }
}
