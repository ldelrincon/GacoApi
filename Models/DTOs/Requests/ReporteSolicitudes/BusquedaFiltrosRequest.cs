namespace gaco_api.Models.DTOs.Requests.ReporteSolicitudes
{
    public class BusquedaFiltrosRequest
    {
        public string? Cliente { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? Estatus { get; set; }

        public int? TipoSolicitud { get; set; }
    }
}
