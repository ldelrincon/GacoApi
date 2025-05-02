namespace gaco_api.Models.DTOs.Requests.ReporteSolicitudes
{
    public class BusquedaReporteFiltrosServicioRequest : BasePaginadorRequest
    {
        public BusquedaFiltrosRequest Busqueda { get; set; }
    }
}
