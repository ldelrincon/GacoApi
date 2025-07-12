using gaco_api.Models.DTOs.Requests.ReporteSolicitudes;

namespace gaco_api.Models.DTOs.Requests.Clientes
{
    public class BusquedaClienteRequest : BasePaginadorRequest
    {
        public BusquedaFiltrosClienteRequest Busqueda { get; set; }
    }
}
